using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saltarelle;
using Saltarelle.TypedMarkupParsers;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class CodeMarkupParserTests {
		[Test]
		public void TestParse_Works() {
			var actual = new CodeMarkupParser().Parse("code", false, " some code ", null);
			Assert.AreEqual(" some code ", actual.InitializerString);
			Globals.AssertThrows(() => actual.ValueRetriever(), (TemplateErrorException ex) => true);
		}

		[Test]
		public void TestParse_ThrowsIfEmpty() {
			Globals.AssertThrows(() => new CodeMarkupParser().Parse("code", true, " ", null), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("code", true, " "));
		}

		[Test]
		public void TestParse_ThrowsIfArray() {
			Globals.AssertThrows(() => new CodeMarkupParser().Parse("code", true, "some code", null), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("code", true, "some code"));
		}
	}
}
