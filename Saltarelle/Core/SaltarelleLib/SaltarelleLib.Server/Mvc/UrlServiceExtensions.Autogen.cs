using System;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace Saltarelle.Mvc {
	public static partial class UrlServiceExtensions {
		public static string GetTypedActionUrl<TController>(this IUrlService svc, Expression<Func<TController, Func<ActionResult>>> f) where TController : IController {
			return GetActionUrlImpl(svc, f);
		}

		public static string GetTypedActionUrl<TController, T1>(this IUrlService svc, Expression<Func<TController, Func<T1, ActionResult>>> f, T1 p1) where TController : IController {
			return GetActionUrlImpl(svc, f, p1);
		}

		public static string GetTypedActionUrl<TController, T1>(this IUrlService svc, Expression<Func<TController, Func<T1, ActionResult>>> f, Unspecified<T1>.ValueClass p1) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, ActionResult>>> f, T1 p1, T2 p2) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2);
		}

		public static string GetTypedActionUrl<TController, T1, T2>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2);
		}

		public static string GetTypedActionUrl<TController, T1, T2>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, T1 p1, T2 p2, T3 p3) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, T2 p2, T3 p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, p4);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, p4);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, p4);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, p4);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, p4);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, p4);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, p4);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, p4);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, (object)null);
		}

		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, (object)null);
		}

	}
}
