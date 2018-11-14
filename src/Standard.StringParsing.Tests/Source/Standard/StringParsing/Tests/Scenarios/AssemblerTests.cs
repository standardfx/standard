using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Standard.StringParsing.Tests
{
    public class AssemblerTests
    {
        [Fact]
        public void CanParseEmpty()
        {
            AssertParser.SucceedsWith(AssemblerParser.Assembler, string.Empty, Assert.Empty);
        }

        [Fact]
        public void CanParseComment()
        {
            AssertParser.SucceedsWith(
                AssemblerParser.Assembler, 
                ";comment",
                lines => Assert.Equal(new AssemblerLine(null, null, null, "comment"), lines.Single()));
        }

        [Fact]
        public void CanParseCommentWithSpaces()
        {
            AssertParser.SucceedsWith(AssemblerParser.Assembler, 
                "  ; comment ",
                lines => Assert.Equal(new AssemblerLine(null, null, null, " comment "), lines.Single()));
        }

        [Fact]
        public void CanParseLabel()
        {
            AssertParser.SucceedsWith(
                AssemblerParser.Assembler, "label:",
                lines => Assert.Equal(new AssemblerLine("label", null, null, null), lines.Single()));
        }

        [Fact]
        public void CanParseLabelWithSpaces()
        {
            AssertParser.SucceedsWith(
                AssemblerParser.Assembler, 
                "  label :  ",
                lines => Assert.Equal(new AssemblerLine("label", null, null, null), lines.Single()));
        }

        [Fact]
        public void CanParseIntruction()
        {
            AssertParser.SucceedsWith(
                AssemblerParser.Assembler, 
                "mov a,b",
                lines => Assert.Equal(new AssemblerLine(null, "mov", new[] { "a", "b" }, null), lines.Single()));
        }

        [Fact]
        public void CanParseIntructionWithSpaces()
        {
            AssertParser.SucceedsWith(
                AssemblerParser.Assembler, 
                " mov   a , b ",
                lines => Assert.Equal(new AssemblerLine(null, "mov", new[] { "a", "b" }, null), lines.Single()));
        }

        [Fact]
        public void CanParseAllTogether()
        {
            AssertParser.SucceedsWith(
                AssemblerParser.Assembler, 
                "  label:  mov   a  ,  b  ;  comment  ",
                lines => Assert.Equal(new AssemblerLine("label", "mov", new[] { "a", "b" }, "  comment  "), lines.Single()));
        }

        [Fact]
        public void CanParseSeceralLines()
        {
            AssertParser.SucceedsWith(
                AssemblerParser.Assembler,
                @"      ;multiline sample
                        label:  
                        mov   a  ,  b;",
                lines => Assert.Equal(
                    new[]
                    {
                        new AssemblerLine(null, null, null, "multiline sample"),
                        new AssemblerLine("label", null, null, null),
                        new AssemblerLine(null, "mov", new[] {"a", "b"}, "")
                    },
                    lines));
        }
    }
}
