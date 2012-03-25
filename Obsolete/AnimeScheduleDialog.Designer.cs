namespace magicAnime
{
	partial class AnimeScheduleDialog
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
			this.label1 = new System.Windows.Forms.Label();
			this.firstOnAirGroup = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.dayComboBox = new System.Windows.Forms.ComboBox();
			this.monthComboBox = new System.Windows.Forms.ComboBox();
			this.yearComboBox = new System.Windows.Forms.ComboBox();
			this.onAirTimeGroup = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.minuteComboBox = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.hourComboBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.dayOfWeekComboBox = new System.Windows.Forms.ComboBox();
			this.undecidedRadioBox = new System.Windows.Forms.RadioButton();
			this.serialRadioBox = new System.Windows.Forms.RadioButton();
			this.okButton = new System.Windows.Forms.Button();
			this.autoRadioBox = new System.Windows.Forms.RadioButton();
			this.firstOnAirGroup.SuspendLayout();
			this.onAirTimeGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label1.Location = new System.Drawing.Point(11, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(359, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "第1話から最終話までの放送日と時刻を一括で設定します。";
			// 
			// firstOnAirGroup
			// 
			this.firstOnAirGroup.Controls.Add(this.label5);
			this.firstOnAirGroup.Controls.Add(this.label4);
			this.firstOnAirGroup.Controls.Add(this.label3);
			this.firstOnAirGroup.Controls.Add(this.dayComboBox);
			this.firstOnAirGroup.Controls.Add(this.monthComboBox);
			this.firstOnAirGroup.Controls.Add(this.yearComboBox);
			this.firstOnAirGroup.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.firstOnAirGroup.Location = new System.Drawing.Point(29, 227);
			this.firstOnAirGroup.Name = "firstOnAirGroup";
			this.firstOnAirGroup.Size = new System.Drawing.Size(356, 64);
			this.firstOnAirGroup.TabIndex = 11;
			this.firstOnAirGroup.TabStop = false;
			this.firstOnAirGroup.Text = "第1週目の日付(※)";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label5.Location = new System.Drawing.Point(315, 29);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(22, 15);
			this.label5.TabIndex = 8;
			this.label5.Text = "日";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label4.Location = new System.Drawing.Point(208, 29);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(22, 15);
			this.label4.TabIndex = 7;
			this.label4.Text = "月";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label3.Location = new System.Drawing.Point(110, 29);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(22, 15);
			this.label3.TabIndex = 6;
			this.label3.Text = "年";
			// 
			// dayComboBox
			// 
			this.dayComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.dayComboBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.dayComboBox.FormattingEnabled = true;
			this.dayComboBox.Location = new System.Drawing.Point(236, 26);
			this.dayComboBox.Name = "dayComboBox";
			this.dayComboBox.Size = new System.Drawing.Size(73, 23);
			this.dayComboBox.TabIndex = 5;
			this.dayComboBox.SelectedIndexChanged += new System.EventHandler(this.dayComboBox_SelectedIndexChanged);
			// 
			// monthComboBox
			// 
			this.monthComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.monthComboBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.monthComboBox.FormattingEnabled = true;
			this.monthComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"});
			this.monthComboBox.Location = new System.Drawing.Point(135, 26);
			this.monthComboBox.Name = "monthComboBox";
			this.monthComboBox.Size = new System.Drawing.Size(67, 23);
			this.monthComboBox.TabIndex = 4;
			this.monthComboBox.SelectedIndexChanged += new System.EventHandler(this.monthComboBox_SelectedIndexChanged);
			// 
			// yearComboBox
			// 
			this.yearComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.yearComboBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.yearComboBox.FormattingEnabled = true;
			this.yearComboBox.Items.AddRange(new object[] {
            "2006",
            "2007",
            "2008",
            "2009",
            "2010"});
			this.yearComboBox.Location = new System.Drawing.Point(16, 26);
			this.yearComboBox.Name = "yearComboBox";
			this.yearComboBox.Size = new System.Drawing.Size(88, 23);
			this.yearComboBox.TabIndex = 3;
			// 
			// onAirTimeGroup
			// 
			this.onAirTimeGroup.Controls.Add(this.label7);
			this.onAirTimeGroup.Controls.Add(this.minuteComboBox);
			this.onAirTimeGroup.Controls.Add(this.label6);
			this.onAirTimeGroup.Controls.Add(this.hourComboBox);
			this.onAirTimeGroup.Controls.Add(this.label2);
			this.onAirTimeGroup.Controls.Add(this.dayOfWeekComboBox);
			this.onAirTimeGroup.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.onAirTimeGroup.Location = new System.Drawing.Point(29, 127);
			this.onAirTimeGroup.Name = "onAirTimeGroup";
			this.onAirTimeGroup.Size = new System.Drawing.Size(309, 85);
			this.onAirTimeGroup.TabIndex = 12;
			this.onAirTimeGroup.TabStop = false;
			this.onAirTimeGroup.Text = "放送時刻(※)";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label7.Location = new System.Drawing.Point(13, 56);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(227, 15);
			this.label7.TabIndex = 11;
			this.label7.Text = "午前0～3時は前日の24～27時扱い";
			// 
			// minuteComboBox
			// 
			this.minuteComboBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.minuteComboBox.FormattingEnabled = true;
			this.minuteComboBox.Items.AddRange(new object[] {
            "00",
            "05",
            "10",
            "15",
            "20",
            "30",
            "40",
            "45",
            "50",
            "55"});
			this.minuteComboBox.Location = new System.Drawing.Point(221, 30);
			this.minuteComboBox.Name = "minuteComboBox";
			this.minuteComboBox.Size = new System.Drawing.Size(48, 23);
			this.minuteComboBox.TabIndex = 10;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label6.Location = new System.Drawing.Point(205, 33);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(10, 15);
			this.label6.TabIndex = 9;
			this.label6.Text = ":";
			// 
			// hourComboBox
			// 
			this.hourComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.hourComboBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.hourComboBox.FormattingEnabled = true;
			this.hourComboBox.Items.AddRange(new object[] {
            "24",
            "25",
            "26",
            "27",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
			this.hourComboBox.Location = new System.Drawing.Point(148, 30);
			this.hourComboBox.Name = "hourComboBox";
			this.hourComboBox.Size = new System.Drawing.Size(48, 23);
			this.hourComboBox.TabIndex = 8;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label2.Location = new System.Drawing.Point(89, 33);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 15);
			this.label2.TabIndex = 7;
			this.label2.Text = "曜日";
			// 
			// dayOfWeekComboBox
			// 
			this.dayOfWeekComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.dayOfWeekComboBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.dayOfWeekComboBox.FormattingEnabled = true;
			this.dayOfWeekComboBox.Items.AddRange(new object[] {
            "日",
            "月",
            "火",
            "水",
            "木",
            "金",
            "土"});
			this.dayOfWeekComboBox.Location = new System.Drawing.Point(16, 30);
			this.dayOfWeekComboBox.Name = "dayOfWeekComboBox";
			this.dayOfWeekComboBox.Size = new System.Drawing.Size(67, 23);
			this.dayOfWeekComboBox.TabIndex = 6;
			this.dayOfWeekComboBox.SelectedIndexChanged += new System.EventHandler(this.dayOfWeekComboBox_SelectedIndexChanged);
			// 
			// undecidedRadioBox
			// 
			this.undecidedRadioBox.AutoSize = true;
			this.undecidedRadioBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.undecidedRadioBox.Location = new System.Drawing.Point(23, 68);
			this.undecidedRadioBox.Name = "undecidedRadioBox";
			this.undecidedRadioBox.Size = new System.Drawing.Size(287, 19);
			this.undecidedRadioBox.TabIndex = 13;
			this.undecidedRadioBox.Text = "手動で入力するので、全て未定にしておく(U)";
			this.undecidedRadioBox.UseVisualStyleBackColor = true;
			this.undecidedRadioBox.CheckedChanged += new System.EventHandler(this.undecidedRadioBox_CheckedChanged);
			// 
			// serialRadioBox
			// 
			this.serialRadioBox.AutoSize = true;
			this.serialRadioBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.serialRadioBox.Location = new System.Drawing.Point(23, 93);
			this.serialRadioBox.Name = "serialRadioBox";
			this.serialRadioBox.Size = new System.Drawing.Size(273, 19);
			this.serialRadioBox.TabIndex = 14;
			this.serialRadioBox.Text = "第1話から最終話まで毎週定時に放送(&E)";
			this.serialRadioBox.UseVisualStyleBackColor = true;
			this.serialRadioBox.CheckedChanged += new System.EventHandler(this.serialRadioBox_CheckedChanged);
			// 
			// okButton
			// 
			this.okButton.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.okButton.Location = new System.Drawing.Point(244, 301);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(145, 33);
			this.okButton.TabIndex = 15;
			this.okButton.Text = "&Ok";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// autoRadioBox
			// 
			this.autoRadioBox.AutoSize = true;
			this.autoRadioBox.Checked = true;
			this.autoRadioBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.autoRadioBox.Location = new System.Drawing.Point(23, 43);
			this.autoRadioBox.Name = "autoRadioBox";
			this.autoRadioBox.Size = new System.Drawing.Size(382, 19);
			this.autoRadioBox.TabIndex = 16;
			this.autoRadioBox.TabStop = true;
			this.autoRadioBox.Text = "オンラインデータ－ベースを利用して自動設定(&O) (推奨)";
			this.autoRadioBox.UseVisualStyleBackColor = true;
			this.autoRadioBox.CheckedChanged += new System.EventHandler(this.autoRadioBox_CheckedChanged);
			// 
			// AnimeScheduleDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(413, 349);
			this.Controls.Add(this.autoRadioBox);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.serialRadioBox);
			this.Controls.Add(this.undecidedRadioBox);
			this.Controls.Add(this.firstOnAirGroup);
			this.Controls.Add(this.onAirTimeGroup);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AnimeScheduleDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "録画日時の一括設定";
			this.Load += new System.EventHandler(this.AnimeScheduleDialog_Load);
			this.firstOnAirGroup.ResumeLayout(false);
			this.firstOnAirGroup.PerformLayout();
			this.onAirTimeGroup.ResumeLayout(false);
			this.onAirTimeGroup.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox firstOnAirGroup;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox dayComboBox;
		private System.Windows.Forms.ComboBox monthComboBox;
		private System.Windows.Forms.ComboBox yearComboBox;
		private System.Windows.Forms.GroupBox onAirTimeGroup;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox minuteComboBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox hourComboBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox dayOfWeekComboBox;
		private System.Windows.Forms.RadioButton undecidedRadioBox;
		private System.Windows.Forms.RadioButton serialRadioBox;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.RadioButton autoRadioBox;
	}
}