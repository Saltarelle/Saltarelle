using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Reflection;
using System.ComponentModel;
using System.Web.UI;
using System.Globalization;
using System.Web.Routing;
using System.DirectoryServices;
using Microsoft.Win32;
using System.IO;

namespace Saltarelle.Mvc
{
	[ValidateInput(false)]
    public class SaltarelleController : Controller {
		public const string DelegateTypeNameParam = "typeName";
		public const string DelegateMethodParam   = "method";
		public const string AssemblyNameParam     = "assemblyName";
		public const string ResourceNameParam     = "resourceName";
		public const string ScriptDebugParam      = "debug";
    
		private static Dictionary<string, string> mimeMap;

		static SaltarelleController() {
			try {
				using (var entry = new DirectoryEntry("IIS://localhost/MimeMap")) {
					PropertyValueCollection properties = entry.Properties["MimeMap"];
					mimeMap = properties.Cast<object>().ToDictionary(x => ((string)x.GetType().InvokeMember("Extension", BindingFlags.GetProperty, null, x, null)).ToLowerInvariant(), x => (string)x.GetType().InvokeMember("MimeType", BindingFlags.GetProperty, null, x, null));
				}
			}
			catch (Exception) {
				// Something is probably wrong with IIS (not installed/not working/whatever. No big deal, just use the map from the registry instead if developing.
				if (System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName.ToLowerInvariant() == "webdev.webserver.exe") {
					mimeMap = (  from key in Registry.ClassesRoot.GetSubKeyNames()
					            where key.StartsWith(".")
					              let value = Registry.GetValue("HKEY_CLASSES_ROOT\\" + key, "Content Type", null) as string
					            where !string.IsNullOrEmpty(value)
					           select new { key, value }
					          ).ToDictionary(x => x.key.ToLowerInvariant(), x => x.value);
				}
				else
					throw;
			}
		}

		private MethodInfo methodInfo;
		private FilterInfo methodFilters;
		
		private void InitializeDelegateData(AuthorizationContext filterContext) {
			string typeName   = (string)filterContext.RouteData.Values[DelegateTypeNameParam],
			       methodName = (string)filterContext.RouteData.Values[DelegateMethodParam];
			Type t = Utils.FindType(typeName);
			var candidates = (  from mi in t.GetMethods((t.IsInterface ? BindingFlags.Instance : BindingFlags.Static) | BindingFlags.Public)
			                   where mi.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase)
			                      && mi.GetCustomAttributes(typeof(AcceptVerbsAttribute), true).Cast<AcceptVerbsAttribute>().Any(attr => attr.IsValidForRequest(filterContext, mi))
			                  select mi
			                 ).ToList();
			if (candidates.Count > 1)
				throw new ArgumentException("More than one matching method");
			else if (candidates.Count == 0)
				throw new ArgumentException("No matching method. If the type is an interface, the method must be an instance method, if it's a class, it must be a public static method. The method must also be decorated with a suitable AcceptVerbsAttribute.");

			methodInfo = candidates[0];

			object[] customAttributes = methodInfo.GetCustomAttributes(true).ToArray();
			methodFilters = new FilterInfo();
			foreach (var a in customAttributes.OfType<IAuthorizationFilter>()) methodFilters.AuthorizationFilters.Add(a);
			foreach (var a in customAttributes.OfType<IActionFilter>()) methodFilters.ActionFilters.Add(a);
			foreach (var a in customAttributes.OfType<IResultFilter>()) methodFilters.ResultFilters.Add(a);
			foreach (var a in customAttributes.OfType<IExceptionFilter>()) methodFilters.ExceptionFilters.Add(a);
		}

		protected override void OnAuthorization(AuthorizationContext filterContext) {
			if ((string)filterContext.RouteData.Values["action"] == "Delegate") {
				InitializeDelegateData(filterContext);
				foreach (var f in methodFilters.AuthorizationFilters) {
					f.OnAuthorization(filterContext);
					if (!Utils.IsNull(filterContext.Result))
						return;
				}
			}
			else
				methodFilters = new FilterInfo();
			base.OnAuthorization(filterContext);
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext) {
			foreach (var f in methodFilters.ActionFilters) {
				f.OnActionExecuting(filterContext);
				if (!Utils.IsNull(filterContext.Result))
					return;
			}
			base.OnActionExecuting(filterContext);
		}

		protected override void OnActionExecuted(ActionExecutedContext filterContext) {
			foreach (var f in methodFilters.ActionFilters) {
				f.OnActionExecuted(filterContext);
				if (!Utils.IsNull(filterContext.Result))
					return;
			}
			base.OnActionExecuted(filterContext);
		}

		protected override void OnResultExecuting(ResultExecutingContext filterContext) {
			foreach (var f in methodFilters.ResultFilters) {
				f.OnResultExecuting(filterContext);
				if (!Utils.IsNull(filterContext.Result))
					return;
			}
			base.OnResultExecuting(filterContext);
		}

		protected override void OnResultExecuted(ResultExecutedContext filterContext) {
			foreach (var f in methodFilters.ResultFilters) {
				f.OnResultExecuted(filterContext);
				if (!Utils.IsNull(filterContext.Result))
					return;
			}
			base.OnResultExecuted(filterContext);
		}

		protected override void OnException(ExceptionContext filterContext) {
			foreach (var f in methodFilters.ExceptionFilters) {
				f.OnException(filterContext);
				if (!Utils.IsNull(filterContext.Result))
					return;
			}
			base.OnException(filterContext);
		}

		public ActionResult Delegate(string typeName, string method) {
			ParameterInfo[] pis = methodInfo.GetParameters();
			object[] parms = new object[pis.Length];
			for (int i = 0; i < pis.Length; i++) {
				string str = Request[pis[i].Name];
				if (string.IsNullOrEmpty(str)) {
					if (pis[i].ParameterType.IsClass || (pis[i].ParameterType.IsGenericType && pis[i].ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>)))
						parms[i] = null;
					else
						throw new ArgumentNullException(pis[i].Name);
				}
				else {
					Type tp = pis[i].ParameterType;
					if (tp.IsGenericType && tp.GetGenericTypeDefinition() == typeof(Nullable<>))
						tp = tp.GetGenericArguments()[0];
					if (tp == typeof(string))
						parms[i] = str;
					else if (tp == typeof(byte))
						parms[i] = byte.Parse(str, NumberFormatInfo.InvariantInfo);
					else if (tp == typeof(short))
						parms[i] = short.Parse(str, NumberFormatInfo.InvariantInfo);
					else if (tp == typeof(int))
						parms[i] = int.Parse(str, NumberFormatInfo.InvariantInfo);
					else if (tp == typeof(long))
						parms[i] = long.Parse(str, NumberFormatInfo.InvariantInfo);
					else if (tp == typeof(float))
						parms[i] = float.Parse(str, NumberFormatInfo.InvariantInfo);
					else if (tp == typeof(double))
						parms[i] = double.Parse(str, NumberFormatInfo.InvariantInfo);
					else if (tp == typeof(bool))
						parms[i] = Utils.ParseBool(str);
					else
						parms[i] = Utils.EvalJson(str, pis[i].ParameterType);
				}
			}
			
			object instance = methodInfo.DeclaringType.IsInterface ? GlobalServices.GetService(methodInfo.DeclaringType) : null;
			object result;
			if (methodInfo.ReturnType == typeof(void)) {
				methodInfo.Invoke(instance, parms);
				result = true;
			}
			else {
				result = methodInfo.Invoke(instance, parms);
			}

			if (Utils.IsNull(result) || !typeof(ActionResult).IsAssignableFrom(result.GetType()))
				result = JavaScript(Utils.Json(result));

			return (ActionResult)result;
		}
		
		[OutputCache(Duration=int.MaxValue, Location=OutputCacheLocation.Any, VaryByParam="*")]
		public ActionResult GetAssemblyScript(string assemblyName, string debug) {
			Assembly asm;
			if (Utils.TryFindAssembly(assemblyName, out asm)) {
				string s = ModuleUtils.GetAssemblyScriptContent(asm, !string.IsNullOrEmpty(debug));
				if (!Utils.IsNull(s))
					return JavaScript(s);
			}
			throw new HttpException(404, "File not found");
		}

		[OutputCache(Duration=int.MaxValue, Location=OutputCacheLocation.Any, VaryByParam="*")]
		public ActionResult GetAssemblyCss(string assemblyName) {
			Assembly asm;
			if (Utils.TryFindAssembly(assemblyName, out asm)) {
				string s = ModuleUtils.GetAssemblyCss(asm);
				if (!Utils.IsNull(s))
					return Content(s, "text/css");
			}
			throw new HttpException(404, "File not found");
		}

		[OutputCache(Duration=int.MaxValue, Location=OutputCacheLocation.Any, VaryByParam="*")]
		public ActionResult GetAssemblyResource(string assemblyName, string resourceName) {
			Assembly asm;
			if (Utils.TryFindAssembly(assemblyName, out asm)) {
				WebResourceAttribute attr = asm.GetCustomAttributes(typeof(WebResourceAttribute), false).Cast<WebResourceAttribute>().SingleOrDefault(x => x.PublicResourceName == resourceName);
				if (!Utils.IsNull(attr)) {
					string mime;
					mimeMap.TryGetValue(Path.GetExtension(resourceName).ToLowerInvariant(), out mime);

					return File(asm.GetManifestResourceStream(attr.ResourceQualifiedName), mime ?? "application/octet-stream");
				}
			}
			throw new HttpException(404, "File not found");
		}
	}
}
