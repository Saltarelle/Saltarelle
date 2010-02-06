using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saltarelle;
using Rhino.Mocks;

namespace SaltarelleParser.Tests {
	/// <summary>
	/// Summary description for IntMarkupParserTests
	/// </summary>
	[TestClass]
	public class UntypedMarkupParserTests {
		private TestContext testContextInstance;

		public TestContext TestContext {
			get {
				return testContextInstance;
			}
			set {
				testContextInstance = value;
			}
		}
		
		[TestMethod]
		public void TestParseMarkup_LiteralWorks() {
			string value = " some value ";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value);
			Assert.AreEqual(actual, new LiteralFragment(value));
		}

		[TestMethod]
		public void TestParse_EmptyLiteralWorks() {
			string value = "";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value);
			Assert.AreEqual(actual, new LiteralFragment(""));
		}

		[TestMethod]
		public void TestParse_LiteralIsHtmlEncoded() {
			string value = "\"";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value);
			Assert.AreEqual(actual, new LiteralFragment("&quot;"));
		}

		[TestMethod]
		public void TestParse_OldCodeSyntaxWorksWithTrim() {
			string value = "code: Some Code() ";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value);
			Assert.AreEqual(actual, new CodeExpressionFragment("Some Code()"));
		}

		[TestMethod]
		public void TestParse_OldCodeSyntaxWorksNoTrim() {
			string value = "code:Some Code()";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value);
			Assert.AreEqual(actual, new CodeExpressionFragment("Some Code()"));
		}

		[TestMethod]
		public void TestParse_NewCodeSyntaxWorksWithTrim() {
			string value = "{= Some Code() }";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value);
			Assert.AreEqual(actual, new CodeExpressionFragment("Some Code()"));
		}

		[TestMethod]
		public void TestParse_NewCodeSyntaxWorksNoTrim() {
			string value = "{= Some Code() }";
			var actual = new UntypedMarkupParser(null).ParseMarkup(value);
			Assert.AreEqual(actual, new CodeExpressionFragment("Some Code()"));
		}
	}
}
