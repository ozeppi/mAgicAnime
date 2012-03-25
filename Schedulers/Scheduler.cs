//=========================================================================
///	<summary>
///		�^��\�t�g�A���v���O�C�� ���N���X ���W���[��
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		�^��\�t�g�A���v���O�C�����N���X
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	public abstract class Scheduler
	{
		//--------------------
		// �^�̐錾
		//--------------------

// <ADD> 2008/10/21 �X�P�W���[����O�̑����N���X ->
		[AttributeUsage(AttributeTargets.Class)]
		public class SchedulerExceptionAtribute : System.Attribute
		{
			private string m_Description;

			public SchedulerExceptionAtribute(string Description) { this.Description = Description; }

			public string Description
			{
				get { return m_Description; }
				set { m_Description = value; }
			}
		}
// <ADD> 2008/10/21 <-

// <ADD> 2008/10/21 �X�P�W���[����O�̊��N���X ->
		[Scheduler.SchedulerExceptionAtribute("�\��̃G���[�ł�")]
		public class SchedulerBaseExecption : Exception { }

		// �����ȗ\�񎞊Ԃ��w�肳�ꂽ���̗�O
		[Scheduler.SchedulerExceptionAtribute("�����ȗ\�񎞊Ԃł�")]
		public class InvalidScheduleTimeException : SchedulerBaseExecption {}
		// �^�掞�Ԃ�0�̗�O
		[Scheduler.SchedulerExceptionAtribute("�^�掞�Ԃ�0���ȉ��ɂȂ�܂�")]
		public class ZeroLengthScheduleTimeException : SchedulerBaseExecption {}
// <ADD> 2008/10/21 <-
// <PENDING> 2008/10/21 ���̗�O��SchedulerBaseException����h������͕̂ۗ�(�݊����̂���) ->
		// �\��d�����̗�O
		public class DoubleBookingException : Exception { }
		// (���ԕύX�O��)�\������𔺂��\��d�����̗�O
		public class DoubleBookingWithLostException : Exception { }
// <PENDING> 2008/10/21 <-

		[Flags]
		public enum AbilityFlag
		{
			MakeReservation		= 0x0001,	// �V�K�\��\
			ChangeReservation	= 0x0002,	// �\��ύX�\
			CancelReservation	= 0x0010,	// �\��L�����Z���\
			ExistReservation	= 0x0020,	// �\��`�F�b�N�\
		}

		//--------------------
		// ���v���p�e�B
		//--------------------
		public abstract string Name { get; }
		public virtual string Folder { get{ return null; } }
		public virtual string Extension { get { return null; } }
		public virtual bool SubDirectory { get { return false; } }
		public abstract AbilityFlag Ability { get; }
		public virtual List<string> GetStations() { return new List<string>(); }	// �e���r�ǈꗗ(ver1.7.1)


// �� 2007/12/09 �p���N���X�ŃR���X�g���N�^���g�����ߒǉ�
		//=========================================================================
		///	<summary>
		///		�R���X�g���N�^
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public Scheduler() {}
// �� 2007/12/09

// �� 2008/01/03 �풓����������O�̃N���[���A�b�v�ɒǉ�
		//=========================================================================
		///	<summary>
		///		�I���O�̃N���[���A�b�v
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public virtual void Close() {}
// �� 2008/01/03


		//=========================================================================
		///	<summary>
		///		�ԑg���ƂɓK�p����^��v���t�@�C���N���X
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/XX/XX �V�K�쐬 ver 1.8.01</history>
		//=========================================================================
		public class Profile
		{
			public virtual void Import(XmlReader xr) {}
			public virtual void Export(XmlWriter xw) {}
		}
		public abstract Type ProfileType { get; }

// �� 2007/08/16 �ǉ�
		//=========================================================================
		///	<summary>
		///		�^��v���t�@�C����ҏW����v���p�e�B�y�[�W
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public class ProfilePage : Form
		{
//			Control Parent;		// �e�t�H�[����
			public ProfilePage()
			{
			}
			public void Create(Control Parent)
			{
				this.TopLevel = false;
				this.Parent = Parent;
				this.Show();
			}
			public virtual void Load( Profile profile ) { }
			public virtual void Save( Profile profile ) { }
		}

		//=========================================================================
		///	<summary>
		///		�v���t�@�C���̌^����Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public virtual Type ProfilePageType { get { return null; } }
//		public virtual ProfilePage OpenProfilePage( Control parent, Profile profile ) { }
// �� 2007/08/16 �ǉ�


		//=========================================================================
		///	<summary>
		///		���Ԃ��w�肵�Ę^��\���V�K�o�^����
		///	</summary>
		/// <remarks>
		///		�����^��ł�����ł́A���ԏd�����Ă����s����Ƃ͌���Ȃ��B
		///		�������A�����^��^�C�g���̏ꍇ�͎��s����B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public abstract void MakeReservation(
			string				title		,
			string				uniqueID	,
			string				tvStation	,
			DateTime			dateTime	,
			int					minute		,
			string				Descript	,
			uint				groupCode	,
			Scheduler.Profile	param		);

		//=========================================================================
		///	<summary>
		///		�w�肳�ꂽ�\����L�����Z������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public abstract void CancelReservation(string title,string uniqueID);

		//=========================================================================
		///	<summary>
		///		�w�肳�ꂽ�\���ύX����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public abstract void ChangeReservation(
			string title			,
			string uniqueID			,
			DateTime newDateTime	,
			uint groupCode			,
			Scheduler.Profile param	);

		//=========================================================================
		///	<summary>
		///		�w�肳�ꂽ�\�񂪍݂邩�m�F����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public abstract bool ExistReservation(string title, string uniqueID);

		//=========================================================================
		///	<summary>
		///		�\��f�[�^�̃t���b�V�����s��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public abstract void Flush();
	}

}

