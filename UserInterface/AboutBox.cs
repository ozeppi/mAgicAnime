//=========================================================================
///	<summary>
///		バージョン情報ダイアログ
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成	Dr.Kurusugawa</history>
//=========================================================================
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace magicAnime
{
	partial class AboutBox : Form
	{
		readonly string copyrightFile	= @"COPYRIGHT.TXT";
		readonly string licenseFile		= @"LICENSE.TXT";

		internal AboutBox()
		{
			InitializeComponent();

			SetComponent();

			// ライセンス情報ファイルの絶対パスを求める
			string	exeDir;
			exeDir = Path.GetDirectoryName( Application.ExecutablePath );
			copyrightFile	= Path.Combine( exeDir, copyrightFile );
			licenseFile		= Path.Combine( exeDir, licenseFile );
		}
		
		private void SetComponent()
		{

			//  アセンブリ情報からの製品情報を表示する情報ボックスを初期化します。
			//  アプリケーションのアセンブリ情報設定を次のいずれかにて変更します:
			//  - [プロジェクト] メニューの [プロパティ] にある [アプリケーション] の [アセンブリ情報]
			//  - AssemblyInfo.cs
			this.Text = String.Format("{0} のバージョン情報", AssemblyTitle);
			this.labelProductName.Text = AssemblyProduct;
// 2009/04/14 ->
			this.labelVersion.Text = String.Format("(アセンブリバージョン {0})", AssemblyVersion);
//			this.labelVersion.Text = String.Format("内部バージョン {0}", AssemblyVersion);
// 2009/04/14 <-
			this.labelProductVersion.Text = String.Format("バージョン {0}", Application.ProductVersion);
//			this.labelCopyright.Text = AssemblyCopyright;
//			this.labelCompanyName.Text = AssemblyCompany;

// <ADD> 2010/02/20 ->
			//------------------------------------------
			// COPYRIGHT.TXTおよびLICENSE.TXTを表示
			//------------------------------------------
			string		copyright;
			string		license;

			try
			{
				TextReader	txt			= new StreamReader(copyrightFile);
				copyright = txt.ReadToEnd();
				txt.Close();
			}
			catch(Exception ex)
			{
				copyright = copyrightFile + " is not found.";
			}

			try
			{
				TextReader	txt			= new StreamReader(licenseFile);
				license = txt.ReadToEnd();
				txt.Close();
			}
			catch(Exception ex)
			{
				license = licenseFile + " is not found.";
			}

			Array.ForEach<string>(	copyright.Split('\n'),
									line => licenseListBox.Items.Add( line ) );
			licenseListBox.Items.Add( "-------------------------------------------------------" );
			Array.ForEach<string>(	license.Split('\n'),
									line => licenseListBox.Items.Add( line ) );
// <ADD> 2010/02/20 <-
		}
		
		internal void ShowSplash()
		{
		
			this.StartPosition = FormStartPosition.CenterScreen;
			this.TopMost = true;
		
			Show();
			
			okButton.Visible = false;
		
			Update();
		
			timer.Start();
		}

		#region アセンブリ属性アクセサ

		public string AssemblyTitle
		{
			get
			{
				// このアセンブリ上のタイトル属性をすべて取得します
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				// 少なくとも 1 つのタイトル属性がある場合
				if (attributes.Length > 0)
				{
					// 最初の項目を選択します
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					// 空の文字列の場合、その項目を返します
					if (titleAttribute.Title != "")
						return titleAttribute.Title;
				}
				// タイトル属性がないか、またはタイトル属性が空の文字列の場合、.exe 名を返します
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public string AssemblyDescription
		{
			get
			{
				// このアセンブリ上の説明属性をすべて取得します
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				// 説明属性がない場合、空の文字列を返します
				if (attributes.Length == 0)
					return "";
				// 説明属性がある場合、その値を返します
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyProduct
		{
			get
			{
				// このアセンブリ上の製品属性をすべて取得します
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				// 製品属性がない場合、空の文字列を返します
				if (attributes.Length == 0)
					return "";
				// 製品属性がある場合、その値を返します
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				// このアセンブリ上の著作権属性をすべて取得します
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				// 著作権属性がない場合、空の文字列を返します
				if (attributes.Length == 0)
					return "";
				// 著作権属性がある場合、その値を返します
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string AssemblyCompany
		{
			get
			{
				// このアセンブリ上の会社属性をすべて取得します
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				// 会社属性がない場合、空の文字列を返します
				if (attributes.Length == 0)
					return "";
				// 会社属性がある場合、その値を返します
				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}
		#endregion

		// 自動的に消す
		private void timer_Tick(object sender, EventArgs e)
		{
			Close();
		}

		private void okButton_Click(object sender, EventArgs e)
		{

		}
	}

}
