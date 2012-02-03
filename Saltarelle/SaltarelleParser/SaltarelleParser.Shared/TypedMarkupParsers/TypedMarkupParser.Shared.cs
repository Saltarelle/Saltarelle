using System;
using Saltarelle.TypedMarkupParsers;
#if SERVER
using System.Text;
using TypedMarkupParserImplDictionary = System.Collections.Generic.Dictionary<string, Saltarelle.ITypedMarkupParserImpl>;
using TypedMarkupParserImplEntry      = System.Collections.Generic.KeyValuePair<string, Saltarelle.ITypedMarkupParserImpl>;
#else
using TypedMarkupParserImplDictionary = System.Dictionary;
using TypedMarkupParserImplEntry      = System.DictionaryEntry;
#endif

namespace Saltarelle {
	public interface ITypedMarkupParser {
		TypedMarkupData ParseMarkup(string markup);
	}

	public class TypedMarkupParser : ITypedMarkupParser {
		private static TypedMarkupParserImplDictionary defaultImplementers;
		
		static TypedMarkupParser() {
			defaultImplementers = new TypedMarkupParserImplDictionary();
			defaultImplementers["str"]  = new StringMarkupParser();
			defaultImplementers["int"]  = new IntMarkupParser();
			defaultImplementers["pos"]  = new PositionMarkupParser();
			defaultImplementers["bool"] = new BoolMarkupParser();
			defaultImplementers["code"] = new CodeMarkupParser();
		}

		private TypedMarkupParserImplDictionary implementers;
	
		public TypedMarkupParser(TypedMarkupParserImplDictionary pluginImplementers) {
			implementers = new TypedMarkupParserImplDictionary();
			foreach (TypedMarkupParserImplEntry kvp in defaultImplementers)
				implementers[kvp.Key] = kvp.Value;
			if (!Utils.IsNull(pluginImplementers)) {
				foreach (TypedMarkupParserImplEntry kvp in pluginImplementers)
					implementers[kvp.Key] = kvp.Value;
			}
		}
	
		public TypedMarkupData ParseMarkup(string markup) {
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

			return ((ITypedMarkupParserImpl)implementers[prefix]).Parse(prefix, isArray, Utils.Substring(markupExceptArr, colon + 1, markupExceptArr.Length - colon - 1));
		}
	}
}
