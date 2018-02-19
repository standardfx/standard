using System.Collections.Generic;

namespace Standard.Data.Markdown
{
    public class MarkdownHeadingBlockToken : IMarkdownExpression, IMarkdownRewritable<MarkdownHeadingBlockToken>
    {
        public MarkdownHeadingBlockToken(IMarkdownRule rule, IMarkdownContext context, InlineContent content, string id, int depth, SourceInfo sourceInfo)
        {
            Rule = rule;
            Context = context;
            Content = content;
            Id = id;
            Depth = depth;
            SourceInfo = sourceInfo;
        }

        public IMarkdownRule Rule { get; }

        public IMarkdownContext Context { get; }

        public InlineContent Content { get; }

        public string Id { get; }

        public int Depth { get; }

        public SourceInfo SourceInfo { get; }

        public MarkdownHeadingBlockToken Rewrite(IMarkdownRewriteEngine rewriterEngine)
        {
            var c = Content.Rewrite(rewriterEngine);
            if (c == Content)
                return this;

            return new MarkdownHeadingBlockToken(Rule, Context, c, Id, Depth, SourceInfo);
        }

        internal MarkdownHeadingBlockToken RewriteId(Dictionary<string, int> idTable)
        {
            var newId = GenerateNewId(idTable, Id);
            if (string.Equals(newId, Id))
                return null;

            return new MarkdownHeadingBlockToken(Rule, Context, Content, newId, Depth, SourceInfo);
        }

        private string GenerateNewId(Dictionary<string, int> idTable, string Id)
        {
            if (idTable.TryGetValue(Id, out int count))
            {
                var newId = Id + "-" + count.ToString();
                idTable[Id] = count + 1;
                return GenerateNewId(idTable, newId);
            }
            else
            {
                idTable[Id] = 1;
                return Id;
            }
        }

        public IEnumerable<IMarkdownToken> GetChildren() => Content.Tokens;
    }
}
