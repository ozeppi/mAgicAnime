namespace magicAnime
{
	partial class SyoboiListDlg
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
			this.syoboiListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.tidColumn = new System.Windows.Forms.ColumnHeader();
			this.conditionComboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.googleButton = new System.Windows.Forms.Button();
			this.filterTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// syoboiListView
			// 
			this.syoboiListView.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.tidColumn} );
			this.syoboiListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.syoboiListView.HideSelection = false;
			this.syoboiListView.Location = new System.Drawing.Point( 18, 76 );
			this.syoboiListView.MultiSelect = false;
			this.syoboiListView.Name = "syoboiListView";
			this.syoboiListView.Size = new System.Drawing.Size( 395, 268 );
			this.syoboiListView.TabIndex = 2;
			this.syoboiListView.UseCompatibleStateImageBehavior = false;
			this.syoboiListView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "番組タイトル";
			this.columnHeader1.Width = 230;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "初回放送";
			this.columnHeader2.Width = 90;
			// 
			// tidColumn
			// 
			this.tidColumn.Text = "TID";
			// 
			// conditionComboBox
			// 
			this.conditionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.conditionComboBox.FormattingEnabled = true;
			this.conditionComboBox.Items.AddRange( new object[] {
            "新番組と放送中の番組全て",
            "今月から放送開始のみ",
            "来月から放送開始のみ"} );
			this.conditionComboBox.Location = new System.Drawing.Point( 97, 18 );
			this.conditionComboBox.Name = "conditionComboBox";
			this.conditionComboBox.Size = new System.Drawing.Size( 316, 23 );
			this.conditionComboBox.TabIndex = 3;
			this.conditionComboBox.SelectedIndexChanged += new System.EventHandler( this.conditionComboBox_SelectedIndexChanged );
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point( 19, 21 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 59, 15 );
			this.label1.TabIndex = 4;
			this.label1.Text = "表示(&V):";
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point( 432, 18 );
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size( 131, 33 );
			this.okButton.TabIndex = 5;
			this.okButton.Text = "&OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler( this.okButton_Click );
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point( 432, 57 );
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size( 131, 33 );
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = "キャンセル";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler( this.cancelButton_Click );
			// 
			// googleButton
			// 
			this.googleButton.Location = new System.Drawing.Point( 432, 123 );
			this.googleButton.Name = "googleButton";
			this.googleButton.Size = new System.Drawing.Size( 131, 33 );
			this.googleButton.TabIndex = 7;
			this.googleButton.Text = "Google検索";
			this.googleButton.UseVisualStyleBackColor = true;
			this.googleButton.Visible = false;
			this.googleButton.Click += new System.EventHandler( this.googleButton_Click );
			// 
			// filterTextBox
			// 
			this.filterTextBox.Location = new System.Drawing.Point( 97, 47 );
			this.filterTextBox.MaxLength = 32;
			this.filterTextBox.Name = "filterTextBox";
			this.filterTextBox.Size = new System.Drawing.Size( 315, 22 );
			this.filterTextBox.TabIndex = 8;
			this.filterTextBox.TextChanged += new System.EventHandler( this.filterTextBox_TextChanged );
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point( 19, 50 );
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size( 72, 15 );
			this.label2.TabIndex = 9;
			this.label2.Text = "絞込み(&S):";
			// 
			// SyoboiListDlg
			// 
			this.ClientSize = new System.Drawing.Size( 575, 356 );
			this.Controls.Add( this.label2 );
			this.Controls.Add( this.filterTextBox );
			this.Controls.Add( this.googleButton );
			this.Controls.Add( this.cancelButton );
			this.Controls.Add( this.okButton );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.conditionComboBox );
			this.Controls.Add( this.syoboiListView );
			this.Font = new System.Drawing.Font( "MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 128 ) ) );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SyoboiListDlg";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "番組の選択";
			this.Load += new System.EventHandler( this.SyoboiListDlg_Load );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView syoboiListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader tidColumn;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ComboBox conditionComboBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button googleButton;
		private System.Windows.Forms.TextBox filterTextBox;
		private System.Windows.Forms.Label label2;
	}
}