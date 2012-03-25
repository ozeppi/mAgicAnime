//=========================================================================
///	<summary>
///		mAgicAnime���C����� ���W���[��
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬 Dr.Kurusugawa</history>
/// <history>2010/02/20 �Â��R�����g���폜</history>
/// <history>2010/05/01 Subversion�ŊǗ����邽�ߕs�v�ȃR�����g�폜</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Diagnostics;
using magicAnime.Properties;
using Helpers;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		mAgicAnime���C����ʃN���X
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	/// <history>2008/05/02 �����o�ϐ�����[�Ɉړ�</history>
	//=========================================================================
	partial class MainForm : Form
	{
		//--------------------------------
		// �O���b�h�Z���̒x���`��
		//--------------------------------
#if _LAZYDRAW
		private struct CellRefresh
		{
			public int		Row;
			public int		Col;
//			public Size	cellSize;
			public DataGridViewElementStates State;
		};
#endif

#if _LAZYDRAW
		private List<CellRefresh>	cellRefreshRequest;
		private Graphics			cellRefreshGraphics;
#endif
		private Image				mCellBuffer;
		private Graphics			mCellGraphics;

		//----------------------
		// �A�C�R�����\�[�X
		//----------------------

		internal struct ViewIcons
		{
			internal void LoadIcons()
			{
				this.videoIcon			= Resources.VideoFile;
				this.completeIcon		= Resources.Encoded;
				this.errorIcon			= Resources.Error;
				this.questionIcon		= Resources.Question;
				this.schedIcon			= Resources.Reserved;
				this.warnIcon			= Resources.Warning;
				this.doubleBookingIcon		= Resources.DoubleBooking;
				this.doubleBookingSchedIcon = Resources.DoubleBookingReserved;
			}
			public Icon	videoIcon			;
			public Icon	completeIcon		;
			public Icon	errorIcon			;
			public Icon	questionIcon		;
			public Icon	schedIcon			;
			public Icon	warnIcon			;
			public Icon	doubleBookingIcon		;
			public Icon	doubleBookingSchedIcon	;
		};
		ViewIcons			mViewIcons;

		//----------------------------------
		// ���[�U�[�C���^�[�t�F�[�X�����o
		//----------------------------------
//		private Timer		drawTimer;
		private Timer		mRefreshTimer;					// �X�V�p�^�C�}
		private Font		mBigFont;						// �G�s�\�[�h�ԍ��t�H���g
		
		private bool		mExitFlag		= false;		//

		private bool		mDraggingRow	= false;		// �s�h���b�O���t���O
		private object		mDraggingItem;					// �h���b�O���̃A�C�e��

		private int			mAspect			= 10;			// 96dpi�ɑ΂���A�X�y�N�g�̔�*10

		public enum ViewMode
		{
			SeriesMode			,	// �b�����[�h
			DayCalenderMode		,	// �����[�h
			WeekCalenderMode	,	// �T���[�h
		}

		private ViewMode	mViewMode;						// �f�[�^�O���b�h�̕\�����[�h


		//-----------------
		// ���̑�
		//-----------------
		private List<ToolStripItem>	mQueuingMenuItems;		// �G���R�[�h�҂����j���[���ڃ��X�g
		private List<ToolStripItem>	mEncodingMenuItems;		// �G���R�[�h�����j���[���ڃ��X�g
		
		//=========================================================================
		///	<summary>
		///		�R���X�g���N�^
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public MainForm()
		{
			InitializeComponent();

			mViewMode = ViewMode.SeriesMode;

			mQueuingMenuItems	= new List<ToolStripItem>();
			mEncodingMenuItems	= new List<ToolStripItem>();
		}

		//=========================================================================
		///	<summary>
		///		[�I�v�V����]���j���[���N���b�N���ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void OptionMenu_Click(object sender, EventArgs e)
		{
			OptionDialog dlg = new OptionDialog();
			
			dlg.ShowDialog();

			RefreshContent();

			// �I�v�V�����ݒ�𔽉f
			Program.OptionChanged(null,null);
		}


		//=========================================================================
		///	<summary>
		///		�t�H�[�������[�h���ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void MainForm_Load(object sender, EventArgs e)
		{

			mViewIcons.LoadIcons();

			// �f�[�^�\���X�V�^�C�}
			mRefreshTimer = new Timer();
			mRefreshTimer.Tick += OnRefreshTimer;
			mRefreshTimer.Interval = 100;
			mRefreshTimer.Start();

			//-----------------------
			// �O���b�h�̏�����
			//-----------------------
			mCellBuffer			= new Bitmap(300,300);
			mCellGraphics		= Graphics.FromImage( mCellBuffer );
#if _LAZYDRAW
			cellRefreshRequest	= new List<CellRefresh>();

			// �O���b�h�x���`��^�C�}����
			drawTimer			= new Timer();
			drawTimer.Tick		+= OnLazyDrawCell;
			drawTimer.Interval	= 300;
			drawTimer.Start();
#endif

			//--------------------------
			// ���̑��̏�����
			//--------------------------
			mViewMode = (ViewMode)Settings.Default.viewMode;
			thumbnailModeButton.Checked = Settings.Default.thumbnailMode;

// <ADD> 2010/04/17 �f�o�b�O�I�v�V���� ->
#if DEBUG
			debugMenu.Visible = true;
#else
			debugMenu.Visible = false;
#endif
// <ADD> 2010/04/17 <-

			RefreshContent();

			//--------------------------------
			// �I�v�V�����ݒ�ɉ����čŏ���
			//--------------------------------
			if ( Settings.Default.minimizeAtStartup	&&
				!Settings.Default.inTaskTray		)
			{
				this.WindowState = FormWindowState.Minimized;			// �ŏ��������܂܋N��
			}
		
				
			mBigFont = new Font("Arial Black", 14, FontStyle.Regular);
			
			//---------------------------------
			// ��ʂ�DPI����A�X�y�N�g����v�Z
			//---------------------------------

			KernelAPI.GDI.TEXTMETRIC tm = new KernelAPI.GDI.TEXTMETRIC();

			if ( KernelAPI.GDI.GetTextMetrics( KernelAPI.GDI.GetDC( 0 ), ref tm ) != 0 )
			{
				mAspect = (int)( tm.tmDigitizedAspectX * 10 / 96 );
				mAspect = mAspect * (int)tm.tmAveCharWidth / 8; // �V�X�e���t�H���g�T�C�Y�̈Ⴂ���␳
			}

			//--------------------------------------
			// �A�X�y�N�g�䗦���O���b�h�Z�����ɓK�p
			//--------------------------------------

			foreach ( DataGridViewColumn col in dataGrid.Columns )
			{
				col.Width = col.Width * mAspect / 10;
			}
			dataGrid.ColumnHeadersHeight = dataGrid.ColumnHeadersHeight * mAspect / 10;

			RefreshControl(); // <ADD> 2010/01/29
		}
		

		//=========================================================================
		///	<summary>
		///		�t�H�[�����\�����ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void MainForm_Shown(object sender, EventArgs e)
		{
			//--------------------------------
			// �E�B���h�E�ʒu�ƃT�C�Y�𕜌�
			//--------------------------------
			if ( !Settings.Default.maximizeWindow )
			{
				this.WindowState = FormWindowState.Normal;
				this.Bounds = Settings.Default.rectWindow;
			} else
			{
				this.WindowState = FormWindowState.Maximized;
			}

			try
			{
				viewSplitContainer.SplitterDistance = viewSplitContainer.Height
													- Settings.Default.logPaneSize;
			}
			catch(Exception ex)
			{
			}

		}

		//=========================================================================
		///	<summary>
		///		�h�L�������g�\���̍X�V���s��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2008/05/02 ���\�b�h���ύX(OnUpdate->RefreshContent)</history>
		//=========================================================================
		internal void RefreshContent()
		{
			AnimeServer		server		= AnimeServer.GetInstance();
			int				rowIndex;

//			dataGrid.Rows.Clear();

// <PENDING> 2010/02/20 ->
			lock (server)
// <PENDING> 2010/02/20 <-
			{
				int	i, addCols;
				int	cw;

				if( thumbnailModeButton.Checked )
				{
					cw = Settings.Default.thumbnailWidth;		// �T���l�C�����[�h�̃Z����
				}
				else
				{
					cw = 48;									// ��T���l�C�����[�h�̃Z����
				}

				//----------------------------
				// ��̊g�k���s��
				//----------------------------

				addCols = 0;

				switch (mViewMode)
				{
					case ViewMode.SeriesMode:
						//----------------------------
						// �b���\�����[�h
						//----------------------------

						foreach( AnimeProgram prog in server.Animes )
						{
							addCols = System.Math.Max( addCols, prog.StoryCount );
						}

						addCols = Math.Min(addCols, 500);	// �O���b�h�̉����ő�l���I�[�o�[���Ȃ�����
						dataGrid.ColumnCount = ColumnStoryCount.Index + addCols + 1;

						for (i = 0; i < addCols; ++i)
						{
							DataGridViewColumn c = dataGrid.Columns[i + ColumnStoryCount.Index + 1];
							c.Width			= cw * mAspect / 10;
							c.HeaderText	= string.Format("{0:0}�b", i + 1);
						}
						break;
					case ViewMode.DayCalenderMode:
						//----------------------------
						// �����ƃJ�����_�[���[�h
						//----------------------------

						addCols = Settings.Default.dayPast + Settings.Default.dayFuture + 1;
						dataGrid.ColumnCount = ColumnStoryCount.Index + addCols + 1;

						for (i = 0; i < addCols; ++i)
						{
							DataGridViewColumn	c = dataGrid.Columns[i + ColumnStoryCount.Index + 1];
							int					d = i - Settings.Default.dayPast;

							c.Width = cw * mAspect / 10;
							if ( d == -1 )
								c.HeaderText = "���";
							else if ( d == 0 )
								c.HeaderText = "����";
							else if ( d == +1 )
								c.HeaderText = "����";
							else
								c.HeaderText = string.Format( "{0:0}��", DateTime.Now.AddDays( d ).Day );
						}
						break;
					case ViewMode.WeekCalenderMode:
						//----------------------------
						// �T���ƃJ�����_�[���[�h
						//----------------------------

						addCols = Settings.Default.weekPast + Settings.Default.weekFuture + 1;
						dataGrid.ColumnCount = ColumnStoryCount.Index + addCols + 1;

						for (i = 0; i < addCols; ++i)
						{
							DataGridViewColumn	c = dataGrid.Columns[i + ColumnStoryCount.Index + 1];
							int					w = i - Settings.Default.weekPast;

							c.Width = cw * mAspect / 10;
							if (w == -1)
								c.HeaderText = "��T";
							else if (w == 0)
								c.HeaderText = "���T";
							else if ( w == +1 )
								c.HeaderText = "���T";
							else
							{
								DateTime aDay		= DateTime.Now.AddDays( w * 7 );
								DateTime firstDay	= aDay.AddDays( -aDay.Day + 1 );				// ���̏��߂̓�
								DateTime sunDay		= firstDay.AddDays( -(int)firstDay.DayOfWeek );	// ���̏��߂̓��̒��O�̓��j��

								c.HeaderText = string.Format(
									"{0:0}��{1:0}"					,
									firstDay.Month					,
									( aDay - sunDay ).Days / 7 + 1	);
							}
						}
						break;
				}

				//----------------------------
				// �b�̃Z���̓\�[�g�֎~
				//----------------------------
				foreach ( DataGridViewColumn col in dataGrid.Columns )
				{
					col.SortMode = DataGridViewColumnSortMode.NotSortable;
				}


				//----------------------------
				// �s�̊g�k���s��
				//----------------------------

				dataGrid.RowCount = server.Animes.Count;

				//----------------------------
				// �e�s�̓��e���X�V
				//----------------------------
				foreach (AnimeProgram prog in server.Animes)
				{
					rowIndex			= server.Animes.IndexOf( prog );
					DataGridViewRow row	= dataGrid.Rows[rowIndex];

					// �Z����Tag���N���A
					foreach( DataGridViewCell cell in row.Cells )
					{
						cell.Tag = null;
					}

					// �Z����AnimeProgram�I�u�W�F�N�g��Ή��t����
					row.Tag = prog;

					// �Z����AnimeEpisode�I�u�W�F�N�g��Ή��t����
					foreach (AnimeEpisode episode in prog.Episodes)
					{
						int col;
						TimeSpan s;
						int hourOffset = Properties.Settings.Default.hoursPerDay - 24;

						switch (mViewMode)
						{
							case ViewMode.SeriesMode:
								//----------------------------
								// �A�b�\�����[�h
								//----------------------------
								col = prog.Episodes.IndexOf(episode) + ColumnStoryCount.Index + 1;
								break;

							case ViewMode.DayCalenderMode:
								//----------------------------
								// �J�����_�[�����ƃ��[�h
								//----------------------------
								if( episode.HasPlan )
								{
									int		diffDays = 0;

									diffDays = DateTimeHelper.DiffDays(
										episode.StartDateTime	,
										DateTime.Now			,
										hourOffset				);

									col = ColumnStoryCount.Index + 1
										+ diffDays
										+ Settings.Default.dayPast;
								}
								else
								{
									col = -1;
								}
								break;
							case ViewMode.WeekCalenderMode:
								//----------------------------
								// �J�����_�[�T���ƃ��[�h
								//----------------------------
								DateTime monday;
								monday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);

								if( episode.HasPlan )
								{
									int		diffWeeks = 0;

									diffWeeks = DateTimeHelper.DiffWeeks(
										episode.StartDateTime	,
										DateTime.Now			,
										hourOffset				);

									col = ColumnStoryCount.Index + 1
										+ diffWeeks
										+ Settings.Default.weekPast;
								}
								else
								{
									col = -1;
								}
								break;
							default:
								col = -1;
								break;
						}

						if (ColumnStoryCount.Index < col && col < row.Cells.Count)
						{
							row.Cells[col].Tag = episode;	// �Z���ƃ��R�[�h��Ή��t����
						}
					}

					SetGridRowData(row, prog);
				}
			}

			UpdateStatusBar();

			//----------------------------
			// ���ǃ{�^�����j���[���ڍX�V
			//----------------------------

			uint count = 0;

			AnimeProgram.EnumRecordCallBack callBack
				= delegate(AnimeEpisode r, object param)
			{
				if (r.Unread && r.IsPlayable)
				{
					ToolStripItem item;

					try
					{
						//--------------------------------
						// �񋓂��ꂽ���ڂ����j���[�ɒǉ�
						//--------------------------------
						if (playUnreadButton.DropDownItems.Count < 30)
						{
							item = playUnreadButton.DropDownItems.Add(r.ToString());
							item.Tag = r;
						}
						count++;
					}
					catch(Exception ex)
					{
					}
				}

			};

			playUnreadButton.DropDownItems.Clear();
			server.EnumAllEpisodes(callBack, null); // ���ǂ�񋓂���
			playUnreadButton.Enabled =	(0 < count)
									&&	!Settings.Default.disableUnread;

			Program.mTrayIcon.RefreshUnread();
		}

		//=========================================================================
		///	<summary>
		///		�h�L�������g�\���̍X�V���s��(�f�[�^�ύX���̂�)
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/11/22 �V�K�쐬</history>
		//=========================================================================
		internal void RefreshIfModified()
		{
			AnimeServer server = AnimeServer.GetInstance();
			if (server.Dirty)
			{
				server.CheckDoubleBooking();	// <PENDING> 2009/11/22
				RefreshContent();
				RefreshSelectedEpisodeInfo();

				server.Dirty = false;
			}
		}

		//=========================================================================
		///	<summary>
		///		�X�e�[�^�X�o�[�̓��e���X�V
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		void UpdateStatusBar()
		{
			AnimeServer server = AnimeServer.GetInstance();

			//------------------------------
			// �^��t�H���_�̏���\��
			//------------------------------
			try
			{
				if ( Directory.Exists( Settings.Default.captureFolder ) )
				{
					string drive;

					drive = Settings.Default.captureFolder.Substring( 0, 2 );

					DriveInfo drvInfo = new DriveInfo( drive );

					recordDriveFreeSpaceLabel.Text =
						"�^��� " +
						Convert.ToString( (float)( drvInfo.TotalFreeSpace / 1024 / 1024 / 100 ) / 10 ) + "/"		+
						Convert.ToString( (float)( drvInfo.TotalSize	   / 1024 / 1024 / 100 ) / 10 ) + "GB ��"	;
				} else
				{
					recordDriveFreeSpaceLabel.Text = "�^��t�H���_: ������܂���";
				}
			}
			catch(Exception ex)
			{
				recordDriveFreeSpaceLabel.Text = "�^���: �󂫗e�ʕs��";
			}

			//------------------------------
			// ���̕����̏���\��
			//------------------------------
			AnimeEpisode earliestEpis = server.QueryEarliestEpisode();

			if (earliestEpis != null)
			{
				if( earliestEpis.HasPlan )
				{
					DateTimeHelper dateTime = new DateTimeHelper(
						earliestEpis.StartDateTime			,
						Settings.Default.hoursPerDay - 24	);

					try
					{
						todayOnAirLabel.Text =	dateTime.ToShortDateString() + " " +
												dateTime.ToShortTimeString() + " " +
												earliestEpis.ToString();
					}
					catch(UpdatingException ex)
					{
						todayOnAirLabel.Text = ex.Message;
					}
					catch(Exception ex)
					{
					}
				}
			}
			else
			{
				todayOnAirLabel.Text = "";
			}

			//----------------------------------
			// �f�[�^�X�V�X�e�[�^�X�\��
			//----------------------------------
			AnimeServer.MyStatus	status = server.GetStatus();

			if( status.updateSequenceBusy )
			{
				logStatusLabel.Text		= "�f�[�^�X�V���ł�(" + status.updateDetail + ")";
				logStatusLabel.IsLink	= false;
			}
			else if( !string.IsNullOrEmpty(status.resultLastUpdate) )
			{
				logStatusLabel.Text		= "�f�[�^�X�V�̌��ʁA�G���[������܂���";
				logStatusLabel.IsLink	= true;
			}
			else
			{
				logStatusLabel.Text		= "";
				logStatusLabel.IsLink	= false;
			}

		}

		//=========================================================================
		///	<summary>
		///		�w�肳�ꂽ�O���b�h�s�̓��e���X�V
		///	</summary>
		/// <remarks>
		///		�w�肳�ꂽ�O���b�h�srow�ɔԑgprog�̓��e���o�͂���
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2008/05/02 �X�R�[�v�ƃ��\�b�h���ύX</history>
		//=========================================================================
		private void SetGridRowData(
			DataGridViewRow	row		,
			AnimeProgram	prog	)
		{
			AnimeEpisode				episode;
			string						comingOnAir = null;
			AnimeProgram.NextEpisode	nextState;

			//------------------------------
			// ����������̏���\��
			//------------------------------

			//AnimeServer.GetInstance().GetUpdateDateTime()
			nextState = prog.GetNextEpisode( DateTime.Now, out episode );

			if( nextState == AnimeProgram.NextEpisode.NextDecided )
			{
				int month, day, hour, dayOfWeek;

				hour	= episode.StartDateTime.Hour;
				
				if(hour < (Settings.Default.hoursPerDay-24))
				{
					hour		+= 24;
					month		= episode.StartDateTime.AddDays(-1.0).Month;
					day			= episode.StartDateTime.AddDays(-1.0).Day;
					dayOfWeek	= (int)episode.StartDateTime.AddDays(-1.0).DayOfWeek;
				}
				else
				{
					month		= episode.StartDateTime.Month;
					day			= episode.StartDateTime.Day;
					dayOfWeek	= (int)episode.StartDateTime.DayOfWeek;
				}
				
				char []DayOfWeekList = {'��','��','��','��','��','��','�y'};
				
				comingOnAir = string.Format(
								"{0:D1}/{1:D2}({2:S}) {3:D2}:{4:D2}",
								month,day							,
								DayOfWeekList[dayOfWeek]			,
								hour,episode.StartDateTime.Minute	);
			}
			else if( nextState == AnimeProgram.NextEpisode.NextUnknown )
			{
				comingOnAir = "�Ȃ�";
			}
			else if( nextState == AnimeProgram.NextEpisode.EndProgram )
			{
				comingOnAir = "�����I��";
			}
			
			//---------------------------------
			// �Z���ɕ\������f�[�^���܂Ƃ߂�
			//---------------------------------
			string[] newRowData =
			{
				prog.title		,											// �ԑg��
				comingOnAir		,											// �������
				prog.tvStation	,											// �e���r��
				
				(prog.EncoderType != null)?
					prog.EncoderProfile.ToString() + "(" + prog.EncoderType.Name + ")"
					: "(�Ȃ�)",												// �G���R�[�h���
				string.Format("{0:D2}",prog.StoryCount),					// �S�b��
			};

			for( int i = 0 ;i < newRowData.Length ;++i )
			{
				row.Cells[i].Value = newRowData[i];							// ��L�̃f�[�^���Z���ɑ��
			}

			//---------------------------------
			// �Z��(�s)�̍������Čv�Z
			//---------------------------------
			if( thumbnailModeButton.Checked )
			{
				row.Height = Settings.Default.thumbnailHeight * mAspect / 10;
			}
			else
			{
				row.Height = 32 * mAspect / 10;
			}

			dataGrid.Invalidate();
		}


#if _LAZYDRAW
		//=========================================================================
		///	<summary>
		///		�Z���̒x���`��v������������
		///	</summary>
		/// <remarks>
		///		�񓯊��ɒx���`�悷�邱�Ƃ�UI�̂Ђ��������h�~����B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void OnLazyDrawCell(object o,EventArgs ea)
		{
			if ( dataGrid != null && !dataGrid.IsDisposed)
			{
				Graphics g = dataGrid.CreateGraphics();

				foreach ( CellRefresh cr in cellRefreshRequest )
				{
					if ( cr.Row >= 0 && cr.Col > ColumnStoryCount.Index )		// �b���Ƃ̗�
					{
						DrawGridCell( g, cr.Row, cr.Col, cr.State );
					}
				}
				cellRefreshRequest.Clear();
			}
		}
#endif

		//=========================================================================
		///	<summary>
		///		�Z���̓��e��`�悷��
		///	</summary>
		/// <remarks>
		///		�Z���T�C�Y�̃o�b�t�@�ɕ`�悵����ɕ`���֓]������B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		void DrawGridCell(
			Graphics					dest,	// �`���
			int							row,	// �s
			int							col,	// ��
			DataGridViewElementStates	state)	// 
		{
			AnimeProgram	prog;
			AnimeEpisode	episode		= null;
			int				storyNumber	= 0;
			Icon			icon		= null;
			string			text;
			int				cellX,	cellY;
			bool			border		= false;

			Graphics g = mCellGraphics;

			if ( dest == null ) return;

			if( row < dataGrid.Rows.Count )
			{
				Color	backColor;					// �Z���̔w�i�F
				int		recordMonth, recordDay;		// 
				int		todayMonth, todayDay;		// 
				bool	toDay;						// 

				//-------------------
				// �O����
				//-------------------

				prog = (AnimeProgram)dataGrid.Rows[ row ].Tag;							// �s�ɑΉ�����ԑg

				if ( prog != null )
					episode = (AnimeEpisode)dataGrid.Rows[ row ].Cells[ col ].Tag;		// �Z���ɑΉ�����b

				if ( episode != null )
					storyNumber = episode.StoryNumber;

				// �Z�����������B��Ă���ꍇ�̑΍�
				int			cellHeight	= dataGrid.Rows[ row ].Height;
				int			cellWidth	= dataGrid.Columns[ col ].Width;

				Rectangle	newRect		= dataGrid.GetCellDisplayRectangle( col, row, false );

				if( newRect.Width < cellWidth )
				{
					newRect.X = newRect.Right - cellWidth;
					newRect.Width	= cellWidth;
				}

				cellX = newRect.Left;
				cellY = newRect.Top;

				newRect.Location = new Point(0,0);

				if ( newRect.Width == 0 || newRect.Height == 0 )						// �Z�����\���̈�����H
					return;

				//-------------------
				// ���œh��Ԃ�
				//-------------------
				g.FillRectangle( new SolidBrush( Color.White ), newRect );

				if ( episode != null && episode.HasPlan )
				{
					//---------------------------
					// ���������{�����ǂ�������
					//---------------------------
					if ( episode.StartDateTime.Hour <= 3 )
					{
						recordMonth = episode.StartDateTime.AddDays( -1.0 ).Month;
						recordDay = episode.StartDateTime.AddDays( -1.0 ).Day;
					} else
					{
						recordMonth = episode.StartDateTime.Month;
						recordDay = episode.StartDateTime.Day;
					}

					if ( DateTime.Now.Hour <= 3 )
					{
						todayMonth = DateTime.Now.AddDays( -1.0 ).Month;
						todayDay = DateTime.Now.AddDays( -1.0 ).Day;
					} else
					{
						todayMonth = DateTime.Now.Month;
						todayDay = DateTime.Now.Day;
					}

					toDay	=	episode.HasPlan
							&&	( todayMonth	== recordMonth )
							&&	( todayDay		== recordDay );
				}
				else
				{
					toDay = false;
				}

				//-------------------------
				// �Z���F�����肷��
				//-------------------------
				if ( storyNumber >= 1 )
				{
					if ( ( state & DataGridViewElementStates.Selected ) != 0 )
					{
						backColor = Color.FromKnownColor( KnownColor.ActiveCaption );	// �I�����ꂽ�Z��
					}
					else if( episode.IsBusy )
					{
						backColor = Color.FromArgb( 0xff, 0xc8, 0xc8 );
					}
					else if(	episode.HasFile
							&&	episode.Unread
							&&	!Settings.Default.disableUnread )
					{
						backColor = Color.FromArgb( 0xff, 0xff, 0xc8 );
					}
					else
					{

						if( toDay )	// �{������
						{
							backColor = Color.FromArgb( 0xe0, 0xff, 0xe0 );
						}
						else		// ���̑�
						{
							//	backColor = dataGrid.BackgroundColor;
							backColor = Color.FromKnownColor( KnownColor.Window );
						}
					}

					g.FillRectangle( new SolidBrush( backColor ), newRect );
					border = true;

					//--------------------------------
					// �T���l�C���\�����[�h
					//--------------------------------
					if ( thumbnailModeButton.Checked )
					{

						if ( episode.Parent.ThambnailImage != null )
						{
							//--------------------------------
							// �T���l�C���C���[�W�`��
							//--------------------------------

							int sw = Settings.Default.thumbnailWidth;
							int sh = Settings.Default.thumbnailHeight;

							g.DrawImage(
								episode.Parent.ThambnailImage,
								newRect,
								new Rectangle(
										episode.Parent.ThambnailSize.Width * ( episode.StoryNumber - 1 ),
										0,
										episode.Parent.ThambnailSize.Width,
										episode.Parent.ThambnailSize.Height ),
								GraphicsUnit.Pixel );

							//-----------------------------------
							// �T�u�^�C�g�������������𔒂�����
							//-----------------------------------
							if ( Settings.Default.thumbnailSubtitle )
							{
								Rectangle rct = newRect;

								rct.Y = rct.Y + rct.Height - Settings.Default.thumbnailWhitebar;
								rct.Height = Settings.Default.thumbnailWhitebar;


								// if (!backColor.Equals(Color.FromKnownColor(KnownColor.Window)))	
								{
									g.FillRectangle( new SolidBrush( Color.FromArgb( 255, backColor ) ), rct );
//									g.FillRectangle(new SolidBrush(Color.FromArgb(220, backColor)), rct);	// �x��?
								}

							}
						} else
						{
							//------------------------------------------
							// �T���l�C���C���[�W���Ȃ��ꍇ�͓h��Ԃ�
							//------------------------------------------
							g.FillRectangle( new SolidBrush( backColor ), newRect );
						}
						// �Z���̋��E��
//							g.DrawRectangle(new Pen(Color.FromKnownColor(KnownColor.InactiveBorder)), newRect);
					}

				} else
				{
					//-------------
					// ��̃Z��
					//-------------
					backColor = dataGrid.BackgroundColor;
//					backColor = Color.FromKnownColor(KnownColor.Control);
					g.FillRectangle( new SolidBrush( backColor ), newRect );
//					g.DrawRectangle( new Pen( backColor ), newRect );
				}


				if ( storyNumber >= 1 )
				{
					AnimeEpisode.State episodeState = episode.CurrentState;

					//-----------------------
					// �A�C�R���`��
					//-----------------------

					if ( storyNumber >= 1 )
					{
						//--------------------------------
						// �T���l�C��������b�͕\�����Ȃ�
						//--------------------------------

						if ( episodeState == AnimeEpisode.State.Notfound )
							icon = mViewIcons.errorIcon;
						else if ( episodeState == AnimeEpisode.State.Recorded )
							icon = mViewIcons.videoIcon;
						else if ( episodeState == AnimeEpisode.State.Encoded )
							icon = mViewIcons.completeIcon;
						else if ( episodeState == AnimeEpisode.State.Undecided )
							icon = mViewIcons.questionIcon;
						else if ( episodeState == AnimeEpisode.State.Scheduling )
							icon = mViewIcons.schedIcon;
						else if ( episodeState == AnimeEpisode.State.Changed )
							icon = mViewIcons.warnIcon;
						else if ( episodeState == AnimeEpisode.State.LostSchedule )
							icon = mViewIcons.warnIcon;

						// �d�����Ă���Episode�̕\��
						if ( episode.IsDoubleBooking() )
						{
							// �d���A�C�R���\��
							if (episodeState == AnimeEpisode.State.Planned	||
								episodeState == AnimeEpisode.State.Changed  ||
								episodeState == AnimeEpisode.State.Scheduling)
							{
									icon = (episodeState == AnimeEpisode.State.Scheduling) ?
										mViewIcons.doubleBookingSchedIcon			:
										mViewIcons.doubleBookingIcon					;
							}
						}

						if ( icon != null )
						{
							if ( thumbnailModeButton.Checked )
								g.DrawIcon( icon, new Rectangle( newRect.X + 4, newRect.Y + 4, 12, 12 ) );
							else
								g.DrawIcon( icon, new Rectangle( newRect.X + 24, newRect.Y + 12, 16, 16 ) );
						}

					}

					//----------------------
					// �b����`��
					//----------------------
					int		tx, ty;		// ������`�悷����W
					Color	tc;			// �����̃J���[

					tx = newRect.X;
					if ( thumbnailModeButton.Checked )
					{
						// �T���l�C�����[�h
						ty = newRect.Bottom - mBigFont.Height;
					} else
					{
						// ��T���l�C�����[�h
						ty = newRect.Y;
					}

					text = string.Format( "{0:0}", episode.StoryNumber );

					g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;	// �A���`�G�C���A�X�L��

					if ( storyNumber <= prog.Episodes.Count
						&& episodeState != AnimeEpisode.State.Planned		// <MOD> 2009/12/28
						&& episodeState != AnimeEpisode.State.Scheduling
						&& episodeState != AnimeEpisode.State.Changed
						&& episodeState != AnimeEpisode.State.Notfound
						&& episodeState != AnimeEpisode.State.Undecided
						&& episodeState != AnimeEpisode.State.LostSchedule )
					{

						//
						// �����̖{��
						//
						if ( episodeState == AnimeEpisode.State.Stored )
							tc = Color.Green;	// �ۑ��ς݂̐F
						else
							tc = Color.Blue;	// �ʏ�̐F

//							g.DrawString(text, bigFont, new SolidBrush(tc), new PointF(tx, ty)); // ������`��

					} else
					{
						tc = Color.Gray;
					}

					g.DrawString( text, mBigFont, new SolidBrush( tc ), new PointF( tx, ty ) ); // ������`��

					//--------------------------
					// �T�u�^�C�g���̕`��
					//--------------------------
					if ( thumbnailModeButton.Checked && Settings.Default.thumbnailSubtitle )		// �T���l�C���\�����[�h�̂�
					{

						bool cs = ( state & DataGridViewElementStates.Selected ) > 0;			// �Z���I���̗L���ɂ���ĐF��ς���

						g.DrawString(
							episode.mSubTitle,
							new Font( "MS UI Gothic", 9 ),
							new SolidBrush( cs ? Color.White : Color.Black ),
							new PointF( newRect.X + 32, newRect.Bottom - 9 - 5 ) );
					}
				}

				//----------------------------
				// �o�b�t�@�����ʂɓ]��
				//----------------------------
				dest.DrawImage( mCellBuffer, cellX, cellY,newRect,GraphicsUnit.Pixel );

				//------------------
				// ���E����`��
				//------------------
				if( border )
				{
					Rectangle	rectBorder	= newRect;
					Pen			penBorder	= new Pen( Color.FromKnownColor( KnownColor.LightGray ) );
					rectBorder.Location = new Point( cellX, cellY );
					rectBorder.Width--;
					rectBorder.Height--;
					dest.DrawRectangle( penBorder, rectBorder );
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		�Z���̃I�[�i�[�`��
		///	</summary>
		/// <remarks>
		///		���ڕ`�悹���A�x���`��L���[�ɃL���[�C���O���Č�ŕ`��
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void dataGrid_CellPainting(
			object sender,
			DataGridViewCellPaintingEventArgs e)
		{
			Graphics g = e.Graphics;
#if !_LAZYDRAW
			if( 0 <= e.RowIndex && ColumnStoryCount.Index < e.ColumnIndex )
			{
				try
				{
					DrawGridCell( g, e.RowIndex, e.ColumnIndex, e.State );
				}
				catch(Exception ex)
				{
				}
				e.Handled = true;
			}
#else
			//-------------------------------
			// �Z���̒x���`��v�����܂Ƃ߂�
			//-------------------------------
			cellRefreshGraphics = g;

			CellRefresh cr = new CellRefresh();

			cr.Row		= e.RowIndex;
			cr.Col		= e.ColumnIndex;
			cr.State	= e.State;
//			cr.cellSize	= e.CellBounds.Size;

			//----------------------------------------------
			// �����Z���̕`��v��������ꍇ�͍폜���Ă���
			//----------------------------------------------
			bool reloop;
			do{
				reloop = false;
				foreach ( CellRefresh it in cellRefreshRequest )
				{
					if ( it.Row == cr.Row && it.Col == cr.Col )
					{
						cellRefreshRequest.Remove( it );
						reloop = true;
						break;
					}
				}
			} while ( reloop ) ;

			//---------------------------
			// �`��v���L���[�ɒǉ�
			//---------------------------
			cellRefreshRequest.Add( cr );



			if ( (cr.Row >= 0) && (cr.Col > ColumnStoryCount.Index) )		// �b���Ƃ̗�
			{
				e.Handled = true;
			} else
			{
				e.Handled = false;
			}
#endif
			
			return;
		}

		
		//=========================================================================
		///	<summary>
		///		[�V�����ԑg]���j���[���N���b�N���ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void NewAnimeMenu_Click(object sender, EventArgs e)
		{
			AnimeProgram	newAnime = new AnimeProgram( AnimeServer.GetInstance() );

			newAnime.StoryCount	= 0;

			//-------------------------
			// �ԑg�_�C�A���O���J��
			//-------------------------

			AnimeDialog dlg = new AnimeDialog();

			if (dlg.ShowDialog(ref newAnime ) == DialogResult.OK)
			{
				//------------------------------
				// �V�����ԑg�����X�g�ɒǉ�
				//------------------------------
				AnimeServer doc = AnimeServer.GetInstance();
				
				doc.AddAnime(newAnime);
				doc.UpdateState(	new List<AnimeProgram>(){ newAnime },
									AnimeServer.GetInstance().UpdateDateTime,
									null );

				RefreshContent();
			}

		}
		
		//=========================================================================
		///	<summary>
		///		[�ԑg���폜]���j���[���N���b�N���ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void DeleteAnimeMenu_Click(object sender, EventArgs e)
		{
			AnimeServer server = AnimeServer.GetInstance();

			lock ( server )
			{
				List<AnimeProgram> delList = new List<AnimeProgram>();	// �폜���X�g

				//-----------------------------------
				// �폜�Ώۂ̔ԑg�����X�g�A�b�v
				//-----------------------------------
				var progs = GridSelectPrograms;

				foreach(AnimeProgram prog in progs)
				{
					DialogResult res;

					//------------------------------
					// �폜�m�F���b�Z�[�W��\��
					//------------------------------
					res = MessageBox.Show(
						this														,
						prog.title + " �����X�g����폜���Ă�낵���ł����H\n"		+
						"�E�\�ł���Η\����폜���܂��B\n"						+ 
						"�E�^��t�@�C���͂��̂܂܎c��܂��B"						,
						"�m�F"														,
						MessageBoxButtons.OKCancel									,
						MessageBoxIcon.Question										);

					if( res == DialogResult.OK )
					{
						delList.Add( prog );
					}
				}

				Cursor oldCursor = Cursor.Current;
				Cursor.Current = Cursors.WaitCursor;

				try
				{
					//------------------------------
					// ���ۂɔԑg�����X�g����폜
					//------------------------------
					foreach ( AnimeProgram ap in delList )
					{
						server.DeleteAnime( ap );
					}
				}
				catch ( Exception ex )
				{
					Program.ShowException( ex, MessageBoxIcon.Error );
				}
				finally
				{
					Cursor.Current = oldCursor;
				}

				RefreshContent();
			}

		}

		//=========================================================================
		///	<summary>
		///		�ԑg�̃v���p�e�B�c�[���{�^�����N���b�N���ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void propertyButton_Click(object sender, EventArgs e)
		{
			programPropertyMenu_Click(sender, e);
		}

		//=========================================================================
		///	<summary>
		///		[�ԑg�̃v���p�e�B]���j���[���N���b�N���ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void programPropertyMenu_Click(object sender, EventArgs e)
		{
			var progs = GridSelectPrograms;

			if( 1 == progs.Count )
			{
				var prog = progs[0];

				AnimeDialog dlg = new AnimeDialog();
				dlg.ShowDialog(ref prog);

				RefreshContent();
			}
		}

		//=========================================================================
		///	<summary>
		///		[�ŐV���ɍX�V]���j���[���ǉ����ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void RefreshMenu_Clicked(object sender, EventArgs e)
		{
			AnimeServer.GetInstance().BeginUpdate(0);
			RefreshContent();
		}

		//=========================================================================
		///	<summary>
		///		[���S�f�[�^�X�V]���j���[���ǉ����ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void ForceRefreshMenu_Click(object sender, EventArgs e)
		{
			AnimeServer.GetInstance().BeginUpdate(updateOption.Force);
			RefreshContent();
		}
		
		//=========================================================================
		///	<summary>
		///		�O���b�h�̗񂪃h���b�O���ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void dataGrid_MouseMove(object sender, MouseEventArgs e)
		{
			// �h���b�O�ňړ�����@�\

			//if (e.ColumnIndex < 0)
			if ((e.Button & MouseButtons.Left) > 0 && mDraggingRow==false)
			{
				DataGridView.HitTestInfo hit;

				// �N���b�N�����_�̃Z���𒲂ׂ�
				hit		= dataGrid.HitTest(e.X, e.Y);

				// �N���b�N�����Z���������̊D�F�Z����
				if (hit != DataGridView.HitTestInfo.Nowhere &&
					hit.ColumnIndex == -1 && hit.RowIndex >= 0)
				{
					DataGridViewRow row;

					row				= dataGrid.Rows[hit.RowIndex];			// �N���b�N���ꂽ�s
					row.Selected	= true;									// �s�I��
//					Console.WriteLine("�s�h���b�O({0:0})",((AnimeProgram)row.Tag).title);

					dataGrid.Capture = true;								// �f�[�^�O���b�h���}�E�X���L���v�`������
					dataGrid.Cursor = Cursors.Hand;

					mDraggingRow		= true;
					mDraggingItem	= row;

					return;
				}
			}

		}

		//=========================================================================
		///	<summary>
		///		�O���b�h�̗񂪃h���b�O�����������̏���
		///	</summary>
		/// <remarks>
		///		�h���b�O�ňړ�����@�\(v1.6.13)
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void dataGrid_MouseUp(object sender, MouseEventArgs e)
		{
			//----------------------------
			// �s�h���b�O��Ԃ�����
			//----------------------------
			if (mDraggingRow)
			{
				DataGridView.HitTestInfo hit;

				dataGrid.Cursor		= Cursors.Default;

				hit = dataGrid.HitTest(e.X, e.Y);					// �{�^�����グ���u�Ԃ̃Z���𓾂�

				if (hit != DataGridView.HitTestInfo.Nowhere)		// �h���b�v���ꂽ�_���O���b�h�ォ�H
				{
//					Console.WriteLine("{0:0},{1:0}", e.X, e.Y);
//					Console.WriteLine("{0:0},{1:0}", h.ColumnIndex, h.RowIndex);

					//-------------------------------------
					// �h���b�v���ׂ����X�g��̈ʒu���v�Z
					//-------------------------------------
					int y,offset;

					if (hit.RowIndex < 0)
					{
						y = 0;												// �Œ�s�Ȃ�g�b�v�ɑ}��
					}
					else
					{
						int cy	= e.Y - hit.RowY;							// �Z������Y�I�t�Z�b�g
						int h	= dataGrid.Rows[hit.RowIndex].Height;		// �Z���̍���
						offset	= 2 * cy / h;								// �s�̐^�����ォ����
						y		= hit.RowIndex + offset;					// �}������s
					}

					if (dataGrid.RowCount <= y)
						y = dataGrid.RowCount;

					if (0 <= y && y <= dataGrid.RowCount)
					{
						//--------------------------------
						// �ԑg���X�g�̓���ւ����s��
						//--------------------------------
//						Console.WriteLine( "�h���b�O��{0:0}",y );

						DataGridViewRow		row;
						AnimeServer			server;
						AnimeProgram		p;

						row		= (DataGridViewRow)mDraggingItem;			// �h���b�O���̍s
						row.Selected	= false;							// �s�I������
						server	= AnimeServer.GetInstance();
						p		= (AnimeProgram)row.Tag;					// �h���b�O�Ώۂ̔ԑg

						if (dataGrid.Rows.IndexOf(row) < y)
							y = y - 1;										// �h���b�O���̍s����Ȃ�h���b�O���-1

						server.DeleteAnime(p);
						server.AddAnime( p, y );

						RefreshContent();
					}
				}

				mDraggingRow			= false;
				mDraggingItem		= null;
				dataGrid.Capture	= false;								// �}�E�X�L���v�`��������
			}

		}

		//=========================================================================
		///	<summary>
		///		�O���b�h�̃Z�����N���b�N���ꂽ���A�G�s�\�[�h����\��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void dataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || e.ColumnIndex < 0)
				return;

			RefreshSelectedEpisodeInfo();
		}

		//=========================================================================
		///	<summary>
		///		�O���b�h�őI������Ă���G�s�\�[�h����\��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/31 dataGrid_CellClick���番��</history>
		//========================================================================
		private void RefreshSelectedEpisodeInfo()
		{
			AnimeProgram	prog			= null;
			AnimeEpisode	episode			= null;
			int				row;
			int				col;
			bool			isSelected		= false;
			var				episodes		= GridSelectEpisodes;

			if( 1 == episodes.Count )
			{
				row = dataGrid.CurrentCell.RowIndex;
				col = dataGrid.CurrentCell.ColumnIndex;
				
				episode	= episodes[0];
				prog	= episode.Parent;

				isSelected = (episode != null && 1 <= episode.StoryNumber );
			}

			if( isSelected )
			{
				int	storyNumber		= episode.StoryNumber;

				string[] RecordStateDescription =
				{
					"(�����I��)�^��t�@�C���Ȃ�",
					"�����v�����m��"			,
					"�^���"					,
					"�ăG���R�[�h��"			,
					"�ۑ��ς�"					,
					"�^��\���"				,
					"�ăG���R�[�h������"		,
					"��������"					,
					"�������ԕύX"				,
					"�������Ȃ�"				,
					"�^��\��Ɏ��s"			,
					"������"					,
				};

				titleLabel.Text			= prog.title +
										  string.Format(
												" ��{0:0}�b {1:0}"		,
												episode.StoryNumber		,
												episode.mSubTitle		);

				filePathLabel.Text		= episode.FilePath;
				RecordStateLabel.Text	= RecordStateDescription[(int)episode.CurrentState];

				if( !episode.HasPlan )
					dateTimeLabel.Text	= "����";
				else
				{
					DateTimeHelper dateTime = new DateTimeHelper(
												episode.StartDateTime				,
												Settings.Default.hoursPerDay - 24	);
					dateTimeLabel.Text = string.Format("{0:D4}/{1:D2}/{2:D2} {3:D2}:{4:D2} ({5:D2}min)",
											dateTime.Year,
											dateTime.Month,
											dateTime.Day,
											dateTime.Hour,
											dateTime.Minute,
											episode.Length);
				}

				if( episode.IsDoubleBooking() )
					if( Settings.Default.enablePriority )
						dateTimeLabel.Text += " (�`���[�i�s���̂��ߘ^��s�\)";
					else
						dateTimeLabel.Text += " (���ԑяd�����蒍��)";

				if( episode.PlanError )
					dateTimeLabel.Text += " �������f�[�^�ُ킠��";
			}
			else
			{
				titleLabel.Text			= "";
				filePathLabel.Text		= "";
				RecordStateLabel.Text	= "";
				dateTimeLabel.Text		= "";
			}
		}

		//=========================================================================
		///	<summary>
		///		�O���b�h�̃Z�����E�N���b�N���ꂽ���A���j���[��\��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2010/05/03 �}���`�Z���I��Ή�</history>
		//========================================================================
		private void dataGrid_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.RowIndex < 0 || e.ColumnIndex < 0)
				return;
			
			AnimeServer doc = AnimeServer.GetInstance();

			if((e.Button & MouseButtons.Right) > 0)
			{
				// �E�N���b�N�����Z����I��
				var				cell		= dataGrid.Rows[ e.RowIndex ].Cells[ e.ColumnIndex ];
				AnimeEpisode	selEpisode	= cell.Tag as AnimeEpisode;

				if(selEpisode != null)
				{
					if(GridSelectEpisodes.IndexOf(selEpisode) < 0)
					{
						dataGrid.ClearSelection();
						cell.Selected = true;
					}
				}

				var episodes = GridSelectEpisodes;

				if( 1 <= episodes.Count )
				{
					bool	isMulti	= (1 < episodes.Count);


					//-----------------------------------------------
					// �E�N���b�N���j���[���ڂ̗L��/������ݒ�
					//-----------------------------------------------
					bool	enablePlay		= true;
					bool	enableEncode	= true;
					bool	enableStore		= true;
					bool	enableReserve	= true;
					bool	enableRename	= true;
					bool	enableCancel	= true;
					bool	enableUnread	= true;
					bool	enableProp		= true;

					foreach(AnimeEpisode ep in episodes)
					{
						enablePlay		&= !isMulti && ep.IsPlayable;
						enableEncode	&= (ep.HasFile && !ep.IsEncoded && !ep.IsStored);
						enableStore		&= (ep.HasFile && !ep.IsStored);
						
						enableReserve	&=	ep.HasPlan
										&&	(!ep.IsReserved || ep.JudgeTimeChanged)
										&&	!ep.JudgeTimeEnd(DateTime.Now)
										&&	ep.IsRecordRequired;

						enableRename	&= ep.HasFile;

						// �L�����Z���ł���̂͘^��J�n����܂�
						enableCancel	&= ep.IsReserved && !ep.IsStartedOnair;

						enableUnread	&= !isMulti;

						enableProp		&= !isMulti;
					}

					//--------------------------------
					// �g���c�[������
					//--------------------------------
					if (Settings.Default.externalTools != null)
					{
						try
						{
							ToolStripItemCollection menuItems = contextMenuStrip.Items;
							List<ToolStripItem> delItems = new List<ToolStripItem>();

							// ���̍��ڂ��폜
							for (int i = menuItems.IndexOf(extToolsGroupSeparator) + 1;
								 i <= menuItems.IndexOf(encodeGroupSeparator) - 1;
								++i)
							{
								delItems.Add(menuItems[i]);
							}
							foreach (ToolStripItem item in delItems)
								menuItems.Remove(item);

							// �c�[�����ڂ�ǉ�
							ExternalToolsSetting	toolSetting =  Settings.Default.externalTools;
							for (int i = 0; i < toolSetting.tools.Count; ++i)
							{
								int pos;
								ToolStripItem item = new ToolStripMenuItem();
								item.Text		= toolSetting.tools[i].toolName;
								item.Enabled	= !isMulti && episodes[0].IsPlayable;
								item.Click		+= new EventHandler( ExternalToolItemClicked );
								item.Tag		= toolSetting.tools[i];

								pos = menuItems.IndexOf(encodeGroupSeparator);
								menuItems.Insert(pos, item);
							}

							extToolsGroupSeparator.Visible = (0 < Settings.Default.externalTools.tools.Count);
						}
						catch (Exception ex)
						{
						}
					}


					playMovieMenu.Enabled		= enablePlay;
					encodeMenu.Enabled			= enableEncode;
					storeMenu.Enabled			= enableStore;
					reserveMenu.Enabled			= enableReserve;
					renameFileMenu.Enabled		= enableRename;
					cancelReserveMenu.Enabled	= enableCancel;
					unreadMenu.Enabled			= enableUnread;
					RecordPropertyMenu.Enabled	= enableProp;

					unreadMenu.Checked = !isMulti && episodes[0].Unread;
					unreadMenu.Visible = !Settings.Default.disableUnread;


					//--------------------------------
					// �J�[�\���ʒu�Ƀ��j���[�\��
					//--------------------------------

					Point pt = PointToClient(Cursor.Position);
					
					contextMenuStrip.Show(this, pt);
				}
				
			}
			
		}

		//=========================================================================
		///	<summary>
		///		�O���b�h�̃Z�����_�u���N���b�N���ꂽ���̏���
		///	</summary>
		/// <remarks>
		///		�_�u���N���b�N�ōĐ�/�\��
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void dataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			AnimeServer server = AnimeServer.GetInstance();
		
			if (e.RowIndex < 0 || e.ColumnIndex < 0)
				return;

			var				episodes	= GridSelectEpisodes;
			AnimeEpisode	ep			= null;

			if( 1 != episodes.Count )
				return;

			ep = episodes[0];

			//---------------------------------
			// �G�s�\�[�h���Đ��\�Ȃ�Đ�
			//---------------------------------
			if (ep.IsPlayable)
			{
				// �d�����������Ă���ԂɃ}�E�X�������ƃZ�����I������Ă��܂����߁A�n���h���O�ɓ�����
				KernelAPI.Window.PostMessage(
					this.Handle,
					KernelAPI.Window.WM_USER,
					IntPtr.Zero,
					IntPtr.Zero );
				return;
			}

			//-------------------------------
			// �G�s�\�[�h�̗\�������
			//-------------------------------
			bool	enableReserve	=	ep.HasPlan
									&&	(!ep.IsReserved || ep.JudgeTimeChanged)
									&&	!ep.JudgeTimeEnd(DateTime.Now)
									&&	ep.IsRecordRequired;

			if( enableReserve )
			{
				reserveMenu_Click( sender, null );
				return;
			}

		}

		//=========================================================================
		///	<summary>
		///		�E�B���h�E�v���V�[�W��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/05/07 �V�K�쐬</history>
		//========================================================================
		protected override void WndProc(ref Message m)
		{
			if( m.Msg == KernelAPI.Window.WM_USER )
			{
				playMovieMenu_Click(null, null);
				return;
			}

			base.WndProc(ref m);
		}

		//=========================================================================
		///	<summary>
		///		[�I��]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void ExitMenu_Click(object sender, EventArgs e)
		{
			mExitFlag = true;
			this.Close();
		}
		
		//=========================================================================
		///	<summary>
		///		�Z���̉E�N���b�N[�Đ�]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void playMovieMenu_Click(object sender, EventArgs e)
		{
			try
			{
				var				episodes	= GridSelectEpisodes;
				AnimeEpisode	ep;

				if( 1 != episodes.Count )
					return;
			
				ep = episodes[0];

				PlayMovie( ep.FilePath );
				ep.Unread = false;
				Invalidate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "�G���[",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//=========================================================================
		///	<summary>
		///		�w�肳�ꂽ����t�@�C�����Đ�����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		internal void PlayMovie(string filePath)
		{
			FormWindowState oldState;
			Process process;

			//--------------------------------
			// �֘A�t����ꂽ�v���C���[�N��
			//--------------------------------
			process = Process.Start(filePath);

			if (Settings.Default.minimizeAtPlayer)
			{
				//-----------------------------
				// �Đ����͍ŏ������ău���b�N
				//-----------------------------

				oldState = this.WindowState;
				this.WindowState = FormWindowState.Minimized;
				dataGrid.Enabled	= false;
				menuStrip.Enabled	= false;

				for (; !process.WaitForExit(500); )
				{
					// �K�v�Ȃ�DoEvents
				}

				dataGrid.Enabled	= true;
				menuStrip.Enabled	= true;
				this.WindowState	= oldState;
			}

		}


		//=========================================================================
		///	<summary>
		///		[�ăG���R�[�h]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void encodeMenu_Click(object sender, EventArgs e)
		{
			var episodes = GridSelectEpisodes;

			episodes.ForEach(ep =>  AnimeServer.GetInstance().AddEncodeJob(ep));
		}

		//=========================================================================
		///	<summary>
		///		[�ۑ���ɓ]��]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void storeMenu_Click(object sender, EventArgs e)
		{
			var episodes = GridSelectEpisodes;

			foreach(var ep in episodes)
			{
				if( ep.HasFile && !ep.IsEncoded && (ep.Parent.EncoderType != null) )
				{
					//---------------------------------------------------
					// �ăG���R�[�h�����ɓ]������ۂ̊m�F���b�Z�[�W�\��
					//---------------------------------------------------
					DialogResult dlgResult;

					dlgResult = MessageBox.Show(
						ep.ToString() + System.Environment.NewLine +
						"�ăG���R�[�h���Ă��܂��񂪁A���̂܂ܕۑ��t�H���_�ֈړ����܂����H",
						"�m�F",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Question);

					if (dlgResult != DialogResult.Yes)
						continue;
				}

				try
				{
					ep.Store();
				}
				catch(Exception ex)
				{
					Program.ShowException(ex,MessageBoxIcon.Warning);
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		�t�H�[����������ԍۂ̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (	e.CloseReason == CloseReason.UserClosing	&&
					Settings.Default.inTaskTray					&&
					mExitFlag == false							)
			{
				//-----------------------------------------------------
				// ���[�U�[��[X]�{�^������������^�X�N�g���C�ɓ����
				//-----------------------------------------------------
				this.Hide();
				e.Cancel = true;

				Program.ShowTrayIcon();
			}
			else
			{
				//---------------------------
				// ���ۂɃA�v�����I��
				//---------------------------

				// �I�����Ă悢���₢���킹
				if( !Program.QueryClose() )
				{
					e.Cancel = true;
					mExitFlag = false;
				}
			}


			if(e.CloseReason==CloseReason.TaskManagerClosing)
			    Logger.Output( "�^�X�N�}�l�[�W���ɂ��I��" );

			if( e.Cancel == false )
			{
				Application.Exit();
			}
		}

		//=========================================================================
		///	<summary>
		///		[�o�[�W�������]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void AboutMenu_Click(object sender, EventArgs e)
		{
			AboutBox aboutBox = new AboutBox();
			aboutBox.ShowDialog();
		}

		//=========================================================================
		///	<summary>
		///		�E�N���b�N[�\��]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void reserveMenu_Click(object sender, EventArgs e)
		{
			Cursor	cur			= this.Cursor;
			var		episodes	= GridSelectEpisodes;
			ReserveManager	rm	= new ReserveManager();

			try
			{
				string			err;

				this.Cursor = Cursors.WaitCursor;

				foreach(var ep in episodes)
				{
					if( ep.IsReservePending() )
					{
						DialogResult	result;

						result = MessageBox.Show(
							this,
							ep.ToString() + System.Environment.NewLine +
							"�^��`���[�i������Ȃ����߁A���̔ԑg�̗\���ۗ����Ă��܂��B\n" +
							"�����I�ɗ\�񂵂܂����H", 
							"�m�F",
							MessageBoxButtons.OKCancel,
							MessageBoxIcon.Warning,
							MessageBoxDefaultButton.Button2);

						if( result != DialogResult.OK )
							continue;
					}

					if( !ep.Reserve( rm, out err ) )
					{
						MessageBox.Show(
							this,
							ep.ToString() + System.Environment.NewLine +
							err,
							"�\��̎��s",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error );
					}
				}

				rm.Flush();
			}
			catch ( Exception ex )
			{
				MessageBox.Show(
					this,
					ex.Message,
					"�G���[",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );
			}
			finally
			{
				this.Cursor = cur;
			}
		}

		//=========================================================================
		///	<summary>
		///		�Z���̉E�N���b�N[�v���p�e�B]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void RecordPropertyMenu_Clicked(object sender, EventArgs e)
		{
			var episodes = GridSelectEpisodes;

			if(1 == episodes.Count)
			{
				RecordDialog recordDialog = new RecordDialog();

				recordDialog.ShowDialog( episodes[0] );
				recordDialog.Dispose();
			}
		}

		//=========================================================================
		///	<summary>
		///		�t�H�[�������ۂɕ���ꂽ��̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			// �\�����[�h�Ɋւ���ݒ��ۑ�

			Settings.Default.viewMode = (int)mViewMode;
			Settings.Default.thumbnailMode = thumbnailModeButton.Checked;

			//----------------------------
			// �E�B���h�E�T�C�Y��ۑ�
			//----------------------------
			if ( this.WindowState == FormWindowState.Normal )
			{
				Settings.Default.rectWindow		= this.Bounds;
				Settings.Default.maximizeWindow	= false;
			}
			else if ( this.WindowState == FormWindowState.Maximized )
			{
//				Settings.Default.rectWindow = this.Bounds;
				Settings.Default.maximizeWindow	= true;
			}

			// �y�C�������ʒu��ۑ�
			Settings.Default.logPaneSize = viewSplitContainer.Height
										 - viewSplitContainer.SplitterDistance;

#if _LAZYDRAW
// <ADD> 2008/03/25 ->
			drawTimer.Dispose();
// <ADD> 2008/03/25 <-
#endif

			Settings.Default.Save();

			AnimeServer.GetInstance().Save();
		}

		//=========================================================================
		///	<summary>
		///		���ǃc�[���{�^�����N���b�N���ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void unreadMenu_Click(object sender, EventArgs e)
		{
			var				episodes	= GridSelectEpisodes;
			AnimeEpisode	ep;

			try
			{
				if( 1 != episodes.Count )
					return;

				ep = episodes[0];

				ep.Unread = !ep.Unread;
				unreadMenu.Checked = ep.Unread;

				RefreshContent();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//=========================================================================
		///	<summary>
		///		���ǃ��j���[���ڂ��N���b�N���ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void playUnreadButton_DropDownItemClicked(
			object sender,
			ToolStripItemClickedEventArgs e)
		{
			AnimeEpisode record;

			try
			{
				record = (AnimeEpisode)e.ClickedItem.Tag;

				PlayMovie( record.FilePath );

				record.Unread = false;

				RefreshContent();
				record.Dirty = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "�G���[",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

		//=========================================================================
		///	<summary>
		///		���ǃc�[���{�^�����N���b�N���ꂽ���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void playUnreadButton_ButtonClick(object sender, EventArgs e)
		{
			//
			// ��ԏ�̃A�C�e�����N���b�N���ꂽ���Ƃɂ���
			//
			ToolStripItemClickedEventArgs eventArgs
				= new ToolStripItemClickedEventArgs(playUnreadButton.DropDownItems[0]);

			playUnreadButton_DropDownItemClicked( playUnreadButton, eventArgs );

		}

		//=========================================================================
		///	<summary>
		///		�ŏI�ۑ���ɓ]���c�[���{�^���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void StoreAllButton_Click(object sender, EventArgs e)
		{
			StoreAllMenu_Click(sender, e);
		}

		//=========================================================================
		///	<summary>
		///		[�ŏI�ۑ���ɓ]��]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void StoreAllMenu_Click(object sender, EventArgs e)
		{
			TransferDialog dlg = new TransferDialog();

			dlg.ShowDialog();
		}

		//=========================================================================
		///	<summary>
		///		[�����T�C�g]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void helpMenu_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process process;

			process = System.Diagnostics.Process.Start( Settings.Default.helpUrl );

		}

		//=========================================================================
		///	<summary>
		///		�ăG���R�[�h���j���[�̃G���R�[�h�����X�g���X�V
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2008/11/16 �G���R�[�h�𕡐��������s�O��ɂ����B</history>
		//========================================================================
		private void PowerMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			ToolStripItemCollection c;
			List<EncodeJob> jobs;
			AnimeServer server = AnimeServer.GetInstance();

			AutoShutdownMenu.Checked = server.AutoShutdown;		// �����V���b�g�_�E���t���O

			//---------------------------------
			// �G���R�[�h�҂��W���u�ꗗ���X�V
			//---------------------------------

			c = BatchListMenuItem.DropDown.Items;

			// �G���R�[�h�҂����j���[���ڍ폜
			foreach( ToolStripMenuItem item in mQueuingMenuItems )
			{
				c.Remove( item );
			}
			mQueuingMenuItems.Clear();

			jobs = server.GetQueueingEncodeJobs();
			foreach (EncodeJob job in jobs)
			{
				ToolStripItem item;
				item = c.Add(job.ToString());
				item.Enabled = false;
				mQueuingMenuItems.Add( item );
			}
			BatchListNothingMenuItem.Visible	= (mQueuingMenuItems.Count == 0);

			//--------------------------------
			// �G���R�[�h���̃W���u�ꗗ���X�V
			//--------------------------------
			EncodeJob[] encodingJobs;

			foreach( ToolStripMenuItem item in mEncodingMenuItems )
			{
				c.Remove( item );
			}
			mEncodingMenuItems.Clear();

			encodingJobs = server.GetCurrentJobs();

			int insertIndex = c.IndexOf( EncodingNothingMenuItem );

			foreach (EncodeJob job in encodingJobs)
			{
				ToolStripItem item;
				item = new ToolStripMenuItem( job.ToString() );
				c.Insert( insertIndex++, item );
				item.Enabled = false;

				mEncodingMenuItems.Add( item );
			}

			EncodingNothingMenuItem.Visible		= (mEncodingMenuItems.Count == 0);

			BatchListNothingMenuItem.Visible = jobs.Count == 0 ? true : false;
		}

		//=========================================================================
		///	<summary>
		///		[�����I�ɃV���b�g�_�E��]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void AutoShutdownMenu_Click(object sender, EventArgs e)
		{
			AnimeServer.GetInstance().AutoShutdown = !AutoShutdownMenu.Checked;
		}

		private void PowerMenuItem_Click(object sender, EventArgs e)
		{

		}

		//=========================================================================
		///	<summary>
		///		[�G���R�[�h���f]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void JobsCancelMenu_Click(object sender, EventArgs e)
		{
			AnimeServer.GetInstance().AutoShutdown = false;					// �����V���b�g�_�E������
			AnimeServer.GetInstance().CancelJobs();
		}

		//=========================================================================
		///	<summary>
		///		[�S�čăG���R�[�h]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void BatchEncodeAllMenu_Click(object sender, EventArgs e)
		{
			AnimeServer.GetInstance().BatchEncodeAll();
		}

		//=========================================================================
		///	<summary>
		///		[���O�\��]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void LogShowMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				TextLogger logger = Program.FileLogger;
				logger.ShowLog();
			}
			catch (Exception c)
			{
				Logger.Output(c.Message);
			}
		}

		//=========================================================================
		///	<summary>
		///		�b���r���[���[�h�ɐ؂�ւ��郁�j���[
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void seriesModeMenu_Click(object sender, EventArgs e)
		{
			mViewMode = ViewMode.SeriesMode;
			RefreshContent();
		}

		//=========================================================================
		///	<summary>
		///		�J�����_�[�r���[���[�h�ɐ؂�ւ��郁�j���[
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void calenderModeMenu_ButtonClick(object sender, EventArgs e)
		{
			if( Settings.Default.calenderMode == 0 )
				mViewMode = ViewMode.WeekCalenderMode;
			else
				mViewMode = ViewMode.DayCalenderMode;

			RefreshContent();
			RefreshControl();
		}

		private void weakModeMenu_Click(object sender, EventArgs e)
		{
			mViewMode = ViewMode.WeekCalenderMode;
			RefreshContent();

			Settings.Default.calenderMode = 0;
			Settings.Default.Save();
		}

		private void dayModeMenu_Click(object sender, EventArgs e)
		{
			mViewMode = ViewMode.DayCalenderMode;
			RefreshContent();

			Settings.Default.calenderMode = 1;
			Settings.Default.Save();
		}

		//=========================================================================
		///	<summary>
		///		�J�����_�[�r���[���[�h�̃I�v�V������ʂ�\�����郁�j���[
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void calenderModeOptionMenu_Click(object sender, EventArgs e)
		{
			CalenderOptionDialog dlg = new CalenderOptionDialog();

			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				Settings.Default.Save();

				RefreshContent();
			}
		}

		//=========================================================================
		///	<summary>
		///		�T���l�C�����[�h�ؑփ{�^���̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void thumbnailModeButton_Click(object sender, EventArgs e)
		{
			RefreshContent();
		}

		//=========================================================================
		///	<summary>
		///		�Z���̏�Ԃ��ύX���ꂽ���A�t�H�[�J�X���𑦎��ɕ`��
		///	</summary>
		/// <remarks>
		///		�x���`�悷��Ǝc���ɂȂ��Ă��܂����߁B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void dataGrid_CellStateChanged(
			object sender,
			DataGridViewCellStateChangedEventArgs e )
		{
			DrawGridCell(
				dataGrid.CreateGraphics()	,
				e.Cell.RowIndex				,
				e.Cell.ColumnIndex			,
				e.Cell.State				);
		}

		private void dataGrid_MouseLeave( object sender, EventArgs e )
		{

		}

		private void dataGrid_CellLeave( object sender, DataGridViewCellEventArgs e )
		{
		}

		//=========================================================================
		///	<summary>
		///		�X�e�[�^�X�o�[�̘^���TIPS���N���b�N�����Ƃ��A�^��t�H���_���J��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void recordDriveFreeSpaceLabel_Click( object sender, EventArgs e )
		{
			if ( Directory.Exists( Settings.Default.captureFolder ) )
			{
				Process.Start(Settings.Default.captureFolder);
			}
		}

		//=========================================================================
		///	<summary>
		///		�X�e�[�^�X�o�[�̕ۑ���TIPS���N���b�N�����Ƃ��A�^��t�H���_���J��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void storeFolderLabel_Click( object sender, EventArgs e )
		{
			if ( Directory.Exists( Settings.Default.saveFolder ) )
			{
				Process.Start( Settings.Default.saveFolder );
			}
		}

		//=========================================================================
		///	<summary>
		///		[�ԑg�\�[�g]���j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void SortMenu_Click( object sender, EventArgs e )
		{
			magicAnime.UserInterface.SortDialog dlg = new magicAnime.UserInterface.SortDialog();

			if ( dlg.ShowDialog() == DialogResult.OK )
			{
				RefreshContent();
			}
		}

		//=========================================================================
		///	<summary>
		///		�S�G�s�\�[�h�̖��ǃt���O���������郁�j���[�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/10/25 �V�K�쐬</history>
		//========================================================================
		private void OnReleaseUnreadMenu_Click( object sender, EventArgs e )
		{
			if( MessageBox.Show(
				"�S�Ă̖��ǃt���O���������Ă�낵���ł����H"	,
				"�m�F"										,
				MessageBoxButtons.OKCancel					,
				MessageBoxIcon.Question						) == DialogResult.OK )
			{
				AnimeServer server = AnimeServer.GetInstance();
				AnimeProgram.EpisodeList list;

				list = server.QueryEpisode( null );					// �S�ԑg�̑S�G�s�\�[�h��񋓂���

				foreach( AnimeEpisode episode in list )
				{
					episode.Unread = false;
				}

				RefreshContent();
			}
		}

		//=========================================================================
		///	<summary>
		///		[�V���b�g�_�E��]���j���[�i�f�o�b�O�p�j
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void debugShutdownMenu_Click( object sender, EventArgs e )
		{
			Program.TryShutdown();
		}

		//=========================================================================
		///	<summary>
		///		[�ۑ��t�@�C�����Ƀ��l�[��]���j���[
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void renameFileMenu_Click( object sender, EventArgs e )
		{
			string			newName;

			var				episodes	= GridSelectEpisodes;

			foreach(var ep in episodes)
			{
				if( ep.RenameFile( out newName ) )
				{
					MessageBox.Show(	"�t�@�C���������l�[�����܂����B\r\n" + newName,
										"����"						,
										MessageBoxButtons.OK		,
										MessageBoxIcon.Information	);
				}
				else
				{
					MessageBox.Show(	"�t�@�C�����̃��l�[���Ɏ��s���܂����B",
										null					,
										MessageBoxButtons.OK	,
										MessageBoxIcon.Warning	);
				}
			}	
		}

		//=========================================================================
		///	<summary>
		///		[�\��L�����Z��]���j���[
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void cancelReserveMenu_Click( object sender, EventArgs e )
		{
			string			newName;
			var				episodes	= GridSelectEpisodes;
			ReserveManager	manager		= new ReserveManager();

			foreach(var ep in episodes)
			{
				if( ep.CancelReserve( manager ) )
				{
//					MessageBox.Show(	"�\����L�����Z�����܂����B",
//										"����"						,
//										MessageBoxButtons.OK		,
//										MessageBoxIcon.Information	);
				}
				else
				{
					if( MessageBox.Show(
						ep.ToString() + System.Environment.NewLine +
						"�\����L�����Z���ł��܂���ł����B\r\n"				+
						"(�^��\�t�g����\����L�����Z�����ĉ�����)\r\n\r\n"	+
						"�G�s�\�[�h�̏�Ԃ������I�Ɂu���\��v�ɂ��܂����H"		,
						null									,
						MessageBoxButtons.YesNo					,
						MessageBoxIcon.Warning					,
						MessageBoxDefaultButton.Button2�@		) == DialogResult.Yes )
					{
						ep.IsReserved = false;
					}
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		�O���c�[�����j���[���ڂ̃N���b�N����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		private void ExternalToolItemClicked(object sender, EventArgs e)
		{
			try
			{
				ToolStripItem	menuItem	= sender as ToolStripItem;
				var				episodes	= GridSelectEpisodes;

				if( 1 != episodes.Count )
					return;

				var	 ep = episodes[0];

				if( menuItem != null )
				{
					ExternalToolItem toolItem = (menuItem.Tag as ExternalToolItem);
					if( toolItem != null )
					{
						Process				proc;
						ProcessStartInfo	starter = new ProcessStartInfo( toolItem.toolPath );

						starter.Arguments = string.Format(
							toolItem.toolCommandLine	,
							ep.FilePath					);

						proc = Process.Start(starter);
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(	"�O���c�[���̋N���Ɏ��s���܂����B" + System.Environment.NewLine +
									ex.Message );
			}
		}

		//=========================================================================
		///	<summary>
		///		�\�����t���b�V���^�C�}
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/11/22 �V�K�쐬</history>
		//========================================================================
		private void OnRefreshTimer(object sender, EventArgs e)
		{
			try
			{
				RefreshIfModified();
				UpdateStatusBar();
				RefreshControl();
				ShowRealtimeLog();
			}
			catch(Exception ex)
			{
			}
		}

		//=========================================================================
		///	<summary>
		///		���O�\���X�V
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/28 �V�K�쐬</history>
		//========================================================================
		private void ShowRealtimeLog()
		{
			MemoryLogger	logger = Program.MemoryLogger;
			List<string>	log;
			log = logger.GetLog();

			for( int i = 0 ; i < log.Count ; ++i )
				logListBox.Items.Add( log[i] );

			if( 0 < log.Count )
				logListBox.SelectedIndex = logListBox.Items.Count - 1;
		}

		//=========================================================================
		///	<summary>
		///		�f�[�^�X�V���ʃ��x���̃N���b�N������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/11/23 �V�K�쐬</history>
		//========================================================================
		private void logStatusLabel_Click(object sender, EventArgs e)
		{
			AnimeServer.MyStatus newStatus = AnimeServer.GetInstance().GetStatus();

			if ( !string.IsNullOrEmpty( newStatus.resultLastUpdate) )
			{
				MessageBox.Show(
					newStatus.resultLastUpdate,
					"�f�[�^�X�V����",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);

				AnimeServer.GetInstance().ClearResultUpdate();
			}
		}

        private void applicationDataMenuItem_Click(object sender, EventArgs e)
        {
			Process.Start( Program.AppDataPath );
        }

		private void logButton_Click(object sender, EventArgs e)
		{
			Settings.Default.showLogPane = !Settings.Default.showLogPane;
			RefreshControl();
		}

		//=========================================================================
		///	<summary>
		///		�R���g���[���̏�ԍX�V
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/29 �V�K�쐬</history>
		//========================================================================
		void RefreshControl()
		{
			logButton.Checked = Settings.Default.showLogPane;

			bool singleSelected = (1 == GridSelectPrograms.Count);

			// ���O�\���y�C���\��
			if( Settings.Default.showLogPane )
				viewSplitContainer.Panel2Collapsed = false;
			else
				viewSplitContainer.Panel2Collapsed = true;

			// �֑���Ԃ̍X�V
			AnimeServer.MyStatus	stat;
			stat = AnimeServer.GetInstance().GetStatus();

			refreshButton.Enabled		= !stat.updateSequenceBusy;
			RefreshMenu.Enabled			= !stat.updateSequenceBusy;
			ForceRefreshMenu.Enabled	= !stat.updateSequenceBusy;

			OnReleaseUnreadMenu.Enabled	= !Settings.Default.disableUnread;
			seriesModeMenu.Checked		= (mViewMode == ViewMode.SeriesMode);
			weekModeMenu.Checked		= (mViewMode == ViewMode.WeekCalenderMode);
			dayModeMenu.Checked			= (mViewMode == ViewMode.DayCalenderMode);

			propertyButton.Enabled		= singleSelected;
			programPropertyMenu.Enabled	= singleSelected;
		}

// <ADD> 2010/04/17 �f�o�b�O�p ->
		private void debugForceEmptyMenu_Click(object sender, EventArgs e)
		{
#if DEBUG
			Program.DebugOption.mForceEmpty ^= true;
#endif
		}

		private void debugMenu_DropDownOpening(object sender, EventArgs e)
		{
#if DEBUG
			debugForceEmptyMenu.Checked = Program.DebugOption.mForceEmpty;
#endif
		}
// <ADD> 2010/01/29 <-

		//=========================================================================
		///	<summary>
		///		�O���b�h�Z���I�����̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/05/03 �V�K�쐬</history>
		//========================================================================
		private void dataGrid_SelectionChanged(object sender, EventArgs e)
		{
			RefreshSelectedEpisodeInfo();
		}

		//=========================================================================
		///	<summary>
		///		�O���b�h�őI������Ă���G�s�\�[�h
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/05/03 �V�K�쐬</history>
		//========================================================================
		private List<AnimeEpisode> GridSelectEpisodes
		{
			get
			{
				var episodes = new List<AnimeEpisode>();

				try
				{
					// �I������Ă���G�s�\�[�h���擾
					foreach(DataGridViewCell cell in dataGrid.SelectedCells)
					{
						AnimeProgram prog;
						AnimeEpisode ep;
						int			storyNumber;

						prog = (AnimeProgram)dataGrid.Rows[cell.RowIndex].Tag;

						if (prog != null)
						{
							ep = (AnimeEpisode)dataGrid.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Tag;

							if (ep != null)
							{
								storyNumber = ep.StoryNumber;

								if(1 <= storyNumber)
								{
									episodes.Add(ep);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					episodes.Clear();
				}
				return episodes;
			}
		}
	
		//=========================================================================
		///	<summary>
		///		�O���b�h�őI������Ă���ԑg
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/05/03 �V�K�쐬</history>
		//========================================================================
		private List<AnimeProgram> GridSelectPrograms
		{
			get
			{
				var progs = new List<AnimeProgram>();

				try
				{
					// �I������Ă���v���O�������擾
					foreach(DataGridViewCell cell in dataGrid.SelectedCells)
					{
						AnimeProgram prog;

						prog = (AnimeProgram)dataGrid.Rows[cell.RowIndex].Tag;

						if (prog != null)
							if( progs.IndexOf(prog) < 0 )
								progs.Add(prog);
					}
				}
				catch(Exception ex)
				{
					progs.Clear();
				}
				return progs;
			}
		}

	}
	
}

