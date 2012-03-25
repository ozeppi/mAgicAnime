//=========================================================================
///	<summary>
///		メインウィンドウに指定するダミーフォーム
///	</summary>
/// <remarks>
///		AppicationContextのMainFormがnullだとWM_QUITメッセージが
///		到来したときアプリケーションを終了する方法がないため。
/// </remarks>
/// <history>2010/05/31 Subversionで管理するため古いコメント削除</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using magicAnime.Properties;

namespace magicAnime.UserInterface
{
	public partial class DummyForm : Form
	{
		public DummyForm()
		{
			InitializeComponent();
		}

		//-----------------------------------------------------------
		// ウィンドウを非表示にする
		//-----------------------------------------------------------
        protected override CreateParams CreateParams
        {
            get
            {
                const System.UInt32		WS_EX_TOOLWINDOW	= 0x80;
//			    const System.UInt32		WS_VISIBLE			= 0x10000000;
                const System.UInt32		WS_POPUP			= 0x80000000;

                CreateParams	prms = base.CreateParams;
                prms.Style		= unchecked( (int)WS_POPUP );
                prms.ExStyle	= (int)WS_EX_TOOLWINDOW;
                prms.Width		= 0;
                prms.Height		= 0;
                return prms;
            }
        }

		//=========================================================================
		///	<summary>
		///		アプリケーションが外部要因によって終了されるときの処理
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/XX/XX 新規作成</history>
		//=========================================================================
		private void DummyForm_FormClosed( object sender, FormClosedEventArgs e )
		{
			// ログに記録
			if(e.CloseReason==CloseReason.WindowsShutDown)
				Logger.Output("システムシャットダウン");

			string	log = string.Format( "終了要因: CloseReason({0})", e.CloseReason.ToString() );

			Logger.Output( log );
//			Application.Exit();
		}

        protected override void  WndProc(ref Message m)
        {
			// サスペンドや休止から復帰したとき、バルーン表示
            if( m.Msg == KernelAPI.Window.WM_POWERBROADCAST )
            {
				if( (uint)m.WParam == KernelAPI.Window.PBT_APMRESUMESUSPEND )
				{
					Logger.Output("休止またはサスペンドから復帰");

					if(	!Settings.Default.disableUnread
					&&	Settings.Default.notifyUnreadBalloon )
					{
						Program.mTrayIcon.PopupUnreadBalloon();
					}
				}
            }

            base.WndProc(ref m);
        }

	}
}
