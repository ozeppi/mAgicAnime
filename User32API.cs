//=========================================================================
///	<summary>
///		USER32 P/Invoke ラッパークラス
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成</history>
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
using KernelAPI;

namespace User32API
{
	public class ListViewControl
	{
		[StructLayout( LayoutKind.Sequential )]
		public struct LVITEM
		{
			public System.UInt32 mask;
			public System.Int32 iItem;
			public System.Int32 iSubItem;
			public System.UInt32 state;
			public System.UInt32 stateMask;
			public System.IntPtr pszText;
			public System.Int32 cchTextMax;
			public System.Int32 iImage;
			public System.Int32 lParam;
			public System.Int32 iIndent;
			public System.Int32 iGroupId;
			public System.Int32 cColumns;
			public System.Int32 puColumns;
		}

		public const int LVM_FIRST = 0x1000;
		public const int LVM_GETITEMCOUNT = LVM_FIRST + 4;
		public const int LVM_SETITEMSTATE = LVM_FIRST + 43;
		public const int LVM_GETITEMTEXT = LVM_FIRST + 45;

		public const int LVIS_STATEIMAGEMASK = 0xF000;
		public const int LVIS_FOCUSED = 1;
		public const int LVIS_SELECTED = 2;

		public const int LVIF_STATE = 0x0008;
		public const int LVIF_TEXT = 0x0001;

		// ABSTRACT	: ListViewにメッセージを送る
		public static System.IntPtr SendListViewMessage(
			int						pid		,	// ターゲットのプロセスID
			System.IntPtr			hListBox,	// リストボックスハンドル
			System.UInt32			Message	,	// 送信するメッセージ
			int						Row		,	// 行数
			ListViewControl.LVITEM	lvitem	,	// アイテム
			ref string				text	)	// 文字列
		{
			System.IntPtr	hAccess;
			System.IntPtr	mem;
			System.IntPtr	result;
			GCHandle		gctemp;
			int				n;

			hAccess = InterProcess.OpenProcess(
				InterProcess.PROCESS_VM_OPERATION	|
				InterProcess.PROCESS_VM_READ		|
				InterProcess.PROCESS_VM_WRITE		,
				0									,
				pid									);

			mem = InterProcess.VirtualAllocEx(
				hAccess						,
				(System.IntPtr)0			,
				4096						,
				InterProcess.MEM_COMMIT		|
				InterProcess.MEM_RESERVE	,
				InterProcess.PAGE_READWRITE	);

			// テキスト取得の場合の処理
			if( Message == LVM_GETITEMTEXT )
			{
				lvitem.mask			= LVIF_TEXT;
				lvitem.pszText		= (System.IntPtr)((int)mem + 0x100);
				lvitem.cchTextMax	= 250;
			}

			// ListViewControl.LVITEM構造体を相手プロセスに書き込む

			gctemp = GCHandle.Alloc( lvitem, GCHandleType.Pinned );

			System.IntPtr a = gctemp.AddrOfPinnedObject();

			InterProcess.WriteProcessMemory(
				hAccess					,
				mem						,
				a						,
				Marshal.SizeOf( lvitem ),
				out n					);

			// メッセージ送信
			result = Window.SendMessage(
				hListBox				,
				Message					,
				(System.IntPtr)Row		,
				mem						);


			if( Message == LVM_GETITEMTEXT )
			{
				byte[] buffer = new byte[256];

				GCHandle gctext = GCHandle.Alloc( buffer, GCHandleType.Pinned );

				InterProcess.ReadProcessMemory(
					hAccess						,
					lvitem.pszText				,
					gctext.AddrOfPinnedObject()	,
					250							,
					out n						);

				gctext.Free();

				string aa;

				text = Encoding.GetEncoding( 932 ).GetString( buffer, 0, 250 );

				Console.Write( text );
			}

			gctemp.Free();

			InterProcess.VirtualFreeEx(
				hAccess					,
				mem						,
				4096					,
				InterProcess.MEM_RELEASE );

			return result;
		}
	}

	public class DateTimeControl
	{
		public struct SYSTEMTIME
		{
			public System.UInt16 wYear;
			public System.UInt16 wMonth;
			public System.UInt16 wDayOfWeek;
			public System.UInt16 wDay;
			public System.UInt16 wHour;
			public System.UInt16 wMinute;
			public System.UInt16 wSecond;
			public System.UInt16 wMilliseconds;
		};

		const	int		DTM_FIRST			= 0x1000;
		const	int		DTM_SETSYSTEMTIME   = (DTM_FIRST + 2);

		//
		// ABSTRACT	: DateTimeコントロールにメッセージを送る
		//
		public static System.IntPtr SetSystemTime(
			int				pid			, // ターゲットのプロセスID
			System.IntPtr	hDateTime	, // DATETIMEコントロールハンドル
			SYSTEMTIME		sysTime		) // SYSTEMTIME構造体
		{
			System.IntPtr	hAccess;
			System.IntPtr	mem;
			System.IntPtr	result;
			GCHandle		gctemp;
			int				n;

			hAccess = InterProcess.OpenProcess(
				InterProcess.PROCESS_VM_OPERATION	|
				InterProcess.PROCESS_VM_READ		|
				InterProcess.PROCESS_VM_WRITE		,
				0									,
				pid									);

			mem = InterProcess.VirtualAllocEx(
				hAccess						,
				(System.IntPtr)0			,
				4096						,
				InterProcess.MEM_COMMIT		|
				InterProcess.MEM_RESERVE	,
				InterProcess.PAGE_READWRITE	);

			// 構造体を相手プロセスに書き込む

			gctemp = GCHandle.Alloc( sysTime, GCHandleType.Pinned );

			System.IntPtr a = gctemp.AddrOfPinnedObject();

			InterProcess.WriteProcessMemory(
				hAccess					,
				mem						,
				a						,
				Marshal.SizeOf( sysTime ),
				out n					);


			result = Window.SendMessage(
				hDateTime			,
				DTM_SETSYSTEMTIME	,
				System.IntPtr.Zero	,		// GDT_VALID
				mem					);

			gctemp.Free();

			InterProcess.VirtualFreeEx( hAccess, mem, 4096, InterProcess.MEM_RELEASE );

			return result;
		}
	}
}

