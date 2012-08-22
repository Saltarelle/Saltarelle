using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Saltarelle.Configuration {
	public class RoutesElement {
		public string AssemblyScripts { get; set; }
		public string AssemblyCss { get; set; }
		public string AssemblyResources { get; set; }
		public string Delegate { get; set; }

	    public RoutesElement() {
	    }

	    public RoutesElement(string assemblyScripts, string assemblyCss, string assemblyResources, string @delegate) {
	        AssemblyScripts = assemblyScripts;
	        AssemblyCss = assemblyCss;
	        AssemblyResources = assemblyResources;
	        Delegate = @delegate;
	    }
	}
}
