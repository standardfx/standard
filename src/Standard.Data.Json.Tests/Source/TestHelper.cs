using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Standard.Data.Json.Tests 
{
    internal static class TestHelper
    {
        public static string GetEmbedFileContent(string fileName)
        {
#if NETSTANDARD
            Assembly assembly = typeof(TestHelper).GetTypeInfo().Assembly;
#else
            Assembly assembly = Assembly.GetExecutingAssembly();
#endif

            string resourceName = string.Format("Standard.Data.Json.Tests.Resource.{0}", fileName);

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();

                    return result;
                }
            }
        }
    }
}
