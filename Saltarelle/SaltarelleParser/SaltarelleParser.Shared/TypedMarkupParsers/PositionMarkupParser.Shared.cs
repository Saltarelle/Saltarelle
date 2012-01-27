using System;
#if SERVER
using System.Text;
#endif

namespace Saltarelle.TypedMarkupParsers {
	[Record]
	internal sealed class StringPositionPair {
		public string s;
		public Position p;
		public StringPositionPair(string s, Position p) {
			this.s = s;
			this.p = p;
		}
	}

	public class PositionMarkupParser : ITypedMarkupParserImpl {
		private StringPositionPair ParseSingle(string markup) {
			markup = markup.Trim();
			if (markup == "np") {
				return new StringPositionPair("PositionHelper.NotPositioned", PositionHelper.NotPositioned);
			}
			else if (markup == "fixed") {
				return new StringPositionPair("PositionHelper.Fixed", PositionHelper.Fixed);
			}
			else if (markup.StartsWith("lt(") && markup.EndsWith(")")) {
				string[] args = Utils.Substring(markup, 3, markup.Length - 4).Split(',');
				if (args.Length != 2 || Utils.RegexExec(args[0], Utils.IntRegex, "") == null || Utils.RegexExec(args[1], Utils.IntRegex, "") == null)
					return null;
				int left = Utils.ParseInt(args[0]), top = Utils.ParseInt(args[1]);
				return new StringPositionPair("PositionHelper.LeftTop(" + Utils.ToStringInvariantInt(left) + ", " + Utils.ToStringInvariantInt(top) + ")", PositionHelper.LeftTop(left, top));
			}
			else
				return null;
		}
	
		public TypedMarkupData Parse(string registeredPrefix, bool isArray, string value) {
			if (isArray) {
				StringBuilder sb = new StringBuilder();
				sb.Append("new Position[] {");
				Position[] positions;
				if (value.Trim() != "") {
					string[] split = value.Split('|');
					positions = new Position[split.Length];
					for (int i = 0; i < split.Length; i++) {
						StringPositionPair v = ParseSingle(split[i]);
						if (Utils.IsNull(v))
							throw ParserUtils.TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage(registeredPrefix, isArray, value));
						sb.Append(i > 0 ? ", " : " ");
						positions[i] = v.p;
						sb.Append(v.s);
					}
				}
				else
					positions = new Position[0];

				sb.Append(" }");
				return new TypedMarkupData(sb.ToString(), delegate() { return positions; });
			}
			else {
				StringPositionPair v = ParseSingle(value);
				if (Utils.IsNull(v))
					throw ParserUtils.TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage(registeredPrefix, isArray, value));
				return new TypedMarkupData(v.s, delegate { return v.p; });
			}
		}
	}
}
