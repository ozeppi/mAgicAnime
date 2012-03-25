//=========================================================================
///	<summary>
///		���C���E�B���h�E�Ɏw�肷��_�~�[�t�H�[��
///	</summary>
/// <remarks>
///		AppicationContext��MainForm��null����WM_QUIT���b�Z�[�W��
///		���������Ƃ��A�v���P�[�V�������I��������@���Ȃ����߁B
/// </remarks>
/// <history>2010/05/31 Subversion�ŊǗ����邽�ߌÂ��R�����g�폜</history>
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
		// �E�B���h�E���\���ɂ���
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
		///		�A�v���P�[�V�������O���v���ɂ���ďI�������Ƃ��̏���
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void DummyForm_FormClosed( object sender, FormClosedEventArgs e )
		{
			// ���O�ɋL�^
			if(e.CloseReason==CloseReason.WindowsShutDown)
				Logger.Output("�V�X�e���V���b�g�_�E��");

			string	log = string.Format( "�I���v��: CloseReason({0})", e.CloseReason.ToString() );

			Logger.Output( log );
//			Application.Exit();
		}

        protected override void  WndProc(ref Message m)
        {
			// �T�X�y���h��x�~���畜�A�����Ƃ��A�o���[���\��
            if( m.Msg == KernelAPI.Window.WM_POWERBROADCAST )
            {
				if( (uint)m.WParam == KernelAPI.Window.PBT_APMRESUMESUSPEND )
				{
					Logger.Output("�x�~�܂��̓T�X�y���h���畜�A");

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
