using System;
using System.DHTML;
using System.XML;

namespace Saltarelle {
	public delegate void XmlAttributeAction(XMLAttribute a);
	public delegate void XmlNodeAction(XMLNode n);

	public static partial class Utils {
		public static int ParseInt(string s) {
			return Number.ParseInt(s, 10);
		}

		public static double ParseDouble(string s) {
			return Number.ParseFloat(s);
		}

		public static string[] RegexExec(string toTest, string pattern, string options) {
			RegularExpression x = new RegularExpression(pattern, options);
			return x.Exec(toTest);
		}
		
		public static string ToStringInvariantInt(int i) {
			return i.ToString();
		}

		public static string ToStringInvariantDouble(double d) {
			return d.ToString();
		}
		
		// Script# has a bug which causes "char c = 'a'" to be translated into "var c = 'a'" rather than "var c = 65"
		public static char CharCode(char c) {
			return (char)((string)(object)c).CharCodeAt(0);
		}
		
		public static string RepeatChar(char ch, int count) {
			if (Type.GetScriptType(ch) == "string") // Script# bug, it translates 'A' to 'A' rather than to 65.
				return string.FromChar(ch, count);
			else
				return string.FromCharCode(ch, count);
		}
		
		public static double Round(double value, int numDecimals) {
			return Math.Round(value * Math.Pow(10, numDecimals)) / Math.Pow(10, numDecimals);
		}

		public static double Ceil(double value) {
			return Math.Ceil(value);
		}

		public static int ArrayLength(ArrayList l) {
			return l.Length;
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

		public static double NaN {
			get { return Number.NaN; }
		}

		public static bool IsNaN(double v) {
			return Number.IsNaN(v);
		}
		
		public static bool IsNull(object o) {
			return Script.IsNull(o);
		}

		public static string JoinStrings(string separator, string[] value) {
			return value.Join(separator);
		}
		
		public static DateTime ParseDateExact(string value, string format) {
			return (DateTime)Type.InvokeMethod(typeof(DateTime), "parseExact", value, format);
		}

		public static string FormatDate(DateTime value, string format) {
			return new DateTime(value.GetUTCFullYear(), value.GetUTCMonth(), value.GetUTCDate(), value.GetUTCHours(), value.GetUTCMinutes(), value.GetUTCSeconds(), value.GetUTCMilliseconds()).Format(format);
		}
		
		public static string FormatNumber(double number, int decimals) {
			return decimals == 0 ? ToStringInvariantInt((int)number) : number.Format("F" + decimals);
		}

		public static Array ArrayResize(Array arr, int size, object newObj) {
			ArrayList rslt = (ArrayList)(object)arr.Clone();
			if (size < arr.Length)
				rslt.RemoveRange(size, arr.Length - size);
			while (rslt.Length < size)
				rslt.Add(newObj);
			return (Array)rslt;
		}

		public static Array ArrayAppend(Array arr, object item) {
			ArrayList rslt = (ArrayList)(object)arr.Clone();
			rslt.Add(item);
			return (Array)rslt;
		}

		public static Array ArrayAppendRange(Array arr, Array add) {
			ArrayList l = (ArrayList)(object)arr.Clone();
			for (int i = 0; i < add.Length; i++)
				l.Add(add[i]);
			return (Array)l;
		}

		public static Array CloneArray(Array arr) {
			return arr.Clone();
		}

		public static Array ArrayRemoveAt(Array arr, int index) {
			ArrayList result = (ArrayList)(object)arr.Clone();
			result.RemoveAt(index);
			return (Array)result;
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

		public static void DoForEachAttribute(XMLNode node, XmlAttributeAction a) {
			XMLNamedNodeMap attr = node.Attributes;
			for (int i = 0, n = attr.Length; i < n; i++)
				a((XMLAttribute)attr.Item(i));
		}

		public static void DoForEachChild(XMLNode node, XmlNodeAction a) {
			for (int i = 0, n = node.ChildNodes.Length; i < n; i++)
				a(node.ChildNodes[i]);
		}

		public static int GetNumChildNodes(XMLNode n) {
			return n.HasChildNodes() ? n.ChildNodes.Length : 0;
		}

		public static int GetNumAttributes(XMLNode n) {
			return n.Attributes.Length;
		}

		public static string NodeName(XMLNode n) {
			return n.NodeName;
		}

		public static string NodeValue(XMLNode n) {
			return n.NodeValue;
		}
		
		public static string NodeOuterXml(XMLNode n) {
			return n.Xml;
		}

		public static XMLDocument ParseXml(string xml) {
			return XMLDocumentParser.Parse(xml);
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
			return s.Substring(0, 1).ToLowerCase() + s.Substr(1);
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

		public static Type FindType(string typeName) {
			Type tp = TryFindType(typeName);
			if (Utils.IsNull(tp))
				throw new Exception("The type " + typeName + " does not exist");
			return tp;
		}

		public static Type TryFindType(string typeName) {
			try {
				return Type.GetType(typeName) ?? null;	// We can get 'undefined' if the namespace is defined, but the type is not ...
			}
			catch (Exception) {
				return null;	// ... and we can get an exception if the namespace is not defined.
			}
		}
		
		public static Exception ArgumentException(string argument) {
			throw new Exception("Argument is invalid: " + argument);
		}

		public static Exception ArgumentNullException(string argument) {
			return new Exception("Argument cannot be null: " + argument);
		}

		/* Useful functions but not part of the emulation layer. Should perhaps be somehwere else
		 */

		public static jQuery Parent(jQuery q, string selector) {
			for (q = q.parent(); q.size() > 0 && !q.isInExpression(selector); q = q.parent())
				;
			return q;
		}

		public static jQuery Prev(jQuery q, string selector) {
			for (q = q.prev(); q.size() > 0 && !q.isInExpression(selector); q = q.prev())
				;
			return q;
		}

		public static jQuery Next(jQuery q, string selector) {
			for (q = q.next(); q.size() > 0 && !q.isInExpression(selector); q = q.next())
				;
			return q;
		}

        public static Delegate Wrap(Delegate d) {
            return (Delegate)Script.Literal("function(){{var x=[this];for(var i=0;i<arguments.length;i++)x.push(arguments[i]);{0}.apply({0},x);}}", d);
        }

		public static void RenderControl(IClientCreateControl c, DOMElement parent) {
			DOMElement newEl = JQueryProxy.jQuery(c.Html).get(0);
			parent.AppendChild(newEl);
			c.Attach();
		}

		public static void Ajax(Dictionary data, string url, bool post, Delegate success, Delegate error) {
			jQuery.ajax(new Dictionary("data", data,
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
