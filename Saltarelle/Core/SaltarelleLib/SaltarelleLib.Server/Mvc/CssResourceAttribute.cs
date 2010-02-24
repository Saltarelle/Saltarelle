using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Saltarelle.Mvc {
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
	public sealed class CssResourceAttribute : Attribute {
		/// <summary>
		/// The name of the resource, which is the same as the name of the LessCss variable that will contain the resource path.
		/// This may only consist of the letters a-z, hyphen and the underscore character.
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Fully qualified name of the resource, e.g. Namespace.SubNamespace.Image.gif
		/// </summary>
		public string ResourceQualifiedName { get; private set; }
		/// <summary>
		/// Name under which the resource is published e.g. Image.gif. By default, the qualified resource name will be used.
		/// </summary>
		public string PublicResourceName { get; set; }
		
		public CssResourceAttribute(string name, string resourceQualifiedName) {
			if (name.ToLowerInvariant().Trim(Enumerable.Range('a', 26).Select(i => (char)i).Concat(new[] { '_', '-' }).ToArray()) != "")
				throw new ArgumentException("name");
			this.Name = name;
			this.ResourceQualifiedName = resourceQualifiedName;
			this.PublicResourceName = resourceQualifiedName;
		}
	}
}
