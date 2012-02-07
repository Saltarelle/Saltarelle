using System;
#if SERVER
using System.Text;
#endif

namespace Saltarelle.TypedMarkupParsers {
	internal class IntMarkupParser : ITypedMarkupParserImpl {
		public TypedMarkupData Parse(string registeredPrefix, bool isArray, string value, ITemplate template) {
			if (isArray) {
				StringBuilder sb = new StringBuilder();
				sb.Append("new int[] {");
				int[] ints;
				if (value.Trim() != "") {
					string[] split = value.Split('|');
					ints = new int[split.Length];
					for (int i = 0; i < split.Length; i++) {
						if (Utils.RegexExec(split[i], Utils.IntRegex, "") == null)
							throw ParserUtils.TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage(registeredPrefix, isArray, value));
						sb.Append(i > 0 ? ", " : " ");
						ints[i] = Utils.ParseInt(split[i]);
						sb.Append(Utils.ToStringInvariantInt(ints[i]));
					}
				}
				else
					ints = new int[0];
				sb.Append(" }");
				return new TypedMarkupData(sb.ToString(), delegate() { return ints; });
			}
			else {
				if (Utils.RegexExec(value, Utils.IntRegex, "") == null)
					throw ParserUtils.TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage(registeredPrefix, isArray, value));
				int i = Utils.ParseInt(value);
				return new TypedMarkupData(Utils.ToStringInvariantInt(i), delegate() { return i; });
			}
		}
	}
}
