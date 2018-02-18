using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Standard
{
    /// <summary>
    /// Extensions for working with text streams.
    /// </summary>
    public static class TextStreamExtension
    {
        //<#
        //  .SYNOPSIS
        //      Returns all lines in @[stream] as a @String array, using UTF8 encoding.
        // 
        //  .PARAM stream
        //      The stream to evaluate.
        //#>
        public static string[] ReadAllLines(this Stream stream)
        {
            return ReadAllLines(stream, System.Text.Encoding.UTF8);
        }

        //<#
        //  .INHERIT ReadAllLines(Stream)
        //
        //  .SYNOPSIS
        //      Returns all lines in @[stream] as a @String array.
        // 
        //  .PARAM encoding
        //      The encoding to use.
        //#>
        public static string[] ReadAllLines(this Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
                
            string line;
            List<string> lines = new List<string>();

            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines.ToArray();
        }
    }
}
