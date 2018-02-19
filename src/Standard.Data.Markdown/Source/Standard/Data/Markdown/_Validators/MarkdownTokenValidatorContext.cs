using System;
using System.Diagnostics;

namespace Standard.Data.Markdown
{
    public class MarkdownTokenValidatorContext : IDisposable
    {
        [ThreadStatic]
        private static MarkdownTokenValidatorContext _current;

        private readonly IMarkdownRewriteEngine _rewriteEngine;
        private readonly string _file;

        internal MarkdownTokenValidatorContext(IMarkdownRewriteEngine rewriteEngine, string file)
        {
            _rewriteEngine = rewriteEngine;
            _file = file;
            Debug.Assert(_current == null, RS.ExpectNullContext);
            _current = this;
        }

        public static IMarkdownRewriteEngine CurrentRewriteEngine => _current?._rewriteEngine;

        public static string CurrentFile => _current?._file;

        void IDisposable.Dispose()
        {
            _current = null;
        }
    }
}
