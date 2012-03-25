namespace magicAnime
{
	partial class RecordDialog
	{
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナで生成されたコード

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.OkButton = new System.Windows.Forms.Button();
			this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.repeatNumberTextBox = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.lengthUpdown = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			this.isReserveErrorCheckBox = new System.Windows.Forms.CheckBox();
			this.isStoredCheckBox = new System.Windows.Forms.CheckBox();
			this.isEncodedCheckBox = new System.Windows.Forms.CheckBox();
			this.isReservedCheckBox = new System.Windows.Forms.CheckBox();
			this.reserveDateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.label4 = new System.Windows.Forms.Label();
			this.hasFileCheckBox = new System.Windows.Forms.CheckBox();
			this.hasPlanCheckBox = new System.Windows.Forms.CheckBox();
			this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.label7 = new System.Windows.Forms.Label();
			this.OpenFileButton = new System.Windows.Forms.Button();
			this.FileTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.StoryNumberUpdown = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.TitleTextBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.SubtitleTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.thumbnailMakedCheckBox = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.timeChangedCheckBox = new System.Windows.Forms.CheckBox();
			this.recordEndDateTime = new System.Windows.Forms.DateTimePicker();
			this.label11 = new System.Windows.Forms.Label();
			this.recordStartDateTime = new System.Windows.Forms.DateTimePicker();
			this.label10 = new System.Windows.Forms.Label();
			this.isStorableCheckBox = new System.Windows.Forms.CheckBox();
			this.isStartedOnairCheckBox = new System.Windows.Forms.CheckBox();
			this.isRecordedCheckBox = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.planErrorCheckbox = new System.Windows.Forms.CheckBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lengthUpdown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.StoryNumberUpdown)).BeginInit();
			this.tabPage2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// OkButton
			// 
			this.OkButton.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.OkButton.Location = new System.Drawing.Point(421, 535);
			this.OkButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.OkButton.Name = "OkButton";
			this.OkButton.Size = new System.Drawing.Size(155, 39);
			this.OkButton.TabIndex = 1;
			this.OkButton.Text = "&Ok";
			this.OkButton.UseVisualStyleBackColor = true;
			this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
			// 
			// OpenFileDialog
			// 
			this.OpenFileDialog.DefaultExt = "mpg";
			this.OpenFileDialog.Filter = "動画ファイル|*.mpg;*.m2p;*.avi;*.ts|MPEGファイル|*.mpg;*.m2p|AVIファイル|*.avi|その他|*.*";
			this.OpenFileDialog.FilterIndex = 4;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.tabControl1.Location = new System.Drawing.Point(11, 15);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(565, 512);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.repeatNumberTextBox);
			this.tabPage1.Controls.Add(this.label9);
			this.tabPage1.Controls.Add(this.lengthUpdown);
			this.tabPage1.Controls.Add(this.label8);
			this.tabPage1.Controls.Add(this.isReserveErrorCheckBox);
			this.tabPage1.Controls.Add(this.isStoredCheckBox);
			this.tabPage1.Controls.Add(this.isEncodedCheckBox);
			this.tabPage1.Controls.Add(this.isReservedCheckBox);
			this.tabPage1.Controls.Add(this.reserveDateTimePicker);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.hasFileCheckBox);
			this.tabPage1.Controls.Add(this.hasPlanCheckBox);
			this.tabPage1.Controls.Add(this.dateTimePicker);
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this.OpenFileButton);
			this.tabPage1.Controls.Add(this.FileTextBox);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.StoryNumberUpdown);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.TitleTextBox);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.SubtitleTextBox);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Location = new System.Drawing.Point(4, 29);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPage1.Size = new System.Drawing.Size(557, 479);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "プロパティ";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// repeatNumberTextBox
			// 
			this.repeatNumberTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.repeatNumberTextBox.Location = new System.Drawing.Point(161, 200);
			this.repeatNumberTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.repeatNumberTextBox.Name = "repeatNumberTextBox";
			this.repeatNumberTextBox.ReadOnly = true;
			this.repeatNumberTextBox.Size = new System.Drawing.Size(219, 19);
			this.repeatNumberTextBox.TabIndex = 11;
			this.repeatNumberTextBox.Text = "第n回目の放送です。";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label9.Location = new System.Drawing.Point(476, 168);
			this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(28, 19);
			this.label9.TabIndex = 10;
			this.label9.Text = "分";
			// 
			// lengthUpdown
			// 
			this.lengthUpdown.Location = new System.Drawing.Point(405, 164);
			this.lengthUpdown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.lengthUpdown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this.lengthUpdown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.lengthUpdown.Name = "lengthUpdown";
			this.lengthUpdown.Size = new System.Drawing.Size(67, 26);
			this.lengthUpdown.TabIndex = 9;
			this.lengthUpdown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.lengthUpdown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label8.Location = new System.Drawing.Point(35, 291);
			this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(287, 19);
			this.label8.TabIndex = 15;
			this.label8.Text = "(録画ソフトに予約している放送時刻)";
			// 
			// isReserveErrorCheckBox
			// 
			this.isReserveErrorCheckBox.AutoSize = true;
			this.isReserveErrorCheckBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.isReserveErrorCheckBox.Location = new System.Drawing.Point(21, 316);
			this.isReserveErrorCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.isReserveErrorCheckBox.Name = "isReserveErrorCheckBox";
			this.isReserveErrorCheckBox.Size = new System.Drawing.Size(380, 23);
			this.isReserveErrorCheckBox.TabIndex = 16;
			this.isReserveErrorCheckBox.Text = "録画予約エラー (予約に異常が発生した場合)";
			this.isReserveErrorCheckBox.UseVisualStyleBackColor = true;
			// 
			// isStoredCheckBox
			// 
			this.isStoredCheckBox.AutoSize = true;
			this.isStoredCheckBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.isStoredCheckBox.Location = new System.Drawing.Point(21, 441);
			this.isStoredCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.isStoredCheckBox.Name = "isStoredCheckBox";
			this.isStoredCheckBox.Size = new System.Drawing.Size(160, 23);
			this.isStoredCheckBox.TabIndex = 22;
			this.isStoredCheckBox.Text = "保存先に転送済";
			this.isStoredCheckBox.UseVisualStyleBackColor = true;
			// 
			// isEncodedCheckBox
			// 
			this.isEncodedCheckBox.AutoSize = true;
			this.isEncodedCheckBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.isEncodedCheckBox.Location = new System.Drawing.Point(21, 410);
			this.isEncodedCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.isEncodedCheckBox.Name = "isEncodedCheckBox";
			this.isEncodedCheckBox.Size = new System.Drawing.Size(140, 23);
			this.isEncodedCheckBox.TabIndex = 21;
			this.isEncodedCheckBox.Text = "再エンコード済";
			this.isEncodedCheckBox.UseVisualStyleBackColor = true;
			// 
			// isReservedCheckBox
			// 
			this.isReservedCheckBox.AutoSize = true;
			this.isReservedCheckBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.isReservedCheckBox.Location = new System.Drawing.Point(23, 226);
			this.isReservedCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.isReservedCheckBox.Name = "isReservedCheckBox";
			this.isReservedCheckBox.Size = new System.Drawing.Size(126, 23);
			this.isReservedCheckBox.TabIndex = 12;
			this.isReservedCheckBox.Text = "録画予約済";
			this.isReservedCheckBox.UseVisualStyleBackColor = true;
			this.isReservedCheckBox.CheckedChanged += new System.EventHandler(this.isReservedCheckBox_CheckedChanged);
			// 
			// reserveDateTimePicker
			// 
			this.reserveDateTimePicker.CustomFormat = "yyyy/MM/dd HH:mm";
			this.reserveDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.reserveDateTimePicker.Location = new System.Drawing.Point(161, 255);
			this.reserveDateTimePicker.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.reserveDateTimePicker.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
			this.reserveDateTimePicker.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
			this.reserveDateTimePicker.Name = "reserveDateTimePicker";
			this.reserveDateTimePicker.Size = new System.Drawing.Size(217, 26);
			this.reserveDateTimePicker.TabIndex = 14;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label4.Location = new System.Drawing.Point(39, 259);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(112, 19);
			this.label4.TabIndex = 13;
			this.label4.Text = "予約日時(&T):";
			// 
			// hasFileCheckBox
			// 
			this.hasFileCheckBox.AutoSize = true;
			this.hasFileCheckBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.hasFileCheckBox.Location = new System.Drawing.Point(21, 348);
			this.hasFileCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.hasFileCheckBox.Name = "hasFileCheckBox";
			this.hasFileCheckBox.Size = new System.Drawing.Size(151, 23);
			this.hasFileCheckBox.TabIndex = 17;
			this.hasFileCheckBox.Text = "録画ファイルあり";
			this.hasFileCheckBox.UseVisualStyleBackColor = true;
			this.hasFileCheckBox.CheckedChanged += new System.EventHandler(this.hasFileCheckBox_CheckedChanged);
			// 
			// hasPlanCheckBox
			// 
			this.hasPlanCheckBox.AutoSize = true;
			this.hasPlanCheckBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.hasPlanCheckBox.Location = new System.Drawing.Point(23, 136);
			this.hasPlanCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.hasPlanCheckBox.Name = "hasPlanCheckBox";
			this.hasPlanCheckBox.Size = new System.Drawing.Size(148, 23);
			this.hasPlanCheckBox.TabIndex = 6;
			this.hasPlanCheckBox.Text = "放送プラン確定";
			this.hasPlanCheckBox.UseVisualStyleBackColor = true;
			this.hasPlanCheckBox.CheckedChanged += new System.EventHandler(this.hasPlanCheckBox_CheckedChanged);
			// 
			// dateTimePicker
			// 
			this.dateTimePicker.CustomFormat = "yyyy/MM/dd HH:mm";
			this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePicker.Location = new System.Drawing.Point(161, 165);
			this.dateTimePicker.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.dateTimePicker.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
			this.dateTimePicker.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
			this.dateTimePicker.Name = "dateTimePicker";
			this.dateTimePicker.Size = new System.Drawing.Size(217, 26);
			this.dateTimePicker.TabIndex = 8;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label7.Location = new System.Drawing.Point(35, 168);
			this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(112, 19);
			this.label7.TabIndex = 7;
			this.label7.Text = "放送日時(&T):";
			// 
			// OpenFileButton
			// 
			this.OpenFileButton.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.OpenFileButton.Location = new System.Drawing.Point(480, 375);
			this.OpenFileButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.OpenFileButton.Name = "OpenFileButton";
			this.OpenFileButton.Size = new System.Drawing.Size(44, 26);
			this.OpenFileButton.TabIndex = 20;
			this.OpenFileButton.Text = "..";
			this.OpenFileButton.UseVisualStyleBackColor = true;
			this.OpenFileButton.Click += new System.EventHandler(this.OpenFileButton_Click_1);
			// 
			// FileTextBox
			// 
			this.FileTextBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.FileTextBox.Location = new System.Drawing.Point(161, 375);
			this.FileTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.FileTextBox.Name = "FileTextBox";
			this.FileTextBox.Size = new System.Drawing.Size(309, 26);
			this.FileTextBox.TabIndex = 19;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label3.Location = new System.Drawing.Point(35, 379);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(89, 19);
			this.label3.TabIndex = 18;
			this.label3.Text = "ファイル(&F):";
			// 
			// StoryNumberUpdown
			// 
			this.StoryNumberUpdown.Enabled = false;
			this.StoryNumberUpdown.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.StoryNumberUpdown.Location = new System.Drawing.Point(161, 52);
			this.StoryNumberUpdown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.StoryNumberUpdown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
			this.StoryNumberUpdown.Name = "StoryNumberUpdown";
			this.StoryNumberUpdown.ReadOnly = true;
			this.StoryNumberUpdown.Size = new System.Drawing.Size(65, 26);
			this.StoryNumberUpdown.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label2.Location = new System.Drawing.Point(19, 56);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(85, 19);
			this.label2.TabIndex = 2;
			this.label2.Text = "第n話(&N):";
			// 
			// TitleTextBox
			// 
			this.TitleTextBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.TitleTextBox.Location = new System.Drawing.Point(161, 20);
			this.TitleTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.TitleTextBox.Name = "TitleTextBox";
			this.TitleTextBox.ReadOnly = true;
			this.TitleTextBox.Size = new System.Drawing.Size(363, 26);
			this.TitleTextBox.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label5.Location = new System.Drawing.Point(19, 22);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(86, 19);
			this.label5.TabIndex = 0;
			this.label5.Text = "タイトル(&I):";
			// 
			// SubtitleTextBox
			// 
			this.SubtitleTextBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.SubtitleTextBox.Location = new System.Drawing.Point(161, 91);
			this.SubtitleTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.SubtitleTextBox.Name = "SubtitleTextBox";
			this.SubtitleTextBox.Size = new System.Drawing.Size(363, 26);
			this.SubtitleTextBox.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label1.Location = new System.Drawing.Point(17, 95);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(122, 19);
			this.label1.TabIndex = 4;
			this.label1.Text = "サブタイトル(&S):";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.planErrorCheckbox);
			this.tabPage2.Controls.Add(this.thumbnailMakedCheckBox);
			this.tabPage2.Controls.Add(this.groupBox1);
			this.tabPage2.Location = new System.Drawing.Point(4, 29);
			this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPage2.Size = new System.Drawing.Size(557, 479);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "詳細";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// thumbnailMakedCheckBox
			// 
			this.thumbnailMakedCheckBox.AutoSize = true;
			this.thumbnailMakedCheckBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.thumbnailMakedCheckBox.Location = new System.Drawing.Point(25, 31);
			this.thumbnailMakedCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.thumbnailMakedCheckBox.Name = "thumbnailMakedCheckBox";
			this.thumbnailMakedCheckBox.Size = new System.Drawing.Size(166, 23);
			this.thumbnailMakedCheckBox.TabIndex = 18;
			this.thumbnailMakedCheckBox.Text = "サムネイル作成済";
			this.thumbnailMakedCheckBox.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.timeChangedCheckBox);
			this.groupBox1.Controls.Add(this.recordEndDateTime);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.recordStartDateTime);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.isStorableCheckBox);
			this.groupBox1.Controls.Add(this.isStartedOnairCheckBox);
			this.groupBox1.Controls.Add(this.isRecordedCheckBox);
			this.groupBox1.Location = new System.Drawing.Point(25, 218);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox1.Size = new System.Drawing.Size(500, 236);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "読み取り専用プロパティ";
			// 
			// timeChangedCheckBox
			// 
			this.timeChangedCheckBox.AutoSize = true;
			this.timeChangedCheckBox.Enabled = false;
			this.timeChangedCheckBox.Location = new System.Drawing.Point(28, 125);
			this.timeChangedCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.timeChangedCheckBox.Name = "timeChangedCheckBox";
			this.timeChangedCheckBox.Size = new System.Drawing.Size(346, 23);
			this.timeChangedCheckBox.TabIndex = 7;
			this.timeChangedCheckBox.Text = "録画すべき時刻と予約した時刻にずれあり";
			this.timeChangedCheckBox.UseVisualStyleBackColor = true;
			// 
			// recordEndDateTime
			// 
			this.recordEndDateTime.CustomFormat = "yyyy/MM/dd HH:mm";
			this.recordEndDateTime.Enabled = false;
			this.recordEndDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.recordEndDateTime.Location = new System.Drawing.Point(163, 189);
			this.recordEndDateTime.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.recordEndDateTime.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
			this.recordEndDateTime.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
			this.recordEndDateTime.Name = "recordEndDateTime";
			this.recordEndDateTime.Size = new System.Drawing.Size(195, 26);
			this.recordEndDateTime.TabIndex = 6;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label11.Location = new System.Drawing.Point(21, 191);
			this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(127, 19);
			this.label11.TabIndex = 5;
			this.label11.Text = "録画終了日時:";
			// 
			// recordStartDateTime
			// 
			this.recordStartDateTime.CustomFormat = "yyyy/MM/dd HH:mm";
			this.recordStartDateTime.Enabled = false;
			this.recordStartDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.recordStartDateTime.Location = new System.Drawing.Point(163, 154);
			this.recordStartDateTime.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.recordStartDateTime.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
			this.recordStartDateTime.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
			this.recordStartDateTime.Name = "recordStartDateTime";
			this.recordStartDateTime.Size = new System.Drawing.Size(195, 26);
			this.recordStartDateTime.TabIndex = 4;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label10.Location = new System.Drawing.Point(21, 156);
			this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(127, 19);
			this.label10.TabIndex = 3;
			this.label10.Text = "録画開始日時:";
			// 
			// isStorableCheckBox
			// 
			this.isStorableCheckBox.AutoSize = true;
			this.isStorableCheckBox.Enabled = false;
			this.isStorableCheckBox.Location = new System.Drawing.Point(28, 94);
			this.isStorableCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.isStorableCheckBox.Name = "isStorableCheckBox";
			this.isStorableCheckBox.Size = new System.Drawing.Size(223, 23);
			this.isStorableCheckBox.TabIndex = 2;
			this.isStorableCheckBox.Text = "保存先に転送できる状態";
			this.isStorableCheckBox.UseVisualStyleBackColor = true;
			// 
			// isStartedOnairCheckBox
			// 
			this.isStartedOnairCheckBox.AutoSize = true;
			this.isStartedOnairCheckBox.Enabled = false;
			this.isStartedOnairCheckBox.Location = new System.Drawing.Point(28, 62);
			this.isStartedOnairCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.isStartedOnairCheckBox.Name = "isStartedOnairCheckBox";
			this.isStartedOnairCheckBox.Size = new System.Drawing.Size(208, 23);
			this.isStartedOnairCheckBox.TabIndex = 1;
			this.isStartedOnairCheckBox.Text = "放送開始時間を過ぎた";
			this.isStartedOnairCheckBox.UseVisualStyleBackColor = true;
			// 
			// isRecordedCheckBox
			// 
			this.isRecordedCheckBox.AutoSize = true;
			this.isRecordedCheckBox.Enabled = false;
			this.isRecordedCheckBox.Location = new System.Drawing.Point(28, 31);
			this.isRecordedCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.isRecordedCheckBox.Name = "isRecordedCheckBox";
			this.isRecordedCheckBox.Size = new System.Drawing.Size(236, 23);
			this.isRecordedCheckBox.TabIndex = 0;
			this.isRecordedCheckBox.Text = "放送中または録画済である";
			this.isRecordedCheckBox.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label6.Location = new System.Drawing.Point(16, 545);
			this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(310, 19);
			this.label6.TabIndex = 0;
			this.label6.Text = "手動で変更する必要は通常ありません。";
			// 
			// planErrorCheckbox
			// 
			this.planErrorCheckbox.AutoSize = true;
			this.planErrorCheckbox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.planErrorCheckbox.Location = new System.Drawing.Point(25, 62);
			this.planErrorCheckbox.Margin = new System.Windows.Forms.Padding(4);
			this.planErrorCheckbox.Name = "planErrorCheckbox";
			this.planErrorCheckbox.Size = new System.Drawing.Size(195, 23);
			this.planErrorCheckbox.TabIndex = 19;
			this.planErrorCheckbox.Text = "放送データに異常あり";
			this.planErrorCheckbox.UseVisualStyleBackColor = true;
			// 
			// RecordDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(593, 584);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.OkButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RecordDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "エピソードのプロパティ";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.lengthUpdown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.StoryNumberUpdown)).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button OkButton;
		private System.Windows.Forms.OpenFileDialog OpenFileDialog;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Button OpenFileButton;
		private System.Windows.Forms.TextBox FileTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown StoryNumberUpdown;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox TitleTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox SubtitleTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DateTimePicker dateTimePicker;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox hasFileCheckBox;
		private System.Windows.Forms.CheckBox hasPlanCheckBox;
		private System.Windows.Forms.CheckBox isReservedCheckBox;
		private System.Windows.Forms.DateTimePicker reserveDateTimePicker;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox isStoredCheckBox;
		private System.Windows.Forms.CheckBox isEncodedCheckBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox isReserveErrorCheckBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown lengthUpdown;
		private System.Windows.Forms.TextBox repeatNumberTextBox;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox isRecordedCheckBox;
		private System.Windows.Forms.CheckBox isStartedOnairCheckBox;
		private System.Windows.Forms.CheckBox isStorableCheckBox;
		private System.Windows.Forms.DateTimePicker recordStartDateTime;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.DateTimePicker recordEndDateTime;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.CheckBox timeChangedCheckBox;
		private System.Windows.Forms.CheckBox thumbnailMakedCheckBox;
		private System.Windows.Forms.CheckBox planErrorCheckbox;
	}
}