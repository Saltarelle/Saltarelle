using System;
#if SERVER
using System.Text;
#endif

namespace Saltarelle.TypedMarkupParsers {
	internal class BoolMarkupParser : ITypedMarkupParserImpl {
		private int ParseSingle(string markup) {
			switch (markup.Trim().ToLowerCase()) {
				case "true":
					return 1;
				case "false":
					return 0;
				default:
					return -1;
			}
		}
	
		public TypedMarkupData Parse(string registeredPrefix, bool isArray, string value, ITemplate template) {
			if (isArray) {
				StringBuilder sb = new StringBuilder();
				sb.Append("new bool[] {");
				bool[] bools;
				if (value.Trim() != "") {
					string[] split = value.Split('|');
					bools = new bool[split.Length];
					for (int i = 0; i < split.Length; i++) {
						int x = ParseSingle(split[i]);
						if (x == -1)
							throw ParserUtils.TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage(registeredPrefix, isArray, value));
						bools[i] = (x == 1);
						sb.Append(i > 0 ? ", " : " ");
						sb.Append(x == 1 ? "true" : "false");
					}
				}
				else
					bools = new bool[0];

				sb.Append(" }");
				return new TypedMarkupData(sb.ToString(), delegate() { return bools; });
			}
			else {
				int x = ParseSingle(value);
				if (x == -1)
					throw ParserUtils.TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage(registeredPrefix, isArray, value));
				return new TypedMarkupData(x == 1 ? "true" : "false", delegate { return x == 1; });
			}
		}
	}
}
