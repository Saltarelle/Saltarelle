using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Saltarelle.Configuration {
	public class PluginElement : ConfigurationElement {
		private static ConfigurationProperty propAssembly;
		private static ConfigurationPropertyCollection properties;

		static PluginElement() {
			propAssembly = new ConfigurationProperty("assembly", typeof(string));
			properties   = new ConfigurationPropertyCollection() { propAssembly };
		}
		
		[ConfigurationProperty("assembly")]
		public string Assembly {
			get { return (string)base[propAssembly]; }
		}

		protected override ConfigurationPropertyCollection Properties {
			get { return properties; }
		}
	}
}
