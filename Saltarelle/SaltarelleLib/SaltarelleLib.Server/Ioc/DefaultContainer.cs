using System;
using System.Collections.Generic;
using System.Linq;

namespace Saltarelle.Ioc {
	public class DefaultContainer : IContainer {
	    private readonly Func<string, Type> _findType;
	    private readonly Func<Type, object> _resolveService;
        private readonly Func<Type, object> _createObject;

        private List<object> _createdObjects = new List<object>();

	    /// <summary>
	    /// Constructs an instance.
	    /// </summary>
	    /// <param name="findType">Function used to retrieve a type by its name.</param>
	    /// <param name="resolveService">Function used to resolve a service. When this method creates an object (directly or indirectly), it must make sure to call INotifyCreate.DependenciesAvailable (if applicable).</param>
	    /// <param name="createObject">Function used to create an object. When this method creates an object, it must make sure to call INotifyCreate.DependenciesAvailable (if applicable).</param>
		public DefaultContainer(Func<string, Type> findType, Func<Type, object> resolveService, Func<Type, object> createObject) {
	        _findType       = findType;
	        _resolveService = resolveService;
            _createObject   = createObject;
	    }

	    public object ResolveService(Type objectType) {
	        return _resolveService(objectType);
	    }

	    public object ResolveServiceByTypeName(string typeName) {
	        return ResolveService(FindType(typeName));
	    }

	    public T ResolveService<T>() {
	        return (T)ResolveService(typeof(T));
	    }

	    public object CreateObject(Type objectType) {
            var result = _createObject(objectType);
            _createdObjects.Add(result);
            return result;
	    }

	    public object CreateObjectByTypeName(string typeName) {
	        return CreateObject(FindType(typeName));
	    }

	    public Type FindType(string typeName) {
	        return _findType(typeName);
	    }

	    public T CreateObject<T>() {
            return (T)CreateObject(typeof(T));
	    }

        private List<object> GatherServices(IEnumerable<object> objects, Dictionary<Type, object> services) {
            var serviceTypes = objects.Select(o => o.GetType()).Distinct().SelectMany(Helpers.FindPropertiesToInject).Select(p => p.Item2).Distinct();
            var createdServices = new List<object>();
            foreach (var st in serviceTypes) {
                if (!services.ContainsKey(st)) {
                    var svc = ResolveService(st);
                    services[st] = svc;
                    createdServices.Add(svc);
                }
            }
            if (createdServices.Count > 0)
                createdServices.AddRange(GatherServices(createdServices, services));
			return createdServices;
        }

		private void GatherServicesAndInvokeBeforeWriteScriptsCallbacks(IScriptManagerService scriptManager, Dictionary<Type, object> services) {
			var allCreatedObjects = _createdObjects;
			while (_createdObjects.Count > 0) {
				var current = _createdObjects;
				_createdObjects = new List<object>();
				foreach (var o in current.OfType<IBeforeWriteScriptsCallback>())
					o.BeforeWriteScripts(scriptManager);
				var newServices = GatherServices(current, services);
				foreach (var o in newServices.OfType<IBeforeWriteScriptsCallback>())
					o.BeforeWriteScripts(scriptManager);

				allCreatedObjects.AddRange(_createdObjects);	// This iteration might have caused new types to be created. In that case, add them to the set and continue the loop.
			}
			_createdObjects = allCreatedObjects;
		}

	    public void ApplyToScriptManager(IScriptManagerService scriptManager) {
            var services = new Dictionary<Type, object>();
            GatherServicesAndInvokeBeforeWriteScriptsCallbacks(scriptManager, services);
            foreach (var asm in _createdObjects.Union(services.Values).Select(o => o.GetType().Assembly).Distinct())
                scriptManager.RegisterClientAssembly(asm);
            foreach (var svc in services)
                scriptManager.RegisterClientService(svc.Key, svc.Value);
	    }
	}
}
