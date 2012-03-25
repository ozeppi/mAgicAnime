//=========================================================================
///	<summary>
///		mAgicAnimeメイン画面 モジュール
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
	///		mAgicAnimeメイン画面クラス
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	/// <history>2008/05/02 メンバ変数を上端に移動</history>
	//=========================================================================
	partial class MainForm : Form
	{
		//--------------------------------
		// グリッドセルの遅延描画
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
		// アイコンリソース
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
		// ユーザーインターフェースメンバ
		//----------------------------------
//		private Timer		drawTimer;
		private Timer		mRefreshTimer;					// 更新用タイマ
		private Font		mBigFont;						// エピソード番号フォント
		
		private bool		mExitFlag		= false;		//

		private bool		mDraggingRow	= false;		// 行ドラッグ中フラグ
		private object		mDraggingItem;					// ドラッグ中のアイテム

		private int			mAspect			= 10;			// 96dpiに対するアスペクトの比*10

		public enum ViewMode
		{
			SeriesMode			,	// 話数モード
			DayCalenderMode		,	// 日モード
			WeekCalenderMode	,	// 週モード
		}

		private ViewMode	mViewMode;						// データグリッドの表示モード


		//-----------------
		// その他
		//-----------------
		private List<ToolStripItem>	mQueuingMenuItems;		// エンコード待ちメニュー項目リスト
		private List<ToolStripItem>	mEncodingMenuItems;		// エンコード中メニュー項目リスト
		
		//=========================================================================
		///	<summary>
		///		コンストラクタ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		[オプション]メニューがクリックされた時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void OptionMenu_Click(object sender, EventArgs e)
		{
			OptionDialog dlg = new OptionDialog();
			
			dlg.ShowDialog();

			RefreshContent();

			// オプション設定を反映
			Program.OptionChanged(null,null);
		}


		//=========================================================================
		///	<summary>
		///		フォームがロードされた時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void MainForm_Load(object sender, EventArgs e)
		{

			mViewIcons.LoadIcons();

			// データ表示更新タイマ
			mRefreshTimer = new Timer();
			mRefreshTimer.Tick += OnRefreshTimer;
			mRefreshTimer.Interval = 100;
			mRefreshTimer.Start();

			//-----------------------
			// グリッドの初期化
			//-----------------------
			mCellBuffer			= new Bitmap(300,300);
			mCellGraphics		= Graphics.FromImage( mCellBuffer );
#if _LAZYDRAW
			cellRefreshRequest	= new List<CellRefresh>();

			// グリッド遅延描画タイマ準備
			drawTimer			= new Timer();
			drawTimer.Tick		+= OnLazyDrawCell;
			drawTimer.Interval	= 300;
			drawTimer.Start();
#endif

			//--------------------------
			// その他の初期化
			//--------------------------
			mViewMode = (ViewMode)Settings.Default.viewMode;
			thumbnailModeButton.Checked = Settings.Default.thumbnailMode;

// <ADD> 2010/04/17 デバッグオプション ->
#if DEBUG
			debugMenu.Visible = true;
#else
			debugMenu.Visible = false;
#endif
// <ADD> 2010/04/17 <-

			RefreshContent();

			//--------------------------------
			// オプション設定に応じて最小化
			//--------------------------------
			if ( Settings.Default.minimizeAtStartup	&&
				!Settings.Default.inTaskTray		)
			{
				this.WindowState = FormWindowState.Minimized;			// 最小化したまま起動
			}
		
				
			mBigFont = new Font("Arial Black", 14, FontStyle.Regular);
			
			//---------------------------------
			// 画面のDPIからアスペクト比を計算
			//---------------------------------

			KernelAPI.GDI.TEXTMETRIC tm = new KernelAPI.GDI.TEXTMETRIC();

			if ( KernelAPI.GDI.GetTextMetrics( KernelAPI.GDI.GetDC( 0 ), ref tm ) != 0 )
			{
				mAspect = (int)( tm.tmDigitizedAspectX * 10 / 96 );
				mAspect = mAspect * (int)tm.tmAveCharWidth / 8; // システムフォントサイズの違いも補正
			}

			//--------------------------------------
			// アスペクト比率をグリッドセル幅に適用
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
		///		フォームが表示された時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void MainForm_Shown(object sender, EventArgs e)
		{
			//--------------------------------
			// ウィンドウ位置とサイズを復元
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
		///		ドキュメント表示の更新を行う
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2008/05/02 メソッド名変更(OnUpdate->RefreshContent)</history>
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
					cw = Settings.Default.thumbnailWidth;		// サムネイルモードのセル幅
				}
				else
				{
					cw = 48;									// 非サムネイルモードのセル幅
				}

				//----------------------------
				// 列の拡縮を行う
				//----------------------------

				addCols = 0;

				switch (mViewMode)
				{
					case ViewMode.SeriesMode:
						//----------------------------
						// 話数表示モード
						//----------------------------

						foreach( AnimeProgram prog in server.Animes )
						{
							addCols = System.Math.Max( addCols, prog.StoryCount );
						}

						addCols = Math.Min(addCols, 500);	// グリッドの横幅最大値をオーバーしないため
						dataGrid.ColumnCount = ColumnStoryCount.Index + addCols + 1;

						for (i = 0; i < addCols; ++i)
						{
							DataGridViewColumn c = dataGrid.Columns[i + ColumnStoryCount.Index + 1];
							c.Width			= cw * mAspect / 10;
							c.HeaderText	= string.Format("{0:0}話", i + 1);
						}
						break;
					case ViewMode.DayCalenderMode:
						//----------------------------
						// 日ごとカレンダーモード
						//----------------------------

						addCols = Settings.Default.dayPast + Settings.Default.dayFuture + 1;
						dataGrid.ColumnCount = ColumnStoryCount.Index + addCols + 1;

						for (i = 0; i < addCols; ++i)
						{
							DataGridViewColumn	c = dataGrid.Columns[i + ColumnStoryCount.Index + 1];
							int					d = i - Settings.Default.dayPast;

							c.Width = cw * mAspect / 10;
							if ( d == -1 )
								c.HeaderText = "昨日";
							else if ( d == 0 )
								c.HeaderText = "今日";
							else if ( d == +1 )
								c.HeaderText = "明日";
							else
								c.HeaderText = string.Format( "{0:0}日", DateTime.Now.AddDays( d ).Day );
						}
						break;
					case ViewMode.WeekCalenderMode:
						//----------------------------
						// 週ごとカレンダーモード
						//----------------------------

						addCols = Settings.Default.weekPast + Settings.Default.weekFuture + 1;
						dataGrid.ColumnCount = ColumnStoryCount.Index + addCols + 1;

						for (i = 0; i < addCols; ++i)
						{
							DataGridViewColumn	c = dataGrid.Columns[i + ColumnStoryCount.Index + 1];
							int					w = i - Settings.Default.weekPast;

							c.Width = cw * mAspect / 10;
							if (w == -1)
								c.HeaderText = "先週";
							else if (w == 0)
								c.HeaderText = "今週";
							else if ( w == +1 )
								c.HeaderText = "来週";
							else
							{
								DateTime aDay		= DateTime.Now.AddDays( w * 7 );
								DateTime firstDay	= aDay.AddDays( -aDay.Day + 1 );				// 月の初めの日
								DateTime sunDay		= firstDay.AddDays( -(int)firstDay.DayOfWeek );	// 月の初めの日の直前の日曜日

								c.HeaderText = string.Format(
									"{0:0}月{1:0}"					,
									firstDay.Month					,
									( aDay - sunDay ).Days / 7 + 1	);
							}
						}
						break;
				}

				//----------------------------
				// 話のセルはソート禁止
				//----------------------------
				foreach ( DataGridViewColumn col in dataGrid.Columns )
				{
					col.SortMode = DataGridViewColumnSortMode.NotSortable;
				}


				//----------------------------
				// 行の拡縮を行う
				//----------------------------

				dataGrid.RowCount = server.Animes.Count;

				//----------------------------
				// 各行の内容を更新
				//----------------------------
				foreach (AnimeProgram prog in server.Animes)
				{
					rowIndex			= server.Animes.IndexOf( prog );
					DataGridViewRow row	= dataGrid.Rows[rowIndex];

					// セルのTagをクリア
					foreach( DataGridViewCell cell in row.Cells )
					{
						cell.Tag = null;
					}

					// セルにAnimeProgramオブジェクトを対応付ける
					row.Tag = prog;

					// セルにAnimeEpisodeオブジェクトを対応付ける
					foreach (AnimeEpisode episode in prog.Episodes)
					{
						int col;
						TimeSpan s;
						int hourOffset = Properties.Settings.Default.hoursPerDay - 24;

						switch (mViewMode)
						{
							case ViewMode.SeriesMode:
								//----------------------------
								// 連話表示モード
								//----------------------------
								col = prog.Episodes.IndexOf(episode) + ColumnStoryCount.Index + 1;
								break;

							case ViewMode.DayCalenderMode:
								//----------------------------
								// カレンダー日ごとモード
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
								// カレンダー週ごとモード
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
							row.Cells[col].Tag = episode;	// セルとレコードを対応付ける
						}
					}

					SetGridRowData(row, prog);
				}
			}

			UpdateStatusBar();

			//----------------------------
			// 未読ボタンメニュー項目更新
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
						// 列挙された項目をメニューに追加
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
			server.EnumAllEpisodes(callBack, null); // 未読を列挙する
			playUnreadButton.Enabled =	(0 < count)
									&&	!Settings.Default.disableUnread;

			Program.mTrayIcon.RefreshUnread();
		}

		//=========================================================================
		///	<summary>
		///		ドキュメント表示の更新を行う(データ変更時のみ)
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/11/22 新規作成</history>
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
		///		ステータスバーの内容を更新
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		void UpdateStatusBar()
		{
			AnimeServer server = AnimeServer.GetInstance();

			//------------------------------
			// 録画フォルダの情報を表示
			//------------------------------
			try
			{
				if ( Directory.Exists( Settings.Default.captureFolder ) )
				{
					string drive;

					drive = Settings.Default.captureFolder.Substring( 0, 2 );

					DriveInfo drvInfo = new DriveInfo( drive );

					recordDriveFreeSpaceLabel.Text =
						"録画先 " +
						Convert.ToString( (float)( drvInfo.TotalFreeSpace / 1024 / 1024 / 100 ) / 10 ) + "/"		+
						Convert.ToString( (float)( drvInfo.TotalSize	   / 1024 / 1024 / 100 ) / 10 ) + "GB 空き"	;
				} else
				{
					recordDriveFreeSpaceLabel.Text = "録画フォルダ: 見つかりません";
				}
			}
			catch(Exception ex)
			{
				recordDriveFreeSpaceLabel.Text = "録画先: 空き容量不明";
			}

			//------------------------------
			// 次の放送の情報を表示
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
			// データ更新ステータス表示
			//----------------------------------
			AnimeServer.MyStatus	status = server.GetStatus();

			if( status.updateSequenceBusy )
			{
				logStatusLabel.Text		= "データ更新中です(" + status.updateDetail + ")";
				logStatusLabel.IsLink	= false;
			}
			else if( !string.IsNullOrEmpty(status.resultLastUpdate) )
			{
				logStatusLabel.Text		= "データ更新の結果、エラーがありました";
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
		///		指定されたグリッド行の内容を更新
		///	</summary>
		/// <remarks>
		///		指定されたグリッド行rowに番組progの内容を出力する
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2008/05/02 スコープとメソッド名変更</history>
		//=========================================================================
		private void SetGridRowData(
			DataGridViewRow	row		,
			AnimeProgram	prog	)
		{
			AnimeEpisode				episode;
			string						comingOnAir = null;
			AnimeProgram.NextEpisode	nextState;

			//------------------------------
			// 次回放送日の情報を表示
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
				
				char []DayOfWeekList = {'日','月','火','水','木','金','土'};
				
				comingOnAir = string.Format(
								"{0:D1}/{1:D2}({2:S}) {3:D2}:{4:D2}",
								month,day							,
								DayOfWeekList[dayOfWeek]			,
								hour,episode.StartDateTime.Minute	);
			}
			else if( nextState == AnimeProgram.NextEpisode.NextUnknown )
			{
				comingOnAir = "なし";
			}
			else if( nextState == AnimeProgram.NextEpisode.EndProgram )
			{
				comingOnAir = "放送終了";
			}
			
			//---------------------------------
			// セルに表示するデータをまとめる
			//---------------------------------
			string[] newRowData =
			{
				prog.title		,											// 番組名
				comingOnAir		,											// 次回放送
				prog.tvStation	,											// テレビ局
				
				(prog.EncoderType != null)?
					prog.EncoderProfile.ToString() + "(" + prog.EncoderType.Name + ")"
					: "(なし)",												// エンコード情報
				string.Format("{0:D2}",prog.StoryCount),					// 全話数
			};

			for( int i = 0 ;i < newRowData.Length ;++i )
			{
				row.Cells[i].Value = newRowData[i];							// 上記のデータをセルに代入
			}

			//---------------------------------
			// セル(行)の高さを再計算
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
		///		セルの遅延描画要求を処理する
		///	</summary>
		/// <remarks>
		///		非同期に遅延描画することでUIのひっかかりを防止する。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void OnLazyDrawCell(object o,EventArgs ea)
		{
			if ( dataGrid != null && !dataGrid.IsDisposed)
			{
				Graphics g = dataGrid.CreateGraphics();

				foreach ( CellRefresh cr in cellRefreshRequest )
				{
					if ( cr.Row >= 0 && cr.Col > ColumnStoryCount.Index )		// 話ごとの列
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
		///		セルの内容を描画する
		///	</summary>
		/// <remarks>
		///		セルサイズのバッファに描画した後に描画先へ転送する。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		void DrawGridCell(
			Graphics					dest,	// 描画先
			int							row,	// 行
			int							col,	// 列
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
				Color	backColor;					// セルの背景色
				int		recordMonth, recordDay;		// 
				int		todayMonth, todayDay;		// 
				bool	toDay;						// 

				//-------------------
				// 前処理
				//-------------------

				prog = (AnimeProgram)dataGrid.Rows[ row ].Tag;							// 行に対応する番組

				if ( prog != null )
					episode = (AnimeEpisode)dataGrid.Rows[ row ].Cells[ col ].Tag;		// セルに対応する話

				if ( episode != null )
					storyNumber = episode.StoryNumber;

				// セル左部分が隠れている場合の対策
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

				if ( newRect.Width == 0 || newRect.Height == 0 )						// セルが表示領域内か？
					return;

				//-------------------
				// 白で塗りつぶす
				//-------------------
				g.FillRectangle( new SolidBrush( Color.White ), newRect );

				if ( episode != null && episode.HasPlan )
				{
					//---------------------------
					// 放送日が本日かどうか判定
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
				// セル色を決定する
				//-------------------------
				if ( storyNumber >= 1 )
				{
					if ( ( state & DataGridViewElementStates.Selected ) != 0 )
					{
						backColor = Color.FromKnownColor( KnownColor.ActiveCaption );	// 選択されたセル
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

						if( toDay )	// 本日放送
						{
							backColor = Color.FromArgb( 0xe0, 0xff, 0xe0 );
						}
						else		// その他
						{
							//	backColor = dataGrid.BackgroundColor;
							backColor = Color.FromKnownColor( KnownColor.Window );
						}
					}

					g.FillRectangle( new SolidBrush( backColor ), newRect );
					border = true;

					//--------------------------------
					// サムネイル表示モード
					//--------------------------------
					if ( thumbnailModeButton.Checked )
					{

						if ( episode.Parent.ThambnailImage != null )
						{
							//--------------------------------
							// サムネイルイメージ描画
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
							// サブタイトルを書く部分を白くする
							//-----------------------------------
							if ( Settings.Default.thumbnailSubtitle )
							{
								Rectangle rct = newRect;

								rct.Y = rct.Y + rct.Height - Settings.Default.thumbnailWhitebar;
								rct.Height = Settings.Default.thumbnailWhitebar;


								// if (!backColor.Equals(Color.FromKnownColor(KnownColor.Window)))	
								{
									g.FillRectangle( new SolidBrush( Color.FromArgb( 255, backColor ) ), rct );
//									g.FillRectangle(new SolidBrush(Color.FromArgb(220, backColor)), rct);	// 遅い?
								}

							}
						} else
						{
							//------------------------------------------
							// サムネイルイメージがない場合は塗りつぶす
							//------------------------------------------
							g.FillRectangle( new SolidBrush( backColor ), newRect );
						}
						// セルの境界線
//							g.DrawRectangle(new Pen(Color.FromKnownColor(KnownColor.InactiveBorder)), newRect);
					}

				} else
				{
					//-------------
					// 空のセル
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
					// アイコン描画
					//-----------------------

					if ( storyNumber >= 1 )
					{
						//--------------------------------
						// サムネイルがある話は表示しない
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

						// 重複しているEpisodeの表示
						if ( episode.IsDoubleBooking() )
						{
							// 重複アイコン表示
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
					// 話数を描画
					//----------------------
					int		tx, ty;		// 文字を描画する座標
					Color	tc;			// 文字のカラー

					tx = newRect.X;
					if ( thumbnailModeButton.Checked )
					{
						// サムネイルモード
						ty = newRect.Bottom - mBigFont.Height;
					} else
					{
						// 非サムネイルモード
						ty = newRect.Y;
					}

					text = string.Format( "{0:0}", episode.StoryNumber );

					g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;	// アンチエイリアス有効

					if ( storyNumber <= prog.Episodes.Count
						&& episodeState != AnimeEpisode.State.Planned		// <MOD> 2009/12/28
						&& episodeState != AnimeEpisode.State.Scheduling
						&& episodeState != AnimeEpisode.State.Changed
						&& episodeState != AnimeEpisode.State.Notfound
						&& episodeState != AnimeEpisode.State.Undecided
						&& episodeState != AnimeEpisode.State.LostSchedule )
					{

						//
						// 文字の本体
						//
						if ( episodeState == AnimeEpisode.State.Stored )
							tc = Color.Green;	// 保存済みの色
						else
							tc = Color.Blue;	// 通常の色

//							g.DrawString(text, bigFont, new SolidBrush(tc), new PointF(tx, ty)); // 文字を描画

					} else
					{
						tc = Color.Gray;
					}

					g.DrawString( text, mBigFont, new SolidBrush( tc ), new PointF( tx, ty ) ); // 文字を描画

					//--------------------------
					// サブタイトルの描画
					//--------------------------
					if ( thumbnailModeButton.Checked && Settings.Default.thumbnailSubtitle )		// サムネイル表示モードのみ
					{

						bool cs = ( state & DataGridViewElementStates.Selected ) > 0;			// セル選択の有無によって色を変える

						g.DrawString(
							episode.mSubTitle,
							new Font( "MS UI Gothic", 9 ),
							new SolidBrush( cs ? Color.White : Color.Black ),
							new PointF( newRect.X + 32, newRect.Bottom - 9 - 5 ) );
					}
				}

				//----------------------------
				// バッファから画面に転送
				//----------------------------
				dest.DrawImage( mCellBuffer, cellX, cellY,newRect,GraphicsUnit.Pixel );

				//------------------
				// 境界線を描画
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
		///		セルのオーナー描画
		///	</summary>
		/// <remarks>
		///		直接描画せず、遅延描画キューにキューイングして後で描画
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
			// セルの遅延描画要求をまとめる
			//-------------------------------
			cellRefreshGraphics = g;

			CellRefresh cr = new CellRefresh();

			cr.Row		= e.RowIndex;
			cr.Col		= e.ColumnIndex;
			cr.State	= e.State;
//			cr.cellSize	= e.CellBounds.Size;

			//----------------------------------------------
			// 同じセルの描画要求がある場合は削除しておく
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
			// 描画要求キューに追加
			//---------------------------
			cellRefreshRequest.Add( cr );



			if ( (cr.Row >= 0) && (cr.Col > ColumnStoryCount.Index) )		// 話ごとの列
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
		///		[新しい番組]メニューがクリックされた時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void NewAnimeMenu_Click(object sender, EventArgs e)
		{
			AnimeProgram	newAnime = new AnimeProgram( AnimeServer.GetInstance() );

			newAnime.StoryCount	= 0;

			//-------------------------
			// 番組ダイアログを開く
			//-------------------------

			AnimeDialog dlg = new AnimeDialog();

			if (dlg.ShowDialog(ref newAnime ) == DialogResult.OK)
			{
				//------------------------------
				// 新しい番組をリストに追加
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
		///		[番組を削除]メニューがクリックされた時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void DeleteAnimeMenu_Click(object sender, EventArgs e)
		{
			AnimeServer server = AnimeServer.GetInstance();

			lock ( server )
			{
				List<AnimeProgram> delList = new List<AnimeProgram>();	// 削除リスト

				//-----------------------------------
				// 削除対象の番組をリストアップ
				//-----------------------------------
				var progs = GridSelectPrograms;

				foreach(AnimeProgram prog in progs)
				{
					DialogResult res;

					//------------------------------
					// 削除確認メッセージを表示
					//------------------------------
					res = MessageBox.Show(
						this														,
						prog.title + " をリストから削除してよろしいですか？\n"		+
						"・可能であれば予約も削除します。\n"						+ 
						"・録画ファイルはそのまま残ります。"						,
						"確認"														,
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
					// 実際に番組をリストから削除
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
		///		番組のプロパティツールボタンがクリックされた時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void propertyButton_Click(object sender, EventArgs e)
		{
			programPropertyMenu_Click(sender, e);
		}

		//=========================================================================
		///	<summary>
		///		[番組のプロパティ]メニューがクリックされた時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		[最新情報に更新]メニューが追加された時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void RefreshMenu_Clicked(object sender, EventArgs e)
		{
			AnimeServer.GetInstance().BeginUpdate(0);
			RefreshContent();
		}

		//=========================================================================
		///	<summary>
		///		[完全データ更新]メニューが追加された時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void ForceRefreshMenu_Click(object sender, EventArgs e)
		{
			AnimeServer.GetInstance().BeginUpdate(updateOption.Force);
			RefreshContent();
		}
		
		//=========================================================================
		///	<summary>
		///		グリッドの列がドラッグされた時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void dataGrid_MouseMove(object sender, MouseEventArgs e)
		{
			// ドラッグで移動する機能

			//if (e.ColumnIndex < 0)
			if ((e.Button & MouseButtons.Left) > 0 && mDraggingRow==false)
			{
				DataGridView.HitTestInfo hit;

				// クリックした点のセルを調べる
				hit		= dataGrid.HitTest(e.X, e.Y);

				// クリックしたセルが左側の灰色セルか
				if (hit != DataGridView.HitTestInfo.Nowhere &&
					hit.ColumnIndex == -1 && hit.RowIndex >= 0)
				{
					DataGridViewRow row;

					row				= dataGrid.Rows[hit.RowIndex];			// クリックされた行
					row.Selected	= true;									// 行選択
//					Console.WriteLine("行ドラッグ({0:0})",((AnimeProgram)row.Tag).title);

					dataGrid.Capture = true;								// データグリッドがマウスをキャプチャする
					dataGrid.Cursor = Cursors.Hand;

					mDraggingRow		= true;
					mDraggingItem	= row;

					return;
				}
			}

		}

		//=========================================================================
		///	<summary>
		///		グリッドの列がドラッグ完了した時の処理
		///	</summary>
		/// <remarks>
		///		ドラッグで移動する機能(v1.6.13)
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void dataGrid_MouseUp(object sender, MouseEventArgs e)
		{
			//----------------------------
			// 行ドラッグ状態を解除
			//----------------------------
			if (mDraggingRow)
			{
				DataGridView.HitTestInfo hit;

				dataGrid.Cursor		= Cursors.Default;

				hit = dataGrid.HitTest(e.X, e.Y);					// ボタンを上げた瞬間のセルを得る

				if (hit != DataGridView.HitTestInfo.Nowhere)		// ドロップされた点がグリッド上か？
				{
//					Console.WriteLine("{0:0},{1:0}", e.X, e.Y);
//					Console.WriteLine("{0:0},{1:0}", h.ColumnIndex, h.RowIndex);

					//-------------------------------------
					// ドロップすべきリスト上の位置を計算
					//-------------------------------------
					int y,offset;

					if (hit.RowIndex < 0)
					{
						y = 0;												// 固定行ならトップに挿入
					}
					else
					{
						int cy	= e.Y - hit.RowY;							// セル内のYオフセット
						int h	= dataGrid.Rows[hit.RowIndex].Height;		// セルの高さ
						offset	= 2 * cy / h;								// 行の真中より上か下か
						y		= hit.RowIndex + offset;					// 挿入する行
					}

					if (dataGrid.RowCount <= y)
						y = dataGrid.RowCount;

					if (0 <= y && y <= dataGrid.RowCount)
					{
						//--------------------------------
						// 番組リストの入れ替えを行う
						//--------------------------------
//						Console.WriteLine( "ドラッグ先{0:0}",y );

						DataGridViewRow		row;
						AnimeServer			server;
						AnimeProgram		p;

						row		= (DataGridViewRow)mDraggingItem;			// ドラッグ元の行
						row.Selected	= false;							// 行選択解除
						server	= AnimeServer.GetInstance();
						p		= (AnimeProgram)row.Tag;					// ドラッグ対象の番組

						if (dataGrid.Rows.IndexOf(row) < y)
							y = y - 1;										// ドラッグ元の行が上ならドラッグ先を-1

						server.DeleteAnime(p);
						server.AddAnime( p, y );

						RefreshContent();
					}
				}

				mDraggingRow			= false;
				mDraggingItem		= null;
				dataGrid.Capture	= false;								// マウスキャプチャを解除
			}

		}

		//=========================================================================
		///	<summary>
		///		グリッドのセルがクリックされた時、エピソード情報を表示
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void dataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || e.ColumnIndex < 0)
				return;

			RefreshSelectedEpisodeInfo();
		}

		//=========================================================================
		///	<summary>
		///		グリッドで選択されているエピソード情報を表示
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/31 dataGrid_CellClickから分離</history>
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
					"(放送終了)録画ファイルなし",
					"放送プラン確定"			,
					"録画済"					,
					"再エンコード済"			,
					"保存済み"					,
					"録画予約済"				,
					"再エンコード処理中"		,
					"放送未定"					,
					"放送時間変更"				,
					"視聴しない"				,
					"録画予約に失敗"			,
					"処理中"					,
				};

				titleLabel.Text			= prog.title +
										  string.Format(
												" 第{0:0}話 {1:0}"		,
												episode.StoryNumber		,
												episode.mSubTitle		);

				filePathLabel.Text		= episode.FilePath;
				RecordStateLabel.Text	= RecordStateDescription[(int)episode.CurrentState];

				if( !episode.HasPlan )
					dateTimeLabel.Text	= "未定";
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
						dateTimeLabel.Text += " (チューナ不足のため録画不能)";
					else
						dateTimeLabel.Text += " (時間帯重複あり注意)";

				if( episode.PlanError )
					dateTimeLabel.Text += " ※放送データ異常あり";
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
		///		グリッドのセルが右クリックされた時、メニューを表示
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2010/05/03 マルチセル選択対応</history>
		//========================================================================
		private void dataGrid_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.RowIndex < 0 || e.ColumnIndex < 0)
				return;
			
			AnimeServer doc = AnimeServer.GetInstance();

			if((e.Button & MouseButtons.Right) > 0)
			{
				// 右クリックしたセルを選択
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
					// 右クリックメニュー項目の有効/無効を設定
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

						// キャンセルできるのは録画開始するまで
						enableCancel	&= ep.IsReserved && !ep.IsStartedOnair;

						enableUnread	&= !isMulti;

						enableProp		&= !isMulti;
					}

					//--------------------------------
					// 拡張ツール項目
					//--------------------------------
					if (Settings.Default.externalTools != null)
					{
						try
						{
							ToolStripItemCollection menuItems = contextMenuStrip.Items;
							List<ToolStripItem> delItems = new List<ToolStripItem>();

							// 元の項目を削除
							for (int i = menuItems.IndexOf(extToolsGroupSeparator) + 1;
								 i <= menuItems.IndexOf(encodeGroupSeparator) - 1;
								++i)
							{
								delItems.Add(menuItems[i]);
							}
							foreach (ToolStripItem item in delItems)
								menuItems.Remove(item);

							// ツール項目を追加
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
					// カーソル位置にメニュー表示
					//--------------------------------

					Point pt = PointToClient(Cursor.Position);
					
					contextMenuStrip.Show(this, pt);
				}
				
			}
			
		}

		//=========================================================================
		///	<summary>
		///		グリッドのセルがダブルクリックされた時の処理
		///	</summary>
		/// <remarks>
		///		ダブルクリックで再生/予約
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
			// エピソードが再生可能なら再生
			//---------------------------------
			if (ep.IsPlayable)
			{
				// 重い処理をしている間にマウスが動くとセルが選択されてしまうため、ハンドラ外に投げる
				KernelAPI.Window.PostMessage(
					this.Handle,
					KernelAPI.Window.WM_USER,
					IntPtr.Zero,
					IntPtr.Zero );
				return;
			}

			//-------------------------------
			// エピソードの予約を入れる
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
		///		ウィンドウプロシージャ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/05/07 新規作成</history>
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
		///		[終了]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void ExitMenu_Click(object sender, EventArgs e)
		{
			mExitFlag = true;
			this.Close();
		}
		
		//=========================================================================
		///	<summary>
		///		セルの右クリック[再生]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
				MessageBox.Show(this, ex.Message, "エラー",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//=========================================================================
		///	<summary>
		///		指定された動画ファイルを再生する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		internal void PlayMovie(string filePath)
		{
			FormWindowState oldState;
			Process process;

			//--------------------------------
			// 関連付けられたプレイヤー起動
			//--------------------------------
			process = Process.Start(filePath);

			if (Settings.Default.minimizeAtPlayer)
			{
				//-----------------------------
				// 再生中は最小化してブロック
				//-----------------------------

				oldState = this.WindowState;
				this.WindowState = FormWindowState.Minimized;
				dataGrid.Enabled	= false;
				menuStrip.Enabled	= false;

				for (; !process.WaitForExit(500); )
				{
					// 必要ならDoEvents
				}

				dataGrid.Enabled	= true;
				menuStrip.Enabled	= true;
				this.WindowState	= oldState;
			}

		}


		//=========================================================================
		///	<summary>
		///		[再エンコード]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void encodeMenu_Click(object sender, EventArgs e)
		{
			var episodes = GridSelectEpisodes;

			episodes.ForEach(ep =>  AnimeServer.GetInstance().AddEncodeJob(ep));
		}

		//=========================================================================
		///	<summary>
		///		[保存先に転送]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void storeMenu_Click(object sender, EventArgs e)
		{
			var episodes = GridSelectEpisodes;

			foreach(var ep in episodes)
			{
				if( ep.HasFile && !ep.IsEncoded && (ep.Parent.EncoderType != null) )
				{
					//---------------------------------------------------
					// 再エンコードせずに転送する際の確認メッセージ表示
					//---------------------------------------------------
					DialogResult dlgResult;

					dlgResult = MessageBox.Show(
						ep.ToString() + System.Environment.NewLine +
						"再エンコードしていませんが、そのまま保存フォルダへ移動しますか？",
						"確認",
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
		///		フォームが閉じられる間際の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (	e.CloseReason == CloseReason.UserClosing	&&
					Settings.Default.inTaskTray					&&
					mExitFlag == false							)
			{
				//-----------------------------------------------------
				// ユーザーが[X]ボタンを押したらタスクトレイに入れる
				//-----------------------------------------------------
				this.Hide();
				e.Cancel = true;

				Program.ShowTrayIcon();
			}
			else
			{
				//---------------------------
				// 実際にアプリを終了
				//---------------------------

				// 終了してよいか問い合わせ
				if( !Program.QueryClose() )
				{
					e.Cancel = true;
					mExitFlag = false;
				}
			}


			if(e.CloseReason==CloseReason.TaskManagerClosing)
			    Logger.Output( "タスクマネージャによる終了" );

			if( e.Cancel == false )
			{
				Application.Exit();
			}
		}

		//=========================================================================
		///	<summary>
		///		[バージョン情報]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void AboutMenu_Click(object sender, EventArgs e)
		{
			AboutBox aboutBox = new AboutBox();
			aboutBox.ShowDialog();
		}

		//=========================================================================
		///	<summary>
		///		右クリック[予約]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
							"録画チューナが足りないため、この番組の予約を保留しています。\n" +
							"強制的に予約しますか？", 
							"確認",
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
							"予約の失敗",
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
					"エラー",
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
		///		セルの右クリック[プロパティ]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		フォームが実際に閉じられた後の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			// 表示モードに関する設定を保存

			Settings.Default.viewMode = (int)mViewMode;
			Settings.Default.thumbnailMode = thumbnailModeButton.Checked;

			//----------------------------
			// ウィンドウサイズを保存
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

			// ペイン分割位置を保存
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
		///		未読ツールボタンがクリックされた時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//=========================================================================
		///	<summary>
		///		未読メニュー項目がクリックされた時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
				MessageBox.Show(this, ex.Message, "エラー",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

		//=========================================================================
		///	<summary>
		///		未読ツールボタンがクリックされた時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void playUnreadButton_ButtonClick(object sender, EventArgs e)
		{
			//
			// 一番上のアイテムをクリックされたことにする
			//
			ToolStripItemClickedEventArgs eventArgs
				= new ToolStripItemClickedEventArgs(playUnreadButton.DropDownItems[0]);

			playUnreadButton_DropDownItemClicked( playUnreadButton, eventArgs );

		}

		//=========================================================================
		///	<summary>
		///		最終保存先に転送ツールボタンの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void StoreAllButton_Click(object sender, EventArgs e)
		{
			StoreAllMenu_Click(sender, e);
		}

		//=========================================================================
		///	<summary>
		///		[最終保存先に転送]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void StoreAllMenu_Click(object sender, EventArgs e)
		{
			TransferDialog dlg = new TransferDialog();

			dlg.ShowDialog();
		}

		//=========================================================================
		///	<summary>
		///		[公式サイト]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void helpMenu_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process process;

			process = System.Diagnostics.Process.Start( Settings.Default.helpUrl );

		}

		//=========================================================================
		///	<summary>
		///		再エンコードメニューのエンコード中リストを更新
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2008/11/16 エンコードを複数同時実行前提にした。</history>
		//========================================================================
		private void PowerMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			ToolStripItemCollection c;
			List<EncodeJob> jobs;
			AnimeServer server = AnimeServer.GetInstance();

			AutoShutdownMenu.Checked = server.AutoShutdown;		// 自動シャットダウンフラグ

			//---------------------------------
			// エンコード待ちジョブ一覧を更新
			//---------------------------------

			c = BatchListMenuItem.DropDown.Items;

			// エンコード待ちメニュー項目削除
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
			// エンコード中のジョブ一覧を更新
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
		///		[自動的にシャットダウン]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		[エンコード中断]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void JobsCancelMenu_Click(object sender, EventArgs e)
		{
			AnimeServer.GetInstance().AutoShutdown = false;					// 自動シャットダウン無効
			AnimeServer.GetInstance().CancelJobs();
		}

		//=========================================================================
		///	<summary>
		///		[全て再エンコード]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void BatchEncodeAllMenu_Click(object sender, EventArgs e)
		{
			AnimeServer.GetInstance().BatchEncodeAll();
		}

		//=========================================================================
		///	<summary>
		///		[ログ表示]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		話数ビューモードに切り替えるメニュー
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void seriesModeMenu_Click(object sender, EventArgs e)
		{
			mViewMode = ViewMode.SeriesMode;
			RefreshContent();
		}

		//=========================================================================
		///	<summary>
		///		カレンダービューモードに切り替えるメニュー
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		カレンダービューモードのオプション画面を表示するメニュー
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		サムネイルモード切替ボタンの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void thumbnailModeButton_Click(object sender, EventArgs e)
		{
			RefreshContent();
		}

		//=========================================================================
		///	<summary>
		///		セルの状態が変更された時、フォーカス等を即時に描画
		///	</summary>
		/// <remarks>
		///		遅延描画すると残像になってしまうため。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		ステータスバーの録画先TIPSをクリックしたとき、録画フォルダを開く
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		ステータスバーの保存先TIPSをクリックしたとき、録画フォルダを開く
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		[番組ソート]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		全エピソードの未読フラグを解除するメニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/10/25 新規作成</history>
		//========================================================================
		private void OnReleaseUnreadMenu_Click( object sender, EventArgs e )
		{
			if( MessageBox.Show(
				"全ての未読フラグを解除してよろしいですか？"	,
				"確認"										,
				MessageBoxButtons.OKCancel					,
				MessageBoxIcon.Question						) == DialogResult.OK )
			{
				AnimeServer server = AnimeServer.GetInstance();
				AnimeProgram.EpisodeList list;

				list = server.QueryEpisode( null );					// 全番組の全エピソードを列挙する

				foreach( AnimeEpisode episode in list )
				{
					episode.Unread = false;
				}

				RefreshContent();
			}
		}

		//=========================================================================
		///	<summary>
		///		[シャットダウン]メニュー（デバッグ用）
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void debugShutdownMenu_Click( object sender, EventArgs e )
		{
			Program.TryShutdown();
		}

		//=========================================================================
		///	<summary>
		///		[保存ファイル名にリネーム]メニュー
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//========================================================================
		private void renameFileMenu_Click( object sender, EventArgs e )
		{
			string			newName;

			var				episodes	= GridSelectEpisodes;

			foreach(var ep in episodes)
			{
				if( ep.RenameFile( out newName ) )
				{
					MessageBox.Show(	"ファイル名をリネームしました。\r\n" + newName,
										"成功"						,
										MessageBoxButtons.OK		,
										MessageBoxIcon.Information	);
				}
				else
				{
					MessageBox.Show(	"ファイル名のリネームに失敗しました。",
										null					,
										MessageBoxButtons.OK	,
										MessageBoxIcon.Warning	);
				}
			}	
		}

		//=========================================================================
		///	<summary>
		///		[予約キャンセル]メニュー
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
//					MessageBox.Show(	"予約をキャンセルしました。",
//										"成功"						,
//										MessageBoxButtons.OK		,
//										MessageBoxIcon.Information	);
				}
				else
				{
					if( MessageBox.Show(
						ep.ToString() + System.Environment.NewLine +
						"予約をキャンセルできませんでした。\r\n"				+
						"(録画ソフトから予約をキャンセルして下さい)\r\n\r\n"	+
						"エピソードの状態を強制的に「未予約」にしますか？"		,
						null									,
						MessageBoxButtons.YesNo					,
						MessageBoxIcon.Warning					,
						MessageBoxDefaultButton.Button2　		) == DialogResult.Yes )
					{
						ep.IsReserved = false;
					}
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		外部ツールメニュー項目のクリック処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
				MessageBox.Show(	"外部ツールの起動に失敗しました。" + System.Environment.NewLine +
									ex.Message );
			}
		}

		//=========================================================================
		///	<summary>
		///		表示リフレッシュタイマ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/11/22 新規作成</history>
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
		///		ログ表示更新
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/28 新規作成</history>
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
		///		データ更新結果ラベルのクリック時処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/11/23 新規作成</history>
		//========================================================================
		private void logStatusLabel_Click(object sender, EventArgs e)
		{
			AnimeServer.MyStatus newStatus = AnimeServer.GetInstance().GetStatus();

			if ( !string.IsNullOrEmpty( newStatus.resultLastUpdate) )
			{
				MessageBox.Show(
					newStatus.resultLastUpdate,
					"データ更新結果",
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
		///		コントロールの状態更新
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/29 新規作成</history>
		//========================================================================
		void RefreshControl()
		{
			logButton.Checked = Settings.Default.showLogPane;

			bool singleSelected = (1 == GridSelectPrograms.Count);

			// ログ表示ペイン表示
			if( Settings.Default.showLogPane )
				viewSplitContainer.Panel2Collapsed = false;
			else
				viewSplitContainer.Panel2Collapsed = true;

			// 禁則状態の更新
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

// <ADD> 2010/04/17 デバッグ用 ->
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
		///		グリッドセル選択時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/05/03 新規作成</history>
		//========================================================================
		private void dataGrid_SelectionChanged(object sender, EventArgs e)
		{
			RefreshSelectedEpisodeInfo();
		}

		//=========================================================================
		///	<summary>
		///		グリッドで選択されているエピソード
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/05/03 新規作成</history>
		//========================================================================
		private List<AnimeEpisode> GridSelectEpisodes
		{
			get
			{
				var episodes = new List<AnimeEpisode>();

				try
				{
					// 選択されているエピソードを取得
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
		///		グリッドで選択されている番組
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/05/03 新規作成</history>
		//========================================================================
		private List<AnimeProgram> GridSelectPrograms
		{
			get
			{
				var progs = new List<AnimeProgram>();

				try
				{
					// 選択されているプログラムを取得
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

