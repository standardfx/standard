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
    public class SerializePolyTests 
    {

#if FEAT_POLY_SERIALIZATION

        [Fact]
        public void SerializePolyObjects() 
        {            
            var graph = new Graph { name = "my graph" };
            graph.nodes = new List<Node>();
            graph.nodes.Add(new NodeA { number = 10f });
            graph.nodes.Add(new NodeB { text = "hello" });

            JsonConvert.IncludeTypeInfo = true;

            var json = JsonConvert.Serialize(graph);
            var jgraph = JsonConvert.Deserialize<Graph>(json);

            var nodeA = jgraph.nodes[0] as NodeA;
            var nodeB = jgraph.nodes[1] as NodeB;

            Assert.True(nodeA != null);
            Assert.True(nodeB != null);
            Assert.True(nodeA != null && nodeA.number == 10, json);
            Assert.True(nodeB != null && nodeB.text == "hello", json);

            JsonConvert.IncludeTypeInfo = false;
        }

#endif

    }
}
