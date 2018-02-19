using System;
using System.Collections.Generic;

namespace Standard.Data.Markdown.Matchers
{
    public class MatchResult
    {
        private readonly MatchContent _mc;
        public int Length { get; }

        public MatchResult(int length, MatchContent mc)
        {
            Length = length;
            _mc = mc;
        }

        public MatchGroup this[string name]
        {
            get
            {
                var group = GetGroup(name);
                if (group == null)
                    throw new ArgumentException($"Group {name} not found.", nameof(name));

                return group.Value;
            }
        }


        public MatchGroup? GetGroup(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return _mc.GetGroup(name);
        }

        public IEnumerable<MatchGroup> EnumerateGroups() =>
            _mc.EnumerateGroups();
    }
}
