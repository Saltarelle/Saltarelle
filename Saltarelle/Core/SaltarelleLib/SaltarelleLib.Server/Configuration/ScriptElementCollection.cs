using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Saltarelle.Configuration {
	[ConfigurationCollection(typeof(ScriptElement), CollectionType=ConfigurationElementCollectionType.BasicMap)]
	public class ScriptElementCollection : ConfigurationElementCollection {
		private static ConfigurationProperty propDebug;
		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		
		static ScriptElementCollection() {
			propDebug  = new ConfigurationProperty("debug", typeof(bool));
			properties = new ConfigurationPropertyCollection() { propDebug };
		}

		public ScriptElement this[int index] {
			get { return (ScriptElement)base.BaseGet(index); }
			set {
				if (base.BaseGet(index) != null)
					base.BaseRemoveAt(index);
				base.BaseAdd(index, value);
			}
		}

		public new ScriptElement this[string name] {
			get { return (ScriptElement)base.BaseGet(name); }
		}

		protected override ConfigurationPropertyCollection Properties {
			get { return properties; }
		}

		[ConfigurationProperty("debug")]
		public bool Debug {
			get { return (bool)base[propDebug]; }
		}

		protected override ConfigurationElement CreateNewElement() {
			return new ScriptElement();
		}

		protected override object GetElementKey(ConfigurationElement element) {
			var se = (ScriptElement)element;
			return (se.Url ?? "") + "|" + (se.Assembly ?? "") + "|" + (se.Resource ?? "");
		}
	}
}
