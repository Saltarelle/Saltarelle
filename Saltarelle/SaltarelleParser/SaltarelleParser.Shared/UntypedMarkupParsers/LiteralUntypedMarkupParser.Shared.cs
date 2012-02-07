using Saltarelle.Fragments;

namespace Saltarelle.UntypedMarkupParsers {
	internal class LiteralUntypedMarkupParser : IUntypedMarkupParserImpl {
		public IFragment TryParse(string value, ITemplate template) {
			return new LiteralFragment(Utils.HtmlEncode(value));
		}
	}
}