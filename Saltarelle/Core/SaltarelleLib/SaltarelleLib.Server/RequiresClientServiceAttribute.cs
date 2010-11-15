using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltarelle {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
	public sealed class RequiresClientServiceAttribute : Attribute {
		public Type ServiceType { get; private set; }
		
		public RequiresClientServiceAttribute(Type serviceType) {
			this.ServiceType = serviceType;
		}
	}
}
