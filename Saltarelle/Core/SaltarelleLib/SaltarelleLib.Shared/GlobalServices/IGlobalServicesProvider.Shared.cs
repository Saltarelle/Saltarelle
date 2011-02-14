using System;
#if SERVER
using System.Collections.Generic;
#endif

namespace Saltarelle {
	public interface IGlobalServicesProvider {
		object GetService(Type serviceType);
		void LoadService(Type serviceType);
		void LoadServiceExplicit(Type serviceType, object implementer);
		bool HasService(Type serviceType);

		#if SERVER
			IEnumerable<KeyValuePair<Type, object>> AllLoadedServices { get; }
		#endif
	}

	#if SERVER
	public static class GlobalServicesProviderExtensions {
		public static IService GetService<IService>(this IGlobalServicesProvider provider) {
			return (IService)provider.GetService(typeof(IService));
		}
		public static void LoadService<IService>(this IGlobalServicesProvider provider) {
			provider.LoadService(typeof(IService));
		}
		public static bool HasService<IService>(this IGlobalServicesProvider provider) {
			return provider.HasService(typeof(IService));
		}
		public static void LoadServiceExplicit<IService, TImplementer>(this IGlobalServicesProvider provider, TImplementer implementer) where TImplementer : IService {
			provider.LoadServiceExplicit(typeof(IService), implementer);
		}
	}
	#endif
}