using System;

namespace Saltarelle {
	/// <summary>
	/// This interface can be implemented by a control or service that wants to be notified just before the script manager renders its script tags.
	/// </summary>
	public interface IBeforeWriteScriptsCallback {
		/// <summary>
		/// This method is called when the script manager is about to render its scripts. Use it eg. if you have to add additional includes.
		/// </summary>
		void BeforeWriteScripts(IScriptManagerService scriptManager);
	}
}