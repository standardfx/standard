using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;
using HANDLE = System.IntPtr;
using Standard.Runtime.Win32API;

namespace Standard.Win32.SafeHandles
{
    public sealed class SafeHGlobalHandle : IDisposable
    {
		/// <summary>
		/// Maintainsreference to other SafeHGlobalHandle objects, the pointer
		/// to which are refered to by this object. This is to ensure that such
		/// objects being referred to wouldn't be unreferenced until this object
		/// is active.
		/// </summary>
		private List<SafeHGlobalHandle> references;

		// Using SafeHandle here doesn't buy much since the pointer is
		// eventually stashed into a native structure. Using a SafeHandle would
		// involve calling DangerousGetHandle in place of ToIntPtr which makes
		// code analysis report CA2001: Avoid calling problematic methods.

		/// <summary>
		/// Unmanaged pointer wrapped by this object
		/// </summary>
		private IntPtr pointer;

		SafeHGlobalHandle()
        {
            pointer = IntPtr.Zero;
        }

        SafeHGlobalHandle(IntPtr handle)
        {
            pointer = handle;
        }

        ~SafeHGlobalHandle()
        {
            Dispose();
        }

        public static SafeHGlobalHandle InvalidHandle
        {
            get { return new SafeHGlobalHandle(IntPtr.Zero); }
        }

        /// <summary>
        /// <para>
        /// Adds reference to other SafeHGlobalHandle objects, the pointer to
        /// which are refered to by this object. This is to ensure that such
        /// objects being referred to wouldn't be unreferenced until this object
        /// is active.
        /// </para>
        /// <para>For e.g. when this object is an array of pointers to other objects</para>
        /// </summary>
        /// <param name="children">Collection of SafeHGlobalHandle objects referred to by this object.</param>
        public void AddSubReference(IEnumerable<SafeHGlobalHandle> children)
        {
            if (references == null)
                references = new List<SafeHGlobalHandle>();

            references.AddRange(children);
        }

        /// <summary>
        /// Allocates from unmanaged memory to represent an array of pointers
        /// and marshals the unmanaged pointers (IntPtr) to the native array
        /// equivalent.
        /// </summary>
        /// <param name="values">Array of unmanaged pointers</param>
        /// <returns>SafeHGlobalHandle object to an native (unmanaged) array of pointers</returns>
        public static SafeHGlobalHandle AllocHGlobal(IntPtr[] values)
        {
            SafeHGlobalHandle result = AllocHGlobal(IntPtr.Size * values.Length);

            Marshal.Copy(values, 0, result.pointer, values.Length);

            return result;
        }

        public static SafeHGlobalHandle AllocHGlobalStruct<T>(T obj) where T : struct
        {
			//Debug.Assert(typeof(T).StructLayoutAttribute.Value == LayoutKind.Sequential);
			SafeHGlobalHandle result = AllocHGlobal(Marshal.SizeOf(typeof(T)));
            Marshal.StructureToPtr(obj, result.pointer, false);

			return result;
        }

        /// <summary>
        /// Allocates from unmanaged memory to represent an array of structures
        /// and marshals the structure elements to the native array of
        /// structures. ONLY structures with attribute StructLayout of
        /// LayoutKind.Sequential are supported.
        /// </summary>
        /// <typeparam name="T">Native structure type</typeparam>
        /// <param name="values">Collection of structure objects</param>
        /// <returns>SafeHGlobalHandle object to an native (unmanaged) array of structures</returns>
        public static SafeHGlobalHandle AllocHGlobal<T>(ICollection<T> values) where T : struct
        {
            //Debug.Assert(typeof(T).StructLayoutAttribute.Value == LayoutKind.Sequential);
            return AllocHGlobal(0, values, values.Count);
        }

        /// <summary>
        /// Allocates from unmanaged memory to represent a structure with a
        /// variable length array at the end and marshal these structure
        /// elements. It is the callers responsibility to marshal what preceeds
        /// the trailing array into the unmanaged memory. ONLY structures with
        /// attribute StructLayout of LayoutKind.Sequential are supported.
        /// </summary>
        /// <typeparam name="T">Type of the trailing array of structures</typeparam>
        /// <param name="prefixBytes">Number of bytes preceeding the trailing array of structures</param>
        /// <param name="values">Collection of structure objects</param>
        /// <param name="count"></param>
        /// <returns>SafeHGlobalHandle object to an native (unmanaged) structure
        /// with a trail array of structures</returns>
        public static SafeHGlobalHandle AllocHGlobal<T>(int prefixBytes, IEnumerable<T> values, int count) where T : struct
        {
            //Debug.Assert(typeof(T).StructLayoutAttribute.Value == LayoutKind.Sequential);
            SafeHGlobalHandle result = AllocHGlobal(prefixBytes + Marshal.SizeOf(typeof(T)) * count);

            IntPtr ptr = new IntPtr(result.pointer.ToInt32() + prefixBytes);
            foreach (var value in values)
            {
                Marshal.StructureToPtr(value, ptr, false);
                ptr.Increment<T>();
            }

            return result;
        }

        /// <summary>
        /// Allocates from unmanaged memory to represent a unicode string (WSTR)
        /// and marshal this to a native PWSTR.
        /// </summary>
        /// <param name="s">String</param>
        /// <returns>SafeHGlobalHandle object to an native (unmanaged) unicode string</returns>
        public static SafeHGlobalHandle AllocHGlobal(string s)
        {
            return new SafeHGlobalHandle(Marshal.StringToHGlobalUni(s));
        }

        /// <summary>
        /// Operator to obtain the unmanaged pointer wrapped by the object. Note
        /// that the returned pointer is only valid for the lifetime of this
        /// object.
        /// </summary>
        /// <returns>Unmanaged pointer wrapped by the object</returns>
        public IntPtr ToIntPtr()
        {
            return pointer;
        }

        #region IDisposable implmentation

        public void Dispose()
        {
            if (pointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(pointer);
                pointer = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        private static SafeHGlobalHandle AllocHGlobal(int cb)
        {
            if (cb < 0)
                throw new ArgumentOutOfRangeException("cb", RS.NegativeLength);

            SafeHGlobalHandle result = new SafeHGlobalHandle();

            // CER
            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally
            {
                result.pointer = Marshal.AllocHGlobal(cb);
            }

            return result;
        }
    }
}
