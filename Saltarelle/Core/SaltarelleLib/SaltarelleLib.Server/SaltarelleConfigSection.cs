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

	[ConfigurationCollection(typeof(PluginElement), CollectionType=ConfigurationElementCollectionType.BasicMap, AddItemName="plugin")]
	public class PluginElementCollection : ConfigurationElementCollection {
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	
		public PluginElement this[int index] {
			get { return (PluginElement)base.BaseGet(index); }
			set {
				if (base.BaseGet(index) != null)
					base.BaseRemoveAt(index);
				base.BaseAdd(value);
			}
		}
		
		public new PluginElement this[string name] {
			get { return (PluginElement)base.BaseGet(name); }
		}
		
		protected override ConfigurationPropertyCollection Properties {
			get { return properties; }
		}

		protected override string ElementName {
			get { return "plugin"; }
		}

		public override ConfigurationElementCollectionType CollectionType {
			get { return ConfigurationElementCollectionType.BasicMap; }
		}
		
		protected override ConfigurationElement CreateNewElement() {
			return new PluginElement();
		}

		protected override object GetElementKey(ConfigurationElement element) {
			return ((PluginElement)element).Assembly;
		}
	}
	
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

	public class SaltarelleConfigSection : ConfigurationSection {
		private static ConfigurationProperty propPlugins;
		private static ConfigurationProperty propRoutes;
		private static ConfigurationPropertyCollection properties;

		static SaltarelleConfigSection() {
			propPlugins = new ConfigurationProperty("plugins", typeof(PluginElementCollection));
			propRoutes  = new ConfigurationProperty("routes", typeof(RoutesElement));
			properties  = new ConfigurationPropertyCollection() { propPlugins, propRoutes };
		}
		
		[ConfigurationProperty("plugins")]
		public PluginElementCollection Plugins {
			get { return (PluginElementCollection)base[propPlugins]; }
		}

		[ConfigurationProperty("routes")]
		public RoutesElement Routes {
			get { return (RoutesElement)base[propRoutes]; }
		}

		protected override ConfigurationPropertyCollection Properties {
			get { return properties; }
		}
	}
	
}
