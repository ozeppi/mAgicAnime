//=========================================================================
///	<summary>
///		アンマネージヘルパ
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2010/01/XX 新規作成	Dr.Kurusugawa</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace Helpers
{
	// アンマネージメモリを管理するクラス
	public class UnmanageMemory : IDisposable
	{
		public UnmanageMemory(int bufferSize)
		{
			mRawData	= new Byte[bufferSize];
			mHandle		= GCHandle.Alloc( mRawData, GCHandleType.Pinned );
		}
		public void Dispose()
		{
			mHandle.Free();
			mRawData = null;
		}

		// メモリのバイト列を返す
		public Byte[] GetBytes() { return mRawData; }
		// メモリサイズを返す
		public int Size { get{ return mRawData.Length; } }
		// メモリアドレスを返す
		public IntPtr GetAddress() { return mHandle.AddrOfPinnedObject(); }

		private Byte[]		mRawData;
		private GCHandle	mHandle;
	}

	// 構造体をBYTE[]に変換するヘルパクラス
	public class StructToBytesHelper : UnmanageMemory
	{
		public StructToBytesHelper( object target )
			: base(Marshal.SizeOf(target))
		{
			mTarget = target;
			Marshal.StructureToPtr( target, GetAddress(), true );
		}

		private object		mTarget;
	}
}