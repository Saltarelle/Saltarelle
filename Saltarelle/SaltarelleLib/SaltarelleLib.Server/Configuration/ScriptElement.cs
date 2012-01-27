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

	public class ScriptElement {
	    public string Url { get; private set; }
		public ScriptPosition Position { get; private set; }
		public string Assembly { get; private set; }
		public string Resource { get; private set; }

	    public ScriptElement(string url, ScriptPosition position, string assembly, string resource) {
	        Url = url;
	        Position = position;
	        Assembly = assembly;
	        Resource = resource;
	    }
	}
}
