using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Saltarelle;
using Saltarelle.NodeProcessors;
using Rhino.Mocks;

namespace SaltarelleParser.Tests {
	[TestClass]
	public class IdFragmentTests {
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
			new IdFragment().WriteCode(tpl, point, cb);
			Assert.AreEqual("sb.Append(Id);" + Environment.NewLine, cb.ToString());
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
		public void TestRender_Works() {
			string id = "SomeId";
			Position pos = PositionHelper.LeftTop(20, 35);
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			Expect.Call(ctl.Id).Return(id);
			mocks.ReplayAll();

			StringBuilder sb = new StringBuilder();
			new IdFragment().Render(tpl, ctl, sb);

			Assert.AreEqual(id, sb.ToString());
			mocks.VerifyAll();
		}
	}
}
