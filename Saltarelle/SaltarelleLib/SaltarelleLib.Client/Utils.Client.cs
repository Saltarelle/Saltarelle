using System;
using System.Collections;
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
			var x = new RegularExpression(pattern, options);
			return x.Exec(toTest);
		}
		
		public static string ToStringInvariantInt(int i) {
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

		public static string ScriptEncode(string s) {
			return jQuery.quoteString(s);
		}
		
		public static string Json(object o) {
			return jQuery.toJSON(o);
		}
		
		public static object EvalJson(string s) {
			return jQuery.evalJSON(s);
		}

		[IgnoreGenericArguments]
		public static T EvalJson<T>(string s) {
			return (T)jQuery.evalJSON(s);
		}

		public static bool IsNull(object o) {
			return Script.IsNullOrUndefined(o);
		}
		
		public static string JoinStrings(string separator, string[] value) {
			return value.Join(separator);
		}
		
		public static Date ParseDateExact(string value, string format) {
			return (Date)Type.InvokeMethod(typeof(Date), "parseExact", value, format);
		}

		public static string FormatDate(Date value, string format) {
			return new Date(value.GetUTCFullYear(), value.GetUTCMonth(), value.GetUTCDate(), value.GetUTCHours(), value.GetUTCMinutes(), value.GetUTCSeconds(), value.GetUTCMilliseconds()).Format(format);
		}
		
		public static string FormatNumber(double number, int decimals) {
			return decimals == 0 ? ToStringInvariantInt((int)number) : number.Format("F" + decimals);
		}

		public static double MillisecondDifference(Date first, Date second) {
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

		public static void Ajax(JsDictionary data, string url, bool post, Delegate success, Delegate error) {
			jQuery.Ajax(new JsDictionary("data", data,
			                             "dataFilter", new EvalJsonDelegate(jQuery.evalJSON),
			                             "dataType", "json",
			                             "cache", false,
			                             "type", post ? "POST" : "GET",
			                             "url", url,
			                             "success", success,
			                             "error", error));
		}
	}
}
