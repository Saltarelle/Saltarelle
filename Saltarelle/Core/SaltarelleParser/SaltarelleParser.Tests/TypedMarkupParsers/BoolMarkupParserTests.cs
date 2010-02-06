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
	public class BoolMarkupParserTests {
		private TestContext testContextInstance;

		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		[TestMethod]
		public void TestParse_TrueWorks() {
			var actual = new BoolMarkupParser().Parse("bool", false, " true ");
			Assert.AreEqual("true", actual.InitializerString);
			Assert.AreEqual(true, (bool)actual.ValueRetriever());
		}

		[TestMethod]
		public void TestParse_FalseWorks() {
			var actual = new BoolMarkupParser().Parse("bool", false, " false ");
			Assert.AreEqual("false", actual.InitializerString);
			Assert.AreEqual(false, (bool)actual.ValueRetriever());
		}

		[TestMethod]
		public void TestParse_BadValueThrows() {
			string s = "bad";
			Globals.AssertThrows(() => new BoolMarkupParser().Parse("bool", false, s), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("bool", false, s));
		}

		[TestMethod]
		public void TestParse_ArrayValuesWork() {
			var actual = new BoolMarkupParser().Parse("bool", true, "");
			Assert.AreEqual("new bool[] { }", actual.InitializerString);
			Assert.IsTrue(new bool[0].SequenceEqual((bool[])actual.ValueRetriever()));

			actual = new BoolMarkupParser().Parse("bool", true, " true ");
			Assert.AreEqual("new bool[] { true }", actual.InitializerString);
			Assert.IsTrue(new bool[] { true }.SequenceEqual((bool[])actual.ValueRetriever()));

			actual = new BoolMarkupParser().Parse("bool", true, " true | false");
			Assert.AreEqual("new bool[] { true, false }", actual.InitializerString);
			Assert.IsTrue(new bool[] { true, false }.SequenceEqual((bool[])actual.ValueRetriever()));

			actual = new BoolMarkupParser().Parse("bool", true, " true|false|true");
			Assert.AreEqual("new bool[] { true, false, true }", actual.InitializerString);
			Assert.IsTrue(new bool[] { true, false, true }.SequenceEqual((bool[])actual.ValueRetriever()));
		}
	}
}
