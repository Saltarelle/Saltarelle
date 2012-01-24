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
	public class PositionMarkupParserTests {
		private TestContext testContextInstance;

		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}
		
		private void AssertPositionsEqual(Position expected, Position actual) {
			Assert.IsTrue(expected.anchor == actual.anchor && expected.left == actual.left && expected.top == actual.top);
		}

		private void AssertPositionsEqual(Position[] expected, Position[] actual) {
			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
				AssertPositionsEqual(expected[i], actual[i]);
		}

		[TestMethod]
		public void TestParse_NotPositionedWorks() {
			var actual = new PositionMarkupParser().Parse("pos", false, "np");
			Assert.AreEqual("PositionHelper.NotPositioned", actual.InitializerString);
			AssertPositionsEqual(PositionHelper.NotPositioned, (Position)actual.ValueRetriever());
		}

		[TestMethod]
		public void TestParse_FixedWorks() {
			var actual = new PositionMarkupParser().Parse("pos", false, "fixed");
			Assert.AreEqual("PositionHelper.Fixed", actual.InitializerString);
			AssertPositionsEqual(PositionHelper.Fixed, (Position)actual.ValueRetriever());
		}

		[TestMethod]
		public void TestParse_LeftTopWorks() {
			var actual = new PositionMarkupParser().Parse("pos", false, "lt(42, 53)");
			Assert.AreEqual("PositionHelper.LeftTop(42, 53)", actual.InitializerString);
			AssertPositionsEqual(PositionHelper.LeftTop(42, 53), (Position)actual.ValueRetriever());
		}

		[TestMethod]
		public void TestParse_InvalidValuesThrow() {
			foreach (string value in new[] { "bad", "lt", "lt(1, 2, 3)", "lt(1, x)", "lt(x, 1)", "lt(1)" }) {
				Globals.AssertThrows(() => new PositionMarkupParser().Parse("pos", false, value), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("pos", false, value));
			}
		}

		[TestMethod]
		public void TestParse_ValidArrayValuesWork() {
			var actual = new PositionMarkupParser().Parse("pos", true, "");
			Assert.AreEqual("new Position[] { }", actual.InitializerString);
			AssertPositionsEqual(new Position[0], (Position[])actual.ValueRetriever());

			actual = new PositionMarkupParser().Parse("pos", true, "lt(23, 45)");
			Assert.AreEqual("new Position[] { PositionHelper.LeftTop(23, 45) }", actual.InitializerString);
			AssertPositionsEqual(new Position[] { PositionHelper.LeftTop(23, 45) }, (Position[])actual.ValueRetriever());

			actual = new PositionMarkupParser().Parse("pos", true, "np | lt(23, 45) | fixed");
			Assert.AreEqual("new Position[] { PositionHelper.NotPositioned, PositionHelper.LeftTop(23, 45), PositionHelper.Fixed }", actual.InitializerString);
			AssertPositionsEqual(new Position[] { PositionHelper.NotPositioned, PositionHelper.LeftTop(23, 45), PositionHelper.Fixed }, (Position[])actual.ValueRetriever());
		}

		[TestMethod]
		public void TestParse_InvalidArrayValueThrows() {
			string value = "np | bad | fixed";
			Globals.AssertThrows(() => new PositionMarkupParser().Parse("pos", true, value), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("pos", true, value));
		}
	}
}
