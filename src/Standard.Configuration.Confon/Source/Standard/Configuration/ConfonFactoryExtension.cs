using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Standard.Data.Confon;

namespace Standard.Configuration
{
    /// <summary>
    /// This class contains methods used to retrieve configuration information from a variety of sources including user-supplied strings, configuration files and assembly resources.
    /// </summary>
    public static class ConfonFactoryExtension
    {
        /// <summary>
        /// Loads a configuration defined in the current application's configuration file, e.g. app.config or web.config
        /// </summary>
        /// <returns>The configuration defined in the configuration file.</returns>
        public static ConfonContext FromAppConfiguration(this ConfonContext context)
        {
           ConfonConfigurationSection section = (ConfonConfigurationSection)ConfigurationManager.GetSection("confon") ?? new ConfonConfigurationSection();
           ConfonContext config = section.Config;
   
           return config;
        }
    }
}
