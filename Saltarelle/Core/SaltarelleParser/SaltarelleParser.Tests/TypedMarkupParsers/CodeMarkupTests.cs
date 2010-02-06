using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saltarelle;
using Saltarelle.TypedMarkupParsers;

namespace SaltarelleParser.Tests {
	[TestClass]
	public class CodeMarkupParserTests {
		private TestContext testContextInstance;

		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		[TestMethod]
		public void TestParse_Works() {
			var actual = new CodeMarkupParser().Parse("code", false, " some code ");
			Assert.AreEqual(" some code ", actual.InitializerString);
			Globals.AssertThrows(() => actual.ValueRetriever(), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestParse_ThrowsIfEmpty() {
			Globals.AssertThrows(() => new CodeMarkupParser().Parse("code", true, " "), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("code", true, " "));
		}

		[TestMethod]
		public void TestParse_ThrowsIfArray() {
			Globals.AssertThrows(() => new CodeMarkupParser().Parse("code", true, "some code"), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("code", true, "some code"));
		}
	}
}
