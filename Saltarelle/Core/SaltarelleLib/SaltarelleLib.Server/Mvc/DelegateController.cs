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

namespace Saltarelle.Mvc
{
	[ValidateInput(false)]
	[Authorize]
    public class DelegateController : Controller
    {
		private MethodInfo methodInfo;
		private OutputCacheAttribute cacheAttrib;
    
		protected override void OnActionExecuting(ActionExecutingContext filterContext) {
			base.OnActionExecuting(filterContext);

			if (filterContext.ActionDescriptor.ActionName == "Execute") {
				string typeName   = (string)filterContext.ActionParameters["typeName"],
				       methodName = (string)filterContext.ActionParameters["method"];
				Type t = Utils.FindType(typeName);
				if (t.IsInterface) {
					methodInfo = t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
					if (methodInfo == null)
						throw new ArgumentException(string.Format("Could not find the method {0} of the interface {1}.", methodName, typeName));
				}
				else {
					methodInfo = t.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
					if (methodInfo == null)
						throw new ArgumentException(string.Format("Could not find the static method {0} of the class {1}.", methodName, typeName));
				}
			}

			if (methodInfo != null) {
				AcceptVerbsAttribute ava = (AcceptVerbsAttribute)methodInfo.GetCustomAttributes(typeof(AcceptVerbsAttribute), false).SingleOrDefault();
				if (ava == null)
					throw new ArgumentException(string.Format("The method {0} of the {1} {2} is not decorated with an AcceptVerbsAttribute.", methodInfo.Name, methodInfo.DeclaringType.IsInterface ? "interface" : "class", methodInfo.DeclaringType.FullName));
				if (!ava.IsValidForRequest(filterContext, methodInfo))
					throw new ArgumentException(string.Format("The method {0} of the {1} {2} does not support this HTTP verb.", methodInfo.Name, methodInfo.DeclaringType.IsInterface ? "interface" : "class", methodInfo.DeclaringType.FullName));
				cacheAttrib = (OutputCacheAttribute)methodInfo.GetCustomAttributes(typeof(OutputCacheAttribute), false).SingleOrDefault();
			}
			if (cacheAttrib != null)
				cacheAttrib.OnActionExecuting(filterContext);
		}

		protected override void OnActionExecuted(ActionExecutedContext filterContext) {
			base.OnActionExecuted(filterContext);
			if (cacheAttrib != null)
				cacheAttrib.OnActionExecuted(filterContext);
		}

		protected override void OnResultExecuting(ResultExecutingContext filterContext) {
			base.OnResultExecuting(filterContext);
			if (cacheAttrib != null)
				cacheAttrib.OnResultExecuting(filterContext);
		}

		protected override void OnResultExecuted(ResultExecutedContext filterContext) {
			base.OnResultExecuted(filterContext);
			if (cacheAttrib != null)
				cacheAttrib.OnResultExecuted(filterContext);
		}
		
		public ActionResult Execute(string typeName, string method) {
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

			if (result == null || !typeof(ActionResult).IsAssignableFrom(result.GetType()))
				result = JavaScript(Utils.Json(result));

			return (ActionResult) result;
		}
	}
}
