//=========================================================================
///	<summary>
///		�G���R�[�h�}�l�[�W���N���X
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬	Dr.Kurusugawa</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;

// �����v���O�C��
using magicAnime.RecordingManager.EncodeManager.CmdLineEncoder;

namespace magicAnime
{
	class EncodeManager
	{
		private static List<Type> encoderList;

		public static List<Type> EncoderList
		{
			get { return encoderList; }
		}

		internal static void InitPlugins()
		{
			Assembly plugin;

			encoderList = new List<Type>();

			// �����v���O�C���̓o�^

			encoderList.Add( typeof(CmdLineEncoderPlugin) );

			//----------------------------------------------------
			// �t�H���_���DLL��񋓂��ăv���O�C���N���X��T��
			//----------------------------------------------------
			Process		myProcess;
			string		currentDir;
			string[]	dllFiles;

			myProcess = Process.GetCurrentProcess();

			currentDir = Path.GetDirectoryName(myProcess.MainModule.FileName);

			dllFiles = Directory.GetFiles(currentDir, @"*.dll");

// <MOD> 2010/04/11 ->
			var assemblies	= new List<Assembly>();
			var modules		= new List<Module>();
			var types		= new List<Type>();

			Array.ForEach<string>(dllFiles, delegate (string dll)
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

			encoderList.AddRange( types.FindAll( type => type.IsSubclassOf(typeof(Encoder)) ) );

//            {
//                plugin = Assembly.LoadFrom(file);
////				Console.WriteLine(plugin.ToString());

//                foreach (Module module in plugin.GetModules())
//                {
//                    foreach (Type type in module.GetTypes())
//                    {
//                        if (type.IsSubclassOf(typeof(Encoder)))
//                        {
//                            encoderList.Add(type);
////							Console.WriteLine(type.ToString());
//                        }
//                    }
//                }
//            }
// <MOD> 2010/04/11 <-
			//scheduler = plugin.CreateInstance("mAgicScheduler");

			//MessageBox.Show(scheduler.ToString());

		}

		////////////////////////////////////////////////////////////////
		// FUNCTION	:	FindEncoder
		// ABSTRACT	:	�w�肳�ꂽ�N���X�̃G���R�[�_�N���X��T��
		////////////////////////////////////////////////////////////////
		public static Type FindEncoder(string fullName)
		{

			foreach (Type type in encoderList)
			{
				if (type.FullName.Equals(fullName))
				{
					return type;
				}
			}

			return null;
		}


	}

}
