using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace magicAnime
{
	internal partial class CalenderForm : Form
	{
		public CalenderForm()
		{
			InitializeComponent();
		}

		private void schedulePicture_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			string[] charaDayOfWeek = { "ì˙", "åé", "âŒ", "êÖ", "ñÿ", "ã‡", "ìy" };
			Font smallFont = new Font("MS UI Gothic", 9,FontStyle.Bold);
			//AnimeProgram selectedProgram = null;
			//AnimeRecord selectedRecord;
			//int storyNumber;
			int colWidth;

			colWidth = 35;
			
			g.Clear(Color.White);
	
			/*foreach(DataGridViewCell selectedCell in dataGrid.SelectedCells)
			{
			
				dataGrid_GetCellAttachedStructure(
					dataGrid.Rows[selectedCell.RowIndex], selectedCell.ColumnIndex,
					out selectedProgram, out selectedRecord, out storyNumber);
			}*/


			g.FillRectangle(
				new SolidBrush( Color.FromKnownColor(KnownColor.GradientActiveCaption)),
				new Rectangle(2,2,schedulePicture.Width - 4,21 ) );
			for(int i=0;i<7;++i)
			{
				g.DrawString(
					charaDayOfWeek[i], smallFont,
					new SolidBrush(Color.White), 45 + i * colWidth, 5);
			}
			
			int topHour		= Properties.Settings.Default.scheduleFirstHour;
			int viewHour	= Properties.Settings.Default.scheduleTimeZone;

			for(int i=0;i<=viewHour;++i)
			{
				g.DrawString(
					string.Format("{0:D2}:00", topHour + i), smallFont,
					new SolidBrush(Color.Gray), 5, 25 + i * 100 / viewHour);
			}
			
			AnimeServer doc = Program.GetAnimeServer();
			
			Brush hatchBrush	= new HatchBrush(HatchStyle.SmallCheckerBoard, Color.Blue	, Color.White);
			Brush focusedBrush	= new HatchBrush(HatchStyle.SmallCheckerBoard, Color.Red	, Color.White);
			Pen borderPen = new Pen(Color.Gray);
			
			foreach(AnimeProgram animeProgram in doc.Animes)
			{
				Rectangle rect;
				DateTime dateTime = animeProgram[1].StartDateTime;
				int dayOfWeek,hour;
				
				hour = dateTime.Hour;

				if (hour <= 3)
				{
					hour += 24;
					dayOfWeek = (int)dateTime.AddDays(-1.0).DayOfWeek;
				}else{
					dayOfWeek	= (int)dateTime.DayOfWeek;
				}
				
				rect = new Rectangle(
						45 + (int)dayOfWeek * colWidth,
						25 + (hour - topHour ) * 100 / viewHour
							+ animeProgram[1].StartDateTime.Minute * 100 / viewHour / 60,
						colWidth, 30 * 100 / viewHour / 60);

				//
				// î‘ëgÇÃÅ°Çï`âÊ
				//
				//g.FillRectangle((animeProgram.Equals(selectedProgram)) ? focusedBrush : hatchBrush, rect);
				g.DrawRectangle(borderPen, rect);

				g.DrawString(animeProgram.title, 
					new Font("MS UI Gothic",8),
					new SolidBrush(Color.Black), rect);
			}

		}
	}

}