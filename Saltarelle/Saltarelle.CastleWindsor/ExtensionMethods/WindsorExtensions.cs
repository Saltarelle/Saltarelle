using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;

namespace Saltarelle.CastleWindsor.ExtensionMethods {
	public static class WindsorExtensions {
		/// <summary>
		/// Registers all plugins in an assembly.
		/// </summary>
		public static BasedOnDescriptor RegisterPlugins(this FromAssemblyDescriptor descriptor) {
			return descriptor.BasedOn<INodeProcessor>().WithServiceSelf().LifestyleCustom<TrulyTransientLifestyle>();
		}

		/// <summary>
		/// Registers all controls in an assembly.
		/// </summary>
		public static BasedOnDescriptor RegisterControls(this FromAssemblyDescriptor descriptor) {
			return descriptor.BasedOn<IControl>().WithServiceSelf().LifestyleCustom<TrulyTransientLifestyle>();
		}
	}
}
