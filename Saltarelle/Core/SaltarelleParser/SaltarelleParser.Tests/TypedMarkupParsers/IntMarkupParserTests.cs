using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saltarelle;
using Saltarelle.TypedMarkupParsers;

namespace SaltarelleParser.Tests {
	/// <summary>
	/// Summary description for IntMarkupParserTests
	/// </summary>
	[TestClass]
	public class IntMarkupParserTests {
		private TestContext testContextInstance;

		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		[TestMethod]
		public void TestParse_ValidScalarValueWorks() {
			var actual = new IntMarkupParser().Parse("int", false, "43");
			Assert.AreEqual("43", actual.InitializerString);
			Assert.AreEqual(43, (int)actual.ValueRetriever());
		}

		[TestMethod]
		public void TestParse_InvalidScalarValueThrows() {
			Globals.AssertThrows(() => new IntMarkupParser().Parse("int", false, "43x"), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("int", false, "43x"));
		}

		[TestMethod]
		public void TestParse_ValidArrayValuesWork() {
			var actual = new IntMarkupParser().Parse("int", true, "");
			Assert.AreEqual("new int[] { }", actual.InitializerString);
			Assert.IsTrue(new int[0].SequenceEqual((int[])actual.ValueRetriever()));

			actual = new IntMarkupParser().Parse("int", true, "43");
			Assert.AreEqual("new int[] { 43 }", actual.InitializerString);
			Assert.IsTrue(new int[] { 43 }.SequenceEqual((int[])actual.ValueRetriever()));

			actual = new IntMarkupParser().Parse("int", true, "43|120|0");
			Assert.AreEqual("new int[] { 43, 120, 0 }", actual.InitializerString);
			Assert.IsTrue(new int[] { 43, 120, 0 }.SequenceEqual((int[])actual.ValueRetriever()));
		}

		[TestMethod]
		public void TestParse_InvalidArrayValueThrows() {
			Globals.AssertThrows(() => new IntMarkupParser().Parse("int", true, "43||120"), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("int", true, "43||120"));
		}
	}
}
