//=========================================================================
///	<summary>
///		mAgicAnime�g���C�A�C�R�� ���W���[��
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬</history>
/// <history>2010/05/31 Subversion�ŊǗ����邽�ߌÂ��R�����g�폜</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Runtime.InteropServices;
using System.Management;
using System.IO;
using magicAnime.Properties;
using Helpers;

namespace magicAnime.UserInterface
{
	//=========================================================================
	///	<summary>
	///		mAgicAnime�g���C�A�C�R�� �N���X
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	internal class TrayIcon
	{
		const	int		maxUnreadCount	= 10;	// �o���[���ɕ\������ő喢�ǐ�

		//-----------------
		// �����o�ϐ�
		//-----------------
		internal NotifyIcon				mNotifyIcon;
		internal ContextMenuStrip		mTrayMenu;
		
		internal ToolStripSeparator		mTrayMenuUnreadSeparetor;
		private ToolStripMenuItem		mTrayRefreshMenu;
		private ToolStripSeparator		mToolStripMenuItem;
		private ToolStripMenuItem		mTrayExitMenu;
		private ToolStripMenuItem		mOpenWindowToolMenu;
		private System.Windows.Forms.Timer	mRefreshTimer;

		//=========================================================================
		///	<summary>
		///		�g���C�A�C�R���̏�����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal void Initialize()
		{
			//---------------------------
			// �g���C�A�C�R���̍쐬
			//---------------------------
			mNotifyIcon					= new NotifyIcon();
			mNotifyIcon.Icon			= Resources.mAigcAnimeLogo;
			mNotifyIcon.Visible			= Settings.Default.inTaskTray;
			mNotifyIcon.DoubleClick		+= notifyIcon_DoubleClick;
			mNotifyIcon.BalloonTipIcon	= ToolTipIcon.Info;
			mNotifyIcon.BalloonTipTitle	= "mAgicAnime";
			mNotifyIcon.Text				= "mAgicAnime";
//			trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject( "notifyIcon.Icon" )));

			//---------------------------
			// �g���C���j���[�̍쐬
			//---------------------------
			mTrayMenu				= new ContextMenuStrip();
			mTrayMenu.Opening		+= new System.ComponentModel.CancelEventHandler( trayMenu_Opening );
			this.mTrayMenu.SuspendLayout();

			mTrayMenuUnreadSeparetor	= new ToolStripSeparator();
			mTrayRefreshMenu			= new ToolStripMenuItem();
			mToolStripMenuItem		= new ToolStripSeparator();
			mOpenWindowToolMenu		= new ToolStripMenuItem();
			mTrayExitMenu			= new ToolStripMenuItem();

			mNotifyIcon.ContextMenuStrip	= mTrayMenu;

			// 
			// trayMenu
			// 
			this.mTrayMenu.Items.AddRange(
				new ToolStripItem[]
				{
					this.mTrayMenuUnreadSeparetor	,
					this.mTrayRefreshMenu			,
					this.mToolStripMenuItem			,
					this.mOpenWindowToolMenu			,
					this.mTrayExitMenu
				});
			this.mTrayMenu.Name = "trayMenu";
			this.mTrayMenu.Size = new System.Drawing.Size( 168, 82 );
			// 
			// trayMenuUnreadSeparetor
			// 
			this.mTrayMenuUnreadSeparetor.Name = "trayMenuUnreadSeparetor";
			this.mTrayMenuUnreadSeparetor.Size = new System.Drawing.Size( 164, 6 );
			// 
			// trayRefreshMenu
			// 
			this.mTrayRefreshMenu.Name = "trayRefreshMenu";
			this.mTrayRefreshMenu.Size = new System.Drawing.Size( 167, 22 );
			this.mTrayRefreshMenu.Text = "�ŐV���ɍX�V(&R)";
			this.mTrayRefreshMenu.Click += new System.EventHandler( this.RefreshMenu_Clicked );
			this.mTrayRefreshMenu.DropDownOpening += new System.EventHandler( this.RefreshMenu_DropDownOpening );
			// 
			// �ToolStripMenuItem
			// 
			this.mToolStripMenuItem.Name = "�ToolStripMenuItem";
			this.mToolStripMenuItem.Size = new System.Drawing.Size( 164, 6 );
			// 
			// openWindowToolMenu
			// 
			this.mOpenWindowToolMenu.Name = "openWindowToolMenu";
			this.mOpenWindowToolMenu.Size = new System.Drawing.Size( 167, 22 );
			this.mOpenWindowToolMenu.Text = "�E�B���h�E���J��(&O)";
			this.mOpenWindowToolMenu.Click += new System.EventHandler( this.openWindowToolMenu_Clicked );
			// 
			// trayExitMenu
			// 
			this.mTrayExitMenu.Name = "trayExitMenu";
			this.mTrayExitMenu.Size = new System.Drawing.Size( 167, 22 );
			this.mTrayExitMenu.Text = "�I��(&X)";
			this.mTrayExitMenu.Click += new System.EventHandler( this.ExitMenu_Click );

			this.mTrayMenu.ResumeLayout();

			//---------------------------
			// ���̑��̏���������
			//---------------------------
			AnimeServer server = AnimeServer.GetInstance();

			server.mPopupSoonBallon += PopupSoonBalloon;

			//----------------
			// �X�V�^�C�}
			//----------------
			mRefreshTimer			= new System.Windows.Forms.Timer();
			mRefreshTimer.Interval	= 100;
			mRefreshTimer.Enabled	= true;
			mRefreshTimer.Tick += delegate (object sender, EventArgs args)
			{
				AnimeServer				animServer = AnimeServer.GetInstance();
				AnimeServer.MyStatus	stat	   = animServer.GetStatus();

				// �f�[�^�X�V���̓g���C�A�C�R����ύX
				mNotifyIcon.Icon	= stat.updateSequenceBusy
								? Resources.mAigcAnimeLogoBusy
								: Resources.mAigcAnimeLogo;
			};

			RefreshUnread();
		}

		//=========================================================================
		///	<summary>
		///		�g���C�A�C�R���̃N���[���A�b�v����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/10/01 �V�K�쐬</history>
		//=========================================================================
		internal void Cleanup()
		{
			AnimeServer server = AnimeServer.GetInstance();

			server.mPopupSoonBallon -= PopupSoonBalloon;
		}


		//=========================================================================
		///	<summary>
		///		�g���C���j���[���J���ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		void trayMenu_Opening( object sender, System.ComponentModel.CancelEventArgs e )
		{
//			e.Cancel = true;
//			OnUpdateDocument();
		}


		//=========================================================================
		///	<summary>
		///		�^�X�N�A�C�R�����_�u���N���b�N���ꂽ���A�E�B���h�E��\������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			// �^�X�N�g���C�A�C�R�����畜�A
			Program.ShowMainForm();
		}


		//=========================================================================
		///	<summary>
		///		[�ŐV���ɍX�V]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void RefreshMenu_Clicked(object sender, EventArgs e)
		{
			AnimeServer.GetInstance().BeginUpdate(0);
		}

		private void RefreshMenu_DropDownOpening(object sender, EventArgs e)
		{

		}

		//=========================================================================
		///	<summary>
		///		[�E�B���h�E���J��]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void openWindowToolMenu_Clicked( object sender, EventArgs e )
		{
			Program.ShowMainForm();
		}

		//=========================================================================
		///	<summary>
		///		[�I��]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void ExitMenu_Click(object sender, EventArgs e)
		{
			// �I�����Ă悢���A�e���ɖ₢���킹��
			if( Program.QueryClose() )
			{
				Application.Exit();
			}
		}

		//=========================================================================
		///	<summary>
		///		���ǃ��j���[���ڂ��N���b�N���ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void playUnreadMenu_DropDownItemClicked( object sender, EventArgs ar )
		{
			AnimeEpisode episode;

			try
			{
				ToolStripDropDownItem	dropItem = (ToolStripDropDownItem)sender;

				episode	= (AnimeEpisode)dropItem.Tag;

				Program.mMainForm.PlayMovie( episode.FilePath );

				episode.Unread = false;
			}
			catch (Exception ex)
			{
				MessageBox.Show(	null				,
									ex.Message			,
									"�G���["			,
									MessageBoxButtons.OK,
									MessageBoxIcon.Error);
			}
		}

		//=========================================================================
		///	<summary>
		///		���ǃ��X�g���j���[���X�V����
		///	</summary>
		/// <remarks>
		///		�f�[�^�ύX�C�x���g���g���K�Ƃ��Ď��s�����B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2008/05/03 ���\�b�h����ύX</history>
		//=========================================================================
		internal void RefreshUnread()
		{
			lock( this )
			{
				//----------------------------
				// ���j���[�̖��ǈꗗ���N���A
				//----------------------------

				int bottom = mTrayMenu.Items.IndexOf( mTrayMenuUnreadSeparetor );

				for ( int i = 0; i < bottom; ++i )
				{
					mTrayMenu.Items.RemoveAt( 0 );
				}

				if( !Settings.Default.disableUnread )
				{
					//--------------------------
					// ���ǈꗗ�����j���[�ɒǉ�
					//--------------------------
					uint count = 0;

					AnimeProgram.EnumRecordCallBack callBack
						= delegate(AnimeEpisode r, object param)
					{
						if (r.Unread && r.IsPlayable)
						{
							ToolStripItem item;

							if ( count < maxUnreadCount )	// �\�����𐧌�
							{
								try
								{
									item			= new ToolStripMenuItem( r.ToString() );
									item.Click		+= playUnreadMenu_DropDownItemClicked;
									item.Tag		= r;
									item.Visible	= true;
									mTrayMenu.Items.Insert( 0, item );

									count++;
								}
								catch(Exception ex)
								{
								}
							}
						}
					};

					AnimeServer.GetInstance().EnumAllEpisodes(callBack, null);	// ���ǂ�񋓂���

					if ( count == 0 )
					{
						//---------------------------------
						// �g���C���j���[��(���ǂȂ�)�ǉ�
						//---------------------------------
						ToolStripMenuItem item;
						item			= new ToolStripMenuItem( "(���ǂȂ�)" );
						item.Enabled	= false;
						mTrayMenu.Items.Insert( 0, item );
					}
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		���ǃo���[�����|�b�v�A�b�v����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2007/12/23 MainForm����ړ�</history>
		//=========================================================================
		internal void PopupUnreadBalloon()
		{
			List<AnimeEpisode> unreads = new List<AnimeEpisode>();

			// ���ǂ��
			unreads = AnimeServer.GetInstance().QueryEpisode(
						ep => ep.Unread && ep.IsPlayable );

			//------------------------------
			// �o���[���ɖ��Ǐ�Ԃ�\��
			//------------------------------
			{
				if( (0 < unreads.Count) && mNotifyIcon.Visible )
				{
				    string t = string.Format("���ǂ̃A�j���� {0:0}�{ ����܂��B\n", unreads.Count);

				    foreach ( AnimeEpisode r in unreads )
				    {
				        t += "\n" + r.ToString();
				    }
				    mNotifyIcon.BalloonTipText = t;
				    mNotifyIcon.ShowBalloonTip(3000);
				}
			}

		}

		//=========================================================================
		///	<summary>
		///		�u�܂��Ȃ��������ԁv�o���[�����|�b�v�A�b�v
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/10/01 �V�K�쐬</history>
		//=========================================================================
		internal void PopupSoonBalloon(
			AnimeEpisode	episode )	// [i] �G�s�\�[�h
		{
			if( mNotifyIcon.Visible)
			{
				string  t;

				if( !episode.HasPlan )
					return;

				DateTimeHelper	dateTime = new DateTimeHelper(
					episode.StartDateTime				,
					Settings.Default.hoursPerDay -24	);

				t = "�܂��Ȃ��������Ԃł�\n\n"
					+ dateTime.ToShortTimeString()
					+ "���� "
					+ episode.ToString();
				
			    mNotifyIcon.BalloonTipText = t;
			    mNotifyIcon.ShowBalloonTip(3000);
			}

		}

	}
}

