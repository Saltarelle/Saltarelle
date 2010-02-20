using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Saltarelle;
using Saltarelle.NodeProcessors;
using Rhino.Mocks;
using Saltarelle.Fragments;

namespace SaltarelleParser.Tests {
	internal class ConfigObjectFragmentTestsTestClass1 : IControl {
		public Position Position { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		public string Id { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		public string Html { get { throw new NotImplementedException(); } }
	}

	internal class ConfigObjectFragmentTestsTestClass2 : IControl {
		public Position Position { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		public string Id { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		public string Html { get { throw new NotImplementedException(); } }
	}

	[TestClass]
	public class ConfigObjectFragmentTests {
		private MockRepository mocks;
		private TestContext testContextInstance;

		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		[TestInitialize]
		public void SetupRepo() {
			mocks = new MockRepository();
		}

		[TestMethod]
		public void TestWriteCode_ServerSideWorks() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new ConfigObjectFragment().WriteCode(tpl, FragmentCodePoint.ServerRender, cb);

			Assert.AreEqual("sb.Append(\" __cfg=\\\"\" + Utils.HtmlEncode(Utils.Json(GetConfig())) + \"\\\"\");" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestWriteCode_ClientSideWorks() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new ConfigObjectFragment().WriteCode(tpl, FragmentCodePoint.ClientRender, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}
		
		[TestMethod]
		public void TestRender_Works() {
			Dictionary<string, IControl> controls = new Dictionary<string,IControl>() {
				{ "Id1", new ConfigObjectFragmentTestsTestClass1() },
				{ "Id2", new ConfigObjectFragmentTestsTestClass2() }
			};
			List<string> namedElements = new List<string> { "Elem1", "Elem2" };
		
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			Expect.Call(ctl.Controls).Return(controls);
			Expect.Call(ctl.NamedElementNames).Return(namedElements);
			mocks.ReplayAll();

			StringBuilder sb = new StringBuilder();
			new ConfigObjectFragment().Render(tpl, ctl, sb);

			string expected = Utils.HtmlEncode("{controlTypes:{"
											 +                 "\"Id1\":'SaltarelleParser.Tests.ConfigObjectFragmentTestsTestClass1',"
											 +                 "\"Id2\":'SaltarelleParser.Tests.ConfigObjectFragmentTestsTestClass2'"
											 +               "},"
											 +  "namedElements:['Elem1','Elem2']"
											 + "}");

			Assert.AreEqual(" __cfg=\"" + expected + "\"", sb.ToString());
			mocks.VerifyAll();
		}
	}
}
