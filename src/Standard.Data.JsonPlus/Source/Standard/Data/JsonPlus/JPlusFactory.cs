using System;
using System.IO;
using System.Reflection;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// This class contains methods used to retrieve Json+ source text from a variety of sources including 
    /// user-supplied strings, configuration files and assembly resources.
    /// </summary>
    public class JPlusFactory
    {
        /// <summary>
        /// Generates an empty Json+ abstract syntax tree.
        /// </summary>
        public static JPlusContext Empty
        {
            get { return ParseString(string.Empty); }
        }

        /// <summary>
        /// Generates a Json+ abstract syntax tree from the source string specified.
        /// </summary>
        /// <param name="source">A string that contains the Json+ source text.</param>
        /// <param name="includeCallback">Callback function used to resolve includes.</param>
        /// <returns>A <see cref="JPlusContext"/> object obtained by parsing <paramref name="source"/>.</returns>
        public static JPlusContext ParseString(string source, Func<string, JPlusRoot> includeCallback)
        {
            JPlusRoot res = JPlusParser.Parse(source, includeCallback);
            return new JPlusContext(res);
        }

        /// <summary>
        /// Generates a Json+ abstract syntax tree from the source string specified.
        /// </summary>
        /// <param name="source">A string that contains the Json+ source text.</param>
        /// <returns>A <see cref="JPlusContext"/> object obtained by parsing <paramref name="source"/>.</returns>
        public static JPlusContext ParseString(string source)
        {
            // TODO: add default include resolver
            return ParseString(source, null);
        }

        private static Assembly GetAssemblyByType(Type type)
        {
#if NETSTANDARD
            return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif           
        }

        /// <summary>
        /// Retrieves a Json+ source text defined in a resource of the current executing assembly.
        /// </summary>
        /// <param name="resourceName">The name of the resource that contains the source text.</param>
        /// <returns>The <see cref="JPlusContext"/> obtained by parsing the source text defined in the current 
        /// executing assembly.</returns>
        internal static JPlusContext FromResource(string resourceName)
        {
#if NETSTANDARD
            Assembly assembly = typeof(JPlusFactory).GetTypeInfo().Assembly;
#else
            Assembly assembly = Assembly.GetExecutingAssembly();
#endif

            return FromResource(resourceName, assembly);
        }

        /// <summary>
        /// Retrieves a Json+ text defined in a resource of the assembly containing the supplied instance object.
        /// </summary>
        /// <param name="resourceName">The name of the resource that contains the source text.</param>
        /// <param name="instanceInAssembly">An instance object located in the assembly to search.</param>
        /// <returns>The <see cref="JPlusContext"/> obtained by parsing the source text defined in the assembly 
        /// that contains the instanced object.</returns>
        public static JPlusContext FromResource(string resourceName, object instanceInAssembly)
        {
            Type type = instanceInAssembly as Type;
            if (type != null)
                return FromResource(resourceName, GetAssemblyByType(type));

            Assembly assembly = instanceInAssembly as Assembly;
            if (assembly != null)
                return FromResource(resourceName, assembly);

            return FromResource(resourceName, GetAssemblyByType(instanceInAssembly.GetType()));
        }

        /// <summary>
        /// Retrieves a Json+ text defined in a resource of the assembly containing the supplied 
        /// type <typeparamref name="TAssembly"/>.
        /// </summary>
        /// <typeparam name="TAssembly">A type located in the assembly to search.</typeparam>
        /// <param name="resourceName">The name of the resource that contains the source text.</param>
        /// <returns>The <see cref="JPlusContext"/> obtained by parsing the source text defined in the assembly 
        /// that contains the type <typeparamref name="TAssembly"/>.</returns>
        public static JPlusContext FromResource<TAssembly>(string resourceName)
        {
            return FromResource(resourceName, GetAssemblyByType(typeof(TAssembly)));
        }

        /// <summary>
        /// Retrieves a Json+ source text defined in a resource of the supplied assembly.
        /// </summary>
        /// <param name="resourceName">The name of the resource that contains the source text.</param>
        /// <param name="assembly">The assembly that contains the given resource.</param>
        /// <returns>The <see cref="JPlusContext"/> obtained by parsing the source text in the assembly that 
        /// contains the given resource.</returns>
        public static JPlusContext FromResource(string resourceName, Assembly assembly)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return ParseString(result);
                }
            }
        }
    }
}
