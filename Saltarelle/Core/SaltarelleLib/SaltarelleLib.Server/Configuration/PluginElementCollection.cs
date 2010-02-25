using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Saltarelle.Configuration {
	[ConfigurationCollection(typeof(PluginElement), CollectionType=ConfigurationElementCollectionType.BasicMap, AddItemName="plugin")]
	public class PluginElementCollection : ConfigurationElementCollection {
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	
		public PluginElement this[int index] {
			get { return (PluginElement)base.BaseGet(index); }
			set {
				if (base.BaseGet(index) != null)
					base.BaseRemoveAt(index);
				base.BaseAdd(index, value);
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
}
