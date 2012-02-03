using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Linq;
using Saltarelle.Mvc;
using dotless.Core;
using dotless.Core.configuration;
using System.Web.Configuration;
using Saltarelle.Configuration;
using System.Configuration;

// ReSharper disable CheckNamespace
namespace Saltarelle.Ioc {
// ReSharper restore CheckNamespace
	public class DefaultContainer : IContainer, ITransferrableService {
        private static ConcurrentDictionary<Type, List<Tuple<string, Type>>> _propertiesToInjectCache = new ConcurrentDictionary<Type, List<Tuple<string, Type>>>();

        private static List<Tuple<string, Type>> CalcPropertiesToInject(Type type) {
            return         type.GetCustomAttributes(typeof(ClientInjectPropertyAttribute), true)
                           .Select(a => Tuple.Create(((ClientInjectPropertyAttribute)a).PropertyName, ((ClientInjectPropertyAttribute)a).PropertyType))
                   .Concat(type.GetProperties()
                           .Where(p => p.GetCustomAttributes(typeof(ClientInjectAttribute), true).Length > 0)
                           .Select(p => Tuple.Create(p.Name, p.PropertyType)))
                   .ToList();
        }

        private static List<Tuple<string, Type>> FindPropertiesToInject(Type type) {
            List<Tuple<string, Type>> result;
            if (_propertiesToInjectCache.TryGetValue(type, out result))
                return result;
            result = CalcPropertiesToInject(type);
            _propertiesToInjectCache.TryAdd(type, result);
            return result;
        }

	    private readonly Func<string, Type> _findType;
	    private readonly Func<Type, object> _resolve;

        private readonly HashSet<Type> _registeredTypes = new HashSet<Type>();

	    public DefaultContainer(Func<string, Type> findType, Func<Type, object> resolve) {
	        _findType = findType;
	        _resolve = resolve;
	    }

	    public object Resolve(Type objectType) {
            _registeredTypes.Add(objectType);
	        return _resolve(objectType);
	    }

	    public object ResolveByTypeName(string typeName) {
	        return Resolve(FindType(typeName));
	    }

	    public Type FindType(string typeName) {
	        return _findType(typeName);
	    }

	    public T Resolve<T>() {
	        return (T)Resolve(typeof(T));
	    }

        private void EnsureAllRequiredTypesRegistered(IEnumerable<Type> types) {
            // This method is required because 1) not all resolving might be done through us (we're likely to be a facade for eg. Windsor), and 2) the server might not use a dependency required by the client.
            var missing = new HashSet<Type>(types.SelectMany(t => FindPropertiesToInject(t)).Select(p => p.Item2));
            missing.ExceptWith(_registeredTypes);
            if (missing.Count > 0) {
                foreach (var m in missing)
                    Resolve(m);
                EnsureAllRequiredTypesRegistered(missing);
            }
        }

	    public object ConfigObject {
            get {
                EnsureAllRequiredTypesRegistered(_registeredTypes);

                var injections = (  from t in _registeredTypes
                                     let props = FindPropertiesToInject(t)
                                   where props.Count > 0
                                  select new { type = t, properties = props.Select(x => new { name = x.Item1, type = x.Item2 }).ToList() }
                                 ).ToList();

                return new { injections };
            }
	    }
	}
}
