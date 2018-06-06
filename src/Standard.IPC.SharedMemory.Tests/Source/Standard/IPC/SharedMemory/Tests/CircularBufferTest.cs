using System;
using System.Linq;
using System.Collections;
using Xunit;
using Standard.IPC.SharedMemory;

namespace Standard.IPC.SharedMemory.Tests
{
    public class CircularBufferTest
    {
		[Fact]
		public void ProducerCanWriteAndConsumerCanRead()
		{
			using (var producer = new CircularBuffer(name: "MySharedCircularMemory", nodeCount: 3, nodeBufferSize: 4))
			using (var consumer = new CircularBuffer(name: "MySharedCircularMemory"))
			{
				// nodeCount must be one larger than the number
				// of writes that must fit in the buffer at any one time
				producer.Write<int>(new int[] { 123 });
				producer.Write<int>(new int[] { 456 });
			
				int[] data = new int[1];
				consumer.Read<int>(data);
				Assert.Equal(123, data[0]);

				consumer.Read<int>(data);
				Assert.Equal(456, data[0]);
			}
		}
	}
}
