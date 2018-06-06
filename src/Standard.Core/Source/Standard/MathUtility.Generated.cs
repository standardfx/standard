﻿using System;
using System.Linq;

namespace Standard
{
	public static partial class MathUtility
	{

		/// <summary>
		/// Returns the smallest of three double-precision floating-point numbers.
		/// </summary>
		[CLSCompliant(true)]
		public static double Min(double val1, double val2, double val3)
		{
			return Math.Min(val1, Math.Min(val2, val3));
		}

		/// <summary>
		/// Returns the smallest of a list of double-precision floating-point numbers.
		/// </summary>
		[CLSCompliant(true)]
		public static double Min(double val1, double val2, params double[] vals)
		{
			return Math.Min(val1, Math.Min(val2, Enumerable.Min(vals)));
		}

		/// <summary>
		/// Returns the largest of three double-precision floating-point numbers.
		/// </summary>
		[CLSCompliant(true)]
		public static double Max(double val1, double val2, double val3)
		{
			return Math.Max(val1, Math.Max(val2, val3));
		}

		/// <summary>
		/// Returns the largest of a list of double-precision floating-point numbers.
		/// </summary>
		[CLSCompliant(true)]
		public static double Max(double val1, double val2, params double[] vals)
		{
			return Math.Max(val1, Math.Max(val2, Enumerable.Max(vals)));
		}

		/// <summary>
		/// Returns the smallest of three 8-bit unsigned integers.
		/// </summary>
		[CLSCompliant(true)]
		public static byte Min(byte val1, byte val2, byte val3)
		{
			return Math.Min(val1, Math.Min(val2, val3));
		}

		/// <summary>
		/// Returns the smallest of a list of 8-bit unsigned integers.
		/// </summary>
		[CLSCompliant(true)]
		public static byte Min(byte val1, byte val2, params byte[] vals)
		{
			return Math.Min(val1, Math.Min(val2, Enumerable.Min(vals)));
		}

		/// <summary>
		/// Returns the largest of three 8-bit unsigned integers.
		/// </summary>
		[CLSCompliant(true)]
		public static byte Max(byte val1, byte val2, byte val3)
		{
			return Math.Max(val1, Math.Max(val2, val3));
		}

		/// <summary>
		/// Returns the largest of a list of 8-bit unsigned integers.
		/// </summary>
		[CLSCompliant(true)]
		public static byte Max(byte val1, byte val2, params byte[] vals)
		{
			return Math.Max(val1, Math.Max(val2, Enumerable.Max(vals)));
		}

		/// <summary>
		/// Returns the smallest of three 16-bit signed integers.
		/// </summary>
		[CLSCompliant(true)]
		public static short Min(short val1, short val2, short val3)
		{
			return Math.Min(val1, Math.Min(val2, val3));
		}

		/// <summary>
		/// Returns the smallest of a list of 16-bit signed integers.
		/// </summary>
		[CLSCompliant(true)]
		public static short Min(short val1, short val2, params short[] vals)
		{
			return Math.Min(val1, Math.Min(val2, Enumerable.Min(vals)));
		}

		/// <summary>
		/// Returns the largest of three 16-bit signed integers.
		/// </summary>
		[CLSCompliant(true)]
		public static short Max(short val1, short val2, short val3)
		{
			return Math.Max(val1, Math.Max(val2, val3));
		}

		/// <summary>
		/// Returns the largest of a list of 16-bit signed integers.
		/// </summary>
		[CLSCompliant(true)]
		public static short Max(short val1, short val2, params short[] vals)
		{
			return Math.Max(val1, Math.Max(val2, Enumerable.Max(vals)));
		}

		/// <summary>
		/// Returns the smallest of three 32-bit unsigned integers.
		/// </summary>
		[CLSCompliant(false)]
		public static uint Min(uint val1, uint val2, uint val3)
		{
			return Math.Min(val1, Math.Min(val2, val3));
		}

		/// <summary>
		/// Returns the smallest of a list of 32-bit unsigned integers.
		/// </summary>
		[CLSCompliant(false)]
		public static uint Min(uint val1, uint val2, params uint[] vals)
		{
			return Math.Min(val1, Math.Min(val2, Enumerable.Min(vals)));
		}

		/// <summary>
		/// Returns the largest of three 32-bit unsigned integers.
		/// </summary>
		[CLSCompliant(false)]
		public static uint Max(uint val1, uint val2, uint val3)
		{
			return Math.Max(val1, Math.Max(val2, val3));
		}

		/// <summary>
		/// Returns the largest of a list of 32-bit unsigned integers.
		/// </summary>
		[CLSCompliant(false)]
		public static uint Max(uint val1, uint val2, params uint[] vals)
		{
			return Math.Max(val1, Math.Max(val2, Enumerable.Max(vals)));
		}

		/// <summary>
		/// Returns the smallest of three 8-bit signed integers.
		/// </summary>
		[CLSCompliant(false)]
		public static sbyte Min(sbyte val1, sbyte val2, sbyte val3)
		{
			return Math.Min(val1, Math.Min(val2, val3));
		}

		/// <summary>
		/// Returns the smallest of a list of 8-bit signed integers.
		/// </summary>
		[CLSCompliant(false)]
		public static sbyte Min(sbyte val1, sbyte val2, params sbyte[] vals)
		{
			return Math.Min(val1, Math.Min(val2, Enumerable.Min(vals)));
		}

		/// <summary>
		/// Returns the largest of three 8-bit signed integers.
		/// </summary>
		[CLSCompliant(false)]
		public static sbyte Max(sbyte val1, sbyte val2, sbyte val3)
		{
			return Math.Max(val1, Math.Max(val2, val3));
		}

		/// <summary>
		/// Returns the largest of a list of 8-bit signed integers.
		/// </summary>
		[CLSCompliant(false)]
		public static sbyte Max(sbyte val1, sbyte val2, params sbyte[] vals)
		{
			return Math.Max(val1, Math.Max(val2, Enumerable.Max(vals)));
		}

		/// <summary>
		/// Returns the smallest of three single-precision floating-point numbers.
		/// </summary>
		[CLSCompliant(true)]
		public static float Min(float val1, float val2, float val3)
		{
			return Math.Min(val1, Math.Min(val2, val3));
		}

		/// <summary>
		/// Returns the smallest of a list of single-precision floating-point numbers.
		/// </summary>
		[CLSCompliant(true)]
		public static float Min(float val1, float val2, params float[] vals)
		{
			return Math.Min(val1, Math.Min(val2, Enumerable.Min(vals)));
		}

		/// <summary>
		/// Returns the largest of three single-precision floating-point numbers.
		/// </summary>
		[CLSCompliant(true)]
		public static float Max(float val1, float val2, float val3)
		{
			return Math.Max(val1, Math.Max(val2, val3));
		}

		/// <summary>
		/// Returns the largest of a list of single-precision floating-point numbers.
		/// </summary>
		[CLSCompliant(true)]
		public static float Max(float val1, float val2, params float[] vals)
		{
			return Math.Max(val1, Math.Max(val2, Enumerable.Max(vals)));
		}

		/// <summary>
		/// Returns the smallest of three 16-bit unsigned integers.
		/// </summary>
		[CLSCompliant(false)]
		public static ushort Min(ushort val1, ushort val2, ushort val3)
		{
			return Math.Min(val1, Math.Min(val2, val3));
		}

		/// <summary>
		/// Returns the smallest of a list of 16-bit unsigned integers.
		/// </summary>
		[CLSCompliant(false)]
		public static ushort Min(ushort val1, ushort val2, params ushort[] vals)
		{
			return Math.Min(val1, Math.Min(val2, Enumerable.Min(vals)));
		}

		/// <summary>
		/// Returns the largest of three 16-bit unsigned integers.
		/// </summary>
		[CLSCompliant(false)]
		public static ushort Max(ushort val1, ushort val2, ushort val3)
		{
			return Math.Max(val1, Math.Max(val2, val3));
		}

		/// <summary>
		/// Returns the largest of a list of 16-bit unsigned integers.
		/// </summary>
		[CLSCompliant(false)]
		public static ushort Max(ushort val1, ushort val2, params ushort[] vals)
		{
			return Math.Max(val1, Math.Max(val2, Enumerable.Max(vals)));
		}

		/// <summary>
		/// Returns the smallest of three 64-bit unsigned integers.
		/// </summary>
		[CLSCompliant(false)]
		public static ulong Min(ulong val1, ulong val2, ulong val3)
		{
			return Math.Min(val1, Math.Min(val2, val3));
		}

		/// <summary>
		/// Returns the smallest of a list of 64-bit unsigned integers.
		/// </summary>
		[CLSCompliant(false)]
		public static ulong Min(ulong val1, ulong val2, params ulong[] vals)
		{
			return Math.Min(val1, Math.Min(val2, Enumerable.Min(vals)));
		}

		/// <summary>
		/// Returns the largest of three 64-bit unsigned integers.
		/// </summary>
		[CLSCompliant(false)]
		public static ulong Max(ulong val1, ulong val2, ulong val3)
		{
			return Math.Max(val1, Math.Max(val2, val3));
		}

		/// <summary>
		/// Returns the largest of a list of 64-bit unsigned integers.
		/// </summary>
		[CLSCompliant(false)]
		public static ulong Max(ulong val1, ulong val2, params ulong[] vals)
		{
			return Math.Max(val1, Math.Max(val2, Enumerable.Max(vals)));
		}

		/// <summary>
		/// Returns the smallest of three 32-bit signed integers.
		/// </summary>
		[CLSCompliant(true)]
		public static int Min(int val1, int val2, int val3)
		{
			return Math.Min(val1, Math.Min(val2, val3));
		}

		/// <summary>
		/// Returns the smallest of a list of 32-bit signed integers.
		/// </summary>
		[CLSCompliant(true)]
		public static int Min(int val1, int val2, params int[] vals)
		{
			return Math.Min(val1, Math.Min(val2, Enumerable.Min(vals)));
		}

		/// <summary>
		/// Returns the largest of three 32-bit signed integers.
		/// </summary>
		[CLSCompliant(true)]
		public static int Max(int val1, int val2, int val3)
		{
			return Math.Max(val1, Math.Max(val2, val3));
		}

		/// <summary>
		/// Returns the largest of a list of 32-bit signed integers.
		/// </summary>
		[CLSCompliant(true)]
		public static int Max(int val1, int val2, params int[] vals)
		{
			return Math.Max(val1, Math.Max(val2, Enumerable.Max(vals)));
		}

		/// <summary>
		/// Returns the smallest of three 64-bit signed integers.
		/// </summary>
		[CLSCompliant(true)]
		public static long Min(long val1, long val2, long val3)
		{
			return Math.Min(val1, Math.Min(val2, val3));
		}

		/// <summary>
		/// Returns the smallest of a list of 64-bit signed integers.
		/// </summary>
		[CLSCompliant(true)]
		public static long Min(long val1, long val2, params long[] vals)
		{
			return Math.Min(val1, Math.Min(val2, Enumerable.Min(vals)));
		}

		/// <summary>
		/// Returns the largest of three 64-bit signed integers.
		/// </summary>
		[CLSCompliant(true)]
		public static long Max(long val1, long val2, long val3)
		{
			return Math.Max(val1, Math.Max(val2, val3));
		}

		/// <summary>
		/// Returns the largest of a list of 64-bit signed integers.
		/// </summary>
		[CLSCompliant(true)]
		public static long Max(long val1, long val2, params long[] vals)
		{
			return Math.Max(val1, Math.Max(val2, Enumerable.Max(vals)));
		}

		/// <summary>
		/// Returns the smallest of three decimal numbers.
		/// </summary>
		[CLSCompliant(true)]
		public static decimal Min(decimal val1, decimal val2, decimal val3)
		{
			return Math.Min(val1, Math.Min(val2, val3));
		}

		/// <summary>
		/// Returns the smallest of a list of decimal numbers.
		/// </summary>
		[CLSCompliant(true)]
		public static decimal Min(decimal val1, decimal val2, params decimal[] vals)
		{
			return Math.Min(val1, Math.Min(val2, Enumerable.Min(vals)));
		}

		/// <summary>
		/// Returns the largest of three decimal numbers.
		/// </summary>
		[CLSCompliant(true)]
		public static decimal Max(decimal val1, decimal val2, decimal val3)
		{
			return Math.Max(val1, Math.Max(val2, val3));
		}

		/// <summary>
		/// Returns the largest of a list of decimal numbers.
		/// </summary>
		[CLSCompliant(true)]
		public static decimal Max(decimal val1, decimal val2, params decimal[] vals)
		{
			return Math.Max(val1, Math.Max(val2, Enumerable.Max(vals)));
		}

	}
}