using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace magicAnime
{
	partial class AnimeScheduleDialog : Form
	{
		AnimeProgram animeProgram;
		public AnimeScheduleDialog()
		{
			InitializeComponent();
		}

		private void AnimeScheduleDialog_Load(object sender, EventArgs e)
		{
		
		}
		
		public DialogResult ShowDialog( ref AnimeProgram animeProgram )
		{
			bool decFlag = false;
			
			DateTime dateTime = DateTime.Now.AddDays( 7.0 );
			
			yearComboBox.Text	= string.Format("{0:0}", dateTime.Year);
			monthComboBox.Text	= string.Format("{0:0}", dateTime.Month);

			if (0 <= dateTime.Hour && dateTime.Hour <= 3)
				decFlag = true;	// ‘O“úˆµ‚¢

			if (decFlag)
			{
				dayOfWeekComboBox.SelectedIndex	= (int)dateTime.AddDays(-1.0).DayOfWeek;
				dayComboBox.Text	= string.Format("{0:0}", dateTime.AddDays(-1.0).Day);
				hourComboBox.Text	= string.Format("{0:0}", dateTime.Hour + 24);
			}else{
				dayOfWeekComboBox.SelectedIndex = (int)dateTime.DayOfWeek;
				dayComboBox.Text	= string.Format("{0:0}", dateTime.Day);
				hourComboBox.Text	= string.Format("{0:0}", dateTime.Hour);
			}
			
			minuteComboBox.Text = string.Format("{0:D2}", dateTime.Minute);
			
			autoRadioBox.Enabled	= animeProgram.linkOnlineDatabase;
			if (!autoRadioBox.Enabled)	undecidedRadioBox.Checked = true;
			
			this.animeProgram = animeProgram;
			
			OnCheckedRadioBox();

			return ShowDialog();
		}

		private void OnCheckedRadioBox()
		{

			onAirTimeGroup.Enabled		= serialRadioBox.Checked;
			firstOnAirGroup.Enabled		= serialRadioBox.Checked;

		}
		
		private void serialRadioBox_CheckedChanged(object sender, EventArgs e)
		{
			OnCheckedRadioBox();
		}
		

		private void autoRadioBox_CheckedChanged(object sender, EventArgs e)
		{
			OnCheckedRadioBox();
		}

		private void undecidedRadioBox_CheckedChanged(object sender, EventArgs e)
		{
			OnCheckedRadioBox();
		}

		private void dayComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void DayComboBoxCell_ItemUpdate(int year, int month, DayOfWeek dayOfWeek)
		{
			DateTime date = new DateTime(year, month, 1, 0, 0, 0);
			int incDays;

			incDays = (int)dayOfWeek - (int)date.DayOfWeek;
			if (incDays < 0) incDays = incDays + 7;
			date = date.AddDays((double)incDays);


			dayComboBox.Items.Clear();

			for (; ; )
			{
				dayComboBox.Items.Add(string.Format("{0:0}", date.Day));

				if (date.Month < date.AddDays(7.0).Month) break;
				date = date.AddDays(7.0);
			}

			dayComboBox.Text = (string)dayComboBox.Items[0];

		}

		private void monthComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{

			dayOfWeekComboBox_SelectedIndexChanged(sender, null);

		}

		private void dayOfWeekComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{

			DayComboBoxCell_ItemUpdate(
				int.Parse(yearComboBox.Text),
				int.Parse(monthComboBox.Text),
				(DayOfWeek)dayOfWeekComboBox.SelectedIndex);

		}

		private void okButton_Click(object sender, EventArgs e)
		{
			//int i;

			if (serialRadioBox.Checked)
			{
				throw new Exception();
/*
				DateTime firstDateTime = Document.CreateDateTime(
					int.Parse( yearComboBox.Text ),
					int.Parse( monthComboBox.Text ),
					int.Parse( dayComboBox.Text ),
					int.Parse( hourComboBox.Text ),
					int.Parse( minuteComboBox.Text ),
					0 );
				
				foreach(AnimeEpisode animeRecord in animeProgram.Episodes)
				{
				
					i = animeProgram.Episodes.IndexOf( animeRecord );

					animeRecord.CurrentState	= AnimeEpisode.State.Future;
					animeRecord.StartDateTime		= firstDateTime.AddDays((double)(i * 7));
					
				}
*/				
			}

			try{

				if (autoRadioBox.Checked)
					animeProgram.UpdatePlan(DateTime.Now);

			}catch(Exception ex)
			{
				Program.ShowException( ex ,MessageBoxIcon.Warning );
			}
			/*if (autoRadioBox.Checked)
			{
				UpdateDialog updateDialog = new UpdateDialog();
				
				updateDialog.ShowDialog();
			
			}*/

			this.DialogResult = DialogResult.OK;
			
			Close();
		}




	}
}