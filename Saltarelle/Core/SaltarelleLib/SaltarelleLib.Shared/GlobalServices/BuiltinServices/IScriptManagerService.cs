using System;
#if SERVER
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
#endif

namespace Saltarelle {
	public interface IScriptManagerService {
		/// <summary>
		/// Get a uniqe id, suitable for assignment to a new control.
		/// </summary>
		/// <returns>A unique id.</returns>
		string GetUniqueId();

		/// <summary>
		/// Register a control as a top level control. This means that its constructor will get called on page load.
		/// The control can later be retrieved using the <see cref="GetTopLevelControl"/> method.
		/// </summary>
		/// <param name="control">The top level control.</param>
		/// <param name="id">Key with which the control can later be retrieved by the <see cref="GetTopLevelControl"/> method.</param>
		void RegisterTopLevelControl(string id, IControl control);

		/// <summary>
		/// Gets a control previously registered by the <see cref="RegisterTopLevelControl(string, Saltarelle.IControl)"/> method. Returns null if the control has not been registered.
		/// </summary>
		/// <param name="id">ID under which the control to return is registered as.</param>
		/// <returns>The registered control, or null if the control has not been registered.</returns>
		IControl GetTopLevelControl(string id);

#if SERVER
		/// <summary>
		/// Registers a service which should be usable on the client. This will cause an instance of this service to be loaded on the server.
		/// The <see cref="RequiresClientServiceAttribute"/> attribute can be used to always register a service when a type is registered.
		/// Registering a service also registers its type.
		/// </summary>
		/// <param name="type">Type of the service. This type must implement the IGlobalService interface</param>
		void RegisterClientService(Type type);

		/// <summary>
		/// Determines whether a service is registered (by the <see cref="RegisterClientService"/> method).
		/// </summary>
		/// <param name="type">Type of the service. This type must implement the IGlobalService interface</param>
		bool IsClientServiceRegistered(Type type);

		/// <summary>
		/// Register that an assembly, and all assemblies it depends on, will be referenced on the client.
		/// </summary>
		/// <param name="asm">Type to register</param>
		void RegisterClientAssembly(Assembly asm);

		/// <summary>
		/// Register a control as a top level control. This means that its constructor will get called on page load.
		/// </summary>
		/// <param name="control">The top level control.</param>
		void RegisterTopLevelControl(IControl control);

		/// <summary>
		/// Instruct the client to include a specific script. This script should not be an assembly script, for those use <see cref="RegisterClientAssembly"/> (or one of the extension methods that delegate to it).
		/// Calling this method multiple times for the same script does NOT result in the script being included more than once.
		/// </summary>
		/// <param name="url">Url of the script to include</param>
		/// <param name="includeBeforeAssemblyScripts">True to include the script before any assembly script, false to include it after all assembly scripts.</param>
		void AddScriptInclude(string url, bool includeBeforeAssemblyScripts);

		/// <summary>
		/// Get an ordered list of scripts to include. This method will return all assembly scripts in the correct topological order of their dependencies.
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetAllRequiredIncludes();
		
		/// <summary>
		/// Add a startup script with lazy evaluation. The script will be retrieved just before it is actually rendered, allowing
		/// the use of startup scripts that depend on the final state of a control, as opposed to when the script is added.
		/// </summary>
		/// <param name="scriptRetriever">Function to invoke to return the startup script. The returned script should be a valid JavaScript statement.</param>
		void AddStartupScript(Func<string> scriptRetriever);

		/// <summary>
		/// Get all registered startup scripts. Each entry is a statement which should be executed on document load.
		/// </summary>
		/// <returns>The registered scripts</returns>
		IEnumerable<string> GetStartupScripts();
		
		/// <summary>
		/// Registers an actio nto be executed just before the script manager content is rendered. This callback can access the script manager.
		/// </summary>
		/// <param name="action">Action to execute just before the content is rendered.</param>
		void RegisterBeforeRenderCallback(Action action);
		
		/// <summary>
		/// Executes all the actions that have been added with the <see cref="RegisterBeforeRenderCallback"/> method.
		/// </summary>
		void ExecuteBeforeRenderCallbacks();
#endif
#if CLIENT
		/// <summary>
		/// Ensure that a script is included. If the script is already included, nothing will happend, if not, it will be synchronously loaded
		/// </summary>
		/// <param name="relativeUrl">Relative url of the script</param>
		void EnsureScriptIncluded(string relativeUrl);
#endif
	}
	
#if SERVER
	public static class IScriptManagerServiceExtensions {
		/// <summary>
		/// Add a startup script.
		/// </summary>
		/// <param name="service">Instance to add the script to.</param>
		/// <param name="script">The startup script. This should be a valid JavaScript statement.</param>
		public static void AddStartupScript(this IScriptManagerService service, string script) {
			service.AddStartupScript(() => script);
		}
		
		/// <summary>
		/// Register a type as being used on the client side. This means that the script for this type's assembly,
		/// and all assemblies it depends on, will be referenced on the client.
		/// This method should be called in the constructors for all objects that are to be used on the client side and are not decorated with a [Record] attribute.
		/// This method will also investigate the [RequiresClientServiceAttribute] attributes for the types and register those services.
		/// </summary>
		/// <param name="service">Instance to register the type for.</param>
		/// <param name="type">Type to register</param>
		public static void RegisterClientType(this IScriptManagerService service, Type type) {
			service.RegisterClientAssembly(type.Assembly);
			foreach (RequiresClientServiceAttribute attr in type.GetCustomAttributes(typeof(RequiresClientServiceAttribute), true))
				service.RegisterClientService(attr.ServiceType);
		}

		/// <summary>
		/// Generic version of <see cref="RegisterClientType(IScriptManagerService,Type)"/>
		/// </summary>
		/// <typeparam name="T">Type to register.</typeparam>
		/// <param name="service">Instance to register the type for.</param>
		public static void RegisterClientType<T>(this IScriptManagerService service) {
			service.RegisterClientType(typeof(T));
		}

		/// <summary>
		/// Generic version of <see cref="IScriptManagerService.RegisterClientService"/>
		/// </summary>
		/// <typeparam name="T">Service to register.</typeparam>
		/// <param name="service">Instance to register the service for.</param>
		public static void RegisterClientService<T>(this IScriptManagerService service) {
			service.RegisterClientService(typeof(T));
		}
	}
#endif
}
