using System.Collections.Immutable;

namespace Standard.Data.Markdown
{
    public interface IMarkdownTokenValidatorProvider
    {
         ImmutableArray<IMarkdownTokenValidator> GetValidators();
    }
}
