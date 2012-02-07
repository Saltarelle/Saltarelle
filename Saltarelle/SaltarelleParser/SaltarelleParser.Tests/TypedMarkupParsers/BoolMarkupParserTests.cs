using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saltarelle;
using Saltarelle.TypedMarkupParsers;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class BoolMarkupParserTests {
		[Test]
		public void TestParse_TrueWorks() {
			var actual = new BoolMarkupParser().Parse("bool", false, " true ", null);
			Assert.AreEqual("true", actual.InitializerString);
			Assert.AreEqual(true, (bool)actual.ValueRetriever());
		}

		[Test]
		public void TestParse_FalseWorks() {
			var actual = new BoolMarkupParser().Parse("bool", false, " false ", null);
			Assert.AreEqual("false", actual.InitializerString);
			Assert.AreEqual(false, (bool)actual.ValueRetriever());
		}

		[Test]
		public void TestParse_BadValueThrows() {
			string s = "bad";
			Globals.AssertThrows(() => new BoolMarkupParser().Parse("bool", false, s, null), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("bool", false, s));
		}

		[Test]
		public void TestParse_ArrayValuesWork() {
			var actual = new BoolMarkupParser().Parse("bool", true, "", null);
			Assert.AreEqual("new bool[] { }", actual.InitializerString);
			Assert.IsTrue(new bool[0].SequenceEqual((bool[])actual.ValueRetriever()));

			actual = new BoolMarkupParser().Parse("bool", true, " true ", null);
			Assert.AreEqual("new bool[] { true }", actual.InitializerString);
			Assert.IsTrue(new bool[] { true }.SequenceEqual((bool[])actual.ValueRetriever()));

			actual = new BoolMarkupParser().Parse("bool", true, " true | false", null);
			Assert.AreEqual("new bool[] { true, false }", actual.InitializerString);
			Assert.IsTrue(new bool[] { true, false }.SequenceEqual((bool[])actual.ValueRetriever()));

			actual = new BoolMarkupParser().Parse("bool", true, " true|false|true", null);
			Assert.AreEqual("new bool[] { true, false, true }", actual.InitializerString);
			Assert.IsTrue(new bool[] { true, false, true }.SequenceEqual((bool[])actual.ValueRetriever()));
		}
	}
}
