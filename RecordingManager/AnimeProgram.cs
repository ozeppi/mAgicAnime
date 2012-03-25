//=========================================================================
///	<summary>
///		mAgicAnime番組データ モジュール
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成 Dr.Kurusugawa</history>
/// <history>2010/05/01 Subversionで管理するため不要なコメント削除</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using KernelAPI;
using MakeThumbnail;
using magicAnime.Properties;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		mAgicAnime番組データ クラス
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	/// <history>2009/04/30 録画テレビ局としょぼかるテレビ局を別にした</history>
	//=========================================================================
	public class AnimeProgram
	{
		//---------------------
		// データメンバ
		//---------------------
		public class EpisodeList : List<AnimeEpisode> { };

		public		string		title;
		public		 bool		linkOnlineDatabase;	// オンラインデータｰベース(しょぼかる)と連動
		public		int			syoboiTid;			// しょぼかるTID
		public		string		tvStation;			// 録画テレビ局名(録画ソフトへ渡す局名)
		public		string		syoboiTvStation;	// しょぼかるテレビ局名(データを引くための局名)
		public		bool		WithoutPower;		// 電源管理から除外
		public		long		adjustStartTime		= 0;		// 録画開始時間+n[分]
		public		long		adjustEndTime		= 0;		// 録画終了時間+n[分]

		// しょぼかるデータ選択ポリシ
		public enum SyobocalPolicy
		{
			SpecifyLatest,			// 最も後に放送するデータを選択
			SpecifyNumber,			// n回目の放送データを選択
			SpecifyEarly			// 現在から最も早く放送するデータを選択
		};

		public		SyobocalPolicy			syobocalPolicy			= SyobocalPolicy.SpecifyEarly;
		// 放送データn回目
		public		int						syobocalSpecifyNumber	= 1;
		public int							priority				= 30;	// 優先度(最低10～最高50)
		public bool							enableFilterKeyword		= false;	// 録画ファイルを指定文字列でフィルタ
		public string						filterKeyword			= "";		// フィルタ文字列

		private	int							mStoryCount;
		private	uint						mHashCode			= 0;
		private	AnimeServer					mParent;
		private bool						mIsDirty			= false;	// 変更フラグ

		private List<Scheduler.Profile>		mSchedulerProfiles;
		private Type						mEncoderType		= null;		// エンコーダクラスのType
		private EncodeProfile				mEncoderProfile		= null;		// エンコーダ設定
		private EpisodeList					mEpisodes;						// この番組の各話(Episode)の集合
		private	Mutex						mEpisodeLock		= new Mutex();
		private DateTime					mLastUpdate;					// データｰベースからの最終更新時刻
		private Image						mThambnailImage		= null;
		private Mutex						mThambnailLock		= new Mutex();


		//=========================================================================
		///	<summary>
		///		コンストラクタ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public AnimeProgram( AnimeServer parent )
		{
			this.mParent			= parent;
			this.title				= "";
			this.mStoryCount		= 0;
			this.linkOnlineDatabase	= true;
			this.syoboiTid			= 0;
			this.mLastUpdate		= new DateTime(2000, 1, 1);
			this.mSchedulerProfiles = new List<Scheduler.Profile>();
			this.WithoutPower		= false;

			this.mIsDirty			= true;
		}


		//=========================================================================
		///	<summary>
		///		親オブジェクトを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public		AnimeServer		Parent
		{
			get	{	return mParent; }
		}

		//=========================================================================
		///	<summary>
		///		変更フラグ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal bool Dirty
		{
			get
			{
				bool childDirty = false;

				try
				{
					mEpisodeLock.WaitOne();
					foreach (AnimeEpisode episode in mEpisodes)
						childDirty |= episode.Dirty;
				}
				finally
				{
					mEpisodeLock.ReleaseMutex();
				}

				return childDirty | mIsDirty;
			}
			set
			{
				if (!value)
				{
					try
					{
						mEpisodeLock.WaitOne();
						foreach (AnimeEpisode episode in mEpisodes)
							episode.Dirty = false;
					}
					finally
					{
						mEpisodeLock.ReleaseMutex();
					}
				}
				mIsDirty = value;
			}
		}

		//=========================================================================
		///	<summary>
		///		話数の取得と変更
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public int		StoryCount
		{
			get { return mStoryCount; }
			set
			{
				int i;

				try
				{
					mEpisodeLock.WaitOne();

					this.mIsDirty = true;

					if( mEpisodes == null )
					{
						mEpisodes = new EpisodeList();
					}

					//-----------------------------------
					// Episodeリストを話数にあわせて拡大
					//-----------------------------------
					if( mStoryCount < value )
					{
						for( i = mStoryCount ; i < value ; ++i )
						{
							mEpisodes.Add(new AnimeEpisode(this, i + 1));
						}
					}
					else
					{
						for( i = mStoryCount ; value < i ; --i )
						{
							mEpisodes.RemoveAt( i - 1 );
						}
					}

					mStoryCount = value;
				}
				finally
				{
					mEpisodeLock.ReleaseMutex();
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		番組ごとの録画設定
		///	</summary>
		/// <remarks>
		///		(スケジューラごとに設定を保持)
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public Scheduler.Profile SchedulerProfile( Type profType )
		{
			if (profType == null)
				return null;

			if (!profType.IsSubclassOf(typeof(Scheduler.Profile)))
				throw new Exception();

			foreach ( Scheduler.Profile prof in mSchedulerProfiles )
			{
				if ( prof.GetType() == profType )
					return prof;
			}

			// なければ追加する
			Scheduler.Profile newProf = Activator.CreateInstance( profType ) as Scheduler.Profile;
			if ( newProf != null )
			{
				mSchedulerProfiles.Add( newProf );
				return newProf;
			}
			return null;
		}

		//=========================================================================
		///	<summary>
		///		エンコーダ種別を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public Type EncoderType
		{
			get { return mEncoderType; }
			set
			{
				if (value != null)
				{
					Encoder encoder;

					if (value != mEncoderType)
					{
						encoder = (Encoder)Activator.CreateInstance(value);

						// エンコーダが変更されたらプロファイルオブジェクトを作り直す
						mEncoderProfile = (EncodeProfile)Activator.CreateInstance(encoder.ProfileType);
						mEncoderType = value;
					}
				}
				else {
					mEncoderProfile = null;
					mEncoderType = null;
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		エンコーダプロファイルを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public EncodeProfile EncoderProfile
		{
			get{ return mEncoderProfile; }
			set
			{
				if (value != null)
				{
					Encoder encoder;

					encoder = (Encoder)Activator.CreateInstance(EncoderType);

					if (value.GetType() != encoder.ProfileType)
						throw new Exception("内部エラー: EncoderProfileが異なります。");
				}
				mEncoderProfile = value;
			}
		}

		//=========================================================================
		///	<summary>
		///		全エピソードのリストを返す
		///	</summary>
		/// <remarks>
		///		リストオブジェクトのコピーを返す。
		///		上位がリストを変更してもこちらのデータに影響しない。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public EpisodeList Episodes
		{
			get
			{
				try
				{
					mEpisodeLock.WaitOne();

					EpisodeList coppied = new EpisodeList();
					
					foreach( AnimeEpisode ep in mEpisodes )
						coppied.Add( ep );

					return coppied;
				}
				finally
				{
					mEpisodeLock.ReleaseMutex();
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		番組固有IDを返す
		///	</summary>
		/// <remarks>
		///		しょぼかるTIDとは異なる
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public uint UniqueID
		{
			get
			{
				if (mHashCode == 0)
				{
					Guid newGuid = System.Guid.NewGuid();
					mHashCode = (uint)newGuid.GetHashCode();
				}

				return mHashCode;
			}
		}

		//=========================================================================
		///	<summary>
		///		最終更新時刻を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public DateTime LastUpdate
		{
			get { return mLastUpdate; }
		}

		//=========================================================================
		///	<summary>
		///		サムネイル用パスを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		// 
		static internal string ThambnailPath
		{
			get
			{
                return Path.Combine( Program.AppDataPath, "Thumbnails" );
			}
		}

		//=========================================================================
		///	<summary>
		///		サムネイルファイル名を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal string ThambnailFile
		{
			get
			{
				return ThambnailPath + @"\" + this.UniqueID.ToString() + ".png";
			}
		}

		//=========================================================================
		///	<summary>
		///		サムネイルイメージサイズを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal Size ThambnailSize
		{
			get
			{
				return new Size(120,90);
			}
		}

		//=========================================================================
		///	<summary>
		///		サムネイルイメージを返す
		///	</summary>
		/// <remarks>
		///		サムネイルは番組ごとに単一のファイルで保持する
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2010/01/02 イメージ作成中の場合はnullを返す</history>
		//=========================================================================
		public Image ThambnailImage
		{
			get
			{
				if( mThambnailLock.WaitOne( 0 ) )
				{
					try
					{
						if (mThambnailImage!=null)
							return mThambnailImage;

						//-----------------------------
						// サムネイル格納フォルダ準備
						//-----------------------------
						if (!Directory.Exists( ThambnailPath ))
							Directory.CreateDirectory( ThambnailPath );

						//-------------------------
						// サムネイルファイル準備
						//-------------------------
						if( File.Exists(ThambnailFile))
						{
							//------------------------
							// 規定のファイルを開く
							//------------------------
							mThambnailImage = new Bitmap( ThambnailFile );
						}
						else
						{
							//---------------
							// 新規作成
							//---------------
							mThambnailImage = new Bitmap(1, 1);
							mThambnailImage.Save(
								ThambnailFile							,
								System.Drawing.Imaging.ImageFormat.Png	);	// pngで保存

							foreach( AnimeEpisode episode in Episodes )
							{
								episode.ThumbnailMaked = false;				// サムネイル作成済フラグをクリア
							}
						}

						return mThambnailImage;
					}
					finally
					{
						mThambnailLock.ReleaseMutex();
					}
				}
				else
				{
					return null;
				}
			}
		}

		
		//=========================================================================
		///	<summary>
		///		第n話のエピソードを取得する(1～)
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public AnimeEpisode this[int storyNumber]
		{
			get
			{
				if (storyNumber < 1 || mStoryCount < storyNumber)
					throw new Exception("内部エラー: 話数が範囲外です");
				return (AnimeEpisode)mEpisodes[storyNumber - 1];
			}
		}

		public delegate void EnumRecordCallBack(AnimeEpisode record, object param);

		//=========================================================================
		///	<summary>
		///		全Episodeを列挙する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public int EnumEpisodes(EnumRecordCallBack callBack, object param)
		{
			foreach (AnimeEpisode episode in this.Episodes)
			{
				callBack(episode, param);
			}

			return mEpisodes.Count;
		}


		//=========================================================================
		///	<summary>
		///		次回放送状態
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <returns>
		///			0	次回放送日取得
		///			-1	次回放送日不明
		///			-2	放送なし(放送終了)
		///	</returns>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public enum NextEpisode
		{
			NextDecided	,
			NextUnknown	,
			EndProgram	,
		};


		//=========================================================================
		///	<summary>
		///		次回放送日付を取得
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal NextEpisode GetNextEpisode(
			DateTime			dateTime	,
			out AnimeEpisode	earlyOnAir	)
		{
			earlyOnAir = null;

			//----------------------------------
			// 最終話以降の日付なら放送終了
			//----------------------------------
			if( Episodes.Count == 0 )
				return NextEpisode.NextUnknown;

			AnimeEpisode lastEpisode = Episodes[Episodes.Count-1];

			if ( lastEpisode.HasPlan &&
				 lastEpisode.StartDateTime < dateTime)
			{
				earlyOnAir	= null;
				return NextEpisode.EndProgram;
			}
			
			//
			// dateTime以降、リストの中で最も早く放送するEpisodeを見つける
			//
			foreach (AnimeEpisode episode in Episodes)
			{
				if( episode.HasPlan &&
					dateTime <= episode.StartDateTime)
				{
					if (earlyOnAir != null)
					{
						if( episode.StartDateTime < earlyOnAir.StartDateTime )
						{
							earlyOnAir = episode;
						}
					}
					else
					{
						earlyOnAir = episode;
					}
				}
				
			}

			if (earlyOnAir==null)
				return NextEpisode.NextUnknown;

			return NextEpisode.NextDecided;
		}

		//=========================================================================
		///	<summary>
		///		録画Episodeの状態を更新
		///	</summary>
		/// <remarks>
		///		オフラインでの状態変化を更新
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal void UpdateState( DateTime now , string []files )
		{
			foreach (AnimeEpisode animeRecord in Episodes)
			{
				animeRecord.UpdateState(now,files);
			}
		}

		//=========================================================================
		///	<summary>
		///		放送プランを更新
		///	</summary>
		/// <remarks>
		///		放送時間の取得だけでなく、話数が増えた場合の追加も行う。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal uint UpdatePlan(DateTime lastUpdate)	// [i] 更新時刻
		{
			SyoboiCalender syoboiCalender = mParent.mSyoboiCalender;
			List<SyoboiCalender.SyoboiRecord> syoboiList = null;
			uint updateCount = 0;

			if ( syoboiTid				== 0		) return 0;
			if ( linkOnlineDatabase		== false	) return 0;

			try{
				string			title;
				List<String>	sourcePage;

				syoboiList = syoboiCalender.DownloadOnAirList( syoboiTid, out title, out sourcePage );

#if DEBUG		// 放送時刻データが空の場合のテスト
				if( Program.DebugOption.mForceEmpty )
					syoboiList.Clear();
#endif

				//--------------------------------
				// 話数が増えた場合はリスト拡充
				//--------------------------------
				int maxNumber = 0;

				foreach (SyoboiCalender.SyoboiRecord record in syoboiList)
					maxNumber = System.Math.Max(maxNumber, record.number);

				if (StoryCount < maxNumber)
					StoryCount = maxNumber;	// 話数が増えた場合だけ増やす

				//-------------------------------
				// エピソードごと放送データ更新
				//-------------------------------
				bool isPlanAbnormal = false;

				foreach (AnimeEpisode episode in Episodes)
				{
					bool abnormal;
					episode.UpdatePlan( syoboiList, out abnormal );
					isPlanAbnormal |= abnormal;
				}

				// 異常発生時、しょぼかるTIDページをバックアップ
				if( isPlanAbnormal )
				{
					string	strDateTime;
					string	fileName;
					string	filePath;
					
					strDateTime = string.Format(	"{0:D4}{1:D2}{2:D2}",
													DateTime.Now.Year	,
													DateTime.Now.Month	,
													DateTime.Now.Day	);

					fileName = String.Format("AbnormalData_tid{0}_{1}.html", syoboiTid, strDateTime);
					filePath = Path.Combine(Program.AppDataPath, fileName);

					var writer = new StreamWriter(filePath, true, Encoding.Unicode);
					sourcePage.ForEach( line => writer.WriteLine(line) );
					writer.Close();

					Logger.Output("異常データを含むTIDページを保存しました。(" + filePath + ")");
				}

				this.mLastUpdate = lastUpdate;
			}
			catch (Exception e)
			{
				throw new Exception(
					"オンラインデータ－ベースのダウンロードに失敗しました\n"
					+ e.Message + "\n" + title);
			}

			return updateCount;
		}

		//=========================================================================
		///	<summary>
		///		サムネイルを更新
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2010/01/02 MakeThumbnailを分離</history>
		//=========================================================================
		internal void UpdateThumbnail()
		{
			List<AnimeEpisode>	targets = new List<AnimeEpisode>();

			foreach (AnimeEpisode episode in Episodes)
				if (!episode.ThumbnailMaked)
					targets.Add( episode );


			foreach(AnimeEpisode episode in targets)
			{
				try
				{
					MakeThumbnail( episode );
				}
				catch(UpdatingException ex)
				{
					Logger.Output( "(サムネイル) " + ex.Message + "(" + episode.ToString() + ")" );
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		指定したエピソードのサムネイルを作成
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/02 MakeThumbnailを分離</history>
		//=========================================================================
		private bool MakeThumbnail(AnimeEpisode episode)
		{
			bool	bMake = false;

			// 「再エンコード後にサムネイル作成」オプション
			if( Properties.Settings.Default.makeThumbnailAfterEncode )
			{
				bMake = episode.HasFile && episode.IsEncoded;
			}
			else
			{
				bMake = episode.HasFile;
			}

			if( bMake )
			{
				Process		prc;
				string		args;

				if (File.Exists(episode.FilePath))
				{
					bool fileUsed = false;

					//-----------------------------
					// 録画中であることを判定
					//-----------------------------
					try
					{
						FileStream openTest = new FileStream(
							episode.FilePath	,
							FileMode.Open		, 
							FileAccess.Read		,
							FileShare.None		);

						openTest.Close();
					}
					catch(IOException e)
					{
						fileUsed = true;	// ファイル使用中で開けない
					}

					if(!fileUsed)
					{
						MemoryStream bmpImage;

						try
						{
							using( VideoImage videoImg = new VideoImage( episode.FilePath ) )
							{
								bmpImage = videoImg.GetFrameImage( Settings.Default.thumbnailSecond );
							}
						}
						catch(VideoImage.CannotOpenException ex)
						{
							throw new UpdatingException("サムネイル作成エラー(ファイルを開けない)");
						}
						catch(VideoImage.RenderFailedException ex)
						{
							throw new UpdatingException("サムネイル作成エラー(イメージを切り出せない)");
						}
						catch(Exception ex)
						{
							throw new UpdatingException("サムネイル作成エラー");
						}

						//----------------------------------------------
						// 番組サムネイルファイルへイメージを連結
						//----------------------------------------------
						try
						{
							mThambnailLock.WaitOne();

							if ( ThambnailImage == null )
								throw new UpdatingException( "内部エラー(UpdateThumbnail)" );

							int oldw = mThambnailImage.Width;								// 旧サムネイルの幅
							int imgw = Math.Max(
											ThambnailSize.Width * episode.StoryNumber,
											oldw									);		// 新サムネイルの幅
							int imgx = ThambnailSize.Width * ( episode.StoryNumber - 1 );	// サムネイルを追加するX座標

							// 横幅が長すぎると扱えないのでリミットをかける
							if (32000 < imgx)
							{
								Logger.Output("サムネイルファイルが大きすぎるため追加できません。");
								return false;
							}
							Image newimg = new Bitmap(imgw, ThambnailSize.Height);		// 新サムネイル
							Image tmpimg = new Bitmap(bmpImage);

							Graphics g = Graphics.FromImage( newimg );

							g.DrawImage( mThambnailImage,
								0									,
								0									,
								new Rectangle(
									0							,
									0							,
									oldw						,
									mThambnailImage.Size.Height ),
								GraphicsUnit.Pixel					);						// 旧サムネイルからコピー

							g.DrawImage(
								tmpimg						,
								new Rectangle(
									imgx				,
									0					,
									ThambnailSize.Width	,
									ThambnailSize.Height)	,
								new Rectangle(
									0					,
									0					,
									tmpimg.Size.Width	,
									tmpimg.Size.Height	)	,	// 新サムネイルに追加
								GraphicsUnit.Pixel			);

							mThambnailImage.Dispose();										// 更新前のイメージを破棄

							newimg.Save(
								ThambnailFile	,
								ImageFormat.Png	);

							tmpimg.Dispose();

							mThambnailImage = newimg;										// 新イメージに置き換え

							episode.ThumbnailMaked = true;

							this.mIsDirty = true;
						}
						finally
						{
							mThambnailLock.ReleaseMutex();
						}
					}
				}
			
			}
				
			return true;
		}

		//=========================================================================
		///	<summary>
		///		番組を新規予約・予約変更・予約確認する
		///	</summary>
		/// <remarks>
		///		将来の放送予定を全て予約済にする。(新規予約または予約変更)
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal void CheckReserve(ReserveManager manager)
		{
			foreach (AnimeEpisode episode in Episodes)
			{
				if( episode.IsReserved )
				{
					episode.CheckReserve(manager);
				}
			}
		}


		//=========================================================================
		///	<summary>
		///		番組データをXMLに書き出す
		///	</summary>
		/// <remarks>
		///		上位(AnimeServer)のSaveから呼ばれる
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void Write(System.Xml.XmlWriter xw)
		{
			xw.WriteStartElement("ProgramInformation");

			// 番組情報の書き出し

			xw.WriteElementString("Hash"				, Convert.ToString(mHashCode));

			xw.WriteElementString("Title"				, title);
			xw.WriteElementString("StoryCount"			, Convert.ToString(mStoryCount));
			xw.WriteElementString("LinkOnlineDatabase"	, linkOnlineDatabase ? "1" : "0");
			xw.WriteElementString("SyoboiTID"			, Convert.ToString(syoboiTid));
			
			xw.WriteElementString("TvStation"			, tvStation);
			xw.WriteElementString("SyoboiTvStation"		, syoboiTvStation);

			xw.WriteElementString("LastUpdate"			, Convert.ToString(mLastUpdate.Ticks));
			xw.WriteElementString("WithoutPower"		, WithoutPower? "1" : "0");
			xw.WriteElementString("AdjustStartTime"		, adjustStartTime.ToString()	);
			xw.WriteElementString("AdjustEndTime"		, adjustEndTime.ToString()		);
			xw.WriteElementString("SyobocalPolicy"		, Enum.GetName( syobocalPolicy.GetType(), syobocalPolicy)	);
			xw.WriteElementString("SyobocalSpecifyNumber", syobocalSpecifyNumber.ToString()							);
			xw.WriteElementString("EnableFilterKeyword"	, Convert.ToString( enableFilterKeyword )	);
			xw.WriteElementString("FilterKeyword"		, filterKeyword								);
			xw.WriteElementString("Priority"			, Convert.ToString(priority)				);

			// 番組各話データの書き出し

			foreach (AnimeEpisode episode in Episodes)
				episode.Write( xw );

			// エンコード情報の書き出し

			if (mEncoderType != null)
			{
				xw.WriteElementString("EncodeClass", mEncoderType.FullName);

				xw.WriteStartElement(mEncoderProfile.TagName);

				mEncoderProfile.Write(xw);
				
				xw.WriteEndElement();
			}
			else
				xw.WriteElementString("EncodeClass", null);

			// スケジューラ情報の書き出し
			foreach ( Scheduler.Profile prof in mSchedulerProfiles )
			{
				xw.WriteStartElement( "SchedulerProfile" );
				xw.WriteAttributeString( "Type", prof.GetType().FullName );
				xw.WriteAttributeString( "Assembly", prof.GetType().Assembly.FullName );

				prof.Export( xw );
				
				xw.WriteEndElement();
			}

			xw.WriteEndElement();
		}

		//=========================================================================
		///	<summary>
		///		XMLから読み込む
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void Read(System.Xml.XmlReader xr)
		{
			mSchedulerProfiles.Clear();

			mEpisodes = new EpisodeList();
			mEncoderProfile	= null;
			int	length = 0;

			while (xr.Read())
			{

				if (xr.NodeType == System.Xml.XmlNodeType.Element)
				{

					if (xr.LocalName.Equals("Title"))
						title = xr.ReadString();
					else if (xr.LocalName.Equals("StoryCount"))
						mStoryCount = xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("Length"))
						length = xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("LinkOnlineDatabase"))
						linkOnlineDatabase = xr.ReadElementContentAsInt() == 1 ? true : false;
					else if (xr.LocalName.Equals("SyoboiTID"))
						syoboiTid = xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("TvStation"))
						tvStation = xr.ReadElementContentAsString();
					else if (xr.LocalName.Equals("SyoboiTvStation"))
						syoboiTvStation = xr.ReadElementContentAsString();
					else if (xr.LocalName.Equals("Hash"))
						mHashCode = (uint)xr.ReadElementContentAsDecimal();
					else if (xr.LocalName.Equals("LastUpdate"))
					{
						string strDateTime = xr.ReadElementContentAsString();
						mLastUpdate = new DateTime(Convert.ToInt64(strDateTime));
					}
					else if (xr.LocalName.Equals("WithoutPower"))
						WithoutPower = xr.ReadElementContentAsInt() == 1 ? true : false;
					else if (xr.LocalName.Equals("AdjustStartTime"))
						adjustStartTime = xr.ReadElementContentAsLong();
					else if (xr.LocalName.Equals("AdjustEndTime"))
						adjustEndTime = xr.ReadElementContentAsLong();
					else if (xr.LocalName.Equals("SyobocalPolicy"))
						syobocalPolicy = (SyobocalPolicy)Enum.Parse(syobocalPolicy.GetType(), xr.ReadElementContentAsString());
					else if (xr.LocalName.Equals("SyobocalSpecifyNumber"))
						syobocalSpecifyNumber = xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("EnableFilterKeyword"))
						enableFilterKeyword = bool.Parse(xr.ReadElementContentAsString());
					else if (xr.LocalName.Equals("FilterKeyword"))
						filterKeyword = xr.ReadElementContentAsString();
					else if(xr.LocalName.Equals("Priority"))
						priority = xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("Records"))
					{
						AnimeEpisode episode;

						// StoryCount++;
						episode = new AnimeEpisode(this, 0);

						episode.Read(xr);

						mEpisodes.Add(episode);
					}
					else if (xr.LocalName.Equals("EncodeClass"))
					{
						Encoder encoder;
						string typeName;

						typeName = xr.ReadElementString();

						mEncoderType = EncodeManager.FindEncoder(typeName);

						if (mEncoderType != null)
						{
							encoder = (Encoder)Activator.CreateInstance(mEncoderType);
							mEncoderProfile = Activator.CreateInstance(encoder.ProfileType) as EncodeProfile;
						}
					}
					else if (mEncoderProfile != null
						   && xr.LocalName.Equals(mEncoderProfile.TagName))
					{
						mEncoderProfile.Read(xr);
					}
					// スケジューラ設定を読み込む
					else if (xr.LocalName.Equals("SchedulerProfile"))
					{
						string profType = xr.GetAttribute("Type");
						string assemName = xr.GetAttribute("Assembly");

						if (profType != null && assemName != null)
						{
							Scheduler.Profile prof;
							System.Runtime.Remoting.ObjectHandle obj;

							try
							{
								if ((obj = Activator.CreateInstance(assemName, profType)) != null)
								{
									if ((prof = obj.Unwrap() as Scheduler.Profile) != null)
									{
										prof.Import(xr);
										mSchedulerProfiles.Add(prof);
									}
								}
							}
							catch (TypeLoadException ex)
							{
							}
						}
					}
				}
				else if (xr.NodeType == System.Xml.XmlNodeType.EndElement)
					if (xr.LocalName.Equals("ProgramInformation"))
						break;
			}

			// ver1.9.02以前からのデータ移行用
			// 「しょぼかるテレビ局名」が無ければ「録画テレビ局名」を代入
			if (string.IsNullOrEmpty(syoboiTvStation))
			{
				syoboiTvStation = tvStation;
			}

			if( 0 < length )
			{
				foreach( AnimeEpisode ep in this.Episodes )
					ep.Length = length;
			}

		}

	}


}
