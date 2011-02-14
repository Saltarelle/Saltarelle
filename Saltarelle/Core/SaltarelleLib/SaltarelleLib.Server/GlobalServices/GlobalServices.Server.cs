using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Threading;
using System.Configuration;
using System.Reflection;

namespace Saltarelle
{
	public static class GlobalServices {
		private static IGlobalServicesProvider provider;

		public static IGlobalServicesProvider Provider { get { return provider; } }

		public static void Init(IGlobalServicesProvider provider) {
			IDisposable d = GlobalServices.provider as IDisposable;
			if (!Utils.IsNull(d))
				d.Dispose();
			GlobalServices.provider    = provider;
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

		public static void LoadService<IService>() {
			provider.LoadService(typeof(IService));
		}

		public static void LoadServiceExplicit<IService, TImplementer>(TImplementer implementer) where TImplementer : IService {
			provider.LoadServiceExplicit(typeof(IService), implementer);
		}

		public static IService GetService<IService>() {
			return (IService)provider.GetService(typeof(IService));
		}
		
		public static bool HasService<IService>() {
			return provider.HasService(typeof(IService));
		}
		
		public static IEnumerable<KeyValuePair<Type, object>> AllLoadedServices {
			get { return provider.AllLoadedServices; }
		}
	}
}
