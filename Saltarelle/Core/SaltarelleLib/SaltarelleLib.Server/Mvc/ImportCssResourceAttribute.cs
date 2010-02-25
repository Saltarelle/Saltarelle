using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Saltarelle.Mvc {
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
	public sealed class ImportCssResourceAttribute : Attribute {
		/// <summary>
		/// Assembly to import resource from.
		/// </summary>
		public Assembly ResourceAssembly { get; private set; }
		/// <summary>
		/// Public name of the resource to import. Note: The public resource name, not the qualifed name or the LessCss variable.
		/// </summary>
		public string   PublicResourceName { get; private set; }

		/// <summary>
		/// The LessCss variable used to identify the resource in css (without @ sign).
		/// This may only consist of the letters a-z, hyphen and the underscore character.
		/// </summary>
		public string   CssVariableName { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="typeInAssembly">A type in the assembly containing the resource.</param>
		/// <param name="resourceName">PublicResourceName of the resource to import.</param>
		public ImportCssResourceAttribute(Type typeInAssembly, string publicResourceName, string cssVariableName) {
			this.ResourceAssembly = typeInAssembly.Assembly;
			WebResourceAttribute attr = this.ResourceAssembly.GetCustomAttributes(typeof(WebResourceAttribute), false).Cast<WebResourceAttribute>().SingleOrDefault(x => x.PublicResourceName == publicResourceName);
			if (attr == null)
				throw new ArgumentException("The assembly " + this.ResourceAssembly.GetName().Name + " does not export a resource with the name " + publicResourceName);

			this.PublicResourceName = attr.PublicResourceName;
			this.CssVariableName    = cssVariableName;
		}
	}
}
