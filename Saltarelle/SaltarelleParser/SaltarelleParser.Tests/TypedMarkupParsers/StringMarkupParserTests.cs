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
	public class StringMarkupParserTests {
		private TestContext testContextInstance;

		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		[TestMethod]
		public void TestParse_NonEmptyValueWorks() {
			var actual = new StringMarkupParser().Parse("str", false, " Some \"value\" ");
			Assert.AreEqual("@\" Some \"\"value\"\" \"", actual.InitializerString);
			Assert.AreEqual(" Some \"value\" ", (string)actual.ValueRetriever());
		}

		[TestMethod]
		public void TestParse_EmptyValueWorks() {
			var actual = new StringMarkupParser().Parse("str", false, "");
			Assert.AreEqual("@\"\"", actual.InitializerString);
			Assert.AreEqual("", (string)actual.ValueRetriever());
		}

		[TestMethod]
		public void TestParse_ArrayValuesWork() {
			var actual = new StringMarkupParser().Parse("str", true, "");
			Assert.AreEqual("new string[] { }", actual.InitializerString);
			Assert.IsTrue(new string[0].SequenceEqual((string[])actual.ValueRetriever()));

			actual = new StringMarkupParser().Parse("str", true, " ");
			Assert.AreEqual("new string[] { @\" \" }", actual.InitializerString);
			Assert.IsTrue(new string[] { " " }.SequenceEqual((string[])actual.ValueRetriever()));

			actual = new StringMarkupParser().Parse("str", true, " | ");
			Assert.AreEqual("new string[] { @\" \", @\" \" }", actual.InitializerString);
			Assert.IsTrue(new string[] { " ", " " }.SequenceEqual((string[])actual.ValueRetriever()));

			actual = new StringMarkupParser().Parse("str", true, " a || b ");
			Assert.AreEqual("new string[] { @\" a \", @\"\", @\" b \" }", actual.InitializerString);
			Assert.IsTrue(new string[] { " a ", "", " b " }.SequenceEqual((string[])actual.ValueRetriever()));

			actual = new StringMarkupParser().Parse("str", true, "a|\"|b");
			Assert.AreEqual("new string[] { @\"a\", @\"\"\"\", @\"b\" }", actual.InitializerString);
			Assert.IsTrue(new string[] { "a", "\"", "b" }.SequenceEqual((string[])actual.ValueRetriever()));
		}
	}
}
