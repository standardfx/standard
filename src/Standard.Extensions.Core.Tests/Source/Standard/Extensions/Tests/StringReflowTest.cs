using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Standard;

namespace Standard.Extensions.Tests
{
    public class StringReflowTest
    {
        [Fact]
        public void SimpleReflow() 
        {
            List<string> actual = "this is a really long long string".Reflow(20).ToList();
            string[] expected = new string[] 
                {
                    "this is a really lon",
                    "g long string"                
                };

            Assert.Equal(expected[0], actual[0]);
            Assert.Equal(expected[1], actual[1]);
        }

        [Fact]
        public void ReflowTurnsNewLineToSpace() 
        {
            List<string> actual = "this is a really\nlong long\r\nstring".Reflow(20).ToList();
            string[] expected = new string[] 
                {
                    "this is a really lon",
                    "g long string"                
                };

            Assert.Equal(expected[0], actual[0]);
            Assert.Equal(expected[1], actual[1]);
        }

        [Fact]
        public void WordReflow() 
        {
            List<string> actual = "this is a really long long string".ReflowWords(20).ToList();
            string[] expected = new string[] 
                {
                    "this is a really",
                    "long long string"                
                };

            Assert.Equal(expected[0], actual[0]);
            Assert.Equal(expected[1], actual[1]);
        }
    }
}

