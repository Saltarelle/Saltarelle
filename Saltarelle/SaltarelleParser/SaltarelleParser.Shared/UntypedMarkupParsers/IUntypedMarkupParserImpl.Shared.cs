namespace Saltarelle {
	public interface IUntypedMarkupParserImpl {
		/// <summary>
		/// Parse a value.
		/// </summary>
		/// <param name="markup">Value to parse.</param>
		/// <param name="template">Template in which the result will be used, or null if not parsing a template.</param>
		IFragment TryParse(string markup, ITemplate template);
	}
}