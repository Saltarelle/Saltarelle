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
	public class LiteralFragmentTests {
		private MockRepository mocks;
	
		[SetUp]
		public void SetupRepo() {
			mocks = new MockRepository();
		}

		[Test]
		public void TestTryMergeWithNext_CanNotMergeWithIdFragment() {
			Assert.IsNull(new LiteralFragment("X").TryMergeWithNext(new IdFragment()));
		}

		[Test]
		public void TestTryMergeWithNext_CanNotMergeCDatas() {
			Assert.IsNull(new LiteralFragment("X", true).TryMergeWithNext(new LiteralFragment("Y", true)));
		}

		[Test]
		public void TestTryMergeWithNext_CanNotMergeCDataWithNonCData() {
			Assert.IsNull(new LiteralFragment("X", true).TryMergeWithNext(new LiteralFragment("Y", false)));
		}

		[Test]
		public void TestTryMergeWithNext_CanNotMergeNonCDataWithCData() {
			Assert.IsNull(new LiteralFragment("X", false).TryMergeWithNext(new LiteralFragment("Y", true)));
		}

		[Test]
		public void TestTryMergeWithNext_CanMergeNonCDatas() {
			var actual = new LiteralFragment("X", false).TryMergeWithNext(new LiteralFragment("Y", false));
			Assert.AreEqual(new LiteralFragment("XY"), actual);
		}

		[Test]
		public void TestTryMergeWithNext_MergeFromEmptyWorks() {
			var actual = new LiteralFragment("", false).TryMergeWithNext(new LiteralFragment("Y", false));
			Assert.AreEqual(new LiteralFragment("Y"), actual);
		}

		[Test]
		public void TestTryMergeWithNext_MergeWithEmptyWorks() {
			var actual = new LiteralFragment("X", false).TryMergeWithNext(new LiteralFragment("", false));
			Assert.AreEqual(new LiteralFragment("X"), actual);
		}

		[Test]
		public void TestTryMergeWithNext_SpacesAreCorrectlyCollapsed() {
			Assert.AreEqual(new LiteralFragment("X Y"), new LiteralFragment("X ", false).TryMergeWithNext(new LiteralFragment(" Y", false)));
			Assert.AreEqual(new LiteralFragment("X Y"), new LiteralFragment("X", false).TryMergeWithNext(new LiteralFragment(" Y", false)));
			Assert.AreEqual(new LiteralFragment("X Y"), new LiteralFragment("X ", false).TryMergeWithNext(new LiteralFragment("Y", false)));
			Assert.AreEqual(new LiteralFragment("X Y"), new LiteralFragment("X ", false).TryMergeWithNext(new LiteralFragment(" Y", false)));
		}
		
		private void TestWriteCode_NonCDataWorks(FragmentCodePoint point) {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new LiteralFragment(" Test \"fragment\"").WriteCode(tpl, point, cb);
			Assert.AreEqual("sb.Append(@\" Test \"\"fragment\"\"\");" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		private void TestWriteCode_CDataWorks(FragmentCodePoint point) {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new LiteralFragment(" Test  \"fragment\"  ", true).WriteCode(tpl, point, cb);
			Assert.AreEqual("sb.Append(@\"<![CDATA[ Test  \"\"fragment\"\"  ]]>\");" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteCode_ServerNonCDataWorks() {
			TestWriteCode_NonCDataWorks(FragmentCodePoint.ServerRender);
		}

		[Test]
		public void TestWriteCode_ServerCDataWorks() {
			TestWriteCode_CDataWorks(FragmentCodePoint.ServerRender);
		}

		[Test]
		public void TestWriteCode_ClientNonCDataWorks() {
			TestWriteCode_NonCDataWorks(FragmentCodePoint.ClientRender);
		}

		[Test]
		public void TestWriteCode_ClientCDataWorks() {
			TestWriteCode_CDataWorks(FragmentCodePoint.ClientRender);
		}

		[Test]
		public void TestRender_NonCDataWorks() {
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			mocks.ReplayAll();
			string text = "Some \"Text\"";
			var sb = new StringBuilder();

			new LiteralFragment(text).Render(tpl, ctl, sb);

			Assert.AreEqual(text, sb.ToString());
			mocks.VerifyAll();
		}

		[Test]
		public void TestRender_CDataWorks() {
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			mocks.ReplayAll();
			string text = "Some \"Text\"";
			var sb = new StringBuilder();

			new LiteralFragment(text, true).Render(tpl, ctl, sb);

			Assert.AreEqual("<![CDATA[" + text + "]]>", sb.ToString());
			mocks.VerifyAll();
		}
	}
}
