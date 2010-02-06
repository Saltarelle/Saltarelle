using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltarelle {
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class UntypedMarkupParserImplAttribute : Attribute {
		public int Priority  { get; set; }
		
		public UntypedMarkupParserImplAttribute() {
		}
		
		public UntypedMarkupParserImplAttribute(int priority) {
			this.Priority = priority;
		}
	}
}
