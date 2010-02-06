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
	public class LiteralFragmentTests {
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
		public void TestTryMergeWithNext_CanNotMergeWithIdFragment() {
			Assert.IsNull(new LiteralFragment("X").TryMergeWithNext(new IdFragment()));
		}

		[TestMethod]
		public void TestTryMergeWithNext_CanNotMergeCDatas() {
			Assert.IsNull(new LiteralFragment("X", true).TryMergeWithNext(new LiteralFragment("Y", true)));
		}

		[TestMethod]
		public void TestTryMergeWithNext_CanNotMergeCDataWithNonCData() {
			Assert.IsNull(new LiteralFragment("X", true).TryMergeWithNext(new LiteralFragment("Y", false)));
		}

		[TestMethod]
		public void TestTryMergeWithNext_CanNotMergeNonCDataWithCData() {
			Assert.IsNull(new LiteralFragment("X", false).TryMergeWithNext(new LiteralFragment("Y", true)));
		}

		[TestMethod]
		public void TestTryMergeWithNext_CanMergeNonCDatas() {
			var actual = new LiteralFragment("X", false).TryMergeWithNext(new LiteralFragment("Y", false));
			Assert.AreEqual(new LiteralFragment("XY"), actual);
		}

		[TestMethod]
		public void TestTryMergeWithNext_MergeFromEmptyWorks() {
			var actual = new LiteralFragment("", false).TryMergeWithNext(new LiteralFragment("Y", false));
			Assert.AreEqual(new LiteralFragment("Y"), actual);
		}

		[TestMethod]
		public void TestTryMergeWithNext_MergeWithEmptyWorks() {
			var actual = new LiteralFragment("X", false).TryMergeWithNext(new LiteralFragment("", false));
			Assert.AreEqual(new LiteralFragment("X"), actual);
		}

		[TestMethod]
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

		[TestMethod]
		public void TestWriteCode_ServerNonCDataWorks() {
			TestWriteCode_NonCDataWorks(FragmentCodePoint.ServerRender);
		}

		[TestMethod]
		public void TestWriteCode_ServerCDataWorks() {
			TestWriteCode_CDataWorks(FragmentCodePoint.ServerRender);
		}

		[TestMethod]
		public void TestWriteCode_ClientNonCDataWorks() {
			TestWriteCode_NonCDataWorks(FragmentCodePoint.ClientRender);
		}

		[TestMethod]
		public void TestWriteCode_ClientCDataWorks() {
			TestWriteCode_CDataWorks(FragmentCodePoint.ClientRender);
		}

		[TestMethod]
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

		[TestMethod]
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
