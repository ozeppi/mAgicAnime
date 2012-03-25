//=========================================================================
///	<summary>
///		AnimeProgram���X�g�̃\�[�g�R���y�A�N���X
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using magicAnime.Properties;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		AnimeProgram���X�g�̃\�[�g�R���y�A���N���X
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	class AnimeSort : System.Collections.Generic.IComparer<AnimeProgram>
	{
		//--------------------
		// �R���y�A����
		//--------------------

		public enum Order
		{
			DayOfWeek,		// �j������
			NextOnair,		// �������������
			//			ReverseDayOfWeek,		// �j������(�t��)
		};

		[FlagsAttribute]
		public enum OrderOption
		{
			LastOrder = 1,// �����I���𖖔���
			Limit1CoursOption = 2,// �\�[�g����ŐV1�N�[���Ɍ���
		};

		private Order order;
		private OrderOption orderOption;
		//		private bool last;

		//=========================================================================
		///	<summary>
		///		�R���X�g���N�^
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public AnimeSort( Order order, OrderOption option )
		{
			this.order = order;
			this.orderOption = option;
			//			this.last			= last;
		}

		//=========================================================================
		///	<summary>
		///		AnimeProgram�̕������ԃR���y�A
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public int Compare( AnimeProgram x, AnimeProgram y )
		{
			AnimeEpisode nextA, nextB;
			AnimeProgram.NextEpisode resultA, resultB;

			//--------------------------
			// �b����0�̏ꍇ�͑O�ɉ�
			//--------------------------
			if( (x.StoryCount == 0) && (y.StoryCount == 0) )
				return 0;
			if( (x.StoryCount != 0) && (y.StoryCount == 0) )
				return +1;
			if( (x.StoryCount == 0) && (y.StoryCount != 0) )
				return -1;

			//--------------------------
			// �����I�����𖖔��ɉ�
			//--------------------------

			DateTime now = DateTime.Now;

			resultA = x.GetNextEpisode( now, out nextA );
			resultB = y.GetNextEpisode( now, out nextB );

			if( ((orderOption & OrderOption.LastOrder) != 0) ||
				  order == Order.NextOnair ) // ������������̏ꍇ�͋���
			{
				if( (resultA != AnimeProgram.NextEpisode.NextDecided) &&
					(resultB != AnimeProgram.NextEpisode.NextDecided) )
				{
					return 0;
				}
				if( (resultA != AnimeProgram.NextEpisode.NextDecided) &&
					 (resultB == AnimeProgram.NextEpisode.NextDecided) )
				{
					return +1;
				}
				if( (resultA == AnimeProgram.NextEpisode.NextDecided) &&
					(resultB != AnimeProgram.NextEpisode.NextDecided) )
				{
					return -1;
				}
			}

			if( order == Order.DayOfWeek )
			{
				//------------------------
				// �����j����
				//------------------------
				int aCount, bCount;
				long aMinute = 0, bMinute = 0;

				//------------------------------------
				// �e�b�̏T�J�n����̕��ώ������v�Z
				//------------------------------------

				aCount = 0;
				foreach( AnimeEpisode aRecord in x.Episodes )
				{
					if( (orderOption & OrderOption.Limit1CoursOption) != 0 )		// �ŐV1�N�[���Ɍ���I�v�V����(070612)
					{
						if( aRecord.StoryNumber < x.StoryCount - 13 )
							continue;
					}
// <MOD> 2009/12/28 ->
					if( aRecord.HasPlan )
//					if( aRecord.CurrentState != AnimeEpisode.State.Undecided )
// <MOD> 2009/12/28 <-
					{
// <MOD> 2009/12/28 ->
						DateTime convTime = aRecord.StartDateTime.AddHours( -(Settings.Default.hoursPerDay - 24) );
//						DateTime convTime = aRecord.StartDateTime.Value.AddHours( -(Settings.Default.hoursPerDay - 24) );
// <MOD> 2009/12/28 <-
						aMinute += ((long)convTime.DayOfWeek * 24 + convTime.Hour) * 60 + convTime.Minute;
						++aCount;
					}
				}

				bCount = 0;
				foreach( AnimeEpisode bRecord in y.Episodes )
				{
					if( (orderOption & OrderOption.Limit1CoursOption) != 0 )		// �ŐV1�N�[���Ɍ���I�v�V����(070612)
					{
						if( bRecord.StoryNumber < y.StoryCount - 13 )
							continue;
					}
// <MOD> 2009/12/28 ->
					if( bRecord.HasPlan )
//					if( bRecord.CurrentState != AnimeEpisode.State.Undecided )
// <MOD> 2009/12/28 <-
					{
// <MOD> 2009/12/28 ->
						DateTime convTime = bRecord.StartDateTime.AddHours( -(Settings.Default.hoursPerDay - 24) );
//						DateTime convTime = bRecord.StartDateTime.Value.AddHours( -(Settings.Default.hoursPerDay - 24) );
// <MOD> 2009/12/28 <-
						bMinute += ((long)convTime.DayOfWeek * 24 + convTime.Hour) * 60 + convTime.Minute;
						++bCount;
					}
				}

				// 070613
				if( (aCount == 0) && (bCount == 0) )
					return 0;
				if( (aCount == 0) && (bCount != 0) )
					return +1;
				if( (aCount != 0) && (bCount == 0) )
					return -1;

				aMinute /= aCount;
				bMinute /= bCount;

				if( aMinute < bMinute )
					return -1;
				else if( bMinute < aMinute )
					return +1;
				else
					return 0;
			}
			else if( order == Order.NextOnair )	
			{
				//------------------------
				// ���������
				//------------------------

				if( nextA.StartDateTime == nextB.StartDateTime )
					return 0;
				if( nextA.StartDateTime < nextB.StartDateTime )
					return -1;
				else
					return +1;
			}

			return 0;
		}

	}
































































































































































































































































}
