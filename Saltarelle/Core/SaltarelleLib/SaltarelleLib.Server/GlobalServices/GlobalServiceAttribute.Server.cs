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
}