using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Xunit;

namespace Standard.Data.Json.Tests 
{
    public class SerializeStructTests 
    {

#if FEAT_STRUCT_SUPPORT

        [Fact]
        public void TestStructWithProperties() 
        {
            var data = new StructWithProperties { x = 10, y = 2 };
            var json = JsonConvert.Serialize(data);
            var data2 = JsonConvert.Deserialize<StructWithProperties>(json);
            Assert.Equal(data.x, data.x);
            Assert.Equal(data.y, data.y);
        }

        [Fact]
        public void TestStructWithFields() 
        {
            var data = new StructWithFields { x = 10, y = 2 };
            var json = JsonConvert.Serialize(data);
            var data2 = JsonConvert.Deserialize<StructWithFields>(json);
            Assert.Equal(data.x, data.x);
            Assert.Equal(data.y, data.y);
        }

#endif

    }
}
