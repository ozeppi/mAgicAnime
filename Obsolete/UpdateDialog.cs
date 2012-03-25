// <DEL> 2009/11/23 ->
////=========================================================================
/////	<summary>
/////		データ更新プログレス画面
/////	</summary>
///// <remarks>
///// </remarks>
///// <history>2006/XX/XX 新規作成</history>
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
//    ///		データ更新プログレス画面クラス
//    ///	</summary>
//    /// <remarks>
//    /// </remarks>
//    /// <history>2006/XX/XX 新規作成</history>
//    //=========================================================================
//    partial class UpdateDialog : Form
//    {
//        //----------------------
//        // メンバ変数
//        //----------------------
//// <DEL> 2008/10/21 ->
////		private BackgroundProc	backgroundProc;
////		private IAsyncResult	procResult;
//// <DEL> 2008/10/21 <-
//        private modifiedFlags	modifiedFlags;
//// <ADD> 2008/10/21 ->
//        private ManualResetEvent endSequenceFlag;		// データ更新シーケンス完了フラグ
//// <ADD> 2008/10/21 <-
		
//// <DEL> 2008/10/21 ->
////		private string			errorMessage;
////		private bool			errorOccurred = false;
//// <DEL> 2008/10/21 <-

//        private delegate void BackgroundProc();

//        //=========================================================================
//        ///	<summary>
//        ///		コンストラクタ
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX 新規作成</history>
//        //=========================================================================
//        public UpdateDialog()
//        {
//            InitializeComponent();
//        }

//        //=========================================================================
//        ///	<summary>
//        ///		データ更新プログレスダイアログを表示する(同期)
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX 新規作成</history>
//        /// <history>2006/11/26 非表示できるようにした</history>
//        //=========================================================================
//        public void ShowUpdateDialog( modifiedFlags modifiedFlags )
//        {
//            this.modifiedFlags = modifiedFlags;

//            try
//            {
//// <DEL> 2008/10/21 ->
////                if (Properties.Settings.Default.hideRefreshDialog
////                    && (modifiedFlags.ForceShowDialog & modifiedFlags) == 0)	// ダイアログ非表示
////                {
////                    //---------------------------------
////                    // ダイアログを表示せず更新処理
////                    //---------------------------------
////// <MOD> 2008/10/21 ->
//////					AnimeServer.GetInstance().UpdateAll(modifiedFlags, null);
////// <MOD> 2008/10/21 <-
////                }
////                else
//// <DEL> 2008/10/21 <-
//                {
//                    //---------------------------------
//                    // ダイアログ表示して更新処理
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
//        ///		データ更新シーケンスからのエラーメッセージ表示
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2008/10/21 新規作成</history>
//        //=========================================================================
//        private void LogoutMessage( string message )
//        {
//            // シーケンススレッドからはコントロールにアクセスできないため
//            if( this.InvokeRequired )
//            {
//                AnimeServer.LogoutDelegate	del = LogoutMessage;
//                this.Invoke( del, new object[]{ message } );
//                return;
//            }

//            //----------------------------
//            // テキストボックスに追加
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
//        ///		ダイアログが表示されたときの処理
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX 新規作成</history>
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
//                // 既にデータ更新実行中など
//                return;
//            }
//// <ADD> 2008/10/21 <-
//// <DEL> 2008/10/21 ->
//            ////----------------------------------
//            //// バックグラウンドで更新処理を開始
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
//        ///		プログレスを更新する
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX 新規作成</history>
//        //=========================================================================
//        private void RefreshProgress(
//            string	phase	,	// 処理フェーズ
////			int		phase	,	// 処理フェーズ
//            int		perc	,	// フェーズ内プログレス
//            string	desc	)	// 詳細説明
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
//        /////		バックグラウンド処理プロシージャ
//        /////	</summary>
//        ///// <remarks>
//        ///// </remarks>
//        ///// <history>2006/XX/XX 新規作成</history>
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
//        ///		タイマハンドラ
//        ///	</summary>
//        /// <remarks>
//        ///		バックグラウンド処理の終了を待つ
//        /// </remarks>
//        /// <history>2006/XX/XX 新規作成</history>
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
//                //// エラーがあれば画面に表示
//                ////----------------------------
//                //if (errorOccurred)
//                //{
//                //    Logger.Output(errorMessage);		// v1.5.25

//                //    ErrorTextBox.Text = errorMessage;
					
//                //    detailsButton_Click(null,null);
					
//                //}
//// <DEL> 2008/10/21 <-

//                //------------------------------
//                // 一定時間で自動的に閉じる
//                //------------------------------
//                if (autoCloseCheckBox.Checked)
//                {
//// <MOD> 2008/10/21 詳細メッセージがあれば長く ->
//                    closeTimer.Interval = (0 < ErrorTextBox.Text.Length) ? 10000 : 50;
////					closeTimer.Interval = errorOccurred ? 3000 : 50;	// エラーなら長く表示
//// <MOD> 2008/10/21 <-
//                    closeTimer.Enabled = true;
//                }

//            }

//        }

//        //=========================================================================
//        ///	<summary>
//        ///		閉じるボタンの処理
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX 新規作成</history>
//        //=========================================================================
//        private void closedButton_Click(object sender, EventArgs e)
//        {
		
//            this.DialogResult = DialogResult.OK;

//            Close();

//        }

//        //=========================================================================
//        ///	<summary>
//        ///		[詳細>>]ボタンの処理
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX 新規作成</history>
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
//        ///		フォームが閉じられる際の処理
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX 新規作成</history>
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
//        ///		一定時間で自動的にフォームを閉じる処理
//        ///	</summary>
//        /// <remarks>
//        /// </remarks>
//        /// <history>2006/XX/XX 新規作成</history>
//        //=========================================================================
//        private void closeTimer_Tick(object sender, EventArgs e)
//        {
//            closedButton_Click(sender,e);
//        }


		
		
//    }
//}

// <DEL> 2009/11/23 <-
