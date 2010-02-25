using System;
#if CLIENT
using NullableDateTime = System.DateTime;
using ArgumentException = System.Exception;
using ParameterDictionary = System.Dictionary;
using ParameterEntry = System.DictionaryEntry;
#else
using NullableDateTime = System.Nullable<System.DateTime>;
using System.Text;
using ParameterDictionary = System.Collections.Generic.IDictionary<string, object>;
using ParameterEntry = System.Collections.Generic.KeyValuePair<string, object>;
using System.Globalization;
#endif

namespace Saltarelle {
	public static partial class Utils {
		public static string IntRegex   = "^\\s*[+-]?\\d+\\s*$";
		public static string FloatRegex = "^\\s*([+-]?(?:(?:\\d+\\.?\\d*)|(?:\\.\\d+))(?:[eE][+-]?\\d+)?)\\s*$";
		
		public static string ScriptStr(string s) {
			return s == null ? "null" : ("'" + ScriptEncode(s) + "'");
		}

		public static string ScriptBool(bool b) {
			return b ? "true" : "false";
		}
		
		public static bool ParseBool(string value) {
			return value == "1" || (value ?? "").ToLowerCase() == "true";
		}

		public static string HtmlEncode(string s) {
			return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\xa0", "&nbsp;").Replace("\"", "&quot;");
		}

		public static string NullIfEmpty(string s) {
			return string.IsNullOrEmpty(s) ? null : s;
		}

#if CLIENT
		[AlternateSignature]
		public extern static string IdAndStyle(string id, Position p, int width, int height);
#endif
		public static string IdAndStyle(string id, Position p, int width, int height, string additionalStyles) {
			string s = PositionHelper.CreateStyle(p, width, height);
			if (!string.IsNullOrEmpty(additionalStyles))
				s = (!string.IsNullOrEmpty(s) ? s + "; " : "") + additionalStyles;
			return "id=\"" + id + "\"" + (!string.IsNullOrEmpty(s) ? " style=\"" + s + "\"" : "");
		}
	}
}
