using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Linq;
using Saltarelle.Mvc;
using Saltarelle.Mvc.CoreServiceImplementations;
using dotless.Core;
using dotless.Core.configuration;
using System.Web.Configuration;
using Saltarelle.Configuration;
using System.Configuration;

// ReSharper disable CheckNamespace
namespace Saltarelle.Ioc {
// ReSharper restore CheckNamespace
	public class DefaultContainer : IContainer {

	    private readonly Func<string, Type> _findType;
	    private readonly Func<Type, object> _resolveService;
        private readonly Func<Type, object> _createObject;

        private readonly List<object> _createdObjects = new List<object>();

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

        private void GatherServices(IEnumerable<object> objects, Dictionary<Type, IService> services) {
            var serviceTypes = objects.Select(o => o.GetType()).Distinct().SelectMany(Helpers.FindPropertiesToInject).Select(p => p.Item2).Distinct();
            var newObjects = new List<object>();
            foreach (var st in serviceTypes) {
                if (!services.ContainsKey(st)) {
                    var svc = ResolveService(st);
                    if (!(svc is IService))
                        throw new InvalidOperationException("The service type " + svc.GetType().FullName + ", implementing the service " + st.FullName + ", does not implement IService.");
                    services[st] = (IService)svc;
                    newObjects.Add(svc);
                }
            }
            if (newObjects.Count > 0)
                GatherServices(newObjects, services);
        }

	    public void ApplyToScriptManager(IScriptManagerService scriptManager) {
            var services = new Dictionary<Type, IService>();
            GatherServices(_createdObjects, services);
            foreach (var asm in _createdObjects.Union(services.Values).Select(o => o.GetType().Assembly).Distinct())
                scriptManager.RegisterClientAssembly(asm);
            foreach (var svc in services)
                scriptManager.RegisterClientService(svc.Key, svc.Value);
	    }
	}
}
