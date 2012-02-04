using System;

namespace Saltarelle.Ioc {
	/// <summary>
	/// This interface must be implemented by all global services that should be possible to use on the client.
	/// </summary>
	public interface IService {
		/// <summary>
		/// Returns the configuration object which is passed to the client instantiation of the service.
		/// </summary>
		object ConfigObject { get; }

		/// <summary>
		/// This method is called when the script manager is about to render its scripts. Use it eg. if you have to add additional includes.
		/// </summary>
		void BeforeWriteScripts(IScriptManagerService scriptManager);
	}
}