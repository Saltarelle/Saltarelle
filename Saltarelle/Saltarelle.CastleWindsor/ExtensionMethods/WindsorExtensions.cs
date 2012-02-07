using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Saltarelle.CastleWindsor.ExtensionMethods {
	/// <summary>
	/// This class contains extension methods for CastleWinsor registration.
	/// </summary>
	public static class WindsorExtensions {
		/// <summary>
		/// Registers all plugins in an assembly.
		/// </summary>
		public static void RegisterPluginsFromAssembly(this IWindsorContainer container, Assembly assembly) {
			var pluginAttributes = new[] { typeof(NodeProcessorAttribute), typeof(TypedMarkupParserImplAttribute), typeof(UntypedMarkupParserImplAttribute) };
			container.Register(AllTypes.FromAssembly(assembly).Where(type => type.GetCustomAttributes(true).Any(a => pluginAttributes.Contains(a.GetType()))).LifestyleCustom<TrulyTransientLifestyle>());
		}

		/// <summary>
		/// Registers all plugins in an assembly.
		/// </summary>
		public static void RegisterPluginsFromAssemblyOf<T>(this IWindsorContainer container) {
			container.RegisterPluginsFromAssembly(typeof(T).Assembly);
		}

		/// <summary>
		/// Registers all controls in an assembly.
		/// </summary>
		public static void RegisterControlsFromAssembly(this IWindsorContainer container, Assembly assembly) {
			container.Register(AllTypes.FromAssembly(assembly).BasedOn<IControl>().WithServiceSelf().LifestyleCustom<TrulyTransientLifestyle>());
		}

		/// <summary>
		/// Registers all controls in an assembly.
		/// </summary>
		public static void RegisterControlsFromAssemblyOf<T>(this IWindsorContainer container) {
			container.RegisterControlsFromAssembly(typeof(T).Assembly);
		}
	}
}
