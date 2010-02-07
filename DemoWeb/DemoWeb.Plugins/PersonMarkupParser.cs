using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltarelle;
using DemoWeb;

namespace DemoWeb.Plugins {
	internal class StringPersonPair {
		public string s;
		public Person p;
		public StringPersonPair(string s, Person p) {
			this.s = s;
			this.p = p;
		}
	}

	[TypedMarkupParserImpl("person")]
	public class PersonMarkupParser : ITypedMarkupParserImpl {
		private StringPersonPair ParseSingle(string markup) {
			string[] arr = markup.Split(',');
			if (arr.Length != 2)
				return null;
			string s1 = arr[0].Trim(), s2 = arr[1].Trim();
			if (s1 == "" || s2 == "")
				return null;
			return new StringPersonPair("new Person(@\"" + s2.Replace("\"", "\"\"") + "\", @\"" + s1.Replace("\"", "\"\"") + "\")", new Person(s2, s1));
		}
	
		public TypedMarkupData Parse(string registeredPrefix, bool isArray, string value) {
			if (isArray) {
				StringBuilder sb = new StringBuilder();
				sb.Append("new Person[] {");
				Person[] persons;
				if (value.Trim() != "") {
					string[] split = value.Split('|');
					persons = new Person[split.Length];
					for (int i = 0; i < split.Length; i++) {
						StringPersonPair v = ParseSingle(split[i]);
						if (v == null)
							throw new TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage(registeredPrefix, isArray, value));
						sb.Append(i > 0 ? ", " : " ");
						persons[i] = v.p;
						sb.Append(v.s);
					}
				}
				else
					persons = new Person[0];

				sb.Append(" }");
				return new TypedMarkupData(sb.ToString(), delegate() { return persons; });
			}
			else {
				StringPersonPair v = ParseSingle(value);
				if (v == null)
					throw new TemplateErrorException(ParserUtils.MakeTypedMarkupErrorMessage(registeredPrefix, isArray, value));
				return new TypedMarkupData(v.s, delegate { return v.p; });
			}
		}
	}
}
