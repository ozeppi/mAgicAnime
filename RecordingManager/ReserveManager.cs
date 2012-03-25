//=========================================================================
///	<summary>
///		�^��\�t�g���䃂�W���[��
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬 Dr.Kurusugawa</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Xml;
using magicAnime.Properties;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		�^��\�t�g���䃉�b�p�[�N���X
	///	</summary>
	/// <remarks>
	///		�^��\�t�g�A���v���O�C�����o�R���ė\��̓o�^��폜���s���B
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	/// <history>2007/08/15	�N�����ɑS�v���O�C����Scheduler�C���X�^���X������Ă���</history>
	//=========================================================================
	public class ReserveManager
	{
		//----------------
		// �^�̒�`
		//----------------
		internal enum ChangeResult
		{
			Dontcare,	// �G��Ȃ�
			OK		,	// ����
			Lost	,	// �\���������
			Denied	,	// �\��𓮂����Ȃ�
		};
		
		//-------------------------------
		// �����o�ϐ�
		//-------------------------------
		private static List<Scheduler> schedulerList;

		//=========================================================================
		///	<summary>
		///		�v���O�C���C���X�^���X���X�g��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public static List<Scheduler> SchedulerList
		{
			get { return schedulerList; }
		}

		//=========================================================================
		///	<summary>
		///		�S�v���O�C�������[�h����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal static void InitPlugins()
		{
			Assembly plugin;

			schedulerList = new List<Scheduler>();
			
			//-------------------------
			// ����iEPG�v���O�C���o�^
			//-------------------------

			schedulerList.Add(new iEPGScheduler());

			//-----------------------------
			// �v���O�C��DLL��T���ĔF��
			//-----------------------------
			Process		my;
			string		d;
			string		[]dlls;

			my = Process.GetCurrentProcess();

			d = Path.GetDirectoryName( my.MainModule.FileName );

			dlls = Directory.GetFiles(d, "*.dll");

// <MOD> 2010/04/11 ->
			var assemblies	= new List<Assembly>();
			var modules		= new List<Module>();
			var types		= new List<Type>();
			var schedTypes	= new List<Type>();

			Array.ForEach<string>(dlls, delegate (string dll)
				{
					try
					{
						assemblies.Add( Assembly.LoadFrom(dll) );
					}
					catch(Exception ex)
					{
						// ���֌W�ȃA���}�l�[�W�hDLL�Ȃǂ𖳎�
					}
				});

			assemblies.ForEach( assey => modules.AddRange( assey.GetModules() ) );
			modules.ForEach( mod => types.AddRange( mod.GetTypes() ) );
			schedTypes = types.FindAll( type => type.IsSubclassOf(typeof(Scheduler)) );

			schedTypes.ForEach( delegate (Type type)
				{
					Scheduler sched;
					sched = Activator.CreateInstance( type ) as Scheduler;
					if ( sched != null )
						schedulerList.Add( sched );
				} );

//            foreach (string f in dlls)
//            {
//                {
//                    plugin = Assembly.LoadFrom(f);

//                    foreach (Module m in plugin.GetModules())
//                    {
//                        foreach (Type type in m.GetTypes())
//                        {
//                            if (type.IsSubclassOf(typeof(Scheduler)))
//                            {
//                                Scheduler sched;
//                                sched = Activator.CreateInstance( type ) as Scheduler;
//                                if ( sched != null )
//                                {
//                                    schedulerList.Add( sched );
//                                }
//                            }
//                        }
//                    }
//                }
//            }
// <MOD> 2010/04/11 <-
			//sched = plugin.CreateInstance("mAgicScheduler");

			//MessageBox.Show(sched.ToString());

		}

// �� 2008/01/03
		//=========================================================================
		///	<summary>
		///		�풓���Ă���v���O�C�������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal static void CleanupPlugins()
		{
			if( schedulerList != null )
			{
				foreach( Scheduler sched in schedulerList )
				{
					sched.Close();
				}
				schedulerList = null;
			}
		}
// �� 2008/01/03

		//=========================================================================
		///	<summary>
		///		�w�肳�ꂽ�N���X�̃v���O�C���C���X�^���X��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/08/15 �V�K�쐬</history>
		//=========================================================================
		public static Scheduler FindScheduler( string typeName )
		{
			foreach ( Scheduler sched in schedulerList )
			{
				if ( sched.GetType().Name.Equals( typeName ) )
					return sched;
			}
			return null;
		}

// �� 2007/08/15 �ǉ�
		//=========================================================================
		///	<summary>
		///		�f�t�H���g�X�P�W���[����Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public static Scheduler DefaultScheduler
		{
			get
			{
				return FindScheduler( Settings.Default.schedulerType );
			}
		}
// �� 2007/08/15 �ǉ�

// �� 2007/08/16 �ǉ�
		//=========================================================================
		///	<summary>
		///		Scheduler�^�C�v����Profile�^�C�v��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/08/16 �V�K�쐬</history>
		//=========================================================================
		public Type GetProfileType()
		{
			throw new NotImplementedException();
		}
// �� 2007/08/16 �ǉ�

		//=========================================================================
		///	<summary>
		///		�ԑg��V�K�\�񂷂�
		///	</summary>
		/// <remarks>
		///		�\�񏈗��ɏ�Q�����������ꍇ�͗�O�𓊂���
		/// </remarks>
		/// <returns>
		///		���������ꍇ��true
		///		���������X�P�[�W�����ɔ\�͂��Ȃ��ꍇ��false
		/// </returns>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal bool MakeReservation(
			string		title		,	// �^��^�C�g��
			string		uniqueID	,	// �^��̈�ӂ�ID
			string		tvStation	,	// �e���r�ǖ�
			DateTime	start		,	// �J�n����
			uint		length		,	// �^�撷��
			uint		groupID		,	// �^��̃O���[�vID
			Scheduler.Profile param	)	// �t�ѐݒ�
		{
			Scheduler sched	= DefaultScheduler;

			if (sched!=null &&
				(sched.Ability & Scheduler.AbilityFlag.MakeReservation) > 0)	// �X�P�W���[�����w�肳��āA�\�񂪉\���H
			{

				// �t�@�C�����ɂ���Ę^��t�@�C�������H
				if (Settings.Default.specifiedFile == IdentifyFileMethod.ByFileNameWithID)
				{
					title	= string.Format(
								Settings.Default.specifiedNameFormat	,
								uniqueID								,
								title									);		// ID���܂񂾘^��^�C�g���ɂ���
				}

				//--------------------------
				// �^��\�t�g�ɗ\���������
				//--------------------------
				sched.MakeReservation(
					title									,
					uniqueID								,
					GetRegisteredStationName( tvStation )	,
					start									,
					(int)length								,
					title									,
					groupID									,
					param									);

				return true;
			}

			return false;
		}


		//=========================================================================
		///	<summary>
		///		�\���ύX����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/05/08 CancelReservation�ɑΉ�</history>
		//=========================================================================
		internal ChangeResult ChangeReservation(
			string		title		,	// �^��^�C�g��
			string		uniqueID	,	// �^��̈�ӂ�ID
			string		tvStation	,	// �e���r�ǖ�
			DateTime	start		,	// �J�n����
			uint		length		,	// �^�撷��
			uint		groupID		,	// �^��̃O���[�vID
			Scheduler.Profile param	)	// �^��ݒ�
		{
			Scheduler sched = DefaultScheduler;

			if ( sched == null ) return ChangeResult.Dontcare;

			Logger.Output("(�\��)���ԕύX���� " + start.ToString() + "(" + title + ")" );

			//----------------------------
			// �\�Ȃ璼�ڗ\���ύX
			//----------------------------
			if ((sched.Ability & Scheduler.AbilityFlag.ChangeReservation) > 0)
			{
				try
				{
					sched.ChangeReservation( title, uniqueID, start, groupID, param );
				}
				catch ( Exception ex )
				{
					Logger.Output( "(�\��Ǘ�)" + ex.Message );
					return ChangeResult.Denied;
				}

				return ChangeResult.OK;
			}

			//-----------------------------
			// �L�����Z���Ɠo�^���s��
			//-----------------------------
			if ( ( sched.Ability & Scheduler.AbilityFlag.CancelReservation ) > 0 )
			{
				//-----------------------
				// ��U�A�Â��\����폜
				//-----------------------
				try
				{
					sched.CancelReservation( title, uniqueID );
				}
				catch ( Exception ex )
				{
					Logger.Output("(�\��Ǘ�)" + ex.Message);
					return ChangeResult.Denied;
				}

				//-----------------------
				// ���߂ė\���o�^
				//-----------------------
				try
				{
					// �t�@�C�����ɂ���Ę^��t�@�C�������H
					if ( Settings.Default.specifiedFile == IdentifyFileMethod.ByFileNameWithID )
					{
						title = string.Format(
							Settings.Default.specifiedNameFormat	,
							uniqueID								,
							title									); // ID���܂񂾘^��^�C�g���ɂ���
					}

					sched.MakeReservation(
						title									,
						uniqueID								,
						GetRegisteredStationName( tvStation )	,
						start									,
						(int)length								,
						title									,
						groupID									,
						param									);
				}
				catch ( Scheduler.DoubleBookingException dex )
				{
					throw;
//					return ChangeResult.Lost;
				}
				catch ( Exception ex )
				{
					throw;
				}

				return ChangeResult.OK;
			}

// �� 2007/10/28 �v���O�C�����������Ȃ��ꍇ��Don't care��������
//			return ChangeResult.OK;
			return ChangeResult.Dontcare;
// �� 2007/10/28
		}

		//=========================================================================
		///	<summary>
		///		�w�肳�ꂽ�^�C�g���EID�̗\�񂪑��݂��邩�`�F�b�N����
		///	</summary>
		/// <remarks>
		///		�߂�l�̓`�F�b�N�̉ہB
		/// </remarks>
		/// <history>2007/08/16 �V�K�쐬</history>
		//=========================================================================
// <MOD> 2008/10/21 ->
		internal bool ExistReservation(
			string		title		,	// [i] �^�C�g��
			string		uniqueID	,	// [i] �\��ID
			out bool	exist		)	// [o] �\�񂪑��݂���
		//internal bool ExistReservation(
		//    string title	,
		//    string uniqueID)
// <MOD> 2008/10/21 <-
		{
			Scheduler sched;

// <MOD> 2008/10/21 ->
			sched = DefaultScheduler;

// <ADD> 2008/11/16 �f�t�H���g�X�P�W���[�����w�肳��Ă��Ȃ��ꍇ�X���[ ->
			if( sched == null )
			{
				exist  = false;
				return false;
			}
// <ADD> 2008/11/16 <-

			bool existReservation = (0 < (sched.Ability & Scheduler.AbilityFlag.ExistReservation));

			if( (sched != null) && existReservation )
			{
				exist = sched.ExistReservation(title, uniqueID);

				return true;
			}

			exist = false;
			return false;
			//sched = DefaultScheduler;
			//if (sched == null) return true;

			//return sched.ExistReservation(title, uniqueID);
// <MOD> 2008/10/21 <-
		}

		//=========================================================================
		///	<summary>
		///		�\����폜����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/05/08 �V�K�쐬</history>
		//=========================================================================
// <MOD> 2008/10/22 ->
		internal bool CancelReservation(
//		internal void CancelReservation(
// <MOD> 2008/10/22 <-
			string title		,
			string uniqueID		)
		{
			Scheduler sched;

			sched = DefaultScheduler;
// <MOD> 2008/10/22 ->
			if ( sched == null ) return false;
//			if ( sched == null ) return;
// <MOD> 2008/10/22 <-

			if ( ( sched.Ability & Scheduler.AbilityFlag.CancelReservation ) == 0 )
// <MOD> 2008/10/22 ->
				return false;
//				return;
// <MOD> 2008/10/22 <-

			sched.CancelReservation( title, uniqueID );
// <ADD> 2008/10/22 ->
			return true;
// <ADD> 2008/10/22 <-
		}


		//=========================================================================
		///	<summary>
		///		�^��\�t�g�ɗ\��̃R�~�b�g��������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void Flush()
		{
			Scheduler sched = DefaultScheduler;

			if (sched != null)
			{
				sched.Flush();
			}
		}


		//=========================================================================
		///	<summary>
		///		�e���r�ǖ��e�[�u��XML�p�X��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		static public string TvStationsXml	// <MOD> 2009/12/30
//		static private string TvStationsXml
		{
			get
			{
				string filePath;
// <MOD> 2009/12/27 ->
                filePath = Path.Combine( Program.AppDataPath, "tvStationsII.xml" );
//				filePath = Path.GetDirectoryName(Application.ExecutablePath);
//				filePath += Path.DirectorySeparatorChar;
//				filePath += "tvStationsII.xml";
// <MOD> 2009/12/27 <-
				return filePath;
			}
		}

		internal class StationTable
		{
			public string Name;			// �e���r�ǖ�
			public string RegName;			// �o�^�ǖ�
		};

		//=========================================================================
		///	<summary>
		///		�e���r�ǖ��e�[�u����ǂݍ���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		static internal List<StationTable> LoadStationTable()
		{
			XmlReader	r;
			List<StationTable>		list	= new List<StationTable>();
			StationTable			t		= null;

			try
			{
				r = XmlReader.Create(TvStationsXml);

				while (r.Read())
				{
					if (r.NodeType == XmlNodeType.Element)
					{
						if (r.LocalName.ToUpper().Equals("STATION"))
						{
							t = new StationTable();
						}
						if (r.LocalName.ToUpper().Equals("NAME"))
						{
							t.Name = r.ReadElementContentAsString();
						}
						if (r.LocalName.ToUpper().Equals("REGNAME"))
						{
							t.RegName = r.ReadElementContentAsString();
						}

					}
					else if (r.NodeType == XmlNodeType.EndElement)
					{
						if (r.LocalName.ToUpper().Equals("STATION"))
						{
							list.Add(t);
						}
					}

				}

				r.Close();

			}catch (Exception)
			{
				list.Clear();
			}

			return list;
		}

		//=========================================================================
		///	<summary>
		///		�e���r�ǖ��e�[�u���ɓo�^����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		static internal void ModifyStationTable(
			string		n	,	// �e���r�ǖ�
			string		r	)	// �o�^�ǖ�
		{
			List<StationTable>		list	= LoadStationTable();
			bool					w		= true;				// �V�K�ǉ��t���O

			foreach (StationTable e in list)
			{
				if (e.Name.ToUpper().Equals(n.ToUpper()))
				{
					e.RegName	= r;
					w			= false;
				}
			}

			if ( w )
			{
				StationTable e = new StationTable();

				e.Name			= n;
				e.RegName		= r;

				list.Add( e );
			}

			SaveStationTable( list );

		}

		//=========================================================================
		///	<summary>
		///		�e���r�ǖ��e�[�u����ۑ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		static internal void SaveStationTable(List<StationTable> list)
		{
			XmlTextWriter w;

			w				= new XmlTextWriter( TvStationsXml, System.Text.Encoding.UTF8);
			w.Formatting	= Formatting.Indented;

			w.WriteStartDocument();
			w.WriteStartElement("REPLACETABLE");

			foreach (StationTable e in list)
			{
				w.WriteStartElement( "STATION" );

				w.WriteElementString("NAME", e.Name );
				w.WriteElementString("REGNAME", e.RegName);

				w.WriteEndElement();
			}

			w.WriteEndElement();
			w.WriteEndDocument();
			w.Close();

		}
	
		//=========================================================================
		///	<summary>
		///		�e���r�ǖ��e�[�u�������ɁA�e���r�ǖ���u��������
		///	</summary>
		/// <remarks>
		///		��`���Ȃ���Ό��̕���������̂܂ܕԂ�
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		static public string GetRegisteredStationName(string tvStation)
		{
			List<StationTable> list = LoadStationTable();

			foreach (StationTable e in list)
			{
				if (e.Name.ToUpper().Equals(tvStation.ToUpper()))
				{
					return e.RegName;
				}
			}

			return tvStation;
		}


		////////////////////////////////////////////////////////////////
		// FUNCTION	:	CreateDefaultScheduler
		// ABSTRACT	:	
		////////////////////////////////////////////////////////////////
// �� 2007/08/15 �폜
		//internal static Scheduler CreateDefaultScheduler()
		//{
		//    Scheduler s;

		//    try
		//    {
		//        if (Settings.Default.schedulerType.Equals(""))
		//            return null;

		//        //
		//        // �I�v�V�����Ŏw�肳�ꂽScheduler�I�u�W�F�N�g���쐬
		//        //
		//        s = (Scheduler)System.Activator.CreateInstance(
		//                        FindScheduler(Settings.Default.schedulerType)
		//                    );
		//    }
		//    catch (Exception)
		//    {
		//        s = null;
		//    }

		//    return s;
		//}
// �� 2007/08/15 �폜



	}
}
