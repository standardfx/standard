using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Standard.StringParsing.Tests
{
    public class AmqpErrorParserTests
    {    
        [Fact]
        public void ShouldParseAMQPErrorString()
        {
            const string originalErrorString =
                "The AMQP operation was interrupted: AMQP close-reason, initiated by Peer, " +
                "code=406, text=\"PRECONDITION_FAILED - parameters for queue 'my.redeclare.queue' in vhost '/' not equivalent\", " +
                "classId=50, methodId=10, cause=";

            var itemsResult = AmqpErrorParser.Eval(originalErrorString).OfType<KeyValue>().ToDictionary(x => x.Key, x => x.Value);
            
            // foreach (var amqpErrorItem in itemsResult)
            // {
            //     Console.Out.WriteLine("{0}", amqpErrorItem);
            // }

            Assert.Equal("406", itemsResult["code"].ToString());
            Assert.Equal("PRECONDITION_FAILED - parameters for queue 'my.redeclare.queue' in vhost '/' not equivalent", itemsResult["text"].ToString());
            Assert.Equal("50", itemsResult["classId"].ToString());
            Assert.Equal("10", itemsResult["methodId"].ToString());
        }
    }
}
