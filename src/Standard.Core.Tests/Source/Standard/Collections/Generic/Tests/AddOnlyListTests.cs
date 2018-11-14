using System;
using System.Linq;
using System.Text;
using Xunit;
using Standard.Collections.Generic;

namespace Standard.Collections.Generic.Tests
{
    public class AddOnlyListTests
    {
        private int[] addNumbers = new int[]
        {
            11, 22, 33, 44, 55, 66, 77, 88, 99, 
            1010, 111, 1212, 1313, 1414, 1515,
            1616, 1717, 1818
        };

        [Fact]
        public void CanGrow()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < 32; i++)
            {
                sb.AppendFormat("{0}/{1}\r\n", i, AddOnlyList<int>.GetBucketIndex(i));
            }

            var expected = "0/0,1/0,2/0,3/1,4/1,5/1,6/1," +
                "7/2,8/2,9/2,10/2,11/2,12/2,13/2,14/2," +
                "15/3,16/3,17/3,18/3,19/3,20/3,21/3,22/3,23/3,24/3,25/3,26/3,27/3,28/3,29/3,30/3," +
                "31/4,";

            expected = expected.Replace(",", "\r\n");

            Assert.Equal(expected, sb.ToString());
        }

        [Fact]
        public void BehaveLikeIList()
        {
            var ea = new AddOnlyList<int>(size => new int[size]);

            for (int iter = 0; iter < 2; iter++)
            {
                Assert.Empty(ea);

                ea.Add(addNumbers[0]);
                Assert.Single(ea);
                ea.Add(addNumbers[1]);
                Assert.Equal(2, ea.Count);

                Assert.Contains(addNumbers[0], ea);
                Assert.DoesNotContain(222, ea);

                for (int i = 2; i < addNumbers.Length; i++)
                {
                    ea.Add(addNumbers[i]);
                }

                Assert.Equal(12221, ea.Sum());

                Assert.Equal(6, ea.IndexOf(77));

                Assert.Equal(77, ea[6]);

                ea[6] = 777;
                Assert.Equal(777, ea[6]);

                // clear and try again
                ea.Clear();
            }
        }
    }
}