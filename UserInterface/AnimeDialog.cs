//=========================================================================
///	<summary>
///		mAgicAnime番組プロパティ画面 モジュール
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
using System.Drawing;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using magicAnime.UserInterface;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		mAgicAnime番組プロパティ画面クラス
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	//=========================================================================
	internal partial class AnimeDialog : Form
	{
		//--------------------
		// メンバ変数
		//--------------------
		private AnimeProgram			mProgram;
		private SyoboiCalender			mSyoboCal;
		private int						mSelectedTid;
		private Hashtable				mEncoderTable;
		private Form					mEncoderForm;
		private Type					mCurrentEncoder;					// 現在のエンコーダ設定
		private EncodeProfile			mCurrentProfile;
		private Scheduler.ProfilePage	mProfilePage;
		private int						mZappingTid				= 0;		// 選択TID
		private string					mSelectTvStation		= null;
		private string					mInputTitle				= null;
		private bool					mLinkOnline				= false;
		private bool					mCheckedRecordTvStation	= false;

		private Thread					mThreadQuerySequence	= null;
		private ManualResetEvent		mSequenceEndFlag		= new ManualResetEvent( false );

		private class Status
		{
			internal bool	busyQueryInfo;				// 番組データを取得中
		}
		private Status					mStatus					= new Status();
		private System.Windows.Forms.Timer	mUpdateTimer		= null;

		//=========================================================================
		///	<summary>
		///		コンストラクタ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public AnimeDialog()
		{
			InitializeComponent();

			mStatus.busyQueryInfo	= false;
		}
		
		//=========================================================================
		///	<summary>
		///		ダイアログを開く時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public DialogResult ShowDialog(ref AnimeProgram prog )
		{
			mSyoboCal = AnimeServer.GetInstance().mSyoboiCalender;

			mProgram = prog;

			//-------------------
			// 共通項目
			//-------------------
			mLinkOnline = prog.linkOnlineDatabase;
			linkOnlineDatabaseCheckBox.Checked = mLinkOnline;

			//-------------------
			// "しょぼかる"タブ
			//-------------------
			specifyLatestRadio.Checked	= (prog.syobocalPolicy == AnimeProgram.SyobocalPolicy.SpecifyLatest);
			specifyNumberRadio.Checked	= (prog.syobocalPolicy == AnimeProgram.SyobocalPolicy.SpecifyNumber);
			specifyNumberUpdown.Value	= prog.syobocalSpecifyNumber;
			specifyEarlyRadioButton.Checked = (prog.syobocalPolicy == AnimeProgram.SyobocalPolicy.SpecifyEarly);

			//-------------------
			// "放送"タブ
			//-------------------

			if ( !string.IsNullOrEmpty( mProgram.title ) )
			{
				if( mLinkOnline )
					mInputTitle = mProgram.title;
				else
					titleTextBox.Text = mProgram.title;
			}
			storyCountComboBox.Text = string.Format("{0:0}", mProgram.StoryCount);

			mSelectTvStation				= mProgram.syoboiTvStation;
			recordTvStationTextBox.Text		= mProgram.tvStation;
			mCheckedRecordTvStation			= (mProgram.syoboiTvStation != mProgram.tvStation);
			recordTvStationCheckBox.Checked = mCheckedRecordTvStation;

			syoboiTidUpdown.Value = mProgram.syoboiTid;

			//-------------------
			// "録画予約"タブ
			//-------------------
			Scheduler sched = ReserveManager.DefaultScheduler;
			List<string> stations;

			if (sched != null)
			{
				//----------------------------------------
				// 録画プラグインのプロパティページ表示
				//----------------------------------------
				if ( sched.ProfilePageType != null )
				{
					mProfilePage = (Scheduler.ProfilePage)Activator.CreateInstance( sched.ProfilePageType );
					mProfilePage.Create( reservePanel );

					mProfilePage.Load( mProgram.SchedulerProfile( sched.ProfileType ) );
				}
			}

			tvStationComboBox_SelectedIndexChanged( null,null );	// 放送ページのTV局コンボを初期化

			adjustStartTimeUpdown.Value	= mProgram.adjustStartTime;
			adjustEndTimeUpdown.Value	= mProgram.adjustEndTime;

			//-------------------
			// "エンコード"タブ
			//-------------------
			string selectedName = null;

			mCurrentEncoder	= mProgram.EncoderType;
			mCurrentProfile	= mProgram.EncoderProfile;

			mEncoderTable = new Hashtable();
			
			//--------------------------
			// エンコーダ一覧を表示
			//--------------------------
			foreach (Type encoderType in EncodeManager.EncoderList)
			{
				Encoder encoder;

				encoder = (Encoder)Activator.CreateInstance(encoderType);

				if (mCurrentEncoder == encoderType)
				{
					selectedName = encoder.Name;
				}

				encoderComboBox.Items.Add(encoder.Name);

				mEncoderTable.Add(encoder.Name, encoder);
			}

			if (EncodeManager.EncoderList.Count == 0)
			{
				encodeCheckBox.Enabled = false;
			}else{
				encoderComboBox.SelectedIndex = 0;
				if (selectedName != null)
					encoderComboBox.Text = selectedName;	// 現在のエンコーダを選択
			}

			if (mProgram.EncoderProfile !=null)				// 再エンコードチェックボックス
			{
				encodeCheckBox.Checked = true;
			}


			//-------------------
			// "その他"タブ
			//-------------------

			WithoutPowerCheckBox.Checked	= mProgram.WithoutPower;
			filterKeywordCheckBox.Checked	= mProgram.enableFilterKeyword;
			filterKeywordTextBox.Text		= mProgram.filterKeyword;

			// コントロールの禁則を更新
			specifyNumberRadio_CheckedChanged(this, null);
			filterKeywordCheckBox_CheckedChanged(this,null);
			recordTvStation_CheckedChanged(this,null);

			try
			{
				PriorityCombobox.SelectedIndex = 4 - ((mProgram.priority - 10) / 10);
			}
			catch(Exception ex)
			{
				PriorityCombobox.SelectedIndex = 2;
			}

			RefreshControl();

			return ShowDialog();
		}

		//=========================================================================
		///	<summary>
		///		[OK]ボタンの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void okButton_Click(object sender, EventArgs e)
		{
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			this.DialogResult	= DialogResult.OK;

			try
			{
				//------------------
				// "放送"タブ
				//------------------

				mProgram.title = titleTextBox.Text;
				if (int.Parse(storyCountComboBox.Text) < 0)
					throw new Exception("パラメータが不正です");
				mProgram.StoryCount = int.Parse(storyCountComboBox.Text);

				mProgram.syoboiTvStation	= syoboiTvStationComboBox.Text;
				if( !mLinkOnline || mLinkOnline && recordTvStationCheckBox.Checked )
					mProgram.tvStation		= recordTvStationTextBox.Text;
				else
					mProgram.tvStation		= syoboiTvStationComboBox.Text;

				//-------------------
				// "しょぼかる"タブ
				//-------------------
				if( specifyLatestRadio.Checked )
					mProgram.syobocalPolicy = AnimeProgram.SyobocalPolicy.SpecifyLatest;
				else if( specifyNumberRadio.Checked )
					mProgram.syobocalPolicy = AnimeProgram.SyobocalPolicy.SpecifyNumber;
				else if( specifyEarlyRadioButton.Checked )
					mProgram.syobocalPolicy = AnimeProgram.SyobocalPolicy.SpecifyEarly;

				mProgram.syobocalSpecifyNumber = (int)specifyNumberUpdown.Value;

				//------------------
				// "録画"タブ
				//------------------

				if ( mProfilePage != null )
				{
					Scheduler sched = ReserveManager.DefaultScheduler;
					mProfilePage.Save( mProgram.SchedulerProfile( sched.ProfileType ) );
				}

				mProgram.adjustStartTime	= (long)adjustStartTimeUpdown.Value;
				mProgram.adjustEndTime		= (long)adjustEndTimeUpdown.Value;

				//------------------
				// エンコードタブ
				//------------------

				mProgram.EncoderType	= mCurrentEncoder;
				if ( mCurrentEncoder != null )
					mProgram.EncoderProfile	= mCurrentProfile;		// 再エンコあり
				else
					mProgram.EncoderProfile = null;					// 再エンコなし

				//------------------
				// 共通
				//------------------

				mProgram.linkOnlineDatabase	= mLinkOnline;
				mProgram.syoboiTid			= mSelectedTid;

				//------------------
				// その他
				//------------------

				mProgram.WithoutPower = WithoutPowerCheckBox.Checked;
				mProgram.enableFilterKeyword	= filterKeywordCheckBox.Checked;
				mProgram.filterKeyword			= filterKeywordTextBox.Text;

				if( 0 <= PriorityCombobox.SelectedIndex )
					mProgram.priority			= ((4 - PriorityCombobox.SelectedIndex) + 1) * 10;
				else
					mProgram.priority			= 30;
	
				// 番組のデータ更新
				mProgram.UpdatePlan(DateTime.Now);

				Close();

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				this.DialogResult = DialogResult.Cancel;
			}
			finally
			{
				Cursor.Current = oldCursor;
			}

		}
		
		//=========================================================================
		///	<summary>
		///		オンラインDBと連動チェックボックスの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void linkOnlineDatabaseCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			mLinkOnline = linkOnlineDatabaseCheckBox.Checked;

			RefreshControl();
		}

		//=========================================================================
		///	<summary>
		///		再エンコードチェックボックスの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void encodeCheckBox_CheckedChanged_1(object sender, EventArgs e)
		{
			encoderComboBox_SelectedIndexChanged(sender,e);

			RefreshControl();
		}

		//=========================================================================
		///	<summary>
		///		エンコーダ選択コンボボックスが変更されたときの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void encoderComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Encoder encoder = null;

			if (encodeCheckBox.Checked)
				encoder = (Encoder)mEncoderTable[encoderComboBox.Text];

			if (mEncoderForm != null)
			{
				//
				// エンコーダが選択から外されたらプロパティページを削除
				//
				mEncoderForm.Close();

				mCurrentEncoder = null;
			}

			if (encoder != null)
			{
				//--------------------------------------------
				// 選択されたエンコーダのプロパティページ作成
				//--------------------------------------------
				mCurrentEncoder = encoder.GetType();

				if (mCurrentEncoder == mProgram.EncoderType &&
					mProgram.EncoderProfile != null)
				{
					mCurrentProfile = mProgram.EncoderProfile;
				}
				else
				{
					//-------------------------------------
					// 選択エンコーダのプロファイルを作成
					//-------------------------------------
					mCurrentProfile = (EncodeProfile)Activator.CreateInstance(encoder.ProfileType);
				}

				//------------------------------
				// プロファイル設定ページを表示
				//------------------------------
				mEncoderForm = encoder.CreatePropertyPage(
								encoderPanel	,
								mCurrentProfile	);

			}
			else {
				mEncoderForm = null;
			}

		}

		//=========================================================================
		///	<summary>
		///		[番組一覧]ボタンの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void syoboiListButton_Click(object sender, EventArgs e)
		{
			SyoboiListDlg dlg = new SyoboiListDlg();

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				syoboiTidUpdown.Value = dlg.tid;
			}

		}

		//=========================================================================
		///	<summary>
		///		しょぼかるTIDが変更された時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void syoboiTidUpdown_ValueChanged(object sender, EventArgs e)
		{
			mZappingTid = (int)syoboiTidUpdown.Value;
		}

		//=========================================================================
		///	<summary>
		///		番組ページの視聴テレビ局コンボを変更した時の処理
		///	</summary>
		/// <remarks>
		///		「視聴テレビ局」は主にしょぼかるデータを参照するためのテレビ局名。
		///		「録画テレビ局」は録画ソフトに渡すためのテレビ局名。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void tvStationComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			// 録画テレビ局名テキストボックスに代入
			if( mLinkOnline && !recordTvStationCheckBox.Checked )
				recordTvStationTextBox.Text = syoboiTvStationComboBox.Text;
		}

		private void registCheckBox_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void specifyNumberRadio_CheckedChanged(object sender, EventArgs e)
		{
			specifyNumberUpdown.Enabled = specifyNumberRadio.Checked;
		}

		private void filterKeywordCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			filterKeywordTextBox.Enabled = filterKeywordCheckBox.Checked;
		}

		private void recordTvStation_CheckedChanged(object sender, EventArgs e)
		{
			mCheckedRecordTvStation = recordTvStationCheckBox.Checked;
			RefreshControl();
		}

		private void recordTvStationTextBox_TextChanged(object sender, EventArgs e)
		{
		}

		//=========================================================================
		///	<summary>
		///		オンライン番組情報の取得シーケンス(非同期)
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/10/18 新規作成</history>
		//=========================================================================
		delegate void LetProgramInformation();
		private void DoQueryInfoSequence()
		{
            for (; ; )
			{
                if (mSequenceEndFlag.WaitOne( 0, false ))
                    break;

// <PENDING> 2009/11/24 仮想デスクトップソフトからウィンドウが消される対策 ->
				if( !this.Visible )
					break;
// <PENDING> 2009/11/24 <-

				if( mZappingTid != mSelectedTid && mLinkOnline )
				{
					List<SyoboiCalender.SyoboiRecord>	recordList		= null; 
					ArrayList							tvStationList	= null;

                    lock (mStatus) { mStatus.busyQueryInfo = true; }

					//-----------------------------------
					// 選択された番組の放送局一覧を更新
					//-----------------------------------
					try
					{
						string title = "";

						LetProgramInformation setWaitingMsg = delegate() { syoboiTitleTextBox.Text = "(データ取得中)"; };
						this.Invoke(setWaitingMsg);

						// 番組データを取得
						if (0 < mZappingTid)
						{
							mSelectedTid = mZappingTid;

							recordList		= mSyoboCal.DownloadOnAirList(mSelectedTid, out title);
							tvStationList	= mSyoboCal.ListupTvStation(recordList);
						}

						// 番組データを画面にセット
						LetProgramInformation proc = delegate()
						{
							syoboiTvStationComboBox.Items.Clear();
							syoboiTvStationComboBox.Items.Add("(指定なし)");
							syoboiTvStationComboBox.SelectedIndex = 0;

							if( 0 < mZappingTid )
							{
								syoboiTitleTextBox.Text	= title;
								titleTextBox.Text		= title;

								foreach (string tvStation in tvStationList)
									syoboiTvStationComboBox.Items.Add(tvStation);

								if (mSelectTvStation != null)
								{
									syoboiTvStationComboBox.Text = mSelectTvStation;
									mSelectTvStation = null;
								}

								// 話数を表示
								int maxNumber = 0;

								foreach (SyoboiCalender.SyoboiRecord record in recordList)
									maxNumber = System.Math.Max(maxNumber, record.number);

								storyCountComboBox.Text = Convert.ToString(maxNumber);

								if (mInputTitle != null)
								{
									titleTextBox.Text = mInputTitle;
									mInputTitle=  null;
								}
							}
						};

                        this.Invoke(proc);

					}
					catch (Exception x)
					{
//						Program.ShowException(x, MessageBoxIcon.Error);
					}

					lock (mStatus) { mStatus.busyQueryInfo = false; }
				}

				Thread.Sleep( 200 );
			}
		}

		//=========================================================================
		///	<summary>
		///		ダイアログを閉じる時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/10/18 新規作成</history>
		//=========================================================================
		private void AnimeDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			mSequenceEndFlag.Set();
			mThreadQuerySequence = null;
		}

		//=========================================================================
		///	<summary>
		///		ダイアログを表示する際の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/10/18 新規作成</history>
		//=========================================================================
		private void AnimeDialog_Shown(object sender, EventArgs e)
		{
			if (mThreadQuerySequence == null)
			{
                mThreadQuerySequence = new Thread(new ThreadStart(DoQueryInfoSequence));
                mThreadQuerySequence.Start();
            }

			if (mUpdateTimer == null)
			{
				mUpdateTimer = new System.Windows.Forms.Timer();
                mUpdateTimer.Tick += new EventHandler(OnUpdateTimer);
                mUpdateTimer.Interval = 100;
				mUpdateTimer.Start();
            }
        }

		//=========================================================================
		///	<summary>
		///		表示更新処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/10/18 新規作成</history>
		//=========================================================================
		private void OnUpdateTimer(object sender, EventArgs args)
		{
			RefreshControl();
		}

		//=========================================================================
		///	<summary>
		///		テレビ局名変換テーブル画面を開く
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/03/28 新規作成</history>
		//=========================================================================
		private void stationTableButton_Click(object sender, EventArgs e)
		{
			StationTableDialog	dlg;
			string				stationName;

			if( recordTvStationCheckBox.Checked )
				stationName = recordTvStationTextBox.Text;
            else
                if( 0 < syoboiTvStationComboBox.SelectedIndex )
					stationName = syoboiTvStationComboBox.Text;
				else
					return;

			dlg = new StationTableDialog(stationName);

			dlg.ShowDialog();
		}
		//=========================================================================
		///	<summary>
		///		コントロールの状態更新
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/05/03 新規作成</history>
		//=========================================================================
		private void RefreshControl()
		{
			okButton.Enabled				= !mStatus.busyQueryInfo;
			syoboiTvStationComboBox.Enabled = !mStatus.busyQueryInfo && mLinkOnline;
			storyCountComboBox.Enabled		= !mStatus.busyQueryInfo;
			titleTextBox.Enabled			= !mStatus.busyQueryInfo;
			stationTableButton.Enabled		= !mStatus.busyQueryInfo;

//			waitingLabel.Visible			= mStatus.busyQueryInfo;
			encoderGroup.Enabled			= encodeCheckBox.Checked;
			recordTvStationTextBox.Enabled	= (mLinkOnline && mCheckedRecordTvStation) || !mLinkOnline;
			recordTvStationCheckBox.Enabled	= mLinkOnline;
		}

	}

}

