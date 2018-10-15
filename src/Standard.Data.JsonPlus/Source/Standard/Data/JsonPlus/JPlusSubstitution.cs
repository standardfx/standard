using System.Collections.Generic;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// This class represents a substitution element.
    /// </summary>
    /// <remarks>
    /// <code>
    /// foo {  
    ///   defaultInstances = 10
    ///   deployment{
    ///     /user/time{
    ///       nr-of-instances = $defaultInstances
    ///     }
    ///   }
    /// }
    /// </code>
    /// </remarks>
    public class JPlusSubstitution : IJPlusElement, IJPlusObjectCandidate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusSubstitution"/> class.
        /// </summary>
        protected JPlusSubstitution()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusSubstitution" /> class.
        /// </summary>
        /// <param name="path">The path to the target element.</param>
        public JPlusSubstitution(string path)
        {
            Path = path;
        }

        /// <summary>
        /// The full path to the value which should substitute this instance.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The evaluated value from the <see cref="Path"/> property.
        /// </summary>
        public JPlusValue ResolvedValue { get; set; }

        /// <see cref="IJPlusElement.IsString()"/>
        public bool IsString()
        {
            return ResolvedValue.IsString();
        }

        /// <see cref="IJPlusElement.GetString()"/>
        public string GetString()
        {
            return ResolvedValue.GetString();
        }

        /// <see cref="IJPlusElement.IsArray()"/>
        public bool IsArray()
        {
            return ResolvedValue.IsArray();
        }

        /// <see cref="IJPlusElement.GetArray()"/>
        public IList<JPlusValue> GetArray()
        {
            return ResolvedValue.GetArray();
        }

        /// <see cref="IJPlusObjectCandidate.IsObject()"/>
        public bool IsObject()
        {
            return ResolvedValue != null && ResolvedValue.IsObject();
        }

        /// <see cref="IJPlusObjectCandidate.GetObject()"/>
        public JPlusObject GetObject()
        {
            return ResolvedValue.GetObject();
        }
    }
}

