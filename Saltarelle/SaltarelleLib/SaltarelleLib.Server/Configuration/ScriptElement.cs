using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;

namespace Saltarelle.Configuration {
	public enum ScriptPosition {
		BeforeCoreScripts,
		BeforeAssemblyScripts,
		AfterAssemblyScripts
	}

	public class ScriptElement : ConfigurationElement {
		private static ConfigurationProperty propUrl;
		private static ConfigurationProperty propPosition;
		private static ConfigurationProperty propAssembly;
		private static ConfigurationProperty propResource;
		private static ConfigurationPropertyCollection properties;

		static ScriptElement() {
			propUrl      = new ConfigurationProperty("url", typeof(string));
			propPosition = new ConfigurationProperty("position", typeof(ScriptPosition));
			propAssembly = new ConfigurationProperty("assembly", typeof(string));
			propResource = new ConfigurationProperty("resource", typeof(string));
			properties   = new ConfigurationPropertyCollection() { propUrl, propPosition, propAssembly, propResource };
		}
		
		[ConfigurationProperty("url")]
		public string Url {
			get { return (string)base[propUrl]; }
		}
		
		[ConfigurationProperty("position")]
		public ScriptPosition Position {
			get { return (ScriptPosition)base[propPosition]; }
		}

		[ConfigurationProperty("assembly")]
		public string Assembly {
			get { return (string)base[propAssembly]; }
		}
		
		[ConfigurationProperty("resource")]
		public string Resource {
			get { return (string)base[propResource]; }
		}

		protected override ConfigurationPropertyCollection Properties {
			get { return properties; }
		}
	}
}
