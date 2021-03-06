﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;
using System.Reflection;
using System.Linq.Expressions;
using System.Web;
using Saltarelle.Configuration;
using System.Web.Configuration;
using System.Configuration;

namespace Saltarelle.Mvc {
	public class MvcRouteService : IRouteService {
		public const string AssemblyScriptsRouteName   = "AssemblyScripts";
		public const string AssemblyCssRouteName       = "AssemblyCss";
		public const string AssemblyResourcesRouteName = "AssemblyResources";
		public const string DelegateRouteName          = "Delegate";
		
		private static Func<RouteValueDictionary, string> assemblyVersionDependency =
			vals => {
				Assembly asm;
				string name = (string)vals[SaltarelleController.AssemblyNameParam];
				if (name != null && Utils.TryFindAssembly(name, out asm)) {
					try {
						var v = new Version(FileVersionInfo.GetVersionInfo(asm.Location).ProductVersion);
						if (v.Major > 0 || v.Minor > 0 || v.Build > 0 || v.Revision > 0)
							return v.ToString();
					}
					catch (Exception) {	// Assembly methods have a habit of failing randomly (eg. if the assembly is dynamic).
					}

					try {
						var v = asm.GetName().Version;
						if (v != null && v.Major > 0 || v.Minor > 0 || v.Build > 0 || v.Revision > 0)
							return v.ToString();
					}
					catch (Exception) {
					}
					return null;
				}
				return null;
			};

		private static MethodInfo MethodInfo<T1, T2>(Expression<Func<T1, T2>> expr) {
			return ((MethodCallExpression)expr.Body).Method;
		}
		
		private static string RemoveControllerSuffix(string s) {
			return s == null || !s.EndsWith("Controller") ? s : s.Substring(0, s.Length - "Controller".Length);
		}
		
		private static void RegisterSingle<T1, T2>(RouteCollection routes, string routeName, string routeUrl, Expression<Func<T1, T2>> action, IDictionary<string, Func<RouteValueDictionary, string>> dependencies) {
			MethodInfo mi = ((MethodCallExpression)action.Body).Method;
			var defaults   = new RouteValueDictionary() { { "controller", RemoveControllerSuffix(mi.DeclaringType.Name) }, { "action", mi.Name } };
			var dataTokens = new RouteValueDictionary() { { "Namespaces", mi.DeclaringType.Namespace } };

			if (dependencies != null && dependencies.Count > 0)
				routes.Add(routeName, new VersionedRoute(routeUrl, defaults, new RouteValueDictionary(), dataTokens, dependencies, new MvcRouteHandler()));
			else
				routes.Add(routeName, new Route(routeUrl, defaults, new RouteValueDictionary(), dataTokens, new MvcRouteHandler()));
		}

		public static void RegisterRoutes(RouteCollection routes) {
			var config = SaltarelleConfig.GetFromWebConfig();
			if (config == null)
				throw new ConfigurationErrorsException("The <saltarelle> section is missing from web.config.");
			bool debugScripts = config.Scripts.Debug;

			if (string.IsNullOrEmpty(config.Routes.AssemblyScripts))
				throw new ConfigurationErrorsException("The saltarelle/routes/@assemblyScripts configuration attribute must be specified.");
			if (!config.Routes.AssemblyScripts.Contains("{" + SaltarelleController.AssemblyNameParam + "}"))
				throw new ConfigurationErrorsException("The saltarelle/routes/@assemblyScripts configuration attribute must contain the route value placeholder {" + SaltarelleController.AssemblyNameParam + "}.");

			if (string.IsNullOrEmpty(config.Routes.AssemblyCss))
				throw new ConfigurationErrorsException("The saltarelle/routes/@assemblyCss configuration attribute must be specified.");
			if (!config.Routes.AssemblyCss.Contains("{" + SaltarelleController.AssemblyNameParam + "}"))
				throw new ConfigurationErrorsException("The saltarelle/routes/@assemblyCss configuration attribute must contain the route value placeholder {" + SaltarelleController.AssemblyNameParam + "}.");

			if (string.IsNullOrEmpty(config.Routes.AssemblyResources))
				throw new ConfigurationErrorsException("The saltarelle/routes/@assemblyResources configuration attribute must be specified.");
			if (!config.Routes.AssemblyResources.Contains("{" + SaltarelleController.AssemblyNameParam + "}") || !config.Routes.AssemblyResources.Contains("{version}") || !config.Routes.AssemblyResources.Contains("{*" + SaltarelleController.ResourceNameParam + "}"))
				throw new ConfigurationErrorsException("The saltarelle/routes/@assemblyResources configuration attribute must contain the route value placeholders {" + SaltarelleController.AssemblyNameParam + "}, {version} and {*" + SaltarelleController.ResourceNameParam + "}.");

			if (string.IsNullOrEmpty(config.Routes.Delegate))
				throw new ConfigurationErrorsException("The saltarelle/routes/@delegate configuration attribute must be specified.");
			if (!config.Routes.Delegate.Contains("{" + SaltarelleController.DelegateTypeNameParam + "}") || !config.Routes.Delegate.Contains("{" + SaltarelleController.DelegateMethodParam + "}"))
				throw new ConfigurationErrorsException("The saltarelle/routes/@delegate configuration attribute must contain the route value placeholders {" + SaltarelleController.DelegateTypeNameParam + "} and {" + SaltarelleController.DelegateMethodParam + "}.");

			RegisterSingle(routes, AssemblyScriptsRouteName,   config.Routes.AssemblyScripts,   (SaltarelleController c) => c.GetAssemblyScript(null, null),   debugScripts ? new Dictionary<string, Func<RouteValueDictionary, string>>() { { "version", assemblyVersionDependency }, { SaltarelleController.ScriptDebugParam, _ => "1" } } : new Dictionary<string, Func<RouteValueDictionary, string>>() { { "version", assemblyVersionDependency } });
			RegisterSingle(routes, AssemblyCssRouteName,       config.Routes.AssemblyCss,       (SaltarelleController c) => c.GetAssemblyCss(null),            new Dictionary<string, Func<RouteValueDictionary, string>>() { { "version", assemblyVersionDependency } });
			RegisterSingle(routes, AssemblyResourcesRouteName, config.Routes.AssemblyResources, (SaltarelleController c) => c.GetAssemblyResource(null, null), new Dictionary<string, Func<RouteValueDictionary, string>>() { { "version", assemblyVersionDependency } });
			RegisterSingle(routes, DelegateRouteName,          config.Routes.Delegate,          (SaltarelleController c) => c.Delegate(null, null), null);
		}
		
		public string GetVirtualPath(string routeName, IDictionary<string, object> routeValues) {
            var rvd = (routeValues != null ? new RouteValueDictionary(routeValues) : new RouteValueDictionary());
			return RouteTable.Routes.GetVirtualPath(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()), routeName, rvd).VirtualPath;
		}
		
		public string GetVirtualPath(string routeName) {
			return GetVirtualPath(routeName, null);
		}

		public string GetAssemblyResourceUrl(Assembly asm, string resourcePublicName) {
			return GetVirtualPath(AssemblyResourcesRouteName, new RouteValueDictionary() { { SaltarelleController.AssemblyNameParam, asm.GetName().Name }, { SaltarelleController.ResourceNameParam, resourcePublicName } });
		}

		public string GetAssemblyScriptUrl(Assembly asm) {
			return GetVirtualPath(AssemblyScriptsRouteName, new RouteValueDictionary() { { SaltarelleController.AssemblyNameParam, asm.GetName().Name } });
		}

		public string GetAssemblyCssUrl(Assembly asm) {
			return GetVirtualPath(AssemblyCssRouteName, new RouteValueDictionary() { { SaltarelleController.AssemblyNameParam, asm.GetName().Name } });
		}

		public string GetDelegateUrl(MethodInfo mi) {
			return GetVirtualPath(DelegateRouteName, new RouteValueDictionary() { { SaltarelleController.DelegateTypeNameParam, mi.DeclaringType.FullName }, { SaltarelleController.DelegateMethodParam, mi.Name } });
		}
		
		public string GetDelegateUrlTemplate(Type type) {
			string ph = "__METHOD_NAME__PH_";
			return GetVirtualPath(DelegateRouteName, new RouteValueDictionary() { { SaltarelleController.DelegateTypeNameParam, type.FullName }, { SaltarelleController.DelegateMethodParam, ph } }).Replace(ph, "{0}");
		}

		public string GetDelegateUrl<T>(Expression<Func<T>> method) {
			return GetDelegateUrl(((MethodCallExpression)method.Body).Method);
		}

		public RouteData CurrentRoute { get; set; }
	}
}
