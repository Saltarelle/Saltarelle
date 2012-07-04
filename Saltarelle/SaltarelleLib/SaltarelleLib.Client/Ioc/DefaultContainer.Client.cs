using System;
using System.Collections.Generic;

namespace Saltarelle.Ioc {
    public class DefaultContainer : IContainer {
        private Dictionary<string, ScriptManagerConfigServiceEntry> serviceConfig;
        private Dictionary<string, IList<ScriptManagerConfigInjectedPropertyRow>> injections;
        private Dictionary<string, object> runningServices;

        public DefaultContainer() {
            serviceConfig   = new Dictionary<string, ScriptManagerConfigServiceEntry>();
            injections      = new Dictionary<string, IList<ScriptManagerConfigInjectedPropertyRow>>();
            runningServices = new Dictionary<string, object>();
            runningServices[typeof(IContainer).FullName] = this;
        }

        public void RegisterServiceConfig(string typeName, ScriptManagerConfigServiceEntry config) {
            if (!this.serviceConfig.ContainsKey(typeName))
                this.serviceConfig[typeName] = config;
        }

        public void RegisterInjection(string typeName, IList<ScriptManagerConfigInjectedPropertyRow> injections) {
            if (!this.injections.ContainsKey(typeName))
                this.injections[typeName] = injections;
        }

        private void Inject(object o) {
            string typeName = o.GetType().FullName;
            if (injections.ContainsKey(typeName)) {
                foreach (ScriptManagerConfigInjectedPropertyRow r in injections[typeName]) {
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
