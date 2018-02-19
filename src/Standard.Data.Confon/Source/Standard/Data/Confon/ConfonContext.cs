using System;
using System.Collections.Generic;
using System.Linq;

namespace Standard.Data.Confon
{
    /// <summary>
    /// This class represents the main configuration object used by a project when configuring objects within the system. To put it simply, it's the internal representation of a Confon string.
    /// </summary>
    public class ConfonContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfonContext"/> class.
        /// </summary>
        public ConfonContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfonContext"/> class.
        /// </summary>
        /// <param name="root">The root node to base this configuration.</param>
        /// <exception cref="ArgumentNullException">"The root value cannot be null."</exception>
        public ConfonContext(ConfonRoot root)
        {
            if (root.Value == null)
                throw new ArgumentNullException("root.Value");

            Root = root.Value;
            Substitutions = root.Substitutions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfonContext"/> class.
        /// </summary>
        /// <param name="source">The configuration to use as the primary source.</param>
        /// <param name="fallback">The configuration to use as a secondary source.</param>
        /// <exception cref="ArgumentNullException">The source configuration cannot be null.</exception>
        public ConfonContext(ConfonContext source, ConfonContext fallback)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Root = source.Root;
            Fallback = fallback;
        }

        /// <summary>
        /// The configuration used as a secondary source.
        /// </summary>
        public ConfonContext Fallback 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Determines if this root node contains any values
        /// </summary>
        public virtual bool IsEmpty
        {
            get { return Root == null || Root.IsEmpty; }
        }

        /// <summary>
        /// The root node of this configuration section
        /// </summary>
        public virtual ConfonValue Root 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// An enumeration of substitutions values
        /// </summary>
        public IEnumerable<ConfonSubstitution> Substitutions 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Generates a deep clone of the current configuration.
        /// </summary>
        /// <returns>A deep clone of the current configuration</returns>
        protected ConfonContext Copy()
        {
            // deep clone
            return new ConfonContext
            {
                Fallback = Fallback != null ? Fallback.Copy() : null,
                Root = Root,
                Substitutions = Substitutions
            };
        }

        public virtual IList<string> GetChildNodeNames()
        {
            return GetChildNodeNames(null, false, false);
        }

        public virtual IList<string> GetChildNodeNames(string path)
        {
            return GetChildNodeNames(path, false, false);
        }

        public virtual IList<string> GetChildNodeNames(string path, bool fullPath)
        {
            return GetChildNodeNames(path, fullPath, false);
        }

        public virtual IList<string> GetChildNodeNames(string path, bool fullPath, bool excludeFallback)
        {
            ConfonValue currentNode = Root;
            if (currentNode == null)
                throw new InvalidOperationException(RS.ErrNullCurrentNode);

            IList<string> result = new List<string>();
            if (string.IsNullOrEmpty(path))
            {
                ConfonObject objectNode = currentNode.GetObject();

                if (objectNode != null)
                    result = objectNode.Items.Keys.ToList();

                if (!excludeFallback && Fallback != null)
                    result = result.Union(Fallback.GetChildNodeNames()).ToList();
            }
            else
            {
                currentNode = GetNode(path);
                ConfonObject objectNode = currentNode.GetObject();
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

        private ConfonValue GetNode(string path)
        {
            string[] elements = new ConfonPath(path).AsArray();
            ConfonValue currentNode = Root;
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
        /// Retrieves a boolean value from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the value to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the value doesn't exist.</param>
        /// <returns>The boolean value defined in the specified path.</returns>
        public virtual bool GetBoolean(string path, bool defaultValue = false)
        {
            ConfonValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetBoolean();
        }

        /// <summary>
        /// Retrieves a long value, optionally suffixed with a 'b', from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the value to retrieve.</param>
        /// <returns>The long value defined in the specified path.</returns>
        public virtual long? GetByteSize(string path)
        {
            ConfonValue value = GetNode(path);
            if (value == null) 
                return null;
                
            return value.GetByteSize();
        }

        /// <summary>
        /// Retrieves an integer value from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the value to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the value doesn't exist.</param>
        /// <returns>The integer value defined in the specified path.</returns>
        public virtual int GetInt32(string path, int defaultValue = 0)
        {
            ConfonValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetInt32();
        }

        /// <summary>
        /// Retrieves a long value from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the value to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the value doesn't exist.</param>
        /// <returns>The long value defined in the specified path.</returns>
        public virtual long GetInt64(string path, long defaultValue = 0)
        {
            ConfonValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetInt64();
        }

        /// <summary>
        /// Retrieves a string value from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the value to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the value doesn't exist.</param>
        /// <returns>The string value defined in the specified path.</returns>
        public virtual string GetString(string path, string defaultValue = null)
        {
            ConfonValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetString();
        }

        /// <summary>
        /// Retrieves a float value from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the value to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the value doesn't exist.</param>
        /// <returns>The float value defined in the specified path.</returns>
        public virtual float GetSingle(string path, float defaultValue = 0)
        {
            ConfonValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetSingle();
        }

        /// <summary>
        /// Retrieves a decimal value from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the value to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the value doesn't exist.</param>
        /// <returns>The decimal value defined in the specified path.</returns>
        public virtual decimal GetDecimal(string path, decimal defaultValue = 0)
        {
            ConfonValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetDecimal();
        }

        /// <summary>
        /// Retrieves a double value from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the value to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the value doesn't exist.</param>
        /// <returns>The double value defined in the specified path.</returns>
        public virtual double GetDouble(string path, double defaultValue = 0)
        {
            ConfonValue value = GetNode(path);
            if (value == null)
                return defaultValue;

            return value.GetDouble();
        }

        /// <summary>
        /// Retrieves a list of boolean values from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the values to retrieve.</param>
        /// <returns>The list of boolean values defined in the specified path.</returns>
        public virtual IList<Boolean> GetBooleanList(string path)
        {
            ConfonValue value = GetNode(path);
            return value.GetBooleanList();
        }

        /// <summary>
        /// Retrieves a list of decimal values from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the values to retrieve.</param>
        /// <returns>The list of decimal values defined in the specified path.</returns>
        public virtual IList<decimal> GetDecimalList(string path)
        {
            ConfonValue value = GetNode(path);
            return value.GetDecimalList();
        }

        /// <summary>
        /// Retrieves a list of float values from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the values to retrieve.</param>
        /// <returns>The list of float values defined in the specified path.</returns>
        public virtual IList<float> GetSingleList(string path)
        {
            ConfonValue value = GetNode(path);
            return value.GetSingleList();
        }

        /// <summary>
        /// Retrieves a list of double values from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the values to retrieve.</param>
        /// <returns>The list of double values defined in the specified path.</returns>
        public virtual IList<double> GetDoubleList(string path)
        {
            ConfonValue value = GetNode(path);
            return value.GetDoubleList();
        }

        /// <summary>
        /// Retrieves a list of int values from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the values to retrieve.</param>
        /// <returns>The list of int values defined in the specified path.</returns>
        public virtual IList<int> GetInt32List(string path)
        {
            ConfonValue value = GetNode(path);
            return value.GetInt32List();
        }

        /// <summary>
        /// Retrieves a list of long values from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the values to retrieve.</param>
        /// <returns>The list of long values defined in the specified path.</returns>
        public virtual IList<long> GetInt64List(string path)
        {
            ConfonValue value = GetNode(path);
            return value.GetInt64List();
        }

        /// <summary>
        /// Retrieves a list of byte values from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the values to retrieve.</param>
        /// <returns>The list of byte values defined in the specified path.</returns>
        public virtual IList<byte> GetByteList(string path)
        {
            ConfonValue value = GetNode(path);
            return value.GetByteList();
        }

        /// <summary>
        /// Retrieves a list of string values from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the values to retrieve.</param>
        /// <returns>The list of string values defined in the specified path.</returns>
        public virtual IList<string> GetStringList(string path)
        {
            ConfonValue value = GetNode(path);
            if (value == null) 
                return new string[0];
            return value.GetStringList();
        }

        /// <summary>
        /// Retrieves a new configuration from the current configuration
        /// with the root node being the supplied path.
        /// </summary>
        /// <param name="path">The path that contains the configuration to retrieve.</param>
        /// <returns>A new configuration with the root node being the supplied path.</returns>
        public virtual ConfonContext GetContext(string path)
        {
            ConfonValue value = GetNode(path);
            if (Fallback != null)
            {
                ConfonContext f = Fallback.GetContext(path);
                if (value == null && f == null)
                    return null;
                if (value == null)
                    return f;

                return new ConfonContext(new ConfonRoot(value)).WithFallback(f);
            }

            if (value == null)
                return null;

            return new ConfonContext(new ConfonRoot(value));
        }

        /// <summary>
        /// Retrieves a <see cref="ConfonValue"/> from a specific path.
        /// </summary>
        /// <param name="path">The path that contains the value to retrieve.</param>
        /// <returns>The <see cref="ConfonValue"/> found at the location if one exists, otherwise <c>null</c>.</returns>
        public ConfonValue GetValue(string path)
        {
            ConfonValue value = GetNode(path);
            return value;
        }

        /// <summary>
        /// Retrieves a <see cref="TimeSpan"/> value from the specified path in the configuration.
        /// </summary>
        /// <param name="path">The path that contains the value to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the value doesn't exist.</param>
        /// <param name="allowInfinite"><c>true</c> if infinite timespans are allowed; otherwise <c>false</c>.</param>
        /// <returns>The <see cref="TimeSpan"/> value defined in the specified path.</returns>
        public virtual TimeSpan GetTimeSpan(string path, TimeSpan? defaultValue = null, bool allowInfinite = true)
        {
            ConfonValue value = GetNode(path);
            if (value == null)
                return defaultValue.GetValueOrDefault();

            return value.GetTimeSpan(allowInfinite);
        }

        /// <summary>
        /// Converts the current configuration to a string.
        /// </summary>
        /// <returns>A string containing the current configuration.</returns>
        public override string ToString()
        {
            if (Root == null)
                return string.Empty;

            return Root.ToString();
        }

        /// <summary>
        /// Configure the current configuration with a secondary source.
        /// </summary>
        /// <param name="fallback">The configuration to use as a secondary source.</param>
        /// <returns>The current configuration configured with the specified fallback.</returns>
        /// <exception cref="ArgumentException">Config can not have itself as fallback.</exception>
        public virtual ConfonContext WithFallback(ConfonContext fallback)
        {
            if (fallback == this)
                throw new ArgumentException(RS.ErrSelfReferencingFallback, "fallback");

            ConfonContext clone = Copy();

            ConfonContext current = clone;
            while (current.Fallback != null)
            {
                current = current.Fallback;
            }
            current.Fallback = fallback;

            return clone;
        }


        /// <summary>
        /// Determine if a JAPSON configuration element exists at the specified location
        /// </summary>
        /// <param name="path">The location to check for a configuration value.</param>
        /// <returns><c>true</c> if a value was found, <c>false</c> otherwise.</returns>
        public virtual bool HasPath(string path)
        {
            ConfonValue value = GetNode(path);
            return value != null;
        }

        /// <summary>
        /// Adds the supplied configuration string as a fallback to the supplied configuration.
        /// </summary>
        /// <param name="config">The configuration used as the source.</param>
        /// <param name="fallback">The string used as the fallback configuration.</param>
        /// <returns>The supplied configuration configured with the supplied fallback.</returns>
        public static ConfonContext operator +(ConfonContext config, string fallback)
        {
            ConfonContext fallbackConfig = ConfonFactory.ParseString(fallback);
            return config.WithFallback(fallbackConfig);
        }

        /// <summary>
        /// Adds the supplied configuration as a fallback to the supplied configuration string.
        /// </summary>
        /// <param name="confon">The configuration string used as the source.</param>
        /// <param name="fallbackConfig">The configuration used as the fallback.</param>
        /// <returns>A configuration configured with the supplied fallback.</returns>
        public static ConfonContext operator +(string confon, ConfonContext fallbackConfig)
        {
            ConfonContext config = ConfonFactory.ParseString(confon);
            return config.WithFallback(fallbackConfig);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="ConfonContext"/>.
        /// </summary>
        /// <param name="confon">The string that contains a configuration.</param>
        /// <returns>A configuration based on the supplied string.</returns>
        public static implicit operator ConfonContext(string confon)
        {
            ConfonContext config = ConfonFactory.ParseString(confon);
            return config;
        }

        /// <summary>
        /// Retrieves an enumerable key value pair representation of the current configuration.
        /// </summary>
        /// <returns>The current configuration represented as an enumerable key value pair.</returns>
        public virtual IEnumerable<KeyValuePair<string, ConfonValue>> AsEnumerable()
        {
            HashSet<string> used = new HashSet<string>();
            ConfonContext current = this;
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
    /// This class contains convenience methods for working with <see cref="ConfonContext"/>.
    /// </summary>
    public static class ConfonContextExtensions
    {
        /// <summary>
        /// Retrieves the current configuration or the fallback
        /// configuration if the current one is null.
        /// </summary>
        /// <param name="config">The configuration used as the source.</param>
        /// <param name="fallback">The configuration to use as a secondary source.</param>
        /// <returns>The current configuration or the fallback configuration if the current one is null.</returns>
        public static ConfonContext SafeWithFallback(this ConfonContext config, ConfonContext fallback)
        {
            return config == null
                ? fallback
                : ReferenceEquals(config, fallback)
                    ? config
                    : config.WithFallback(fallback);
        }

        /// <summary>
        /// Determines if the supplied configuration has any usable content period.
        /// </summary>
        /// <param name="config">The configuration used as the source.</param>
        /// <returns><c>true></c> if the <see cref="ConfonContext" /> is null or <see cref="ConfonContext.IsEmpty" />; otherwise <c>false</c>.</returns>
        public static bool IsNullOrEmpty(this ConfonContext config)
        {
            return config == null || config.IsEmpty;
        }
    }
}
