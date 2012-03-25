//=========================================================================
///	<summary>
///		保存先に転送ダイアログ
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成	Dr.Kurusugawa</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace magicAnime
{
	public partial class TransferDialog : Form
	{
		private string	mFileName;
		private int		mCount			= 0;
		private int		mCompleteCount	= 0;
		private bool	mIgnoreError	= false;

		private ProgressBar			progressBar;
		private BackgroundWorker	bgWorker;
		private Label				fileLabel;
		private CheckedListBox		checkAnimeListBox;
		private Button				startButton;
		private Button				stopButton;
		private Label				descriptLabel;
		private FolderBrowserDialog	folderBrowser;
		private Button				folderButton;
		private Label				folderLabel;
		private Button				allClearButton;
		private Button				allSelectButton;
		private CheckBox			ignoreErrorCheckBox;
		CheckedListBox.CheckedIndexCollection	checkedList;

		public TransferDialog()
		{
			InitializeComponent();
		}

		private void stopButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void shownButton_Click(object sender, EventArgs e)
		{
			progressBar.Value = 0;

		}

		private void TransferDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			bgWorker.CancelAsync();
		}

		//=========================================================================
		///	<summary>
		///		保存先転送プロシージャ
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker	worker	= (BackgroundWorker)sender;
			AnimeServer			doc		= AnimeServer.GetInstance();
			AnimeProgram.EnumRecordCallBack		cb;
			int		i			= 0;
			bool	canceled	= false;

// <DEL> 2010/01/17 ->
//            // 転送可能な番組数を数える
//            cb = delegate(AnimeEpisode a, object param)
//            {
//// <MOD> 2009/12/30 ->
//                if( !a.IsStored && a.IsStorable )
////				if (a.IsStorable)
//// <MOD> 2009/12/30 <-
//                    mCount++;
//            };
//            doc.EnumAllEpisodes(cb, null);
// <DEL> 2010/01/17 <-

// <MOD> 2010/01/17 ->
			// 選択された番組の列挙
			List<AnimeProgram> progs = new List<AnimeProgram>();

			foreach (int k in checkedList)
				progs.Add( doc.Animes[k] );

			// 転送すべきエピソードの列挙
			List<AnimeEpisode> targets = new List<AnimeEpisode>();

			foreach( AnimeProgram pg in progs )
				foreach( AnimeEpisode ep in pg.Episodes )
					if( ep.IsStorable && !ep.IsStored )
						targets.Add( ep );

			// 選択されｔエピソードを保存先に転送
			mCount = targets.Count;

			foreach( AnimeEpisode target in targets )
			{
				mFileName = target.GetFormattedFileName();

				worker.ReportProgress(100 * i++ / mCount);

				try
				{
					target.Store();
					mCompleteCount++;
				}
				catch ( Exception ex )
				{
					if( !mIgnoreError )
						Program.ShowException( ex, MessageBoxIcon.Warning );
				}

				if (worker.CancellationPending)	// キャンセル要求あり？
				{
					e.Cancel = true;
					canceled = true;
				}
			}


//            // エピソードの転送処理
//            cb = delegate(AnimeEpisode animeRecord, object param)
//            {
//                bool matched = false;
//                if (canceled) return;

//                // 選択されている番組か？
//                foreach (int k in checkedList)
//                {
//// <MOD> 2009/12/30 ->
//                    matched = doc.Animes[k].Episodes.Contains(animeRecord);
////					if (0 <= doc.Animes[k].Episodes.IndexOf(animeRecord))
////						matched = true;
//// <MOD> 2009/12/30 <-
//                }

//// <MOD> 2009/12/30 ->
//                if( matched && !animeRecord.IsStored && animeRecord.IsStorable )
////				if (matched && animeRecord.IsStorable)
//// <MOD> 2009/12/30 <-
//                {
//                    fileName = animeRecord.GetFormattedFileName();

//                    worker.ReportProgress(100 * i++ / count);

//                    try
//                    {
//                        animeRecord.Store();							// 転送
//                    }
//                    catch ( Exception ex )
//                    {
//                        Program.ShowException( ex, MessageBoxIcon.Warning );
//                    }

//                    completeCount++;

//                    if (worker.CancellationPending)						// キャンセル要求あり？
//                    {
//                        e.Cancel = true;
//                        canceled = true;
//                    }
//                }
//            };

//            doc.EnumAllEpisodes(cb, null);
// <MOD> 2010/01/17 <-

			mFileName = "";
			worker.ReportProgress(100);
		}

		private void bgWorker_RunWorkerCompleted(
			object sender,
			RunWorkerCompletedEventArgs e)
		{
			string text;
			//stopButton.Enabled	= true;
			//stopButton.Text		= "閉じる(&C)";
			text = string.Format("{0}/{1}個のファイルを転送しました", mCompleteCount, mCount);
			MessageBox.Show(text, "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

			Close();
		}

		private void bgWorker_ProgressChanged(
			object sender,
			ProgressChangedEventArgs e)
		{
			fileLabel.Text		= mFileName;
			progressBar.Value	= e.ProgressPercentage;
		}

		//
		//
		//
		private void FormShown(object sender, EventArgs e)
		{
			AnimeServer s = AnimeServer.GetInstance();

			//
			// アニメ一覧をリストボックスに追加
			//
			foreach (AnimeProgram a in s.Animes)
			{
				int i;
				i = checkAnimeListBox.Items.Add( a.title );
				checkAnimeListBox.SetItemChecked(i,true);
			}


			folderLabel.Text = Properties.Settings.Default.saveFolder;

		}

		// 開始ボタンの処理
		private void startButton_Click(object sender, EventArgs e)
		{
			checkedList = checkAnimeListBox.CheckedIndices;

			checkAnimeListBox.Enabled	= false;
			startButton.Enabled			= false;
			stopButton.Enabled			= true;
			descriptLabel.Text			= "転送中です。";

			mIgnoreError				= ignoreErrorCheckBox.Checked;

			bgWorker.RunWorkerAsync();
		}

		private void InitializeComponent()
		{
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.bgWorker = new System.ComponentModel.BackgroundWorker();
			this.fileLabel = new System.Windows.Forms.Label();
			this.checkAnimeListBox = new System.Windows.Forms.CheckedListBox();
			this.startButton = new System.Windows.Forms.Button();
			this.stopButton = new System.Windows.Forms.Button();
			this.descriptLabel = new System.Windows.Forms.Label();
			this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
			this.folderButton = new System.Windows.Forms.Button();
			this.folderLabel = new System.Windows.Forms.Label();
			this.allClearButton = new System.Windows.Forms.Button();
			this.allSelectButton = new System.Windows.Forms.Button();
			this.ignoreErrorCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(12, 199);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(250, 19);
			this.progressBar.TabIndex = 0;
			// 
			// bgWorker
			// 
			this.bgWorker.WorkerReportsProgress = true;
			this.bgWorker.WorkerSupportsCancellation = true;
			this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
			this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
			this.bgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorker_ProgressChanged);
			// 
			// fileLabel
			// 
			this.fileLabel.Location = new System.Drawing.Point(10, 221);
			this.fileLabel.Name = "fileLabel";
			this.fileLabel.Size = new System.Drawing.Size(252, 23);
			this.fileLabel.TabIndex = 1;
			// 
			// checkAnimeListBox
			// 
			this.checkAnimeListBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.checkAnimeListBox.FormattingEnabled = true;
			this.checkAnimeListBox.Location = new System.Drawing.Point(12, 12);
			this.checkAnimeListBox.Name = "checkAnimeListBox";
			this.checkAnimeListBox.Size = new System.Drawing.Size(363, 166);
			this.checkAnimeListBox.TabIndex = 2;
			// 
			// startButton
			// 
			this.startButton.Location = new System.Drawing.Point(381, 12);
			this.startButton.Name = "startButton";
			this.startButton.Size = new System.Drawing.Size(131, 30);
			this.startButton.TabIndex = 3;
			this.startButton.Text = "開始(&S)";
			this.startButton.UseVisualStyleBackColor = true;
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// stopButton
			// 
			this.stopButton.Location = new System.Drawing.Point(381, 48);
			this.stopButton.Name = "stopButton";
			this.stopButton.Size = new System.Drawing.Size(131, 30);
			this.stopButton.TabIndex = 4;
			this.stopButton.Text = "中止(&T)";
			this.stopButton.UseVisualStyleBackColor = true;
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// descriptLabel
			// 
			this.descriptLabel.AutoSize = true;
			this.descriptLabel.Location = new System.Drawing.Point(268, 202);
			this.descriptLabel.Name = "descriptLabel";
			this.descriptLabel.Size = new System.Drawing.Size(43, 12);
			this.descriptLabel.TabIndex = 5;
			this.descriptLabel.Text = "Ready...";
			// 
			// folderButton
			// 
			this.folderButton.Enabled = false;
			this.folderButton.Location = new System.Drawing.Point(381, 264);
			this.folderButton.Name = "folderButton";
			this.folderButton.Size = new System.Drawing.Size(131, 30);
			this.folderButton.TabIndex = 6;
			this.folderButton.Text = "転送先フォルダ(&F)...";
			this.folderButton.UseVisualStyleBackColor = true;
			this.folderButton.Click += new System.EventHandler(this.folderButton_Click);
			// 
			// folderLabel
			// 
			this.folderLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.folderLabel.Enabled = false;
			this.folderLabel.Location = new System.Drawing.Point(12, 267);
			this.folderLabel.Name = "folderLabel";
			this.folderLabel.Size = new System.Drawing.Size(344, 25);
			this.folderLabel.TabIndex = 7;
			this.folderLabel.Text = "label1";
			this.folderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.folderLabel.UseMnemonic = false;
			// 
			// allClearButton
			// 
			this.allClearButton.Location = new System.Drawing.Point(381, 148);
			this.allClearButton.Name = "allClearButton";
			this.allClearButton.Size = new System.Drawing.Size(131, 30);
			this.allClearButton.TabIndex = 8;
			this.allClearButton.Text = "全て解除(&C)";
			this.allClearButton.UseVisualStyleBackColor = true;
			this.allClearButton.Click += new System.EventHandler(this.allClearButton_Click);
			// 
			// allSelectButton
			// 
			this.allSelectButton.Location = new System.Drawing.Point(381, 112);
			this.allSelectButton.Name = "allSelectButton";
			this.allSelectButton.Size = new System.Drawing.Size(131, 30);
			this.allSelectButton.TabIndex = 9;
			this.allSelectButton.Text = "全て選択(&A)";
			this.allSelectButton.UseVisualStyleBackColor = true;
			this.allSelectButton.Click += new System.EventHandler(this.allSelectButton_Click);
			// 
			// ignoreErrorCheckBox
			// 
			this.ignoreErrorCheckBox.AutoSize = true;
			this.ignoreErrorCheckBox.Checked = true;
			this.ignoreErrorCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ignoreErrorCheckBox.Location = new System.Drawing.Point(381, 198);
			this.ignoreErrorCheckBox.Name = "ignoreErrorCheckBox";
			this.ignoreErrorCheckBox.Size = new System.Drawing.Size(95, 16);
			this.ignoreErrorCheckBox.TabIndex = 10;
			this.ignoreErrorCheckBox.Text = "エラーを無視(&I)";
			this.ignoreErrorCheckBox.UseVisualStyleBackColor = true;
			// 
			// TransferDialog
			// 
			this.ClientSize = new System.Drawing.Size(525, 253);
			this.Controls.Add(this.ignoreErrorCheckBox);
			this.Controls.Add(this.allSelectButton);
			this.Controls.Add(this.allClearButton);
			this.Controls.Add(this.folderLabel);
			this.Controls.Add(this.folderButton);
			this.Controls.Add(this.descriptLabel);
			this.Controls.Add(this.stopButton);
			this.Controls.Add(this.startButton);
			this.Controls.Add(this.checkAnimeListBox);
			this.Controls.Add(this.fileLabel);
			this.Controls.Add(this.progressBar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TransferDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "転送";
			this.Shown += new System.EventHandler(this.FormShown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private void folderButton_Click(object sender, EventArgs e)
		{
			folderBrowser.ShowDialog();
		}

		private void allClearButton_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checkAnimeListBox.Items.Count; ++i)
			{
				checkAnimeListBox.SetItemChecked(i, false);
			}
		}

		private void allSelectButton_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checkAnimeListBox.Items.Count;++i )
			{
				checkAnimeListBox.SetItemChecked(i, true);
			}
		}

	}
}