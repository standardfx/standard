using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System.Security;

namespace Standard.Win32
{
	public static class WinapiUtility
	{
		/// <summary>When an exception is raised, bit shifting is needed to prevent: "System.OverflowException: Arithmetic operation resulted in an overflow."</summary>
		public const int OverflowExceptionBitShift = 65535;
		public const int Ignored = (int)NativeMethods.S_OK;

		#region Lo/Hi word

		public static uint GetHiWord(long highPart)
		{
			return (uint)((highPart >> 32) & 0xFFFFFFFF);
		}

		public static uint GetLoWord(long lowPart)
		{
			return (uint)(lowPart & 0xFFFFFFFF);
		}

		public static ushort GetLoWord(this uint n)
		{
			return (ushort)n;
		}

		public static short GetLoWord(this int n)
		{
			return (short)(n & 0xffff);
		}

		public static ushort GetHiWord(this uint n)
		{
			return (ushort)(n >> 16);
		}

		public static short GetHiWord(this int n)
		{
			return (short)((n >> 16) & 0xffff);
		}

		public static long ToLong(uint highPart, uint lowPart)
		{
			return (((long)highPart) << 32) | (((long)lowPart) & 0xFFFFFFFF);
		}

		#endregion // Lo/Hi word

#pragma warning disable 1573
#pragma warning disable 1574

		/// <summary>Use this to translate error codes into HRESULTs like 0x80070006 for ERROR_INVALID_HANDLE.</summary>
		public static int GetHrFromWin32Error(uint errorCode)
		{
			const int FACILITY_WIN32 = 7;

			if (errorCode <= (int)NativeMethods.S_OK)
				return (int)errorCode;
			return (int)unchecked((errorCode & 0x0000FFFF) | (FACILITY_WIN32 << 16) | 0x80000000);
			//return (int) unchecked(((int) 0x80070000) | errorCode);
		}

		/// <summary>
		/// Returns <c>true</c> if the error code indicates success.
		/// </summary>
		/// <param name="hResult">The error code.</param>
		/// <returns>True if the error code indicates success.</returns>
		public static bool IsSucceed(int hResult)
		{
			return (hResult >= NativeMethods.S_OK);
		}

		/// <summary>
		/// Returns <c>true</c> if the error code indicates failure.
		/// </summary>
		/// <param name="hResult">The error code.</param>
		/// <returns>True if the error code indicates failure.</returns>
		public static bool IsFail(int hResult)
		{
			return (hResult < NativeMethods.S_OK);
		}

		/// <summary>
		/// Returns <c>true</c> if the Win32 error code corresponds to the COM error code.
		/// </summary>
		/// <param name="hResult">The COM error code.</param>
		/// <param name="win32ErrorCode">The Win32 error code.</param>
		/// <returns>Inticates that the Win32 error code corresponds to the COM error code.</returns>
		public static bool IsMatch(int hResult, int win32ErrorCode)
		{
			return (hResult == GetHrFromWin32Error((uint)win32ErrorCode));
		}

		/// <summary>
		/// If the method falls, throws an exception containing the error code.
		/// </summary>
		/// <param name="win32ErrorCode">The error code.</param>
		/// <exception cref="Win32Exception">Thrown if the error code indicates anything but success.</exception>
		[DebuggerStepThrough]
		public static void ThrowIfError(int win32ErrorCode)
		{
			if (win32ErrorCode != NativeMethods.S_OK)
				throw new Win32Exception(win32ErrorCode);
		}

		/// <summary>
		/// Returns the last error message (as string)
		/// </summary>
		[DebuggerStepThrough]
		[SecuritySafeCritical]
		public static string GetLastErrorMessage()
		{
			return GetWin32ErrorMessage(Marshal.GetLastWin32Error());
		}

		/// <summary>
		/// Returns an error error message (as string) based on the error code specified.
		/// </summary>
		/// <param name="win32ErrorCode">The error code.</param>
		[DebuggerStepThrough]
		public static string GetWin32ErrorMessage(int win32ErrorCode)
		{
			return new Win32Exception(win32ErrorCode).Message;
		}

#pragma warning restore 1573
#pragma warning restore 1574

		// Security -- LUID
		public static long LuidToLong(NativeMethods.Luid luid)
		{
			ulong high = (((ulong)luid.HighPart) << 32);
			ulong low = (((ulong)luid.LowPart) & 0x00000000FFFFFFFF);
			return unchecked((long)(high | low));
		}

		public static NativeMethods.Luid LongToLuid(long lluid)
		{
			return new NativeMethods.Luid { HighPart = (uint)(lluid >> 32), LowPart = (uint)(lluid & 0xFFFFFFFF) };
		}
	}
}
