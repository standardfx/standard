using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using Xunit;
using Standard.Data.StringMetrics;

namespace Standard.Data.StringMetrics.Tests
{
    public class TokenizerFixture : IDisposable
    {
        public TokenizerQGram3 TokenizerQGram3;
        public TokenizerQGram3Extended TokenizerQGram3Extended;
        public TokenizerQGram2 TokenizerQGram2;
        public TokenizerSGram2 TokenizerSGram2;
        public TokenizerQGram2Extended TokenizerQGram2Extended;
        public TokenizerSGram2Extended TokenizerSGram2Extended;
        public TokenizerWhitespace TokenizerWhitespace;

        public TokenizerFixture()
        {
            TokenizerQGram3 = new TokenizerQGram3();
            TokenizerQGram3Extended = new TokenizerQGram3Extended();
            TokenizerQGram2 = new TokenizerQGram2();
            TokenizerSGram2 = new TokenizerSGram2();
            TokenizerQGram2Extended = new TokenizerQGram2Extended();
            TokenizerSGram2Extended = new TokenizerSGram2Extended();
            TokenizerWhitespace = new TokenizerWhitespace();
        }

        public void Dispose()
        {
            // do nothing
        }
    }

    public class TokenizerTests : IClassFixture<TokenizerFixture>
    {
        TokenizerFixture fixture;

        public TokenizerTests(TokenizerFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void TokenizerQGram3TestData()
        {
            Collection<string> myKnownResult = new Collection<string>();
            myKnownResult.Add("CHR");
            myKnownResult.Add("HRI");
            myKnownResult.Add("RIS");
            //myKnownResult.TrimExcess();
            Collection<string> myResult = fixture.TokenizerQGram3.Tokenize("CHRIS");
            for (int i = 0; i < myKnownResult.Count; i++) 
            {
                Assert.Equal(myKnownResult[i], myResult[i]);
            }
        }

        [Fact]
        public void TokenizerQGram3ExtendedTestData()
        {
            Collection<string> myKnownResult = new Collection<string>();
            myKnownResult.Add("??C");
            myKnownResult.Add("?CH");
            myKnownResult.Add("CHR");
            myKnownResult.Add("HRI");
            myKnownResult.Add("RIS");
            myKnownResult.Add("IS#");
            myKnownResult.Add("S##");
            Collection<string> myResult = fixture.TokenizerQGram3Extended.Tokenize("CHRIS");
            for (int i = 0; i < myKnownResult.Count; i++) 
            {
                Assert.Equal(myKnownResult[i], myResult[i]);
            }
        }

        [Fact]
        public void TokenizerQGram2TestData()
        {
            Collection<string> myKnownResult = new Collection<string>();
            myKnownResult.Add("CH");
            myKnownResult.Add("HR");
            myKnownResult.Add("RI");
            myKnownResult.Add("IS");
            //myKnownResult.TrimExcess();
            Collection<string> myResult = fixture.TokenizerQGram2.Tokenize("CHRIS");
            for (int i = 0; i < myKnownResult.Count; i++) 
            {
                Assert.Equal(myKnownResult[i], myResult[i]);
            }
        }

        [Fact]
        public void TokenizerQGram2TestWithCci1_Data()
        {
            Collection<string> myKnownResult = new Collection<string>();
            myKnownResult.Add("CH");
            myKnownResult.Add("HR");
            myKnownResult.Add("RI");
            myKnownResult.Add("IS");
            myKnownResult.Add("CR");
            myKnownResult.Add("HI");
            myKnownResult.Add("RS");
            //myKnownResult.TrimExcess();
            fixture.TokenizerQGram2.CharacterCombinationIndex = 1;
            Collection<string> myResult = fixture.TokenizerQGram2.Tokenize("CHRIS");
            for (int i = 0; i < myKnownResult.Count; i++) 
            {
                Assert.Equal(myKnownResult[i], myResult[i]);
            }
        }

        [Fact]
        public void TokenizerQGram2ExtendedTestData()
        {
            Collection<string> myKnownResult = new Collection<string>();
            myKnownResult.Add("?C");
            myKnownResult.Add("CH");
            myKnownResult.Add("HR");
            myKnownResult.Add("RI");
            myKnownResult.Add("IS");
            myKnownResult.Add("S#");
            Collection<string> myResult = fixture.TokenizerQGram2Extended.Tokenize("CHRIS");
            for (int i = 0; i < myKnownResult.Count; i++) 
            {
                Assert.Equal(myKnownResult[i], myResult[i]);
            }
        }

        [Fact]
        public void TokenizerQGram2ExtendedTestCc1_Data()
        {
            Collection<string> myKnownResult = new Collection<string>();
            myKnownResult.Add("?C");
            myKnownResult.Add("CH");
            myKnownResult.Add("HR");
            myKnownResult.Add("RI");
            myKnownResult.Add("IS");
            myKnownResult.Add("S#");
            myKnownResult.Add("?H");
            myKnownResult.Add("CR");
            myKnownResult.Add("HI");
            myKnownResult.Add("RS");
            myKnownResult.Add("I#");
            fixture.TokenizerQGram2Extended.CharacterCombinationIndex = 1;
            Collection<string> myResult = fixture.TokenizerQGram2Extended.Tokenize("CHRIS");
            for (int i = 0; i < myKnownResult.Count; i++) 
            {
                Assert.Equal(myKnownResult[i], myResult[i]);
            }
        }

        [Fact]
        public void TokenizerSGram2ExtendedTestCc1_Data()
        {
            Collection<string> myKnownResult = new Collection<string>();
            myKnownResult.Add("?C");
            myKnownResult.Add("CH");
            myKnownResult.Add("HR");
            myKnownResult.Add("RI");
            myKnownResult.Add("IS");
            myKnownResult.Add("S#");
            myKnownResult.Add("?H");
            myKnownResult.Add("CR");
            myKnownResult.Add("HI");
            myKnownResult.Add("RS");
            myKnownResult.Add("I#");
            Collection<string> myResult = fixture.TokenizerSGram2Extended.Tokenize("CHRIS");
            for (int i = 0; i < myKnownResult.Count; i++) 
            {
                Assert.Equal(myKnownResult[i], myResult[i]);
            }
        }

        [Fact]
        public void TokenizerSGram2TestWithCci1_Data()
        {
            Collection<string> myKnownResult = new Collection<string>();
            myKnownResult.Add("CH");
            myKnownResult.Add("HR");
            myKnownResult.Add("RI");
            myKnownResult.Add("IS");
            myKnownResult.Add("CR");
            myKnownResult.Add("HI");
            myKnownResult.Add("RS");
            Collection<string> myResult = fixture.TokenizerSGram2.Tokenize("CHRIS");
            for (int i = 0; i < myKnownResult.Count; i++) 
            {
                Assert.Equal(myKnownResult[i], myResult[i]);
            }
        }

        [Fact]
        public void TokeniserWhitespaceTestData()
        {
            Collection<string> myKnownResult = new Collection<string>();
            myKnownResult.Add("CHRIS");
            myKnownResult.Add("IS");
            myKnownResult.Add("HERE");
            Collection<string> myResult = fixture.TokenizerWhitespace.Tokenize("CHRIS IS HERE");
            for (int i = 0; i < myKnownResult.Count; i++) 
            {
                Assert.Equal(myKnownResult[i], myResult[i]);
            }
        }

        [Fact]
        public void TokeniserWhitespaceDelimiterTest()
        {
            Collection<string> myKnownResult = new Collection<string>();
            myKnownResult.Add("CHRIS");
            myKnownResult.Add("IS");
            myKnownResult.Add("");
            myKnownResult.Add("HERE");
            myKnownResult.Add("woo");
            Collection<string> myResult = fixture.TokenizerWhitespace.Tokenize("CHRIS\nIS\r HERE\twoo");
            for (int i = 0; i < myKnownResult.Count; i++) 
            {
                Assert.Equal(myKnownResult[i], myResult[i]);
            }
        }
    }    
}
