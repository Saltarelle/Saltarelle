using System;

namespace Saltarelle.Ioc {
    public interface IContainer {
		/// <summary>
		/// Resolves a service. Throws if the service does not exist.
		/// </summary>
        object ResolveService(Type objectType);
		/// <summary>
		/// Resolves a service by its type name. Throws if the service does not exist.
		/// </summary>
        object ResolveServiceByTypeName(string typeName);

		/// <summary>
		/// Creates a new object.
		/// </summary>
        object CreateObject(Type objectType);

		/// <summary>
		/// Creates a new object by its type name.
		/// </summary>
        object CreateObjectByTypeName(string typeName);

        /// <summary>
        /// Finds a type by its full (not assembly-qualified) name.
        /// </summary>
        /// <param name="typeName">Full (not assembly-qualified) name of the type.</param>
		Type FindType(string typeName);

        #if SERVER
			/// <summary>
			/// Generic version of <see cref="ResolveService(Type)"/>.
			/// </summary>
            T ResolveService<T>();

			/// <summary>
			/// Generic version of <see cref="ResolveService(Type)"/>.
			/// </summary>
            T CreateObject<T>();

			/// <summary>
			/// Registers that a service should be available on the client.
			/// </summary>
			void RegisterClientService(Type serviceType);

			/// <summary>
			/// Generic version of <see cref="RegisterClientService(Type)"/>
			/// </summary>
			void RegisterClientService<TService>(TService implementer);

			/// <summary>
			/// Performs the necessary steps to write this object to a script manager.
			/// </summary>
            void ApplyToScriptManager(IScriptManagerService scriptManager);
        #endif
        #if CLIENT
			/// <summary>
			/// Creates a new object, passing an argument to the constructor. The main (only intended) use of this method is during page load.
			/// </summary>
            object CreateObjectWithConstructorArg(Type objectType, object a0);

			/// <summary>
			/// Creates a new object by its type name, passing an argument to the constructor. The main (only intended) use of this method is during page load.
			/// </summary>
            object CreateObjectByTypeNameWithConstructorArg(string typeName, object a0);
        #endif
    }
}