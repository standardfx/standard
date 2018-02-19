using System.Configuration;

namespace Standard.Configuration
{
    /// <summary>
    /// This class represents a custom node within a configuration file.
    /// <code>
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8" ?>
    /// <configuration>
    ///   <configSections>
    ///     <section name="foo" type="Standard.Configuration.ConfonConfigurationSection, Standard.Configuration.Confon" />
    ///   </configSections>
    ///   <foo>
    ///     <confon>
    ///     ...
    ///     </confon>
    ///   </foo>
    /// </configuration>
    /// ]]>
    /// </code>
    /// </summary>
    public class ConfonConfigurationElement : CDataConfigurationElement
    {
        /// <summary>
        /// Gets or sets the configuration string contained in the confon node.
        /// </summary>
        [ConfigurationProperty(ContentPropertyName, IsRequired = true, IsKey = true)]
        public string Content
        {
            get 
            { 
                return (string)base[ContentPropertyName]; 
            }
            set 
            { 
                base[ContentPropertyName] = value; 
            }
        }
    }
}

