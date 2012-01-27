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
	public class PositionFragmentTests {
		private MockRepository mocks;

		[SetUp]
		public void SetupRepo() {
			mocks = new MockRepository();
		}

		private void TestWriteCode_Works(FragmentCodePoint point) {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new PositionFragment().WriteCode(tpl, point, cb);
			Assert.AreEqual("sb.Append(PositionHelper.CreateStyle(Position, -1, -1));" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteCode_ServerWorks() {
			TestWriteCode_Works(FragmentCodePoint.ServerRender);
		}

		[Test]
		public void TestWriteCode_ClientWorks() {
			TestWriteCode_Works(FragmentCodePoint.ClientRender);
		}

		[Test]
		public void TestRender_Works() {
			Position pos = PositionHelper.LeftTop(20, 35);
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			Expect.Call(ctl.Position).Return(pos);
			mocks.ReplayAll();

			StringBuilder sb = new StringBuilder();
			new PositionFragment().Render(tpl, ctl, sb);

			Assert.AreEqual(PositionHelper.CreateStyle(pos, -1, -1), sb.ToString());
			mocks.VerifyAll();
		}
	}
}
