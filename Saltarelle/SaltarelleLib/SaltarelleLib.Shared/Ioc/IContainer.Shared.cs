using System;

namespace Saltarelle.Ioc {
    public interface IContainer {
        object Resolve(Type objectType);
        object ResolveByTypeName(string typeName);
        Type FindType(string typeName);
        #if SERVER
            T Resolve<T>();
        #endif
        #if CLIENT
            object ResolveWithConstructorArg(Type objectType, object a0);
            object ResolveByTypeNameWithConstructorArg(string typeName, object a0);
        #endif
    }
}