//=========================================================================
///	<summary>
///		カレンダー表示モード設定ダイアログ
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
using magicAnime.Properties;

namespace magicAnime
{
	public partial class CalenderOptionDialog : Form
	{
		public CalenderOptionDialog()
		{
			InitializeComponent();
		}

		private void CalenderOptionDialog_Load(object sender, EventArgs e)
		{
			dayPastUpdown.Value		= Settings.Default.dayPast;
			dayFutureUpdown.Value	= Settings.Default.dayFuture;
			weekPastUpdown.Value	= Settings.Default.weekPast;
			weekFutureUpdown.Value	= Settings.Default.weekFuture;
		}

		private void okButton_Clicked(object sender, EventArgs e)
		{
			Settings.Default.dayPast	= (int)dayPastUpdown.Value;
			Settings.Default.dayFuture	= (int)dayFutureUpdown.Value;
			Settings.Default.weekPast	= (int)weekPastUpdown.Value;
			Settings.Default.weekFuture= (int)weekFutureUpdown.Value;

			this.DialogResult = DialogResult.OK;

			Close();
		}
	}
}