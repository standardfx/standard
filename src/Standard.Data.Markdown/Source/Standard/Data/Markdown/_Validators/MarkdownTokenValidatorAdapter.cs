using System.Collections.Generic;
using System.Collections.Immutable;

namespace Standard.Data.Markdown
{
    internal sealed class MarkdownTokenValidatorAdapter : IMarkdownTokenRewriter, IInitializable
    {
        public ImmutableArray<IMarkdownTokenValidator> Validators { get; }

        public MarkdownTokenValidatorAdapter(IEnumerable<IMarkdownTokenValidator> validators)
        {
            Validators = validators.ToImmutableArray();
        }

        public IMarkdownToken Rewrite(IMarkdownRewriteEngine engine, IMarkdownToken token)
        {
            foreach (var validator in Validators)
            {
                validator.Validate(token);
            }
            return token;
        }

        public void Initialize(IMarkdownRewriteEngine rewriteEngine)
        {
            foreach (var item in Validators)
            {
                (item as IInitializable)?.Initialize(rewriteEngine);
            }
        }
    }
}
