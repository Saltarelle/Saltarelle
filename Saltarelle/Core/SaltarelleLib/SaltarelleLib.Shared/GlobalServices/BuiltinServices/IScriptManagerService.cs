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

#if SERVER
		/// <summary>
		/// Register a type as being used. This means that the script for this type's assembly,
		/// and all assemblies it depends on, will be referenced on the client.
		/// This method should be called in the constructors for all objects that are not decorated with a [Record] attribute.
		/// </summary>
		/// <param name="type"></param>
		void RegisterType(Type type);

		/// <summary>
		/// Instruct the client to include a specific script. This script should not be an assembly script, for those use RegisterType().
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
		/// Register a control as a top level control. This means that its constructor will get called on page load.
		/// </summary>
		/// <param name="control">The top level control.</param>
		void RegisterTopLevelControl(IControl control);

		/// <summary>
		/// Add a startup script with lazy evaluation. The script will be retrieved just before it is actually rendered, allowing
		/// the use of startup scripts that depend on the final state of a control, as opposed to when the script is added.
		/// </summary>
		/// <param name="script">The startup script. This should be a valid JavaScript statement.</param>
		void AddStartupScript(Func<string> scriptRetriever);

		/// <summary>
		/// Add a startup script.
		/// </summary>
		/// <param name="script">The startup script. This should be a valid JavaScript statement.</param>
		void AddStartupScript(string script);

		/// <summary>
		/// Get all registered startup scripts. Each entry is a statement which should be executed on document load.
		/// </summary>
		/// <returns>The registered scripts</returns>
		IEnumerable<string> GetStartupScripts();
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
