//=========================================================================
///	<summary>
///		しょぼかるレコード フィルタ
///	</summary>
/// <remarks>
///		番組の全レコードからエピソード番号や局名をキーとしてレコード抽出。
/// </remarks>
/// <history>2009/04/04 新規作成</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace magicAnime.OnLine_DataBase
{
	// しょぼかるレコードフィルタ
	static internal class SyoboiRecordFilter
	{
		//=========================================================================
		///	<summary>
		///		エピソード番号と局名をキーとしてフィルタリング
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/04/04 SyoboiCalenderのメソッドから独立</history>
		//=========================================================================
		internal static List<SyoboiCalender.SyoboiRecord> EpisodeAndStationFilter(
			List<SyoboiCalender.SyoboiRecord>	recordList	,	// [i] 番組の全レコード
			int									storyNumber	,	// [i] エピソード番号
			string								tvStation	)	// [i] テレビ局
		{
			List<SyoboiCalender.SyoboiRecord> result = new List<SyoboiCalender.SyoboiRecord>();

			foreach (SyoboiCalender.SyoboiRecord record in recordList)
			{
				if (record.number == storyNumber &&
					record.tvStation.Equals(tvStation))
				{
					result.Add( record );
				}
			}
			return result;
		}

		//=========================================================================
		///	<summary>
		///		最も後に放送する放送プランを抽出
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/04/04 新規作成</history>
		/// <history>2010/01/04 新しいリストを返すよう変更</history>
		//=========================================================================
		internal static SyoboiCalender.SyoboiRecord LatestPlanFilter(
			List<SyoboiCalender.SyoboiRecord> records,	// [i] 放送レコード
			out int number)								// [o] 放送n回目
		{
			SyoboiCalender.SyoboiRecord		latest	= null;

// <MOD> 2010/01/04 ->
			List<SyoboiCalender.SyoboiRecord> tempList = new List<SyoboiCalender.SyoboiRecord>();

			foreach( SyoboiCalender.SyoboiRecord rec in records )
				tempList.Add( rec );

			// 放送時刻昇順にソート
			tempList.Sort( SortOrderDatetime );
			
			if(0 < tempList.Count)
			{
				number	= tempList.Count - 1;
				return tempList[tempList.Count - 1];
			}
			else
			{
				number	= -1;
				return null;
			}
//			foreach (SyoboiCalender.SyoboiRecord record in records)
//			{
//				if ((latest == null) || (latest.onAirDateTime < record.onAirDateTime))
//					latest = record;
//			}
//			
//			records.Clear();
//
//			if( latest != null )
//				records.Add(latest);
// <MOD> 2010/01/04 <-
		}

		//=========================================================================
		///	<summary>
		///		n番目に放送するプランを抽出
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/04/04 新規作成</history>
		//=========================================================================
		internal static SyoboiCalender.SyoboiRecord NumberPlanFilter(
			List<SyoboiCalender.SyoboiRecord>	records		,	// [i] 放送レコード
			int									Number		,	// [i] 放送n回目(1～)
			out int								selectNumber)	// [o] 放送n回目(放送がない場合は-1)
		{
// <MOD> 2010/01/04 ->
			List<SyoboiCalender.SyoboiRecord> tempList = new List<SyoboiCalender.SyoboiRecord>();

			foreach( SyoboiCalender.SyoboiRecord rec in records )
				tempList.Add( rec );

			// 放送時刻昇順にソート
			tempList.Sort( SortOrderDatetime );

			if((1 <= Number) && (Number <= records.Count))
			{
				selectNumber = Number - 1;
				return records[Number - 1];
			}
			else
			{
				selectNumber = -1;
				return null;
			}
//			if ((1 <= Number) && (Number <= records.Count))
//			{
//				SyoboiCalender.SyoboiRecord result = records[Number - 1];
//				records.Clear();
//				records.Add(result);
//			}
//			else
//			{
//				records.Clear();
//			}
// <MOD> 2010/01/04 <-
		}

// <ADD> 2010/01/04 ->
		//=========================================================================
		///	<summary>
		///		最も早く放送する放送プランを抽出
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/04/04 新規作成</history>
		//=========================================================================
		internal static SyoboiCalender.SyoboiRecord EarlytPlanFilter(
			List<SyoboiCalender.SyoboiRecord> records,	// [io] 放送レコード
			DateTime	now,							// [i] 基準時刻
			out int		number)							// [o] 放送n回目(放送がない場合は-1)
		{
			SyoboiCalender.SyoboiRecord			earliest	= null;
			List<SyoboiCalender.SyoboiRecord>	tempList	= new List<SyoboiCalender.SyoboiRecord>();

			foreach( SyoboiCalender.SyoboiRecord rec in records )
				tempList.Add( rec );

			// 放送時刻昇順にソート
			tempList.Sort( SortOrderDatetime );

			// 基準時刻以降で最も早い放送を得る
			number = -1;

			for( int i = 0 ; i < tempList.Count ; ++i )
			{
				if( now <= tempList[i].onAirDateTime )
				{
					earliest	= tempList[i];
					number		= i;
					break;
				}
			}

			return earliest;
		}

		// レコードの放送時刻を昇順判定
		static private int SortOrderDatetime(
			SyoboiCalender.SyoboiRecord a	,
			SyoboiCalender.SyoboiRecord b	)
		{
			if( a.onAirDateTime == b.onAirDateTime )
				return 0;
			return (a.onAirDateTime < b.onAirDateTime) ? -1 : +1;
		}
// <ADD> 2010/01/04 <-
	}
}
