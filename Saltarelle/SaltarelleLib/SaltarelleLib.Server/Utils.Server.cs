using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;
using System.Text;
using System.Xml;
using System.Web;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace System.Runtime.CompilerServices {
	// to make it possible to use these Script# attibutes in shared files
	public sealed class RecordAttribute : Attribute {}
	public sealed class PreserveCaseAttribute : Attribute {}
}

namespace Saltarelle {
	public static partial class Utils {
		private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };

		private static readonly Dictionary<string, Assembly> asmCache  = new Dictionary<string, Assembly>();

		public static int ParseInt(string s) {
			return int.Parse(s, System.Globalization.NumberFormatInfo.InvariantInfo);
		}

		public static double ParseDouble(string s) {
			return double.Parse(s, System.Globalization.NumberFormatInfo.InvariantInfo);
		}
		
		public static string[] RegexExec(string toTest, string pattern, string options) {
			Regex x = new Regex(pattern, RegexOptions.ECMAScript | (options.Contains("i") ? RegexOptions.IgnoreCase : 0));
			Match m = x.Match(toTest);
			return m.Success ? m.Groups.OfType<Group>().Select(g => g.Value).ToArray() : null;
		}

		public static string ToStringInvariantInt(int i) {
			return Convert.ToString(i, System.Globalization.NumberFormatInfo.InvariantInfo);
		}

		public static string ToStringInvariantDouble(double d) {
			return Convert.ToString(d, System.Globalization.NumberFormatInfo.InvariantInfo);
		}
		
		public static double Round(double value, int numDecimals) {
			return Math.Round(value, numDecimals);
		}

		public static double Ceil(double value) {
			return Math.Ceiling(value);
		}
		
		public static string ScriptEncode(string s) {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < s.Length; i++) {
				switch (s[i]) {
					case '\'': sb.Append("\\\'"); break;
					case '\"': sb.Append("\\\""); break;
					case '\\': sb.Append("\\\\"); break;
					case '\r': sb.Append("\\r"); break;
					case '\n': sb.Append("\\n"); break;
					default:   sb.Append(s[i]); break;
				}
			}

			return sb.ToString();
		}

		public static string InitScript(object o) {
			return JsonConvert.SerializeObject(o, new JavaScriptDateTimeConverter());
		}

		public static string Json(object o) {
			return JsonConvert.SerializeObject(o, jsonSerializerSettings);
		}

		public static object EvalJson(string s, Type objectType) {
			return JsonConvert.DeserializeObject(s, objectType, jsonSerializerSettings);
		}
		
		public static T EvalJson<T>(string s) {
			return JsonConvert.DeserializeObject<T>(s, jsonSerializerSettings);
		}

		public static string JoinStrings(string separator, IList<string> value) {
			return string.Join(separator, value);
		}
		
		public static DateTime? ParseDateExact(string value, string format) {
			DateTime rs;
			if (DateTime.TryParseExact(value, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out rs))
				return DateTime.SpecifyKind(rs, DateTimeKind.Utc);
			else
				return null;
		}
		
		public static string FormatDate(DateTime value, string format) {
			return DateTime.SpecifyKind(value, DateTimeKind.Utc).ToString(format, DateTimeFormatInfo.InvariantInfo);
		}

		public static string FormatNumber(double number, int decimals) {
			return decimals == 0 ? Utils.ToStringInvariantInt((int)number) : number.ToString("F" + decimals, NumberFormatInfo.InvariantInfo);
		}

		public static double MillisecondDifference(DateTime first, DateTime second) {
			return (first - second).TotalMilliseconds;
		}
		
		public static string Escape(string s) {
			return HttpUtility.UrlEncode(s);
		}

		public static string Unescape(string s) {
			return HttpUtility.UrlDecode(s);
		}
		
		public static void DoForEachAttribute(XmlNode node, Action<XmlAttribute> a) {
			foreach (XmlAttribute attr in node.Attributes)
				a(attr);
		}

		public static void DoForEachChild(XmlNode node, Action<XmlNode> a) {
			foreach (XmlNode c in node.ChildNodes)
				a(c);
		}

		public static int GetNumChildNodes(XmlNode n) {
			return n.HasChildNodes ? n.ChildNodes.Count : 0;
		}

		public static XmlDocument ParseXml(string xml) {
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			return doc;
		}
		
		public static void ClearStringBuilder(StringBuilder sb) {
			sb.Length = 0;
		}
		
		public static bool IsStringBuilderEmpty(StringBuilder sb) {
			return sb.Length == 0;
		}
		
		public static string Substring(string s, int start, int len) {
			return s.Substring(start, len);
		}

		public static string MakeCamelCase(string s) {
			if (string.IsNullOrEmpty(s))
				return s;
			if (s.Equals("ID", StringComparison.Ordinal))
				return "id";

			bool hasNonUppercase = false;
			int numUppercaseChars = 0;
			for (int index = 0; index < s.Length; index++) {
				if (char.IsUpper(s, index)) {
					numUppercaseChars++;
				}
				else {
					hasNonUppercase = true;
					break;
				}
			}

			if ((!hasNonUppercase && s.Length != 1) || numUppercaseChars == 0)
				return s;
			else if (numUppercaseChars > 1)
				return s.Substring(0, numUppercaseChars - 1).ToLower(CultureInfo.InvariantCulture) + s.Substring(numUppercaseChars - 1);
			else if (s.Length == 1)
				return s.ToLower(CultureInfo.InvariantCulture);
			else
				return char.ToLower(s[0], CultureInfo.InvariantCulture) + s.Substring(1);
		}
		
		public static string ToLowerCase(this string s) {
			return s.ToLowerInvariant();
		}

		public static object GetPropertyValue(object o, string property) {
			return o.GetType().GetProperty(property).GetValue(o, null);
		}

		public static void SetPropertyValue(object o, string property, object value) {
			var p = o.GetType().GetProperty(property);
			if (value == null || p.PropertyType.IsInstanceOfType(value)) {
				p.SetValue(o, value, null);
			}
			else {
				p.SetValue(o, Convert.ChangeType(value, p.PropertyType, CultureInfo.InvariantCulture), null);
			}
		}

		public static string IdAndStyle(string id, Position p, int width, int height) {
			return IdAndStyle(id, p, width, height, null);
		}

		public static Assembly[] GetAllAssemblies() {
			lock (AppDomain.CurrentDomain) {
				return AppDomain.CurrentDomain.GetAssemblies();
			}
		}

		public static Assembly FindAssembly(string assemblyName) {
			Assembly a;
			if (!TryFindAssembly(assemblyName, out a))
				throw new ArgumentException("The assembly " + assemblyName + " was not found.");
			return a;
		}
		
		public static bool TryFindAssembly(string assemblyName, out Assembly a) {
			if (!asmCache.TryGetValue(assemblyName, out a)) {
				lock (asmCache) {
					asmCache[assemblyName] = a = GetAllAssemblies().SingleOrDefault(x => x.GetName().Name == assemblyName);
				}
			}
			return a != null;
		}

		public static Exception ArgumentException(string argument) {
			return new ArgumentException(argument);
		}

		public static Exception ArgumentNullException(string argument) {
			return new ArgumentNullException(argument);
		}
	}
}
