using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saltarelle;
using Saltarelle.TypedMarkupParsers;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class StringMarkupParserTests {
		[Test]
		public void TestParse_NonEmptyValueWorks() {
			var actual = new StringMarkupParser().Parse("str", false, " Some \"value\" ", null);
			Assert.AreEqual("@\" Some \"\"value\"\" \"", actual.InitializerString);
			Assert.AreEqual(" Some \"value\" ", (string)actual.ValueRetriever());
		}

		[Test]
		public void TestParse_EmptyValueWorks() {
			var actual = new StringMarkupParser().Parse("str", false, "", null);
			Assert.AreEqual("@\"\"", actual.InitializerString);
			Assert.AreEqual("", (string)actual.ValueRetriever());
		}

		[Test]
		public void TestParse_ArrayValuesWork() {
			var actual = new StringMarkupParser().Parse("str", true, "", null);
			Assert.AreEqual("new string[] { }", actual.InitializerString);
			Assert.IsTrue(new string[0].SequenceEqual((string[])actual.ValueRetriever()));

			actual = new StringMarkupParser().Parse("str", true, " ", null);
			Assert.AreEqual("new string[] { @\" \" }", actual.InitializerString);
			Assert.IsTrue(new string[] { " " }.SequenceEqual((string[])actual.ValueRetriever()));

			actual = new StringMarkupParser().Parse("str", true, " | ", null);
			Assert.AreEqual("new string[] { @\" \", @\" \" }", actual.InitializerString);
			Assert.IsTrue(new string[] { " ", " " }.SequenceEqual((string[])actual.ValueRetriever()));

			actual = new StringMarkupParser().Parse("str", true, " a || b ", null);
			Assert.AreEqual("new string[] { @\" a \", @\"\", @\" b \" }", actual.InitializerString);
			Assert.IsTrue(new string[] { " a ", "", " b " }.SequenceEqual((string[])actual.ValueRetriever()));

			actual = new StringMarkupParser().Parse("str", true, "a|\"|b", null);
			Assert.AreEqual("new string[] { @\"a\", @\"\"\"\", @\"b\" }", actual.InitializerString);
			Assert.IsTrue(new string[] { "a", "\"", "b" }.SequenceEqual((string[])actual.ValueRetriever()));
		}
	}
}
