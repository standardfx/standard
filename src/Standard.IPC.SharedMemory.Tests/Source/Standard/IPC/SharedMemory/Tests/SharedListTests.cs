using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;
using Standard;
using Standard.IPC.SharedMemory;

namespace Standard.IPC.SharedMemory.Tests
{
    public class SharedListTests
    {
		[Fact]
        public void IndexerReadWriteIntegerDataMatches()
        {
            var name = Guid.NewGuid().ToString();
            using (var sma = new SharedList<int>(name, 10))
            {
                sma[0] = 3;
                sma[4] = 10;

                using (var smr = new SharedList<int>(name))
                {
                    Assert.Equal(0, smr[1]);
                    Assert.Equal(3, smr[0]);
                    Assert.Equal(10, smr[4]);
                }

                IList<int> list = sma;
                list[0] = 5;
                list[4] = 55;

                using (var smr = new SharedList<int>(name))
                {
                    IList<int> r = smr;

                    Assert.Equal(0, r[1]);
                    Assert.Equal(5, r[0]);
                    Assert.Equal(55, r[4]);
                }

                list[3] = 68;
                IList<int> arraySlice = new ListSegment<int>(list, 1, 8);
                arraySlice[0] = 67;

                using (var smr = new SharedList<int>(name))
                {
                    IList<int> r = smr;
                    IList<int> rarraySlice = new ListSegment<int>(r, 1, 8);

                    Assert.Equal(67, rarraySlice[0]);
                    Assert.Equal(68, rarraySlice[2]);
                    Assert.Equal(55, rarraySlice[3]);
                }

            }
        }

        [Fact]
        public void IndexerOutOfRangeThrowsException()
        {
            var name = Guid.NewGuid().ToString();
            using (var sma = new SharedList<int>(name, 10))
            {
                bool exceptionThrown = false;
                try
                {
                    sma[-1] = 0;
                }
                catch (ArgumentOutOfRangeException)
                {
                    exceptionThrown = true;
                }

                Assert.True(exceptionThrown);

                exceptionThrown = false;
                IList<int> a = sma;
                try
                {
                    a[-1] = 0;
                }
                catch (ArgumentOutOfRangeException)
                {
                    exceptionThrown = true;
                }

                Assert.True(exceptionThrown);

                try
                {
                    exceptionThrown = false;
                    sma[sma.Length] = 0;
                }
                catch (ArgumentOutOfRangeException)
                {
                    exceptionThrown = true;
                }

                Assert.True(exceptionThrown);


                try
                {
                    exceptionThrown = false;
                    a[a.Count] = 0;
                }
                catch (ArgumentOutOfRangeException)
                {
                    exceptionThrown = true;
                }

                Assert.True(exceptionThrown);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        unsafe struct MyTestStruct
        {
            const int MAXLENGTH = 100;

            fixed char name[MAXLENGTH];

            public int ValueA;

            public string Name
            {
                get
                {
                    fixed (char* n = name)
                    {
                        return new String(n);
                    }
                }
                set
                {
                    fixed (char* n = name)
                    {
                        int indx = 0;
                        foreach (char c in value)
                        {
                            *(n + indx) = c;
                            indx++;
                            if (indx >= MAXLENGTH - 1)
                                break;
                        }
                        *(n + indx) = '\0';
                    }
                }
            }
        }

        [Fact]
        public void MyTestStructWorks()
        {
            var my = new MyTestStruct();
            my.Name = "long string long string";
            my.Name = "short string";
            Assert.Equal("short string", my.Name);
        }

        [Fact]
        public void IndexerReadWriteComplexStructDataMatches()
        {
            var name = Guid.NewGuid().ToString();
            using (var sma = new SharedList<MyTestStruct>(name, 10))
            {
                sma[0] = new MyTestStruct { ValueA = 3, Name = "My Test Name" };
                sma[4] = new MyTestStruct { ValueA = 10, Name = "My Test Name2" };

                using (var smr = new SharedList<MyTestStruct>(name))
                {
                    Assert.Equal(0, smr[1].ValueA);
                    Assert.Equal(3, smr[0].ValueA);
                    Assert.Equal("My Test Name", smr[0].Name);
                    Assert.Equal(10, smr[4].ValueA);
                    Assert.Equal("My Test Name2", smr[4].Name);
                }
            }
        }

        [Fact]
        public void CopyToNullArrayThrowsException()
        {
            var name = Guid.NewGuid().ToString();
            using (var sma = new SharedList<int>(name, 10))
            {
                bool exceptionThrown = false;
                try
                {
                    sma.CopyTo(null);
                }
                catch (ArgumentNullException)
                {
                    exceptionThrown = true;
                }
                Assert.True(exceptionThrown);
            }
        }

        [Fact]
        public void WriteNullArrayThrowsException()
        {
            var name = Guid.NewGuid().ToString();
            using (var sma = new SharedList<int>(name, 10))
            {
                bool exceptionThrown = false;
                try
                {
                    sma.Write(null);
                }
                catch (ArgumentNullException)
                {
                    exceptionThrown = true;
                }
                Assert.True(exceptionThrown);
            }
        }

        [Fact]
        public void GetEnumeratorIterateItemsDataMatches()
        {
            var name = Guid.NewGuid().ToString();
            Random r = new Random();
            int bufSize = 1024;
            byte[] data = new byte[bufSize];
            byte[] readBuf = new byte[bufSize];
            using (var sma = new SharedList<byte>(name, bufSize))
            {
                sma.Write(data);

                int value = 0;
                foreach (var item in sma)
                {
                    Assert.Equal(data[value], item);
                    value++;
                }
            }
        }

        [Fact]
        public void AcquireWriteLockReadWriteLocksCorrectly()
        {
            var name = Guid.NewGuid().ToString();
            Random r = new Random();
            int bufSize = 1024;
            byte[] data = new byte[bufSize];
            byte[] readBuf = new byte[bufSize];

            bool readIsFirst = false;
            bool readBlocked = false;
            int syncValue = 0;

            // Fill with random data
            r.NextBytes(data);

            using (var sma = new SharedList<byte>(name, bufSize))
            {
                // Acquire write lock early
                sma.AcquireWriteLock();
                using (var smr = new SharedList<byte>(name))
                {
                    var t1 = Task.Factory.StartNew(() =>
                        {
                            if (System.Threading.Interlocked.Exchange(ref syncValue, 1) == 0)
                                readIsFirst = true;
                            // Should block until write lock is released
                            smr.AcquireReadLock();
                            if (System.Threading.Interlocked.Exchange(ref syncValue, 3) == 4)
                                readBlocked = true;
                            smr.CopyTo(readBuf);
                            smr.ReleaseReadLock();
                        });

                    System.Threading.Thread.Sleep(10);

                    var t2 = Task.Factory.StartNew(() =>
                        {
                            var val = System.Threading.Interlocked.Exchange(ref syncValue, 2);
                            if (val == 0)
                                readIsFirst = false;
                            else if (val == 3)
                                readBlocked = false;
                            System.Threading.Thread.Sleep(10);
                            sma.Write(data);
                            System.Threading.Interlocked.Exchange(ref syncValue, 4);
                            sma.ReleaseWriteLock();
                        });

                    Task.WaitAll(t1, t2);

                    Assert.True(readIsFirst);
                    Assert.True(readBlocked);

                    // Check data was written before read
                    for (var i = 0; i < readBuf.Length; i++)
                    {
                        Assert.Equal(data[i], readBuf[i]);
                    }
                }
            }
        }

        [Fact]
        public void AcquireReadWriteLocksReadWriteBlocks()
        {
            var name = Guid.NewGuid().ToString();
            using (var sma = new SharedList<byte>(name, 10))
            {
                using (var smr = new SharedList<byte>(name))
                {
                    // Acquire write lock
                    sma.AcquireWriteLock();

                    // Should block (and fail to reset write signal)
                    Assert.False(smr.AcquireReadLock(10));

                    sma.ReleaseWriteLock();

                    smr.AcquireReadLock();

                    // Should block (and fail to reset read signal)
                    Assert.False(sma.AcquireWriteLock(10));

                    smr.ReleaseReadLock();
                }
            }
        }

        [Fact]
        public void IListContainsWorks()
        {
            var name = Guid.NewGuid().ToString();
            using (var sma = new SharedList<int>(name, 10))
            {
                sma[0] = 3;
                sma[4] = 10;

                IList<int> a = sma;

                Assert.Contains(10, a);
                Assert.DoesNotContain(11, a);
            }
        }

        [Fact]
        public void GetIListByIndexOf()
        {
            var name = Guid.NewGuid().ToString();
            using (var sma = new SharedList<int>(name, 10))
            {
                sma[0] = 3;
                sma[4] = 10;

                IList<int> a = sma;

                Assert.Equal(4, a.IndexOf(10));
                Assert.Equal(-1, a.IndexOf(11));
            }
        }

        [Fact]
        public void IListIsReadOnly()
        {
            var name = Guid.NewGuid().ToString();
            using (var sma = new SharedList<int>(name, 10))
            {
                sma[0] = 3;
                sma[4] = 10;

                IList<int> a = sma;
                Assert.True(a.IsReadOnly);
            }
        }
	}
}
