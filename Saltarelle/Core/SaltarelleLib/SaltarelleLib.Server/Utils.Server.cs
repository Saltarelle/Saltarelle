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

namespace System {
	// to make it possible to use these Script# attibutes in shared files
	public sealed class RecordAttribute : Attribute {}
	public sealed class PreserveCaseAttribute : Attribute {}
}

namespace Saltarelle {
	public delegate void XmlAttributeAction(XmlAttribute a);
	public delegate void XmlNodeAction(XmlNode n);

	public static partial class Utils {
		private static readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

		public static string BlankImageUrl {
			get { return GlobalServices.GetService<IUrlService>().BlankImageUrl; }
		}

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
		
		public static char CharCode(char ch) {
			return ch;
		}

		public static string RepeatChar(char ch, int count) {
			return new string(ch, count);
		}

		public static double Round(double value, int numDecimals) {
			return Math.Round(value, numDecimals);
		}

		public static double Ceil(double value) {
			return Math.Ceiling(value);
		}
		
		public static int ArrayLength(ArrayList l) {
			return l.Count;
		}

		public static int ArrayLength<T>(List<T> l) {
			return l.Count;
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
			return JsonConvert.SerializeObject(o);
		}

		public static object EvalJson(string s, Type objectType) {
			return JsonConvert.DeserializeObject(s, objectType);
		}
		
		public static T EvalJson<T>(string s) {
			return JsonConvert.DeserializeObject<T>(s);
		}

		public static double NaN {
			get { return double.NaN; }
		}

		public static bool IsNaN(double v) {
			return double.IsNaN(v);
		}
		
		public static string JoinStrings(string separator, string[] value) {
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

		public static T[] ArrayResize<T>(T[] arr, int newSize, T newValue) {
			int oldSize = arr.Length;
			Array.Resize(ref arr, newSize);
			for (int i = oldSize; i < newSize; i++)
				arr[i] = newValue;
			return arr;
		}
		
		public static T[] ArrayAppend<T>(T[] arr, T item) {
			Array.Resize(ref arr, arr.Length + 1);
			arr[arr.Length - 1] = item;
			return arr;
		}
		
		public static T[] ArrayAppendRange<T>(T[] arr, T[] add) {
			int oldLen = arr.Length;
			Array.Resize(ref arr, arr.Length + add.Length);
			for (int i = 0; i < add.Length; i++)
				arr[i + oldLen] = add[i];
			return arr;
		}

		public static T[] CloneArray<T>(T[] arr) {
			return (T[])arr.Clone();
		}
		
		public static T[] ArrayRemoveAt<T>(T[] arr, int index) {
			T[] result = new T[arr.Length - 1];
			for (int i = 0; i < index; i++)
				result[i] = arr[i];
			for (int i = index + 1; i < arr.Length; i++)
				result[i - 1] = arr[i];
			return result;
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
		
		public static void DoForEachAttribute(XmlNode node, XmlAttributeAction a) {
			foreach (XmlAttribute attr in node.Attributes)
				a(attr);
		}

		public static void DoForEachChild(XmlNode node, XmlNodeAction a) {
			foreach (XmlNode c in node.ChildNodes)
				a(c);
		}

		public static int GetNumChildNodes(XmlNode n) {
			return n.HasChildNodes ? n.ChildNodes.Count : 0;
		}

		public static int GetNumAttributes(XmlNode n) {
			return n.Attributes.Count;
		}

		public static string NodeName(XmlNode n) {
			return n.Name;
		}

		public static string NodeValue(XmlNode n) {
			return n.Value;
		}

		public static string NodeOuterXml(XmlNode n) {
			return n.OuterXml;
		}

		public static XmlNode ParseXml(string xml) {
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			return doc.DocumentElement;
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
			return s.Substring(0, 1).ToLowerInvariant() + s.Substring(1);
		}
		
		public static string ToLowerCase(this string s) {
			return s.ToLowerInvariant();
		}

		public static object GetPropertyValue(object o, string property) {
			return o.GetType().GetProperty(property).GetValue(o, null);
		}

		public static void SetPropertyValue(object o, string property, object value) {
			o.GetType().GetProperty(property).SetValue(o, value, null);
		}

		public static string IdAndStyle(string id, Position p, int width, int height) {
			return IdAndStyle(id, p, width, height, null);
		}

		public static Type FindType(string typeName) {
			Type t;
			if (!TryFindType(typeName, out t))
				throw new ArgumentException("The type " + typeName + " was not found in any loaded assembly.");
			return t;
		}

		public static bool TryFindType(string typeName, out Type t) {
			lock (typeCache) {
				if (!typeCache.TryGetValue(typeName, out t)) {
					foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
						t = a.GetType(typeName);
						if (t != null)
							break;
					}
					typeCache[typeName] = t; // perhaps null
				}
			}
			return t != null;
		}

		public static Exception ArgumentException(string argument) {
			return new ArgumentException(argument);
		}

		public static Exception ArgumentNullException(string argument) {
			return new ArgumentNullException(argument);
		}
	}
}
