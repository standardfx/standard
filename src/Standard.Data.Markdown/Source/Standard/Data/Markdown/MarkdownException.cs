using System;

namespace Standard.Data.Markdown
{
    public class MarkdownException : Exception
    {
        public MarkdownException()
        {
        }

        public MarkdownException(string message) 
			: base(message)
        {
        }

        public MarkdownException(string message, Exception innerException) 
			: base(message, innerException)
        {
        }
    }

    public class MarkdownParsingException : MarkdownException
    {
        public SourceInfo SourceInfo { get; private set; }

        public MarkdownParsingException(SourceInfo sourceInfo)
            : this(RS.MarkdownParseFailure, sourceInfo)
        {
        }

        public MarkdownParsingException(string message, SourceInfo sourceInfo) 
			: base(GetMessage(message, sourceInfo))
        {
        }

        public MarkdownParsingException(string message, SourceInfo sourceInfo, Exception innerException) 
			: base(GetMessage(message, sourceInfo), innerException)
        {
        }

        private static string GetMessage(string message, SourceInfo sourceInfo)
        {
            StringBuffer sb = message;
            if (sourceInfo.File != null)
                sb = sb + RS.ExceptionIn + sourceInfo.File;

            if (sourceInfo.LineNumber > 0)
                sb = sb + RS.ExceptionAtLine + sourceInfo.LineNumber.ToString();

            sb += RS.ExceptionContent;
            sb += Environment.NewLine;

			var md = sourceInfo.Markdown;
            if (md.Length > 256)
                md = md.Remove(256);

            foreach (var line in md.Split('\n'))
            {
                sb = sb + "> " + line;
            }
            return sb.ToString();
        }
    }
}
