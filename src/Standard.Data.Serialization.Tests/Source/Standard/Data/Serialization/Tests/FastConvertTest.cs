using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Standard;

namespace Standard.Data.Serialization.Tests
{
    public class FastConvertTest
    {
        private List<int> _intCache;
        private List<long> _longCache;
        private List<double> _doubleCache;
        private const float ZeroTolerance = 1e-6f;

        [Fact]
        public void ConvertInt32ToString() 
        {
            FillNumberCache();
            int loopCount = _intCache.Count;

            int i = 0;
            while (i < loopCount)
            {
                Assert.Equal(Convert.ToString(_intCache[i]), FastConvert.ToString(_intCache[i]));
                i++;
            }
        }

        [Fact]
        public void ConvertStringToInt32() 
        {
            FillNumberCache();
            int loopCount = _intCache.Count;

            int i = 0;
            while (i < loopCount)
            {
                Assert.Equal(_intCache[i], FastConvert.ToInt32(_intCache[i].ToString()));
                i++;
            }
        }

        [Fact]
        public void ConvertUInt32ToString() 
        {
            FillNumberCache();
            int loopCount = _intCache.Count;

            int i = 0;
            while (i < loopCount)
            {
                uint testNum = (uint)_intCache[i] + 2147483647;
                Assert.Equal(Convert.ToString(testNum), FastConvert.ToString(testNum));
                i++;
            }
        }

        [Fact]
        public void ConvertStringToUInt32() 
        {
            FillNumberCache();
            int loopCount = _intCache.Count;

            int i = 0;
            while (i < loopCount)
            {
                uint testNum = (uint)_intCache[i] + 2147483647;
                Assert.Equal(testNum, FastConvert.ToUInt32(testNum.ToString()));
                i++;
            }
        }

        [Fact]
        public void ConvertInt64ToString() 
        {
            FillNumberCache();
            int loopCount = _longCache.Count;

            int i = 0;
            while (i < loopCount)
            {
                Assert.Equal(Convert.ToString(_longCache[i]), FastConvert.ToString(_longCache[i]));
                i++;
            }
        }

        [Fact]
        public void ConvertStringToInt64() 
        {
            FillNumberCache();
            int loopCount = _longCache.Count;

            int i = 0;
            while (i < loopCount)
            {
                Assert.Equal(_longCache[i], FastConvert.ToInt64(_longCache[i].ToString()));
                i++;
            }
        }

        [Fact]
        public void ConvertUInt64ToString() 
        {
            FillNumberCache();
            int loopCount = _longCache.Count;

            int i = 0;
            while (i < loopCount)
            {
                ulong testNum = (ulong)_longCache[i] + 9223372036854775807;
                Assert.Equal(Convert.ToString(testNum), FastConvert.ToString(testNum));
                i++;
            }
        }

        [Fact]
        public void ConvertStringToUInt64() 
        {
            FillNumberCache();
            int loopCount = _longCache.Count;

            int i = 0;
            while (i < loopCount)
            {
                ulong testNum = (ulong)_longCache[i] + 9223372036854775807;
                Assert.Equal(testNum, FastConvert.ToUInt64(testNum.ToString()));
                i++;
            }
        }

        [Fact]
        public void ConvertDoubleToString() 
        {
            FillNumberCache();

            int loopCount = _doubleCache.Count;

            int i = 0;
            while (i < loopCount)
            {
                Assert.Equal(Convert.ToString(_doubleCache[i]), FastConvert.ToString(_doubleCache[i]));
                i++;
            }
        }

        [Fact]
        public void ConvertStringToDouble() 
        {
            FillNumberCache();
            int loopCount = _doubleCache.Count;

            int i = 0;
            while (i < loopCount)
            {
                Assert.True(
                    (_doubleCache[i] - FastConvert.ToDouble(_doubleCache[i].ToString()) < ZeroTolerance) ||
                    (FastConvert.ToDouble(_doubleCache[i].ToString()) - _doubleCache[i] < ZeroTolerance));
                i++;
            }
        }

        [Fact]
        public void ConvertSingleToString() 
        {
            FillNumberCache();

            int loopCount = _doubleCache.Count;

            int i = 0;
            while (i < loopCount)
            {
                Assert.Equal(Convert.ToString((float)_doubleCache[i]), FastConvert.ToString((float)_doubleCache[i]));
                i++;
            }
        }

        [Fact]
        public void ConvertStringToSingle() 
        {
            FillNumberCache();
            int loopCount = _doubleCache.Count;

            int i = 0;
            while (i < loopCount)
            {
                Assert.True(
                    ((float)_doubleCache[i] - FastConvert.ToSingle(_doubleCache[i].ToString()) < ZeroTolerance) ||
                    (FastConvert.ToSingle(_doubleCache[i].ToString()) - (float)_doubleCache[i] < ZeroTolerance));
                i++;
            }
        }

        private void FillNumberCache()
        {
            int sampleSize = 1000;

            if (_intCache == null)            
            {
                _intCache = new List<int>();

                Random r = new Random();
                int i = 0;
                while (i < sampleSize)
                {
                    _intCache.Add(r.Next());
                    i++;
                }
            }

            if (_longCache == null)    
            {
                _longCache = new List<long>();

                Random r = new Random();
                int i = 0;
                while (i < sampleSize)
                {
                    _longCache.Add(r.Next());
                    i++;
                }
            }

            if (_doubleCache == null)    
            {
                _doubleCache = new List<double>();

                Random r = new Random();
                int i = 0;
                while (i < sampleSize)
                {
                    _doubleCache.Add(r.NextDouble());
                    i++;
                }
            }
        }
    }
}
