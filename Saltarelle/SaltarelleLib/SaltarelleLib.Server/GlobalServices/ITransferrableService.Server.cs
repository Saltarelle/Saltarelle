using System;

namespace Saltarelle {
	/// <summary>
	/// This interface must be implemented by all global services that should be possible to transfer to the client.
	/// </summary>
	public interface ITransferrableService {
		/// <summary>
		/// Returns the configuration object which is passed to the client instantiation of the service.
		/// </summary>
		object ConfigObject { get; }
	}
}