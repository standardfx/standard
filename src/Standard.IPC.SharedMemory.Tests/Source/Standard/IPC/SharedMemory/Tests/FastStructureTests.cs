using System;
using System.Linq;
using System.Collections;
using System.Runtime.InteropServices;
using Xunit;
using Standard.IPC.SharedMemory;

namespace Standard.IPC.SharedMemory.Tests
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CompatibleStructure
    {
        public int Integer1;
        public IntPtr Pointer1;
        public IntPtr Pointer2;
        public IntPtr Pointer3;
        public IntPtr Pointer4;
        public fixed byte Contents[8];
        public int Bookend;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IncompatibleNestedStructure
    {
        public int IncompatibleNestedStructure_One;
        public object IncompatibleNestedStructure_Two;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IncompatibleNestedStructure2
    {
        public int IncompatibleNestedStructure_One;
        public object IncompatibleNestedStructure_Two;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512, ArraySubType = UnmanagedType.I2)]
        public char[] Filename;
    }

    public struct HasIncompatibleStructure
    {
        public int HasIncompatibleStructure_One;
        public IncompatibleNestedStructure HasIncompatibleStructure_Two;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ComplexStructure
    {
        public int FirstElement;
        public CompatibleStructure Compatible;
        public int FinalElement;
    }

    public class FastStructureTests
    {
		[Fact]
        public void ThrowOnIncompabitibleNestedType()
        {
        	Assert.Throws<TypeInitializationException>(() => FastStructure<HasIncompatibleStructure>.Size);
        }

        [Fact]
        public void ThrowOnIncompatibleStructure()
        {
        	Assert.Throws<TypeInitializationException>(() => FastStructure<IncompatibleNestedStructure2>.Size);
        }

        [Fact]
        public void GetCompatibleStructureSize()
        {
            Assert.Equal(IntPtr.Size * 4 + 8 + (sizeof(int) * 2), FastStructure<CompatibleStructure>.Size);
        }

        [Fact]
        public void GetComplexStructureSize()
        {
            var sizeOfCompatibleStructure = IntPtr.Size * 4 + 8 + (sizeof(int) * 2);
            var sizeOfComplexStructure = (sizeof(int) * 2) + sizeOfCompatibleStructure;
            Assert.Equal(sizeOfComplexStructure, FastStructure<ComplexStructure>.Size);
        }

		[Fact]
        public void CanAllocHGlobalReadWrite()
        {
            IntPtr mem = Marshal.AllocHGlobal(FastStructure.SizeOf<ComplexStructure>());

            ComplexStructure n = new ComplexStructure();

            n.Compatible.Integer1 = 1;
            n.Compatible.Bookend = 2;

            n.FirstElement = 3;
            n.FinalElement = 9;
            unsafe
            {
                n.Compatible.Contents[0] = 4;
                n.Compatible.Contents[7] = 5;
            }

            FastStructure.StructureToPtr(ref n, mem);

            // Assert that the reading and writing result in same structure
            ComplexStructure m = FastStructure.PtrToStructure<ComplexStructure>(mem);
            Assert.Equal(n, m);
            Assert.Equal(n.Compatible.Integer1, m.Compatible.Integer1);
            Assert.Equal(n.Compatible.Bookend, m.Compatible.Bookend);
            unsafe
            {
                Assert.Equal(n.Compatible.Contents[0], m.Compatible.Contents[0]);
                Assert.Equal(n.Compatible.Contents[7], m.Compatible.Contents[7]);
            }

            // Assert that Marshal.PtrToStructure is compatible
            m = (ComplexStructure)Marshal.PtrToStructure(mem, typeof(ComplexStructure));
            Assert.Equal(n, m);
            Assert.Equal(n.Compatible.Integer1, m.Compatible.Integer1);
            Assert.Equal(n.Compatible.Bookend, m.Compatible.Bookend);
            unsafe
            {
                Assert.Equal(n.Compatible.Contents[0], m.Compatible.Contents[0]);
                Assert.Equal(n.Compatible.Contents[7], m.Compatible.Contents[7]);
            }

            Marshal.FreeHGlobal(mem);
        }
	}
}
