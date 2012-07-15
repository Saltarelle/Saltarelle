using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using Saltarelle;
using Saltarelle.NodeProcessors;
using Rhino.Mocks;
using Saltarelle.Fragments;
using Is = Rhino.Mocks.Constraints.Is;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class InstantiatedControlFragmentTests {
		private MockRepository mocks;

		[SetUp]
		public void SetupRepo() {
			mocks = new MockRepository();
		}

		private void TestWriteCode_NoChildrenWorks(FragmentCodePoint point) {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new InstantiatedControlFragment("CtlId", false, 0).WriteCode(tpl, point, cb);
			Assert.AreEqual("sb.Append(((IControl)CtlId).Html);" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		private void TestWriteCode_CustomInstantiateWorks(FragmentCodePoint point) {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new InstantiatedControlFragment("CtlId", true, 0).WriteCode(tpl, point, cb);
			Assert.AreEqual("if (CtlId == null) throw new InvalidOperationException(\"The control instance CtlId must be assigned before the control can be rendered.\");" + Environment.NewLine
			              + "sb.Append(((IControl)CtlId).Html);" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		private void TestWriteCode_OneChildWorks(FragmentCodePoint point) {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new InstantiatedControlFragment("CtlId", false, 1).WriteCode(tpl, point, cb);
			Assert.AreEqual("((IControlHost)CtlId).SetInnerFragments(new string[] { CtlId_inner1() });" + Environment.NewLine
			              + "sb.Append(((IControl)CtlId).Html);" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		private void TestWriteCode_TwoChildenWorks(FragmentCodePoint point) {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new InstantiatedControlFragment("CtlId", false, 2).WriteCode(tpl, point, cb);
			Assert.AreEqual("((IControlHost)CtlId).SetInnerFragments(new string[] { CtlId_inner1(), CtlId_inner2() });" + Environment.NewLine
			              + "sb.Append(((IControl)CtlId).Html);" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteCode_ServerNoChildrenWorks() {
			TestWriteCode_NoChildrenWorks(FragmentCodePoint.ServerRender);
		}

		[Test]
		public void TestWriteCode_ServerCustomInstantiateWorks() {
			TestWriteCode_CustomInstantiateWorks(FragmentCodePoint.ServerRender);
		}

		[Test]
		public void TestWriteCode_ServerOneChildWorks() {
			TestWriteCode_OneChildWorks(FragmentCodePoint.ServerRender);
		}

		[Test]
		public void TestWriteCode_ServerTwoChildenWorks() {
			TestWriteCode_TwoChildenWorks(FragmentCodePoint.ServerRender);
		}

		[Test]
		public void TestWriteCode_ClientNoChildrenWorks() {
			TestWriteCode_NoChildrenWorks(FragmentCodePoint.ClientRender);
		}

		[Test]
		public void TestWriteCode_ClientCustomInstantiateWorks() {
			TestWriteCode_CustomInstantiateWorks(FragmentCodePoint.ClientRender);
		}

		[Test]
		public void TestWriteCode_ClientOneChildWorks() {
			TestWriteCode_OneChildWorks(FragmentCodePoint.ClientRender);
		}

		[Test]
		public void TestWriteCode_ClientTwoChildenWorks() {
			TestWriteCode_TwoChildenWorks(FragmentCodePoint.ClientRender);
		}

		[Test]
		public void TestRender_WorksWithNoInnerFragments() {
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			var me  = mocks.StrictMock<IControl>();
			Expect.Call(ctl.Controls).Return(new Dictionary<string, IControl>() {{ "CtlId", me }});
			Expect.Call(me.Html).Return("[x]");
			mocks.ReplayAll();

			StringBuilder sb = new StringBuilder();
			new InstantiatedControlFragment("CtlId", false, 0).Render(tpl, ctl, sb);
			
			Assert.AreEqual("[x]", sb.ToString());
			
			mocks.VerifyAll();
		}

		[Test]
		public void TestRender_WorksWithOneInnerFragments() {
			var rf  = mocks.StrictMock<IRenderFunction>();
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			var me  = mocks.StrictMock<IControlHost>();
			Expect.Call(ctl.Controls).Return(new Dictionary<string, IControl>() {{ "CtlId", me }});
			Expect.Call(tpl.GetMember("CtlId_inner1")).Return(rf);
			Expect.Call(rf.Render(tpl, ctl)).Return("[x]");
			Expect.Call(() => me.SetInnerFragments(null)).IgnoreArguments().Constraints(Is.Matching<string[]>(x => x.Length == 1 && x[0] == "[x]"));
			Expect.Call(me.Html).Return("[z]");
			mocks.ReplayAll();

			StringBuilder sb = new StringBuilder();
			new InstantiatedControlFragment("CtlId", false, 1).Render(tpl, ctl, sb);
			
			Assert.AreEqual("[z]", sb.ToString());
			
			mocks.VerifyAll();
		}

		[Test]
		public void TestRender_WorksWithTwoInnerFragments() {
			var rf1 = mocks.StrictMock<IRenderFunction>();
			var rf2 = mocks.StrictMock<IRenderFunction>();
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			var me  = mocks.StrictMock<IControlHost>();
			Expect.Call(ctl.Controls).Return(new Dictionary<string, IControl>() {{ "CtlId", me }});
			Expect.Call(tpl.GetMember("CtlId_inner1")).Return(rf1);
			Expect.Call(tpl.GetMember("CtlId_inner2")).Return(rf2);
			Expect.Call(rf1.Render(tpl, ctl)).Return("[x]");
			Expect.Call(rf2.Render(tpl, ctl)).Return("[y]");
			Expect.Call(() => me.SetInnerFragments(null)).IgnoreArguments().Constraints(Is.Matching<string[]>(x => x.Length == 2 && x[0] == "[x]" && x[1] == "[y]"));
			Expect.Call(me.Html).Return("[z]");
			mocks.ReplayAll();

			StringBuilder sb = new StringBuilder();
			new InstantiatedControlFragment("CtlId", false, 2).Render(tpl, ctl, sb);
			
			Assert.AreEqual("[z]", sb.ToString());
			
			mocks.VerifyAll();
		}
	}
}
