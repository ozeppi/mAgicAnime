//=========================================================================
///	<summary>
///		KERNEL32 P/Invoke ���b�p�[�N���X
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace KernelAPI
{
	public class FileSystem
	{

		/////////////////////////////////////////////////////////////////////////////
		// FUNCTION	:	KernelAPI::ConvertToShortPathName
		// ABSTRACT	:	�p�X����DOS�V���[�g�t�@�C���p�X���ɕϊ�����
		/////////////////////////////////////////////////////////////////////////////
		static public string ConvertToShortPathName(string longFilePath)
		{
// �� 2008/03/027
//			StringBuilder str = new StringBuilder(260);
			StringBuilder str = new StringBuilder(512);
// �� 2008/03/027
			int len;

			len = GetShortPathName( longFilePath, str, str.Capacity );

			if (len < 0)
				throw new Exception("�t�@�C�����̕ϊ��Ɏ��s���܂���");

			return (string)str.ToString();
		}

		[DllImport("kernel32.dll")]
		private extern static int GetShortPathName(string s1, StringBuilder s2, int i1);
	}

	public class Window
	{
		[DllImport("user32.dll")]
		public extern static IntPtr SendMessage(
			IntPtr hWnd,		// ���M��E�B���h�E�̃n���h�� 
			UInt32 Msg,		// ���b�Z�[�W 
			IntPtr wParam,	// ���b�Z�[�W�̍ŏ��̃p�����[�^ 
			IntPtr lParam	// ���b�Z�[�W�� 2 �Ԗڂ̃p�����[�^ 
		);

		[DllImport("user32.dll")]
		public extern static bool PostMessage(
			IntPtr hWnd,		// ���M��E�B���h�E�̃n���h�� 
			UInt32 Msg,		// ���b�Z�[�W 
			IntPtr wParam,	// ���b�Z�[�W�̍ŏ��̃p�����[�^ 
			IntPtr lParam	// ���b�Z�[�W�� 2 �Ԗڂ̃p�����[�^ 
		);

		[DllImport( "user32.dll" ,EntryPoint = "SendMessage")]
		public extern static IntPtr SendGetTextMessage(
			IntPtr hWnd,		// ���M��E�B���h�E�̃n���h�� 
			UInt32 Msg,		// ���b�Z�[�W 
			IntPtr wParam,	// ���b�Z�[�W�̍ŏ��̃p�����[�^ 
			out string text		// ���b�Z�[�W�� 2 �Ԗڂ̃p�����[�^ 
		);
		
		[DllImport("user32")]
		public static extern UInt32 GetWindowThreadProcessId(
			IntPtr		hWnd,
			out Int32	pid	);

		[DllImport("user32")]
		public static extern bool IsWindow(IntPtr hWnd);

		[DllImport("user32")]
		public static extern int FindWindow(string strclassName, string strWindowName);
		[DllImport("user32")]
		public static extern int FindWindowEx(
			IntPtr hParent,IntPtr hChild,
			string strclassName, string strWindowName);

		public const uint WM_GETTEXT		= 0x000D;
		public const uint WM_COMMAND		= 0x0111;
		public const uint WM_DESTROY		= 0x0002;
		public const uint WM_USER			= 0x0400;
		public const uint WM_KEYDOWN		= 0x0101;
		public const uint WM_KEYUP			= 0x0102;
		public const uint WM_POWERBROADCAST = 0x0218;

		public const uint PBT_APMRESUMESUSPEND	= 0x7;
		public const uint PBT_APMSUSPEND		= 0x4;

		[DllImport("user32")]
		public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

		[DllImport( "user32" )]
		public static extern bool SetWindowText( IntPtr hWindow, string Caption );

		// �R���g���[���̃e�L�X�g���擾
		public static void GetControlText(
			IntPtr			hWnd	,	// ���M��E�B���h�E�̃n���h�� 
			out string		text	)
		{
			byte[]		buffer	= new byte[256];
			GCHandle	gc		= GCHandle.Alloc( buffer, GCHandleType.Pinned );
			int			i;

			Window.SendMessage(
				hWnd					,
				WM_GETTEXT				,
				(System.IntPtr)255		,
				gc.AddrOfPinnedObject()	);

			gc.Free();

			for ( i = 0; buffer[ i ] != '\0'; ++i );

			text = System.Text.Encoding.GetEncoding( 932 ).GetString( buffer ,0,i);
		}

		[DllImport("user32")]
		public static extern bool ReleaseCapture();
	}

	public class Display
	{
		[Flags]
		public enum DispChangeField
		{
			DM_BITSPERPEL		= 0x040000,
			DM_PELSWIDTH		= 0x080000,
			DM_PELSHEIGHT		= 0x100000,
			DM_DISPLAYFLAGS		= 0x200000,
			DM_DISPLAYFREQUENCY	= 0x400000,
		};

		[StructLayout(LayoutKind.Sequential)]
		public struct DEVMODE
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string			dmDeviceName;
			public System.Int16 dmSpecVersion;
			public System.Int16 dmDriverVersion;
			public System.Int16 dmSize;
			public System.Int16 dmDriverExtra;
			public System.Int32 dmFields;
			public System.Int16 dmOrientation;
			public System.Int16 dmPaperSize;
			public System.Int16 dmPaperLength;
			public System.Int16 dmPaperWidth;
			public System.Int16 dmScale;
			public System.Int16 dmCopies;
			public System.Int16 dmDefaultSource;
			public System.Int16 dmPrintQuality;
			public System.Int16 dmColor;
			public System.Int16 dmDuplex;
			public System.Int16 dmYResolution;
			public System.Int16 dmTTOption;
			public System.Int16 dmCollate;
			[MarshalAs(UnmanagedType.ByValTStr,SizeConst=32)]
			public string dmFormName;
			public System.Int16 dmUnusedPadding;
			public System.Int16 dmBitsPerPel;
			public System.Int32 dmPelsWidth;
			public System.Int32 dmPelsHeight;
			public System.Int32 dmDisplayFlags;
			public System.Int32 dmDisplayFrequency;
		} ;

		[DllImport("user32")]
		extern private static System.Int32 ChangeDisplaySettings(ref DEVMODE devMode,System.Int32 Flags);
		[DllImport("user32")]
		extern private static System.Int32 ChangeDisplaySettings(IntPtr nullPtr, System.Int32 Flags);
	
		public static void SetMode(int w,int h)
		{
			DEVMODE devMode = new DEVMODE();

			devMode.dmSize			= (short)Marshal.SizeOf(devMode);
			devMode.dmPelsWidth		= w;
			devMode.dmPelsHeight	= h;
			devMode.dmFields		= (int)(DispChangeField.DM_PELSWIDTH | DispChangeField.DM_PELSHEIGHT);

			ChangeDisplaySettings(ref devMode, 0);
		}

		public static void ResetMode()
		{
			ChangeDisplaySettings((IntPtr)null, 0);
		}

	}

	public class GDI
	{
		[DllImport("USER32")]
		extern public static System.Int32 GetDC(System.Int32 hWindow);

		[DllImport("GDI32")]
		extern public static System.Int32 GetTextMetrics(System.Int32 hDC,ref TEXTMETRIC tm);
		[StructLayout(LayoutKind.Sequential)]
		public struct TEXTMETRIC
		{
			public UInt32 tmHeight;
			public UInt32 tmAscent;
			public UInt32 tmDescent;
			public UInt32 tmInternalLeading;
			public UInt32 tmExternalLeading;
			public UInt32 tmAveCharWidth;
			public UInt32 tmMaxCharWidth;
			public UInt32 tmWeight;
			public UInt32 tmOverhang;
			public UInt32 tmDigitizedAspectX;
			public UInt32 tmDigitizedAspectY;

			public System.Byte tmFirstChar;
			public System.Byte tmLastChar;
			public System.Byte tmDefaultChar;
			public System.Byte tmBreakChar;
			public System.Byte tmItalic;
			public System.Byte tmUnderlined;
			public System.Byte tmStruckOut;
			public System.Byte tmPitchAndFamily;
			public System.Byte tmCharSet;
		}
	
	}

	public class PrivateProfile
	{
		private const int maxLen = 255;

		[DllImport( "kernel32.dll" )]
		private static extern uint GetPrivateProfileString(
			  string ApplicationName
			, string KeyName
			, string Default
			, System.Text.StringBuilder StringBuilder
			, uint nSize
			, string FileName );
		[DllImport( "kernel32.dll" )]
		private static extern uint WritePrivateProfileString(
			  string ApplicationName
			, string KeyName
			, string Parameter
			, string FileName );

		public void Open( string filePath )
		{
			if( this.filePath != null )
				throw new Exception( "�����G���[: ���ɊJ����Ă��܂�" );

			if( !System.IO.File.Exists( filePath ) )
				throw new Exception( "�����G���[: �t�@�C����������܂���" );

			this.filePath = filePath;
		}

		public string GetKeyString(
			string SectionName,
			string KeyName,
			string Default )
		{
			StringBuilder sb = new StringBuilder( maxLen );

			uint ret = GetPrivateProfileString(
				SectionName, KeyName, Default,
				sb, Convert.ToUInt32( sb.Capacity ), filePath );

			return sb.ToString();
		}

		public void WriteKeyString(
			string SectionName,
			string KeyName,
			string Parameter )
		{

			WritePrivateProfileString( SectionName, KeyName, Parameter, filePath );

		}

		public void Close()
		{
			this.filePath = null;
		}

		private string filePath = null;
	}

	public class InterProcess
	{
		[DllImport( "kernel32.dll" )]
		public extern static IntPtr OpenProcess(
			UInt32 DesiredAccess,
			System.Int32 Inherit,
			System.Int32 ProcessId
		);

		public const int PROCESS_VM_OPERATION	= 0x08;
		public const int PROCESS_VM_READ		= 0x10;
		public const int PROCESS_VM_WRITE		= 0x20;

		[DllImport( "kernel32.dll" )]
		public extern static IntPtr VirtualAllocEx(
		  IntPtr hProcess,         // ���蓖�Ă�����������ێ�����v���Z�X
		  IntPtr lpAddress,        // ���蓖�Ă����J�n�A�h���X
		  System.Int32 dwSize,            // ���蓖�Ă����̈�̃o�C�g�P�ʂ̃T�C�Y
		  System.Int32 flAllocationType,  // ���蓖�Ẵ^�C�v
		  System.Int32 flProtect          // �A�N�Z�X�ی�̃^�C�v
		);

		public const int MEM_RESERVE		= 0x2000;
		public const int MEM_COMMIT		= 0x1000;
		public const int MEM_RELEASE		= 0x8000;

		public const int PAGE_READWRITE	= 0x0004;

		[DllImport( "kernel32.dll" )]
		public extern static System.Int32 VirtualFreeEx(
			IntPtr hProcess,  // �����������������ێ�����v���Z�X
			IntPtr lpAddress, // ����������������̈�̊J�n�A�h���X
			UInt32 dwSize,     // ����������������̈�̃o�C�g�P�ʂ̃T�C�Y
			UInt32 dwFreeType  // �������̃^�C�v
		);

		[DllImport( "kernel32.dll" )]
		public extern static System.Int32 WriteProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			IntPtr lpBuffer,
			System.Int32 nSize,
			out System.Int32 lpNumberOfBytesWritten
		);

		[DllImport( "kernel32.dll" )]
		public extern static System.Int32 ReadProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			IntPtr lpBuffer,
			System.Int32 nSize,
			out System.Int32 lpNumberOfBytesRead
		);
	}

    public class Platform
    {
        public static System.Int32 EWX_SHUTDOWN = 0x0001;
// �� 2008/03/20
		public static System.Int32 EWX_POWEROFF = 0x08;
// �� 2008/03/20
        [DllImport( "user32.dll" )]
        public extern static bool ExitWindowsEx(
          System.Int32 uFlags       , // �V���b�g�_�E������
          System.Int32 dwReserved     // �\��ς�
        );

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();

        [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle,
            uint DesiredAccess,
            out IntPtr TokenHandle);

        [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true,
            CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool LookupPrivilegeValue(string lpSystemName,
            string lpName,
            out long lpLuid);

        [System.Runtime.InteropServices.StructLayout(
           System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        public struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            public long Luid;
            public int Attributes;
        }

        [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(
            IntPtr TokenHandle,
            bool DisableAllPrivileges,
            ref TOKEN_PRIVILEGES NewState,
            int BufferLength,
            IntPtr PreviousState,
            IntPtr ReturnLength);
    }

// <ADD> 2010/05/01 ->
	public class WinThread
	{
		[Flags]
		public enum ThreadAccess : int
		{
		  SUSPEND_RESUME = 2,
		}

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenThread(
			ThreadAccess dwDesiredAccess,
			bool bInheritHandle,
			int dwThreadId);
		[DllImport("kernel32.dll")]
		public static extern uint SuspendThread(IntPtr hThread);
		[DllImport("kernel32.dll")]
		public static extern int ResumeThread(IntPtr hThread);

		// �v���Z�X�̑S�ẴX���b�h�ɑ΂��ăT�X�y���h�^���W���[��
		public static void SuspendResumeAllThread(Process proc, bool suspend)
		{
			foreach(ProcessThread pt in proc.Threads)
			{
				const ThreadAccess access = ThreadAccess.SUSPEND_RESUME;
				IntPtr	hThread;

				hThread = WinThread.OpenThread(access, false, pt.Id);

				if( suspend )
					WinThread.SuspendThread(hThread);
				else
					WinThread.ResumeThread(hThread);

				KernelAPI.General.CloseHandle(hThread);
			}
		}
	}

	public class General
	{
		[DllImport("kernel32.dll")]
		public static extern void CloseHandle(IntPtr hObject);
	}
// <ADD> 2010/05/01 <-

}

