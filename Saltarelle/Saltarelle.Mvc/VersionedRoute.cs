using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Routing;
using DependencyRetriever = System.Func<System.Web.Routing.RouteValueDictionary, string>;

namespace Saltarelle.Mvc {
	public class VersionedRoute : Route {
		private IDictionary<string, DependencyRetriever> dependencies;

		public VersionedRoute(string url, IRouteHandler handler, IDictionary<string, DependencyRetriever> dependencies) : base(url, handler) {
			this.dependencies = dependencies;
		}

		public VersionedRoute(string url, RouteValueDictionary defaults, IDictionary<string, DependencyRetriever> dependencies, IRouteHandler handler) : base(url, defaults, handler) {
			this.dependencies = dependencies;
		}

		public VersionedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IDictionary<string, DependencyRetriever> dependencies, IRouteHandler handler) : base(url, defaults, constraints, handler) {
			this.dependencies = dependencies;
		}

		public VersionedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IDictionary<string, DependencyRetriever> dependencies, IRouteHandler handler) : base(url, defaults, constraints, dataTokens, handler) {
			this.dependencies = dependencies;
		}
	
		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {
			RouteValueDictionary rvd = new RouteValueDictionary(values);
			foreach (var dep in dependencies) {
				string v = dep.Value(values);
				if (v != null)
					rvd[dep.Key] = v;
			}
			return base.GetVirtualPath(requestContext, rvd);
		}
	}
}