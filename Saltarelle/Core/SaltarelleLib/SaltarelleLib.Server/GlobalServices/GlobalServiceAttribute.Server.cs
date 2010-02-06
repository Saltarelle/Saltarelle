using System;

namespace Saltarelle {
	/// <summary>
	/// Indicates that a class is a global service. A global service can optionally implement the IGlobalService interface
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class GlobalServiceAttribute : Attribute {
		public Type InterfaceType { get; private set; }

		public GlobalServiceAttribute(Type interfaceType) {
			this.InterfaceType = interfaceType;
		}
	}
	
	public interface IGlobalService {
		/// <summary>
		/// Called after the service is created, but before it is returned to the requester. Do things such as load services that this service depends on, and add client scripts if the server should be available client side.
		/// </summary>
		void Setup();
	}
}