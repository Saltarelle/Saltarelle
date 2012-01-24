using System;
#if SERVER
using System.Text;
#endif

namespace Saltarelle.TypedMarkupParsers {
	internal class CodeMarkupParser : ITypedMarkupParserImpl {
		public TypedMarkupData Parse(string registeredPrefix, bool isArray, string value) {
			if (isArray || value.Trim() == "")
				throw ParserUtils.TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage(registeredPrefix, isArray, value));
			return new TypedMarkupData(value, delegate { throw ParserUtils.TemplateErrorException("Cannot use code markup in non-compiled template"); });
		}
	}
}
