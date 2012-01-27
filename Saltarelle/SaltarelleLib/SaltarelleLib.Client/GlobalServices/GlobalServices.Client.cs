using System;

namespace Saltarelle
{
	public static class GlobalServices {
		private static ClientGlobalServicesProvider provider = new ClientGlobalServicesProvider();
		public static IGlobalServicesProvider Provider { get { return provider; } }

		public static void SetService(Type serviceType, object implementer) {
			provider.SetService(serviceType, implementer);
		}
		
		public static void LoadService(Type serviceType) {
			provider.LoadService(serviceType);
		}

		public static object GetService(Type serviceType) {
			return provider.GetService(serviceType);
		}
		
		public static bool HasService(Type serviceType) {
			return provider.HasService(serviceType);
		}
	}
	
	public class ClientGlobalServicesProvider : IGlobalServicesProvider {
		private Dictionary services = new Dictionary();

		public void SetService(Type serviceType, object implementer) {
			services[serviceType.FullName] = implementer;
		}
		
		public void LoadService(Type serviceType) {
			if (!services.ContainsKey(serviceType.FullName))
				throw new Exception("The service " + serviceType.FullName + " was not loaded.");
		}

		public void LoadServiceExplicit(Type serviceType, object implementer) {
			if (services.ContainsKey(serviceType.FullName))
				throw new Exception("The service " + serviceType.FullName + " has already been loaded.");
			services[serviceType.FullName] = implementer;
		}

		public object GetService(Type serviceType) {
			if (!services.ContainsKey(serviceType.FullName))
				throw new Exception("The service " + serviceType.FullName + " was not loaded.");
			return services[serviceType.FullName];
		}
		
		public bool HasService(Type serviceType) {
			return services.ContainsKey(serviceType.FullName);
		}
	}
}
