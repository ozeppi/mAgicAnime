//=========================================================================
///	<summary>
///		iEPG/tvpi�\�� ���W���[��
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		iEPG/tvpi�\��N���X
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	class iEPGScheduler : Scheduler
	{
		//-----------------------------
		// �I�[�o�[���C�h�v���p�e�B
		//-----------------------------
		public override string Name { get { return "iEPG"; } }
		public override AbilityFlag Ability
		{
			get { return AbilityFlag.MakeReservation; }
		}
		public override Type ProfileType
		{
			get { return null; }
		}

		//=========================================================================
		///	<summary>
		///		iEPG�Ή��^��\�t�g�ɗ\���o�^
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public override void MakeReservation(
			string		title		,	// [i] �^��^�C�g��
			string		uniqueID	,	// [i] ���j�[�NID
			string		tvStation	,	// [i] �e���r�ǖ�
			DateTime	dateTime	,	// [i] �^��J�n����
			int			minute		,	// [i] �^�掞��(��)
			string		Descript	,	// [i] �ԑg�̐���
			uint		groupCode	,	// [i] �^��O���[�v
			Profile		param		)	// [i] �ǉ��p�����[�^
		{
			DateTime		endDateTime;
			FileStream		file		= null;
			StreamWriter	sw;
			string			tempName;

			endDateTime = dateTime.AddMinutes(minute);

			//----------------------------
			// �e���|����tvpi�t�@�C���쐬
			//----------------------------

			file = File.Open(Path.GetTempFileName(), FileMode.Open);

			file.Close();

			tempName = Path.GetDirectoryName(file.Name) + @"\" +
						Path.GetFileNameWithoutExtension(file.Name) + ".tvpi";		// \�������Ă��� 07/01/14

			if (File.Exists(tempName))												// 07/01/14
			{
				throw new Exception("�e���|�����t�@�C�������ɑ��݂��Ă��܂�(" + tempName + ")");
			}

			File.Move(file.Name, tempName);

			file = new FileStream(tempName, FileMode.Open);

			//----------------------------------
			// tvpi�t�@�C���ɗ\������o��
			//----------------------------------
			sw = new StreamWriter(file, Encoding.GetEncoding(932));

			// �w�b�_�����o��
			sw.WriteLine( "Content-type: application/x-tv-program-info; charset=shift_jis");
			sw.WriteLine( "version: 1");

			// �R���e���c�������o��
			sw.WriteLine( "station: "		+ tvStation	);
			sw.WriteLine( "program-title: "	+ title		);
//			sw.WriteLine("program-title: " + title.Substring(0,title.IndexOf(",")));
			sw.WriteLine( "year: "	+ string.Format("{0:D4}",dateTime.Year)		);
			sw.WriteLine( "month: "	+ string.Format("{0:D2}",dateTime.Month)	);
			sw.WriteLine( "date: "	+ string.Format("{0:D2}",dateTime.Day)		);

			sw.WriteLine( "start: " + string.Format("{0:D2}:{1:D2}", dateTime.Hour		, dateTime.Minute	) );
			sw.WriteLine( "end: "	+ string.Format("{0:D2}:{1:D2}", endDateTime.Hour	, endDateTime.Minute) );

			sw.WriteLine( "" );
//				sw.WriteLine(title.Substring(title.IndexOf(",")+1));
			sw.WriteLine( uniqueID );

			sw.Close();

			//--------------------------------------------
			// tvpi�t�@�C���Ɋ֘A�t����ꂽ�A�v�����N��
			//--------------------------------------------
			try
			{
				Process process;

				process = Process.Start( tempName );

				process.WaitForExit();
			}
			catch (System.ComponentModel.Win32Exception)
			{
				throw new Exception("iEPG/tvpi�t�@�C�����^��\�t�g�Ɋ֘A�t�����Ă��܂���B");
			}
			finally
			{
				if (file != null)
				{
					file.Close();
					File.Delete(file.Name);
				}
			}

		}

		//=========================================================================
		///	<summary>
		///		�\����L�����Z��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public override void CancelReservation(
			string title,
			string uniqueID)
		{
			//
			// iEPG�ł̓L�����Z���s�\
			//
		}

		//=========================================================================
		///	<summary>
		///		�\���ύX
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public override void ChangeReservation(
			string title				,
			string uniqeuID				,
			DateTime newDateTime		,
			uint groupCode				,
			Profile param					)
		{
		}

		//=========================================================================
		///	<summary>
		///		�\����m�F
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public override bool ExistReservation(
			string title,		// �^��^�C�g��
			string uniqueID	)	// ���j�[�NID
		{
			return true;
		}

		//=========================================================================
		///	<summary>
		///		�\����t���b�V��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public override void Flush()
		{
		}

	}

}
