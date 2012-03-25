//=========================================================================
///	<summary>
///		mAgicAnime�ԑg�v���p�e�B��� ���W���[��
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬 Dr.Kurusugawa</history>
/// <history>2010/05/01 Subversion�ŊǗ����邽�ߕs�v�ȃR�����g�폜</history>
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
	///		mAgicAnime�ԑg�v���p�e�B��ʃN���X
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	internal partial class AnimeDialog : Form
	{
		//--------------------
		// �����o�ϐ�
		//--------------------
		private AnimeProgram			mProgram;
		private SyoboiCalender			mSyoboCal;
		private int						mSelectedTid;
		private Hashtable				mEncoderTable;
		private Form					mEncoderForm;
		private Type					mCurrentEncoder;					// ���݂̃G���R�[�_�ݒ�
		private EncodeProfile			mCurrentProfile;
		private Scheduler.ProfilePage	mProfilePage;
		private int						mZappingTid				= 0;		// �I��TID
		private string					mSelectTvStation		= null;
		private string					mInputTitle				= null;
		private bool					mLinkOnline				= false;
		private bool					mCheckedRecordTvStation	= false;

		private Thread					mThreadQuerySequence	= null;
		private ManualResetEvent		mSequenceEndFlag		= new ManualResetEvent( false );

		private class Status
		{
			internal bool	busyQueryInfo;				// �ԑg�f�[�^���擾��
		}
		private Status					mStatus					= new Status();
		private System.Windows.Forms.Timer	mUpdateTimer		= null;

		//=========================================================================
		///	<summary>
		///		�R���X�g���N�^
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public AnimeDialog()
		{
			InitializeComponent();

			mStatus.busyQueryInfo	= false;
		}
		
		//=========================================================================
		///	<summary>
		///		�_�C�A���O���J�����̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public DialogResult ShowDialog(ref AnimeProgram prog )
		{
			mSyoboCal = AnimeServer.GetInstance().mSyoboiCalender;

			mProgram = prog;

			//-------------------
			// ���ʍ���
			//-------------------
			mLinkOnline = prog.linkOnlineDatabase;
			linkOnlineDatabaseCheckBox.Checked = mLinkOnline;

			//-------------------
			// "����ڂ���"�^�u
			//-------------------
			specifyLatestRadio.Checked	= (prog.syobocalPolicy == AnimeProgram.SyobocalPolicy.SpecifyLatest);
			specifyNumberRadio.Checked	= (prog.syobocalPolicy == AnimeProgram.SyobocalPolicy.SpecifyNumber);
			specifyNumberUpdown.Value	= prog.syobocalSpecifyNumber;
			specifyEarlyRadioButton.Checked = (prog.syobocalPolicy == AnimeProgram.SyobocalPolicy.SpecifyEarly);

			//-------------------
			// "����"�^�u
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
			// "�^��\��"�^�u
			//-------------------
			Scheduler sched = ReserveManager.DefaultScheduler;
			List<string> stations;

			if (sched != null)
			{
				//----------------------------------------
				// �^��v���O�C���̃v���p�e�B�y�[�W�\��
				//----------------------------------------
				if ( sched.ProfilePageType != null )
				{
					mProfilePage = (Scheduler.ProfilePage)Activator.CreateInstance( sched.ProfilePageType );
					mProfilePage.Create( reservePanel );

					mProfilePage.Load( mProgram.SchedulerProfile( sched.ProfileType ) );
				}
			}

			tvStationComboBox_SelectedIndexChanged( null,null );	// �����y�[�W��TV�ǃR���{��������

			adjustStartTimeUpdown.Value	= mProgram.adjustStartTime;
			adjustEndTimeUpdown.Value	= mProgram.adjustEndTime;

			//-------------------
			// "�G���R�[�h"�^�u
			//-------------------
			string selectedName = null;

			mCurrentEncoder	= mProgram.EncoderType;
			mCurrentProfile	= mProgram.EncoderProfile;

			mEncoderTable = new Hashtable();
			
			//--------------------------
			// �G���R�[�_�ꗗ��\��
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
					encoderComboBox.Text = selectedName;	// ���݂̃G���R�[�_��I��
			}

			if (mProgram.EncoderProfile !=null)				// �ăG���R�[�h�`�F�b�N�{�b�N�X
			{
				encodeCheckBox.Checked = true;
			}


			//-------------------
			// "���̑�"�^�u
			//-------------------

			WithoutPowerCheckBox.Checked	= mProgram.WithoutPower;
			filterKeywordCheckBox.Checked	= mProgram.enableFilterKeyword;
			filterKeywordTextBox.Text		= mProgram.filterKeyword;

			// �R���g���[���֑̋����X�V
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
		///		[OK]�{�^���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void okButton_Click(object sender, EventArgs e)
		{
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			this.DialogResult	= DialogResult.OK;

			try
			{
				//------------------
				// "����"�^�u
				//------------------

				mProgram.title = titleTextBox.Text;
				if (int.Parse(storyCountComboBox.Text) < 0)
					throw new Exception("�p�����[�^���s���ł�");
				mProgram.StoryCount = int.Parse(storyCountComboBox.Text);

				mProgram.syoboiTvStation	= syoboiTvStationComboBox.Text;
				if( !mLinkOnline || mLinkOnline && recordTvStationCheckBox.Checked )
					mProgram.tvStation		= recordTvStationTextBox.Text;
				else
					mProgram.tvStation		= syoboiTvStationComboBox.Text;

				//-------------------
				// "����ڂ���"�^�u
				//-------------------
				if( specifyLatestRadio.Checked )
					mProgram.syobocalPolicy = AnimeProgram.SyobocalPolicy.SpecifyLatest;
				else if( specifyNumberRadio.Checked )
					mProgram.syobocalPolicy = AnimeProgram.SyobocalPolicy.SpecifyNumber;
				else if( specifyEarlyRadioButton.Checked )
					mProgram.syobocalPolicy = AnimeProgram.SyobocalPolicy.SpecifyEarly;

				mProgram.syobocalSpecifyNumber = (int)specifyNumberUpdown.Value;

				//------------------
				// "�^��"�^�u
				//------------------

				if ( mProfilePage != null )
				{
					Scheduler sched = ReserveManager.DefaultScheduler;
					mProfilePage.Save( mProgram.SchedulerProfile( sched.ProfileType ) );
				}

				mProgram.adjustStartTime	= (long)adjustStartTimeUpdown.Value;
				mProgram.adjustEndTime		= (long)adjustEndTimeUpdown.Value;

				//------------------
				// �G���R�[�h�^�u
				//------------------

				mProgram.EncoderType	= mCurrentEncoder;
				if ( mCurrentEncoder != null )
					mProgram.EncoderProfile	= mCurrentProfile;		// �ăG���R����
				else
					mProgram.EncoderProfile = null;					// �ăG���R�Ȃ�

				//------------------
				// ����
				//------------------

				mProgram.linkOnlineDatabase	= mLinkOnline;
				mProgram.syoboiTid			= mSelectedTid;

				//------------------
				// ���̑�
				//------------------

				mProgram.WithoutPower = WithoutPowerCheckBox.Checked;
				mProgram.enableFilterKeyword	= filterKeywordCheckBox.Checked;
				mProgram.filterKeyword			= filterKeywordTextBox.Text;

				if( 0 <= PriorityCombobox.SelectedIndex )
					mProgram.priority			= ((4 - PriorityCombobox.SelectedIndex) + 1) * 10;
				else
					mProgram.priority			= 30;
	
				// �ԑg�̃f�[�^�X�V
				mProgram.UpdatePlan(DateTime.Now);

				Close();

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "�x��", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				this.DialogResult = DialogResult.Cancel;
			}
			finally
			{
				Cursor.Current = oldCursor;
			}

		}
		
		//=========================================================================
		///	<summary>
		///		�I�����C��DB�ƘA���`�F�b�N�{�b�N�X�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void linkOnlineDatabaseCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			mLinkOnline = linkOnlineDatabaseCheckBox.Checked;

			RefreshControl();
		}

		//=========================================================================
		///	<summary>
		///		�ăG���R�[�h�`�F�b�N�{�b�N�X�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void encodeCheckBox_CheckedChanged_1(object sender, EventArgs e)
		{
			encoderComboBox_SelectedIndexChanged(sender,e);

			RefreshControl();
		}

		//=========================================================================
		///	<summary>
		///		�G���R�[�_�I���R���{�{�b�N�X���ύX���ꂽ�Ƃ��̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void encoderComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Encoder encoder = null;

			if (encodeCheckBox.Checked)
				encoder = (Encoder)mEncoderTable[encoderComboBox.Text];

			if (mEncoderForm != null)
			{
				//
				// �G���R�[�_���I������O���ꂽ��v���p�e�B�y�[�W���폜
				//
				mEncoderForm.Close();

				mCurrentEncoder = null;
			}

			if (encoder != null)
			{
				//--------------------------------------------
				// �I�����ꂽ�G���R�[�_�̃v���p�e�B�y�[�W�쐬
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
					// �I���G���R�[�_�̃v���t�@�C�����쐬
					//-------------------------------------
					mCurrentProfile = (EncodeProfile)Activator.CreateInstance(encoder.ProfileType);
				}

				//------------------------------
				// �v���t�@�C���ݒ�y�[�W��\��
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
		///		[�ԑg�ꗗ]�{�^���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
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
		///		����ڂ���TID���ύX���ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void syoboiTidUpdown_ValueChanged(object sender, EventArgs e)
		{
			mZappingTid = (int)syoboiTidUpdown.Value;
		}

		//=========================================================================
		///	<summary>
		///		�ԑg�y�[�W�̎����e���r�ǃR���{��ύX�������̏���
		///	</summary>
		/// <remarks>
		///		�u�����e���r�ǁv�͎�ɂ���ڂ���f�[�^���Q�Ƃ��邽�߂̃e���r�ǖ��B
		///		�u�^��e���r�ǁv�͘^��\�t�g�ɓn�����߂̃e���r�ǖ��B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void tvStationComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			// �^��e���r�ǖ��e�L�X�g�{�b�N�X�ɑ��
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
		///		�I�����C���ԑg���̎擾�V�[�P���X(�񓯊�)
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/10/18 �V�K�쐬</history>
		//=========================================================================
		delegate void LetProgramInformation();
		private void DoQueryInfoSequence()
		{
            for (; ; )
			{
                if (mSequenceEndFlag.WaitOne( 0, false ))
                    break;

// <PENDING> 2009/11/24 ���z�f�X�N�g�b�v�\�t�g����E�B���h�E���������΍� ->
				if( !this.Visible )
					break;
// <PENDING> 2009/11/24 <-

				if( mZappingTid != mSelectedTid && mLinkOnline )
				{
					List<SyoboiCalender.SyoboiRecord>	recordList		= null; 
					ArrayList							tvStationList	= null;

                    lock (mStatus) { mStatus.busyQueryInfo = true; }

					//-----------------------------------
					// �I�����ꂽ�ԑg�̕����ǈꗗ���X�V
					//-----------------------------------
					try
					{
						string title = "";

						LetProgramInformation setWaitingMsg = delegate() { syoboiTitleTextBox.Text = "(�f�[�^�擾��)"; };
						this.Invoke(setWaitingMsg);

						// �ԑg�f�[�^���擾
						if (0 < mZappingTid)
						{
							mSelectedTid = mZappingTid;

							recordList		= mSyoboCal.DownloadOnAirList(mSelectedTid, out title);
							tvStationList	= mSyoboCal.ListupTvStation(recordList);
						}

						// �ԑg�f�[�^����ʂɃZ�b�g
						LetProgramInformation proc = delegate()
						{
							syoboiTvStationComboBox.Items.Clear();
							syoboiTvStationComboBox.Items.Add("(�w��Ȃ�)");
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

								// �b����\��
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
		///		�_�C�A���O����鎞�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/10/18 �V�K�쐬</history>
		//=========================================================================
		private void AnimeDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			mSequenceEndFlag.Set();
			mThreadQuerySequence = null;
		}

		//=========================================================================
		///	<summary>
		///		�_�C�A���O��\������ۂ̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/10/18 �V�K�쐬</history>
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
		///		�\���X�V����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/10/18 �V�K�쐬</history>
		//=========================================================================
		private void OnUpdateTimer(object sender, EventArgs args)
		{
			RefreshControl();
		}

		//=========================================================================
		///	<summary>
		///		�e���r�ǖ��ϊ��e�[�u����ʂ��J��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/03/28 �V�K�쐬</history>
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
		///		�R���g���[���̏�ԍX�V
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/05/03 �V�K�쐬</history>
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

