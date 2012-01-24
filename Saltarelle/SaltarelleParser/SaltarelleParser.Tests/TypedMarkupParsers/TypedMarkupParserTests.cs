using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saltarelle;
using Rhino.Mocks;

namespace SaltarelleParser.Tests {
	/// <summary>
	/// Summary description for IntMarkupParserTests
	/// </summary>
	[TestClass]
	public class TypedMarkupParserTests {
		private MockRepository mocks;
		private ITypedMarkupParserImpl intImplementer;
		private ITypedMarkupParserImpl strImplementer;
		private TypedMarkupParser parser;

		private TestContext testContextInstance;

		public TestContext TestContext {
			get {
				return testContextInstance;
			}
			set {
				testContextInstance = value;
			}
		}
		
		[TestInitialize]
		public void SetupRepo() {
			mocks = new MockRepository();
			intImplementer = mocks.StrictMock<ITypedMarkupParserImpl>();
			strImplementer = mocks.StrictMock<ITypedMarkupParserImpl>();
			parser = new TypedMarkupParser(new Dictionary<string,ITypedMarkupParserImpl>() { { "int", intImplementer }, { "str", strImplementer } });
		}

		[TestMethod]
		public void TestParse_ValidNonArrayValueWorks() {
			TypedMarkupData result = new TypedMarkupData("", delegate { return new object(); });
			Expect.Call(strImplementer.Parse("str", false, "some:value")).Return(result);
			mocks.ReplayAll();

			var actual = parser.ParseMarkup("str:some:value");

			Assert.AreSame(result, actual);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestParse_ValidArrayValueOldSyntaxWorks() {
			TypedMarkupData result = new TypedMarkupData("", delegate { return new object(); });
			Expect.Call(strImplementer.Parse("str", true, "some:value")).Return(result);
			mocks.ReplayAll();

			var actual = parser.ParseMarkup("arr:str:some:value");

			Assert.AreSame(result, actual);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestParse_ValidArrayValueNewSyntaxWorks() {
			TypedMarkupData result = new TypedMarkupData("", delegate { return new object(); });
			Expect.Call(strImplementer.Parse("str", true, "some:value")).Return(result);
			mocks.ReplayAll();

			var actual = parser.ParseMarkup("str[]:some:value");

			Assert.AreSame(result, actual);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestParse_ErrorIfNoPrefix() {
			mocks.ReplayAll();
			string value = "some value";
			Globals.AssertThrows(() => parser.ParseMarkup(value), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage2(value));
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestParse_ErrorIfArrIsTheOnlyPrefix() {
			mocks.ReplayAll();
			string value = "arr:some value";
			Globals.AssertThrows(() => parser.ParseMarkup(value), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage2(value));
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestParse_ErrorIfBadPrefix() {
			mocks.ReplayAll();
			string value = "bad:some value";
			Globals.AssertThrows(() => parser.ParseMarkup(value), (TemplateErrorException ex) => ex.Message == ParserUtils.MakeTypedMarkupErrorMessage2(value));
			mocks.VerifyAll();
		}
	}
}
