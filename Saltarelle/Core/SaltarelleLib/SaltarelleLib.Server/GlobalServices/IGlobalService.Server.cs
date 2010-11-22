using System;

namespace Saltarelle {
	/// <summary>
	/// This interface must be implemented by all global services (interfaces which can be used with the <see cref="GlobalServices"/> class.
	/// </summary>
	public interface IGlobalService {
		/// <summary>
		/// Called after the service is created, but before it is returned to the requester. Do things such as load services that this service depends on, and add client scripts if the server should be available client side.
		/// </summary>
		void Setup();
		
		/// <summary>
		/// Returns the configuration object which is passed to the client instantiation of the service.
		/// This property will only be investigated for services which are to be transferred to the client.
		/// Services which do not have a clientside facet can safely throw a <see cref="NotImplementedException"/>.
		/// </summary>
		object ConfigObject { get; }
	}
}