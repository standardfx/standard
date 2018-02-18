using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Standard
{
    /// <summary>
    /// HTTP related extensions
    /// </summary>
    public static class HttpUtility
    {
        private const char KeyValueSeparator = '=';
        private const char KeyValuePairSeparator = '&';

        /// <summary>
        /// Encode a dictionary of key/values into a query string. All keys and values are sent through <see cref="Uri.EscapeUriString"/>.
        /// </summary>
        public static string AsQueryString(IDictionary<string, object> properties)
        {
            if (properties == null)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            foreach (var propertyItem in properties)
            {
                builder.Append(KeyValuePairSeparator);
                builder.Append(Uri.EscapeUriString(propertyItem.Key));
                builder.Append(KeyValueSeparator);
                builder.Append(Uri.EscapeUriString(propertyItem.Value.ToString()));
            }
            builder.Remove(0, 1);

            return builder.ToString();
        }
    }
}
