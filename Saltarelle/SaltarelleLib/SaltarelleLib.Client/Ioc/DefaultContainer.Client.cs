using System;

namespace Saltarelle.Ioc {
    public class DefaultContainer : IContainer {
        private Dictionary serviceConfig;
        private Dictionary injections;
        private Dictionary runningServices;

        public DefaultContainer() {
            serviceConfig   = new Dictionary();
            injections      = new Dictionary();
            runningServices = new Dictionary();
            runningServices[typeof(IContainer).FullName] = this;
        }

        public void RegisterServiceConfig(Dictionary/*<string, ScriptManagerConfigServiceEntry>*/ config) {
            foreach (DictionaryEntry e in config) {
                if (!this.serviceConfig.ContainsKey(e.Key))
                    this.serviceConfig[e.Key] = e.Value;
            }
        }

        public void RegisterInjections(Dictionary/*<string, IList<ScriptManagerConfigInjectedPropertyRow>*/ injections) {
            foreach (DictionaryEntry e in injections) {
                if (!this.injections.ContainsKey(e.Key))
                    this.injections[e.Key] = e.Value;
            }
        }

        private void Inject(object o) {
            string typeName = o.GetType().FullName;
            if (injections.ContainsKey(typeName)) {
                foreach (ScriptManagerConfigInjectedPropertyRow r in (ArrayList)injections[typeName]) {
                    Utils.SetPropertyValue(o, r.propertyName, ResolveServiceByTypeName(r.typeName));
                }
            }
			if (o is INotifyCreated)
				((INotifyCreated)o).DependenciesAvailable();
        }

        public object ResolveService(Type objectType) {
            return ResolveServiceByTypeName(objectType.FullName);
        }

        public object ResolveServiceByTypeName(string typeName) {
            if (runningServices.ContainsKey(typeName))
                return runningServices[typeName];

            if (serviceConfig.ContainsKey(typeName)) {
                object result = CreateObjectByTypeNameWithConstructorArg(((ScriptManagerConfigServiceEntry)serviceConfig[typeName]).type, ((ScriptManagerConfigServiceEntry)serviceConfig[typeName]).config);
                runningServices[typeName] = result;
                return result;
            }

            throw new Exception("Service " + typeName + " has not been loaded.");
        }

        public Type FindType(string typeName) {
            try {
	            return Type.GetType(typeName) ?? null; // We can get 'undefined' if the namespace is defined, but the type is not ...
			}
            catch (Exception) {
                // ... and we can get an exception if the namespace is not defined.
                return null;
            }
		}

        public object CreateObject(Type objectType) {
            object result = Type.CreateInstance(objectType);
            Inject(result);
            return result;
        }

        public object CreateObjectByTypeName(string typeName) {
            return CreateObject(FindType(typeName));
        }

        public object CreateObjectWithConstructorArg(Type objectType, object a0) {
            object result = Type.CreateInstance(objectType, a0);
            Inject(result);
            return result;
        }

        public object CreateObjectByTypeNameWithConstructorArg(string typeName, object a0) {
            return CreateObjectWithConstructorArg(FindType(typeName), a0);
        }
    }
}
