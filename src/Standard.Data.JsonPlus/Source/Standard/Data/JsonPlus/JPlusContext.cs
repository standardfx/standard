using System;
using System.Collections.Generic;
using System.Linq;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// The abstract syntax tree of a Json+ source text. Use this class to transverse a Json+ tree.
    /// </summary>
    public class JPlusContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusContext"/> class.
        /// </summary>
        public JPlusContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusContext"/> class.
        /// </summary>
        /// <param name="root">The root node.</param>
        /// <exception cref="ArgumentNullException">The root value cannot be `null`.</exception>
        public JPlusContext(JPlusRoot root)
        {
            if (root.Value == null)
                throw new ArgumentNullException("root.Value");

            Root = root.Value;
            Substitutions = root.Substitutions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusContext"/> class.
        /// </summary>
        /// <param name="source">The context to use as the primary source.</param>
        /// <param name="fallback">The context to use as a secondary source.</param>
        /// <exception cref="ArgumentNullException">The source configuration cannot be null.</exception>
        public JPlusContext(JPlusContext source, JPlusContext fallback)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Root = source.Root;
            Fallback = fallback;
        }

        /// <summary>
        /// Gets the context used as a secondary source.
        /// </summary>
        public JPlusContext Fallback 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Determines whether the root element contains any value.
        /// </summary>
        public virtual bool IsEmpty
        {
            get { return Root == null || Root.IsEmpty; }
        }

        /// <summary>
        /// Returns the root element.
        /// </summary>
        public virtual JPlusValue Root 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Gets or sets the enumeration of substitutions.
        /// </summary>
        public IEnumerable<JPlusSubstitution> Substitutions 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Create a deep clone of the current context.
        /// </summary>
        /// <returns>A deep clone of the current context.</returns>
        protected JPlusContext Copy()
        {
            // deep clone
            return new JPlusContext
            {
                Fallback = Fallback != null ? Fallback.Copy() : null,
                Root = Root,
                Substitutions = Substitutions
            };
        }

        /// <summary>
        /// Returns the name of all properties of the root context.
        /// </summary>
        public virtual IList<string> GetChildNodeNames()
        {
            return GetChildNodeNames(null, false, false);
        }

        /// <summary>
        /// Returns the name of all properties of a <see cref="JPlusObject"/>.
        /// </summary>
        /// <param name="path">The path to the <see cref="JPlusObject"/>.</param>
        public virtual IList<string> GetChildNodeNames(string path)
        {
            return GetChildNodeNames(path, false, false);
        }

        /// <summary>
        /// Returns the name of all properties of a <see cref="JPlusObject"/>.
        /// </summary>
        /// <param name="path">The path to the <see cref="JPlusObject"/>.</param>
        /// <param name="fullPath">Return the full path of all properties instead of the name only.</param>
        /// <returns>The names of all properties.</returns>
        public virtual IList<string> GetChildNodeNames(string path, bool fullPath)
        {
            return GetChildNodeNames(path, fullPath, false);
        }

        /// <summary>
        /// Returns the name of all properties of a <see cref="JPlusObject"/>.
        /// </summary>
        /// <param name="path">The path to the <see cref="JPlusObject"/>.</param>
        /// <param name="fullPath">Return the full path of all properties instead of the name only.</param>
        /// <param name="excludeFallback">Exclude properties in the fallback context.</param>
        /// <returns>The names of all properties.</returns>
        public virtual IList<string> GetChildNodeNames(string path, bool fullPath, bool excludeFallback)
        {
            JPlusValue currentNode = Root;
            if (currentNode == null)
                throw new InvalidOperationException(RS.ErrNullCurrentNode);

            IList<string> result = new List<string>();
            if (string.IsNullOrEmpty(path))
            {
                JPlusObject objectNode = currentNode.GetObject();

                if (objectNode != null)
                    result = objectNode.Items.Keys.ToList();

                if (!excludeFallback && Fallback != null)
                    result = result.Union(Fallback.GetChildNodeNames()).ToList();
            }
            else
            {
                currentNode = GetNode(path);
                JPlusObject objectNode = currentNode.GetObject();
                if (objectNode == null)
                {
                    if (Fallback != null)
                        result = Fallback.GetChildNodeNames(path);
                }
                else
                {
                    result = objectNode.Items.Keys.ToList();

                    if (!excludeFallback && Fallback != null)
                        result = result.Union(Fallback.GetChildNodeNames(path)).ToList();
                }
            }

            if (result == null || result.Count == 0)
                return null;

            if (!fullPath)
            {
                return result;
            }
            else
            {
                List<string> outputResult = new List<string>(result.Count);
                foreach (string member in result)
                {
                    if (member.Contains("."))
                    {
                        string memberEscaped = "'" + member.Replace("'", "\\'") + "'";
                        outputResult.Add(path + "." + memberEscaped);
                    }
                    else
                    {
                        outputResult.Add(path + "." + member);
                    }
                }

                return outputResult;
            }
        }

        private JPlusValue GetNode(string path)
        {
            string[] elements = new JPlusPath(path).AsArray();
            JPlusValue currentNode = Root;
            if (currentNode == null)
                throw new InvalidOperationException(RS.ErrNullCurrentNode);

            foreach (string key in elements)
            {
                currentNode = currentNode.GetChildObject(key);
                if (currentNode == null)
                {
                    if (Fallback != null)
                        return Fallback.GetNode(path);

                    return null;
                }
            }
            return currentNode;
        }

        /// <summary>
        /// Return an element as a <see cref="bool"/>.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <param name="defaultValue">The default value to return if the element specified by <paramref name="path"/> does not exist.</param>
        /// <returns>The <see cref="bool"/> value of the element specified by <paramref name="path"/>, or <paramref name="defaultValue"/> if the element does not exist.</returns>
        public virtual bool GetBoolean(string path, bool defaultValue = false)
        {
            JPlusValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetBoolean();
        }

        /// <summary>
        /// Returns an element as a <see cref="Nullable{Int64}"/> object by parsing the value as a number with data size unit.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <returns>The <see cref="Nullable{Int64}"/> value of the element specified by <paramref name="path"/>.</returns>
        /// <see cref="JPlusValue.GetByteSize()"/>
        public virtual long? GetByteSize(string path)
        {
            JPlusValue value = GetNode(path);
            if (value == null) 
                return null;
                
            return value.GetByteSize();
        }

        /// <summary>
        /// Returns an element as an <see cref="int"/>.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <param name="defaultValue">The default value to return if the element specified by <paramref name="path"/> does not exist. Defaults to zero.</param>
        /// <returns>The <see cref="Int32"/> value of the element specified by <paramref name="path"/>, or <paramref name="defaultValue"/> if the element does not exist.</returns>
        public virtual int GetInt32(string path, int defaultValue = 0)
        {
            JPlusValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetInt32();
        }

        /// <summary>
        /// Returns an element as a <see cref="long"/>.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <param name="defaultValue">The default value to return if the element specified by <paramref name="path"/> does not exist. Defaults to zero.</param>
        /// <returns>The <see cref="Int64"/> value of the element specified by <paramref name="path"/>, or <paramref name="defaultValue"/> if the element does not exist.</returns>
        public virtual long GetInt64(string path, long defaultValue = 0)
        {
            JPlusValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetInt64();
        }

        /// <summary>
        /// Returns an element as a <see cref="string"/>.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <param name="defaultValue">The default value to return if the element specified by <paramref name="path"/> does not exist. Defaults to `null`.</param>
        /// <returns>The <see cref="String"/> value of the element specified by <paramref name="path"/>, or <paramref name="defaultValue"/> if the element does not exist.</returns>
        public virtual string GetString(string path, string defaultValue = null)
        {
            JPlusValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetString();
        }

        /// <summary>
        /// Returns an element as a <see cref="float"/>.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <param name="defaultValue">The default value to return if the element specified by <paramref name="path"/> does not exist. Defaults to zero.</param>
        /// <returns>The <see cref="Single"/> value of the element specified by <paramref name="path"/>, or <paramref name="defaultValue"/> if the element does not exist.</returns>
        public virtual float GetSingle(string path, float defaultValue = 0)
        {
            JPlusValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetSingle();
        }

        /// <summary>
        /// Returns an element as a <see cref="decimal"/>.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <param name="defaultValue">The default value to return if the element specified by <paramref name="path"/> does not exist. Defaults to zero.</param>
        /// <returns>The <see cref="Decimal"/> value of the element specified by <paramref name="path"/>, or <paramref name="defaultValue"/> if the element does not exist.</returns>
        public virtual decimal GetDecimal(string path, decimal defaultValue = 0)
        {
            JPlusValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetDecimal();
        }

        /// <summary>
        /// Returns an element as a <see cref="double"/>.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <param name="defaultValue">The default value to return if the element specified by <paramref name="path"/> does not exist. Defaults to zero.</param>
        /// <returns>The <see cref="Double"/> value of the element specified by <paramref name="path"/>, or <paramref name="defaultValue"/> if the element does not exist.</returns>
        public virtual double GetDouble(string path, double defaultValue = 0)
        {
            JPlusValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetDouble();
        }

        /// <summary>
        /// Returns an element as an enumerable collection <see cref="bool"/> objects.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <returns>The <see cref="IList{Boolean}"/> value of the element specified by <paramref name="path"/>.</returns>
        public virtual IList<Boolean> GetBooleanList(string path)
        {
            JPlusValue value = GetNode(path);
            return value.GetBooleanList();
        }

        /// <summary>
        /// Returns an element as an enumerable collection <see cref="decimal"/> objects.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <returns>The <see cref="IList{Decimal}"/> value of the element specified by <paramref name="path"/>.</returns>
        public virtual IList<decimal> GetDecimalList(string path)
        {
            JPlusValue value = GetNode(path);
            return value.GetDecimalList();
        }

        /// <summary>
        /// Returns an element as an enumerable collection <see cref="single"/> objects.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <returns>The <see cref="IList{Single}"/> value of the element specified by <paramref name="path"/>.</returns>
        public virtual IList<float> GetSingleList(string path)
        {
            JPlusValue value = GetNode(path);
            return value.GetSingleList();
        }

        /// <summary>
        /// Returns an element as an enumerable collection <see cref="double"/> objects.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <returns>The <see cref="IList{Double}"/> value of the element specified by <paramref name="path"/>.</returns>
        public virtual IList<double> GetDoubleList(string path)
        {
            JPlusValue value = GetNode(path);
            return value.GetDoubleList();
        }

        /// <summary>
        /// Returns an element as an enumerable collection <see cref="int"/> objects.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <returns>The <see cref="IList{Int32}"/> value of the element specified by <paramref name="path"/>.</returns>
        public virtual IList<int> GetInt32List(string path)
        {
            JPlusValue value = GetNode(path);
            return value.GetInt32List();
        }

        /// <summary>
        /// Returns an element as an enumerable collection <see cref="long"/> objects.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <returns>The <see cref="IList{Int64}"/> value of the element specified by <paramref name="path"/>.</returns>
        public virtual IList<long> GetInt64List(string path)
        {
            JPlusValue value = GetNode(path);
            return value.GetInt64List();
        }

        /// <summary>
        /// Returns an element as an enumerable collection <see cref="byte"/> objects.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <returns>The <see cref="IList{Byte}"/> value of the element specified by <paramref name="path"/>.</returns>
        public virtual IList<byte> GetByteList(string path)
        {
            JPlusValue value = GetNode(path);
            return value.GetByteList();
        }

        /// <summary>
        /// Returns an element as an enumerable collection <see cref="string"/> objects.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <returns>The <see cref="IList{String}"/> value of the element specified by <paramref name="path"/>.</returns>
        public virtual IList<string> GetStringList(string path)
        {
            JPlusValue value = GetNode(path);
            if (value == null) 
                return new string[0];
            return value.GetStringList();
        }

        /// <summary>
        /// Returns a new context, with the element at the Json+ query path specified as the root element.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <returns>A new context, with the element at the path specified by <paramref name="path"/> as the root element.</returns>
        public virtual JPlusContext GetContext(string path)
        {
            JPlusValue value = GetNode(path);
            if (Fallback != null)
            {
                JPlusContext f = Fallback.GetContext(path);
                if (value == null && f == null)
                    return null;
                if (value == null)
                    return f;

                return new JPlusContext(new JPlusRoot(value)).WithFallback(f);
            }

            if (value == null)
                return null;

            return new JPlusContext(new JPlusRoot(value));
        }

        /// <summary>
        /// Returns an element as a generic <see cref="JPlusValue"/> object.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <returns>The <see cref="JPlusValue"/> value of the element specified by <paramref name="path"/>, or `null` if the element does not exist.</returns>
        public JPlusValue GetValue(string path)
        {
            JPlusValue value = GetNode(path);
            return value;
        }

        /// <summary>
        /// Returns an element as a <see cref="TimeSpan"/> object.
        /// </summary>
        /// <param name="path">A Json+ query path that identifies an element in the context tree.</param>
        /// <param name="defaultValue">The default value to return if the element specified by <paramref name="path"/> does not exist. Defaults to `null`.</param>
        /// <param name="allowInfinite">Set to `true` to allow the keyword `infinite`, which will return <see cref="Timeout.InfiniteTimeSpan"/>. Otherwise, `false`. Defaults to `true`.</param>
        /// <returns>The <see cref="TimeSpan"/> value of the element specified by <paramref name="path"/>, or <paramref name="defaultValue"/> if the element does not exist.</returns>
        public virtual TimeSpan GetTimeSpan(string path, TimeSpan? defaultValue = null, bool allowInfinite = true)
        {
            JPlusValue value = GetNode(path);
            if (value == null)
                return defaultValue.GetValueOrDefault();

            return value.GetTimeSpan(allowInfinite);
        }

        /// <summary>
        /// Returns a string representation of the current context.
        /// </summary>
        /// <returns>A string representation of the current context.</returns>
        public override string ToString()
        {
            if (Root == null)
                return string.Empty;

            return Root.ToString();
        }

        /// <summary>
        /// Create a new <see cref="JPlusContext"/> that is equal to the current context, plus the specified source as a fallback.
        /// </summary>
        /// <param name="fallback">The context to use as a secondary source.</param>
        /// <exception cref="ArgumentException">A context cannot use itself as the fallback context.</exception>
        /// <returns>A <see cref="JPlusContext"/> instance that is equal to the current context, plus <paramref name="fallback"/> as the fallback context.</returns>
        public virtual JPlusContext WithFallback(JPlusContext fallback)
        {
            if (fallback == this)
                throw new ArgumentException(RS.ErrSelfReferencingFallback, nameof(fallback));

            JPlusContext clone = Copy();

            JPlusContext current = clone;
            while (current.Fallback != null)
            {
                current = current.Fallback;
            }
            current.Fallback = fallback;

            return clone;
        }


        /// <summary>
        /// Determines if an element exists at the specified query path.
        /// </summary>
        /// <param name="path">The Json+ query path.</param>
        /// <returns>`true` if an element exists at the <paramref name="path"/> specified. Otherwise, `false`.</returns>
        public virtual bool HasPath(string path)
        {
            JPlusValue value = GetNode(path);
            return value != null;
        }

        /// <summary>
        /// Adds the supplied Json+ source string as a fallback to the primary Json+ source string.
        /// </summary>
        /// <param name="source">The source context.</param>
        /// <param name="fallback">The Json+ source to use as the fallback context.</param>
        /// <returns>The effective context.</returns>
        public static JPlusContext operator +(JPlusContext source, string fallback)
        {
            JPlusContext fallbackContext = JPlusFactory.ParseString(fallback);
            return source.WithFallback(fallbackContext);
        }

        /// <summary>
        /// Adds the supplied context as a fallback to the supplied Json+ source.
        /// </summary>
        /// <param name="source">The Json+ source.</param>
        /// <param name="fallback">The context used as the fallback.</param>
        /// <returns>The effective context.</returns>
        public static JPlusContext operator +(string source, JPlusContext fallback)
        {
            JPlusContext context = JPlusFactory.ParseString(source);
            return context.WithFallback(fallback);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="string"/> to <see cref="JPlusContext"/>.
        /// </summary>
        /// <param name="source">The Json+ source.</param>
        /// <returns>A context based on the supplied string.</returns>
        public static implicit operator JPlusContext(string source)
        {
            JPlusContext context = JPlusFactory.ParseString(source);
            return context;
        }

        /// <summary>
        /// Returns an enumerable key-value pair representation of the current context.
        /// </summary>
        /// <returns>The current context represented as an enumerable collection of key-value pairs.</returns>
        public virtual IEnumerable<KeyValuePair<string, JPlusValue>> AsEnumerable()
        {
            HashSet<string> used = new HashSet<string>();
            JPlusContext current = this;
            while (current != null)
            {
                foreach (var kvp in current.Root.GetObject().Items)
                {
                    if (!used.Contains(kvp.Key))
                    {
                        yield return kvp;
                        used.Add(kvp.Key);
                    }
                }
                current = current.Fallback;
            }
        }
    }

    /// <summary>
    /// This class contains convenience methods for working with <see cref="JPlusContext"/>.
    /// </summary>
    public static class JPlusContextExtensions
    {
        /// <summary>
        /// Retrieves the current context, or the fallback context if the current context is `null`.
        /// </summary>
        /// <param name="context">The context used as the source.</param>
        /// <param name="fallback">The context to use as a secondary source.</param>
        /// <returns>The current <paramref name="context"/>, or <paramref name="fallback"/> if the current context is `null`.</returns>
        public static JPlusContext SafeWithFallback(this JPlusContext context, JPlusContext fallback)
        {
            return context == null
                ? fallback
                : ReferenceEquals(context, fallback)
                    ? context
                    : context.WithFallback(fallback);
        }

        /// <summary>
        /// Determines if the supplied context has any usable content.
        /// </summary>
        /// <param name="context">The context used as the source.</param>
        /// <returns>`true` if <paramref name="context"/> is null or empty. Otherwise, `false`.</returns>
        public static bool IsNullOrEmpty(this JPlusContext context)
        {
            return context == null || context.IsEmpty;
        }
    }
}
