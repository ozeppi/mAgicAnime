//=========================================================================
///	<summary>
///		エピソードデータクラス
///	</summary>
/// <remarks>
///		アニメ各話の録画状態を管理し、予約・エンコード等の処理を行う
/// </remarks>
/// <history>2006/XX/XX 新規作成 Dr.Kurusugawa</history>
/// <history>2007/08/16	番組ごとのスケジューラ設定に対応</history>
/// <history>2007/11/11	Stateの変更をproperty setからSetStateメソッドに変更</history>
/// <history>2009/12/28	古い改修コメント削除</history>
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
using System.Runtime.InteropServices;
using KernelAPI;
using magicAnime.Properties;
using magicAnime.OnLine_DataBase;
using Helpers;
//using Microsoft.VisualBasic.FileIO;

namespace magicAnime
{
	public class EpisodeMethodException : Exception
	{
		public EpisodeMethodException(string mes)
			: base(mes)
		{
		}
	};

	//=========================================================================
	///	<summary>
	///		アニメ各話の録画状態を管理する
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	//=========================================================================
	public class AnimeEpisode
	{
		//---------------------------
		// 列挙子の定義
		//---------------------------

		//=========================================================================
		///	<summary>
		///		エピソードの状態
		///	</summary>
		/// <remarks>
		///		Ver1.9.18以降、この情報は表示に使うのみで
		///		処理の可否判断に使用してはならない。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		public enum State
		{
			Notfound	,	// 録画ファイルが見つからない
			Planned		,	// 放送プラン確定
			Recorded	,	// 録画済
			Encoded		,	// 再エンコード済
			Stored		,	// 保存済
			Scheduling	,	// 録画予約済
			Encoding	,	// エンコード中
			Undecided	,	// 未定
			Changed		,	// 放送時間変更
			DontCare	,	// 視聴しない
			LostSchedule,	// 予約消失
			Busy		,	// 内部処理中
		};

		//---------------------------
		// メンバ変数
		//---------------------------
		private	AnimeProgram	mParent;                            // 番組オブジェクト
		private bool			mHasPlan		= false;			// 放送スケジュール確定
		private	DateTime		mStartTime;                         // 放送開始時刻
		private int				mLength			= 0;				// 放送長さ[min]
		private bool			mIsReserved		= false;			// 予約済フラグ
		private DateTime		mReserveStartTime;					// 録画ソフトに予約している開始時刻
		private bool			mHasFile		= false;			// 動画ファイルあり
		private string			mFilePath;							// 動画ファイルパス
		private	bool			mThumbnailMaked	= false;            // サムネイル作成済フラグ
		private bool			mIsUnread		= true;				// 未読フラグ
		private bool			mIsReserveError	= false;			// 予約エラーフラグ
		private bool			mIsEncoded		= false;			// 再エンコード済
		private bool			mIsStored		= false;			// 保存先に転送済
		private bool			mIsBusy			= false;			// 処理中フラグ
		public	string			mSubTitle		= "";				// サブタイトル
		private bool			mIsDirty		= false;			// データ変更フラグ
		private int				mStoryNumber	= 0;				// エピソード番号
		private int				mRepeatNumber	= -1;				// n回目の放送
		private bool			mPlanError		= false;			// 放送プランデータ異常

		private	Encoder			encoding		= null;				// エンコード処理中のエンコーダオブジェクト


		//=========================================================================
		///	<summary>
		///		コンストラクタ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		public AnimeEpisode(
			AnimeProgram	animeProgram	,
			State			initialState	,
			string			inFilePath		,
			DateTime		inDateTime		,
			int				inStoryNumber	,
			string			inSubTitle		)
		{


			this.mStoryNumber	= inStoryNumber;
			this.mSubTitle		= inSubTitle;
			this.mIsUnread		= true;
			
			this.mParent		= animeProgram;

			this.Dirty			= true;
		}

		//=========================================================================
		///	<summary>
		///		コンストラクタ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public AnimeEpisode( AnimeProgram animeProgram, int storyNumber )
		{
			this.mFilePath	= null;
			this.mSubTitle	= "";
			
			this.mParent	= animeProgram;
			
			this.mStoryNumber = storyNumber;

			this.Dirty = true;
		}


		//=========================================================================
		///	<summary>
		///		親オブジェクトへの参照を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public AnimeProgram Parent	{ get { return mParent; } }

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
			get { return mIsDirty; }
			set { mIsDirty = value; }
		}

		//=========================================================================
		///	<summary>
		///		未読フラグ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal bool Unread
		{
			get { return this.mIsUnread; }
			set
			{
				this.mIsUnread = value;
				this.mIsDirty = true;
			}
		}

		internal bool IsReserved
		{
			get{ return mIsReserved; }
			set{ mIsReserved = value; }
		}

		// 予約している時刻と放送時刻が異なるか判定
		internal bool JudgeTimeChanged
		{
			get
			{
				if( this.mHasPlan && this.mIsReserved )
					return (this.StartRecordDateTime != this.mReserveStartTime);
				return false;
			}
		}

		internal bool HasPlan
		{
			get { return mHasPlan; }
			set
			{
				mHasPlan	= value;
				Dirty		= true;
			}
		}

		internal bool HasFile
		{
			get { return mHasFile; }
			set
			{
				mHasFile	= value;
				Dirty		= true;
			}
		}

		internal bool IsEncoded
		{
			get{ return mIsEncoded; }
			set
			{
				mIsEncoded	= value;
				Dirty		= true;
			}
		}

		internal bool IsStored
		{
			get { return mIsStored; }
			set
			{
				mIsStored	= value;
				Dirty		= true;
			}
		}

		internal bool IsBusy
		{
			get{ return mIsBusy; }
		}

		internal DateTime ReserveDateTime
		{
			get{ return mReserveStartTime; }
			set
			{
				mReserveStartTime	= value;
				Dirty				= true;
			}
		}

		internal bool IsReserveError
		{
			get{ return mIsReserveError; }
			set
			{
				mIsReserveError	= value;
				Dirty			= true;
			}
		}

		// 放送時間が終わっているか判定
		internal bool JudgeTimeEnd(DateTime now)
		{
			if( mHasPlan )
				return EndDateTime < now;
			return false;
		}

		// 予約録画を必要とするか返す(既に録画されていればfalse)
		internal bool IsRecordRequired
		{
			get{ return !mHasFile; }
		}
		internal int Length
		{
			get{ return mLength; }
			set
			{
				mLength = value;
			}
		}
		internal int RepeatNumber
		{
			get{ return mRepeatNumber; }
		}
		// 予約録画中もしくは録画後であるか返す
		internal bool IsRecorded
		{
			get
			{
				if( this.HasPlan && this.IsReserved )
					return (this.StartDateTime <= DateTime.Now);
				else if( this.HasFile )
					return true;
				return false;
			}
		}
		// 放送開始時間を過ぎたか返す
		internal bool IsStartedOnair
		{
			get
			{
				if( this.HasPlan )
					return (this.StartDateTime <= DateTime.Now);
				return false;
			}
		}
		// 放送プランデータ異常の有無
		internal bool PlanError
		{
			get{ return mPlanError; }
			set
			{
				mPlanError		= value;
				Dirty			= true;
			}
		}
		
		//=========================================================================
		///	<summary>
		///		放送開始時刻を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public DateTime StartDateTime
		{
			get
			{
				return mStartTime;
			}
			set
			{
				mStartTime	= value;
				Dirty		= true;
			}
		}

		//=========================================================================
		///	<summary>
		///		放送終了時刻を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public DateTime EndDateTime
		{
			get
			{
				return mStartTime.AddMinutes(mLength);
			}
		}

		//=========================================================================
		///	<summary>
		///		録画開始時刻を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public DateTime StartRecordDateTime
		{
			get
			{
				return mStartTime.AddMinutes( Settings.Default.reserveStart
											+ Parent.adjustStartTime );
			}
		}

		//=========================================================================
		///	<summary>
		///		録画終了時刻を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public DateTime EndRecordDateTime
		{
			get
			{
				return EndDateTime.AddMinutes( Settings.Default.reserveEnd
											 + Parent.adjustEndTime );
			}
		}

		//=========================================================================
		///	<summary>
		///		現在のエピソード状態を返す
		///	</summary>
		/// <remarks>
		///		Ver1.9.18以降、状態の値は保持しない。
		///		各フラグから状態を決めてそれを返すのみ。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public State CurrentState
		{
			get
			{
				if( IsBusy )
					return State.Busy;

				if( HasFile )
				{
					if( IsStored )
						return State.Stored;
					if( IsEncoded && !IsStored )
						return State.Encoded;
					return State.Recorded;
				}

				if( !HasPlan )
					return State.Undecided;
				else
				{
					if( JudgeTimeChanged )
						return State.Changed;
					else
					{
						if( IsReserveError )
							return State.LostSchedule;
						if( IsReserved )
							if( JudgeTimeEnd( DateTime.Now ) )
								return State.Notfound;
							else
								return State.Scheduling;
						else
							if( JudgeTimeEnd( DateTime.Now ) )
								return State.Notfound;
							else
								return State.Planned;
					}
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		対応する動画ファイルパスを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public string FilePath	
		{
			get
			{
				return mHasFile ? mFilePath : "";
			}
			set
			{
				mFilePath	= value;
				this.Dirty	= true;
			}
		}

		public int StoryNumber		{ get { return mStoryNumber;  } }

		//=========================================================================
		///	<summary>
		///		再生可能か示す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public bool IsPlayable
		{
			get
			{
				return mHasFile && !mIsBusy;
			}
		}

		//=========================================================================
		///	<summary>
		///		保存先に転送可能か示す
		///	</summary>
		/// <remarks>
		///		エンコード済、もしくは録画済で再エンコードなしならTRUE
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal bool IsStorable
		{
			get
			{
				if( mHasFile )
				{
					if( mIsEncoded )
						return true;
					if( !mIsEncoded	&&
						Parent.EncoderProfile == null	)
						return true;
				}
				return false;
			}
		}

		//=========================================================================
		///	<summary>
		///		サムネイル作成済みか返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal bool ThumbnailMaked
		{
			get { return mThumbnailMaked;  }
			set { mThumbnailMaked = value;  }
		}

		//=========================================================================
		///	<summary>
		///		放送時刻に重複があるか返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal bool IsDoubleBooking()
		{
			List<AnimeEpisode>	conflicts = Parent.Parent.DoubleBookingEpisodes;
			if( conflicts != null )
			{
				return conflicts.Contains( this );
			}
			return false;
		}


		//=========================================================================
		///	<summary>
		///		放送プランを更新
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2007/05/06 更新の条件を変更</history>
		/// <history>2009/01/04 メソッド名変更</history>
		/// <history>2010/04/17 古いコメント削除</history>
		//=========================================================================
		internal void UpdatePlan(
			List<SyoboiCalender.SyoboiRecord>	syoboiList	,	// [i] しょぼかるデータ
			out bool							abnormal	)	// [o] true:放送データに異常を検出
		{
			DateTime					now			 = DateTime.Now;
			SyoboiCalender.SyoboiRecord	syoboiRecord = null;

			abnormal = false;

			//-----------------------------
			// 放送日時データを更新
			//-----------------------------
			// 日時を変更できるのは予約録画が開始する前まで
			bool isRecording	= this.IsReserved && this.IsStartedOnair;
			bool isRecorded		= this.HasFile;

			bool dontCare		= this.mIsBusy || (isRecording || isRecorded);

			//------------------------------------
			// 該当するエピソードのデータを検索
			//------------------------------------

			var records = SyoboiRecordFilter.EpisodeAndStationFilter(
				syoboiList,
				mStoryNumber,
				mParent.syoboiTvStation);

			// 本放送と再放送のデータを区別

			if(Parent.syobocalPolicy == AnimeProgram.SyobocalPolicy.SpecifyLatest)
			{
				// 最も後の放送を選択
				syoboiRecord = SyoboiRecordFilter.LatestPlanFilter( records, out mRepeatNumber );
			}
			else if (Parent.syobocalPolicy == AnimeProgram.SyobocalPolicy.SpecifyNumber)
			{
				// n回目の放送を選択
				syoboiRecord = SyoboiRecordFilter.NumberPlanFilter(
					records,
					Parent.syobocalSpecifyNumber,
					out mRepeatNumber);
			}
			else if (Parent.syobocalPolicy == AnimeProgram.SyobocalPolicy.SpecifyEarly)
			{
				// 最も早い放送を選択
				bool	keep = false;

				// 現在時刻に応じて選択する放送プランが変わるが
				// 予約録画開始後には変えない。
				if( this.HasPlan && this.IsReserved )
					keep = (this.StartDateTime <= now);

				if( keep )
				{
					// 録画開始後はプランを変更しない
					if( 0 <= mRepeatNumber )
						syoboiRecord = SyoboiRecordFilter.NumberPlanFilter(
							records,
							mRepeatNumber,
							out mRepeatNumber );
					else
						records = null;
				}
				else
				{
					syoboiRecord = SyoboiRecordFilter.EarlytPlanFilter(
						records,
						now,
						out mRepeatNumber );

					if( syoboiRecord == null  )
					{
						// 今後の放送がない場合は最後の放送を選択しておく
						syoboiRecord = SyoboiRecordFilter.LatestPlanFilter(
							records,
							out mRepeatNumber );
					}
				}
			}

			if (syoboiRecord != null)
			{
				//-------------------------------------------------------------
				// オンラインDBに更新が見つかった場合、ローカルデータを更新
				//-------------------------------------------------------------
				if( mPlanError )
				{
					Logger.Output("サーバーの放送データが復活しています - " + this.ToString());
					mPlanError	= false;
				}

				// サブタイトルをセット
				if( !string.IsNullOrEmpty(syoboiRecord.subtitle) &&
					(mSubTitle != syoboiRecord.subtitle)		 )
				{
					mSubTitle	= syoboiRecord.subtitle;
					Dirty		= true;
				}

				if( !dontCare )
				{
					// 取得した放送プランで上書き
					if( !mHasPlan
					||	(mStartTime	!= syoboiRecord.onAirDateTime)
					||	(mLength	!= syoboiRecord.length)			)
					{
						mHasPlan	= true;
						mStartTime	= syoboiRecord.onAirDateTime;
						mLength		= syoboiRecord.length;
						Dirty		= true;
					}

				}
			}
			else
			{
				//--------------------------------------
				// データーベースに情報がなかった場合
				//--------------------------------------
				if( !dontCare )
				{
					if( mHasPlan && Settings.Default.keepPlan )
					{
						// 何らかの原因でしょぼかるからデータが消えていても時刻は残しておく
						if( !mPlanError )
						{
							mPlanError	= true;
							Dirty		= true;
							Logger.Output("サーバーの放送データが消えていますが現状のデータを維持します - " + this.ToString());
							abnormal	= true;
						}
					}
					else
					{
						mHasPlan	= false;
					}
					Dirty		= true;

					if( IsReserved )
					{
						Logger.Output("予約済エントリの放送データが無くなっています - " + this.ToString());
						abnormal	= true;
					}
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		録画状態の更新や録画ファイルを対応付ける
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal void UpdateState(
			DateTime	now			,	// 更新基準時間
			string[]	movieFiles	)	// 録画ファイルリスト
		{
			if( mHasPlan && !mHasFile )
			{
				{
					//----------------------------------------
					// 放送時間に合致する録画ファイルを検索
					//----------------------------------------
					string	filterKeyword;
					filterKeyword	= Parent.enableFilterKeyword ?
										Parent.filterKeyword : null;

					string filename = AnimeServer.FindCapturedFile(
								GetUniqueString()	,
								StartDateTime		,
								EndDateTime			,
								movieFiles			,
								filterKeyword		);

					if ( filename != null ) // ファイルが見つかれば録画済にする
					{
						this.mHasFile	= true;
						this.mFilePath	= filename;
						this.mIsUnread	= true;
						this.mIsEncoded	= false;	// 見つけたファイルは「再エンコ済」や「転送済」ではない
						this.mIsStored	= false;
						this.Dirty		= true;
					}
				}
			}
		
			if( !mHasFile )
			{
				//-----------------------------
				// 保存済ファイルを検索
				//-----------------------------
				try
				{
					bool		isExist		= false;
					string		targetDir	= Settings.Default.saveFolder;
					
					if( !string.IsNullOrEmpty(targetDir)	&&
						Directory.Exists( targetDir )		)
					{
						if( Settings.Default.classificatePrograms )
							targetDir = Path.Combine( targetDir, PathHelper.ToFileTitle( mParent.title ) );

						isExist = Directory.Exists( targetDir );

						if( isExist )
						{
							string[]	files;
							string		pattern = GetFormattedFileName() + ".*";

							files = Directory.GetFiles( targetDir, pattern );

							if( files.Length == 1 )
							{
								this.mHasFile	= true;
								this.mFilePath	= files[0];
								this.mIsEncoded	= false;
								this.mIsStored	= true;
								this.Dirty		= true;
							}
						}
					}
				}
				catch(Exception ex)
				{
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		このエピソードの録画予約に利用する一意な文字列を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public string GetUniqueString()
		{
			string unique;

			unique = mParent.UniqueID.ToString() + "_";			// 説明部分
			unique += mStoryNumber.ToString();

			return unique;
		}

		//=========================================================================
		///	<summary>
		///		このエピソードの予約を保留すべきか判断
		///	</summary>
		/// <remarks>
		///		優先度無効時は常にfalseを返す。
		///		上位側ではこの結果を見てReserveを呼び出すか判断すべき。
		///		(手動で強制予約したい場合はこの限りではない)
		/// </remarks>
		/// <history>2009/11/23 新規作成</history>
		//=========================================================================
		internal bool IsReservePending()
		{
			if( Settings.Default.enablePriority )
			{
				AnimeServer server = Parent.Parent;
					
				server.CheckDoubleBooking();

				return server.DoubleBookingEpisodes.Contains(this);
			}
			else
				return false;
		}

		//=========================================================================
		///	<summary>
		///		このエピソードを録画ソフトに予約
		///	</summary>
		/// <remarks>
		///		既に予約されている場合は何もしない。
		///		放送時間の変更を予約に反映させる。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2009/06/28 録画ソフト"なし"は予約失敗扱い</history>
		//=========================================================================
		internal bool Reserve(
			ReserveManager		manager			,	// [i] 録画予約マネージャ
			out string			errorMessage	)	// [o] 予約失敗詳細
		{
			string descript;

			descript = string.Format("{0:0} {1:0}話", Parent.title, StoryNumber);

			if( !IsReserved || JudgeTimeChanged )
			{
				DateTime	start	= DateTime.Now;
				int			length	= 0;

				//-----------------------------------
				// 録画開始時刻と録画終了時刻を計算
				//-----------------------------------
				try
				{
					start	= this.StartRecordDateTime;
					length	= (int)(this.EndRecordDateTime - start).TotalMinutes;

					if( length <= 0 )
						throw new Scheduler.ZeroLengthScheduleTimeException();

					//-------------------------------
					// 番組の予約プロファイルを取得
					//-------------------------------
					Scheduler.Profile prof = null;

					if (ReserveManager.DefaultScheduler				!= null		&&
						ReserveManager.DefaultScheduler.ProfileType != null		)
					{
						prof = Parent.SchedulerProfile( ReserveManager.DefaultScheduler.ProfileType );
					}

					if( !IsReserved )
					{
						//--------------------------------------
						// 今後の放送・予約が消失 → 新規予約
						//--------------------------------------
						if (manager.MakeReservation(
								descript,
								GetUniqueString(),
								Parent.tvStation,
								start,
								(uint)length,
								Parent.UniqueID,
								prof))
						{
							this.mIsReserved		= true;
							this.mIsReserveError	= false;
							this.mReserveStartTime	= start;
							this.Dirty				= true;
						}
						else
						{
							this.mIsReserved		= false;
							this.mIsReserveError	= true;
							this.Dirty				= true;
							errorMessage = "録画ソフトが指定されていません。(オプション画面で選択して下さい)";
							return false;
						}
					}
					else if( JudgeTimeChanged )
					{
						ReserveManager.ChangeResult res;
						
						//--------------------------------------
						// 放送時間変更 → 放送時間変更を行う
						//--------------------------------------
						res = manager.ChangeReservation(
								descript				,
								GetUniqueString()		,
								Parent.tvStation		,
								start					,
								(uint)length			,
								Parent.UniqueID			,
								prof					);
						
						if ( res == ReserveManager.ChangeResult.OK )				// 変更に成功？
						{
							this.mIsReserved		= true;
							this.mIsReserveError	= false;
							this.mReserveStartTime	= start;
							this.Dirty				= true;
						}
						else if ( res == ReserveManager.ChangeResult.Lost )			// 予約が失われた？
						{
							this.mIsReserved		= false;
							this.mIsReserveError	= true;
							this.Dirty				= true;
						}
					}

				}
				catch (Scheduler.DoubleBookingException e)										// 予約が重複？
				{
					Logger.Output( "(予約管理)予約が重複するため、予約を登録できません。 " + start.ToString() + " - " + descript );
					this.mIsReserved		= false;
					this.mIsReserveError	= true;
					this.Dirty				= true;
					errorMessage = "予約が重複するため、予約を登録できません。 " + start.ToString() + " - " + descript;
					return false;
				}
				catch(Scheduler.SchedulerBaseExecption e)
				{
					object		[]objAttributes;
					string		errorDetail			= "予約時にエラーが発生しました。";

					objAttributes = e.GetType().GetCustomAttributes(
						typeof(Scheduler.SchedulerExceptionAtribute), true );

					if( objAttributes != null )
					{
						Scheduler.SchedulerExceptionAtribute	exceptionAtrribute;
						exceptionAtrribute = objAttributes[0] as Scheduler.SchedulerExceptionAtribute;

						if( exceptionAtrribute != null )
							errorDetail = exceptionAtrribute.Description;
					}

					Logger.Output( "(予約管理)"
						+ errorDetail
						+ " "
						+ start.ToString()
						+ "～"
						+ EndDateTime.ToString()
						+ " - "
						+ descript				);

					this.mIsReserved		= false;
					this.mIsReserveError	= true;
					this.Dirty				= true;

					errorMessage = errorDetail + " " + descript;
					Logger.Output( "(予約管理)" + errorMessage );
					return false;
				}
				catch(System.Exception ex)
				{
					this.mIsReserved		= false;
					this.mIsReserveError	= true;
					this.Dirty				= true;

					errorMessage = ex.Message + " " + descript;
					Logger.Output( "(予約管理)" + errorMessage );
					return false;
				}

			}

			errorMessage = "";
			return true;
		}

		//=========================================================================
		///	<summary>
		///		録画ソフトに予約が入っているか確認
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2008/10/21 予約確認に不可能なプラグインなら無視</history>
		//=========================================================================
		internal void CheckReserve(ReserveManager manager)
		{
			string descript;

			descript = string.Format("{0:0} {1:0}話", Parent.title, StoryNumber);

			bool	evaluated;
			bool	exist;

			if( IsReserved )
			{
				//------------------------------
				// 予約が消えていないかチェック
				//------------------------------
				evaluated = manager.ExistReservation( descript, GetUniqueString(), out exist);

				if( evaluated && !exist )
				{
					this.mIsReserved		= false;
					this.mIsReserveError	= true;
					this.Dirty				= true;
				}
			}
			else
			{
				//------------------------------------
				// 消えた予約が復活しているかチェック
				//------------------------------------
                evaluated = manager.ExistReservation(descript, GetUniqueString(), out exist);

                if ( evaluated && exist )
                {
					this.mIsReserved		= true;
					this.mIsReserveError	= false;
					this.mReserveStartTime	= this.mStartTime;	// <PENDING> 2009/12/28
					this.Dirty				= true;
                }
			}
		}

		//=========================================================================
		///	<summary>
		///		録画ソフトの予約をキャンセルする
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/10/22 新規作成</history>
		//=========================================================================
		internal bool CancelReserve(ReserveManager manager)
		{
			string		descript;

			descript = string.Format("{0:0} {1:0}話", Parent.title, StoryNumber);

			if( !IsReserved )
				return false;

			try
			{
				if( !manager.CancelReservation( descript, GetUniqueString() ) )
					return false;
			}
			catch(Exception ex)
			{
				return false;
			}

			this.mIsReserved	= false;
			this.Dirty			= true;

			return true;
		}


		//=========================================================================
		///	<summary>
		///		このエピソードの録画ファイルを再エンコードする
		///	</summary>
		/// <remarks>
		///		エンコード終了まで制御は戻らない。
		///		EncodeJobクラスを利用して非同期で実行する。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal void Encode()
		{
			Encoder	encoder		= null;

			if( !mHasFile )
				throw new EpisodeMethodException("ファイルが指定されていない");
			if( mIsEncoded )
				throw new EpisodeMethodException("既にエンコードされている");
			if( mIsBusy )
				throw new EpisodeMethodException("処理中のためエンコード開始できない");

			if (Properties.Settings.Default.encodedFolder.Equals(""))
				throw new EpisodeMethodException("エンコード出力先フォルダが設定されていません");

			if (Parent.EncoderType == null)
				throw new EpisodeMethodException("エンコードプロファイルが指定されていません");

			if (!File.Exists(mFilePath))
				throw new EpisodeMethodException("エンコード元のファイルがありません" + mFilePath);

			try
			{
				string encodedFile;

				encodedFile = Path.Combine(
								Settings.Default.encodedFolder	,
								GetFormattedFileName()			);

				//----------------------
				// encoding状態に変更
				//----------------------
				this.mIsBusy	= true;
				this.mIsDirty	= true;

				//----------------------
				// 予約追加情報の準備
				//----------------------
				Encoder.TvProgramAdditionalInfo ai = new Encoder.TvProgramAdditionalInfo();

				ai.Title		= PathHelper.ToFileTitle(Parent.title);
				ai.StoryNumber	= this.StoryNumber.ToString();
				ai.Subtitle		= PathHelper.ToFileTitle(this.mSubTitle);
				ai.TvStation	= Parent.tvStation;
				ai.StartDate	= this.StartDateTime.ToShortDateString();
				ai.StartTime	= this.StartDateTime.ToShortTimeString();

				//----------------------
				// エンコーダの準備
				//----------------------
				encoder = (Encoder)Activator.CreateInstance(Parent.EncoderType);

				encodedFile += encoder.Extension;

				encoding = encoder;

				//--------------------
				// エンコーダ実行
				//--------------------

				encoder.DoEncode(
					FilePath				,
					ref encodedFile			,
					Parent.EncoderProfile	,
					ai						);							// エンコーダプラグイン呼び出し

				//----------------------------------------
				// エンコード後、元の録画ファイル削除
				//----------------------------------------
				if ( Settings.Default.removeSourceWhenEncoded )
				{
					try
					{
						File.Delete( mFilePath + "." );

						//
						// 場所が規定の録画フォルダならサブディレクトリごと削除
						//
						if (Settings.Default.captureSubDir)
						{
							string subFolder, parentFolder;

							subFolder		= Path.GetDirectoryName( mFilePath + "." );
							parentFolder	= Directory.GetParent( subFolder ).FullName;

							if (Settings.Default.captureFolder.Equals(parentFolder))
							{
								string[] files;

								// まずディレクトリ内のファイルを全て削除しておく
								files = Directory.GetFiles(subFolder);
								foreach (string f in files)
									File.Delete(f);

								Directory.Delete(subFolder, true);
							}
						}

					}
					catch (Exception e)
					{
						Logger.Output(e.Message);
					}

				}

				//-----------------------
				// エンコード済状態に
				//-----------------------
				this.FilePath	= encodedFile;
				this.mIsEncoded	= true;
			}
			catch (AbortException x)
			{
				this.mIsBusy	= false;
				throw x;
			}
			catch (Exception x)
			{
				this.mIsBusy	= false;
				throw x;
			}
			finally
			{
				this.encoding	= null;
				this.mIsBusy	= false;
				this.mIsDirty	= true;
			}
		}

		//=========================================================================
		///	<summary>
		///		エンコード処理を中断
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void CancelEncode()
		{
			if (encoding!=null)
			{
				encoding.AbortEncodeProcess();
			}
		}


		//=========================================================================
		///	<summary>
		///		フォーマットされたファイル名を取得する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public string GetFormattedFileName()
		{
			string formattedName, titleName, subTitleName;

			titleName		= PathHelper.ToFileTitle(mParent.title);
			subTitleName	= PathHelper.ToFileTitle(mSubTitle);

			try
			{
				string	dateStr = "";
				string	timeStr = "";
				string	station	= "";

				if(this.HasPlan)
				{
					var helper = new DateTimeHelper(
						this.StartDateTime,
						Settings.Default.hoursPerDay - 24);
					
					dateStr	= helper.ToShortDateString().Replace("/", "");
					timeStr	= helper.ToShortTimeString().Replace(":", "");
				}

				station = Parent.tvStation;

				formattedName = string.Format(
					Settings.Default.saveNameFormat,
					titleName,
					mStoryNumber,
					subTitleName,
					dateStr,
					timeStr,
					station);
			}
			catch(Exception ex)
			{
				throw new UpdatingException("保存ファイル名書式が正しくありません。");
			}

			return formattedName;
		}

		//=========================================================================
		///	<summary>
		///		フォーマットされた番組エピソード文字列を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public override string ToString()
		{
			string myName;

			try
			{
				string	dateStr = "";
				string	timeStr = "";
				string	station	= "";

				if(this.HasPlan)
				{
					var helper = new DateTimeHelper(
						this.StartDateTime,
						Settings.Default.hoursPerDay - 24);
					
					dateStr	= helper.ToShortDateString();
					timeStr	= helper.ToShortTimeString();
				}

				station = Parent.tvStation;

				myName = string.Format(
					Settings.Default.saveNameFormat,
					mParent.title,
					mStoryNumber,
					mSubTitle,
					dateStr,
					timeStr,
					station);
			}
			catch(Exception ex)
			{
				throw new UpdatingException("保存ファイル名書式が正しくありません。");
			}

			return myName;
		}

		//=========================================================================
		///	<summary>
		///		このエピソードの録画ファイルを保存先へ移動する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void Store()
		{
			string		storeName, storeDir;

			if( !mHasFile )
				throw new EpisodeMethodException("ファイルが指定されていない");
			if( mIsStored )
				throw new EpisodeMethodException("既に転送済");
			if( mIsBusy )
				throw new EpisodeMethodException("処理中のため転送できない");

			try
			{
				//----------------------
				// 処理中状態に遷移
				//----------------------
				this.mIsBusy	= true;
				this.mIsDirty	= true;

				if (Settings.Default.saveFolder.Trim().Equals(""))
					throw new EpisodeMethodException("オプションで最終保存先が設定されていません。");

				if ( !File.Exists( mFilePath ) )
					throw new EpisodeMethodException("ファイルが存在しないため、転送できません。" + mFilePath);

				storeDir = Settings.Default.saveFolder;	// 保存先フォルダ

				//----------------------------------
				// タイトルごとのサブフォルダ作成
				//----------------------------------
				if (Settings.Default.classificatePrograms)
				{
					string subDir;

					subDir		= PathHelper.ToFileTitle(mParent.title);

					storeDir	= Path.Combine( storeDir, subDir );

					if (!Directory.Exists(storeDir))
						Directory.CreateDirectory(storeDir);
				}

				storeName = Path.Combine( storeDir, GetFormattedFileName() + Path.GetExtension(mFilePath) );

				File.Move( mFilePath + ".", storeName );

				//----------------------------------
				// 付属するファイルも一緒に移動する
				//----------------------------------
				if ( Settings.Default.copyWithOthers )
				{
					string []files;

					files = Directory.GetFiles(
								Path.GetDirectoryName( mFilePath ) + ".",
								Path.GetFileNameWithoutExtension( mFilePath ) + ".*" );
				
					foreach( string f in files )
					{
						// ファイル名"A.B.C"のうち多重拡張子".B.C"を切り出す
						string	fname;
						string	multiExt;
						for(fname = f;;)
						{
							if( string.IsNullOrEmpty( Path.GetExtension( fname ) ) )
								break;
							fname = Path.GetFileNameWithoutExtension(fname);
						}
						multiExt = Path.GetFileName(f).Substring( fname.Length );

						File.Move( f, Path.Combine( storeDir, GetFormattedFileName() + multiExt ) );
					}
				
				}

				//----------------------------------
				// 元のサブディレクトリを削除する
				//----------------------------------
				if ( Settings.Default.captureSubDir &&
					 Settings.Default.removeSubdir )
				{
					// 場所が規定の録画フォルダかチェック
					string subFolder, parentFolder;

					subFolder		= Path.GetDirectoryName( mFilePath + "." ) + ".";
					parentFolder	= Directory.GetParent( subFolder ).FullName;

					if ( Settings.Default.captureFolder.Equals( parentFolder ) )
					{
						string[] files;

						// まずディレクトリ内のファイルを全て削除しておく
						files = Directory.GetFiles( subFolder );
						foreach ( string f in files )
							File.Delete( f + "." );

						Directory.Delete( subFolder, true );
					}
				}

				this.mIsStored	= true;
				this.FilePath	= storeName;
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				this.mIsBusy	= false;
				this.Dirty		= true;
			}
		}

		//=========================================================================
		///	<summary>
		///		動画ファイルを規定のファイル名にリネームする
		///	</summary>
		/// <remarks>
		///		保存先への移動は行わない。
		/// </remarks>
		/// <history>2008/10/22 新規作成</history>
		//=========================================================================
		public bool RenameFile( out string newName )
		{
			string		storeName;

			newName = null;

			if( !mHasFile )
				return false;

			if( !File.Exists( mFilePath ) )
				return false;

			try
			{
				string subDir = Path.GetDirectoryName(mFilePath);

				newName = Path.Combine(subDir, GetFormattedFileName() + Path.GetExtension(mFilePath));

				File.Move(mFilePath + ".", newName);

				mFilePath = newName;

			}
			catch (Exception)
			{
				newName = null;
				return false;
			}
			finally
			{
				this.Dirty = true;
			}

			return true;
		}

		//=========================================================================
		///	<summary>
		///		XMLへ書き出す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void Write(System.Xml.XmlWriter xw)
		{
			xw.WriteStartElement("Records");

			xw.WriteElementString("StoryNumber"		, Convert.ToString(mStoryNumber));
			xw.WriteElementString("FilePath"		, mFilePath);
			xw.WriteElementString("DateTime"		, Convert.ToString(mStartTime.Ticks));
			xw.WriteElementString("SubTitle"		, mSubTitle);
			xw.WriteElementString("Unread"			, mIsUnread ? "1":"0" );

			xw.WriteElementString("ThumbnailMaked"	, mThumbnailMaked ?"1":"0");

			xw.WriteElementString("HasPlan"			, mHasPlan.ToString()		);
			xw.WriteElementString("IsReserved"		, mIsReserved.ToString()	);
			xw.WriteElementString("ReserveStartTime", Convert.ToString(mReserveStartTime.Ticks));
			xw.WriteElementString("HasFile"			, mHasFile.ToString()		);
			xw.WriteElementString("IsReserveError"	, mIsReserveError.ToString());
			xw.WriteElementString("IsEncoded"		, mIsEncoded.ToString()		);
			xw.WriteElementString("IsStored"		, mIsStored.ToString()		);
			xw.WriteElementString("Length"			, mLength.ToString()		);
			xw.WriteElementString("RepeatNumber"	, mRepeatNumber.ToString()	);
			xw.WriteElementString("PlanError"		, mPlanError.ToString()		);

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
			while (xr.Read())
			{
				if (xr.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (xr.LocalName.Equals("StoryNumber"))
						mStoryNumber = xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("FilePath"))
						mFilePath = xr.ReadElementContentAsString();
					// Ver1.9.18以前からの以降
					else if (xr.LocalName.Equals("State"))
						MigrationState( (State)xr.ReadElementContentAsInt() );
					else if (xr.LocalName.Equals("SubTitle"))
						mSubTitle = xr.ReadElementContentAsString();
					else if (xr.LocalName.Equals("Unread"))
						mIsUnread = xr.ReadElementContentAsInt() == 1 ? true : false;
					else if (xr.LocalName.Equals("ThumbnailMaked"))
						mThumbnailMaked = xr.ReadElementContentAsInt() == 1 ? true : false;
					else if (xr.LocalName.Equals("DateTime"))
					{
						string strDateTime = xr.ReadElementContentAsString();
						mStartTime = new DateTime(Convert.ToInt64(strDateTime));
					}
					else if (xr.LocalName.Equals("HasPlan"))
						mHasPlan			= bool.Parse( xr.ReadElementContentAsString() );
					else if (xr.LocalName.Equals("IsReserved"))
						mIsReserved			= bool.Parse( xr.ReadElementContentAsString() );
					else if (xr.LocalName.Equals("ReserveStartTime"))
						mReserveStartTime	= new DateTime( Convert.ToInt64( xr.ReadElementContentAsString() ) );
					else if (xr.LocalName.Equals("HasFile"))
						mHasFile			= bool.Parse( xr.ReadElementContentAsString() );
					else if (xr.LocalName.Equals("IsReserveError"))
						mIsReserveError		= bool.Parse( xr.ReadElementContentAsString() );
					else if (xr.LocalName.Equals("IsEncoded"))
						mIsEncoded			= bool.Parse( xr.ReadElementContentAsString() );
					else if (xr.LocalName.Equals("IsStored"))
						mIsStored			= bool.Parse( xr.ReadElementContentAsString() );
					else if (xr.LocalName.Equals("Length"))
						mLength				= xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("RepeatNumber"))
						mRepeatNumber		= xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("PlanError"))
						PlanError			= bool.Parse( xr.ReadElementContentAsString() );
				}
				else if (xr.NodeType == System.Xml.XmlNodeType.EndElement)
					if (xr.LocalName.Equals("Records"))
						break;
			}

		}

		// Ver1.9.18以前からのデータ移行用
		private void MigrationState( State state )
		{
			this.mHasPlan		= false;
			this.mHasFile		= false;
			this.mIsReserved	= false;
			this.mIsReserveError= false;
			this.mIsEncoded		= false;
			this.mIsStored		= false;

			switch( state )
			{
			case State.Notfound:
				this.mHasPlan		= true;
				break;
			case State.Planned:
				this.mHasPlan		= true;
				break;
			case State.Recorded:
				this.mHasPlan		= true;
				this.mHasFile		= true;
				break;
			case State.Encoded:
				this.mHasPlan		= true;
				this.mHasFile		= true;
				this.mIsEncoded		= true;
				break;
			case State.Stored:
				this.mHasPlan		= true;
				this.mHasFile		= true;
				this.mIsStored		= true;
				break;
			case State.Scheduling:
				this.mHasPlan		= true;
				this.mHasPlan		= true;
				this.mIsReserved	= true;
				break;
			case State.Changed:
				this.mHasPlan		= true;
				this.mIsReserved	= true;
				this.mReserveStartTime = DateTime.MinValue;	// <PENDING> 2009/12/28
				break;
			case State.LostSchedule:
				this.mHasPlan			= true;
				this.mIsReserveError	= true;
				break;
			}

		}
	}


}
