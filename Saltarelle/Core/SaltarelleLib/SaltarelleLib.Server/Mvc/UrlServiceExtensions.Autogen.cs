using System;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.ComponentModel;

namespace Saltarelle.Mvc {
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static partial class UrlServiceExtensions {
		public delegate TResult Func<T1, T2, T3, T4, T5, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
		public delegate TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController>(this IUrlService svc, Expression<Func<TController, Func<ActionResult>>> f) where TController : IController {
			return GetActionUrlImpl(svc, f);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1>(this IUrlService svc, Expression<Func<TController, Func<T1, ActionResult>>> f, T1 p1) where TController : IController {
			return GetActionUrlImpl(svc, f, p1);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1>(this IUrlService svc, Expression<Func<TController, Func<T1, ActionResult>>> f, Unspecified<T1>.ValueClass p1) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, ActionResult>>> f, T1 p1, T2 p2) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, T1 p1, T2 p2, T3 p3) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, T2 p2, T3 p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, p4);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, p4);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, p4);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, p4);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, p4);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, p4);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, p4);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, p4);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, p4, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, T4 p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, p4, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, p4, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, p4, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, p4, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, p4, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, p4, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, p4, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, (object)null, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, (object)null, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, (object)null, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, (object)null, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, (object)null, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, (object)null, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, (object)null, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, T5 p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, (object)null, p5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, T2 p2, T3 p3, T4 p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, p4, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, T4 p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, p4, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, p4, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, p4, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, p4, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, p4, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, p4, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, p4, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, p4, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, p4, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, p4, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, p4, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, p4, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, p4, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, p4, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, p4, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, (object)null, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, (object)null, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, (object)null, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, (object)null, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, (object)null, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, (object)null, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, (object)null, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, T5 p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, (object)null, p5, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, T3 p3, T4 p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, p4, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, T4 p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, p4, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, p4, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, p4, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, p4, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, p4, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, p4, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, p4, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, (object)null, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, (object)null, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, (object)null, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, (object)null, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, (object)null, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, (object)null, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, (object)null, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, T6 p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, (object)null, (object)null, p6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, p4, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, T4 p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, p4, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, p4, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, p4, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, p4, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, p4, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, p4, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, p4, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, (object)null, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, (object)null, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, (object)null, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, (object)null, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, (object)null, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, (object)null, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, (object)null, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, T5 p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, (object)null, p5, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, T3 p3, T4 p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, p4, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, T4 p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, p4, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, p4, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, T4 p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, p4, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, p4, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, T4 p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, p4, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, p4, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, T4 p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, p4, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, p3, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, T3 p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, p3, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, p3, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, T3 p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, p3, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, p2, (object)null, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, T2 p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, p2, (object)null, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, T1 p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, p1, (object)null, (object)null, (object)null, (object)null, (object)null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetTypedActionUrl<TController, T1, T2, T3, T4, T5, T6>(this IUrlService svc, Expression<Func<TController, Func<T1, T2, T3, T4, T5, T6, ActionResult>>> f, Unspecified<T1>.ValueClass p1, Unspecified<T2>.ValueClass p2, Unspecified<T3>.ValueClass p3, Unspecified<T4>.ValueClass p4, Unspecified<T5>.ValueClass p5, Unspecified<T6>.ValueClass p6) where TController : IController {
			return GetActionUrlImpl(svc, f, (object)null, (object)null, (object)null, (object)null, (object)null, (object)null);
		}

	}
}
