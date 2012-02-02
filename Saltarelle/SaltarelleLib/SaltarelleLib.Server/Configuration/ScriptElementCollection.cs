using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Saltarelle.Configuration {
	public class ScriptElementCollection : List<ScriptElement> {
        public bool Debug { get; set; }
	}
}
