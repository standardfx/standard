using System.Configuration;
using Standard.Data.Confon;

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
   ///   ...
   ///   </foo>
   /// </configuration>
   /// ]]>
   /// </code>
   /// </summary>
   public class ConfonConfigurationSection : ConfigurationSection
   {
      private const string ConfigurationPropertyName = "confon";
      private ConfonContext _config;

      /// <summary>
      /// Retrieves a <see cref="Config"/> from the contents of the custom node within a configuration file.
      /// </summary>
      public ConfonContext Config
      {
         get 
         { 
            return _config ?? (_config = ConfonFactory.ParseString(Confon.Content)); 
         }
      }

      /// <summary>
      /// Retrieves the configuration string from the custom node.
      /// </summary>
      [ConfigurationProperty(ConfigurationPropertyName, IsRequired = true)]
      public ConfonConfigurationElement Confon
      {
         get 
         { 
            return (ConfonConfigurationElement)base[ConfigurationPropertyName]; 
         }
         set 
         { 
            base[ConfigurationPropertyName] = value; 
         }
      }
   }
}