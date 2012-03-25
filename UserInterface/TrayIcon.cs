//=========================================================================
///	<summary>
///		mAgicAnimeトレイアイコン モジュール
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成</history>
/// <history>2010/05/31 Subversionで管理するため古いコメント削除</history>
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
	///		mAgicAnimeトレイアイコン クラス
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	//=========================================================================
	internal class TrayIcon
	{
		const	int		maxUnreadCount	= 10;	// バルーンに表示する最大未読数

		//-----------------
		// メンバ変数
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
		///		トレイアイコンの初期化
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal void Initialize()
		{
			//---------------------------
			// トレイアイコンの作成
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
			// トレイメニューの作成
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
			this.mTrayRefreshMenu.Text = "最新情報に更新(&R)";
			this.mTrayRefreshMenu.Click += new System.EventHandler( this.RefreshMenu_Clicked );
			this.mTrayRefreshMenu.DropDownOpening += new System.EventHandler( this.RefreshMenu_DropDownOpening );
			// 
			// ｰToolStripMenuItem
			// 
			this.mToolStripMenuItem.Name = "ｰToolStripMenuItem";
			this.mToolStripMenuItem.Size = new System.Drawing.Size( 164, 6 );
			// 
			// openWindowToolMenu
			// 
			this.mOpenWindowToolMenu.Name = "openWindowToolMenu";
			this.mOpenWindowToolMenu.Size = new System.Drawing.Size( 167, 22 );
			this.mOpenWindowToolMenu.Text = "ウィンドウを開く(&O)";
			this.mOpenWindowToolMenu.Click += new System.EventHandler( this.openWindowToolMenu_Clicked );
			// 
			// trayExitMenu
			// 
			this.mTrayExitMenu.Name = "trayExitMenu";
			this.mTrayExitMenu.Size = new System.Drawing.Size( 167, 22 );
			this.mTrayExitMenu.Text = "終了(&X)";
			this.mTrayExitMenu.Click += new System.EventHandler( this.ExitMenu_Click );

			this.mTrayMenu.ResumeLayout();

			//---------------------------
			// その他の初期化処理
			//---------------------------
			AnimeServer server = AnimeServer.GetInstance();

			server.mPopupSoonBallon += PopupSoonBalloon;

			//----------------
			// 更新タイマ
			//----------------
			mRefreshTimer			= new System.Windows.Forms.Timer();
			mRefreshTimer.Interval	= 100;
			mRefreshTimer.Enabled	= true;
			mRefreshTimer.Tick += delegate (object sender, EventArgs args)
			{
				AnimeServer				animServer = AnimeServer.GetInstance();
				AnimeServer.MyStatus	stat	   = animServer.GetStatus();

				// データ更新中はトレイアイコンを変更
				mNotifyIcon.Icon	= stat.updateSequenceBusy
								? Resources.mAigcAnimeLogoBusy
								: Resources.mAigcAnimeLogo;
			};

			RefreshUnread();
		}

		//=========================================================================
		///	<summary>
		///		トレイアイコンのクリーンアップ処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/10/01 新規作成</history>
		//=========================================================================
		internal void Cleanup()
		{
			AnimeServer server = AnimeServer.GetInstance();

			server.mPopupSoonBallon -= PopupSoonBalloon;
		}


		//=========================================================================
		///	<summary>
		///		トレイメニューが開かれた時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		void trayMenu_Opening( object sender, System.ComponentModel.CancelEventArgs e )
		{
//			e.Cancel = true;
//			OnUpdateDocument();
		}


		//=========================================================================
		///	<summary>
		///		タスクアイコンをダブルクリックされた時、ウィンドウを表示する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			// タスクトレイアイコンから復帰
			Program.ShowMainForm();
		}


		//=========================================================================
		///	<summary>
		///		[最新情報に更新]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		[ウィンドウを開く]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void openWindowToolMenu_Clicked( object sender, EventArgs e )
		{
			Program.ShowMainForm();
		}

		//=========================================================================
		///	<summary>
		///		[終了]メニューの処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void ExitMenu_Click(object sender, EventArgs e)
		{
			// 終了してよいか、各所に問い合わせる
			if( Program.QueryClose() )
			{
				Application.Exit();
			}
		}

		//=========================================================================
		///	<summary>
		///		未読メニュー項目がクリックされた時の処理
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
									"エラー"			,
									MessageBoxButtons.OK,
									MessageBoxIcon.Error);
			}
		}

		//=========================================================================
		///	<summary>
		///		未読リストメニューを更新する
		///	</summary>
		/// <remarks>
		///		データ変更イベントをトリガとして実行される。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2008/05/03 メソッド名を変更</history>
		//=========================================================================
		internal void RefreshUnread()
		{
			lock( this )
			{
				//----------------------------
				// メニューの未読一覧をクリア
				//----------------------------

				int bottom = mTrayMenu.Items.IndexOf( mTrayMenuUnreadSeparetor );

				for ( int i = 0; i < bottom; ++i )
				{
					mTrayMenu.Items.RemoveAt( 0 );
				}

				if( !Settings.Default.disableUnread )
				{
					//--------------------------
					// 未読一覧をメニューに追加
					//--------------------------
					uint count = 0;

					AnimeProgram.EnumRecordCallBack callBack
						= delegate(AnimeEpisode r, object param)
					{
						if (r.Unread && r.IsPlayable)
						{
							ToolStripItem item;

							if ( count < maxUnreadCount )	// 表示数を制限
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

					AnimeServer.GetInstance().EnumAllEpisodes(callBack, null);	// 未読を列挙する

					if ( count == 0 )
					{
						//---------------------------------
						// トレイメニューに(未読なし)追加
						//---------------------------------
						ToolStripMenuItem item;
						item			= new ToolStripMenuItem( "(未読なし)" );
						item.Enabled	= false;
						mTrayMenu.Items.Insert( 0, item );
					}
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		未読バルーンをポップアップする
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2007/12/23 MainFormから移動</history>
		//=========================================================================
		internal void PopupUnreadBalloon()
		{
			List<AnimeEpisode> unreads = new List<AnimeEpisode>();

			// 未読を列挙
			unreads = AnimeServer.GetInstance().QueryEpisode(
						ep => ep.Unread && ep.IsPlayable );

			//------------------------------
			// バルーンに未読状態を表示
			//------------------------------
			{
				if( (0 < unreads.Count) && mNotifyIcon.Visible )
				{
				    string t = string.Format("未読のアニメが {0:0}本 あります。\n", unreads.Count);

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
		///		「まもなく放送時間」バルーンをポップアップ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/10/01 新規作成</history>
		//=========================================================================
		internal void PopupSoonBalloon(
			AnimeEpisode	episode )	// [i] エピソード
		{
			if( mNotifyIcon.Visible)
			{
				string  t;

				if( !episode.HasPlan )
					return;

				DateTimeHelper	dateTime = new DateTimeHelper(
					episode.StartDateTime				,
					Settings.Default.hoursPerDay -24	);

				t = "まもなく放送時間です\n\n"
					+ dateTime.ToShortTimeString()
					+ "から "
					+ episode.ToString();
				
			    mNotifyIcon.BalloonTipText = t;
			    mNotifyIcon.ShowBalloonTip(3000);
			}

		}

	}
}

