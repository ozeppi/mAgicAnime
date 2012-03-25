namespace magicAnime.UserInterface
{
	partial class SortDialog
	{
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows フォーム デザイナで生成されたコード

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.orderNextonairRadioButton = new System.Windows.Forms.RadioButton();
			this.firstOrderGroupBox = new System.Windows.Forms.GroupBox();
			this.limit1CoursCheckBox = new System.Windows.Forms.CheckBox();
			this.saturdayRadioButton = new System.Windows.Forms.RadioButton();
			this.sundayRadioButton = new System.Windows.Forms.RadioButton();
			this.sortMethodRadioButton = new System.Windows.Forms.RadioButton();
			this.sortButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.lastCheckBox = new System.Windows.Forms.CheckBox();
			this.firstOrderGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// orderNextonairRadioButton
			// 
			this.orderNextonairRadioButton.AutoSize = true;
			this.orderNextonairRadioButton.Location = new System.Drawing.Point( 31, 120 );
			this.orderNextonairRadioButton.Name = "orderNextonairRadioButton";
			this.orderNextonairRadioButton.Size = new System.Drawing.Size( 168, 17 );
			this.orderNextonairRadioButton.TabIndex = 1;
			this.orderNextonairRadioButton.Text = "次回放送日でソートする(&N)";
			this.orderNextonairRadioButton.UseVisualStyleBackColor = true;
			// 
			// firstOrderGroupBox
			// 
			this.firstOrderGroupBox.Controls.Add( this.limit1CoursCheckBox );
			this.firstOrderGroupBox.Controls.Add( this.saturdayRadioButton );
			this.firstOrderGroupBox.Controls.Add( this.sundayRadioButton );
			this.firstOrderGroupBox.Location = new System.Drawing.Point( 22, 20 );
			this.firstOrderGroupBox.Name = "firstOrderGroupBox";
			this.firstOrderGroupBox.Size = new System.Drawing.Size( 325, 84 );
			this.firstOrderGroupBox.TabIndex = 2;
			this.firstOrderGroupBox.TabStop = false;
			// 
			// limit1CoursCheckBox
			// 
			this.limit1CoursCheckBox.AutoSize = true;
			this.limit1CoursCheckBox.Location = new System.Drawing.Point( 25, 58 );
			this.limit1CoursCheckBox.Name = "limit1CoursCheckBox";
			this.limit1CoursCheckBox.Size = new System.Drawing.Size( 200, 17 );
			this.limit1CoursCheckBox.TabIndex = 2;
			this.limit1CoursCheckBox.Text = "時刻の平均を最新13話に限る(&L)";
			this.limit1CoursCheckBox.UseVisualStyleBackColor = true;
			// 
			// saturdayRadioButton
			// 
			this.saturdayRadioButton.AutoSize = true;
			this.saturdayRadioButton.Enabled = false;
			this.saturdayRadioButton.Location = new System.Drawing.Point( 160, 27 );
			this.saturdayRadioButton.Name = "saturdayRadioButton";
			this.saturdayRadioButton.Size = new System.Drawing.Size( 106, 17 );
			this.saturdayRadioButton.TabIndex = 1;
			this.saturdayRadioButton.Text = "土曜日基準(&T)";
			this.saturdayRadioButton.UseVisualStyleBackColor = true;
			// 
			// sundayRadioButton
			// 
			this.sundayRadioButton.AutoSize = true;
			this.sundayRadioButton.Checked = true;
			this.sundayRadioButton.Enabled = false;
			this.sundayRadioButton.Location = new System.Drawing.Point( 25, 27 );
			this.sundayRadioButton.Name = "sundayRadioButton";
			this.sundayRadioButton.Size = new System.Drawing.Size( 106, 17 );
			this.sundayRadioButton.TabIndex = 0;
			this.sundayRadioButton.TabStop = true;
			this.sundayRadioButton.Text = "日曜日基準(&S)";
			this.sundayRadioButton.UseVisualStyleBackColor = true;
			// 
			// sortMethodRadioButton
			// 
			this.sortMethodRadioButton.AutoSize = true;
			this.sortMethodRadioButton.Checked = true;
			this.sortMethodRadioButton.Location = new System.Drawing.Point( 31, 18 );
			this.sortMethodRadioButton.Name = "sortMethodRadioButton";
			this.sortMethodRadioButton.Size = new System.Drawing.Size( 251, 17 );
			this.sortMethodRadioButton.TabIndex = 3;
			this.sortMethodRadioButton.TabStop = true;
			this.sortMethodRadioButton.Text = "全話の曜日と時刻の平均を基準にソート(&D)";
			this.sortMethodRadioButton.UseVisualStyleBackColor = true;
			this.sortMethodRadioButton.CheckedChanged += new System.EventHandler( this.sortMethodRadioButton_CheckedChanged );
			// 
			// sortButton
			// 
			this.sortButton.Location = new System.Drawing.Point( 373, 19 );
			this.sortButton.Name = "sortButton";
			this.sortButton.Size = new System.Drawing.Size( 123, 35 );
			this.sortButton.TabIndex = 4;
			this.sortButton.Text = "ソート(&O)";
			this.sortButton.UseVisualStyleBackColor = true;
			this.sortButton.Click += new System.EventHandler( this.sortButton_Click );
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point( 373, 60 );
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size( 123, 35 );
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "キャンセル(&C)";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler( this.cancelButton_Click );
			// 
			// lastCheckBox
			// 
			this.lastCheckBox.AutoSize = true;
			this.lastCheckBox.Checked = true;
			this.lastCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.lastCheckBox.Location = new System.Drawing.Point( 22, 155 );
			this.lastCheckBox.Name = "lastCheckBox";
			this.lastCheckBox.Size = new System.Drawing.Size( 219, 17 );
			this.lastCheckBox.TabIndex = 6;
			this.lastCheckBox.Text = "放送終了した番組は末尾に並べる(&L)";
			this.lastCheckBox.UseVisualStyleBackColor = true;
			// 
			// SortDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 7F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 506, 193 );
			this.Controls.Add( this.lastCheckBox );
			this.Controls.Add( this.cancelButton );
			this.Controls.Add( this.sortButton );
			this.Controls.Add( this.sortMethodRadioButton );
			this.Controls.Add( this.firstOrderGroupBox );
			this.Controls.Add( this.orderNextonairRadioButton );
			this.Font = new System.Drawing.Font( "MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 128 ) ) );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SortDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "番組のソート";
			this.Load += new System.EventHandler( this.SortDialog_Load );
			this.firstOrderGroupBox.ResumeLayout( false );
			this.firstOrderGroupBox.PerformLayout();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton orderNextonairRadioButton;
		private System.Windows.Forms.GroupBox firstOrderGroupBox;
		private System.Windows.Forms.RadioButton sortMethodRadioButton;
		private System.Windows.Forms.RadioButton saturdayRadioButton;
		private System.Windows.Forms.RadioButton sundayRadioButton;
		private System.Windows.Forms.Button sortButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.CheckBox lastCheckBox;
		private System.Windows.Forms.CheckBox limit1CoursCheckBox;
	}
}