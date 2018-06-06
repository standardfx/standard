using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Standard;

namespace Standard.Extensions.Tests
{
    public class StringCroppingTest
    {
        [Fact]
        public void CanGetLeftNChars() 
        {
            string actual = "this is a long string".FromStart(4);
            string expected = "this";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGetRightNCharsByLeftNegative() 
        {
            string actual = "this is a long string".FromStart(-6);
            string expected = "string";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGetRightNChars() 
        {
            string actual = "this is a long string".FromEnd(6);
            string expected = "string";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGetLeftNCharsByRightNegative() 
        {
            string actual = "this is a long string".FromEnd(-4);
            string expected = "this";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGetBefore() 
        {
            string actual = "this is a long string".Before(" a ");
            string actual2 = "this is a long string".Before(" A ");
            string actual3 = "this is a long string".BeforeIgnoreCase(" A ");
            string expected = "this is";

            Assert.Equal(expected, actual);
            Assert.Equal("this is a long string", actual2);
            Assert.Equal(expected, actual3);
        }

        [Fact]
        public void CanGetAfter() 
        {
            string actual = "this is a long string".After("long ");
            string actual2 = "this is a long string".After(" Long ");
            string actual3 = "this is a long string".AfterIgnoreCase(" Long ");
            string expected = "string";

            Assert.Equal(expected, actual);
            Assert.Equal("this is a long string", actual2);
            Assert.Equal(expected, actual3);
        }

        [Fact]
        public void CanGetBetween() 
        {
            string source = "xxx<html>dfsafds<head>fds<title>4534543</title>#$$!#@$#<title>897978783</title>cccc</head>....</html>yyy";

            string[] actual = source.Between(new string[] { "<title>" }, new string[] { "</title>" });
            string[] expected = new string[] { "4534543", "897978783" };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGetFirstBetween() 
        {
            string source = "xxx<html>dfsafds<head>fds<title>4534543</title>#$$!#@$#<title>897978783</title>cccc</head>....</html>yyy";

            string actual = source.FirstBetween(new string[] { "<title>" }, new string[] { "</title>" });
            string expected = "4534543";

            Assert.Equal(expected, actual);
        }
    }
}

