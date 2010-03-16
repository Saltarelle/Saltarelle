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
	public class InstantiatedControlFragmentTests {
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

		private void TestWriteCode_NoChildrenWorks(FragmentCodePoint point) {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new InstantiatedControlFragment("CtlId", false, false).WriteCode(tpl, point, cb);
			Assert.AreEqual("sb.Append(((IControl)CtlId).Html);" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		private void TestWriteCode_CustomInstantiateWorks(FragmentCodePoint point) {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new InstantiatedControlFragment("CtlId", true, false).WriteCode(tpl, point, cb);
			Assert.AreEqual("if (Utils.IsNull(CtlId)) throw new InvalidOperationException(\"The control instance CtlId must be assigned before the control can be rendered.\");" + Environment.NewLine
			              + "sb.Append(((IControl)CtlId).Html);" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		private void TestWriteCode_OneChildWorks(FragmentCodePoint point) {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new InstantiatedControlFragment("CtlId", false, true).WriteCode(tpl, point, cb);
			Assert.AreEqual("((IControlHost)CtlId).SetInnerHtml(CtlId_inner());" + Environment.NewLine
			              + "sb.Append(((IControl)CtlId).Html);" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		private void TestWriteCode_TwoChildenWorks(FragmentCodePoint point) {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new InstantiatedControlFragment("CtlId", false, true).WriteCode(tpl, point, cb);
			Assert.AreEqual("((IControlHost)CtlId).SetInnerHtml(CtlId_inner());" + Environment.NewLine
			              + "sb.Append(((IControl)CtlId).Html);" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestWriteCode_ServerNoChildrenWorks() {
			TestWriteCode_NoChildrenWorks(FragmentCodePoint.ServerRender);
		}

		[TestMethod]
		public void TestWriteCode_ServerCustomInstantiateWorks() {
			TestWriteCode_CustomInstantiateWorks(FragmentCodePoint.ServerRender);
		}

		[TestMethod]
		public void TestWriteCode_ServerOneChildWorks() {
			TestWriteCode_OneChildWorks(FragmentCodePoint.ServerRender);
		}

		[TestMethod]
		public void TestWriteCode_ServerTwoChildenWorks() {
			TestWriteCode_TwoChildenWorks(FragmentCodePoint.ServerRender);
		}

		[TestMethod]
		public void TestWriteCode_ClientNoChildrenWorks() {
			TestWriteCode_NoChildrenWorks(FragmentCodePoint.ClientRender);
		}

		[TestMethod]
		public void TestWriteCode_ClientCustomInstantiateWorks() {
			TestWriteCode_CustomInstantiateWorks(FragmentCodePoint.ClientRender);
		}

		[TestMethod]
		public void TestWriteCode_ClientOneChildWorks() {
			TestWriteCode_OneChildWorks(FragmentCodePoint.ClientRender);
		}

		[TestMethod]
		public void TestWriteCode_ClientTwoChildenWorks() {
			TestWriteCode_TwoChildenWorks(FragmentCodePoint.ClientRender);
		}

		[TestMethod]
		public void TestRender_WorksWithNoInnerFragments() {
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			var me  = mocks.StrictMock<IControl>();
			Expect.Call(ctl.Controls).Return(new Dictionary<string, IControl>() {{ "CtlId", me }});
			Expect.Call(me.Html).Return("[x]");
			mocks.ReplayAll();

			StringBuilder sb = new StringBuilder();
			new InstantiatedControlFragment("CtlId", false, false).Render(tpl, ctl, sb);
			
			Assert.AreEqual("[x]", sb.ToString());
			
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestRender_WorksWithInnerFragments() {
			var rf = mocks.StrictMock<IRenderFunction>();
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			var me  = mocks.StrictMock<IControlHost>();
			Expect.Call(ctl.Controls).Return(new Dictionary<string, IControl>() {{ "CtlId", me }});
			Expect.Call(tpl.GetMember("CtlId_inner")).Return(rf);
			Expect.Call(rf.Render(tpl, ctl)).Return("[x]");
			Expect.Call(() => me.SetInnerHtml("[x]"));
			Expect.Call(me.Html).Return("[z]");
			mocks.ReplayAll();

			StringBuilder sb = new StringBuilder();
			new InstantiatedControlFragment("CtlId", false, true).Render(tpl, ctl, sb);
			
			Assert.AreEqual("[z]", sb.ToString());
			
			mocks.VerifyAll();
		}
	}
}
