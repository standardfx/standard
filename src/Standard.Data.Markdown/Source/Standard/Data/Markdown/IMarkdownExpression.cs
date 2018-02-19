using System.Collections.Generic;

namespace Standard.Data.Markdown
{
    public interface IMarkdownExpression : IMarkdownToken
    {
        IEnumerable<IMarkdownToken> GetChildren();
    }
}
