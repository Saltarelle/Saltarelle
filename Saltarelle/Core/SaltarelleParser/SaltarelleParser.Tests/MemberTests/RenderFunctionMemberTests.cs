using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Saltarelle;
using Saltarelle.NodeProcessors;
using Rhino.Mocks;
using Saltarelle.Members;
using Rhino.Mocks.Constraints;

namespace SaltarelleParser.Tests {
	[TestClass]
	public class RenderFunctionMemberTests {
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

		private void TestWriteDefinition_Works(bool server) {
			ITemplate tpl = mocks.StrictMock<ITemplate>();
			if (!server)
				Expect.Call(tpl.EnableClientCreate).Return(true);
			mocks.ReplayAll();
		
			var cb = new CodeBuilder();

			var f1 = mocks.StrictMock<IFragment>();
			var f2 = mocks.StrictMock<IFragment>();
			
			using (mocks.Ordered()) {
				Expect.Call(f1.TryMergeWithNext(f2)).Return(null);
				Expect.Call(() => f1.WriteCode(tpl, server ? FragmentCodePoint.ServerRender : FragmentCodePoint.ClientRender, cb)).Do((Action<ITemplate, FragmentCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[a]")));
				Expect.Call(() => f2.WriteCode(tpl, server ? FragmentCodePoint.ServerRender : FragmentCodePoint.ClientRender, cb)).Do((Action<ITemplate, FragmentCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[b]")));
			}
		
			string expected = "private string Method(int p1, string param2) {" + Environment.NewLine
			                +  "	StringBuilder sb = new StringBuilder();" + Environment.NewLine
			                +  "	[a]" + Environment.NewLine
			                +  "	[b]" + Environment.NewLine
			                +  "	return sb.ToString();" + Environment.NewLine
			                +  "}" + Environment.NewLine + Environment.NewLine;
			                       
			mocks.ReplayAll();

			var member = new RenderFunctionMember("Method", "int p1, string param2");
			member.AddFragment(f1);
			member.AddFragment(f2);
			if (server)
				member.WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			else
				member.WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestWriteServerDefinition_Works() {
			TestWriteDefinition_Works(true);
		}

		[TestMethod]
		public void TestWriteClientDefinition_Works() {
			TestWriteDefinition_Works(false);
		}

		[TestMethod]
		public void TestWriteClientDefinition_NothingWrittenIfClientCreateNotEnabled() {
			ITemplate tpl = mocks.StrictMock<ITemplate>();
			Expect.Call(tpl.EnableClientCreate).Return(false);
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new RenderFunctionMember("Test", "").WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}
		
		[TestMethod]
		public void TestWriteCode_NothingWrittenWhenItShouldNot() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			foreach (var cp in new[] { MemberCodePoint.ServerIdChanging, MemberCodePoint.ClientIdChanging, MemberCodePoint.ServerConstructor, MemberCodePoint.ClientConstructor, MemberCodePoint.TransferConstructor, MemberCodePoint.ConfigObjectInit, MemberCodePoint.Attach, MemberCodePoint.AttachSelf }) {
				var cb = new CodeBuilder();
				new RenderFunctionMember("Test", "").WriteCode(tpl, cp, cb);
				Assert.AreEqual("", cb.ToString());
				Assert.AreEqual(0, cb.IndentLevel);
			}
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestRender_Works() {
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			var f1  = mocks.StrictMock<IFragment>();
			var f2  = mocks.StrictMock<IFragment>();
			Expect.Call(f1.TryMergeWithNext(f2)).Return(null);
			Expect.Call(() => f1.Render(null, null, null)).IgnoreArguments().Constraints(Is.Same(tpl), Is.Same(ctl), Is.NotNull()).Do((Action<ITemplate, IInstantiatedTemplateControl, StringBuilder>)((_, __, sb) => sb.Append("[a]")));
			Expect.Call(() => f2.Render(null, null, null)).IgnoreArguments().Constraints(Is.Same(tpl), Is.Same(ctl), Is.NotNull()).Do((Action<ITemplate, IInstantiatedTemplateControl, StringBuilder>)((_, __, sb) => sb.Append("[b]")));
			mocks.ReplayAll();
			var m = new RenderFunctionMember("Test", "");
			m.AddFragment(f1);
			m.AddFragment(f2);
			Assert.AreEqual("[a][b]", m.Render(tpl, ctl));
			mocks.VerifyAll();
		}
	}
}
