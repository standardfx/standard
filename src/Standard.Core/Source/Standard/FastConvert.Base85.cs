using System;
using System.Security;
using System.IO;
using System.Text;
using Standard.Core;

namespace Standard
{
    partial class FastConvert
    {
        private const int AsciiOffset = 33;
        private static uint[] s_pow85 = 
            { 
                85 * 85 * 85 * 85, 
                85 * 85 * 85, 
                85 * 85, 
                85, 
                1 
            };

        /// <summary>
        /// Decodes a Base85 string to its binary form.
        /// </summary>
        /// <param name="value">The Base85 encoded string.</param>
        public static byte[] FromBase85String(string value)
        {
            return Base85Decode(value, false);
        }

        /// <summary>
        /// Decodes a Base85 string to its binary form.
        /// </summary>
        /// <param name="value">The Base85 encoded string.</param>
        /// <param name="prefix">Prefix characters that needs to be trimmed off.</param>
        /// <param name="suffix">Suffix characters that needs to be trimmed off.</param>
        public static byte[] FromBase85String(string value, string prefix, string suffix)
        {
            if (prefix == null)
                throw new ArgumentNullException("prefix");
            if (suffix == null)
                throw new ArgumentNullException("suffix");

            return Base85Decode(value, true, prefix, suffix);
        }

        /// <summary>
        /// Encodes binary data to Base85 string.
        /// </summary>
        /// <param name="value">The binary data to be encoded.</param>
        public static string ToBase85String(byte[] value)
        {
            return Base85Encode(value, 0, false);
        }

        /// <summary>
        /// Encodes binary data to Base85 string.
        /// </summary>
        /// <param name="value">The binary data to be encoded.</param>
        /// <param name="lineLength">Insert line break for every number of characters specified. If value is 0 or less, no line break will be inserted.</param>
        public static string ToBase85String(byte[] value, int lineLength)
        {
            if (lineLength < 0)
                throw new ArgumentOutOfRangeException("lineLength", RS.BadBase85LineLength);

            return Base85Encode(value, lineLength, false);
        }

        /// <summary>
        /// Encodes binary data to Base85 string.
        /// </summary>
        /// <param name="value">The binary data to be encoded.</param>
        /// <param name="prefix">Prefix characters that will be prepended to the output.</param>
        /// <param name="suffix">Prefix characters that will be appended to the output.</param>
        public static string ToBase85String(byte[] value, string prefix, string suffix)
        {
            if (prefix == null)
                throw new ArgumentNullException("prefix");
            if (suffix == null)
                throw new ArgumentNullException("suffix");

            return Base85Encode(value, 0, true, prefix, suffix);
        }

        /// <summary>
        /// Encodes binary data to Base85 string.
        /// </summary>
        /// <param name="value">The binary data to be encoded.</param>
        /// <param name="lineLength">Insert line break for every number of characters specified. If value is 0 or less, no line break will be inserted.</param>
        /// <param name="prefix">Prefix characters that will be prepended to the output.</param>
        /// <param name="suffix">Prefix characters that will be appended to the output.</param>
        public static string ToBase85String(byte[] value, int lineLength, string prefix, string suffix)
        {
            if (prefix == null)
                throw new ArgumentNullException("prefix");
            if (suffix == null)
                throw new ArgumentNullException("suffix");
            if (lineLength < 0)
                throw new ArgumentOutOfRangeException("lineLength", RS.BadBase85LineLength);
                
            return Base85Encode(value, lineLength, true, prefix, suffix);
        }

        private static byte[] Base85Decode(string value, bool enforceMarks, string prefixMark = "<~", string suffixMark = "~>")
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (enforceMarks)
            {
                if (!value.StartsWith(prefixMark) | !value.EndsWith(suffixMark)) 
                    throw new FormatException(string.Format(RS.BadBase85PrefixSuffix, prefixMark, suffixMark));
            }

            // strip prefix and suffix if present
            if (value.StartsWith(prefixMark))
                value = value.Substring(prefixMark.Length);

            if (value.EndsWith(suffixMark))
                value = value.Substring(0, value.Length - suffixMark.Length);

            MemoryStream ms = new MemoryStream();
            int count = 0;
            bool processChar = false;
            byte[] decodedBlock = new byte[4];
            byte[] encodedBlock = new byte[5];
            uint tuple = 0;

            foreach (char c in value)
            {
                switch (c)
                {
                    case 'z':
                        if (count != 0)
                            throw new FormatException(RS.Base85CannotHaveZ);

                        decodedBlock[0] = 0;
                        decodedBlock[1] = 0;
                        decodedBlock[2] = 0;
                        decodedBlock[3] = 0;
                        ms.Write(decodedBlock, 0, decodedBlock.Length);
                        processChar = false;
                        break;

                    case '\n': 
                    case '\r': 
                    case '\t': 
                    case '\0': 
                    case '\f': 
                    case '\b': 
                        processChar = false;
                        break;

                    default:
                        if (c < '!' || c > 'u')
                            throw new FormatException(string.Format(RS.BadBase85Char, c));

                        processChar = true;
                        break;
                }

                if (processChar)
                {
                    tuple += ((uint)(c - AsciiOffset) * s_pow85[count]);
                    count++;
                    if (count == encodedBlock.Length)
                    {
                        for (int i = 0; i < decodedBlock.Length; i++)
                        {
                            decodedBlock[i] = (byte)(tuple >> 24 - (i * 8));   
                        }

                        ms.Write(decodedBlock, 0, decodedBlock.Length);
                        tuple = 0;
                        count = 0;
                    }
                }
            }

            // if we have some bytes left over at the end..
            if (count != 0)
            {
                if (count == 1) 
                    throw new FormatException(RS.BadBase85EndBlock);

                count--;
                tuple += s_pow85[count];

                for (int i = 0; i < count; i++)
                {
                    decodedBlock[i] = (byte)(tuple >> 24 - (i * 8));   
                }

                for (int i = 0; i < count; i++)
                {
                    ms.WriteByte(decodedBlock[i]);
                }
            }

            return ms.ToArray();
        }

        private static string Base85Encode(byte[] value, int lineLength, bool enforceMarks, string prefixMark = "<~", string suffixMark = "~>")
        {
            if (value == null)
                throw new ArgumentNullException("value");

            byte[] decodedBlock = new byte[4];
            byte[] encodedBlock = new byte[5];
            uint tuple = 0;
            int linePos = 0;
            int count = 0;

            // output builder
            StringBuilder sb = new StringBuilder((int)(value.Length * (encodedBlock.Length / decodedBlock.Length)));

            // prefix
            if (enforceMarks) 
            {
                if (lineLength > 0 && (linePos + prefixMark.Length > lineLength))
                {
                    linePos = 0;
                    sb.Append('\n');
                }
                else
                {
                    linePos += prefixMark.Length;
                }
                sb.Append(prefixMark);
            }

            // encoder core
            foreach (byte b in value)
            {
                if (count >= decodedBlock.Length - 1)
                {
                    tuple |= b;
    
                    if (tuple == 0)
                    {
                        sb.Append('z');
                        linePos++;
                        if (lineLength > 0 && (linePos >= lineLength))
                        {
                            linePos = 0;
                            sb.Append('\n');
                        }
                    }
                    else
                    {
                        for (int i = encodedBlock.Length - 1; i >= 0; i--)
                        {
                            encodedBlock[i] = (byte)((tuple % 85) + AsciiOffset);
                            tuple /= 85;
                        }

                        for (int i = 0; i < encodedBlock.Length; i++)
                        {
                            char c = (char)encodedBlock[i];

                            sb.Append(c);
                            linePos++;
                            if (lineLength > 0 && (linePos >= lineLength))
                            {
                                linePos = 0;
                                sb.Append('\n');
                            }
                        }
                    }
    
                    tuple = 0;
                    count = 0;
                }
                else
                {
                    tuple |= (uint)(b << (24 - (count * 8)));
                    count++;
                }
            }

            // if we have some bytes left over at the end...
            if (count > 0)
            {
                for (int i = encodedBlock.Length - 1; i >= 0; i--)
                {
                    encodedBlock[i] = (byte)((tuple % 85) + AsciiOffset);
                    tuple /= 85;
                }

                for (int i = 0; i < (count + 1); i++)
                {
                    char c = (char)encodedBlock[i];

                    sb.Append(c);
                    linePos++;
                    if (lineLength > 0 && (linePos >= lineLength))
                    {
                        linePos = 0;
                        sb.Append('\n');
                    }
                }
            }

            // suffix
            if (enforceMarks) 
            {
                if (lineLength > 0 && (linePos + suffixMark.Length > lineLength))
                {
                    linePos = 0;
                    sb.Append('\n');
                }
                else
                {
                    linePos += suffixMark.Length;
                }
                sb.Append(suffixMark);
            }

            // done
            return sb.ToString();
        }
    }
}
