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
	public class CodeFragmentTests {
		private MockRepository mocks;

		[SetUp]
		public void SetupRepo() {
			mocks = new MockRepository();
		}

		private void TestWriteCode_Works(FragmentCodePoint point) {
			for (int i = -5; i <= 5; i++) {
				var tpl = mocks.StrictMock<ITemplate>();
				mocks.ReplayAll();
			
				CodeBuilder cb = new CodeBuilder();
				for (int x = 0; x < 5; x++)
					cb.Indent();
				new CodeFragment("code", i).WriteCode(tpl, point, cb);
				Assert.AreEqual("\t\t\t\t\tcode" + Environment.NewLine, cb.ToString());
				Assert.AreEqual(5 + i, cb.IndentLevel);
				mocks.VerifyAll();

				tpl = mocks.StrictMock<ITemplate>();
				mocks.ReplayAll();
				cb = new CodeBuilder();
				for (int x = 0; x < 5; x++)
					cb.Indent();
				new CodeFragment(null, i).WriteCode(tpl, point, cb);
				Assert.AreEqual("", cb.ToString());
				Assert.AreEqual(5 + i, cb.IndentLevel);
				mocks.VerifyAll();
			}
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
		public void TestRender_Throws() {
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			mocks.ReplayAll();
			StringBuilder sb = new StringBuilder();
			Globals.AssertThrows(() => new CodeFragment("[Some code]", 0).Render(tpl, ctl, sb), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}
	}
}
