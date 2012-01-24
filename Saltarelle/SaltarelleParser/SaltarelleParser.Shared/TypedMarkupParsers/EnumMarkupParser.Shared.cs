using System;
#if SERVER
using System.Text;
#endif

namespace Saltarelle.TypedMarkupParsers {
	[Record]
	internal sealed class SingleEnumMarkupValue {
		public object value;
		public string typeName;
		public string valueStr;

		public SingleEnumMarkupValue(object value, string typeName, string valueStr) {
			this.value    = value;
			this.typeName = typeName;
			this.valueStr = valueStr;
		}
	}

	internal class EnumMarkupParser : ITypedMarkupParserImpl {
		private SingleEnumMarkupValue ParseSingle(string markup) {
			markup = markup.Trim();
			int colon = markup.LastIndexOf('.');
			if (colon == -1)
				return null;

			string typeName = markup.Substring(0, colon), valueStr = Utils.Substring(markup, colon + 1, markup.Length - colon - 1);

			object value = null;
			bool   valid;
			try {
				value = Enum.Parse(Utils.FindType(typeName), valueStr);
				valid = true;
			}
			catch (Exception) {
				valid = false;
				#if CLIENT
					try {
						value = Enum.Parse(Utils.FindType(typeName), Utils.MakeCamelCase(valueStr));
						valid = true;
					}
					catch (Exception) {
					}
				#endif
			}
			
			return valid ? new SingleEnumMarkupValue(value, typeName, valueStr) : null;
		}

		public TypedMarkupData Parse(string registeredPrefix, bool isArray, string value) {
			if (isArray) {
				value = value.Trim();
				if (value == "")
					throw ParserUtils.TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage(registeredPrefix, isArray, value));

				StringBuilder sb = new StringBuilder();
				Array enums = null;

				if (value.EndsWith("!")) {
					value = value.Substring(0, value.Length - 1);
					try {
						Type tp = Utils.FindType(value);
						#if SERVER
							enums = Array.CreateInstance(tp, 0);
						#else
							enums = new int[0];
						#endif
						sb.Append("new " + value + "[0]");
					}
					catch (Exception) {
						throw ParserUtils.TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage(registeredPrefix, isArray, value));
					}
				}
				else {
					string[] split = value.Split('|');
					string typeName = null;
					for (int i = 0; i < split.Length; i++) {
						SingleEnumMarkupValue v = ParseSingle(split[i]);
						if (Utils.IsNull(v) || (!Utils.IsNull(typeName) && typeName != v.typeName))
							throw ParserUtils.TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage(registeredPrefix, isArray, value));
						if (Utils.IsNull(typeName)) {
							typeName = v.typeName;
							sb.Append("new " + typeName + "[] {");
							#if SERVER
								enums = Array.CreateInstance(Utils.FindType(typeName), split.Length);
							#else
								enums = new int[split.Length];
							#endif
						}
						#if SERVER
							enums.SetValue(v.value, i);
						#else
							enums[i] = v.value;
						#endif
						sb.Append(i > 0 ? ", " : " ");
						sb.Append(v.typeName + "." + v.value);
					}
					sb.Append(" }");
				}
				return new TypedMarkupData(sb.ToString(), delegate() { return enums; });
			}
			else {
				SingleEnumMarkupValue v = ParseSingle(value);
				if (Utils.IsNull(v))
					throw ParserUtils.TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage(registeredPrefix, isArray, value));
				return new TypedMarkupData(v.typeName + "." + v.valueStr, delegate() { return v.value; });
			}
		}
	}
}
