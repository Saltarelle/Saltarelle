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
	}
}