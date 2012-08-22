using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltarelle {
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class NodeProcessorAttribute : Attribute {
		public int Priority  { get; set; }
		
		public NodeProcessorAttribute() {
		}
		
		public NodeProcessorAttribute(int priority) {
			this.Priority = priority;
		}
	}
}
