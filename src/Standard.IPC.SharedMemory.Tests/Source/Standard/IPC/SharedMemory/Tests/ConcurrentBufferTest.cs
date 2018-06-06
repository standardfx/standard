using System;
using System.Linq;
using System.Collections;
using Xunit;
using Standard.IPC.SharedMemory;

namespace Standard.IPC.SharedMemory.Tests
{
    public class ConcurrentBufferTest
    {
		[Fact]
		public void ProducerCanWriteAndConsumerCanRead()
		{
			using (var producer = new ConcurrentBuffer(name: "MySharedBuffer", bufferSize: 1024))
			using (var consumer = new ConcurrentBuffer(name: "MySharedBuffer"))
			{
				int data = 123;
				producer.Write<int>(ref data);
				data = 456;
				producer.Write<int>(ref data, 1000);
				
				int readData;
				consumer.Read<int>(out readData);
				Assert.Equal(123, readData);
				consumer.Read<int>(out readData, 1000);
				Assert.Equal(456, readData);
			}
		}

        [Fact]
        public void ReadWriteBytesDataMatches()
        {
            var name = Guid.NewGuid().ToString();
            Random r = new Random();
            byte[] data = new byte[1024];
            byte[] readData = new byte[1024];
            r.NextBytes(data);

            using (var buf = new ConcurrentBuffer(name, 1024))
            using (var buf2 = new ConcurrentBuffer(name))
            {
                buf.Write(data);
                buf2.Read(readData);

                for (var i = 0; i < data.Length; i++)
                {
                    Assert.Equal(data[i], readData[i]);
                }
            }
        }

        [Fact]
        public void ReadCanThrowTimeoutException()
        {
            bool timedout = false;
            var name = Guid.NewGuid().ToString();
            byte[] data = new byte[1024];
            byte[] readData = new byte[1024];

            using (var buf = new ConcurrentBuffer(name, 1024))
            using (var buf2 = new ConcurrentBuffer(name))
            {
                // Set a small timeout to speed up the test
                buf2.ReadWriteTimeout = 0;

                // Introduce possible deadlock by acquiring without releasing the write lock.
                buf.AcquireWriteLock();

                // We want the AcquireReadLock to fail
                if (!buf2.AcquireReadLock(1))
                {
                    try
                    {
                        // Read should timeout with TimeoutException because buf.ReleaseWriteLock has not been called
                        buf2.Read(readData);
                    }
                    catch (TimeoutException e)
                    {
                        timedout = true;
                    }
                }

                Assert.True(timedout);

                // Remove the deadlock situation, by releasing the write lock...
                buf.ReleaseWriteLock();

                // ...and ensure that we can now read the data
                if (buf.AcquireReadLock(1))
                    buf2.Read(readData);
                else
                    Assert.True(false); // test has failed
            }
        }
	}
}
