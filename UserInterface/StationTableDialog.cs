//=========================================================================
///	<summary>
///		テレビ局名変換テーブル 画面
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2010/03/28 AnimeDialog.csから分離 Dr.Kurusugawa</history>
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
	public partial class StationTableDialog : Form
	{
		string	mStationName;

		public StationTableDialog(string stationName)
		{
			InitializeComponent();

			mStationName = stationName;
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void okButton_Click(object sender, EventArgs e)
		{

			if (!referenceTvStationTextBox.Text.Equals(""))
			{

				if (registCheckBox.Checked)
				{
					string s = referenceTvStationTextBox.Text;	// 放送局名
					string r = registerdStationComboBox.Text;	// プリセット局名

					ReserveManager.ModifyStationTable(s, r);	// 対応を保存する
				}
				else
				{
					string s = referenceTvStationTextBox.Text;	// 放送局名
					ReserveManager.ModifyStationTable(s, s);	// 対応を保存する
				}

			}


			this.Close();
		}

		private void StationTableForm_Shown(object sender, EventArgs e)
		{

            referenceTvStationTextBox.Text = mStationName;

			//--------------------------------------
			// 録画ソフトの登録テレビ局一覧を表示
			//--------------------------------------
			var		sched		= ReserveManager.DefaultScheduler;

			if (sched != null)
			{
				var		stations	= sched.GetStations();

				foreach (var s in stations)
					registerdStationComboBox.Items.Add( s );
			}

            //-------------------------
            // エイリアス名を更新
            //-------------------------
            string		regName;

            if (string.IsNullOrEmpty(mStationName))
            {
                registCheckBox.Checked = false;
                registCheckBox.Enabled = false;
            }
            else
            {
                regName = ReserveManager.GetRegisteredStationName(mStationName);

                registerdStationComboBox.Text = regName;
                registCheckBox.Enabled = true;
                registCheckBox.Checked = !mStationName.Equals(regName);	// 登録名と同じならチェックしない
            }
		}

		private void registCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if(referenceTvStationTextBox.Text.Equals(""))
			    registerdStationComboBox.Enabled = false;	// 放送局名がなければ登録局名コンボは無効
			else
			    registerdStationComboBox.Enabled = registCheckBox.Checked;
		}
	}
}
