using System;
using System.Collections.Generic;
using Saltarelle.TypedMarkupParsers;

namespace Saltarelle {
	public interface ITypedMarkupParser {
		TypedMarkupData ParseMarkup(string markup, ITemplate template);
	}

	public class TypedMarkupParser : ITypedMarkupParser {
		private static Dictionary<string, ITypedMarkupParserImpl> defaultImplementers;
		
		static TypedMarkupParser() {
			defaultImplementers = new Dictionary<string, ITypedMarkupParserImpl>();
			defaultImplementers["str"]  = new StringMarkupParser();
			defaultImplementers["int"]  = new IntMarkupParser();
			defaultImplementers["pos"]  = new PositionMarkupParser();
			defaultImplementers["bool"] = new BoolMarkupParser();
			defaultImplementers["code"] = new CodeMarkupParser();
		}

		private Dictionary<string, ITypedMarkupParserImpl> implementers;
	
		public TypedMarkupParser(IDictionary<string, ITypedMarkupParserImpl> pluginImplementers) {
			implementers = new Dictionary<string, ITypedMarkupParserImpl>();
			foreach (var kvp in defaultImplementers)
				implementers[kvp.Key] = kvp.Value;
			if (pluginImplementers != null) {
				foreach (var kvp in pluginImplementers)
					implementers[kvp.Key] = kvp.Value;
			}
		}
	
		public TypedMarkupData ParseMarkup(string markup, ITemplate template) {
			bool isArray = false;
			string markupExceptArr;
			if (markup.StartsWith("arr:")) {
				isArray = true;
				markupExceptArr = Utils.Substring(markup, 4, markup.Length - 4);
			}
			else
				markupExceptArr = markup;

			int colon = markupExceptArr.IndexOf(":");
			if (colon == -1)
				throw ParserUtils.TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage2(markup));
			string prefix = markupExceptArr.Substring(0, colon);
			if (prefix.EndsWith("[]")) {
				isArray = true;
				prefix  = prefix.Substring(0, prefix.Length - 2);
			}
			if (!implementers.ContainsKey(prefix))
				throw ParserUtils.TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage2(markup));

			return ((ITypedMarkupParserImpl)implementers[prefix]).Parse(prefix, isArray, Utils.Substring(markupExceptArr, colon + 1, markupExceptArr.Length - colon - 1), template);
		}
	}
}
