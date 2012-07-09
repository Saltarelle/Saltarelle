using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DemoWeb.Plugins;
using DemoWeb.Webapp.Controllers;
using Saltarelle;
using Saltarelle.CastleWindsor.ExtensionMethods;
using Saltarelle.Configuration;
using Saltarelle.Mvc;
using Saltarelle.UI;

namespace DemoWeb.Webapp {
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication {
		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			
			Saltarelle.Mvc.MvcRouteService.RegisterRoutes(routes);

			routes.MapRoute(
				"HomeShortcut",              // Route name
				"{action}",                  // URL with parameters
				new { controller = "Home" }  // Parameter defaults
			);

			routes.MapRoute(
				"Default",                                              // Route name
				"{controller}/{action}/{id}",                           // URL with parameters
				new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
			);

		}

		public static void RegisterSaltarelleCoreServices(IWindsorContainer container, SaltarelleConfig saltarelleConfig) {
			container.Register(Component.For<Saltarelle.IRouteService>().ImplementedBy<Saltarelle.Mvc.MvcRouteService>().LifeStyle.Singleton,
							   Component.For<Saltarelle.IModuleUtils>().ImplementedBy<Saltarelle.Mvc.ModuleUtils>().LifeStyle.Singleton,
			                   Component.For<Saltarelle.Ioc.IContainer>().UsingFactoryMethod(() => Saltarelle.CastleWindsor.ContainerFactory.CreateContainer(container)).LifestylePerWebRequest(),
							   Component.For<Saltarelle.IScriptManagerService>().UsingFactoryMethod(() => new Saltarelle.DefaultScriptManagerService(container.Resolve<IRouteService>(), container.Resolve<IModuleUtils>(), saltarelleConfig)).LifeStyle.PerWebRequest,
							   Component.For<Saltarelle.Mvc.SaltarelleController>().LifeStyle.PerWebRequest,
							   Component.For<Saltarelle.UI.ISaltarelleUIService>().ImplementedBy<Saltarelle.UI.DefaultSaltarelleUIService>().LifeStyle.Singleton
							  );
		}

		protected void Application_Start() {
			var container = new WindsorContainer();
			Saltarelle.CastleWindsor.ContainerFactory.PrepareWindsorContainer(container);

			RegisterSaltarelleCoreServices(container, SaltarelleConfig.GetFromWebConfig());
			container.RegisterPluginsFromAssembly(typeof(CopyrightNodeProcessor).Assembly);
			container.RegisterControlsFromAssembly(typeof(Lesson1Control).Assembly);
			container.RegisterControlsFromAssembly(typeof(Label).Assembly);
			container.Register(AllTypes.FromAssemblyContaining<HomeController>().BasedOn<IController>().WithService.Self().LifestylePerWebRequest());

			RegisterRoutes(RouteTable.Routes);

			DependencyResolver.SetResolver(container.Resolve, s => (object[])container.ResolveAll(s));
		}
	}
}