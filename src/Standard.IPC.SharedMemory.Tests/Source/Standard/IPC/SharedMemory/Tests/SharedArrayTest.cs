using System;
using System.Linq;
using System.Collections;
using Xunit;
using Standard.IPC.SharedMemory;

namespace Standard.IPC.SharedMemory.Tests
{
    public class SharedArrayTest
    {
		[Fact]
		public void ConsumerSyncsWithProducer()
		{
			using (var producer = new SharedArray<int>("MySharedArray", 10))
			using (var consumer = new SharedArray<int>("MySharedArray"))
			{
			    producer[0] = 123;
			    producer[producer.Length - 1] = 456;
			    
			    Assert.Equal(123, consumer[0]);
			    Assert.Equal(456, consumer[consumer.Length - 1]);
			}
		}
	}
}
