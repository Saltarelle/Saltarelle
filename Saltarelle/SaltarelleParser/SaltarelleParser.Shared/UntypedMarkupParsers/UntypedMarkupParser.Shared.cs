using Saltarelle.UntypedMarkupParsers;

namespace Saltarelle {
	public interface IUntypedMarkupParser {
		IFragment ParseMarkup(string markup, ITemplate template);
	}

	public class UntypedMarkupParser : IUntypedMarkupParser {
		private static IUntypedMarkupParserImpl[] defaultParsers = { new CodeUntypedMarkupParser(),
		                                                             new LiteralUntypedMarkupParser()
		                                                           };

		private IUntypedMarkupParserImpl[] parsers;

		public UntypedMarkupParser(IUntypedMarkupParserImpl[] pluginImplementers) {
			int numPluginImplementers = (!Utils.IsNull(pluginImplementers) ? pluginImplementers.Length : 0);
			parsers = new IUntypedMarkupParserImpl[numPluginImplementers + defaultParsers.Length];
			for (int i = 0; i < numPluginImplementers; i++)
				parsers[i] = pluginImplementers[i];
			for (int i = 0; i < defaultParsers.Length; i++)
				parsers[i + numPluginImplementers] = defaultParsers[i];
		}

		public IFragment ParseMarkup(string markup, ITemplate template) {
			foreach (IUntypedMarkupParserImpl p in parsers) {
				IFragment f = p.TryParse(markup, template);
				if (!Utils.IsNull(f))
					return f;
			}
			throw ParserUtils.TemplateErrorException("Invalid markup " + markup); // Should never happen since the LiteralUntypedMarkupParser is a catch-all
		}
	}
}