using System;

namespace Saltarelle {
	public interface ITypedMarkupParserImpl {
		/// <summary>
		/// Parse a value.
		/// </summary>
		/// <param name="registeredPrefix">Prefix under which this markup parser is registered.</param>
		/// <param name="isArray">Whether the property is an array.</param>
		/// <param name="value">Value to parse.</param>
		/// <param name="template">Template in which the result will be used, or null if not parsing a template.</param>
		TypedMarkupData Parse(string registeredPrefix, bool isArray, string value, ITemplate template);
	}
}
