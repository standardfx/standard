using System.Configuration;
using System.Linq;
using Xunit;
using Standard.Configuration;
using Standard.Data.Confon;

namespace Standard.Configuration.Confon.Tests
{
    public class ConfigurationSpecTest
    {
        [Fact]
        public void CanDeserializeConfonConfigurationFromConfigFile()
        {
            ConfonConfigurationSection section = (ConfonConfigurationSection)ConfigurationManager.GetSection("foo");
            Assert.NotNull(section);
            Assert.False(string.IsNullOrEmpty(section.Confon.Content));
            ConfonContext config = section.Config;
            Assert.NotNull(config);
        }

        /*
        [Fact]
        public void CanCreateConfigFromSourceObject()
        {
            var source = new MyObjectConfig
            {
                StringProperty = "aaa",
                BoolProperty = true,
                IntergerArray = new[]{1,2,3,4 }
            };

            var config = ConfonFactory.FromObject(source);

            Assert.Equal("aaa", config.GetString("StringProperty"));
            Assert.Equal(true, config.GetBoolean("BoolProperty"));

            Assert.Equal(new[] { 1, 2, 3, 4 }, config.GetInt32List("IntergerArray").ToArray());
        }
        */
        
        [Fact]
        public void CanMergeObjects()
        {
            string confon1 = @"
a {
    b = 123
    c = 456
    d = 789
    sub {
        aa = 123
    }
}
";

            string confon2 = @"
a {
    c = 999
    e = 888
    sub {
        bb = 456
    }
}
";

            var root1 = ConfonParser.Parse(confon1, null);
            var root2 = ConfonParser.Parse(confon2, null);

            var obj1 = root1.Value.GetObject();
            var obj2 = root2.Value.GetObject();
            obj1.Merge(obj2);

            ConfonContext config = new ConfonContext(root1);

            Assert.Equal(123, config.GetInt32("a.b"));
            Assert.Equal(456, config.GetInt32("a.c"));
            Assert.Equal(789, config.GetInt32("a.d"));
            Assert.Equal(888, config.GetInt32("a.e"));
            Assert.Equal(888, config.GetInt32("a.e"));
            Assert.Equal(123, config.GetInt32("a.sub.aa"));
            Assert.Equal(456, config.GetInt32("a.sub.bb"));
        }

        public class MyObjectConfig
        {
            public string StringProperty { get; set; }
            public bool BoolProperty { get; set; }
            public int[] IntergerArray { get; set; }
        }
   }
}
