namespace Saltarelle.UntypedMarkupParsers {
	internal class LiteralUntypedMarkupParser : IUntypedMarkupParserImpl {
		public IFragment TryParse(string value) {
			return new LiteralFragment(Utils.HtmlEncode(value));
		}
	}
}