using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Web.Routing;

namespace Saltarelle.Mvc {
	public static class Unspecified<T> {
		public class ValueClass { private ValueClass() {} }
		public static readonly ValueClass Value = null;
	}

	public static partial class UrlServiceExtensions {
		/// <summary>
		/// Get a template url to invoke a method on a type. The type can be either a type, in which case static methods can be invoked,
		/// or an interface, in which case it should be an interace retrievable from GlobalServices.GetService().
		/// </summary>
		/// <param name="type">The type in question</param>
		/// <returns>A url template, where {0} can be replaced by a method name, to invoke a method on this type</returns>
		public static string GetNonControllerActionUrlTemplate(this IUrlService svc, Type type) {
			return GetTypedActionUrl(svc, (DelegateController c) => c.Execute, type.FullName, "__METHOD__NAME__").Replace("__METHOD__NAME__", "{0}");
		}

		private static MethodInfo FindMethodInfo(Expression expr) {
			var outerCall = expr as LambdaExpression;
			if (outerCall == null)
				throw new ArgumentException("Bad usage.");
			var body = outerCall.Body;
			if (body.NodeType == ExpressionType.Convert || body.NodeType == ExpressionType.ConvertChecked)
				body = ((UnaryExpression)body).Operand;
			if (body.NodeType != ExpressionType.Call)
				throw new ArgumentException("Bad usage.");
			MethodCallExpression call = (MethodCallExpression)body;
			if (call.Method.DeclaringType != typeof(System.Delegate) || call.Method.Name != "CreateDelegate")
				throw new ArgumentException("Bad usage.");
			var miExpr = call.Arguments[2] as ConstantExpression;
			if (miExpr == null)
				throw new ArgumentException("Bad usage.");

			return (MethodInfo)miExpr.Value;
		}
		
		private static RouteValueDictionary GetRouteValuesFromParameters(MethodInfo mi, object[] parameters) {
			RouteValueDictionary result = new RouteValueDictionary();
			ParameterInfo[] formalParams = mi.GetParameters();
			if (parameters.Length != formalParams.Length)
				throw new ArgumentException("Bad usage.");
			for (int i = 0; i < parameters.Length; i++) {
				if (parameters[i] != null)
					result.Add(formalParams[i].Name, parameters[i]);
			}
			return result;
		}

		private static string GetActionUrlImpl(IUrlService svc, Expression expr, params object[] parameters) {
			MethodInfo mi = FindMethodInfo(expr);
			RouteValueDictionary values = GetRouteValuesFromParameters(mi, parameters);
			return svc.GetActionUrl(mi, values);
		}
	}
}
