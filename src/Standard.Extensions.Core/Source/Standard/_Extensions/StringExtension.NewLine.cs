using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Standard
{
    partial class StringExtension
    {
        public static string NormalizeNewLine(this string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input == string.Empty)
                return input;

            // Allow 10% as a rough guess of how much the string may grow.
            // If we're wrong we'll either waste space or have extra copies -
            // it will still work
            StringBuilder builder = new StringBuilder((int)(input.Length * 1.1));

            bool lastWasCr = false;

            foreach (char c in input)
            {
                if (lastWasCr)
                {
                    lastWasCr = false;
                    if (c == '\n')
                        continue; // Already written \r\n
                }
                switch (c)
                {
                    case '\r':
                        builder.Append("\r\n");
                        lastWasCr = true;
                        break;
                    case '\n':
                        builder.Append("\r\n");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }

            if (Environment.NewLine == "\r\n")
                return builder.ToString();
            else
                return builder.ToString().Replace("\r\n", Environment.NewLine);
        }
    }
}
