using System;
using Saltarelle.Ioc;
#if SERVER
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
#endif

namespace Saltarelle {
	public interface IScriptManagerService
    {
		/// <summary>
		/// Get a uniqe id, suitable for assignment to a new control.
		/// </summary>
		/// <returns>A unique id.</returns>
		string GetUniqueId();

		/// <summary>
		/// Gets a control previously registered by the <see cref="RegisterTopLevelControl"/> method. Returns null if the control has not been registered.
		/// </summary>
		/// <param name="id">ID under which the control to return is registered as.</param>
		/// <returns>The registered control, or null if the control has not been registered.</returns>
		IControl GetTopLevelControl(string id);

		/// <summary>
		/// Register a control as a top level control. This means that its constructor will get called on page load.
		/// The control can later be retrieved using the <see cref="GetTopLevelControl"/> method with its ID as the key.
		/// Note: This does NOT register the assembly containing the type.
		/// </summary>
		/// <param name="control">The top level control.</param>
		void RegisterTopLevelControl(IControl control);

#if SERVER
		/// <summary>
		/// Register that an assembly, and all assemblies it depends on, is usable from script. User code should normally not need to call this.
		/// </summary>
		/// <param name="asm">Assembly to register</param>
		void RegisterClientAssembly(Assembly asm);

		/// <summary>
		/// Instruct the client to include a specific script. This script should not be an assembly script, for those use <see cref="RegisterClientAssembly"/> (or one of the extension methods that delegate to it).
		/// Calling this method multiple times for the same script does NOT result in the script being included more than once.
		/// </summary>
		/// <param name="url">Url of the script to include</param>
		/// <param name="includeBeforeAssemblyScripts">True to include the script before any assembly script, false to include it after all assembly scripts.</param>
		void AddScriptInclude(string url, bool includeBeforeAssemblyScripts);

		/// <summary>
		/// DON'T USE THIS METHOD FROM USER CODE. Register a client service (called by the container). Always register services in a container.
		/// </summary>
		void RegisterClientService(Type serviceType, object implementer);

		/// <summary>
		/// Gets the markup for the scripts managed by this script manager.
		/// </summary>
        IHtmlString GetMarkup();

		/// <summary>
		/// Gets the markup for the scripts managed by this script manager.
		/// </summary>
		/// <param name="container">Container that contains the control.</param>
		/// <param name="control">Control to transfer in the fragment.</param>
        ControlDocumentFragment CreateControlDocumentFragment(IContainer container, IControl control);
#endif
#if CLIENT
		/// <summary>
		/// Ensure that a script is included. If the script is already included, nothing will happend, if not, it will be synchronously loaded
		/// </summary>
		/// <param name="relativeUrl">Relative url of the script</param>
		void EnsureScriptIncluded(string relativeUrl);
#endif
	}
}
