using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CreatedServicePair = System.Collections.Generic.KeyValuePair<System.Type, object>;

namespace Saltarelle {
	public class DefaultGlobalServicesModule : IHttpModule {
		private const string CreatedServicesKey = "{EF8495B3-D241-4b32-AA39-B34D09F3E7E6}";
		private object padlock = new object();

		private class DefaultGlobalServicesProvider : IGlobalServicesProvider {
			private IDictionary<Type, Type> serviceProviders;

			public DefaultGlobalServicesProvider(Dictionary<Type, Type> serviceProviders) {
				this.serviceProviders = serviceProviders;
			}
		
			private object GetService(Type serviceType, bool allowLoad) {
				if (serviceType == null || !serviceType.IsInterface)
					throw new ArgumentException("serviceType must be an interface");
				IList<CreatedServicePair> registered = (IList<CreatedServicePair>)HttpContext.Current.Items[CreatedServicesKey];
				if (registered == null)
					HttpContext.Current.Items[CreatedServicesKey] = registered = new List<CreatedServicePair>();
				CreatedServicePair regEntry = registered.FirstOrDefault(x => x.Key == serviceType);
				if (regEntry.Value != null)
					return regEntry.Value;
				
				if (!allowLoad)
					return null;

				Type classType;
				if (!serviceProviders.TryGetValue(serviceType, out classType))
					throw new InvalidOperationException("There is no type that implements the interface " + serviceType.FullName);

				object implementer = Activator.CreateInstance(classType);
				regEntry = new CreatedServicePair(serviceType, implementer);
				var igs = implementer as IGlobalService;
				if (igs != null) {
					// Kind of a hack: The service must be registered during the call to its own Setup method, however, later we must make sure that dependencies will appear in the correct order.
					int index = registered.Count;
					registered.Add(regEntry);
					igs.Setup();
					registered.RemoveAt(index);
				}
				registered.Add(regEntry);
				return implementer;
			}
			
			public object GetService(Type serviceType) {
				return GetService(serviceType, true);
			}

			public void LoadService(Type serviceType) {
				GetService(serviceType, true);
			}
			
			public bool HasService(Type serviceType) {
				return GetService(serviceType, false) != null;
			}
			
			public IEnumerable<KeyValuePair<Type, object>> AllLoadedServices {
				get {
					var l = (IList<CreatedServicePair>)HttpContext.Current.Items[CreatedServicesKey];
					if (l != null) {
						foreach (var x in l)
							yield return x;
					}
				}
			}
		}

		public void Init(HttpApplication context) {
			context.BeginRequest += Application_BeginRequest;
			context.EndRequest   += Application_EndRequest;
			
			if (GlobalServices.Provider == null) {
				lock (padlock) {
					// The Init event seems to fire more than once sometimes, so we might get errors during startup unless we do this.
					if (GlobalServices.Provider == null) {
						var serviceProviders = (  from asm in AppDomain.CurrentDomain.GetAssemblies()
						                          from tp in asm.GetTypes()
						                           let attr = (GlobalServiceAttribute)tp.GetCustomAttributes(typeof(GlobalServiceAttribute), false).FirstOrDefault()
						                         where attr != null
						                        select new { svcInterface = attr.InterfaceType, implementer = tp }
						                       ).ToDictionary(x => x.svcInterface, x => x.implementer);
						GlobalServices.Init(new DefaultGlobalServicesProvider(serviceProviders));
					}
				}
			}
		}

		void Application_BeginRequest(object sender, EventArgs e) {
		}

		void Application_EndRequest(object sender, EventArgs e) {
			IList<CreatedServicePair> list = (IList<CreatedServicePair>)HttpContext.Current.Items[CreatedServicesKey];
			if (list != null) {
				for (int i = list.Count - 1; i >= 0; i--) {
					var igs = list[i].Value as IDisposable;
					if (igs != null)
						igs.Dispose();
				}
			}
		}

		public void Dispose() {
		}
	}
}
