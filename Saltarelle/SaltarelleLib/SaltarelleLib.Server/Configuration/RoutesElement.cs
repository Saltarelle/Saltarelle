using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Saltarelle.Configuration {
	public class RoutesElement : ConfigurationElement {
		private static ConfigurationProperty propAssemblyScripts;
		private static ConfigurationProperty propAssemblyCss;
		private static ConfigurationProperty propAssemblyResources;
		private static ConfigurationProperty propDelegate;
		
		private static ConfigurationPropertyCollection properties;

		static RoutesElement() {
			propAssemblyScripts   = new ConfigurationProperty("assemblyScripts", typeof(string));
			propAssemblyCss       = new ConfigurationProperty("assemblyCss", typeof(string));
			propAssemblyResources = new ConfigurationProperty("assemblyResources", typeof(string));
			propDelegate          = new ConfigurationProperty("delegate", typeof(string));
			properties            = new ConfigurationPropertyCollection() { propAssemblyScripts, propAssemblyCss, propAssemblyResources, propDelegate };
		}

		[ConfigurationProperty("assemblyScripts")]
		public string AssemblyScripts {
			get { return (string)base[propAssemblyScripts]; }
		}

		[ConfigurationProperty("assemblyCss")]
		public string AssemblyCss {
			get { return (string)base[propAssemblyCss]; }
		}

		[ConfigurationProperty("assemblyResources")]
		public string AssemblyResources {
			get { return (string)base[propAssemblyResources]; }
		}

		[ConfigurationProperty("delegate")]
		public string Delegate {
			get { return (string)base[propDelegate]; }
		}

		protected override ConfigurationPropertyCollection Properties {
			get { return properties; }
		}
	}
}
