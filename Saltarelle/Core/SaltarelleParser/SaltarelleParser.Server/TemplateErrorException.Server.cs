using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Saltarelle {
	[Serializable]
	public class TemplateErrorException : Exception {
		public TemplateErrorException() { }
		public TemplateErrorException(string message) : base(message) { }
		public TemplateErrorException(string message, Exception inner) : base(message, inner) { }
		protected TemplateErrorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
