using System;

namespace Standard.Data.Markdown
{
    public class Options
    {
		public Options()
		{
			Highlight = null;
			Sanitizer = null;
			LangPrefix = "lang-";
			HeaderPrefix = "";
			XHtml = false;
			Sanitize = false;
			Pedantic = false;
			Mangle = true;
			Smartypants = false;
			Breaks = false;
			Gfm = true;
			Tables = true;
			SmartLists = false;
			ShouldExportSourceInfo = false;
		}

		#region Properties

		public Func<string, string, string> Highlight { get; set; }

        public Func<string, string> Sanitizer { get; set; }

        public string LangPrefix { get; set; }

        public string HeaderPrefix { get; set; }

        public bool XHtml { get; set; }

        public bool Sanitize { get; set; }

        public bool Pedantic { get; set; }

        public bool Mangle { get; set; }

        public bool Smartypants { get; set; }

        public bool Breaks { get; set; }

        public bool Gfm { get; set; }

        public bool Tables { get; set; }

        public bool SmartLists { get; set; }

        public bool ShouldExportSourceInfo { get; set; }

        public bool LegacyMode { get; set; }

        #endregion // Properties
    }
}
