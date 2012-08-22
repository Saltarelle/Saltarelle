using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Saltarelle {
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class TypedMarkupParserImplAttribute : Attribute {
		private static Regex validPrefixRegex = new Regex("^[0-9a-zA-Z]+$");
	
		public string Prefix { get; set; }
		
		public TypedMarkupParserImplAttribute(string prefix) {
			if (prefix == null || !validPrefixRegex.IsMatch(prefix)) throw new ArgumentException(prefix);
			this.Prefix = prefix;
		}
	}
}
