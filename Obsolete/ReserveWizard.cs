using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace magicAnime
{
	partial class ReserveWizard : Form
	{
		AnimeRecord animeRecord;
		public ReserveWizard()
		{
			InitializeComponent();
		}

		public DialogResult ShowDialog(AnimeRecord animeRecord)
		{
			if (animeRecord == null)
				throw new NotImplementedException();

			this.animeRecord = animeRecord;

			registTitleLabel.Text = string.Format(
				"{0:0} {1:0}�b",
				animeRecord.Parent.title,
				animeRecord.StoryNumber);

			tvStationLabel.Text = animeRecord.Parent.tvStation;

			return ShowDialog();
		}

		private void nextButton_Click(object sender, EventArgs e)
		{/*
			//
			// iEPG�o�R��mAgicTV�ɗ\��o�^����
			//

			Scheduler scheduler = new iEPGScheduler();

			animeRecord.Reserve(scheduler, null);

			System.Threading.Thread.Sleep( 500 );

			//
			// mAgicTV�̗\��G���g������o�^�����G���g����T��
			//
			mAgicReservation registReservation = null;

			mAgicScheduler.CallbackEnumScheduleEntries func
					= delegate (mAgicReservation reservation)
			{
				//
				// �\��^�C�g����@�ȍ~�̈�ӂȕ�����Ŕ��f����
				//
				string temp1, temp2;

				temp1 = reservation.title.Trim();
				temp2 = animeRecord.GetReserveTitle().Trim();
				
				temp1 = temp1.Substring(temp1.LastIndexOf('@'));
				temp2 = temp2.Substring(temp2.LastIndexOf('@'));

				if (temp1.Equals(temp2))	// ��������
				{
					registReservation = reservation;
				}

				return false;
			};

			mAgicScheduler.EnumScheduleEntries(func);

			if (registReservation == null)
			{
				throw new NotImplementedException("mAgicTV�ւ̗\��o�^�Ɏ��s���܂���");
			}

			//
			// �\��f�[�^���e���v���[�g�Ƃ��ăt�@�C���ɕۑ�����
			//

			string templateFilename;

			templateFilename = ReserveManager.GetReservationTemplateFileName(animeRecord.Parent);

			registReservation.Write( templateFilename);

			this.DialogResult = DialogResult.OK;
			Close();*/

		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;

			Close();
		}

	}

}


