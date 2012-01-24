using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Saltarelle.Configuration {
	public class SaltarelleConfigSection : ConfigurationSection {
		private static ConfigurationProperty propPlugins;
		private static ConfigurationProperty propRoutes;
		private static ConfigurationProperty propScripts;
		private static ConfigurationPropertyCollection properties;

		static SaltarelleConfigSection() {
			propPlugins = new ConfigurationProperty("plugins", typeof(PluginElementCollection));
			propRoutes  = new ConfigurationProperty("routes", typeof(RoutesElement));
			propScripts = new ConfigurationProperty("scripts", typeof(ScriptElementCollection));
			properties  = new ConfigurationPropertyCollection() { propPlugins, propRoutes, propScripts };
		}
		
		[ConfigurationProperty("plugins")]
		public PluginElementCollection Plugins {
			get { return (PluginElementCollection)base[propPlugins]; }
		}

		[ConfigurationProperty("routes")]
		public RoutesElement Routes {
			get { return (RoutesElement)base[propRoutes]; }
		}
		
		[ConfigurationProperty("scripts")]
		public ScriptElementCollection Scripts {
			get { return (ScriptElementCollection)base[propScripts];  }
		}

		protected override ConfigurationPropertyCollection Properties {
			get { return properties; }
		}
	}
	
}
