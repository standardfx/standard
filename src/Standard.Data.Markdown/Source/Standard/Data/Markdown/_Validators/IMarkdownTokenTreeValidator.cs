using System.Collections.Immutable;

namespace Standard.Data.Markdown
{
    public interface IMarkdownTokenTreeValidator
    {
        void Validate(ImmutableArray<IMarkdownToken> tokens);
    }
}
