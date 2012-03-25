//=========================================================================
///	<summary>
///		エピソードプロパティダイアログ
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成</history>
/// <history>2009/12/28	古い改修コメント削除</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace magicAnime
{
	partial class RecordDialog : Form
	{
		AnimeEpisode	mEpisode	= null;
	
		public RecordDialog()
		{
			InitializeComponent();
		}

		private void RefreshControl()
		{
			dateTimePicker.Enabled			= hasPlanCheckBox.Checked;
			lengthUpdown.Enabled			= hasPlanCheckBox.Checked;
			reserveDateTimePicker.Enabled	= isReservedCheckBox.Checked;
			FileTextBox.Enabled				= hasFileCheckBox.Checked;
		}
		
		public DialogResult ShowDialog(AnimeEpisode r)
		{
			this.mEpisode			= r;
		
			TitleTextBox.Text		= r.Parent.title;
			SubtitleTextBox.Text	= r.mSubTitle;
			FileTextBox.Text		= r.FilePath;
			StoryNumberUpdown.Value	= r.StoryNumber;

			hasPlanCheckBox.Checked		= r.HasPlan;
			if( r.HasPlan )
			{
				dateTimePicker.Value	= r.StartDateTime;
				lengthUpdown.Value		= r.Length;
			}

			isReservedCheckBox.Checked	= r.IsReserved;
			if( r.IsReserved )
				reserveDateTimePicker.Value	= r.ReserveDateTime;
			isReserveErrorCheckBox.Checked	= r.IsReserveError;

			hasFileCheckBox.Checked		= r.HasFile;
			FileTextBox.Text			= r.FilePath;

			isEncodedCheckBox.Checked	= r.IsEncoded;
			isStoredCheckBox.Checked	= r.IsStored;
			
// <ADD> 2010/01/04 ->
			repeatNumberTextBox.Text	= (0 <= r.RepeatNumber)
										? "第" + (r.RepeatNumber + 1).ToString() + "回目の放送です。"
										: "第?回目の放送です。";
			isRecordedCheckBox.Checked	= r.IsRecorded;
			isStartedOnairCheckBox.Checked = r.IsStartedOnair;
// <ADD> 2010/01/04 <-
// <ADD> 2010/01/06 ->
			isStorableCheckBox.Checked	= r.IsStorable;
// <ADD> 2010/01/06 <-
// <ADD> 2010/01/11 ->
			timeChangedCheckBox.Checked	= r.JudgeTimeChanged;
			try
			{
				recordStartDateTime.Value	= r.StartRecordDateTime;
				recordEndDateTime.Value		= r.EndRecordDateTime;
			}
			catch(ArgumentOutOfRangeException ex)
			{
				recordStartDateTime.Value	= recordStartDateTime.MinDate;
				recordEndDateTime.Value		= recordEndDateTime.MinDate;
			}
// <ADD> 2010/01/11 <-
// <ADD> 2010/02/07 ->
			thumbnailMakedCheckBox.Checked	= r.ThumbnailMaked;
// <ADD> 2010/02/07 <-
// <ADD> 2010/04/17 ->
			planErrorCheckbox.Checked		= r.PlanError;
// <ADD> 2010/04/17 <-

			RefreshControl();

			return ShowDialog();
		}

		private void OpenFileButton_Click_1(object sender, EventArgs e)
		{

			OpenFileDialog.FileName = FileTextBox.Text;

			if( OpenFileDialog.ShowDialog() == DialogResult.OK )
			{

				FileTextBox.Text = OpenFileDialog.FileName;
			}

		}

		// ダブルクォーテーションの囲みを削除
		private string MakeNakedFilePath( string path )
		{
			Regex parser = new Regex("((\\\"{1}(?<Path>.*?)\\\"{1})|(?<Path>(.*)))");
			Match match		= parser.Match( path );
			if (match.Success)
				return match.Groups["Path"].Value;
			return path;
		}

		// [OK]ボタンの処理
		private void OkButton_Click(object sender, EventArgs e)
		{
			AnimeEpisode	ep		= mEpisode;

			ep.mSubTitle			= SubtitleTextBox.Text;

			ep.HasPlan				= hasPlanCheckBox.Checked;
			if( ep.HasPlan )
			{
				ep.StartDateTime	= dateTimePicker.Value;
				ep.StartDateTime	= ep.StartDateTime.AddSeconds( -ep.StartDateTime.Second );
				ep.Length			= (int)lengthUpdown.Value;
			}

			ep.IsReserved			= isReservedCheckBox.Checked;
			if( ep.IsReserved )
			{
				ep.ReserveDateTime	= reserveDateTimePicker.Value;
				ep.ReserveDateTime	= ep.ReserveDateTime.AddSeconds( -ep.ReserveDateTime.Second );
			}
			ep.IsReserveError		= isReserveErrorCheckBox.Checked;

			ep.HasFile				= hasFileCheckBox.Checked;
			ep.FilePath				= MakeNakedFilePath( FileTextBox.Text );

			ep.IsEncoded			= isEncodedCheckBox.Checked;
			ep.IsStored				= isStoredCheckBox.Checked;

// <ADD> 2010/02/07 ->
			ep.ThumbnailMaked		= thumbnailMakedCheckBox.Checked;
// <ADD> 2010/02/07 <-
// <ADD> 2010/04/17 ->
			ep.PlanError			= planErrorCheckbox.Checked;
// <ADD> 2010/04/17 <-

/*			try
			{
				string newFilePath = FileTextBox.Text;

// <ADD> 2009/06/24 ファイルパスが変更されたとき、チェックと警告を行う ->
				Predicate<AnimeEpisode.State> predState =
					delegate(AnimeEpisode.State target) { return (target == state); };

				// ファイルを持つ状態でのみチェック
				if (!Array.Exists<AnimeEpisode.State>(hasNotfileStates, predState))
				{
// <MOD> 2009/08/02 ダブルクォーテーションの囲みを削除 ->
					Regex parser = new Regex("((\\\"{1}(?<Path>.*?)\\\"{1})|(?<Path>(.*)))");
					Match match		= parser.Match( newFilePath );
					if (match.Success)
					{
						newFilePath = match.Groups["Path"].Value;
					}
					
					if (mEpisode.FilePath != newFilePath)
//					if (animeRecord.FilePath != FileTextBox.Text)
// <MOD> 2009/08/02 <-
					{
// <MOD> 2009/08/02 ->
						if (!File.Exists( newFilePath ))
//						if (!File.Exists(FileTextBox.Text))
// <MOD> 2009/08/02 <-
						{
							if (MessageBox.Show("指定したファイルが見つかりませんがよろしいですか？" +
												System.Environment.NewLine + newFilePath,
												null,
												MessageBoxButtons.OKCancel,
												MessageBoxIcon.Question) != DialogResult.OK)
							{
								return;
							}
						}
					}
				}
// <ADD> 2009/06/24 <-
// <MOD> 2009/08/02 ->

// <MOD> 2009/12/28 ->

				mEpisode.mSubTitle = SubtitleTextBox.Text;
				//animeRecord.StoryNumber	= (int)StoryNumberUpdown.Value;


				//if ( state == AnimeEpisode.State.Undecided ||
				//    state == AnimeEpisode.State.Notfound ||
				//    state == AnimeEpisode.State.Decided )
				//    animeRecord.FilePath = "";
				//else
				//    animeRecord.FilePath = FileTextBox.Text;

// <MOD> 2009/11/22 ->
				mEpisode.Dirty = true;
//				Program.RaiseModifiedEvent();
// <MOD> 2009/11/22 <-

				//throw new Exception("未実装");

			}
			catch ( Exception ex )
			{
				MessageBox.Show(
					ex.Message				,
					"内部エラー"				,
					MessageBoxButtons.OK	,
					MessageBoxIcon.Error	);
			}*/

			Close();
		}

		private void hasPlanCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			RefreshControl();
		}

		private void isReservedCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			RefreshControl();
		}

		private void hasFileCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			RefreshControl();
		}

	}
	
}