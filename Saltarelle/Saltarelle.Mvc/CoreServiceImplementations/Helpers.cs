using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltarelle.Ioc;

namespace Saltarelle.Mvc.CoreServiceImplementations {
    internal class Helpers {
        private static readonly ConcurrentDictionary<Type, List<Tuple<string, Type>>> _propertiesToInjectCache = new ConcurrentDictionary<Type, List<Tuple<string, Type>>>();

        private static List<Tuple<string, Type>> CalcPropertiesToInject(Type type) {
            return         type.GetCustomAttributes(typeof(ClientInjectPropertyAttribute), true)
                .Select(a => Tuple.Create(((ClientInjectPropertyAttribute)a).PropertyName, ((ClientInjectPropertyAttribute)a).PropertyType))
                .Concat(type.GetProperties()
                            .Where(p => p.GetCustomAttributes(typeof(ClientInjectAttribute), true).Length > 0)
                            .Select(p => Tuple.Create(p.Name, p.PropertyType)))
                .ToList();
        }

        public static List<Tuple<string, Type>> FindPropertiesToInject(Type type) {
            List<Tuple<string, Type>> result;
            if (_propertiesToInjectCache.TryGetValue(type, out result))
                return result;
            result = CalcPropertiesToInject(type);
            _propertiesToInjectCache.TryAdd(type, result);
            return result;
        }
    }
}
