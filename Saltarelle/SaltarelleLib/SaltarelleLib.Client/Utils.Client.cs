using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Xml;
using jQueryApi;

namespace Saltarelle {
	public static partial class Utils {
		public static int ParseInt(string s) {
			return int.Parse(s, 10);
		}

		public static double ParseDouble(string s) {
			return double.Parse(s);
		}

		public static string[] RegexExec(string toTest, string pattern, string options) {
			var x = new Regex(pattern, options);
			return (string[])(object)x.Exec(toTest);
		}
		
		public static string ToStringInvariantInt(int i) {
			return i.ToString();
		}

		public static string ToStringInvariantLong(long i) {
			return i.ToString();
		}

		public static string ToStringInvariantDouble(double d) {
			return d.ToString();
		}
		
		public static double Round(double value, int numDecimals) {
			return Math.Round(value * Math.Pow(10, numDecimals)) / Math.Pow(10, numDecimals);
		}

		public static double Ceil(double value) {
			return Math.Ceil(value);
		}

		private static Regex escapeable = new Regex(@"[""\\\x00-\x1f\x7f-\x9f]", "g");
		private static JsDictionary<string, string> substitutions;

		public static string ScriptEncode(string s) {
			if (escapeable.Test(s)) {
				return "\"" + s.ReplaceRegex(escapeable, a => {
					var c = substitutions[a];
					if (Type.GetScriptType(c) == "string") {
					    return c;
					}
					int code = a.CharCodeAt(0);
					return "\\u00" + (code / 16).ToString(16) + (code % 16).ToString(16);
				}) + "\"";
			}
			return "\"" + s + "\"";
		}
		
		private static Regex DateRegex = new Regex(@"^/Date\((-?\d+)\)/$");

		public static string Json(object o) {
			return System.Serialization.Json.Stringify(o);
		}
		
		public static object EvalJson(string s) {
			return System.Serialization.Json.Parse(s, (_, v) => {
				if (Type.GetScriptType(v) == "string") {
					var m = DateRegex.Exec((string)v);
					if (m != null)
						return new DateTime(int.Parse(m[1], 10));
				}
				return v;
			});
		}

		[IgnoreGenericArguments]
		public static T EvalJson<T>(string s) {
			return (T)EvalJson(s);
		}

		public static string JoinStrings(string separator, string[] value) {
			return value.Join(separator);
		}

		public static string JoinStrings(string separator, List<string> value) {
			return value.Join(separator);
		}

		public static DateTime? ParseDateExact(string value, string format) {
			return DateTime.ParseExactUtc(value, format);
		}

		public static string FormatDate(DateTime value, string format) {
			return new DateTime(value.GetUtcFullYear(), value.GetUtcMonth(), value.GetUtcDate(), value.GetUtcHours(), value.GetUtcMinutes(), value.GetUtcSeconds(), value.GetUtcMilliseconds()).Format(format);
		}
		
		public static string FormatNumber(double number, int decimals) {
			return decimals == 0 ? ToStringInvariantInt((int)number) : number.Format("F" + decimals);
		}

		public static double MillisecondDifference(DateTime first, DateTime second) {
			return first - second;
		}

		public static string Escape(string s) {
			return s.Escape();
		}

		public static string Unescape(string s) {
			return s.Unescape();
		}

		public static void DoForEachAttribute(XmlNode node, Action<XmlAttribute> a) {
			var attr = node.Attributes;
			for (int i = 0, n = attr.Count; i < n; i++)
				a((XmlAttribute)attr[i]);
		}

		public static void DoForEachChild(XmlNode node, Action<XmlNode> a) {
			for (int i = 0, n = node.ChildNodes.Count; i < n; i++)
				a(node.ChildNodes[i]);
		}

		public static int GetNumChildNodes(XmlNode n) {
			return n.HasChildNodes() ? n.ChildNodes.Count : 0;
		}

		public static XmlDocument ParseXml(string xml) {
			return XmlDocumentParser.Parse(xml);
		}

		public static void ClearStringBuilder(StringBuilder sb) {
			sb.Clear();
		}
		
		public static bool IsStringBuilderEmpty(StringBuilder sb) {
			return sb.IsEmpty;
		}

		public static string Substring(string s, int start, int len) {
			return s.Substr(start, len);
		}

		public static string MakeCamelCase(string s) {
			if (string.IsNullOrEmpty(s))
				return s;
			if (s == "ID")
				return "id";

			bool hasNonUppercase = false;
			int numUppercaseChars = 0;
			for (int index = 0; index < s.Length; index++) {
				if (s.CharCodeAt(index) >= 'A' && s.CharCodeAt(index) <= 'Z') {
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
				return s.Substring(0, numUppercaseChars - 1).ToLowerCase() + s.Substr(numUppercaseChars - 1);
			else if (s.Length == 1)
				return s.ToLowerCase();
			else
				return s.Substr(0, 1).ToLowerCase() + s.Substr(1);
		}

		public static object GetPropertyValue(object o, string property) {
			if (Script.IsUndefined(Type.GetField(o, "get_" + property)))
				property = MakeCamelCase(property);
			return Type.GetProperty(o, property);
		}

		public static void SetPropertyValue(object o, string property, object value) {
			if (Script.IsUndefined(Type.GetField(o, "set_" + property)))
				property = MakeCamelCase(property);
			Type.SetProperty(o, property, value);
		}

		public static Exception ArgumentException(string argument) {
			throw new Exception("Argument is invalid: " + argument);
		}

		public static Exception ArgumentNullException(string argument) {
			return new Exception("Argument cannot be null: " + argument);
		}

		/* Useful functions but not part of the emulation layer. Should perhaps be somehwere else
		 */

		public static jQueryObject Prev(jQueryObject q, string selector) {
			for (q = q.Prev(); q.Size() > 0 && !q.Is(selector); q = q.Prev())
				;
			return q;
		}

		public static jQueryObject Next(jQueryObject q, string selector) {
			for (q = q.Next(); q.Size() > 0 && !q.Is(selector); q = q.Next())
				;
			return q;
		}

		public static void RenderControl(IClientCreateControl c, Element parent) {
			var newEl = jQuery.FromHtml(c.Html).GetElement(0);
			parent.AppendChild(newEl);
			c.Attach();
		}

		public static jQueryXmlHttpRequest Ajax(JsDictionary<string, object> data, string url, bool post, AjaxRequestCallback success, AjaxErrorCallback error) {
			var converters = new JsDictionary<string, Func<string, object>>();
			converters["text json"] = EvalJson;
			return jQuery.Ajax(new jQueryAjaxOptions { Data = data, Converters = converters, DataType = "json", Cache = false, Type = post ? "POST" : "GET", Url = url, Success = success, Error = error });
		}

		[ScriptAlias("this")]
		static DateTime ThisDate { get { return null; } }

		static Utils() {
			substitutions = new JsDictionary<string, string>();
			substitutions["\b"] = "\\b";
			substitutions["\t"] = "\\t";
			substitutions["\n"] = "\\n";
			substitutions["\f"] = "\\f";
			substitutions["\r"] = "\\r";
			substitutions["\""] = "\\\"";
			substitutions["\\"] = "\\\\";

			((dynamic)typeof(DateTime).Prototype).toJSON = (Func<string>)(() => { return @"\/Date(" + ToStringInvariantLong(ThisDate.ValueOf()) + @")\/"; });
		}
	}
}
