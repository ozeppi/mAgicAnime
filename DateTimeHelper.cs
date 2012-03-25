//=========================================================================
///	<summary>
///		日付時刻ヘルパ
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2010/01/XX 新規作成	Dr.Kurusugawa</history>
/// <history>2010/05/12 Subversionで管理するため不要なコメント削除</history>
//=========================================================================
using System;

namespace Helpers
{
	public class DateTimeHelper
	{
		private DateTime	mDateTime;
		private int			mHourOffset;

		public DateTimeHelper(DateTime dateTime, int hourOffset)
		{
			mDateTime	= dateTime;
			mHourOffset	= hourOffset;
		}

		public int Year	
		{
			get { return mDateTime.AddHours( -mHourOffset ).Year;		}
		}
		public int Month
		{
			get { return mDateTime.AddHours( -mHourOffset ).Month;		}
		}

		public int Day
		{
			get
			{
				return mDateTime.AddHours( -mHourOffset ).Day;
			}
		}

		public int Hour
		{
			get
			{
				int			dayCorrect	= 0;
				TimeSpan	span		= mDateTime.Date - mDateTime.AddHours( -mHourOffset ).Date;
				dayCorrect = (int)span.TotalDays;
				return mDateTime.Hour + dayCorrect * 24;
			}
		}
		public int Minute	{ get { return mDateTime.Minute;		} }

		// 何日違いか求める(当日なら0)
		static public int DiffDays(
			DateTime	dateA		,	// [i] 被減算日時
			DateTime	dateB		,	// [i] 減算日時
			int			hourOffset	)	// [i] 1日を(24+n)時間として扱う
		{
			dateA = dateA.AddHours( -hourOffset ).Date;
			dateB = dateB.AddHours( -hourOffset ).Date;

			if( dateB < dateA )
				return (int)(dateA - dateB).TotalDays;
			else
				return -(int)(dateB - dateA).TotalDays;
		}

		// 何週違いか求める(当週なら0)
		static public int DiffWeeks(
			DateTime	dateA		,	// [i] 被減算日時
			DateTime	dateB		,	// [i] 減算日時
			int			hourOffset	)	// [i] 1日を(24+n)時間として扱う
		{
			dateA = dateA.AddHours( -hourOffset ).Date;
			dateB = dateB.AddHours( -hourOffset ).Date;

			dateA = dateA.AddDays( -(int)dateA.DayOfWeek );
			dateB = dateB.AddDays( -(int)dateB.DayOfWeek );

			if( dateB < dateA )
				return +((int)(dateA - dateB).TotalDays) / 7;
			else
				return -((int)(dateB - dateA).TotalDays) / 7;
		}

		public string ToShortDateString()
		{
			return string.Format("{0:D4}/{1:D2}/{2:D2}",
				this.Year,
				this.Month,
				this.Day);
		}
		public string ToShortTimeString()
		{
			return string.Format("{0:D2}:{1:D2}",
				this.Hour,
				this.Minute);
		}

	}
}