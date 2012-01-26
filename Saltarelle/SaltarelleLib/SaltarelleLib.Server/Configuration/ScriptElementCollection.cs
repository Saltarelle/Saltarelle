using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Saltarelle.Configuration {
	public class ScriptElementCollection : ReadOnlyCollection<ScriptElement> {
        public bool Debug { get; set; }

        public ScriptElementCollection(List<ScriptElement> l, bool debug) : base(l) {
            Debug = debug;
        }
	}
}
