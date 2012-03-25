//=========================================================================
///	<summary>
///		����ڂ��郌�R�[�h �t�B���^
///	</summary>
/// <remarks>
///		�ԑg�̑S���R�[�h����G�s�\�[�h�ԍ���ǖ����L�[�Ƃ��ă��R�[�h���o�B
/// </remarks>
/// <history>2009/04/04 �V�K�쐬</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace magicAnime.OnLine_DataBase
{
	// ����ڂ��郌�R�[�h�t�B���^
	static internal class SyoboiRecordFilter
	{
		//=========================================================================
		///	<summary>
		///		�G�s�\�[�h�ԍ��Ƌǖ����L�[�Ƃ��ăt�B���^�����O
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/04/04 SyoboiCalender�̃��\�b�h����Ɨ�</history>
		//=========================================================================
		internal static List<SyoboiCalender.SyoboiRecord> EpisodeAndStationFilter(
			List<SyoboiCalender.SyoboiRecord>	recordList	,	// [i] �ԑg�̑S���R�[�h
			int									storyNumber	,	// [i] �G�s�\�[�h�ԍ�
			string								tvStation	)	// [i] �e���r��
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
		///		�ł���ɕ�����������v�����𒊏o
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/04/04 �V�K�쐬</history>
		/// <history>2010/01/04 �V�������X�g��Ԃ��悤�ύX</history>
		//=========================================================================
		internal static SyoboiCalender.SyoboiRecord LatestPlanFilter(
			List<SyoboiCalender.SyoboiRecord> records,	// [i] �������R�[�h
			out int number)								// [o] ����n���
		{
			SyoboiCalender.SyoboiRecord		latest	= null;

// <MOD> 2010/01/04 ->
			List<SyoboiCalender.SyoboiRecord> tempList = new List<SyoboiCalender.SyoboiRecord>();

			foreach( SyoboiCalender.SyoboiRecord rec in records )
				tempList.Add( rec );

			// �������������Ƀ\�[�g
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
		///		n�Ԗڂɕ�������v�����𒊏o
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/04/04 �V�K�쐬</history>
		//=========================================================================
		internal static SyoboiCalender.SyoboiRecord NumberPlanFilter(
			List<SyoboiCalender.SyoboiRecord>	records		,	// [i] �������R�[�h
			int									Number		,	// [i] ����n���(1�`)
			out int								selectNumber)	// [o] ����n���(�������Ȃ��ꍇ��-1)
		{
// <MOD> 2010/01/04 ->
			List<SyoboiCalender.SyoboiRecord> tempList = new List<SyoboiCalender.SyoboiRecord>();

			foreach( SyoboiCalender.SyoboiRecord rec in records )
				tempList.Add( rec );

			// �������������Ƀ\�[�g
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
		///		�ł�����������������v�����𒊏o
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/04/04 �V�K�쐬</history>
		//=========================================================================
		internal static SyoboiCalender.SyoboiRecord EarlytPlanFilter(
			List<SyoboiCalender.SyoboiRecord> records,	// [io] �������R�[�h
			DateTime	now,							// [i] �����
			out int		number)							// [o] ����n���(�������Ȃ��ꍇ��-1)
		{
			SyoboiCalender.SyoboiRecord			earliest	= null;
			List<SyoboiCalender.SyoboiRecord>	tempList	= new List<SyoboiCalender.SyoboiRecord>();

			foreach( SyoboiCalender.SyoboiRecord rec in records )
				tempList.Add( rec );

			// �������������Ƀ\�[�g
			tempList.Sort( SortOrderDatetime );

			// ������ȍ~�ōł����������𓾂�
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

		// ���R�[�h�̕�����������������
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
