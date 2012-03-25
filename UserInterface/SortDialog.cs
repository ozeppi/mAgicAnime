//=========================================================================
///	<summary>
///		番組ソートダイアログ
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

namespace magicAnime.UserInterface
{
	public partial class SortDialog : Form
	{
		public SortDialog()
		{
			InitializeComponent();
		}

		private void sortMethodRadioButton_CheckedChanged( object sender, EventArgs e )
		{
			firstOrderGroupBox.Enabled = sortMethodRadioButton.Checked;
		}

		private void SortDialog_Load( object sender, EventArgs e )
		{
			sortMethodRadioButton_CheckedChanged( null, null );
		}

		private void sortButton_Click( object sender, EventArgs e )
		{
			AnimeServer server = AnimeServer.GetInstance();
			AnimeSort.OrderOption orderOption = 0;

			// ソート実行
			lock ( server )
			{
				AnimeSort.Order order;

				if( sortMethodRadioButton.Checked )
				{
					order = AnimeSort.Order.DayOfWeek;
				}
				else if( orderNextonairRadioButton.Checked )
				{
					order = AnimeSort.Order.NextOnair;
				}
				else
				{
					order = 0;
				}

				if( lastCheckBox.Checked )
				{
					orderOption |= AnimeSort.OrderOption.LastOrder;
				}
				if( (order == AnimeSort.Order.DayOfWeek) && limit1CoursCheckBox.Checked )
				{
					orderOption |= AnimeSort.OrderOption.Limit1CoursOption;
				}

				AnimeSort comparer = new AnimeSort( order, orderOption );

// <MOD> 2010/01/06 ->
				server.SortAnime( comparer );
//				server.Animes.Sort( comparer );
// <MOD> 2010/01/06 <-

			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancelButton_Click( object sender, EventArgs e )
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

	}

}