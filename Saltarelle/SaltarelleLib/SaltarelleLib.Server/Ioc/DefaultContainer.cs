using System;
using System.Collections.Generic;
using System.Linq;

namespace Saltarelle.Ioc {
	public class DefaultContainer : IContainer {
	    private readonly Func<string, Type> _findType;
	    private readonly Func<Type, object> _resolveService;
        private readonly Func<Type, object> _createObject;

		/// <summary>
		/// The first item in the tuple is the object, the second is the service type implemented by this object, or null if the object does not implement any service (which most objects don't).
		/// </summary>
        private HashSet<Tuple<object, Type>> _createdObjects = new HashSet<Tuple<object, Type>>();

		private HashSet<Type> _registeredClientUsableTypes = new HashSet<Type>();

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
            _createdObjects.Add(Tuple.Create(result, (Type)null));
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

        public void RegisterClientService(Type serviceType) {
            if (!serviceType.IsInterface || serviceType == typeof(IService))
                throw new InvalidOperationException("Transferred services must be interfaces, and must not be the IService interface (tried to register type " + serviceType.FullName + ").");
            _createdObjects.Add(Tuple.Create(ResolveService(serviceType), serviceType));
        }

        public void RegisterClientService<TService>(TService implementer) {
			RegisterClientService(typeof(TService));
        }

		public void EnsureTypeClientUsable(Type type) {
			if (!_registeredClientUsableTypes.Contains(type)) {
				_registeredClientUsableTypes.Add(type);
				foreach (var serviceType in Helpers.FindPropertiesToInject(type).Select(p => p.Item2).Distinct())
					RegisterClientService(serviceType);
			}
		}

		public void EnsureTypeClientUsable<T>() {
			EnsureTypeClientUsable(typeof(T));
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
				_createdObjects = new HashSet<Tuple<object, Type>>();
				foreach (var o in current.Select(x => x.Item1).OfType<IBeforeWriteScriptsCallback>())
					o.BeforeWriteScripts(scriptManager);
				var newServices = GatherServices(current.Select(o => o.Item1), services);
				foreach (var o in newServices.OfType<IBeforeWriteScriptsCallback>())
					o.BeforeWriteScripts(scriptManager);

				foreach (var o in _createdObjects)	// This iteration might have caused new types to be created. In that case, add them to the set and continue the loop.
					allCreatedObjects.Add(o);
			}
			_createdObjects = allCreatedObjects;
		}

	    public void ApplyToScriptManager(IScriptManagerService scriptManager) {
            var services = new Dictionary<Type, object>();
            GatherServicesAndInvokeBeforeWriteScriptsCallbacks(scriptManager, services);
            foreach (var asm in         _createdObjects.Select(o => o.Item1.GetType().Assembly)
			                    .Concat(services.Values.Select(o => o.GetType().Assembly))
			                    .Concat(_registeredClientUsableTypes.Select(t => t.Assembly))
			                    .Distinct())
			{
                scriptManager.RegisterClientAssembly(asm);
			}

            foreach (var svc in services.Concat(_createdObjects.Where(t => t.Item2 != null).Select(t => new KeyValuePair<Type, object>(t.Item2, t.Item1))).Distinct())
                scriptManager.RegisterClientService(svc.Key, svc.Value);
	    }
	}
}
