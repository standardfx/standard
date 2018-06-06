using System;
using System.Linq;
using System.Collections;
using Xunit;
using Standard;

namespace Standard.Extensions.Tests
{
    public class NonGenericCollectionsTest
    {
		[Fact]
		public void GetFirstMember()
		{
			Queue q = new Queue();
			q.Enqueue(1);
	    	q.Enqueue(2);
	    	q.Enqueue(3);
	    	Assert.Throws<InvalidOperationException>(() => q.First(x => (int)x > 5));
	    	Assert.Equal(1, q.First(x => (int)x < 3));

		}

		[Fact]
        public void GetFirstOrDefault() 
        {
	    	Queue q = new Queue();
	    	q.Enqueue(1);
	    	q.Enqueue(2);
	    	q.Enqueue(3);
	    	Assert.Null(q.FirstOrDefault(x => (int)x > 5));
	    	Assert.Equal(1, q.FirstOrDefault(x => (int)x < 3));
        }
	}
}

