using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saltarelle;
using Rhino.Mocks;
using Saltarelle.Fragments;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class UntypedMarkupParserTests {
		[Test]
		public void TestParseMarkup_LiteralWorks() {
			string value = " some value ";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value, null);
			Assert.AreEqual(actual, new LiteralFragment(value));
		}

		[Test]
		public void TestParse_EmptyLiteralWorks() {
			string value = "";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value, null);
			Assert.AreEqual(actual, new LiteralFragment(""));
		}

		[Test]
		public void TestParse_LiteralIsHtmlEncoded() {
			string value = "\"";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value, null);
			Assert.AreEqual(actual, new LiteralFragment("&quot;"));
		}

		[Test]
		public void TestParse_OldCodeSyntaxWorksWithTrim() {
			string value = "code: Some Code() ";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value, null);
			Assert.AreEqual(actual, new CodeExpressionFragment("Some Code()"));
		}

		[Test]
		public void TestParse_OldCodeSyntaxWorksNoTrim() {
			string value = "code:Some Code()";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value, null);
			Assert.AreEqual(actual, new CodeExpressionFragment("Some Code()"));
		}

		[Test]
		public void TestParse_NewCodeSyntaxWorksWithTrim() {
			string value = "{= Some Code() }";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value, null);
			Assert.AreEqual(actual, new CodeExpressionFragment("Some Code()"));
		}

		[Test]
		public void TestParse_NewCodeSyntaxWorksNoTrim() {
			string value = "{= Some Code() }";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value, null);
			Assert.AreEqual(actual, new CodeExpressionFragment("Some Code()"));
		}
	}
}
