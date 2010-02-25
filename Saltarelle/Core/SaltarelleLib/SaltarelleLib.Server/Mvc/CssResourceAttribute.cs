using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Saltarelle.Mvc {
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
	public class CssResourceAttribute : WebResourceAttribute {
		/// <summary>
		/// The LessCss variable used to identify the resource in css (without @ sign).
		/// This may only consist of the letters a-z, hyphen and the underscore character.
		/// </summary>
		public string CssVariableName { get; private set; }
		
		public CssResourceAttribute(string resourceQualifiedName, string publicResourceName, string cssVariableName) : base(resourceQualifiedName, publicResourceName) {
			if (cssVariableName.ToLowerInvariant().Trim(Enumerable.Range('a', 26).Select(i => (char)i).Concat(new[] { '_', '-' }).ToArray()) != "")
				throw new ArgumentException("name");
			this.CssVariableName = cssVariableName;
		}
	}
}
