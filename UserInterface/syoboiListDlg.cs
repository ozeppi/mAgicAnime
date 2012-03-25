//=========================================================================
///	<summary>
///		�ԑg�ꗗ�_�C�A���O
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬	Dr.Kurusugawa</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace magicAnime
{
	public partial class SyoboiListDlg : Form
	{
		public int tid;
		private List<SyoboiCalender.SyoboiProgram> animeList;

		public SyoboiListDlg()
		{
			InitializeComponent();
		}

		private void SyoboiListDlg_Load(object sender, EventArgs e)
		{
			conditionComboBox.SelectedIndex = 2;
		}

		private void conditionComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateList(true);
		}

		//
		// ABSTRACT	: ���X�g���X�V����
		//
		private void UpdateList(bool download)
		{
			Cursor oldCursor;
			int year, month;

			oldCursor = Cursor.Current;									// �J�[�\����ҋ@��Ԃ�
			Cursor.Current = Cursors.WaitCursor;

			year = DateTime.Now.Year;
			month = DateTime.Now.Month;

			try
			{
				if ( download )
				{
// <MOD> 2090/02/11 ->
					SyoboiCalender syoboiCalender = AnimeServer.GetInstance().mSyoboiCalender;
//					SyoboiCalender syoboiCalender = new SyoboiCalender();
// <MOD> 2090/02/11 <-
					animeList = syoboiCalender.DownloadPrograms();
				}
			}
			catch ( Exception )
			{
				MessageBox.Show(
					"����ڂ��J�����_�[����̃f�[�^�擾�Ɏ��s���܂����B\n" +
					"�ʐM�G���[���T�[�o�[���d�l�ύX�̉\��������܂��B"	,
					"�G���["												,
					MessageBoxButtons.OK								,
					MessageBoxIcon.Warning								);

				//linkOnlineDatabaseCheckBox.Checked = false;
				//linkOnlineDatabaseCheckBox.Enabled = false;
			}

			syoboiListView.Items.Clear();

			//
			// �ԑg�����X�g�r���[�ɒǉ�
			//
			foreach ( SyoboiCalender.SyoboiProgram anime in animeList )
			{
				bool matched = false;

				if ( conditionComboBox.SelectedIndex == 0 )					// �S�đΏ�
				{
					matched = true;
				} else if ( conditionComboBox.SelectedIndex == 1 ||			// �����܂��͗����J�n
							conditionComboBox.SelectedIndex == 2 )
				{
					//
					// ����/�����̕����J�n������
					//
					if ( ( anime.seasonOnAir.RecordState &
						( SyoboiCalender.SyoboiProgram.SeasonOnAir.YearDecided
						| SyoboiCalender.SyoboiProgram.SeasonOnAir.MonthDecided ) ) > 0 )
					{
						int season, additional = 0;
						season = anime.seasonOnAir.year * 12 + anime.seasonOnAir.month;

						if ( conditionComboBox.SelectedIndex == 2 )
							additional = 1;
						
						if ( season == year * 12 + month + additional )
						{
							matched = true;
						}
					}
				}

				if ( filterTextBox.Text != "" )								// �i���ݏ���������ꍇ
				{
					matched &= ( 0 <= anime.title.IndexOf( filterTextBox.Text ) ) ? true : false;
				}

				if ( matched )
				{
					ListViewItem item;
					item = syoboiListView.Items.Add( anime.title );
					item.SubItems.Add( anime.seasonOnAir.ToString() );
					item.SubItems.Add( anime.tid.ToString() );
				}
			}

			//linkOnlineDatabaseCheckBox.Checked = animeProgram.linkOnlineDatabase;
			//syoboiTitleComboBox.Text = animeProgram.syoboiTitle;
			//tvStationComboBox.Text = animeProgram.tvStation;
			//selectedTid = animeProgram.syoboiTid;

			//linkOnlineDatabaseCheckBox.Checked = true;

			//syoboiTitleComboBox_OnUpdate();

			Cursor.Current = oldCursor;
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Close();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection items;

			items = syoboiListView.SelectedItems;

			if (items.Count>0)
			{
				this.tid = System.Convert.ToInt32( items[0].SubItems[2].Text );

				this.DialogResult = DialogResult.OK;
				Close();
			}

		}

		private void googleButton_Click(object sender, EventArgs e)
		{

		}

		//
		// ABSTRACT	: �i���ݏ������ύX���ꂽ�Ƃ��̏���
		//
		private void filterTextBox_TextChanged( object sender, EventArgs e )
		{
			UpdateList( false );		// �t�B���^���������ύX���ă��X�g�X�V
		}
	}
}