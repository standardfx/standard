using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Standard.StringParsing.Tests
{
    internal class PosAwareStr : IPositionAware<PosAwareStr>
    {
        public PosAwareStr SetPosition(Position startPos, int length)
        {
            Position = startPos;
            Length = length;
            return this;
        }

        public Position Position { get; set; }

        public int Length { get; set; }

        public string Value { get; set; }
    }
}
