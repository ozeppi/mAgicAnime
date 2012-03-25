//=========================================================================
///	<summary>
///		mAgicAnime番組管理サービス モジュール
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成 Dr.Kurusugawa</history>
/// <history>2010/02/20 古いコメントを削除</history>
/// <history>2010/05/01 Subversionで管理するため不要なコメント削除</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Timers;
using System.Runtime.InteropServices;
using System.Xml;
using System.Net;
using Microsoft.Win32;
using KernelAPI;
using magicAnime.Properties;


namespace magicAnime
{
	[FlagsAttribute]
	public enum updateOption
	{
		Force			= 128,	// 強制更新
	};

	// 自動予約モード
	public enum ReserveControl
	{
		nDaysFromNow,		// データ更新時にn日間の予約
		ImmediatlyBefore,	// 放送直前に予約
		noAutoReserve		// 自動で予約しない
	};

	public enum IdentifyFileMethod
	{
		ByCreateTimeStamp	= 0,
		ByUpdateTimeStamp	= 2,
		ByFileNameWithID	= 1,
	};

	public class UpdatingException : Exception
	{
		public UpdatingException(string mes)
			: base(mes)
		{
		}
	};
	
	//=========================================================================
	///	<summary>
	///		mAgicAnime番組管理サービスクラス
	///	</summary>
	/// <remarks>
	///		放送データの管理から自動再エンコードまで行う。
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	//=========================================================================
	public class AnimeServer : IDisposable
	{
		//-------------------------
		// Singletonインスタンス
		//-------------------------
		private static AnimeServer theInstance;
		public static AnimeServer GetInstance()
		{
			if ( theInstance == null )
				new AnimeServer();

			return theInstance;
		}

		public const int RetryInterval	= 250;	// ネットワーク接続のリトライ間隔

		//---------------------
		// 型定義
		//---------------------
		public class AnimeList : List<AnimeProgram> {};

		// ステータスクラス
		internal struct MyStatus
		{
			internal bool	updateSequenceBusy;	// データ更新シーケンス動作中
			internal string	updateDetail;		// データ更新詳細
			internal string resultLastUpdate;	// 最後のデータ更新に結果
			internal bool	completeUpdate;		// データ更新完了フラグ
		}

		public delegate void ProgressUpdateDelegate(int Phase,int MaxPhase, string d);
		public delegate void UpdateProgress(string Phase, int Perc, string desc);

		// ログ表示デリゲート
		public delegate void LogoutDelegate(string message);
		// データ更新シーケンス実行デリゲート
		private delegate void UpdateProcDelegate(
			updateOption		option		,	// [i] データ更新オプション
			List<AnimeProgram>	prog		,	// [i] 更新対象の番組
			UpdateProgress		onProgress	,	// [i] プログレス表示コールバック
			LogoutDelegate		logout		);	// [i] ログ表示コールバック
		// 「まもなく放送時間」バルーンのポップアップ処理デリゲート
		public delegate void PopupSoonBalloonDelegate( AnimeEpisode e );

		//---------------------
		// メンバ
		//---------------------
		internal SyoboiCalender		mSyoboiCalender = new SyoboiCalender();	// しょぼかるアクセスオブジェクト
		// 放送後再エンコードを行ったエピソードのリスト
		internal List<WeakReference> mAutoEncodedEpisodes = new List<WeakReference>();

		private AnimeList			mAnimeList		= new AnimeList();
		private Thread				mBackGroundSequence;				// バックグラウンドシーケンス用スレッド
		private ManualResetEvent	mPulseSequenceTerminate;			// シーケンス終了要求
		private DateTime?			mPrevSequenceTime = null;			// バックグラウンドシーケンスの前回実行時間
		private bool				mAutoShutdown	= false;			// エンコード完了時に自動シャットダウン
		private bool				mNowEncoding;
		private	DateTime			mUpdateTime		= DateTime.Now;
		private	DateTime?			mBeforeAutoUpdateTime;				// 前回の自動更新時刻
		private FileSystemWatcher	mFileWatcher;						// 録画ディレクトリ監視オブジェクト
		public PopupSoonBalloonDelegate		mPopupSoonBallon;
		private AnimeProgram.EpisodeList	mPopuppedSoonEpisodes;
		private bool				mDoingUpdateSequence = false;		// データ更新シーケンス実行中フラグ
		private BatchManager		mEncodeJobManager;					// エンコードジョブマネージャ
		private BootManager			mBootManager	= new BootManager();// ブートマネージャ
		// 放送時間が重複しているEpisodeのリスト
		private	List<AnimeEpisode>	mDoubleBookingEpisodes = null;
		private Mutex				mLockStatus = new Mutex();
		private MyStatus			mMyStatus;
		// 放送前に強制データ更新したエピソードのリスト
		private class UpdatedSoon
		{
			internal int			mDonePoint;		// 更新ポイント(放送n分前)
			internal WeakReference	mEpisode;		// 対象エピソード
		};
		private List<UpdatedSoon>	mUpdatedSoonList = new List<UpdatedSoon>();

		//---------------------
		// プロパティ
		//---------------------
		public	List<AnimeEpisode> DoubleBookingEpisodes
		{
			get{ return mDoubleBookingEpisodes; }
		}
		public bool AutoShutdown
		{
			get	{ return mAutoShutdown; }
			set { mAutoShutdown = value; }
		}
		public	DateTime	UpdateDateTime		{ get{ return mUpdateTime; } }

		//=========================================================================
		///	<summary>
		///		全番組のリストを返す
		///	</summary>
		/// <remarks>
		///		リストオブジェクトのコピーを返す。
		///		上位がリストを変更してもこちらのデータに影響しない。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public	AnimeList	Animes
		{
			get
			{
				lock(mAnimeList)
				{
					AnimeList coppied = new AnimeList();

					foreach( AnimeProgram prog in mAnimeList )
						coppied.Add( prog );

					return coppied;
				}
			}
		}


		//=========================================================================
		///	<summary>
		///		コンストラクタ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public AnimeServer()
		{
			if ( theInstance != null )
				throw new Exception("既にAnimeServerインスタンスが存在します。");

			theInstance = this;

			mPopuppedSoonEpisodes = new AnimeProgram.EpisodeList();
		
			//------------------------------
			// 番組データファイルのロード
			//------------------------------
			lock(this)
			{

				if (File.Exists(AnimeProgramsXmlPath))
				{
					if( !Load( AnimeProgramsXmlPath ) )
					{
						//------------------------------------------
						// animePrograms.YYYYMMHH.xmlにバックアップ
						//------------------------------------------
						try
						{
							string	strBackup;
							string strDateTime;
							
							strDateTime = string.Format(	"{0:D4}{1:D2}{2:D2}",
															DateTime.Now.Year	,
															DateTime.Now.Month	,
															DateTime.Now.Day	);

							strBackup = Path.ChangeExtension( AnimeProgramsXmlPath, strDateTime + ".xml" );

							File.Copy(	AnimeProgramsXmlPath	,
										strBackup				);
							Logger.Output( "読み込み失敗したファイルをバックアップしました: " + strBackup );
						}
						catch(Exception ex)
						{
						}

						//------------------------------------
						// 一つ前のデータファイルを読み込む
						//------------------------------------
						if( File.Exists( AnimeProgramsPreviousXmlPath ) )
						{
							Logger.Output( "一つ前のデータファイルを読み込みます - " + AnimeProgramsPreviousXmlPath );
							if( !Load( AnimeProgramsPreviousXmlPath ) )
							{
								Logger.Output( "これも読み込みに失敗しました" );
							}
						}
					}
				}
			}

			//---------------------------------
			// バックグラウンドシーケンス開始
			//---------------------------------
			
			mPulseSequenceTerminate = new ManualResetEvent( false );

			mBackGroundSequence = new Thread( new ThreadStart( this.BackGroundMain ) );
			mBackGroundSequence.Start();

			//------------------------------------
			// エンコードジョブマネージャ初期化
			//------------------------------------
			mEncodeJobManager = new BatchManager( Settings.Default.concurrentNums );

			mEncodeJobManager.ProcessedEvent	+= OnEncodeJobProcessed;
			mEncodeJobManager.JobErrorEvent	+= Logger.Output;

			//-----------------------------------------
			// ジョブ実行中のアプリ終了時中断確認
			//-----------------------------------------

			// エンコード中にアプリを閉じる場合の処理を登録
			Program.AppClosing += ClosingProc;

			try
			{
				if ( Settings.Default.autoUpdate )
					StartWatchingCaptureFolder( Settings.Default.captureFolder );

			}catch(Exception e)
			{
				Logger.Output(e.Message);
			}

		}

		//=========================================================================
		///	<summary>
		///		Dispose
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void Dispose()
		{
			if( mBackGroundSequence != null )
			{
				Logger.Output( "バックグラウンドシーケンス停止" );
				mPulseSequenceTerminate.Set();

				try
				{
					while( !mBackGroundSequence.Join( 100 ) )
					{
						Logger.Output( "...シーケンス停止待ち" );
					}

					Logger.Output( "...シーケンスが停止しました" );
				}
				catch( ThreadStateException )
				{
				}
			}

			Program.AppClosing -= ClosingProc;

			mEncodeJobManager.Finalize();

			EndWatchingCaptureFolder();
		}

		//=========================================================================
		///	<summary>
		///		ジョブ実行中のアプリ終了確認ハンドラ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
	    private void ClosingProc(
	        object							sender	,
	        Program.AppClosingEventArags	e		)
		{
		    DialogResult	dr;
			BatchJob[]		jobs;
			bool			executing = false;

			jobs = mEncodeJobManager.GetCurrentJob();

			foreach( BatchJob job in jobs )
				executing |= (job != null);

			if( !executing )
			{
				e.Cancel = false;
			}
			else
			{
				//--------------------------------
				// (ジョブ実行中)確認画面を表示
				//--------------------------------

				dr = MessageBox.Show(
					"エンコードジョブを実行中です。\n"	+
					"処理を中断してよろしいですか？"	,
					"確認"								,
					MessageBoxButtons.OKCancel			,
					MessageBoxIcon.Exclamation			,
					MessageBoxDefaultButton.Button2		);

				if (dr != DialogResult.OK)
				{
					e.Cancel = true;
					return;
				}

				//----------------------
				// 実行中のジョブを中断
				//----------------------
				mEncodeJobManager.CancelJobs();
			}
		}

		//=========================================================================
		///	<summary>
		///		番組データファイルを格納するファイルのパスを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		static public string AnimeProgramsXmlPath
		{
			get
			{
				string filePath;
                filePath = Path.Combine( Program.AppDataPath, "animePrograms.xml" );
				return filePath;
			}
		}

		//=========================================================================
		///	<summary>
		///		一つ前のデータファイルのパスを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/05/06 新規作成</history>
		//=========================================================================
		static public string AnimeProgramsPreviousXmlPath
		{
			get
			{
				return Path.ChangeExtension( AnimeProgramsXmlPath, "previous.xml" );
			}
		}
		
		//=========================================================================
		///	<summary>
		///		番組データファイルを吐き出す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void Save()
		{
			string			fileName	= AnimeProgramsXmlPath;
			XmlTextWriter	xmlWriter;

			//--------------------------
			// 保存前のファイルを退避
			//--------------------------
			try
			{
				string		prevName	= AnimeProgramsPreviousXmlPath;

				if( File.Exists( prevName ) )
					File.Delete( prevName );

				File.Move( fileName, prevName );
			}
			catch(Exception ex)
			{
			}
			
			lock(this)
			{
				xmlWriter = new XmlTextWriter( fileName, System.Text.Encoding.UTF8 );

				xmlWriter.Formatting = Formatting.Indented;

				xmlWriter.WriteStartDocument();
				
				xmlWriter.WriteStartElement("番組");

				foreach(AnimeProgram p in Animes)
					p.Write(xmlWriter);

				xmlWriter.WriteEndElement();
			
				xmlWriter.WriteEndDocument();

				xmlWriter.Flush();
				xmlWriter.Close();
			}
		
		}
		
		//=========================================================================
		///	<summary>
		///		番組データファイルを読み込む
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2008/05/06 引数にファイル名を渡すようにした</history>
		//=========================================================================
		public bool Load( string fileName )
		{
			XmlTextReader x = null;
			AnimeProgram a = null;

			try
			{
				lock(mAnimeList)
				{
					x = new XmlTextReader(fileName);
					mAnimeList.Clear();

					while (x.Read())
					{
						if (x.NodeType == XmlNodeType.Element)
						{
							if (x.LocalName.Equals("ProgramInformation"))
							{
								a = new AnimeProgram( this );
								a.Read(x);
								mAnimeList.Add(a);
							}

						}//else if (x.NodeType == System.Xml.XmlNodeType.EndElement)

					}
					x.Close();
				}
			}
			catch (Exception ex)
			{
				if ( x != null )
				{
					x.Close();
				}

				Logger.Output( "データファイルの読み込みに失敗しました - " + fileName );
				Logger.Output( "エラー詳細: " + ex.Message );

				return false;
			}

			return true;
		}

		//=========================================================================
		///	<summary>
		///		新規アニメを追加する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void AddAnime(AnimeProgram prog)
		{
			AddAnime(prog, -1);
		}
		public void AddAnime(AnimeProgram prog, int index)
		{
			lock( mAnimeList )
			{
				if( 0 <= index )
					mAnimeList.Insert( index, prog );
				else
					mAnimeList.Add( prog );
			}
		}

		//=========================================================================
		///	<summary>
		///		アニメ録画管理を削除する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void DeleteAnime(AnimeProgram prog)
		{
		
			lock(this)
			{
				lock( mAnimeList )
					mAnimeList.Remove( prog );

				//---------------------------------
				// 録画ソフト側の予約をキャンセル
				//---------------------------------
				foreach(AnimeEpisode episode in prog.Episodes)
				{
					if( episode.IsReserved )
					{
						ReserveManager rm = new ReserveManager();
						string descript;

						descript = string.Format(
							"{0:0} {1:0}話"			,
							prog.title				,
							episode.StoryNumber		);	// 暫定

						rm.CancelReservation( descript,episode.GetUniqueString() );
					}

				}

			}

		}

		//=========================================================================
		///	<summary>
		///		アニメ一覧をソート
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/06 新規作成</history>
		//=========================================================================
		internal void SortAnime(AnimeSort sort)
		{
			lock( mAnimeList )
				mAnimeList.Sort( sort );
			
		}

		//=========================================================================
		///	<summary>
		///		録画データをオンラインで更新する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void UpdateOnline(
			List<AnimeProgram>	animes	,		// [i] 更新対象の番組
			bool				force	,		// [i] 強制更新
			ProgressUpdateDelegate callBack )	// [i] プログレス通知
		{
			int i = 0, count;
			List<SyoboiCalender.SyoboiUpdate> updateList = null;

			SyoboiCalender syoboCal = this.mSyoboiCalender;

			count = animes.Count;

			callBack(0, count, "更新情報をダウンロードしています");
			for( int j = 0 ; ; ++j )
			{
				try
				{
					updateList = syoboCal.DownloadUpdateList();
					if( updateList != null )
						break;
				}
				catch(Exception ex)
				{
					if( ex.GetType().IsSubclassOf(typeof(WebException)) )
						throw;

					if( j < Settings.Default.retryDownload )
					{
						Logger.Output(
							string.Format(
								"接続エラーのためリトライ({0}/{1})",
								j + 1, Settings.Default.retryDownload));

						Thread.Sleep( RetryInterval );
					}
					else
						throw new UpdatingException("しょぼかるへのネットワーク接続エラー");
				}
			}

			foreach (AnimeProgram prog in animes)
			{
				DateTime updateDate = new DateTime(2000,1,1);

				if (prog.linkOnlineDatabase)
				{
					bool update = false;

					// 「前回更新日時」が異常(現在時刻より進んでいる)な場合の対策
					if (DateTime.Now < prog.LastUpdate)
					{
						Logger.Output("(しょぼかる) mAgicAnimeの前回更新時刻が不正のため、取得し直します(" + prog.title + ")");
						update = true;
					}
					else
					{
						//-------------------------------
						// 1週間更新がなければ強制更新
						//-------------------------------
						if (7 <= (DateTime.Now - prog.LastUpdate).Days)
						{
							update = true;
							updateDate = DateTime.Now;
							Logger.Output("(しょぼかる) 一週間更新情報がなかったので強制データ更新します(" + prog.title + ")");
						}
						else
						{
							//-------------------------------
							// この番組の新着データを探す
							//-------------------------------
							foreach (SyoboiCalender.SyoboiUpdate u in updateList)
							{
								if (prog.syoboiTid == u.tid)
								{
									// 前回更新日時より後にしょぼかるが更新されていたら更新する
									if (prog.LastUpdate < u.updateDate)
									{
										update = true;
										updateDate = (updateDate < u.updateDate) ? u.updateDate : updateDate;
										Logger.Output("(しょぼかる) 番組情報が更新されています(" + u.updateDate + " - " + prog.title + ")");
									}
								}
							}

						}

					}

					if (force && !update)
					{
						updateDate	= DateTime.Now;
						update		= true;

						if( force )
							Logger.Output("(しょぼかる)強制データ更新します(" + prog.title + ")");
					}

					//-------------------------------
					// 番組データを更新
					//-------------------------------
					if(update)
						prog.UpdatePlan(updateDate);

					if (callBack != null)
						callBack(i++, count, prog.title);

				}
			}

		}

		//=========================================================================
		///	<summary>
		///		番組のサムネイルを更新する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal void UpdateThumbnail()
		{
			if (Settings.Default.forbiddenThumbnail)	// サムネイル作成を禁止
				return;

			foreach (AnimeProgram p in Animes)
				p.UpdateThumbnail();
		
		}

		//=========================================================================
		///	<summary>
		///		全ての番組に対し、放送時間の重複をチェックする
		///	</summary>
		/// <remarks>
		///		時間が重複する全てのエピソードを割り出す(優先度無効時)
		///		チューナの数が足りず録画できなくなるエピソードを割り出す(優先度有効時)
		/// </remarks>
		/// <history>2007/11/11 新規作成</history>
		/// <history>2009/11/21 n個以上の重複チェックに対応</history>
		/// <history>2009/11/23 チューナの数が足りる分は重複としないよう変更</history>
		//=========================================================================
		private delegate void SweepDoubleBookingDelegate(
			SweepDoubleBookingDelegate d,
			int m,
			int i,
			DateTime startTime,
			DateTime endTime,
			List<int> path);

		internal void CheckDoubleBooking()
		{
			try
			{
				List<AnimeEpisode>	episodes		= QueryEpisode( OnAirCondition );
				List<AnimeEpisode>	conflicts		= new List<AnimeEpisode>();
				int					threshould		= Settings.Default.overlapThreshould;
				int					margin			= Settings.Default.overlapMargin;
				bool				enablePriority	= Settings.Default.enablePriority;

				// 放送時間順にソート
				episodes.Sort( new SortAnimeListOrderUpperDateTime() );

				// n重複をチェックするため、k^n個の全組み合わせをスィープする。
				SweepDoubleBookingDelegate sweep = delegate(
					SweepDoubleBookingDelegate _sweep, // [i]
					int			m			,	// [i] 
					int			index		,	// [i] 
					DateTime	startTime	,	// [i] 
					DateTime	endTime		,	// [i] 
					List<int>	_path		)	// [i] 辿ったノードを除外するための経路
				{
					bool pathTerminate = true;	// 組み合わせツリーの末端

					for (int i = index; i < episodes.Count - 1; ++i)
					{
						AnimeEpisode episode		= episodes[ i ];

						// 明らかに重ならない時間帯を無視して探索範囲を狭める
						if (endTime < episode.StartDateTime)
							break;

						if (!_path.Contains(i))
						{
							if ( episode.StartDateTime < endTime &&
								 startTime < episode.EndDateTime  )
							{
								DateTime overlapStartTime	= (startTime < episode.StartDateTime)
															? episode.StartDateTime : startTime;
								DateTime overlapEndTime		= (endTime < episode.EndDateTime)
															? endTime : episode.EndDateTime;

	//							if (m <= threshould )
								{
									if (margin < (overlapEndTime - overlapStartTime).TotalMinutes)
									{
										_path.Add(i);

										// 明らかに重ならない時間帯を無視して探索範囲を狭める
										// 重複範囲 T1&T2&･･･=[startTime,endTime] を求める過程の
										// 最も早い時間帯Tnのインデックスnを得る。
										int minIndex = -1;
#if _TEST
//									_path.ForEach(delegate(int a)
//									{
//										minIndex = (minIndex < 0) ? a : Math.Min(minIndex, a);
//									});
#else
										minIndex = 0;
#endif

										_sweep(_sweep, m + 1, minIndex, overlapStartTime, overlapEndTime, _path);

										_path.RemoveAt(_path.Count - 1);

										pathTerminate = false;
									}
								}

							}
						}

					}

					if( pathTerminate )
					{
						//----------------------------------------
						// エピソードの組み合わせを重複として記録
						//----------------------------------------
						List<AnimeEpisode> episodesByPriority = new List<AnimeEpisode>();

						foreach (int j in _path)
							episodesByPriority.Add(episodes[j]);

						// 時間帯が重複するエピソードを番組優先度順にソート
						episodesByPriority.Sort(delegate(AnimeEpisode epA, AnimeEpisode epB)
						{
							if( epA == null && epB != null )	return -1;
							if (epA != null && epB == null)		return +1;

							if (epA.Parent.priority == epB.Parent.priority)
	// <PENDING> 2009/11/23 優先度が同じなら適当に決める ->
							{
								if (epA.Parent.UniqueID == epB.Parent.UniqueID)
									return 0;
								return ( epA.Parent.UniqueID < epB.Parent.UniqueID ) ? +1 : -1;
								//return 0;
							}
	// <PENDING> 2009/11/23 <-

							return (epA.Parent.priority < epB.Parent.priority) ? +1 : -1;
						});


						foreach (AnimeEpisode epi in episodesByPriority)
						{
							// チューナ数が足りるものは重複にしない(優先度有効時)
							if(  enablePriority && (threshould <= episodesByPriority.IndexOf(epi)) ||
								!enablePriority && (2 <= episodesByPriority.Count)					)
							{
								if (!conflicts.Contains(epi))
									conflicts.Add(epi);
							}
						}
					}
				};

				List<int> path = new List<int>();
				sweep(sweep, 1, 0, DateTime.MinValue, DateTime.MaxValue, path);

				mDoubleBookingEpisodes = conflicts;
			}
			catch (Exception ex)
			{
			}
		}


		//=========================================================================
		///	<summary>
		///		番組エピソードを放送日時の降順にソートするコンペアクラス
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		class SortAnimeListOrderLowerDateTime : IComparer<AnimeEpisode>
		{
			public int Compare( AnimeEpisode x, AnimeEpisode y )
			{
				if ( x.StartDateTime > y.StartDateTime )
					return -1;
				else if ( y.StartDateTime > x.StartDateTime )
					return +1;
				else
					return 0;
			}
		}


		//=========================================================================
		///	<summary>
		///		番組エピソードを放送日時の昇順にソートする
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		class SortAnimeListOrderUpperDateTime : IComparer<AnimeEpisode>
		{
			public int Compare( AnimeEpisode x, AnimeEpisode y )
			{
				if ( x.StartDateTime > y.StartDateTime )
					return +1;
				else if ( y.StartDateTime > x.StartDateTime )
					return -1;
				else
					return 0;
			}
		}

		//=========================================================================
		///	<summary>
		///		規定の予約処理(将来n日間の放送分を予約)を行う
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal void ReserveProc(
			List<AnimeProgram>	animes		,	// [i] 対象の番組
			ReserveManager		manager		,	// [i] 録画予約マネージャ
			LogoutDelegate		logout		,	// [i] ログ出力コールバック
			bool				changeOnly	)	// [i] 予約時間の変更のみ
		{
			//lock(this)
			//{
			List<AnimeEpisode>	list = new List<AnimeEpisode>();
			DateTime			now = DateTime.Now;

			CheckDoubleBooking();

			//----------------------------------------
			// 予約するエピソードを放送時間順に列挙
			//----------------------------------------
			foreach ( AnimeProgram prog in animes )
			{
				foreach( AnimeEpisode episode in prog.Episodes )
				{
					// 向こうn日間の放送を予約する
					DateTime dateLimit = now.AddDays( Settings.Default.reserveDays );

					bool	isEnd	= episode.JudgeTimeEnd( now );
					bool	make	=	episode.HasPlan
									&&	!episode.IsReserved
									&&	episode.IsRecordRequired
									&&	(episode.StartDateTime < dateLimit)
									&&	!isEnd;
					bool	change	= episode.JudgeTimeChanged;
					bool	ignore	= episode.IsReserveError;		// 予約エラーが出ているとき自動予約しない

					if( ((make && !changeOnly) || change) && !ignore )
					{
						list.Add( episode );
					}
				}
			}

			list.Sort( new SortAnimeListOrderLowerDateTime() );

			//--------------------
			// 予約処理を行う
			//--------------------

			for ( int i = 0; i < list.Count;++i )
			{
				string errorMessage;

				if( list[i].IsReservePending() )
				{
					Logger.Output( "...チューナ数が足りないため、予約は保留します(" + list[i].ToString() + ")" );
				}
				else
				{
					if( !list[i].Reserve( manager, out errorMessage ) )
					{
						// 予約エラー原因をメッセージ表示
						if( logout != null )
							logout( errorMessage );
					}
				}
			}

			//

			manager.Flush();

			//----------------------------------------
			// 正常に予約が登録されているかチェック
			//----------------------------------------
			foreach (AnimeProgram p in Animes)
			{
				p.CheckReserve( manager );
			}
			
		}

		//=========================================================================
		///	<summary>
		///		録画Episodeの状態を更新する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void UpdateState(
			List<AnimeProgram>	animes	,	// [i] 更新対象の番組
			DateTime			now		,	// [i] 現在時刻
			string				[]files	)	// [i] 被検索対象のファイル一覧
		{
			if (files == null)
				files = ListupMovies();

			foreach (AnimeProgram p in animes)
			{
				p.UpdateState( now,files );
			}
		}

		
		//=========================================================================
		///	<summary>
		///		設定された録画フォルダからファイルを列挙する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		static public string[] ListupMovies()
		{
			string[] files;

			if (!Directory.Exists(Settings.Default.captureFolder))
			{
				Logger.Output( "録画フォルダが見つかりません(" + Settings.Default.captureFolder.ToString() + ")" );
				files = new string[0];
				return files;
			}

			if (!Settings.Default.captureSubDir)										// サブディレクトリの中に録画ファイルが作られる？
			{

				files = Directory.GetFiles(
					Settings.Default.captureFolder		,
					"*" + Settings.Default.strExtension	);								// 録画ファイルを列挙する
			} else
			{

				files = Directory.GetDirectories( Settings.Default.captureFolder );		// 録画サブディレクトリを列挙する
			}

			return files;
		}

	
		//=========================================================================
		///	<summary>
		///		全番組の全Episodeを列挙する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public int EnumAllEpisodes(
			AnimeProgram.EnumRecordCallBack	callBack	,
			object							param		)
		{
			int count = 0;

			foreach (AnimeProgram p in this.Animes)
			{
				count += p.EnumEpisodes( callBack ,param );
			}

			return count;
		}

		public delegate bool QueryEpisodeCondition( AnimeEpisode r );		// QueryEpisodeの条件

		//=========================================================================
		///	<summary>
		///		条件に合致するEpisodeのリストを得る
		///	</summary>
		/// <remarks>
		///		conditionがnullの場合は全てのEpisodeを列挙する。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public AnimeProgram.EpisodeList QueryEpisode( QueryEpisodeCondition condition )
		{
			AnimeProgram.EpisodeList newList = new AnimeProgram.EpisodeList();

			foreach( AnimeProgram prog in Animes )
			{
				foreach( AnimeEpisode episode in prog.Episodes )
				{
					if( condition == null							||
						(condition != null && condition( episode )) )
					{
						newList.Add( episode );
					}
				}
			}

			return newList;
		}

		//=========================================================================
		///	<summary>
		///		Episodeがエンコード待ちな条件
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public static bool WaitingEncodeCondition(AnimeEpisode r)
		{
			return ( r.Parent.EncoderType != null &&
					 r.HasFile && !r.IsEncoded && !r.IsStored );
		}

		//=========================================================================
		///	<summary>
		///		Episodeが放送待ちな条件
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public static bool OnAirCondition(AnimeEpisode r)
		{
			return r.HasPlan && !r.JudgeTimeEnd( DateTime.Now );
		}

		//=========================================================================
		///	<summary>
		///		最も早く放送するエピソードを取得
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/06/25 新規作成</history>
		//=========================================================================
		public AnimeEpisode QueryEarliestEpisode()
		{
			List<AnimeEpisode> epis;

			epis = QueryEpisode(AnimeServer.OnAirCondition);

			Comparison<AnimeEpisode> dateTimeComparison
				= delegate(AnimeEpisode a, AnimeEpisode b)
			{
				return a.StartDateTime.CompareTo(b.StartDateTime);
			};

			// 時刻順にソート
			epis.Sort(dateTimeComparison);

			return (0 < epis.Count) ? epis[0] : null;
		}

		//=========================================================================
		///	<summary>
		///		指定された時刻から録画ファイルを検索する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2007/04/18 ショートファイル名に変換する必要がなくなった</history>
		/// <history>2009/04/30 ファイル名を指定文字列でフィルタするオプション追加</history>
		//=========================================================================
		static public string FindCapturedFile(
			string		UniqueID	,	// [i] エピソードの識別ID
			DateTime	startTime	,	// [i] 放送時刻
			DateTime	endTime		,	// [i] 放送終了時刻
			string[]	movieFiles	,	// [i] 動画ファイルリスト
			string		filterName	)	// [i] ファイル名のフィルタ(オプション)
		{
			int				minMergin = Settings.Default.captureMergin;
			List<string>	matchList = new List<string>();
		
			foreach(string fn in movieFiles)
			{
				DateTime	timeStamp;
				DateTime	targetTime;
				string		sfn;				// ファイル名
				bool		matched	= false;	// 合致フラグ
				bool		isDir	= false;

				switch (Settings.Default.specifiedFile)
				{
				case IdentifyFileMethod.ByCreateTimeStamp:
				case IdentifyFileMethod.ByUpdateTimeStamp:
					//-----------------------
					// 作成(更新)時刻で特定
					//-----------------------
					sfn = fn + ".";

					isDir	=	Directory.Exists(sfn)
							&&	(0 < (File.GetAttributes(sfn) & FileAttributes.Directory));

					if( Settings.Default.specifiedFile == IdentifyFileMethod.ByCreateTimeStamp )
					{
						timeStamp	= isDir	?	Directory.GetCreationTime(sfn)	:
												File.GetCreationTime(sfn)		;
						targetTime	= startTime;
					}
					else
					{
						timeStamp	= isDir	?	Directory.GetLastWriteTime(sfn)	:
												File.GetLastWriteTime(sfn)		;
						targetTime	= endTime;
					}

					matched	= (Math.Abs( (targetTime - timeStamp).TotalMinutes ) < minMergin);
					
					break;
				case IdentifyFileMethod.ByFileNameWithID:
					//-----------------------
					// ファイル名のIDで特定
					//-----------------------
					if (0 <= fn.IndexOf(UniqueID+"_"))							// ファイル名にIDがあるか？
					{
						matched = true;
					}

					break;
				}

				if( matched )													// 合致？
				{
					if (Settings.Default.captureSubDir)							// サブディレクトリ内にファイルが作られるか？
					{
						string[]	files;
						string		dir;

						dir = fn + ".";

						if ((File.GetAttributes( dir ) & FileAttributes.Directory) > 0)		// ディレクトリか？
						{
							string ext = Settings.Default.strExtension;			// 録画ファイルの規定の拡張子
							files = Directory.GetFiles( dir, "*" +  ext );		// サブディレクトリ内のファイルを列挙

							if (files.Length > 0)
								matchList.Add( files[0] );		// サブディレクトリ内の一番目の録画ファイルをターゲットとする
						}

					}else{
						matchList.Add( fn );
					}

				}
				
			}

			//------------------------------------------
			// リストのファイル名を指定文字列でフィルタ
			//------------------------------------------
			if( !string.IsNullOrEmpty( filterName ) )
			{
				List<string>	remList = new List<string>();
				foreach (string fn in matchList)
				{
					if ( fn.ToUpper().IndexOf(filterName.ToUpper()) < 0 )
					{
						remList.Add( fn );
					}
				}

				foreach( string fn in remList )
					matchList.Remove( fn );
			}

			if( 1 <= matchList.Count )
				return matchList[ 0 ];
		
			return null;
		}

		//=========================================================================
		///	<summary>
		///		手動でエンコードジョブを投入する(非同期)
		///	</summary>
		/// <remarks>
		///		同じジョブが重複して投入されても無視
		///		手動エンコード中に自動エンコードの時間が来てもジョブを追加しない
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void AddEncodeJob(AnimeEpisode record)
		{
			EncodeJob job = new EncodeJob(record);

			if( mEncodeJobManager.Contains(job) )
				return;

			if( !mNowEncoding )
			{
				mAutoShutdown = false;
			}

			mNowEncoding = true;

			mEncodeJobManager.AddJob(job);
		}

		//=========================================================================
		///	<summary>
		///		バッチ登録されているエンコードジョブリストを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public List<EncodeJob> GetQueueingEncodeJobs()
		{
			List<EncodeJob> encodeJobs = new List<EncodeJob>();
			BatchJob[] jobs;

			jobs = mEncodeJobManager.GetQueueingJobs();

			// EncodeJobのみ洗い出す
			foreach (BatchJob job in jobs)
			{
				if (job.GetType() == typeof(EncodeJob))
				{
					encodeJobs.Add((EncodeJob)job);
				}
			}

			return encodeJobs;
		}

		//=========================================================================
		///	<summary>
		///		エンコード中のジョブを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public EncodeJob[] GetCurrentJobs()
		{

			BatchJob[]			jobs;
			List<EncodeJob>		encodeJobs = new List<EncodeJob>();
			
			jobs = mEncodeJobManager.GetCurrentJob();

			foreach( BatchJob job in jobs )
			{
				if (job != null &&
					job.GetType() == typeof(EncodeJob))
				{
					encodeJobs.Add( (EncodeJob)job );
				}
			}

			if( 0 < encodeJobs.Count )
				return encodeJobs.ToArray();

			return new EncodeJob[0];
		}

		//=========================================================================
		///	<summary>
		///		エンコードジョブを全てキャンセル
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/11/16 新規作成</history>
		//=========================================================================
		internal void CancelJobs()
		{
			mEncodeJobManager.CancelJobs();
		}

		//=========================================================================
		///	<summary>
		///		エンコードジョブが1つ終了した時の処理
		///	</summary>
		/// <remarks>
		///		自動シャットダウンプロパティがTrueのとき、シャットダウンをかける。
		///		手動で追加したときはシャットダウンしない。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void OnEncodeJobProcessed(
			BatchJob		job		,	// [i] 完了したジョブ
			bool			last	)	// [i] TRUE:最後のジョブ FALSE:他にジョブあり
		{
			//if (job.GetType() == typeof(EncodeJob))
			{
				// 残るジョブが0ならシャットダウン...
				if ( last )
				{
					mNowEncoding = false;

					if( AutoShutdown )
					{
						Logger.Output( "エンコードジョブ全完了につきシャットダウン" );
						Program.TryShutdown();
					}
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		未エンコードの全てのEpisodeをエンコードバッチジョブ投入する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public int BatchEncodeAll()
		{
			//---------------------------------------------
			// エンコード可能な状態のEpisodeをリストアップ
			//---------------------------------------------
			AnimeProgram.EpisodeList list = QueryEpisode( WaitingEncodeCondition );

			mAutoShutdown = Settings.Default.autoShutdownAtEncoded;
			mNowEncoding = true;

			//-------------------------------------
			// エンコードバッチジョブをエンキュー
			//-------------------------------------
			foreach( AnimeEpisode episode in list )
			{
				mEncodeJobManager.AddJob( new EncodeJob( episode ) );
			}
			
			// 追加したジョブ数を返す
			int		totalCount = mEncodeJobManager.JobCount;

			if( totalCount == 0 )
				mNowEncoding = false;

			string		log;
			log = string.Format(	"全エンコードジョブを投入 - 追加ジョブ数:{0:0} 待機ジョブ数:{1:0}",
									list.Count		,
									totalCount		);
			Logger.Output( log );

			return list.Count;
		}

		//=========================================================================
		///	<summary>
		///		起動直後の自動処理を実行
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/05/04 OnIntervalから分離</history>
		//=========================================================================
		private void FirstProc()
		{
			if( Settings.Default.autoTransferAtBoot )
			{
				//-------------------------
				// 起動時に保存先へ転送
				//-------------------------
				try
				{
					int count = 0;

					Logger.Output( "[シーケンス] 起動時に保存先へ転送..." );

					AnimeProgram.EpisodeList list;

					list = QueryEpisode( delegate (AnimeEpisode ep)
					{
						bool bExclude	=	Settings.Default.autoMoveWithoutFirstEpisode
										&&	(ep.StoryNumber == 1);

						return		ep.IsStorable
								&&	!ep.IsStored
								&&	!bExclude;
					});

					foreach( AnimeEpisode ep in list )
					{
						try
						{
							ep.Store();
							++count;
						}
						catch( EpisodeMethodException ex )
						{
							Logger.Output( "(自動転送)" + ex.Message );
						}
					}

					if ( 0 < count )
					{
						Logger.Output( "(起動時自動転送)保存先(" + Settings.Default.saveFolder + ")へ" + count.ToString() + "本を移動しました。" );
					}
				}
				catch ( Exception ex )
				{
					Logger.Output( "(自動転送)" + ex.Message + ex.StackTrace );
				}
			}
		}


		//=========================================================================
		///	<summary>
		///		バックグラウンドシーケンス
		///	</summary>
		/// <remarks>
		///		バックグラウンドで実行し、指定の時刻にエンコードジョブの投入など
		///		を行う。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2008/07/20 スレッドで書き直し</history>
		/// <history>2008/10/01 放送前にバルーン表示する機能追加</history>
		//=========================================================================
		void BackGroundMain()
		{
			Logger.Output( "[シーケンス] バックグラウンドシーケンスが開始しました" );

			//----------------------
			// 起動直後のウェイト
			//----------------------
			long	wait = Settings.Default.startupWait;

			Logger.Output("[シーケンス] 起動直後のウェイト(" + wait.ToString() + "sec)");

			DateTime	endTime = DateTime.Now.AddSeconds( wait );

			for (; ; )
			{
				if (mPulseSequenceTerminate.WaitOne(0, false))
				{
					Logger.Output("...中断");
					break;
				}

				if( 0 <= DateTime.Now.CompareTo( endTime ) )
				{
					Logger.Output("...OK");
					break;
				}
			}

			//----------------------
			// 起動直後の自動処理
			//----------------------

			Logger.Output( "[シーケンス] 起動後の初期処理" );

			FirstProc();

			for(;;)
			{
				//----------------------------------
				// 終了要求を確認、及びスリープ
				//----------------------------------
				if( mPulseSequenceTerminate.WaitOne( 5000, false ) )
				{
					Logger.Output( "[シーケンス] バックグラウンドシーケンスを終了します" );
					break;
				}

				// 前回と同一時分か判定
				DateTime	nowDateTime	= DateTime.Now;
				bool		haveJust	= false;

				if( mPrevSequenceTime.HasValue )
				{
					haveJust	= ( (mPrevSequenceTime.Value.Hour	== nowDateTime.Hour)	&&
									(mPrevSequenceTime.Value.Minute	== nowDateTime.Minute)	);
				}

				//----------------------------------------
				// 放送終了後、一定時間後に保存先へ転送
				//----------------------------------------
				if( Settings.Default.autoTransferAtAfterRecord )
				{
					AutoTrasnferProc();
				}

				//----------------------------------------
				// 放送終了後、一定時間後に再エンコード
				//----------------------------------------
				if( Settings.Default.autoEncodeInAfterRecord )
				{
					AutoEncodeProc();
				}

				//-------------------------------------------
				// 指定時刻に自動的にエンコードジョブを投入
				//-------------------------------------------
				if(	Settings.Default.scheduleEncodeEveryday	&&
					mNowEncoding == false					)
				{
					int			encodeTime	= Settings.Default.scheduleEncodeTime;
					bool		arise		= false;

					arise = ( nowDateTime.Hour	== encodeTime / 60	&&
							 nowDateTime.Minute	== encodeTime % 60	);

					if ( arise && !haveJust )
					{
						int count;

						//-------------------------
						// バッチエンコード投入
						//-------------------------
						Logger.Output( "[シーケンス] バッチエンコード開始" );

						count = BatchEncodeAll();

						// エンコードするファイルがなければシャットダウン
						if(count == 0)
						{
							Logger.Output( "[シーケンス] ...必要なし" );
							if( mAutoShutdown )
							{
								Logger.Output( "[シーケンス] ...直ちにシャットダウン" );
								Program.TryShutdown();
							}
						}
					}
				}

				//----------------------------
				// 放送直前に強制データ更新
				//----------------------------
				if( Settings.Default.updateOnAirSoon )
					UpdateOnAirSoon();

				//----------------------------
				// 一定時間ごとにデータ更新
				//----------------------------
				if( Settings.Default.autoRefresh )
				{
					// 初回は更新せず、次から更新する(起動時更新と重複するため)
					if( !mBeforeAutoUpdateTime.HasValue )
					{
						mBeforeAutoUpdateTime = nowDateTime;
					}

					if ( ( DateTime.Now - mBeforeAutoUpdateTime.Value ).TotalMinutes >= Settings.Default.updateInterval )
					{
						Logger.Output( "[シーケンス] 自動更新" );
						BeginUpdate( 0 );

						mBeforeAutoUpdateTime = DateTime.Now;
					}

				}

				//-------------------------------
				// 番組放送直前にバルーン表示
				//-------------------------------
				if( Settings.Default.showSoonBalloon )
				{
					DateTime	now				= DateTime.Now;
					int			popupMergin		= Settings.Default.timeSoonBalloon;

				   // 放送時刻一定時間前～放送時刻かを判定
				   QueryEpisodeCondition isBroadcastingSoon = delegate (AnimeEpisode r)
				   {
						if( !r.HasPlan )
							return false;

						return (r.StartDateTime.AddMinutes( -popupMergin ) <= now)	&&
								(now < r.StartDateTime)										;
				   };

				   AnimeProgram.EpisodeList	soonEpisodes;
				   soonEpisodes = QueryEpisode( isBroadcastingSoon );

					// 既にポップアップ表示したエピソードを除外
					Predicate<AnimeEpisode> isAlreadyPopupped = delegate (AnimeEpisode r)
					{
						return mPopuppedSoonEpisodes.Contains( r );
					};
					soonEpisodes.RemoveAll( isAlreadyPopupped );
					
					// ポップアップ済リストから放送時間が過ぎたエピソードを削除
					Predicate<AnimeEpisode> isAlreadyBroadcasted = delegate (AnimeEpisode r)
					{
						if( !r.HasPlan )
							return true;

						return r.EndDateTime < now;
					};
					mPopuppedSoonEpisodes.RemoveAll( isAlreadyBroadcasted );


				   foreach( AnimeEpisode episode in soonEpisodes )
				   {
						if( mPopupSoonBallon != null )
							mPopupSoonBallon( episode );

						// 既にポップアップ表示したエピソードを記憶しておく
						mPopuppedSoonEpisodes.Add( episode );
					}
				}

				mPrevSequenceTime = nowDateTime;

				// 放送直前に予約を入れるモード
				if (Settings.Default.reserveControl == ReserveControl.ImmediatlyBefore)
				{
					ReserveImmediatlyBefore( Settings.Default.reserveImmediatlyBeforeMinutes );
				}
			}

		}

		//=========================================================================
		///	<summary>
		///		放送終了後の自動転送処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/05/04 OnIntervalから分離</history>
		//=========================================================================
		private void AutoTrasnferProc()
		{
			try
			{
//				AnimeEpisode stored = null;
				DateTime now = DateTime.Now;

				List<AnimeEpisode> episodes;

				episodes = QueryEpisode( delegate (AnimeEpisode ep)
				{
					if( ep.HasFile && !ep.IsStored && ep.IsStorable )
					{
						DateTime endTime = ep.EndDateTime.AddMinutes( Settings.Default.autoTransferTime );

						// 録画後、数分後？
						if ( ( endTime <= now ) &&
							(( now - endTime ).Ticks / TimeSpan.TicksPerSecond) < 60 )
						{
							bool	exclude = false;
							// 第1話は除外するオプション
							exclude =	Settings.Default.autoMoveWithoutFirstEpisode
									&&	(1 == ep.StoryNumber);

							return !exclude;
						}
					}
					return false;
				} );

				foreach( AnimeEpisode ep in episodes )
				{
					ep.Store();
					Logger.Output( "(放送後自動転送)保存先(" + Settings.Default.saveFolder + ")へ" + ep.ToString() + "を移動しました。" );
				}

			}
			catch ( Exception ex )
			{
				Logger.Output( "(自動転送)" + ex.Message + ex.StackTrace );
			}
		}

		//=========================================================================
		///	<summary>
		///		放送終了後の再エンコード開始処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/05/04 新規作成</history>
		//=========================================================================
		private void AutoEncodeProc()
		{
			try
			{
				AnimeEpisode				target		= null;
				DateTime					now			= DateTime.Now;
				AnimeProgram.EpisodeList	targetEpisodes;

				// 「録画済」かつ「放送終了後n分」のエピソードを検索
				targetEpisodes = QueryEpisode(delegate(AnimeEpisode ep)
				{
					if( ep.HasFile && !ep.IsEncoded && !ep.IsStored )
					{
						DateTime triggerTime = ep.EndDateTime.AddMinutes(
												Settings.Default.autoEncodeMergin );

						// 放送終了後n分？
						return	(triggerTime <= now)
							&&	(now < triggerTime.AddMinutes(1));
					}
					return false;
				} );

				foreach (AnimeEpisode episode in targetEpisodes)
				{
					// 既に処理したエピソードは無視
					bool alreadyProcessed = false;
					alreadyProcessed = mAutoEncodedEpisodes.Exists( delegate (WeakReference epRef)
					{
						AnimeEpisode ep = epRef.Target as AnimeEpisode;
						return (ep != null) && (ep == episode);
					} );

					if (!alreadyProcessed)
					{
						mAutoEncodedEpisodes.Add(new WeakReference(episode));

						// 次の放送時間まで時間がなければエンコード開始しない
						if (Settings.Default.dontBeginEncodeLessThanMinutes)
						{
							AnimeEpisode earliestEpis;
							earliestEpis = QueryEarliestEpisode();

							TimeSpan span = (earliestEpis.StartDateTime - DateTime.Now);

							if (span.TotalMinutes < Settings.Default.maxDelayTimeDontBeginEncode)
							{
								Logger.Output("(放送後自動再エンコード)次の放送まで時間がないためエンコード開始しません - " + episode.ToString());
								break;
							}
						}

						//-------------------------
						// エンコードジョブ投入
						//-------------------------
						AddEncodeJob(episode);

						if (target != null)
						{
							Logger.Output("(放送後自動再エンコード)バッチジョブを投入しました - " + target.ToString());
						}
					}
				}
			}
			catch ( Exception ex )
			{
				Logger.Output( "(放送後自動再エンコード)" + ex.Message + ex.StackTrace );
			}
		}

		//=========================================================================
		///	<summary>
		///		放送直前n分前に予約を入れる
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/06/26 新規作成</history>
		//=========================================================================
		private void ReserveImmediatlyBefore(
			int		beforeMin	)	// [i] 放送開始n分前
		{
			try
			{
				DateTime			now			= DateTime.Now;
				List<AnimeEpisode>	immedEpisodes;

				CheckDoubleBooking();

				immedEpisodes = QueryEpisode(delegate(AnimeEpisode ep)
				{
					if( ep.HasPlan && ep.IsRecordRequired && !ep.IsReserved )
					{
						return	(	(ep.StartDateTime.AddMinutes(-beforeMin) <= now)
								&&	(now < ep.StartDateTime) );
					}
					return false;
				} );

				foreach( AnimeEpisode ep in immedEpisodes )
				{
					string	err;

					if( ep.IsReservePending() )
					{
						Logger.Output( "(放送直前予約) チューナ数が足りないため、予約は保留します(" + ep.ToString() + ")" );
					}
					else
					{
						if( ep.Reserve(new ReserveManager(), out err) )
							Logger.Output( "(放送直前予約) 予約完了 - " + ep.ToString() );
						else
							Logger.Output("(放送直前予約) 予約失敗(" + err + ") - " + ep.ToString());
					}
				}

				foreach (AnimeEpisode ep in immedEpisodes)
					ep.Dirty = true;
			}
			catch( Exception ex )
			{
				Logger.Output( "(放送直前予約) エラー(" + ex.Message + ")" );
			}
		}

		//=========================================================================
		///	<summary>
		///		データ更新処理を開始する(非同期)
		///	</summary>
		/// <remarks>
		///		開始に成功したらtrueを返す。
		/// </remarks>
		/// <history>2006/11/26 新規作成</history>
		/// <history>2008/10/21 非同期に改修。エラーログ表示用コールバック追加。</history>
		//=========================================================================
		public bool BeginUpdate(
			updateOption		option	)	// [i] データ更新オプション
		{
			return BeginUpdate( option, null );
		}
		public bool BeginUpdate(
			updateOption		option	,	// [i] データ更新オプション
			List<AnimeProgram>	animes	)	// [i] 更新する番組リスト
		{
			UpdateProcDelegate	delUpdate	= UpdateSequence;
			ManualResetEvent	endFlag		= new ManualResetEvent( false );
			// 経過表示
			UpdateProgress		progDelgate	= delegate (string Phase, int perc, string text )
			{
				if( mLockStatus.WaitOne() )
				{
					mMyStatus.updateDetail = Phase + " " + text;
					mLockStatus.ReleaseMutex();
				}
			};
			//---------------------
			// ログ出力
			//---------------------
			LogoutDelegate		logoutDelgate	= delegate (string text)
			{
				if (mLockStatus.WaitOne())
				{
					if ( !string.IsNullOrEmpty( mMyStatus.resultLastUpdate ) )
						mMyStatus.resultLastUpdate += System.Environment.NewLine;

					mMyStatus.resultLastUpdate += text;

					Logger.Output( text );

					mLockStatus.ReleaseMutex();
				}
			};

			//---------------------
			// 完了時処理
			//---------------------
			AsyncCallback completeCallback = delegate(IAsyncResult result)
			{
				if (mLockStatus.WaitOne())
				{
					mMyStatus.completeUpdate = true;
					mLockStatus.ReleaseMutex();
				}

				mDoingUpdateSequence = false;
				endFlag.Set();
			};

// <PENDIG> 2009/11/23 明示的に結果をクリアする？ ->
//			if (mLockStatus.WaitOne())
//			{
//				mThisStatus.resultLastUpdate = "";
//				mLockStatus.ReleaseMutex();
//			}
// <PENDIG> 2009/11/23 <-

			if( animes == null )
				animes = this.Animes;

			try
			{
				if( mDoingUpdateSequence )
				{
					// 既に実行中...
					return false;
				}

				endFlag.Reset();

				// データ更新シーケンスを開始する
				delUpdate.BeginInvoke(
					option,
					animes,
					progDelgate,
					logoutDelgate,
					completeCallback,
					null);

				mDoingUpdateSequence = true;
			}
			catch(Exception)
			{
				return false;
			}

			return true;
		}


		//=========================================================================
		///	<summary>
		///		データ更新シーケンス
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/10/21 新規作成</history>
		//=========================================================================
		private void UpdateSequence(
			updateOption		option		,	// [i] データ更新オプション
			List<AnimeProgram>	animes		,	// [i] 更新対象の番組
			UpdateProgress		onProgress	,	// [i] プログレス表示コールバック
			LogoutDelegate		logout		)	// [i] ログ表示コールバック
		{

			try
			{
				//-------------------------------------
				// オンラインデーターベースから更新
				//-------------------------------------

				if( onProgress != null )
				{
					onProgress( "オンラインデータベースにアクセス", 0, null );
				}

				AnimeServer.ProgressUpdateDelegate onOnlineProgress = delegate(
					int			perc	,
					int			max		,
					string		descipt	)
				{
					//---------------------------------------
					// 更新中の経過を上位にコールバック
					//---------------------------------------
					if (max == 0) max = 1;
					if( onProgress != null )
					{
						onProgress(	"オンラインデータベースにアクセス",
									100 * perc / max				,
									descipt							);
					}
				};

				bool force = ((option & updateOption.Force) != 0);	// 強制更新フラグ

				UpdateOnline(animes, force, onOnlineProgress);

				//-------------------------------------
				// Episode状態の更新
				//-------------------------------------

				if( onProgress != null )
				{
					onProgress( "録画ファイルの探索と状態の更新", 0, null );
				}

				UpdateState(animes, DateTime.Now, null);

				//-------------------------------------
				// 録画予約
				//-------------------------------------

				{
					if (onProgress != null)
					{
						onProgress("録画を予約", 0, null);
					}

					bool	changeOnly = false;
					changeOnly = (Settings.Default.reserveControl != ReserveControl.nDaysFromNow);

					ReserveProc(animes, new ReserveManager(), logout, changeOnly);
				}

				//-------------------------------------
				// 電源設定
				//-------------------------------------

				if (Settings.Default.autoPowerManagement)
				{
					if( onProgress != null )
					{
						onProgress( "PC自動起動を設定", 0, null );
					}

					ApplyBootSchedule();
				}

				//-------------------------------------
				// サムネイル更新
				//-------------------------------------

				if( onProgress != null )
				{
					onProgress( "サムネイル更新", 0, null );
				}

				UpdateThumbnail();

				// 放送時間重複のチェック
				if( onProgress != null )
				{
					onProgress( "放送時間の重複チェック", 0, null );
				}

				CheckDoubleBooking();

				//-------------------------------------
				// 更新完了
				//-------------------------------------

				if( onProgress != null )
				{
					onProgress( "更新完了", 0, null );
				}

				Save();
			}
			catch(UpdatingException e)
			{
				if( logout != null )
					logout("データ更新エラー(" + e.Message +")");
			}
			catch (Exception e)
			{
				{
					Logger.Output("内部エラー詳細: " + e.Message + "(" + e.StackTrace + ")");

					if( logout != null )
						logout("予期しない内部エラーが発生しました。(詳細はログを参照)");
				}
			}

			GC.Collect();
		}

		//=========================================================================
		///	<summary>
		///		アプリケーション設定が変更された時、動的に設定を反映
		///	</summary>
		/// <remarks>
		///		オプション画面が閉じられたタイミングなどで呼ぶこと。
		/// </remarks>
		/// <history>2009/02/14 新規作成</history>
		//=========================================================================
		internal void ApplyOption()
		{
			Logger.Output( "アプリケーション設定が変更されました。" );

			try
			{
				//---------------------
				// 録画フォルダの監視
				//---------------------
				EndWatchingCaptureFolder();

				if( Settings.Default.autoUpdate )
					StartWatchingCaptureFolder( Settings.Default.captureFolder );

			}
			catch (Exception e)
			{
				Logger.Output(e.Message);
			}
		}

		//=========================================================================
		///	<summary>
		///		録画用フォルダの監視を開始する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private bool StartWatchingCaptureFolder( string watchPath )
		{
			Logger.Output("...録画フォルダを監視(" + watchPath + (")"));

			if (mFileWatcher != null)
			{
				Logger.Output("......エラー(既にアクティブ)");
				return false;
			}

			try
			{

				if (!Directory.Exists(watchPath))
				{
					Logger.Output( "......エラー(パスが設定されていません)" );
					return false;
				}

				mFileWatcher = new FileSystemWatcher();

				mFileWatcher.Path			= watchPath;
				mFileWatcher.Filter			= "";
				mFileWatcher.NotifyFilter	= NotifyFilters.FileName | NotifyFilters.DirectoryName;
				mFileWatcher.Created			+= new FileSystemEventHandler( OnCreatedCaptureFile );

				mFileWatcher.EnableRaisingEvents = true;

				Logger.Output("......OK");
			}
			catch ( Exception )
			{
				mFileWatcher = null;
				Logger.Output("......エラー");
				return false;
			}

			return true;
		}

		//=========================================================================
		///	<summary>
		///		録画フォルダ監視を中止する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void EndWatchingCaptureFolder()
		{
			if ( mFileWatcher != null )
			{
				mFileWatcher.EnableRaisingEvents = false;
				mFileWatcher.Dispose();

				mFileWatcher = null;
			}
		}


		//=========================================================================
		///	<summary>
		///		録画フォルダにファイルが作成された場合の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2008/05/03 メソッド名変更(onFileChanged->OnCreatedCaptureFile)</history>
		//=========================================================================
		private void OnCreatedCaptureFile(
			System.Object source,
			System.IO.FileSystemEventArgs e )
		{
			try
			{
				if ( Settings.Default.autoUpdate )	// 自動アップデートオプション
				{

					if ( e.ChangeType == System.IO.WatcherChangeTypes.Created )
					{
						DateTime nowDateTime = DateTime.Now;

						// サブディレクトリが作られてから中にファイルが作られるまで
						// タイムラグがあるので少しsleepしておく
						if ( Settings.Default.captureSubDir )
						{
							Thread.Sleep( 3000 );

						} else
						{
							// 無関係な拡張子のファイルなら処理しない
							if ( !Path.GetExtension( e.FullPath ).Equals( Settings.Default.strExtension ) )
								return;
						}

						string[] temp = { e.FullPath };

						UpdateState( Animes, DateTime.Now, temp );
					}

				}

			}
			catch ( Exception ex )
			{
				Logger.Output( ex.Message );
			}

		}

		//=========================================================================
		///	<summary>
		///		BootTimerへPC起動スケジュールを反映
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2009/06/29 BootManagerから移動</history>
		//=========================================================================
		public void ApplyBootSchedule()
		{
			if (!Settings.Default.autoPowerManagement)
				return;

			mBootManager.Clear();

			//--------------------------------------------------
			// 予約済のエピソードを起動スケジュールリストに登録
			//--------------------------------------------------
			AnimeProgram.EpisodeList	episodeList;
			DateTime					now			= DateTime.Now;

			// 自動起動する放送を検索
			if( (Settings.Default.reserveControl == ReserveControl.ImmediatlyBefore)
			||  (Settings.Default.reserveControl == ReserveControl.noAutoReserve) )
			{
				//----------------------------------------
				// (放送直前に予約)今後n日間の放送を検索
				//----------------------------------------
				episodeList = QueryEpisode(delegate(AnimeEpisode ep)
				{
					if (ep.Parent.WithoutPower)
						return false;

					// 「未定」以外であれば放送時間データあり
					if( ep.HasPlan )
					{
						// 今後n日以内か判定
						{
							int nDays = Settings.Default.autoBootNDays;

							var endTime = ep.EndDateTime.AddMinutes(Settings.Default.shutdownPutoff);

							if ((now <= endTime) &&
								(ep.EndDateTime < now.AddDays(nDays)))
							{
								return true;
							}
						}
					}
					return false;
				});
			}
			else
			{
				//----------------------
				// 予約済の放送を検索
				//----------------------
				episodeList = QueryEpisode(delegate(AnimeEpisode episode)
				{
					if (episode.Parent.WithoutPower)
						return false;
					return	episode.HasPlan && episode.IsReserved;
				});
			}

			// 起動スケジュールに追加
			foreach (AnimeEpisode episode in episodeList)
			{
				DateTime startTime, endTime;

				startTime	= episode.StartDateTime;
				endTime		= episode.EndDateTime;

				// 備考の番組タイトルを入れる
				mBootManager.Add(startTime, endTime, episode.Parent.title);
			}

			// 近い時間帯同士を結合する
			mBootManager.Sort(new BootManager.ITimeZoneComparer());
			mBootManager.Unification();

			mBootManager.ApplyBootTimer( DateTime.Now );
		}

		//=========================================================================
		///	<summary>
		///		変更フラグ
		///	</summary>
		/// <remarks>
		///		データが変更されていればtrueを返す。
		///		上位からfalseをセットして表示更新が必要なタイミングを待つ。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal bool Dirty
		{
			get
			{
				bool childDirty = false;
				lock (mAnimeList)
				{
					foreach (AnimeProgram prog in mAnimeList)
						childDirty |= prog.Dirty;
				}

				return childDirty;
			}
			set
			{
				if (!value)
				{
					lock (mAnimeList)
					{
						foreach (AnimeProgram prog in mAnimeList)
							prog.Dirty = false;
					}
				}
//				isDirty = value;
			}
		}

		//=========================================================================
		///	<summary>
		///		オブジェクトのステータスを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/11/23 新規作成</history>
		//=========================================================================
		internal MyStatus GetStatus()
		{
			MyStatus copied;

			if (!mLockStatus.WaitOne())
				throw new Exception("内部エラー(GetStatus)");

			mMyStatus.updateSequenceBusy = mDoingUpdateSequence;

			copied = mMyStatus;

			mLockStatus.ReleaseMutex();

			return copied;
		}

		//=========================================================================
		///	<summary>
		///		データ更新結果をクリア
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/11/23 新規作成</history>
		//=========================================================================
		internal void ClearResultUpdate()
		{
			if (!mLockStatus.WaitOne())
				throw new Exception("内部エラー(ClearResultUpdate)");

			mMyStatus.resultLastUpdate = "";

			mLockStatus.ReleaseMutex();
		}

		//=========================================================================
		///	<summary>
		///		データ更新完了フラグをクリア
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/11/23 新規作成</history>
		//=========================================================================
		internal void ResetCompleteUpdate()
		{
			if (!mLockStatus.WaitOne())
				throw new Exception("内部エラー(ResetCompleteUpdate)");

			mMyStatus.completeUpdate = false;

			mLockStatus.ReleaseMutex();
		}

		//=========================================================================
		///	<summary>
		///		放送直前に強制データ更新
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/27 新規作成</history>
		//=========================================================================
		private void UpdateOnAirSoon()
		{
			var			episodes	= QueryEpisode( ep => true );
			DateTime	now			= DateTime.Now;
			List<int>	untilOnAir	= Settings.Default.untilOnAirMinutes;
			var			target		= new List<AnimeProgram>();

			foreach( AnimeEpisode ep in episodes )
			{
				if( ep.HasPlan )
				{
					// 放送までの時間[min]
					long	remain		= (long)(ep.StartDateTime - now).TotalMinutes;
					var		passedPoint	= new List<int>();

					// 経過したチェックポイント(n1,n2...分前)を取得
					foreach( int point in untilOnAir )
						if( remain < point )
							passedPoint.Add( point );

					passedPoint.Sort();

					if( 0 < remain )
					{
						if( 0 < passedPoint.Count )
						{
							// 通過した直近のチェックポイント
							int			minPoint	= passedPoint[0];
							UpdatedSoon entry;
							bool		exec		= false;

							// 最後に実行したチェックポイントより進んだか判定
							entry = mUpdatedSoonList.Find( e => (e.mEpisode.Target == ep) );

							if( entry != null )
								exec = (minPoint < entry.mDonePoint);
							else
								exec = true;

							if( exec )
							{
								Logger.Output( "[シーケンス] 放送" + minPoint.ToString() + "分前 強制データ更新"
											 + "(" + ep.Parent.title + ")" );

								if( !target.Contains( ep.Parent ) )
									target.Add( ep.Parent );

								// 更新ポイントを記録
								if( entry != null )
									entry.mDonePoint = minPoint;
								else
								{
									entry = new UpdatedSoon();
									entry.mDonePoint		= minPoint;
									entry.mEpisode			= new WeakReference(ep);
									mUpdatedSoonList.Add( entry );
								}
							}
						}
					}
				}
			}

			if( 0 < target.Count )
				BeginUpdate( updateOption.Force, target );

			// 実行済リストをクリーンアップ
			var deadList = mUpdatedSoonList.FindAll(
				delegate (UpdatedSoon entry)
			{
				AnimeEpisode ep = entry.mEpisode.Target as AnimeEpisode;
				if( ep != null )
					return (ep.HasPlan && ep.StartDateTime < now);
				return false;
			} );

			mUpdatedSoonList.RemoveAll( entry => deadList.Contains(entry) );
		}

	}


}
