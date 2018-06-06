using System;
using System.Text;

namespace Standard
{
    public static class StringBuilderExtension
    {
        public static StringBuilder RemoveEnd(this StringBuilder sb, int count)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));

            return sb.Remove(sb.Length - count, count);
        }

        public static StringBuilder RemoveStart(this StringBuilder sb, int count)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));
                
            return sb.Remove(0, count);
        }
    }
}