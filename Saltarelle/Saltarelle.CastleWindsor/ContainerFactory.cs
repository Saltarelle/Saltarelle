using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Saltarelle.Ioc;

namespace Saltarelle.CastleWindsor {
	/// <summary>
	/// This class contains methods to create and <see cref="IContainer"/> powered by Castle Windsor.
	/// </summary>
	public class ContainerFactory {
		private static readonly ConcurrentDictionary<string, Type> _typeCache = new ConcurrentDictionary<string, Type>();

		private static Type TryFindType(string typeName) {
			Type result = null;
			if (!_typeCache.TryGetValue(typeName, out result)) {
				foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
					result = a.GetType(typeName);
					if (result != null)
						break;
				}
				if (result != null)
					_typeCache[typeName] = result; // Don't cache if the type was not found, as it should be rare and its assembly might be loaded later.
			}
			return result;
		}

		private static Type FindType(string typeName) {
			var result = TryFindType(typeName);
			if (result == null)
				throw new ArgumentException("Type " + typeName + " does not exist.");
			return result;
		}

		/// <summary>
		/// Create a container based on a Castle Windsor container.
		/// </summary>
		/// <param name="underlyingContainer">Underlying Winsor container to resolve components.</param>
		public static IContainer CreateContainer(IWindsorContainer underlyingContainer) {
			if (!(underlyingContainer.Kernel.ReleasePolicy is TrulyTransientReleasePolicy))
				throw new ArgumentException("The underlying container seems not to have been prepared by PrepareWindsorContainer().", "underlyingContainer");
			return new DefaultContainer(typeName => FindType(typeName), serviceType => underlyingContainer.Resolve(serviceType), objectType => underlyingContainer.Resolve(objectType));
		}

		/// <summary>
		/// Prepares a windsor container so it can be used in the <see cref="CreateContainer"/> method. This will change the release policy for the container.
		/// </summary>
		public static void PrepareWindsorContainer(IWindsorContainer container) {
			container.Kernel.ComponentCreated += (model, instance) => {
				var nc = instance as INotifyCreated;
				if (nc != null)
					nc.DependenciesAvailable();
			};
			container.Kernel.ReleasePolicy = new TrulyTransientReleasePolicy(container.Kernel);
		}
	}
}
