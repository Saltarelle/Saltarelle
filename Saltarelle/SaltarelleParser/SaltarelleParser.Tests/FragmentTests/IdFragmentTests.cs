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

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class IdFragmentTests {
		private MockRepository mocks;

		[SetUp]
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

		[Test]
		public void TestWriteCode_ServerSideWorks() {
			TestWriteCode_Works(FragmentCodePoint.ServerRender);
		}

		[Test]
		public void TestWriteCode_ClientSideWorks() {
			TestWriteCode_Works(FragmentCodePoint.ClientRender);
		}

		[Test]
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
