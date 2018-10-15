using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// This class represents an object element in a Confon string.
    /// </summary>
    /// <remarks>
    /// <code>
    /// foo {  
    ///   child {
    ///     grandchild {  
    ///       receive = on 
    ///       autoreceive = on
    ///       lifecycle = on
    ///       event-stream = on
    ///       unhandled = on
    ///     }
    ///   }
    /// }
    /// </code>
    /// </remarks>
    public class JPlusObject : IJPlusElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusObject"/> class.
        /// </summary>
        public JPlusObject()
        {
            Items = new Dictionary<string, JPlusValue>();
        }

        /// <summary>
        /// Retrieves the underlying map that contains the barebones object values.
        /// </summary>
        public IDictionary<string, object> Unwrapped
        {
            get
            {
                return Items.ToDictionary(k => k.Key, v =>
                    {
                        JPlusObject obj = v.Value.GetObject();

                        if (obj != null)
                            return (object)obj.Unwrapped;

                        return v.Value;
                    });
            }
        }

        /// <summary>
        /// Retrieves the underlying map that this element is based on.
        /// </summary>
        public Dictionary<string, JPlusValue> Items 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Determines whether this element is a string.
        /// </summary>
        /// <returns>This method always return `false`.</returns>
        public bool IsString()
        {
            return false;
        }

        /// <summary>
        /// Retrieves the string representation of this element.
        /// </summary>
        /// <exception cref="NotImplementedException">This element is an object and not a string. Therefore, calling this method will always result in an exception.</exception>
        /// <returns>Calling this method will result in an <see cref="NotImplementedException"/>.</returns>
        public string GetString()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether this element is an array.
        /// </summary>
        /// <returns>This method always return `false`.</returns>
        public bool IsArray()
        {
            return false;
        }

        /// <summary>
        /// Returns this element as an enumerable list of <see cref="JPlusValue"/>.
        /// </summary>
        /// <exception cref="NotImplementedException">This element is an object and not an array. Therefore, calling this method will always result in an exception.</exception>
        /// <returns>Calling this method will result in an <see cref="NotImplementedException"/>.</returns>
        public IList<JPlusValue> GetArray()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the value of the property with the specified name.
        /// </summary>
        /// <param name="key">The name of the property.</param>
        /// <returns>The value associated with the specified property name <paramref name="key"/>, or `null` if the property name does not exist.</returns>
        public JPlusValue GetKey(string key)
        {
            if (Items.ContainsKey(key))
                return Items[key];

            return null;
        }

        /// <summary>
        /// Returns the value of the property with the specified name. If the property does not exist, the property is created with a blank value.
        /// </summary>
        /// <param name="key">The name of the property.</param>
        /// <returns>The value associated with the specified property name <paramref name="key"/>, or an empty <see cref="JPlusValue"/> if the property name does not exist.</returns>
        public JPlusValue GetOrCreateKey(string key)
        {
            if (Items.ContainsKey(key))
                return Items[key];

            JPlusValue child = new JPlusValue();
            Items.Add(key, child);
            return child;
        }

        /// <summary>
        /// Returns a string representation of this element.
        /// </summary>
        /// <returns>A string representation of this element.</returns>
        public override string ToString()
        {
            return ToString(0);
        }

        /// <summary>
        /// Returns a string representation of this element.
        /// </summary>
        /// <param name="indent">The number of spaces to indent the string.</param>
        /// <returns>A string representation of this element.</returns>
        public string ToString(int indent)
        {
            string i = new string(' ', indent * 2);
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in Items)
            {
                string key = QuoteIfNeeded(kvp.Key);
                sb.AppendFormat("{0}{1} : {2}\r\n", i, key, kvp.Value.ToString(indent));
            }

            return sb.ToString();
        }

        private string QuoteIfNeeded(string text)
        {
            if (text.ToCharArray().Intersect(" \t".ToCharArray()).Any())
                return "\"" + text + "\"";

            return text;
        }

        /// <summary>
        /// Merge the properties of a <see cref="JPlusObject"/> into this element.
        /// </summary>
        /// <param name="other">The <see cref="JPlusObject"/> to merge with this element.</param>
        public void Merge(JPlusObject other)
        {
            Dictionary<string, JPlusValue> thisItems = Items;
            Dictionary<string, JPlusValue> otherItems = other.Items;

            foreach (var otherItem in otherItems)
            {
                if (thisItems.ContainsKey(otherItem.Key))
                {
                    // other key was present in this object.
                    // if we have a value, just ignore the other value, unless it is an object
                    JPlusValue thisItem = thisItems[otherItem.Key];

                    //if both values are objects, merge them
                    if (thisItem.IsObject() && otherItem.Value.IsObject())
                        thisItem.GetObject().Merge(otherItem.Value.GetObject());
                }
                else
                {
                    //other key was not present in this object, just copy it over
                    Items.Add(otherItem.Key,otherItem.Value);
                }
            }            
        }
    }
}
