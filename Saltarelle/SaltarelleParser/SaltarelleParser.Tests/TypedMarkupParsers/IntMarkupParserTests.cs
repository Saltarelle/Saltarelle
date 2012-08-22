using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saltarelle;
using Saltarelle.TypedMarkupParsers;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class IntMarkupParserTests {
		[Test]
		public void TestParse_ValidScalarValueWorks() {
			var actual = new IntMarkupParser().Parse("int", false, "43", null);
			Assert.AreEqual("43", actual.InitializerString);
			Assert.AreEqual(43, (int)actual.ValueRetriever());
		}

		[Test]
		public void TestParse_InvalidScalarValueThrows() {
			Globals.AssertThrows(() => new IntMarkupParser().Parse("int", false, "43x", null), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("int", false, "43x"));
		}

		[Test]
		public void TestParse_ValidArrayValuesWork() {
			var actual = new IntMarkupParser().Parse("int", true, "", null);
			Assert.AreEqual("new int[] { }", actual.InitializerString);
			Assert.IsTrue(new int[0].SequenceEqual((int[])actual.ValueRetriever()));

			actual = new IntMarkupParser().Parse("int", true, "43", null);
			Assert.AreEqual("new int[] { 43 }", actual.InitializerString);
			Assert.IsTrue(new int[] { 43 }.SequenceEqual((int[])actual.ValueRetriever()));

			actual = new IntMarkupParser().Parse("int", true, "43|120|0", null);
			Assert.AreEqual("new int[] { 43, 120, 0 }", actual.InitializerString);
			Assert.IsTrue(new int[] { 43, 120, 0 }.SequenceEqual((int[])actual.ValueRetriever()));
		}

		[Test]
		public void TestParse_InvalidArrayValueThrows() {
			Globals.AssertThrows(() => new IntMarkupParser().Parse("int", true, "43||120", null), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("int", true, "43||120"));
		}
	}
}
