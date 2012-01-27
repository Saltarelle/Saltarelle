using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Saltarelle.Configuration {
	public class RoutesElement {
		public string AssemblyScripts { get; private set; }
		public string AssemblyCss { get; private set; }
		public string AssemblyResources { get; private set; }
		public string Delegate { get; private set; }

	    public RoutesElement(string assemblyScripts, string assemblyCss, string assemblyResources, string @delegate) {
	        AssemblyScripts = assemblyScripts;
	        AssemblyCss = assemblyCss;
	        AssemblyResources = assemblyResources;
	        Delegate = @delegate;
	    }
	}
}
