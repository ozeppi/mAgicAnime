namespace magicAnime
{
	partial class MainForm
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.filePictureBox = new System.Windows.Forms.PictureBox();
			this.titlePictureBox = new System.Windows.Forms.PictureBox();
			this.statusPictureBox = new System.Windows.Forms.PictureBox();
			this.datePictureBox = new System.Windows.Forms.PictureBox();
			this.dataGrid = new System.Windows.Forms.DataGridView();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnStoryCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.NewAnimeMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.DeleteAnimeMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.programPropertyMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.RefreshMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.ForceRefreshMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.SortMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.OnReleaseUnreadMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.StoreAllMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.OptionMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.LogShowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.ExitMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.PowerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.BatchEncodeAllMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.BatchListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.JobsCancelMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.EncodingNothingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.BatchListNothingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
			this.AutoShutdownMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.ヘルプHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
			this.applicationDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.AboutMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.debugMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.debugShutdownMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
			this.debugForceEmptyMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.statusBar = new System.Windows.Forms.StatusStrip();
			this.logStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.todayOnAirLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.storeFolderLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.recordDriveFreeSpaceLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.playMovieMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.reserveMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.cancelReserveMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.extToolsGroupSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.拡張ツールToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.encodeGroupSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.encodeMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.storeMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
			this.renameFileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.unreadMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.RecordPropertyMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.panel3 = new System.Windows.Forms.Panel();
			this.titleLabel = new System.Windows.Forms.Label();
			this.RecordStateLabel = new System.Windows.Forms.Label();
			this.dateTimeLabel = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.filePathLabel = new System.Windows.Forms.Label();
			this.mainToolBar = new System.Windows.Forms.ToolStrip();
			this.newAnimeButton = new System.Windows.Forms.ToolStripButton();
			this.propertyButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.refreshButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.StoreAllButton = new System.Windows.Forms.ToolStripButton();
			this.playUnreadButton = new System.Windows.Forms.ToolStripSplitButton();
			this.unreadListMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.changeResolutionButton = new System.Windows.Forms.ToolStripButton();
			this.seriesModeMenu = new System.Windows.Forms.ToolStripButton();
			this.calenderModeMenu = new System.Windows.Forms.ToolStripSplitButton();
			this.weekModeMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.dayModeMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
			this.calenderModeOptionMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.thumbnailModeButton = new System.Windows.Forms.ToolStripButton();
			this.logButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.viewSplitContainer = new System.Windows.Forms.SplitContainer();
			this.logListBox = new System.Windows.Forms.ListBox();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			((System.ComponentModel.ISupportInitialize)(this.filePictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.titlePictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusPictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.datePictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
			this.menuStrip.SuspendLayout();
			this.statusBar.SuspendLayout();
			this.contextMenuStrip.SuspendLayout();
			this.panel3.SuspendLayout();
			this.mainToolBar.SuspendLayout();
			this.viewSplitContainer.Panel1.SuspendLayout();
			this.viewSplitContainer.Panel2.SuspendLayout();
			this.viewSplitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			label6.Location = new System.Drawing.Point(24, 8);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(69, 15);
			label6.TabIndex = 10;
			label6.Text = "タイトル名:";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			label7.Location = new System.Drawing.Point(24, 60);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(40, 15);
			label7.TabIndex = 5;
			label7.Text = "状態:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			label4.Location = new System.Drawing.Point(24, 32);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(67, 15);
			label4.TabIndex = 3;
			label4.Text = "ファイル名:";
			// 
			// toolStripMenuItem5
			// 
			toolStripMenuItem5.Name = "toolStripMenuItem5";
			toolStripMenuItem5.Size = new System.Drawing.Size(171, 6);
			// 
			// toolStripSeparator6
			// 
			toolStripSeparator6.Name = "toolStripSeparator6";
			toolStripSeparator6.Size = new System.Drawing.Size(171, 6);
			// 
			// filePictureBox
			// 
			this.filePictureBox.BackColor = System.Drawing.Color.Transparent;
			this.filePictureBox.Image = global::magicAnime.Properties.Resources.Title;
			this.filePictureBox.Location = new System.Drawing.Point(9, 32);
			this.filePictureBox.Name = "filePictureBox";
			this.filePictureBox.Size = new System.Drawing.Size(16, 16);
			this.filePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.filePictureBox.TabIndex = 12;
			this.filePictureBox.TabStop = false;
			// 
			// titlePictureBox
			// 
			this.titlePictureBox.BackColor = System.Drawing.Color.Transparent;
			this.titlePictureBox.Image = global::magicAnime.Properties.Resources.Movie;
			this.titlePictureBox.Location = new System.Drawing.Point(9, 8);
			this.titlePictureBox.Name = "titlePictureBox";
			this.titlePictureBox.Size = new System.Drawing.Size(16, 16);
			this.titlePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.titlePictureBox.TabIndex = 13;
			this.titlePictureBox.TabStop = false;
			// 
			// statusPictureBox
			// 
			this.statusPictureBox.BackColor = System.Drawing.Color.Transparent;
			this.statusPictureBox.Image = global::magicAnime.Properties.Resources.Flag;
			this.statusPictureBox.Location = new System.Drawing.Point(9, 59);
			this.statusPictureBox.Name = "statusPictureBox";
			this.statusPictureBox.Size = new System.Drawing.Size(16, 16);
			this.statusPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.statusPictureBox.TabIndex = 14;
			this.statusPictureBox.TabStop = false;
			// 
			// datePictureBox
			// 
			this.datePictureBox.BackColor = System.Drawing.Color.Transparent;
			this.datePictureBox.Image = global::magicAnime.Properties.Resources.DateTime;
			this.datePictureBox.Location = new System.Drawing.Point(311, 59);
			this.datePictureBox.Name = "datePictureBox";
			this.datePictureBox.Size = new System.Drawing.Size(16, 16);
			this.datePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.datePictureBox.TabIndex = 15;
			this.datePictureBox.TabStop = false;
			// 
			// dataGrid
			// 
			this.dataGrid.AllowUserToAddRows = false;
			this.dataGrid.AllowUserToDeleteRows = false;
			this.dataGrid.AllowUserToResizeRows = false;
			this.dataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dataGrid.BackgroundColor = System.Drawing.SystemColors.Window;
			this.dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
			this.dataGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGrid.ColumnHeadersHeight = 24;
			this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.ColumnTime,
            this.Column3,
            this.Column2,
            this.ColumnStoryCount});
			this.dataGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.dataGrid.Location = new System.Drawing.Point(0, 3);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.ReadOnly = true;
			this.dataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.dataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
			this.dataGrid.RowTemplate.Height = 21;
			this.dataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dataGrid.ShowCellToolTips = false;
			this.dataGrid.ShowEditingIcon = false;
			this.dataGrid.Size = new System.Drawing.Size(750, 286);
			this.dataGrid.TabIndex = 0;
			this.dataGrid.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGrid_CellMouseUp);
			this.dataGrid.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellLeave);
			this.dataGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellDoubleClick);
			this.dataGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGrid_MouseMove);
			this.dataGrid.CellStateChanged += new System.Windows.Forms.DataGridViewCellStateChangedEventHandler(this.dataGrid_CellStateChanged);
			this.dataGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGrid_MouseUp);
			this.dataGrid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGrid_CellPainting);
			this.dataGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellClick);
			this.dataGrid.SelectionChanged += new System.EventHandler(this.dataGrid_SelectionChanged);
			// 
			// Column1
			// 
			this.Column1.Frozen = true;
			this.Column1.HeaderText = "タイトル";
			this.Column1.MaxInputLength = 250;
			this.Column1.Name = "Column1";
			this.Column1.ReadOnly = true;
			this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Column1.Width = 200;
			// 
			// ColumnTime
			// 
			this.ColumnTime.HeaderText = "次回放送日";
			this.ColumnTime.Name = "ColumnTime";
			this.ColumnTime.ReadOnly = true;
			this.ColumnTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.ColumnTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.ColumnTime.Width = 120;
			// 
			// Column3
			// 
			this.Column3.HeaderText = "視聴局";
			this.Column3.Name = "Column3";
			this.Column3.ReadOnly = true;
			this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// Column2
			// 
			this.Column2.HeaderText = "再エンコード";
			this.Column2.Name = "Column2";
			this.Column2.ReadOnly = true;
			this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// ColumnStoryCount
			// 
			this.ColumnStoryCount.HeaderText = "全話数";
			this.ColumnStoryCount.Name = "ColumnStoryCount";
			this.ColumnStoryCount.ReadOnly = true;
			this.ColumnStoryCount.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.ColumnStoryCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.ColumnStoryCount.Width = 40;
			// 
			// menuStrip
			// 
			this.menuStrip.BackColor = System.Drawing.SystemColors.Menu;
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.PowerMenuItem,
            this.ヘルプHToolStripMenuItem,
            this.debugMenu});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Padding = new System.Windows.Forms.Padding(6, 2, 0, 4);
			this.menuStrip.Size = new System.Drawing.Size(750, 24);
			this.menuStrip.TabIndex = 1;
			this.menuStrip.Text = "menuStrip";
			// 
			// fileMenu
			// 
			this.fileMenu.BackColor = System.Drawing.SystemColors.Menu;
			this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewAnimeMenu,
            this.DeleteAnimeMenu,
            this.programPropertyMenu,
            this.toolStripMenuItem1,
            this.RefreshMenu,
            this.ForceRefreshMenu,
            this.toolStripSeparator1,
            this.SortMenu,
            this.OnReleaseUnreadMenu,
            this.toolStripMenuItem4,
            this.StoreAllMenu,
            this.toolStripMenuItem2,
            this.OptionMenu,
            this.LogShowMenuItem,
            this.toolStripMenuItem3,
            this.ExitMenu});
			this.fileMenu.ForeColor = System.Drawing.SystemColors.ControlText;
			this.fileMenu.Name = "fileMenu";
			this.fileMenu.Size = new System.Drawing.Size(71, 18);
			this.fileMenu.Text = "ファイル(&F)";
			// 
			// NewAnimeMenu
			// 
			this.NewAnimeMenu.Image = global::magicAnime.Properties.Resources.NewProgram;
			this.NewAnimeMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.NewAnimeMenu.Name = "NewAnimeMenu";
			this.NewAnimeMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.NewAnimeMenu.Size = new System.Drawing.Size(256, 22);
			this.NewAnimeMenu.Text = "新しい番組(&N)";
			this.NewAnimeMenu.Click += new System.EventHandler(this.NewAnimeMenu_Click);
			// 
			// DeleteAnimeMenu
			// 
			this.DeleteAnimeMenu.Image = ((System.Drawing.Image)(resources.GetObject("DeleteAnimeMenu.Image")));
			this.DeleteAnimeMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.DeleteAnimeMenu.Name = "DeleteAnimeMenu";
			this.DeleteAnimeMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
			this.DeleteAnimeMenu.Size = new System.Drawing.Size(256, 22);
			this.DeleteAnimeMenu.Text = "番組の削除(&D)";
			this.DeleteAnimeMenu.Click += new System.EventHandler(this.DeleteAnimeMenu_Click);
			// 
			// programPropertyMenu
			// 
			this.programPropertyMenu.Image = global::magicAnime.Properties.Resources.ProgramProperty;
			this.programPropertyMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.programPropertyMenu.Name = "programPropertyMenu";
			this.programPropertyMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
			this.programPropertyMenu.Size = new System.Drawing.Size(256, 22);
			this.programPropertyMenu.Text = "番組のプロパティ(&P)";
			this.programPropertyMenu.Click += new System.EventHandler(this.programPropertyMenu_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(253, 6);
			// 
			// RefreshMenu
			// 
			this.RefreshMenu.Image = global::magicAnime.Properties.Resources.Update;
			this.RefreshMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.RefreshMenu.Name = "RefreshMenu";
			this.RefreshMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.RefreshMenu.Size = new System.Drawing.Size(256, 22);
			this.RefreshMenu.Text = "新着データ更新(&R)";
			this.RefreshMenu.Click += new System.EventHandler(this.RefreshMenu_Clicked);
			// 
			// ForceRefreshMenu
			// 
			this.ForceRefreshMenu.Image = global::magicAnime.Properties.Resources.FullUpdate;
			this.ForceRefreshMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.ForceRefreshMenu.Name = "ForceRefreshMenu";
			this.ForceRefreshMenu.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.R)));
			this.ForceRefreshMenu.Size = new System.Drawing.Size(256, 22);
			this.ForceRefreshMenu.Text = "完全データ更新(&E)";
			this.ForceRefreshMenu.Click += new System.EventHandler(this.ForceRefreshMenu_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(253, 6);
			// 
			// SortMenu
			// 
			this.SortMenu.Name = "SortMenu";
			this.SortMenu.Size = new System.Drawing.Size(256, 22);
			this.SortMenu.Text = "番組をソート(&S)";
			this.SortMenu.Click += new System.EventHandler(this.SortMenu_Click);
			// 
			// OnReleaseUnreadMenu
			// 
			this.OnReleaseUnreadMenu.Name = "OnReleaseUnreadMenu";
			this.OnReleaseUnreadMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.OnReleaseUnreadMenu.Size = new System.Drawing.Size(256, 22);
			this.OnReleaseUnreadMenu.Text = "全ての未読フラグを解除(&F)";
			this.OnReleaseUnreadMenu.Click += new System.EventHandler(this.OnReleaseUnreadMenu_Click);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(253, 6);
			this.toolStripMenuItem4.Visible = false;
			// 
			// StoreAllMenu
			// 
			this.StoreAllMenu.Image = global::magicAnime.Properties.Resources.SaveAll;
			this.StoreAllMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.StoreAllMenu.Name = "StoreAllMenu";
			this.StoreAllMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
			this.StoreAllMenu.Size = new System.Drawing.Size(256, 22);
			this.StoreAllMenu.Text = "最終保存先へ転送(&M)";
			this.StoreAllMenu.Click += new System.EventHandler(this.StoreAllMenu_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(253, 6);
			// 
			// OptionMenu
			// 
			this.OptionMenu.Name = "OptionMenu";
			this.OptionMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.OptionMenu.Size = new System.Drawing.Size(256, 22);
			this.OptionMenu.Text = "オプション(&O)";
			this.OptionMenu.Click += new System.EventHandler(this.OptionMenu_Click);
			// 
			// LogShowMenuItem
			// 
			this.LogShowMenuItem.Name = "LogShowMenuItem";
			this.LogShowMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
			this.LogShowMenuItem.Size = new System.Drawing.Size(256, 22);
			this.LogShowMenuItem.Text = "ログ表示(&L)";
			this.LogShowMenuItem.Click += new System.EventHandler(this.LogShowMenuItem_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(253, 6);
			// 
			// ExitMenu
			// 
			this.ExitMenu.Name = "ExitMenu";
			this.ExitMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.ExitMenu.Size = new System.Drawing.Size(256, 22);
			this.ExitMenu.Text = "終了(&X)";
			this.ExitMenu.Click += new System.EventHandler(this.ExitMenu_Click);
			// 
			// PowerMenuItem
			// 
			this.PowerMenuItem.BackColor = System.Drawing.SystemColors.Menu;
			this.PowerMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BatchEncodeAllMenu,
            this.BatchListMenuItem,
            this.toolStripMenuItem7,
            this.AutoShutdownMenu});
			this.PowerMenuItem.Name = "PowerMenuItem";
			this.PowerMenuItem.Size = new System.Drawing.Size(83, 18);
			this.PowerMenuItem.Text = "エンコード(&E)";
			this.PowerMenuItem.DropDownOpened += new System.EventHandler(this.PowerMenuItem_DropDownOpened);
			this.PowerMenuItem.Click += new System.EventHandler(this.PowerMenuItem_Click);
			// 
			// BatchEncodeAllMenu
			// 
			this.BatchEncodeAllMenu.Name = "BatchEncodeAllMenu";
			this.BatchEncodeAllMenu.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.B)));
			this.BatchEncodeAllMenu.Size = new System.Drawing.Size(354, 22);
			this.BatchEncodeAllMenu.Text = "未処理分をエンコードキューに入れる(&E)";
			this.BatchEncodeAllMenu.Click += new System.EventHandler(this.BatchEncodeAllMenu_Click);
			// 
			// BatchListMenuItem
			// 
			this.BatchListMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.JobsCancelMenu,
            toolStripMenuItem5,
            this.EncodingNothingMenuItem,
            toolStripSeparator6,
            this.BatchListNothingMenuItem});
			this.BatchListMenuItem.Name = "BatchListMenuItem";
			this.BatchListMenuItem.Size = new System.Drawing.Size(354, 22);
			this.BatchListMenuItem.Text = "バッチエンコードキュー";
			// 
			// JobsCancelMenu
			// 
			this.JobsCancelMenu.Name = "JobsCancelMenu";
			this.JobsCancelMenu.Size = new System.Drawing.Size(174, 22);
			this.JobsCancelMenu.Text = "全てキャンセル";
			this.JobsCancelMenu.Click += new System.EventHandler(this.JobsCancelMenu_Click);
			// 
			// EncodingNothingMenuItem
			// 
			this.EncodingNothingMenuItem.Enabled = false;
			this.EncodingNothingMenuItem.Name = "EncodingNothingMenuItem";
			this.EncodingNothingMenuItem.Size = new System.Drawing.Size(174, 22);
			this.EncodingNothingMenuItem.Text = "(エンコード中なし)";
			this.EncodingNothingMenuItem.Visible = false;
			// 
			// BatchListNothingMenuItem
			// 
			this.BatchListNothingMenuItem.Enabled = false;
			this.BatchListNothingMenuItem.Name = "BatchListNothingMenuItem";
			this.BatchListNothingMenuItem.Size = new System.Drawing.Size(174, 22);
			this.BatchListNothingMenuItem.Text = "(エンコード待ちなし)";
			this.BatchListNothingMenuItem.Visible = false;
			// 
			// toolStripMenuItem7
			// 
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new System.Drawing.Size(351, 6);
			// 
			// AutoShutdownMenu
			// 
			this.AutoShutdownMenu.Name = "AutoShutdownMenu";
			this.AutoShutdownMenu.Size = new System.Drawing.Size(354, 22);
			this.AutoShutdownMenu.Text = "エンコード完了時にシャットダウン(&S)";
			this.AutoShutdownMenu.Click += new System.EventHandler(this.AutoShutdownMenu_Click);
			// 
			// ヘルプHToolStripMenuItem
			// 
			this.ヘルプHToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpMenu,
            this.toolStripMenuItem8,
            this.applicationDataMenuItem,
            this.toolStripSeparator7,
            this.AboutMenu});
			this.ヘルプHToolStripMenuItem.Name = "ヘルプHToolStripMenuItem";
			this.ヘルプHToolStripMenuItem.Size = new System.Drawing.Size(67, 18);
			this.ヘルプHToolStripMenuItem.Text = "ヘルプ(&H)";
			// 
			// helpMenu
			// 
			this.helpMenu.Name = "helpMenu";
			this.helpMenu.Size = new System.Drawing.Size(182, 22);
			this.helpMenu.Text = "公式サイト(&W)";
			this.helpMenu.Click += new System.EventHandler(this.helpMenu_Click);
			// 
			// toolStripMenuItem8
			// 
			this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			this.toolStripMenuItem8.Size = new System.Drawing.Size(179, 6);
			// 
			// applicationDataMenuItem
			// 
			this.applicationDataMenuItem.Name = "applicationDataMenuItem";
			this.applicationDataMenuItem.Size = new System.Drawing.Size(182, 22);
			this.applicationDataMenuItem.Text = "アプリケーションデータ";
			this.applicationDataMenuItem.Click += new System.EventHandler(this.applicationDataMenuItem_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(179, 6);
			// 
			// AboutMenu
			// 
			this.AboutMenu.Name = "AboutMenu";
			this.AboutMenu.Size = new System.Drawing.Size(182, 22);
			this.AboutMenu.Text = "バージョン情報(&A)...";
			this.AboutMenu.Click += new System.EventHandler(this.AboutMenu_Click);
			// 
			// debugMenu
			// 
			this.debugMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugShutdownMenu,
            this.toolStripMenuItem10,
            this.debugForceEmptyMenu});
			this.debugMenu.Name = "debugMenu";
			this.debugMenu.Size = new System.Drawing.Size(75, 18);
			this.debugMenu.Text = "デバッグ(&D)";
			this.debugMenu.Visible = false;
			this.debugMenu.DropDownOpening += new System.EventHandler(this.debugMenu_DropDownOpening);
			// 
			// debugShutdownMenu
			// 
			this.debugShutdownMenu.Name = "debugShutdownMenu";
			this.debugShutdownMenu.Size = new System.Drawing.Size(331, 22);
			this.debugShutdownMenu.Text = "シャットダウンテスト";
			this.debugShutdownMenu.Click += new System.EventHandler(this.debugShutdownMenu_Click);
			// 
			// toolStripMenuItem10
			// 
			this.toolStripMenuItem10.Name = "toolStripMenuItem10";
			this.toolStripMenuItem10.Size = new System.Drawing.Size(328, 6);
			// 
			// debugForceEmptyMenu
			// 
			this.debugForceEmptyMenu.Name = "debugForceEmptyMenu";
			this.debugForceEmptyMenu.Size = new System.Drawing.Size(331, 22);
			this.debugForceEmptyMenu.Text = "更新時、しょぼかる放送データを強制的に空にする";
			this.debugForceEmptyMenu.Click += new System.EventHandler(this.debugForceEmptyMenu_Click);
			// 
			// statusBar
			// 
			this.statusBar.AllowMerge = false;
			this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logStatusLabel,
            this.toolStripStatusLabel1,
            this.todayOnAirLabel,
            this.storeFolderLabel,
            this.recordDriveFreeSpaceLabel});
			this.statusBar.Location = new System.Drawing.Point(0, 571);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(750, 29);
			this.statusBar.Stretch = false;
			this.statusBar.TabIndex = 2;
			this.statusBar.Text = "statusStrip1";
			// 
			// logStatusLabel
			// 
			this.logStatusLabel.ActiveLinkColor = System.Drawing.SystemColors.ControlText;
			this.logStatusLabel.BackColor = System.Drawing.SystemColors.Control;
			this.logStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.logStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.logStatusLabel.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.logStatusLabel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.logStatusLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.logStatusLabel.Name = "logStatusLabel";
			this.logStatusLabel.Size = new System.Drawing.Size(447, 24);
			this.logStatusLabel.Spring = true;
			this.logStatusLabel.Text = "起動中・・・";
			this.logStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.logStatusLabel.Click += new System.EventHandler(this.logStatusLabel_Click);
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel1.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(94, 24);
			this.toolStripStatusLabel1.Text = "今度の放送";
			// 
			// todayOnAirLabel
			// 
			this.todayOnAirLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.todayOnAirLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.todayOnAirLabel.Name = "todayOnAirLabel";
			this.todayOnAirLabel.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
			this.todayOnAirLabel.Size = new System.Drawing.Size(24, 24);
			// 
			// storeFolderLabel
			// 
			this.storeFolderLabel.AutoToolTip = true;
			this.storeFolderLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.storeFolderLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.storeFolderLabel.Image = ((System.Drawing.Image)(resources.GetObject("storeFolderLabel.Image")));
			this.storeFolderLabel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.storeFolderLabel.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.storeFolderLabel.IsLink = true;
			this.storeFolderLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.storeFolderLabel.LinkColor = System.Drawing.SystemColors.HotTrack;
			this.storeFolderLabel.Name = "storeFolderLabel";
			this.storeFolderLabel.Padding = new System.Windows.Forms.Padding(10, 2, 10, 2);
			this.storeFolderLabel.Size = new System.Drawing.Size(86, 24);
			this.storeFolderLabel.Text = "保存先";
			this.storeFolderLabel.ToolTipText = "保存先";
			this.storeFolderLabel.Click += new System.EventHandler(this.storeFolderLabel_Click);
			// 
			// recordDriveFreeSpaceLabel
			// 
			this.recordDriveFreeSpaceLabel.AutoToolTip = true;
			this.recordDriveFreeSpaceLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.recordDriveFreeSpaceLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.recordDriveFreeSpaceLabel.Image = ((System.Drawing.Image)(resources.GetObject("recordDriveFreeSpaceLabel.Image")));
			this.recordDriveFreeSpaceLabel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.recordDriveFreeSpaceLabel.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.recordDriveFreeSpaceLabel.IsLink = true;
			this.recordDriveFreeSpaceLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.recordDriveFreeSpaceLabel.LinkColor = System.Drawing.SystemColors.HotTrack;
			this.recordDriveFreeSpaceLabel.Name = "recordDriveFreeSpaceLabel";
			this.recordDriveFreeSpaceLabel.Padding = new System.Windows.Forms.Padding(10, 2, 10, 2);
			this.recordDriveFreeSpaceLabel.Size = new System.Drawing.Size(84, 24);
			this.recordDriveFreeSpaceLabel.Text = "0 [MB]";
			this.recordDriveFreeSpaceLabel.ToolTipText = "録画フォルダの空き容量";
			this.recordDriveFreeSpaceLabel.Click += new System.EventHandler(this.recordDriveFreeSpaceLabel_Click);
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playMovieMenu,
            this.reserveMenu,
            this.cancelReserveMenu,
            this.extToolsGroupSeparator,
            this.拡張ツールToolStripMenuItem,
            this.encodeGroupSeparator,
            this.encodeMenu,
            this.storeMenu,
            this.toolStripMenuItem6,
            this.renameFileMenu,
            this.unreadMenu,
            this.RecordPropertyMenu});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(219, 220);
			// 
			// playMovieMenu
			// 
			this.playMovieMenu.Image = ((System.Drawing.Image)(resources.GetObject("playMovieMenu.Image")));
			this.playMovieMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.playMovieMenu.Name = "playMovieMenu";
			this.playMovieMenu.Size = new System.Drawing.Size(218, 22);
			this.playMovieMenu.Text = "再生(&P)";
			this.playMovieMenu.Click += new System.EventHandler(this.playMovieMenu_Click);
			// 
			// reserveMenu
			// 
			this.reserveMenu.Image = ((System.Drawing.Image)(resources.GetObject("reserveMenu.Image")));
			this.reserveMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.reserveMenu.Name = "reserveMenu";
			this.reserveMenu.Size = new System.Drawing.Size(218, 22);
			this.reserveMenu.Text = "録画予約(&R)";
			this.reserveMenu.Click += new System.EventHandler(this.reserveMenu_Click);
			// 
			// cancelReserveMenu
			// 
			this.cancelReserveMenu.Name = "cancelReserveMenu";
			this.cancelReserveMenu.Size = new System.Drawing.Size(218, 22);
			this.cancelReserveMenu.Text = "予約キャンセル";
			this.cancelReserveMenu.Click += new System.EventHandler(this.cancelReserveMenu_Click);
			// 
			// extToolsGroupSeparator
			// 
			this.extToolsGroupSeparator.Name = "extToolsGroupSeparator";
			this.extToolsGroupSeparator.Size = new System.Drawing.Size(215, 6);
			this.extToolsGroupSeparator.Visible = false;
			// 
			// 拡張ツールToolStripMenuItem
			// 
			this.拡張ツールToolStripMenuItem.Enabled = false;
			this.拡張ツールToolStripMenuItem.Name = "拡張ツールToolStripMenuItem";
			this.拡張ツールToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			this.拡張ツールToolStripMenuItem.Text = "拡張ツール";
			this.拡張ツールToolStripMenuItem.Visible = false;
			// 
			// encodeGroupSeparator
			// 
			this.encodeGroupSeparator.Name = "encodeGroupSeparator";
			this.encodeGroupSeparator.Size = new System.Drawing.Size(215, 6);
			// 
			// encodeMenu
			// 
			this.encodeMenu.Name = "encodeMenu";
			this.encodeMenu.Size = new System.Drawing.Size(218, 22);
			this.encodeMenu.Text = "エンコードキューに入れる(&E)";
			this.encodeMenu.Click += new System.EventHandler(this.encodeMenu_Click);
			// 
			// storeMenu
			// 
			this.storeMenu.Image = global::magicAnime.Properties.Resources.SaveAll;
			this.storeMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.storeMenu.Name = "storeMenu";
			this.storeMenu.Size = new System.Drawing.Size(218, 22);
			this.storeMenu.Text = "最終保存先へ転送(&M)";
			this.storeMenu.Click += new System.EventHandler(this.storeMenu_Click);
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(215, 6);
			// 
			// renameFileMenu
			// 
			this.renameFileMenu.Name = "renameFileMenu";
			this.renameFileMenu.Size = new System.Drawing.Size(218, 22);
			this.renameFileMenu.Text = "保存ファイル名にリネーム(&C)";
			this.renameFileMenu.Click += new System.EventHandler(this.renameFileMenu_Click);
			// 
			// unreadMenu
			// 
			this.unreadMenu.Checked = true;
			this.unreadMenu.CheckState = System.Windows.Forms.CheckState.Checked;
			this.unreadMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.unreadMenu.Name = "unreadMenu";
			this.unreadMenu.Size = new System.Drawing.Size(218, 22);
			this.unreadMenu.Text = "未読フラグ(&U)";
			this.unreadMenu.Click += new System.EventHandler(this.unreadMenu_Click);
			// 
			// RecordPropertyMenu
			// 
			this.RecordPropertyMenu.Image = global::magicAnime.Properties.Resources.ProgramProperty;
			this.RecordPropertyMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.RecordPropertyMenu.Name = "RecordPropertyMenu";
			this.RecordPropertyMenu.Size = new System.Drawing.Size(218, 22);
			this.RecordPropertyMenu.Text = "エピソードのプロパティ(&O)";
			this.RecordPropertyMenu.Click += new System.EventHandler(this.RecordPropertyMenu_Clicked);
			// 
			// panel3
			// 
			this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel3.BackColor = System.Drawing.Color.Transparent;
			this.panel3.Controls.Add(this.datePictureBox);
			this.panel3.Controls.Add(this.statusPictureBox);
			this.panel3.Controls.Add(this.titlePictureBox);
			this.panel3.Controls.Add(this.filePictureBox);
			this.panel3.Controls.Add(this.titleLabel);
			this.panel3.Controls.Add(label6);
			this.panel3.Controls.Add(this.RecordStateLabel);
			this.panel3.Controls.Add(this.dateTimeLabel);
			this.panel3.Controls.Add(this.label9);
			this.panel3.Controls.Add(label7);
			this.panel3.Controls.Add(this.filePathLabel);
			this.panel3.Controls.Add(label4);
			this.panel3.Location = new System.Drawing.Point(0, 66);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(575, 80);
			this.panel3.TabIndex = 13;
			// 
			// titleLabel
			// 
			this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.titleLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.titleLabel.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.titleLabel.Location = new System.Drawing.Point(97, 7);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(475, 20);
			this.titleLabel.TabIndex = 11;
			this.titleLabel.UseMnemonic = false;
			// 
			// RecordStateLabel
			// 
			this.RecordStateLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.RecordStateLabel.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.RecordStateLabel.Location = new System.Drawing.Point(97, 59);
			this.RecordStateLabel.Name = "RecordStateLabel";
			this.RecordStateLabel.Size = new System.Drawing.Size(196, 20);
			this.RecordStateLabel.TabIndex = 6;
			// 
			// dateTimeLabel
			// 
			this.dateTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dateTimeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dateTimeLabel.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.dateTimeLabel.Location = new System.Drawing.Point(412, 60);
			this.dateTimeLabel.Name = "dateTimeLabel";
			this.dateTimeLabel.Size = new System.Drawing.Size(160, 20);
			this.dateTimeLabel.TabIndex = 8;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label9.Location = new System.Drawing.Point(333, 60);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(70, 15);
			this.label9.TabIndex = 7;
			this.label9.Text = "放送日時:";
			// 
			// filePathLabel
			// 
			this.filePathLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.filePathLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.filePathLabel.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.filePathLabel.Location = new System.Drawing.Point(97, 31);
			this.filePathLabel.Name = "filePathLabel";
			this.filePathLabel.Size = new System.Drawing.Size(475, 20);
			this.filePathLabel.TabIndex = 4;
			this.filePathLabel.UseMnemonic = false;
			// 
			// mainToolBar
			// 
			this.mainToolBar.AllowMerge = false;
			this.mainToolBar.ImageScalingSize = new System.Drawing.Size(32, 32);
			this.mainToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newAnimeButton,
            this.propertyButton,
            this.toolStripSeparator2,
            this.refreshButton,
            this.toolStripButton3,
            this.toolStripSeparator3,
            this.StoreAllButton,
            this.playUnreadButton,
            this.toolStripSeparator5,
            this.changeResolutionButton,
            this.seriesModeMenu,
            this.calenderModeMenu,
            this.toolStripSeparator4,
            this.thumbnailModeButton,
            this.logButton});
			this.mainToolBar.Location = new System.Drawing.Point(0, 24);
			this.mainToolBar.Name = "mainToolBar";
			this.mainToolBar.Size = new System.Drawing.Size(750, 39);
			this.mainToolBar.Stretch = true;
			this.mainToolBar.TabIndex = 14;
			this.mainToolBar.Text = "toolStrip1";
			// 
			// newAnimeButton
			// 
			this.newAnimeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.newAnimeButton.Image = global::magicAnime.Properties.Resources.NewProgram;
			this.newAnimeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.newAnimeButton.Name = "newAnimeButton";
			this.newAnimeButton.Size = new System.Drawing.Size(36, 36);
			this.newAnimeButton.Text = "新規番組";
			this.newAnimeButton.Click += new System.EventHandler(this.NewAnimeMenu_Click);
			// 
			// propertyButton
			// 
			this.propertyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.propertyButton.Image = global::magicAnime.Properties.Resources.ProgramProperty;
			this.propertyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.propertyButton.Name = "propertyButton";
			this.propertyButton.Size = new System.Drawing.Size(36, 36);
			this.propertyButton.Text = "番組プロパティ";
			this.propertyButton.Click += new System.EventHandler(this.propertyButton_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
			// 
			// refreshButton
			// 
			this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.refreshButton.Image = global::magicAnime.Properties.Resources.Update;
			this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Size = new System.Drawing.Size(36, 36);
			this.refreshButton.Text = "新着データ更新";
			this.refreshButton.Click += new System.EventHandler(this.RefreshMenu_Clicked);
			// 
			// toolStripButton3
			// 
			this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new System.Drawing.Size(23, 36);
			this.toolStripButton3.Text = "encode";
			this.toolStripButton3.Visible = false;
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 39);
			// 
			// StoreAllButton
			// 
			this.StoreAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.StoreAllButton.Image = global::magicAnime.Properties.Resources.SaveAll;
			this.StoreAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.StoreAllButton.Name = "StoreAllButton";
			this.StoreAllButton.Size = new System.Drawing.Size(36, 36);
			this.StoreAllButton.Text = "全て最終保存先に転送";
			this.StoreAllButton.Click += new System.EventHandler(this.StoreAllButton_Click);
			// 
			// playUnreadButton
			// 
			this.playUnreadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.playUnreadButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unreadListMenu});
			this.playUnreadButton.Image = global::magicAnime.Properties.Resources.PlayUnread;
			this.playUnreadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.playUnreadButton.Name = "playUnreadButton";
			this.playUnreadButton.Size = new System.Drawing.Size(48, 36);
			this.playUnreadButton.Text = "未読を再生";
			this.playUnreadButton.ButtonClick += new System.EventHandler(this.playUnreadButton_ButtonClick);
			this.playUnreadButton.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.playUnreadButton_DropDownItemClicked);
			// 
			// unreadListMenu
			// 
			this.unreadListMenu.Enabled = false;
			this.unreadListMenu.Name = "unreadListMenu";
			this.unreadListMenu.Size = new System.Drawing.Size(102, 22);
			this.unreadListMenu.Text = "(なし)";
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 39);
			// 
			// changeResolutionButton
			// 
			this.changeResolutionButton.CheckOnClick = true;
			this.changeResolutionButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.changeResolutionButton.Image = global::magicAnime.Properties.Resources.window;
			this.changeResolutionButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.changeResolutionButton.Name = "changeResolutionButton";
			this.changeResolutionButton.Size = new System.Drawing.Size(36, 36);
			this.changeResolutionButton.Text = "VGA解像度に切替";
			this.changeResolutionButton.Visible = false;
			// 
			// seriesModeMenu
			// 
			this.seriesModeMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.seriesModeMenu.Image = global::magicAnime.Properties.Resources.NumberingMode;
			this.seriesModeMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.seriesModeMenu.Name = "seriesModeMenu";
			this.seriesModeMenu.Size = new System.Drawing.Size(36, 36);
			this.seriesModeMenu.Text = "ナンバリングモード";
			this.seriesModeMenu.Click += new System.EventHandler(this.seriesModeMenu_Click);
			// 
			// calenderModeMenu
			// 
			this.calenderModeMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.calenderModeMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.weekModeMenu,
            this.dayModeMenu,
            this.toolStripMenuItem9,
            this.calenderModeOptionMenu});
			this.calenderModeMenu.Image = global::magicAnime.Properties.Resources.CalenderMode;
			this.calenderModeMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.calenderModeMenu.Name = "calenderModeMenu";
			this.calenderModeMenu.Size = new System.Drawing.Size(48, 36);
			this.calenderModeMenu.Text = "カレンダーモード";
			this.calenderModeMenu.ButtonClick += new System.EventHandler(this.calenderModeMenu_ButtonClick);
			// 
			// weekModeMenu
			// 
			this.weekModeMenu.Name = "weekModeMenu";
			this.weekModeMenu.Size = new System.Drawing.Size(168, 22);
			this.weekModeMenu.Text = "週単位で表示(&W)";
			this.weekModeMenu.Click += new System.EventHandler(this.weakModeMenu_Click);
			// 
			// dayModeMenu
			// 
			this.dayModeMenu.Name = "dayModeMenu";
			this.dayModeMenu.Size = new System.Drawing.Size(168, 22);
			this.dayModeMenu.Text = "日単位で表示(&D)";
			this.dayModeMenu.Click += new System.EventHandler(this.dayModeMenu_Click);
			// 
			// toolStripMenuItem9
			// 
			this.toolStripMenuItem9.Name = "toolStripMenuItem9";
			this.toolStripMenuItem9.Size = new System.Drawing.Size(165, 6);
			// 
			// calenderModeOptionMenu
			// 
			this.calenderModeOptionMenu.Name = "calenderModeOptionMenu";
			this.calenderModeOptionMenu.Size = new System.Drawing.Size(168, 22);
			this.calenderModeOptionMenu.Text = "設定(&S)";
			this.calenderModeOptionMenu.Click += new System.EventHandler(this.calenderModeOptionMenu_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 39);
			// 
			// thumbnailModeButton
			// 
			this.thumbnailModeButton.CheckOnClick = true;
			this.thumbnailModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.thumbnailModeButton.Image = global::magicAnime.Properties.Resources.Thumbnail;
			this.thumbnailModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.thumbnailModeButton.Name = "thumbnailModeButton";
			this.thumbnailModeButton.Size = new System.Drawing.Size(36, 36);
			this.thumbnailModeButton.Text = "サムネイルモード";
			this.thumbnailModeButton.Click += new System.EventHandler(this.thumbnailModeButton_Click);
			// 
			// logButton
			// 
			this.logButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.logButton.Image = global::magicAnime.Properties.Resources.Log1;
			this.logButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.logButton.Name = "logButton";
			this.logButton.Size = new System.Drawing.Size(36, 36);
			this.logButton.Text = "ログ";
			this.logButton.ToolTipText = "リアルタイムログ表示ペイン";
			this.logButton.Click += new System.EventHandler(this.logButton_Click);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(6, 39);
			// 
			// viewSplitContainer
			// 
			this.viewSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.viewSplitContainer.Location = new System.Drawing.Point(0, 149);
			this.viewSplitContainer.Name = "viewSplitContainer";
			this.viewSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// viewSplitContainer.Panel1
			// 
			this.viewSplitContainer.Panel1.Controls.Add(this.dataGrid);
			// 
			// viewSplitContainer.Panel2
			// 
			this.viewSplitContainer.Panel2.Controls.Add(this.logListBox);
			this.viewSplitContainer.Size = new System.Drawing.Size(750, 419);
			this.viewSplitContainer.SplitterDistance = 288;
			this.viewSplitContainer.TabIndex = 15;
			// 
			// logListBox
			// 
			this.logListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.logListBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.logListBox.FormattingEnabled = true;
			this.logListBox.IntegralHeight = false;
			this.logListBox.ItemHeight = 15;
			this.logListBox.Location = new System.Drawing.Point(3, 3);
			this.logListBox.Name = "logListBox";
			this.logListBox.ScrollAlwaysVisible = true;
			this.logListBox.Size = new System.Drawing.Size(747, 124);
			this.logListBox.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(750, 600);
			this.Controls.Add(this.viewSplitContainer);
			this.Controls.Add(this.mainToolBar);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.menuStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "mAgicAnime.NET";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.filePictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.titlePictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusPictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.datePictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.statusBar.ResumeLayout(false);
			this.statusBar.PerformLayout();
			this.contextMenuStrip.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.mainToolBar.ResumeLayout(false);
			this.mainToolBar.PerformLayout();
			this.viewSplitContainer.Panel1.ResumeLayout(false);
			this.viewSplitContainer.Panel2.ResumeLayout(false);
			this.viewSplitContainer.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGrid;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileMenu;
		private System.Windows.Forms.StatusStrip statusBar;
		private System.Windows.Forms.ToolStripMenuItem ExitMenu;
		private System.Windows.Forms.ToolStripMenuItem NewAnimeMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem StoreAllMenu;
		private System.Windows.Forms.ToolStripMenuItem OptionMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem RefreshMenu;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem playMovieMenu;
		private System.Windows.Forms.ToolStripSeparator encodeGroupSeparator;
		private System.Windows.Forms.ToolStripMenuItem encodeMenu;
		private System.Windows.Forms.ToolStripMenuItem storeMenu;
		private System.Windows.Forms.ToolStripMenuItem DeleteAnimeMenu;
		private System.Windows.Forms.ToolStripMenuItem SortMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
		private System.Windows.Forms.ToolStripMenuItem RecordPropertyMenu;
		private System.Windows.Forms.ToolStripMenuItem PowerMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AutoShutdownMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label titleLabel;
		private System.Windows.Forms.Label dateTimeLabel;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label RecordStateLabel;
		private System.Windows.Forms.Label filePathLabel;
		private System.Windows.Forms.ToolStrip mainToolBar;
		private System.Windows.Forms.ToolStripButton newAnimeButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton refreshButton;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripStatusLabel logStatusLabel;
		private System.Windows.Forms.ToolStripStatusLabel recordDriveFreeSpaceLabel;
		private System.Windows.Forms.ToolStripStatusLabel todayOnAirLabel;
		private System.Windows.Forms.ToolStripMenuItem ヘルプHToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
		private System.Windows.Forms.ToolStripMenuItem AboutMenu;
		private System.Windows.Forms.ToolStripMenuItem reserveMenu;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripMenuItem programPropertyMenu;
		private System.Windows.Forms.ToolStripMenuItem unreadMenu;
		private System.Windows.Forms.ToolStripSplitButton playUnreadButton;
		private System.Windows.Forms.ToolStripMenuItem unreadListMenu;
		private System.Windows.Forms.ToolStripButton propertyButton;
		private System.Windows.Forms.ToolStripButton StoreAllButton;
		private System.Windows.Forms.ToolStripMenuItem helpMenu;
		private System.Windows.Forms.ToolStripButton changeResolutionButton;
		private System.Windows.Forms.ToolStripMenuItem BatchListMenuItem;
		private System.Windows.Forms.ToolStripMenuItem BatchListNothingMenuItem;
		private System.Windows.Forms.ToolStripMenuItem BatchEncodeAllMenu;
		private System.Windows.Forms.ToolStripMenuItem JobsCancelMenu;
		private System.Windows.Forms.ToolStripMenuItem LogShowMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ForceRefreshMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripButton seriesModeMenu;
		private System.Windows.Forms.ToolStripSplitButton calenderModeMenu;
		private System.Windows.Forms.ToolStripMenuItem weekModeMenu;
		private System.Windows.Forms.ToolStripMenuItem dayModeMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem9;
		private System.Windows.Forms.ToolStripMenuItem calenderModeOptionMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripButton thumbnailModeButton;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTime;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnStoryCount;
		private System.Windows.Forms.ToolStripMenuItem EncodingNothingMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel storeFolderLabel;
		private System.Windows.Forms.ToolStripMenuItem OnReleaseUnreadMenu;
		private System.Windows.Forms.ToolStripMenuItem debugMenu;
		private System.Windows.Forms.ToolStripMenuItem debugShutdownMenu;
		private System.Windows.Forms.ToolStripMenuItem renameFileMenu;
		private System.Windows.Forms.ToolStripMenuItem cancelReserveMenu;
		private System.Windows.Forms.ToolStripSeparator extToolsGroupSeparator;
        private System.Windows.Forms.ToolStripMenuItem 拡張ツールToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applicationDataMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.SplitContainer viewSplitContainer;
		private System.Windows.Forms.ListBox logListBox;
		private System.Windows.Forms.ToolStripButton logButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
		private System.Windows.Forms.PictureBox filePictureBox;
		private System.Windows.Forms.PictureBox titlePictureBox;
		private System.Windows.Forms.PictureBox statusPictureBox;
		private System.Windows.Forms.PictureBox datePictureBox;
		private System.Windows.Forms.ToolStripMenuItem debugForceEmptyMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem10;
	}
}

