// <DEL> 2009/11/23 ->
////=========================================================================
/////	<summary>
/////		�f�[�^�X�V�v���O���X���
/////	</summary>
///// <remarks>
///// </remarks>
///// <history>2006/XX/XX �V�K�쐬</history>
////=========================================================================
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Text;
//using System.Threading;
//using System.Windows.Forms;

//namespace magicAnime
//{
//    //=========================================================================
//    ///	<summary>
//    ///		�f�[�^�X�V�v���O���X��ʃN���X
//    ///	</summary>
//    /// <remarks>
//    /// </remarks>
//    /// <history>2006/XX/XX �V�K�쐬</history>
//    //=========================================================================
//    partial class UpdateDialog : Form
//    {
//        //----------------------
//        // �����o�ϐ�
//        //----------------------
//// <DEL> 2008/10/21 ->
////		private BackgroundProc	backgroundProc;
////		private IAsyncResult	procResult;
//// <DEL> 2008/10/21 <-
//        private modifiedFlags	modifiedFlags;
//// <ADD> 2008/10/21 ->
//        private ManualResetEvent endSequenceFlag;		// �f�[�^�X�V�V�[�P���X�����t���O
//// <ADD> 2008/10/21 <-
		
//// <DEL> 2008/10/21 ->
////		private string			errorMessage;
////		private bool			errorOccurred = false;
//// <DEL> 2008/10/21 <-

//        private delegate void BackgroundProc();

//        //=========================================================================
//        ///	<summary>
//        ///		�R���X�g���N�^
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX �V�K�쐬</history>
//        //=========================================================================
//        public UpdateDialog()
//        {
//            InitializeComponent();
//        }

//        //=========================================================================
//        ///	<summary>
//        ///		�f�[�^�X�V�v���O���X�_�C�A���O��\������(����)
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX �V�K�쐬</history>
//        /// <history>2006/11/26 ��\���ł���悤�ɂ���</history>
//        //=========================================================================
//        public void ShowUpdateDialog( modifiedFlags modifiedFlags )
//        {
//            this.modifiedFlags = modifiedFlags;

//            try
//            {
//// <DEL> 2008/10/21 ->
////                if (Properties.Settings.Default.hideRefreshDialog
////                    && (modifiedFlags.ForceShowDialog & modifiedFlags) == 0)	// �_�C�A���O��\��
////                {
////                    //---------------------------------
////                    // �_�C�A���O��\�������X�V����
////                    //---------------------------------
////// <MOD> 2008/10/21 ->
//////					AnimeServer.GetInstance().UpdateAll(modifiedFlags, null);
////// <MOD> 2008/10/21 <-
////                }
////                else
//// <DEL> 2008/10/21 <-
//                {
//                    //---------------------------------
//                    // �_�C�A���O�\�����čX�V����
//                    //---------------------------------
//                    ShowDialog();
//                }

//            }
//            catch (Exception e)
//            {
//                Logger.Output( e.Message );
//            }

//        }

//// <ADD> 2008/10/21 ->
//        //=========================================================================
//        ///	<summary>
//        ///		�f�[�^�X�V�V�[�P���X����̃G���[���b�Z�[�W�\��
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2008/10/21 �V�K�쐬</history>
//        //=========================================================================
//        private void LogoutMessage( string message )
//        {
//            // �V�[�P���X�X���b�h����̓R���g���[���ɃA�N�Z�X�ł��Ȃ�����
//            if( this.InvokeRequired )
//            {
//                AnimeServer.LogoutDelegate	del = LogoutMessage;
//                this.Invoke( del, new object[]{ message } );
//                return;
//            }

//            //----------------------------
//            // �e�L�X�g�{�b�N�X�ɒǉ�
//            //----------------------------
//            ErrorTextBox.Text += message + "\r\n";
//            ErrorTextBox.SelectionStart		= 0;
//            ErrorTextBox.SelectionLength	= 0;
			
//            detailsButton_Click(null,null);
//        }
//// <ADD> 2008/10/21 <-

//        private void UpdateDialog_Load(object sender, EventArgs e)
//        {
//        }

//        //=========================================================================
//        ///	<summary>
//        ///		�_�C�A���O���\�����ꂽ�Ƃ��̏���
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX �V�K�쐬</history>
//        //=========================================================================
//        private void UpdateDialog_Shown(object sender, EventArgs e)
//        {

//            this.closedButton.Enabled = false;

//// <ADD> 2008/10/21 ->
//            AnimeServer animeServer = AnimeServer.GetInstance();

//            if( endSequenceFlag == null )
//                endSequenceFlag = new ManualResetEvent(false);

//            if( !animeServer.BeginUpdate(
//                modifiedFlags	,
//                RefreshProgress	,
//                LogoutMessage	,
//                endSequenceFlag	) )
//            {
//                // ���Ƀf�[�^�X�V���s���Ȃ�
//                return;
//            }
//// <ADD> 2008/10/21 <-
//// <DEL> 2008/10/21 ->
//            ////----------------------------------
//            //// �o�b�N�O���E���h�ōX�V�������J�n
//            ////----------------------------------
//            //backgroundProc = Proc;

//            //procResult = backgroundProc.BeginInvoke(null, null);
//// <DEL> 2008/10/21 <-

//            Timer.Interval = 250;
//            Timer.Enabled	= true;
//        }

//        private delegate void OnRefreshProgress();

//        //=========================================================================
//        ///	<summary>
//        ///		�v���O���X���X�V����
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX �V�K�쐬</history>
//        //=========================================================================
//        private void RefreshProgress(
//            string	phase	,	// �����t�F�[�Y
////			int		phase	,	// �����t�F�[�Y
//            int		perc	,	// �t�F�[�Y���v���O���X
//            string	desc	)	// �ڍא���
//        {

//            OnRefreshProgress d = delegate()
//            {
//                ProgressLabel.Text = phase;

//                if( desc != null )
//                {
//                    descriptionLabel.Text = desc;
//                }
//                else
//                {
//                    descriptionLabel.Text = "";
//                }
				
//                progressBar.Value = perc;
//            };

//            ProgressLabel.Invoke(d);

//        }

//// <DEL> 2008/10/21 ->
//        ////=========================================================================
//        /////	<summary>
//        /////		�o�b�N�O���E���h�����v���V�[�W��
//        /////	</summary>
//        ///// <remarks>
//        ///// </remarks>
//        ///// <history>2006/XX/XX �V�K�쐬</history>
//        ////=========================================================================
//        //private void Proc()
//        //{
//        //    try
//        //    {
//        //        AnimeServer.GetInstance().UpdateAll( modifiedFlags, RefreshProgress );

//        //        //System.Threading.Thread.Sleep(5000);
			
//        //    }catch(Exception e)
//        //    {
//        //        errorMessage = e.Message;
				
//        //        errorOccurred = true;

//        //        return;
//        //    }
		
//        //    ///closedButton_Click(null,null);
		
//        //}
//// <DEL> 2008/10/21 <-

//        //=========================================================================
//        ///	<summary>
//        ///		�^�C�}�n���h��
//        ///	</summary>
//        /// <remarks>
//        ///		�o�b�N�O���E���h�����̏I����҂�
//        /// </remarks>
//        /// <history>2006/XX/XX �V�K�쐬</history>
//        //=========================================================================
//        private void Timer_Tick(object sender, EventArgs e)
//        {
//// <DEL> 2008/10/21 ->
//            if( endSequenceFlag.WaitOne( 0, false ) )
////			if(procResult.IsCompleted)
//// <DEL> 2008/10/21 <-
//            {
//                Timer.Enabled = false;

//                this.closedButton.Enabled = true;

//// <DEL> 2008/10/21 ->
//                ////----------------------------
//                //// �G���[������Ή�ʂɕ\��
//                ////----------------------------
//                //if (errorOccurred)
//                //{
//                //    Logger.Output(errorMessage);		// v1.5.25

//                //    ErrorTextBox.Text = errorMessage;
					
//                //    detailsButton_Click(null,null);
					
//                //}
//// <DEL> 2008/10/21 <-

//                //------------------------------
//                // ��莞�ԂŎ����I�ɕ���
//                //------------------------------
//                if (autoCloseCheckBox.Checked)
//                {
//// <MOD> 2008/10/21 �ڍ׃��b�Z�[�W������Β��� ->
//                    closeTimer.Interval = (0 < ErrorTextBox.Text.Length) ? 10000 : 50;
////					closeTimer.Interval = errorOccurred ? 3000 : 50;	// �G���[�Ȃ璷���\��
//// <MOD> 2008/10/21 <-
//                    closeTimer.Enabled = true;
//                }

//            }

//        }

//        //=========================================================================
//        ///	<summary>
//        ///		����{�^���̏���
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX �V�K�쐬</history>
//        //=========================================================================
//        private void closedButton_Click(object sender, EventArgs e)
//        {
		
//            this.DialogResult = DialogResult.OK;

//            Close();

//        }

//        //=========================================================================
//        ///	<summary>
//        ///		[�ڍ�>>]�{�^���̏���
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX �V�K�쐬</history>
//        //=========================================================================
//        private void detailsButton_Click(object sender, EventArgs e)
//        {
//            Size s = this.Size;

//            if (!detailsButton.Enabled) return;
			
//            s.Height += ErrorTextBox.Height + 15;
			
//            this.Size = s;

//            detailsButton.Enabled = false;
//        }

//        //=========================================================================
//        ///	<summary>
//        ///		�t�H�[����������ۂ̏���
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX �V�K�쐬</history>
//        //=========================================================================
//        private void UpdateDialog_FormClosing(object sender, FormClosingEventArgs e)
//        {
//// <MOD> 2008/10/21 ->
//            if( !endSequenceFlag.WaitOne( 0, false ) )
////			if (!procResult.IsCompleted)
//// <MOD> 2008/10/21 <-
//                e.Cancel = true;
//        }

//        //=========================================================================
//        ///	<summary>
//        ///		��莞�ԂŎ����I�Ƀt�H�[������鏈��
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX �V�K�쐬</history>
//        //=========================================================================
//        private void closeTimer_Tick(object sender, EventArgs e)
//        {
//            closedButton_Click(sender,e);
//        }


		
		
//    }
//}

// <DEL> 2009/11/23 <-
