using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Standard;

namespace Standard.Extensions.Tests
{
    public class StringTruncateTest
    {
        [Fact]
        public void LengthBasedTruncation() 
        {
            string actual = "this is a long string";
            string actual1 = actual.Truncate(actual.Length);
            string actual2 = actual.Truncate(actual.Length + 6);
            string actual3 = actual.Truncate(4);
            string actual4 = actual.Truncate(4, true);
            string actual5 = actual.Truncate(4, false);
            string actual6 = actual.Truncate(4, "~~~");
            string actual7 = actual.Truncate(4, "~~~", true);
            string actual8 = actual.Truncate(4, "~~~", false);

            string expected1 = "this is a long string";
            string expected2 = "this is a long string";
            string expected3 = "this";
            string expected4 = "ring";
            string expected5 = "this";
            string expected6 = "t~~~";
            string expected7 = "~~~g";
            string expected8 = "t~~~";

            Assert.Equal(expected1, actual1);
            Assert.Equal(expected2, actual2);
            Assert.Equal(expected3, actual3);
            Assert.Equal(expected4, actual4);
            Assert.Equal(expected5, actual5);
            Assert.Equal(expected6, actual6);
            Assert.Equal(expected7, actual7);
            Assert.Equal(expected8, actual8);
        }

        [Fact]
        public void WordBasedTruncation() 
        {
            string actual = "this is a long string";

            string actual1 = actual.TruncateWords(5); // truncate words = actual num of words
            string actual2 = actual.TruncateWords(8); // truncate words > actual num of words
            string actual3 = actual.TruncateWords(4); // truncate words < actual num of words
            string actual4 = actual.TruncateWords(4, true);  // rtl
            string actual5 = actual.TruncateWords(4, false); // ltr
            string actual6 = actual.TruncateWords(4, "~~~");         // custom tail
            string actual7 = actual.TruncateWords(4, "~~~", true);   // custom tail + rtl
            string actual8 = actual.TruncateWords(4, "~~~", false);  // custom tail + ltr

            string expected1 = "this is a long string";
            string expected2 = "this is a long string";
            string expected3 = "is a long string";
            string expected4 = "this is a long";
            string expected5 = "is a long string";
            string expected6 = "~~~is a long string";
            string expected7 = "this is a long~~~";
            string expected8 = "~~~is a long string";

            Assert.Equal(expected1, actual1);
            Assert.Equal(expected2, actual2);
            Assert.Equal(expected3, actual3);
            Assert.Equal(expected4, actual4);
            Assert.Equal(expected5, actual5);
            Assert.Equal(expected6, actual6);
            Assert.Equal(expected7, actual7);
            Assert.Equal(expected8, actual8);
        }
    }
}

