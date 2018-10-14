using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Standard.Core;

namespace Standard
{
    /// <summary>
    /// Extensions for working with text streams.
    /// </summary>
    public static class TextStreamExtension
    {
        /// <summary>
        /// Returns all lines in a stream as a string array, using UTF8 encoding.
        /// </summary>
        /// <param name="stream">The stream of text.</param>
        /// <returns>
        /// An array of strings.
        /// </returns>
        public static string[] ReadAllLines(this Stream stream)
        {
            return ReadAllLines(stream, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Returns all lines in a stream as a string array, using UTF8 encoding.
        /// </summary>
        /// <param name="stream">The stream of text.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>
        /// An array of strings.
        /// </returns>
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
