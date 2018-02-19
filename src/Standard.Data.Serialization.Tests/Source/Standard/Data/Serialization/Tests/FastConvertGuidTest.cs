using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;
using Standard;

namespace Standard.Serialization.Tests
{
    public class FastConvertBase16Test
    {
        [Fact]
        public void ConversionIsValid()
        {
            Guid g1 = Guid.NewGuid();
            byte[] b = g1.ToByteArray();

            string x = FastConvert.ToBase16String(b, true);
            byte[] y = FastConvert.FromBase16String(x, true);
            Guid g2 = new Guid(y);
            Assert.True(g1 == g2);
        }
    }

    public class FastConvertBase85Test
    {
        [Fact]
        public void ConversionIsValid()
        {
            Guid g1 = Guid.NewGuid();
            byte[] b = g1.ToByteArray();

            string x = FastConvert.ToBase85String(b);
            byte[] y = FastConvert.FromBase85String(x);
            Guid g2 = new Guid(y);
            Assert.True(g1 == g2);
        }
    }

#if !DEBUG
    public class FastConvertGuidPerfTest
    {
        private readonly ITestOutputHelper output;

        public FastConvertGuidPerfTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        private List<Guid> _guidSampleCache;
        private List<string> _guidSampleStringCache;
        private const int PERF_SAMPLE_SIZE = 1000000;

        private void FillGuidSampleCache(bool clear = false)
        {
            if (_guidSampleCache == null)
                _guidSampleCache = new List<Guid>();

            if (_guidSampleStringCache == null)
                _guidSampleStringCache = new List<string>();

            if (clear)
            {
                _guidSampleCache.Clear();
                _guidSampleStringCache.Clear();
            }

            if (_guidSampleCache.Count < PERF_SAMPLE_SIZE)
            {
                output.WriteLine("[TOUT/INFO] Filling sample cache: {0}", PERF_SAMPLE_SIZE - _guidSampleCache.Count);

                for (int i = (_guidSampleCache.Count - 1); i < PERF_SAMPLE_SIZE; i++)
                {
                    _guidSampleCache.Add(Guid.NewGuid());
                }
            }

            if (_guidSampleStringCache.Count < PERF_SAMPLE_SIZE)
            {
                output.WriteLine("[TOUT/INFO] Filling sample cache: {0}", PERF_SAMPLE_SIZE - _guidSampleStringCache.Count);

                for (int i = (_guidSampleStringCache.Count - 1); i < PERF_SAMPLE_SIZE; i++)
                {
                    _guidSampleStringCache.Add(Guid.NewGuid().ToString());
                }
            }

            return;
        }

        [Fact]
        public void GuidToStringPerf()
        {
            FillGuidSampleCache();
            var t = new Stopwatch();

            t.Restart();
            for (int i = 0; i < PERF_SAMPLE_SIZE; i++)
            {
                FastConvert.ToString(_guidSampleCache[i]);
            }
            t.Stop();
            TimeSpan fastConvertPerf = t.Elapsed;
            output.WriteLine("[TOUT/PERF] FastConvert_ToString: {0}", fastConvertPerf);

            t.Restart();
            for (int i = 0; i < PERF_SAMPLE_SIZE; i++)
            {
                Convert.ToString(_guidSampleCache[i]);
            }
            t.Stop();
            TimeSpan systemConvertPerf = t.Elapsed;
            output.WriteLine("[TOUT/PERF] Convert_ToString: {0}", systemConvertPerf);
            output.WriteLine("[TOUT/PERF] " + 
                ToFriendlyPerfResult(fastConvertPerf.Ticks, systemConvertPerf.Ticks).Replace("{#me}", "FastConvert.ToString").Replace("{#ref}", "Convert.ToString"));

            t.Restart();
            for (int i = 0; i < PERF_SAMPLE_SIZE; i++)
            {
                _guidSampleCache[i].ToString();
            }
            t.Stop();
            TimeSpan guidSelfPerf = t.Elapsed;
            output.WriteLine("[TOUT/PERF] Guid_ToString: {0}", guidSelfPerf);
            output.WriteLine("[TOUT/PERF] " + 
                ToFriendlyPerfResult(fastConvertPerf.Ticks, guidSelfPerf.Ticks).Replace("{#me}", "FastConvert.ToString").Replace("{#ref}", "Guid.ToString"));
        }

        [Fact]
        public void StringToGuidPerf()
        {
            FillGuidSampleCache();
            var t = new Stopwatch();

            t.Restart();
            for (int i = 0; i < PERF_SAMPLE_SIZE; i++)
            {
                FastConvert.ToGuid(_guidSampleStringCache[i]);
            }
            t.Stop();
            TimeSpan fastConvertPerf = t.Elapsed;
            output.WriteLine("[TOUT/PERF] FastConvert_ToGuid: {0}", fastConvertPerf);

            t.Restart();
            for (int i = 0; i < PERF_SAMPLE_SIZE; i++)
            {
                Guid.Parse(_guidSampleStringCache[i]);
            }
            t.Stop();
            TimeSpan guidParsePerf = t.Elapsed;
            output.WriteLine("[TOUT/PERF] Guid_Parse: {0}", guidParsePerf);
            output.WriteLine("[TOUT/PERF] " + 
                ToFriendlyPerfResult(fastConvertPerf.Ticks, guidParsePerf.Ticks).Replace("{#me}", "FastConvert.ToGuid").Replace("{#ref}", "Guid.Parse"));
        }

        /// <summary>
        /// Turns performance time comparison to a human friendly sentence.
        /// </summary>
        /// <param name="perfTime">Actual time taken, in ticks.</param>
        /// <param name="refTime">Reference benchmark time in ticks.</param>
        /// <returns>
        /// Human friendly result statement.
        /// </returns>
        private string ToFriendlyPerfResult(long perfTime, long refTime)
        {
            double doubleApprox = 0.00001;
            bool isBetter = false;
            if (perfTime > refTime)
                isBetter = false;
            else
                isBetter = true;

            double diffTimes;
            if (isBetter)
                diffTimes = Math.Round(((double)refTime / (double)perfTime) - 1, 4);
            else
                diffTimes = Math.Round(((double)perfTime / (double)refTime) - 1, 4);
            
            string friendlyCompareResult;
            if (diffTimes - 1 > doubleApprox)
                friendlyCompareResult = (diffTimes + 1).ToString() + " times the speed of";
            else if (diffTimes - 0 > doubleApprox)
                friendlyCompareResult = string.Format("{0}% faster than", (diffTimes) * 100);
            else if (diffTimes >= 0 && diffTimes - 0 <= doubleApprox)
                friendlyCompareResult = "about the same speed as";
            else
                friendlyCompareResult = string.Format("{0}% the speed of", diffTimes * 100);
            
            if (isBetter)
                return string.Format("#me is {0} #ref", friendlyCompareResult).Replace("#me", "{#me}").Replace("#ref", "{#ref}");
            else
                return string.Format("#ref is {0} #me", friendlyCompareResult).Replace("#me", "{#me}").Replace("#ref", "{#ref}");
        }

        /// <summary>
        /// Turns performance time comparison to a human friendly sentence.
        /// </summary>
        /// <param name="perfTime">Actual time taken, in ticks.</param>
        /// <param name="refTime">Reference benchmark time in ticks.</param>
        /// <param name="longerIsBetter">Treats longer time taken as better performance.</param>
        /// <returns>
        /// Human friendly result statement.
        /// </returns>
        private string ToFriendlyPerfResult(long perfTime, long refTime, bool longerIsBetter)
        {
            // if longer is better, just swap #me and #ref
            if (longerIsBetter)
            {
                return ToFriendlyPerfResult(refTime, perfTime).Replace(
                    "{#me}", "{{#foo}}"
                ).Replace(
                    "{#ref}", "{#me}"
                ).Replace(
                    "{{#foo}}", "{#ref}"
                );
            }
            else
            {
                return ToFriendlyPerfResult(perfTime, refTime);
            }
        }
    }
#endif

    public class FastConvertGuidTest
    {
        [Fact]
        public void GuidSerialization()
        {
            Guid g = Guid.NewGuid();
            foreach (string c in new string[] { "d", "n", "p", "b", "x" })
            {
                Assert.Equal(g.ToString(c), FastConvert.ToString(g, c));
            }
        }        

        [Fact]
        public void GuidDeserialization()
        {
            Guid g = Guid.NewGuid();
            foreach (string c in new string[] { "d", "n", "p", "b", "x" })
            {
                string gstr = g.ToString(c);
                Assert.True(Guid.Parse(gstr) == FastConvert.ToGuid(gstr, c));
            }
        }        
    }
}
