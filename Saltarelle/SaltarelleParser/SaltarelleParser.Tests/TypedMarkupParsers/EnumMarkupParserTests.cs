using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saltarelle;
using Saltarelle.TypedMarkupParsers;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class EnumMarkupParserTests {
		[Test]
		public void TestParse_ValidScalarValueWorks() {
			var actual = new EnumMarkupParser().Parse("enum", false, "System.AttributeTargets.All");
			Assert.AreEqual("System.AttributeTargets.All", actual.InitializerString);
			Assert.AreEqual(AttributeTargets.All, (AttributeTargets)actual.ValueRetriever());
		}

		[Test]
		public void TestParse_ScalarValueWithoutDotThrows() {
			string s = "BadValue";
			Globals.AssertThrows(() => new EnumMarkupParser().Parse("enum", false, s), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("enum", false, s));
		}

		[Test]
		public void TestParse_UnknownValueThrows() {
			string s = "AttributeTargets.BadValue";
			Globals.AssertThrows(() => new EnumMarkupParser().Parse("enum", false, s), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("enum", false, s));
		}

		[Test]
		public void TestParse_InvalidQualifiedNameThrows() {
			string s = "134d.fc f";
			Globals.AssertThrows(() => new EnumMarkupParser().Parse("enum", false, s), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("enum", false, s));
		}

		[Test]
		public void TestParse_ValidArrayValueWorks() {
			var actual = new EnumMarkupParser().Parse("enum", true, "System.AttributeTargets.Assembly");
			Assert.AreEqual("new System.AttributeTargets[] { System.AttributeTargets.Assembly }", actual.InitializerString);
			Assert.IsTrue(new[] { AttributeTargets.Assembly }.SequenceEqual((AttributeTargets[])actual.ValueRetriever()));

			actual = new EnumMarkupParser().Parse("enum", true, "System.AttributeTargets.Assembly | System.AttributeTargets.Class");
			Assert.AreEqual("new System.AttributeTargets[] { System.AttributeTargets.Assembly, System.AttributeTargets.Class }", actual.InitializerString);
			Assert.IsTrue(new[] { AttributeTargets.Assembly, AttributeTargets.Class }.SequenceEqual((AttributeTargets[])actual.ValueRetriever()));

			actual = new EnumMarkupParser().Parse("enum", true, "System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Constructor");
			Assert.AreEqual("new System.AttributeTargets[] { System.AttributeTargets.Assembly, System.AttributeTargets.Class, System.AttributeTargets.Constructor }", actual.InitializerString);
			Assert.IsTrue(new[] { AttributeTargets.Assembly, AttributeTargets.Class, AttributeTargets.Constructor }.SequenceEqual((AttributeTargets[])actual.ValueRetriever()));
		}

		[Test]
		public void TestParse_EmptyArrayValueWorks() {
			var actual = new EnumMarkupParser().Parse("enum", true, "System.AttributeTargets!");
			Assert.AreEqual("new System.AttributeTargets[0]", actual.InitializerString);
			Assert.AreEqual(0, ((AttributeTargets[])actual.ValueRetriever()).Length);
		}

		[Test]
		public void TestParse_DifferentTypesInArrayThrows() {
			string s = "AttributeTargets.Assembly|ConsoleColor.Black";
			Globals.AssertThrows(() => new EnumMarkupParser().Parse("enum", true, s), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("enum", true, s));
		}

		[Test]
		public void TestParse_EmptyArrayThrows() {
			string s = "";
			Globals.AssertThrows(() => new EnumMarkupParser().Parse("enum", true, s), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage("enum", true, s));
		}
	}
}
