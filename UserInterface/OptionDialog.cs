//=========================================================================
///	<summary>
///		mAgicAnimeオプション画面 モジュール
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成</history>
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
using System.Configuration;
using magicAnime.Properties;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		mAgicAnimeオプション画面 クラス
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	//=========================================================================
	partial class OptionDialog : Form
	{
		//-------------------
		// メンバ変数
		//-------------------
		private Hashtable				mSchedulerTable;
		private Hashtable				mEncoderTable;
		private ExternalToolsSetting	mExtTools;
		private List<int>				mUntilOnAirMinutes = new List<int>();

		//=========================================================================
		///	<summary>
		///		コンストラクタ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public OptionDialog()
		{
			InitializeComponent();
		}

		//=========================================================================
		///	<summary>
		///		画面がロードされた時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void OptionForm_Load(object sender, EventArgs e)
		{

			//-------------------------
			// "録画システム"タブ
			//-------------------------
			{
				schedulerComboBox.Items.Clear();
				schedulerComboBox.Items.Add("(なし)");
				schedulerComboBox.SelectedIndex = 0;

				mSchedulerTable = new Hashtable();
				foreach ( Scheduler scheduler in ReserveManager.SchedulerList)
				{
					schedulerComboBox.Items.Add(scheduler.Name);

					mSchedulerTable[scheduler.Name] = scheduler;

					if ( scheduler.GetType().Name.Equals( Settings.Default.schedulerType ) )
					{
						schedulerComboBox.SelectedItem = scheduler.Name;
					}
				}

				reserveStartAddUpdown.Value = Settings.Default.reserveStart;
				reserveEndUpdown.Value		= Settings.Default.reserveEnd;

				reserveDaysUpdown.Value		= Settings.Default.reserveDays;
				reserveNDaysFromNowRadiobutton.Checked		=
					(Settings.Default.reserveControl == ReserveControl.nDaysFromNow);
				reserveImmediatlyBeforeRadiobutton.Checked	=
					(Settings.Default.reserveControl == ReserveControl.ImmediatlyBefore);
				reserveNoAutoRadiobutton.Checked			=
					(Settings.Default.reserveControl == ReserveControl.noAutoReserve);

				reserveImmediatlyBeforeMinUpdown.Value	= Settings.Default.reserveImmediatlyBeforeMinutes;
			}

			//-------------------------
			// "予約詳細"タブ
			//-------------------------
			tunerUpdown.Value				= Settings.Default.overlapThreshould;
			overlapMarginUpdown.Value		= Settings.Default.overlapMargin;
			enablePriorityCheckbox.Checked = Settings.Default.enablePriority;

			//-------------------------
			// "録画ファイル"タブ
			//-------------------------
			{
				captureMerginUpDown.Value				= Settings.Default.captureMergin;		// 
				captureExtensionComboBox.Text			= Settings.Default.strExtension.Substring(1);	// 
				captureSubDirOptionBox.Checked			= Settings.Default.captureSubDir;		// 
				captureFolderTextBox.Text				= Settings.Default.captureFolder;		// 

				RadioButton []specFileRadios =
				{
					specifiedByTimeRadioButton,
					specifiedByNameRadioButton,
					specifyByEndTimeRadioButton,
				};

				for( int i = 0 ; i < specFileRadios.Length ; ++i )
					specFileRadios[i].Tag = i;

				specFileRadios[(int)Settings.Default.specifiedFile].Checked = true;

				exclamationZenkakuCheckBox.Checked = 0 < (Settings.Default.fileTitleMode & 1);
			}

			//-------------------------
			// "エンコード"タブ
			//-------------------------
			{
				mEncoderTable = new Hashtable();
				
				foreach(Type encoderType in EncodeManager.EncoderList)
				{
					Encoder encoder;
					encoder = (Encoder)Activator.CreateInstance(encoderType);

					encoderListBox.Items.Add( encoder.Name );

					mEncoderTable.Add( encoder.Name, encoder );
				}

				if (EncodeManager.EncoderList.Count == 0)
				{
					encoderListBox.Enabled = false;
					EncoderOptionButton.Enabled = false;
				}
				else {
					encoderListBox.SelectedIndex = 0;
				}

				concurrentNumsUpdown.Value						= Settings.Default.concurrentNums;

				encodedFolderTextBox.Text						= Settings.Default.encodedFolder;
				autoUpdateCheckBox.Checked						= Settings.Default.autoUpdate;
				prohibitUpdateWhileRecordingCheckBox.Checked	= Settings.Default.prohibitUpdateWhileRecording;
				removeSourceWhenEncodedCheckBox.Checked			= Settings.Default.removeSourceWhenEncoded;
			}

			scheduleEncodeEverydayCheckBox.Checked		= Settings.Default.scheduleEncodeEveryday;
			scheduleEncodeHour.Text						= Convert.ToString(Settings.Default.scheduleEncodeTime / 60);
			scheduleEncodeMinute.Text					= Convert.ToString(Settings.Default.scheduleEncodeTime % 60);
			autoShutdownAtEncodedCheckbox.Checked		= Settings.Default.autoShutdownAtEncoded;
			autoShutdownTypeComboBox.SelectedIndex		= Settings.Default.autoShutdownType;

			autoEncodeInAfterRecordCheckBox.Checked		= Settings.Default.autoEncodeInAfterRecord;
			autoEncodeNumeric.Value						= Settings.Default.autoEncodeMergin;
			dontBeginLessThanCheckBox.Checked			= Settings.Default.dontBeginEncodeLessThanMinutes;
			dontBeginLessThanMinutesUpdown.Value		= Settings.Default.maxDelayTimeDontBeginEncode;

			autoEncodeInAfterRecordCheckBox_CheckedChanged(null,null);

			//----------------------
			// "保存ファイル"タブ
			//----------------------

			saveFolderTextBox.Text					= Settings.Default.saveFolder;
			classificateCheckBox.Checked			= Settings.Default.classificatePrograms;
			saveNameFormatComboBox.Text				= Settings.Default.saveNameFormat;
			saveNameFormatComboBox_TextUpdate(this, null);

			scheduleFirstHourComboBox.Text			= Convert.ToString(Settings.Default.scheduleFirstHour);
			scheduleTimeZoneComboBox.Text			= Convert.ToString(Settings.Default.scheduleTimeZone);

			//-------------------------
			// 「電源管理」タブ
			//-------------------------
			autoPowerManagementCheckBox.Checked		= Settings.Default.autoPowerManagement;
			bootTimeUnificationUpdown.Value			= Settings.Default.bootupTimeUnification;
			shutdownMethodComboBox.SelectedIndex	= (int)Settings.Default.shutdownMethod;
			bootupHastenUpdown.Value				= Settings.Default.bootupHasten;
			shutdownPutoffUpdown.Value				= (Decimal)Settings.Default.shutdownPutoff;
			autoBootupCheckbox.Checked				= Settings.Default.autoBootup;
			autoShutdownCheckbox.Checked			= Settings.Default.autoShutdown;
			autoBootNDaysUpdown.Value				= Settings.Default.autoBootNDays;

			schedulerComboBox_SelectedIndexChanged(null, null);
			autoBootWhenRecordingCheckBox_CheckedChanged(null, null);

			ignoreExistCheckBox.Checked					= Settings.Default.ignoreExist;
			copyWithOthersCheckBox.Checked				= Settings.Default.copyWithOthers;
			removeSubdirCheckBox.Checked				= Settings.Default.removeSubdir;
			autoTransferAtBootCheckBox.Checked			= Settings.Default.autoTransferAtBoot;
			autoTransferInAfterRecordCheckBox.Checked	= Settings.Default.autoTransferAtAfterRecord;
			autoTransferTimeNumeric.Value				= Settings.Default.autoTransferTime;
			autoMoveWithoutFirstCheckBox.Checked		= Settings.Default.autoMoveWithoutFirstEpisode;

			//-------------------------
			// オンラインタブ
			//-------------------------

			syoboiTitleUrlTextBox.Text	= Settings.Default.syoboiTitleList;
			syoboiTidUrlTextBox.Text	= Settings.Default.syoboiTid;
			syoboiRssTextBox.Text		= Settings.Default.syoboiRss;
			syoboiProgInfoTextBox.Text	= Settings.Default.syoboiProg;

			//-------------------------
			// グリッドビュータブ
			//-------------------------

			thumbnailSubtitleCheckBox.Checked	= Settings.Default.thumbnailSubtitle;
			thumbnailSecondUpdown.Value			= Settings.Default.thumbnailSecond;
			thumbnailSizeComboBox.SelectedIndex	= Settings.Default.thumbnailSize;

			forbiddenThumbnailCheckBox.Checked	= Settings.Default.forbiddenThumbnail;
			makeThumbnailAfterEncodeCheckBox.Checked
												= Settings.Default.makeThumbnailAfterEncode;

			//-------------------------
			// "外部ツール"タブ
			//-------------------------
			if (Settings.Default.externalTools != null)
				mExtTools = (ExternalToolsSetting)Settings.Default.externalTools.Clone();
			else
				mExtTools = new ExternalToolsSetting();

			//-------------------------
			// "データ更新"タブ
			//-------------------------
			bootRefreshCheckBox.Checked		= Settings.Default.bootRefresh;
			autoRefreshCheckBox.Checked		= Settings.Default.autoRefresh;
			updateIntervalUpdown.Value		= Settings.Default.updateInterval;
			mUntilOnAirMinutes				= Settings.Default.untilOnAirMinutes;
			updateOnAirSoonCheckBox.Checked	= Settings.Default.updateOnAirSoon;
			SetUntilOnAirMinutesTextBox();

			//-------------------------
			// "その他"タブ
			//-------------------------

			inTaskTrayCheckBox.Checked		= Settings.Default.inTaskTray;
			disableUnreadCheckBox.Checked	= Settings.Default.disableUnread;
			notifyUnreadCheckBox.Checked	= Settings.Default.notifyUnreadBalloon;
			popupSoonBalloonCheckBox.Checked= Settings.Default.showSoonBalloon;
			timeSoonBalloonUpdown.Value		= Settings.Default.timeSoonBalloon;

			hoursPerDayUpDown.Value			= Settings.Default.hoursPerDay;

			minimizeAtStartupCheckBox.Checked		= Settings.Default.minimizeAtStartup;
			minimizeAtPlayerCheckBox.Checked		= Settings.Default.minimizeAtPlayer;

			startupWaitUpdown.Value			= Settings.Default.startupWait;

			RefreshContent();
		}

		//=========================================================================
		///	<summary>
		///		録画フォルダ[参照]ボタンの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void captureFolderButton_Click(object sender, EventArgs e)
		{
	
			folderBrowser.SelectedPath	= captureFolderTextBox.Text;
			folderBrowser.Description	= (string)captureFolderTextBox.Tag;

			if(folderBrowser.ShowDialog()==DialogResult.OK)
			{
				captureFolderTextBox.Text = folderBrowser.SelectedPath;
			}

		}

		//=========================================================================
		///	<summary>
		///		[OK]ボタンの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void OkButton_Click(object sender, EventArgs e)
		{

			//-------------------------
			// "録画システム"タブ
			//-------------------------
			{

				if (schedulerComboBox.SelectedIndex <= 0)
					Settings.Default.schedulerType = "";
				else{
					Scheduler scheduler = (Scheduler)mSchedulerTable[schedulerComboBox.Text];
					Settings.Default.schedulerType = scheduler.GetType().Name;
				}

				Settings.Default.reserveStart	= (int)reserveStartAddUpdown.Value;
				Settings.Default.reserveEnd		= (int)reserveEndUpdown.Value;
				Settings.Default.reserveDays	= (int)reserveDaysUpdown.Value;

				if( reserveNDaysFromNowRadiobutton.Checked )
					Settings.Default.reserveControl = ReserveControl.nDaysFromNow;
				else if( reserveImmediatlyBeforeRadiobutton.Checked )
					Settings.Default.reserveControl = ReserveControl.ImmediatlyBefore;
				else if (reserveNoAutoRadiobutton.Checked)
					Settings.Default.reserveControl = ReserveControl.noAutoReserve;

				Settings.Default.reserveImmediatlyBeforeMinutes = (int)reserveImmediatlyBeforeMinUpdown.Value;
			}


			//-------------------------
			// "予約詳細"タブ
			//-------------------------
			Settings.Default.overlapThreshould	= (int)tunerUpdown.Value;
			Settings.Default.overlapMargin		= (int)overlapMarginUpdown.Value;
			Settings.Default.enablePriority		= enablePriorityCheckbox.Checked;


			//-------------------------
			// "録画ファイル"タブ
			//-------------------------

			Settings.Default.captureMergin		= (int)captureMerginUpDown.Value;
			Settings.Default.captureExtension	= -1;
			Settings.Default.strExtension		= "." + captureExtensionComboBox.Text;
			Settings.Default.captureSubDir		= captureSubDirOptionBox.Checked;
			Settings.Default.captureFolder		= captureFolderTextBox.Text;

			Settings.Default.concurrentNums					= (int)concurrentNumsUpdown.Value;
			Settings.Default.encodedFolder					= encodedFolderTextBox.Text;
			Settings.Default.autoUpdate						= autoUpdateCheckBox.Checked;
			Settings.Default.prohibitUpdateWhileRecording	= prohibitUpdateWhileRecordingCheckBox.Checked;
			Settings.Default.removeSourceWhenEncoded		= removeSourceWhenEncodedCheckBox.Checked;

			Settings.Default.autoEncodeInAfterRecord		= autoEncodeInAfterRecordCheckBox.Checked;
			Settings.Default.autoEncodeMergin				= (int)autoEncodeNumeric.Value;
			Settings.Default.dontBeginEncodeLessThanMinutes = dontBeginLessThanCheckBox.Checked;
			Settings.Default.maxDelayTimeDontBeginEncode	= (int)dontBeginLessThanMinutesUpdown.Value;

			unchecked
			{
				Settings.Default.fileTitleMode		&= (uint)~1;
			}
			Settings.Default.fileTitleMode		|= (uint)(exclamationZenkakuCheckBox.Checked ? 1 : 0);

			//---------------------
			// "保存ファイル"タブ
			//---------------------

			Settings.Default.saveFolder					= saveFolderTextBox.Text;
			Settings.Default.classificatePrograms		= classificateCheckBox.Checked;
			Settings.Default.saveNameFormat				= saveNameFormatComboBox.Text;

			Settings.Default.scheduleEncodeEveryday		= scheduleEncodeEverydayCheckBox.Checked;
			Settings.Default.scheduleEncodeTime			= int.Parse(scheduleEncodeHour.Text) * 60 + int.Parse(scheduleEncodeMinute.Text);
			Settings.Default.autoShutdownAtEncoded		= autoShutdownAtEncodedCheckbox.Checked;
			Settings.Default.autoShutdownType			= autoShutdownTypeComboBox.SelectedIndex;

			Settings.Default.scheduleFirstHour			= int.Parse(scheduleFirstHourComboBox.Text);
			Settings.Default.scheduleTimeZone			= int.Parse(scheduleTimeZoneComboBox.Text);

			RadioButton []specFileRadios =
			{
				specifiedByTimeRadioButton,
				specifiedByNameRadioButton,
				specifyByEndTimeRadioButton,
			};

			foreach( RadioButton rad in specFileRadios  )
				if( rad.Checked )
					Settings.Default.specifiedFile = (IdentifyFileMethod)rad.Tag;

			Settings.Default.ignoreExist				= ignoreExistCheckBox.Checked;
			Settings.Default.copyWithOthers				= copyWithOthersCheckBox.Checked;
			Settings.Default.removeSubdir				= removeSubdirCheckBox.Checked;

			Settings.Default.autoTransferAtBoot			= autoTransferAtBootCheckBox.Checked;
			Settings.Default.autoTransferAtAfterRecord	= autoTransferInAfterRecordCheckBox.Checked;
			Settings.Default.autoTransferTime			= (int)autoTransferTimeNumeric.Value;
			Settings.Default.autoMoveWithoutFirstEpisode = autoMoveWithoutFirstCheckBox.Checked;

			//-------------------------
			// 自動起動タブ
			//-------------------------
			Settings.Default.autoPowerManagement	= autoPowerManagementCheckBox.Checked;
			Settings.Default.bootupTimeUnification	= (uint)bootTimeUnificationUpdown.Value;
			Settings.Default.shutdownMethod			= (uint)shutdownMethodComboBox.SelectedIndex;
			Settings.Default.bootupHasten			= (uint)bootupHastenUpdown.Value;
			Settings.Default.shutdownPutoff			= (uint)shutdownPutoffUpdown.Value;
			Settings.Default.autoBootup				= autoBootupCheckbox.Checked;
			Settings.Default.autoShutdown			= autoShutdownCheckbox.Checked;
			Settings.Default.autoBootNDays			= (int)autoBootNDaysUpdown.Value;

			//-------------------------
			// オンラインタブ
			//-------------------------

			Settings.Default.syoboiTitleList	= syoboiTitleUrlTextBox.Text;
			Settings.Default.syoboiTid			= syoboiTidUrlTextBox.Text;
			Settings.Default.syoboiRss			= syoboiRssTextBox.Text;
			Settings.Default.syoboiProg			= syoboiProgInfoTextBox.Text;

			//-------------------------
			// グリッドビュータブ
			//-------------------------

			Settings.Default.thumbnailSubtitle	= thumbnailSubtitleCheckBox.Checked;
			Settings.Default.thumbnailSecond	= (int)thumbnailSecondUpdown.Value;
			Settings.Default.thumbnailSize		= thumbnailSizeComboBox.SelectedIndex;

			int[] widthList		= new int[] { 60, 90, 120, 160 };
			int[] heightList	= new int[] { 45, 60,  90, 120 };
			Settings.Default.thumbnailWidth		= widthList[Settings.Default.thumbnailSize];
			Settings.Default.thumbnailHeight	= heightList[Settings.Default.thumbnailSize];

			Settings.Default.forbiddenThumbnail = forbiddenThumbnailCheckBox.Checked;
			Settings.Default.makeThumbnailAfterEncode
												= makeThumbnailAfterEncodeCheckBox.Checked;

			//-------------------------
			// "外部ツール"タブ
			//-------------------------
			Settings.Default.externalTools = (ExternalToolsSetting)mExtTools.Clone();

			//-------------------------
			// "データ更新"タブ
			//-------------------------
			Settings.Default.bootRefresh			= bootRefreshCheckBox.Checked;		// v1.7.13
			Settings.Default.autoRefresh			= autoRefreshCheckBox.Checked;
			Settings.Default.updateInterval			= (int)updateIntervalUpdown.Value;
			Settings.Default.untilOnAirMinutes		= mUntilOnAirMinutes;
			Settings.Default.updateOnAirSoon		= updateOnAirSoonCheckBox.Checked;

			//-------------------------
			// "その他"タブ
			//-------------------------

			Settings.Default.inTaskTray				= inTaskTrayCheckBox.Checked;
			Settings.Default.disableUnread			= disableUnreadCheckBox.Checked;
			Settings.Default.notifyUnreadBalloon	= notifyUnreadCheckBox.Checked;
			Settings.Default.showSoonBalloon		= popupSoonBalloonCheckBox.Checked;
			Settings.Default.timeSoonBalloon		= (int)timeSoonBalloonUpdown.Value;

			Settings.Default.hoursPerDay = (int)hoursPerDayUpDown.Value;

			Settings.Default.minimizeAtStartup			= minimizeAtStartupCheckBox.Checked;
			Settings.Default.minimizeAtPlayer			= minimizeAtPlayerCheckBox.Checked;

			Settings.Default.startupWait			= (long)startupWaitUpdown.Value;

			//-----------------------
			// 設定の保存
			//-----------------------

			Settings.Default.Save();

			this.DialogResult = DialogResult.OK;

			AnimeServer.GetInstance().ApplyOption();

			Close();
		}

		//=========================================================================
		///	<summary>
		///		キャンセルボタンの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void CancellingButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void autoUpdateCheckBox_CheckedChanged(object sender, EventArgs e)
		{
		}


		//=========================================================================
		///	<summary>
		///		エンコード先[参照]ボタンの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void encodedFolderButton_Click(object sender, EventArgs e)
		{
		
			folderBrowser.SelectedPath = encodedFolderTextBox.Text;
			folderBrowser.Description = (string)encodedFolderTextBox.Tag;

			if (folderBrowser.ShowDialog() == DialogResult.OK)
			{
				encodedFolderTextBox.Text = folderBrowser.SelectedPath;
			}

		}

		//=========================================================================
		///	<summary>
		///		保存先[参照]ボタンの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void saveFolderButton_Click(object sender, EventArgs e)
		{

			folderBrowser.SelectedPath = saveFolderTextBox.Text;
			folderBrowser.Description = (string)saveFolderTextBox.Tag;

			if (folderBrowser.ShowDialog() == DialogResult.OK)
			{
				saveFolderTextBox.Text = folderBrowser.SelectedPath;
			}

		}

		//=========================================================================
		///	<summary>
		///		保存ファイル名書式が変更された時のサンプル文字列表示
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void saveNameFormatComboBox_TextUpdate(object sender, EventArgs e)
		{
			string sampleText;

			try
			{
				sampleText = string.Format(
					saveNameFormatComboBox.Text,
					"まじあにねっと",
					1,
					"東奔西走アニメライフ",
					"20070401",
					"2500",
					"東都テレビ");

				sampleNameFormatTextBox.Text = sampleText;
			}
			catch (Exception)
			{
				sampleNameFormatTextBox.Text = "書式が正しくありません";
			}
			
		}

		//=========================================================================
		///	<summary>
		///		保存ファイル名書式が変更された時のサンプル文字列表示
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void saveNameFormatComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			saveNameFormatComboBox_TextUpdate(sender,e);
		}

		private void mAgicTVCheckBox_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void tabPage6_Click(object sender, EventArgs e)
		{
		}

		//=========================================================================
		///	<summary>
		///		録画プラグインコンボボックスが変更されたときの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void schedulerComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Scheduler scheduler;

			if (schedulerComboBox.SelectedIndex == 0)
			{
				captureExtensionComboBox.Enabled = true;

				captureFolderTextBox.Enabled = true;
				captureSubDirOptionBox.Enabled = true;
				captureFolderButton.Enabled = true;

				schedulerAbility1.Checked = false;
				schedulerAbility2.Checked = false;
				schedulerAbility3.Checked = false;
			}
			else
			{

				try
				{

					scheduler = (Scheduler)mSchedulerTable[schedulerComboBox.Text];

					if (scheduler.Extension != null)
					{
						captureExtensionComboBox.Text = scheduler.Extension.Substring(1);
					}
					else
					{
					}

					if (scheduler.Folder != null)
					{
						captureFolderTextBox.Text = scheduler.Folder;
						captureSubDirOptionBox.Checked = scheduler.SubDirectory;

						captureFolderTextBox.Enabled = false;
						captureSubDirOptionBox.Enabled = false;
						captureFolderButton.Enabled = false;
					}
					else
					{
						captureFolderTextBox.Enabled = true;
						captureSubDirOptionBox.Enabled = true;
						captureFolderButton.Enabled = true;
					}

					schedulerAbility1.Checked = (scheduler.Ability & Scheduler.AbilityFlag.MakeReservation) > 0 ? true : false;
					schedulerAbility2.Checked = (scheduler.Ability & Scheduler.AbilityFlag.ChangeReservation) > 0 ? true : false;
					schedulerAbility3.Checked = (scheduler.Ability & Scheduler.AbilityFlag.CancelReservation) > 0 ? true : false;
				}
				catch (Exception x)
				{
					MessageBox.Show(x.Message, "エラー",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}

			}

		}


		//=========================================================================
		///	<summary>
		///		電源管理チェックボックスの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void autoBootWhenRecordingCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			autoBootWhenRecordingGroup.Enabled = autoPowerManagementCheckBox.Checked;
			autoBootupCheckbox_CheckedChanged(null, null);
			autoShutdownCheckbox_CheckedChanged(null, null);
		}

		private void manualRadioBox_CheckedChanged(object sender, EventArgs e)
		{

		}

		//=========================================================================
		///	<summary>
		///		決まった時間に再エンコードチェックボックス
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void scheduleEncodeEverydayCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			encodeEverydayGroup.Enabled = scheduleEncodeEverydayCheckBox.Checked;
		}

		//=========================================================================
		///	<summary>
		///		自動的にシャットダウンチェックボックス
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void autoShutdownAtEncodedCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			autoShutdownTypeComboBox.Enabled = autoShutdownAtEncodedCheckbox.Checked;
		}

		//=========================================================================
		///	<summary>
		///		エンコーダのオプションを表示するボタン
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void EncoderOptionButton_Click(object sender, EventArgs e)
		{
			Encoder encoder;

			encoder = (Encoder)mEncoderTable[encoderListBox.Text];

			if (encoder!=null)
				encoder.ShowOptionDialog();
		}

		//=========================================================================
		///	<summary>
		///		自動更新チェックボックスの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void autoRefreshCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			autoRefreshGroup.Enabled = autoRefreshCheckBox.Checked;
		}

		//=========================================================================
		///	<summary>
		///		タスクトレイに常駐チェックボックスの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void inTaskTrayCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			taskTrayGroup.Enabled = inTaskTrayCheckBox.Checked;
		}

		//=========================================================================
		///	<summary>
		///		サブディレクトリごとチェックボックス
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void captureSubDirOptionBox_CheckedChanged( object sender, EventArgs e )
		{
			removeSubdirCheckBox.Enabled = captureSubDirOptionBox.Checked;
		}

		//=========================================================================
		///	<summary>
		///		録画後に自動保存チェックボックスの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void autoTransferAtAfterRecordCheckBox_CheckedChanged( object sender, EventArgs e )
		{
			if( autoTransferInAfterRecordCheckBox.Checked　==　true　)
			{
				ConfirmAutoUpdateON();
			}

			autoTransferTimeNumeric.Enabled = autoTransferInAfterRecordCheckBox.Checked;
		}

		//=========================================================================
		///	<summary>
		///		録画後に自動再エンコードチェックボックスの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/05/05 新規作成</history>
		//=========================================================================
		private void autoEncodeInAfterRecordCheckBox_CheckedChanged( object sender, EventArgs e )
		{
			if (autoEncodeInAfterRecordCheckBox.Checked == true )
			{
				ConfirmAutoUpdateON();
			}

			autoEncodeNumeric.Enabled = autoEncodeInAfterRecordCheckBox.Checked;
			dontBeginLessThanCheckBox.Enabled = autoEncodeInAfterRecordCheckBox.Checked;

			dontBeginLessThanCheckBox_CheckedChanged(null,null);
		}

		//=========================================================================
		///	<summary>
		///		録画ファイル作成を検知オプションを有効にさせる
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/05/05 新規作成</history>
		//=========================================================================
		private void ConfirmAutoUpdateON()
		{
			if( autoUpdateCheckBox.Checked　==　false　)
			{
				MessageBox.Show(
					"録画ファイル作成を検知するオプションを有効にします。",
					"確認",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				autoUpdateCheckBox.Checked = true;
			}

		}

		//=========================================================================
		///	<summary>
		///		「放送時間直前にポップアップ」チェックボックスが変更された時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/10/01 新規作成</history>
		//=========================================================================
		private void popupSoonBalloonCheckBox_CheckedChanged( object sender, EventArgs e )
		{
			timeSoonBalloonUpdown.Enabled = popupSoonBalloonCheckBox.Checked;
		}

		private void forbiddenThumbnailCheckBox_CheckedChanged( object sender, EventArgs e )
		{
			bool bChecked = forbiddenThumbnailCheckBox.Checked;
			makeThumbnailAfterEncodeCheckBox.Enabled = !bChecked;
		}

		private void dontBeginLessThanCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			dontBeginLessThanMinutesUpdown.Enabled =	dontBeginLessThanCheckBox.Enabled
													&&	dontBeginLessThanCheckBox.Checked;
		}

		private void autoBootupCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			RefreshControl();
		}

		private void autoShutdownCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			RefreshControl();
		}

		// ユーザー設定フォルダを開く
		private void openUserConfigFolderLinklabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
            try
            {
                string  configFolder = Program.AppConfigPath;
				System.Diagnostics.Process.Start( configFolder );
            }
            catch(Exception ex)
            {
            }
		}

		private void reserveNDaysFromNowRadiobutton_CheckedChanged(object sender, EventArgs e)
		{
			RefreshControl();
		}

		private void reserveImmediatlyBeforeRadiobutton_CheckedChanged(object sender, EventArgs e)
		{
			RefreshControl();
		}

		// コントロール状態の更新
		private void RefreshControl()
		{
			reserveDaysUpdown.Enabled					= reserveNDaysFromNowRadiobutton.Checked;
			reserveImmediatlyBeforeMinUpdown.Enabled	= reserveImmediatlyBeforeRadiobutton.Checked;

			bootupHastenUpdown.Enabled		= autoBootupCheckbox.Checked;

			shutdownPutoffUpdown.Enabled	= autoShutdownCheckbox.Checked;
			shutdownMethodComboBox.Enabled	= autoShutdownCheckbox.Checked;
		}

		// コントロール内容の更新
		private void RefreshContent()
		{
			//---------------------------
			// 拡張ツールリストの更新
			//---------------------------
			string oldSelected = extToolsListbox.Text;
			extToolsListbox.Items.Clear();

			for (int i = 0; i < mExtTools.tools.Count; ++i)
				extToolsListbox.Items.Add(mExtTools.tools[i].toolName);

			if (0 < extToolsListbox.Items.Count)
				extToolsListbox.SelectedValue = oldSelected;
		}

		// 拡張ツールリストに追加
		private void extToolsAddButton_Click(object sender, EventArgs e)
		{
			ExternalToolItem	item = new ExternalToolItem();
			item.toolName			= extToolNameTextbox.Text;
			item.toolPath			= extToolsPathTextbox.Text;
			item.toolCommandLine	= extToolCmdLineTextbox.Text;

			if (ValidExtToolItem( item ))
				mExtTools.tools.Add( item );
			else
				MessageBox.Show( "入力パラメータを確認して下さい。", null,
								MessageBoxButtons.OK, MessageBoxIcon.Warning	 );

			RefreshContent();	
		}

		// 拡張ツールリストの選択項目を置換
		private void extToolsReplaceButton_Click(object sender, EventArgs e)
		{
			if (0 <= extToolsListbox.SelectedIndex)
			{
				ExternalToolItem item = new ExternalToolItem();
				item.toolName			= extToolNameTextbox.Text;
				item.toolPath			= extToolsPathTextbox.Text;
				item.toolCommandLine	= extToolCmdLineTextbox.Text;

				if (ValidExtToolItem(item) )
					mExtTools.tools[extToolsListbox.SelectedIndex] = item;
				else
					MessageBox.Show("入力パラメータを確認して下さい。", null,
									MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
	
			RefreshContent();
		}

		private bool ValidExtToolItem( ExternalToolItem item )
		{
			return	!string.IsNullOrEmpty( item.toolName ) &&
					!string.IsNullOrEmpty( item.toolPath );
		}

		// 拡張ツールリストの選択項目を削除
		private void extToolsRemoveButton_Click(object sender, EventArgs e)
		{
			if (0 <= extToolsListbox.SelectedIndex)
			{
				mExtTools.tools.RemoveAt(extToolsListbox.SelectedIndex);
			}

			RefreshContent();
		}

		private void extToolsPathButton_Click(object sender, EventArgs e)
		{
			extToolsPathDialog.FileName = extToolsPathTextbox.Text;

			if (extToolsPathDialog.ShowDialog() == DialogResult.OK)
			{
				extToolsPathTextbox.Text = extToolsPathDialog.FileName;
			}
		}

		private void extToolsListbox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (0 <= extToolsListbox.SelectedIndex)
			{
				ExternalToolItem item = mExtTools.tools[ extToolsListbox.SelectedIndex ];

				extToolNameTextbox.Text		= item.toolName;
				extToolsPathTextbox.Text	= item.toolPath;
				extToolCmdLineTextbox.Text	= item.toolCommandLine;
			}
		}

		private void updateOnAirSoonCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			updateOnAirSoonGroupBox.Enabled  = updateOnAirSoonCheckBox.Checked;
		}

		//=========================================================================
		///	<summary>
		///		「放送直前n分前」テキストボックスの確定処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/10/01 新規作成</history>
		//=========================================================================
		private void untilOnAirTextBox_Validating(object sender, CancelEventArgs e)
		{
			string	text = untilOnAirTextBox.Text;

			string[] numList = text.Split(',');
			var untilList = new List<int>();

			try
			{
				foreach( string num in numList )
				{
					if( !string.IsNullOrEmpty( num ) )
					{
						int min = int.Parse( num );
						if( min < 1 )
							throw new Exception();
						if( 60 < min )
							throw new Exception();
						untilList.Add( min );
					}
				}

				untilList.Sort();
				mUntilOnAirMinutes = untilList;

				SetUntilOnAirMinutesTextBox();
			}
			catch(Exception ex)
			{
				e.Cancel = true;
				MessageBox.Show(
					"書式が正しくありません。",
					null,
					MessageBoxButtons.OK ,
					MessageBoxIcon.Information);
			}
		}

		private void SetUntilOnAirMinutesTextBox()
		{
			string text = "";
			foreach( int num in mUntilOnAirMinutes )
			{
				if( !string.IsNullOrEmpty( text ) )
					text += ", ";
				text += num.ToString();
			}
			untilOnAirTextBox.Text = text;

		}
		
	}
}