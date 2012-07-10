using System;
#if CLIENT
using System.Runtime.CompilerServices;
using ArgumentException = System.Exception;
#else
using System.Text;
using System.Globalization;
#endif

namespace Saltarelle {
	public static partial class Utils {
		public static string IntRegex   = "^\\s*[+-]?\\d+\\s*$";
		public static string FloatRegex = "^\\s*([+-]?(?:(?:\\d+\\.?\\d*)|(?:\\.\\d+))(?:[eE][+-]?\\d+)?)\\s*$";
		
		public static string ScriptStr(string s) {
			return IsNull(s) ? "null" : ("'" + ScriptEncode(s) + "'");
		}

		public static string ScriptBool(bool b) {
			return b ? "true" : "false";
		}
		
		public static bool ParseBool(string value) {
			return value == "1" || (value ?? "").ToLowerCase() == "true";
		}

		public static string HtmlEncode(string s) {
			return !Utils.IsNull(s)
			     ? s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\xa0", "&nbsp;").Replace("\"", "&quot;")
				 : null;
		}

		public static string NullIfEmpty(string s) {
			return string.IsNullOrEmpty(s) ? null : s;
		}

		public static string IdAndStyle(string id, Position p, int width, int height, string additionalStyles = null) {
			string s = PositionHelper.CreateStyle(p, width, height);
			if (!string.IsNullOrEmpty(additionalStyles))
				s = (!string.IsNullOrEmpty(s) ? s + "; " : "") + additionalStyles;
			return "id=\"" + id + "\"" + (!string.IsNullOrEmpty(s) ? " style=\"" + s + "\"" : "");
		}
	}
}
