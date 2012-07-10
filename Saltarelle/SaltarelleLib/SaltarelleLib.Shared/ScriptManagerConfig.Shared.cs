using System.Runtime.CompilerServices;
#if SERVER
using ServiceDictionary = System.Collections.Generic.Dictionary<string, Saltarelle.ScriptManagerConfigServiceEntry>;
using InjectionDictionary = System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<Saltarelle.ScriptManagerConfigInjectedPropertyRow>>;
#else
using ServiceDictionary = System.Collections.Generic.JsDictionary<string, Saltarelle.ScriptManagerConfigServiceEntry>;
using InjectionDictionary = System.Collections.Generic.JsDictionary<string, System.Collections.Generic.List<Saltarelle.ScriptManagerConfigInjectedPropertyRow>>;
#endif

namespace Saltarelle {
    [Record]
    public sealed class ScriptManagerConfigControlRow {
        public string type;
        public object config;
    }

    [Record]
    public sealed class ScriptManagerConfigServiceEntry {
        public string type;
        public object config;
    }

    [Record]
    public sealed class ScriptManagerConfigInjectedPropertyRow {
        public string propertyName;
        public string typeName;
    }

    [Record]
    public sealed class ScriptManagerConfig {
        public int nextUniqueId;
        public ServiceDictionary services;
        public InjectionDictionary injections;
        public ScriptManagerConfigControlRow[] controls;
    }
}