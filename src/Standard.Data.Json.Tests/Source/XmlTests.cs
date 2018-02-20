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

namespace Standard.Data.Json.Tests
{
    public class XmlTests
    {
		[Fact]
		public void TestUsingAttributeOfXmlForName()
		{
			var data = new XmlTestClass { Name = "Value" };
			var json = JsonConvert.Serialize(data);

			Assert.Contains("XmlName", json);
		}
	}

	public class XmlTestClass
	{
		[XmlElement(ElementName = "XmlName")]
		public string Name { get; set; }

	}
}
