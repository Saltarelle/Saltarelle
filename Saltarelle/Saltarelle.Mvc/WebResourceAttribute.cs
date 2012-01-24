using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Saltarelle.Mvc {
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
	public class WebResourceAttribute : Attribute {
		/// <summary>
		/// Fully qualified name of the resource, e.g. Namespace.SubNamespace.Image.gif
		/// </summary>
		public string ResourceQualifiedName { get; private set; }
		/// <summary>
		/// Name under which the resource is published e.g. Image.gif.
		/// </summary>
		public string PublicResourceName { get; private set; }
		
		public WebResourceAttribute(string resourceQualifiedName, string publicResourceName) {
			this.ResourceQualifiedName = resourceQualifiedName;
			this.PublicResourceName    = publicResourceName;
		}
	}
}
