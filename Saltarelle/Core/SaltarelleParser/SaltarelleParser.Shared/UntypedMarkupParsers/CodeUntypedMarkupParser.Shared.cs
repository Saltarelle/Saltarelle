namespace Saltarelle.UntypedMarkupParsers {
	internal class CodeUntypedMarkupParser : IUntypedMarkupParserImpl {
		public IFragment TryParse(string markup) {
			if (markup.StartsWith("code:"))
				return new CodeExpressionFragment(Utils.Substring(markup, 5, markup.Length - 5).Trim());
			else if (markup.StartsWith("{=") && markup.EndsWith("}"))
				return new CodeExpressionFragment(Utils.Substring(markup, 2, markup.Length - 3).Trim());
			return null;
		}
	}
}