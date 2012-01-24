using System;
#if SERVER
using System.Text;
#endif

namespace Saltarelle.TypedMarkupParsers {
	internal class StringMarkupParser : ITypedMarkupParserImpl {
		private string Fix(string value) {
			return "@\"" + value.Replace("\"", "\"\"") + "\"";
		}
	
		public TypedMarkupData Parse(string registeredPrefix, bool isArray, string value) {
			if (isArray) {
				StringBuilder sb = new StringBuilder();
				sb.Append("new string[] {");
				string[] strings;
				if (value != "") {
					strings = value.Split('|');
					for (int i = 0; i < strings.Length; i++) {
						sb.Append(i > 0 ? ", " : " ");
						sb.Append(Fix(strings[i]));
					}
				}
				else
					strings = new string[0];
				sb.Append(" }");
				return new TypedMarkupData(sb.ToString(), delegate() { return strings; });
			}
			else {
				return new TypedMarkupData(Fix(value), delegate() { return value; });
			}
		}
	}
}
