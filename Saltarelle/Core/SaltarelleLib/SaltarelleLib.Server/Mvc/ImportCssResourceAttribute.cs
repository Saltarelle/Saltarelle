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

		public string   CssVariableName { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="typeInAssembly">A type in the assembly containing the resource.</param>
		/// <param name="resourceName">Name of the resource to import. Note: The name, not the PublicResourceName or the QualifiedResourceName. This parameter will also by default be used as the css variable name.</param>
		public ImportCssResourceAttribute(Type typeInAssembly, string resourceName) {
			this.ResourceAssembly = typeInAssembly.Assembly;
			CssResourceAttribute attr = this.ResourceAssembly.GetCustomAttributes(typeof(CssResourceAttribute), false).Cast<CssResourceAttribute>().SingleOrDefault(x => x.Name == resourceName);
			if (attr == null)
				throw new ArgumentException("The assembly " + this.ResourceAssembly.GetName().Name + " does not export a css resource with the name " + resourceName);

			this.PublicResourceName = attr.PublicResourceName;
			this.CssVariableName    = resourceName;
		}
	}
}
