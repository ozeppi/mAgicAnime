//=========================================================================
///	<summary>
///		サムネイル作成クラス
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
using Helpers;

namespace MakeThumbnail
{
	class DirectShow
	{
		//----------------------------------
		// DirectShow COM定義
		// (必要な部分のみの簡易定義)
		//----------------------------------
		public static readonly UInt32	S_OK	= 0;
		public static readonly UInt32	S_FALSE	= 1;

		public static readonly int	AM_SEEKING_NoPositioning		= 0x0;
		public static readonly int	AM_SEEKING_AbsolutePositioning	= 0x1;

		public static readonly Guid CLSID_FilterGraph =
				new Guid("e436ebb3-524f-11ce-9f53-0020af0ba770");

		[ComImport]
		[Guid("56a868a9-0ad4-11ce-b03a-0020af0ba770")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)] 
		public interface IGraphBuilder
		{
			void AddFilter(IntPtr pFilter, string pName);
			void RemoveFilter(IntPtr pFilter);
			void EnumFilters(out IntPtr ppEnum);
			void FindFilterByName(string pName, out IntPtr ppFilter);
			void ConnectDirect(IntPtr ppinOut, IntPtr ppinIn, [In, Out] IntPtr pmt);
			void Reconnect(IntPtr ppin);
			void Disconnect(IntPtr ppin);
			void SetDefaultSyncSource();
			//
			void Connect(IntPtr ppinOut, IntPtr ppinIn);
			void Render(IntPtr ppinOut);
			[PreserveSig]
			Int32 RenderFile(string lpcwstrFile, string lpcwstrPlayList);
			void AddSourceFilter(string lpcwstrFileName, string lpcwstrFilterName, out IntPtr ppFilter);
			void SetLogFile(IntPtr hFile);
			void Abort();
			void ShouldOperationContinue();
		}

		[ComImport]
		[Guid("56A868B1-0AD4-11CE-B03A-0020AF0BA770")]
		[InterfaceType(ComInterfaceType.InterfaceIsDual)] 
		public interface IMediaControl
		{
			void Run();

			void Pause();
		    
			void Stop();

			void GetState( [In] int msTimeout, [Out] out int pfs);

			void RenderFile(
			[In, MarshalAs(UnmanagedType.BStr)] string strFilename);

			void AddSourceFilter(
			[In, MarshalAs(UnmanagedType.BStr)] string strFilename, 
			[Out, MarshalAs(UnmanagedType.Interface)] out object ppUnk);

			[return : MarshalAs(UnmanagedType.Interface)]
			object FilterCollection();

			[return : MarshalAs(UnmanagedType.Interface)]
			object RegFilterCollection();
		    
			void StopWhenReady(); 
		}

		[ComImport]
		[Guid("36B73880-C2C8-11CF-8B46-00805F6CEF60")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IMediaSeeking
		{
			[PreserveSig]
			int GetCapabilities(out Int32 pCapabilities);

			[PreserveSig]
			int CheckCapabilities([In, Out] ref Int32 pCapabilities);

			[PreserveSig]
			int IsFormatSupported([In] ref Guid pFormat);

			[PreserveSig]
			int QueryPreferredFormat([Out] out Guid pFormat);

			[PreserveSig]
			int GetTimeFormat([Out] out Guid pFormat);

			[PreserveSig]
			int IsUsingTimeFormat([In] ref Guid pFormat);

			[PreserveSig]
			int SetTimeFormat([In] ref Guid pFormat);

			[PreserveSig]
			int GetDuration(out long pDuration);

			[PreserveSig]
			int GetStopPosition(out long pStop);

			[PreserveSig]
			int GetCurrentPosition(out long pCurrent);

			[PreserveSig]
			int ConvertTimeFormat(
				out long pTarget,
				[In] ref Guid pTargetFormat,
				long Source, 
				[In] ref Guid pSourceFormat);

			[PreserveSig]
			int SetPositions(
				[In, Out] ref long pCurrent,
				Int32 dwCurrentFlags,
				IntPtr pStop,
//				[In, Out] ref long pStop,
				Int32 dwStopFlags);

			[PreserveSig]
			int GetPositions(
				out long pCurrent,
				out long pStop);

			[PreserveSig]
			int GetAvailable(
				out long pEarliest,
				out long pLatest);

			[PreserveSig]
			int SetRate(double dRate);

			[PreserveSig]
			int GetRate(out double pdRate);

			[PreserveSig]
			int GetPreroll(out long pllPreroll);
		}

		[ComImport]
		[Guid("56a868b5-0ad4-11ce-b03a-0020af0ba770")]
		[InterfaceType(ComInterfaceType.InterfaceIsDual)] 
		public interface IBasicVideo
		{
			void get_AvgTimePerFrame( 
				[Out] out long AvgTimePerFrame);
	        
			void get_BitRate( 
				[Out] out long pBitRate);
	        
			void get_BitErrorRate( 
				[Out] out long pBitErrorRate);
	        
			[PreserveSig]
			int get_VideoWidth( 
				[Out] out long pVideoWidth);

	        [PreserveSig]
			int get_VideoHeight( 
				[Out] out long pVideoHeight);
	        
			void put_SourceLeft( 
				long SourceLeft);
	        
			void get_SourceLeft( 
				out long pSourceLeft);
	        
			void put_SourceWidth( 
				long SourceWidth);
	        
			void get_SourceWidth( 
				out long pSourceWidth);
	        
			void put_SourceTop( 
				long SourceTop);
	        
			void get_SourceTop( 
				out long pSourceTop);
	        
			void put_SourceHeight( 
				long SourceHeight);
	        
			void get_SourceHeight(
				out long pSourceHeight);
	        
			void put_DestinationLeft( 
				out long DestinationLeft);
	        
			void get_DestinationLeft( 
				out long pDestinationLeft);
	        
			void put_DestinationWidth( 
				long DestinationWidth);
	        
			void get_DestinationWidth( 
				out long pDestinationWidth);
	        
			void put_DestinationTop( 
				out long DestinationTop);
	        
			void get_DestinationTop( 
				out long pDestinationTop);
	        
			void put_DestinationHeight( 
				long DestinationHeight);
	        
			void get_DestinationHeight( 
				out long pDestinationHeight);
	        
			void SetSourcePosition( 
				long Left,
				long Top,
				long Width,
				long Height);
	        
			void GetSourcePosition( 
				out long pLeft,
				out long pTop,
				out long pWidth,
				out long pHeight);
	        
			void SetDefaultSourcePosition();
	        
			void SetDestinationPosition( 
				long Left,
				long Top,
				long Width,
				long Height);
	        
			void GetDestinationPosition( 
				out long pLeft,
				out long pTop,
				out long pWidth,
				out long pHeight);
	        
			void SetDefaultDestinationPosition();
	        
			void GetVideoSize( 
				out long pWidth,
				out long pHeight);
	        
			void GetVideoPaletteEntries( 
				long StartIndex,
				long Entries,
				out long pRetrieved,
				out long pPalette);
	        
			void GetCurrentImage( 
				[Out][In] ref long pBufferSize,
				IntPtr pImage );
	        
			void IsUsingDefaultSource();
	        
			void IsUsingDefaultDestination();
		}
	}

	public class VideoImage : IDisposable
	{
		//--------------------------
		// BMPファイル構造体
		//--------------------------
		[StructLayout(LayoutKind.Sequential,Pack=1)]
		public struct BITMAPFILEHEADER
		{
			public UInt16	bfType;
			public UInt32	bfSize;
			public UInt16	bfReserved1;
			public UInt16	bfReserved2;
			public UInt32	bfOffBits;
		};

		[StructLayout(LayoutKind.Sequential,Pack=4)]
		public struct BITMAPINFOHEADER
		{
			public UInt32	biSize;
			public Int32	biWidth;
			public Int32	biHeight;
			public UInt16	biPlanes;
			public UInt16	biBitCount;
			public UInt32	biCompression;
			public UInt32	biSizeImage;
			public Int32	biXPelsPerMeter;
			public Int32	biYPelsPerMeter;
			public UInt32	biClrUsed;
			public UInt32	biClrImportant;
		};

		//---------------
		// 例外クラス
		//---------------
		public class CannotOpenException : Exception
		{
		}
		public class RenderFailedException : Exception
		{
		}

		// メンバ
		private DirectShow.IGraphBuilder	mGraphBuilder;
		private DirectShow.IMediaControl	mMediaControl;
		private DirectShow.IMediaSeeking	mMediaSeek;
		private DirectShow.IBasicVideo		mBasicVideo;

		//=========================================================================
		///	<summary>
		///		コンストラクタ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/24 新規作成</history>
		//=========================================================================
		public VideoImage( string SourcePath ) // [i] 動画ファイルパス
		{
			try
			{
				mGraphBuilder = (DirectShow.IGraphBuilder)Activator.CreateInstance(
					                Type.GetTypeFromCLSID(DirectShow.CLSID_FilterGraph));

				mMediaControl	= mGraphBuilder as DirectShow.IMediaControl;
				mMediaSeek		= mGraphBuilder as DirectShow.IMediaSeeking;
				mBasicVideo		= mGraphBuilder as DirectShow.IBasicVideo;

				mMediaControl.RenderFile( SourcePath );
			}
			catch(Exception ex)
			{
				throw new CannotOpenException();
			}
		}

		public void Dispose()
		{
			mGraphBuilder.Abort();

			mGraphBuilder	= null;
			mMediaControl	= null;
			mMediaSeek		= null;
			mBasicVideo		= null;
		}

		//=========================================================================
		///	<summary>
		///		動画の指定されたフレームのBMPストリームを作成
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/24 新規作成</history>
		//=========================================================================
		unsafe public MemoryStream GetFrameImage(
			long	pos			)	// [i] 撮影位置[ms]
		{
			try
			{
				long			width;
				long			height;
				long			bufSize			= 0;

				//-------------------------
				// 指定位置までシーク
				//-------------------------
				Guid TIME_FORMAT_MEDIA_TIME = new Guid("7b785574-8c82-11cf-bc0c-00aa00ac74f6");

				mMediaSeek.SetTimeFormat( ref TIME_FORMAT_MEDIA_TIME );

				long	posMicroSec	= pos * 1000 * 1000 * 10; // per 0.1microsec

				int Result = 0;

				Result = mMediaSeek.SetPositions(
					ref posMicroSec ,
					DirectShow.AM_SEEKING_AbsolutePositioning,
					IntPtr.Zero,
					DirectShow.AM_SEEKING_NoPositioning );

				if( Result != DirectShow.S_OK )
					throw new RenderFailedException();

				//--------------------------
				// フレームイメージ取り込み
				//--------------------------

				mBasicVideo.get_VideoWidth(out width);
				mBasicVideo.get_VideoHeight(out height);

				if( width <= 0 || height <= 0 )
					throw new RenderFailedException();

				mBasicVideo.GetCurrentImage(ref bufSize, IntPtr.Zero);

				if( bufSize <= 0 )
					throw new RenderFailedException();
				
				using( UnmanageMemory bmpMem = new UnmanageMemory( (int)bufSize ) )
				{
					mBasicVideo.GetCurrentImage( ref bufSize, bmpMem.GetAddress() );


					//------------------------------------------------
					// BMPファイルイメージをメモリストリームへ書き出し
					//------------------------------------------------
					BITMAPFILEHEADER	bmphdr	= new BITMAPFILEHEADER();
					BITMAPINFOHEADER	bmpinfo	= new BITMAPINFOHEADER();

					MemoryStream fs = new MemoryStream( (int)bufSize );

					bmphdr.bfType		= ('M' << 8) | 'B';
					bmphdr.bfSize		= (uint)(Marshal.SizeOf(bmphdr) + Marshal.SizeOf(bmpinfo) + bufSize);
					bmphdr.bfOffBits	= (uint)(Marshal.SizeOf(bmphdr) + Marshal.SizeOf(bmpinfo));

					bmpinfo.biSize		= (uint)(Marshal.SizeOf(bmpinfo));
					bmpinfo.biWidth		= (int)width;
					bmpinfo.biHeight	= (int)height;
					bmpinfo.biPlanes	= 1;
					bmpinfo.biBitCount	= 32;
					
					StructToBytesHelper	bmphdrBytes  = new StructToBytesHelper( bmphdr );
					StructToBytesHelper	bmpinfoBytes = new StructToBytesHelper( bmpinfo );

					fs.Write( bmphdrBytes.GetBytes() , 0, bmphdrBytes.Size );
					fs.Write( bmpinfoBytes.GetBytes(), 0, bmpinfoBytes.Size );
					fs.Write( bmpMem.GetBytes(), 0, (int)bufSize );

					return fs;
				}
			}
			catch(Exception ex)
			{
				throw new RenderFailedException();
			}
		}
	}
}
