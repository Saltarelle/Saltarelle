using System;

namespace Saltarelle.Ioc {
    public interface IContainer {
        object ResolveService(Type objectType);
        object ResolveServiceByTypeName(string typeName);

        object CreateObject(Type objectType);
        object CreateObjectByTypeName(string typeName);

        Type FindType(string typeName);
        #if SERVER
            T ResolveService<T>();
            T CreateObject<T>();
            void ApplyToScriptManager(IScriptManagerService scriptManager);
        #endif
        #if CLIENT
            object CreateObjectWithConstructorArg(Type objectType, object a0);
            object CreateObjectByTypeNameWithConstructorArg(string typeName, object a0);
        #endif
    }
}