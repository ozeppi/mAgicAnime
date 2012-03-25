//=========================================================================
///	<summary>
///		エンコードマネージャクラス
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成	Dr.Kurusugawa</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;

// 内部プラグイン
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

			// 内部プラグインの登録

			encoderList.Add( typeof(CmdLineEncoderPlugin) );

			//----------------------------------------------------
			// フォルダ上のDLLを列挙してプラグインクラスを探す
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
						// 無関係なアンマネージドDLLなどを無視
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
		// ABSTRACT	:	指定されたクラスのエンコーダクラスを探す
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
