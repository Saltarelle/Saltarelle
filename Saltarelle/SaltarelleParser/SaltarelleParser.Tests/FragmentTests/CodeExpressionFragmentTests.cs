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
	[TestClass]
	public class CodeExpressionFragmentTests {
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

		private void TestWriteCode_Works(FragmentCodePoint point) {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new CodeExpressionFragment("[Some code]").WriteCode(tpl, point, cb);
			Assert.AreEqual("sb.Append([Some code]);" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestWriteCode_ServerSideWorks() {
			TestWriteCode_Works(FragmentCodePoint.ServerRender);
		}

		[TestMethod]
		public void TestWriteCode_ClientSideWorks() {
			TestWriteCode_Works(FragmentCodePoint.ClientRender);
		}
		
		[TestMethod]
		public void TestRender_Throws() {
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			mocks.ReplayAll();
			StringBuilder sb = new StringBuilder();
			Globals.AssertThrows(() => new CodeExpressionFragment("[Some code]").Render(tpl, ctl, sb), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}
	}
}
