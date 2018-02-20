using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Standard.Data.Json.Tests
{
#if FEAT_SUPPORT_XML_ATTRIBUTE
	public class XmlTests
    {
		private readonly ITestOutputHelper output;

		public XmlTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void TestUsingAttributeOfXmlForName()
		{
			var data = new XmlTestClass { Name = "MyValue" };
			var json = JsonConvert.Serialize(data);
			output.WriteLine(json);

			Assert.Contains("XmlName", json);
		}
	}
#endif

	public class XmlTestClass
	{
		[XmlElement(ElementName = "XmlName")]
		public string Name { get; set; }

	}
}
