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

namespace SaltarelleParser.Tests {
	[TestClass]
	public class FieldMemberTests {
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
		public void TestWriteServerDefinition_Works() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual("private Namespace.ServerType TestId;" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestWriteClientDefinition_Works() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("private Namespace.ClientType TestId;" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestWriteServerIdChangedCode_NothingWritten() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.ServerIdChanged, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestWriteClientIdChangedCode_NothingWritten() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.ClientIdChanged, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}
		
		[TestMethod]
		public void TestWriteServerConstructorCode_NothingWritten() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.ServerConstructor, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestWriteClientConstructorCode_NothingWritten() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.ClientConstructor, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestWriteTransferConstructorCode_Works() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.TransferConstructor, cb);
			Assert.AreEqual("this.TestId = (Namespace.ClientType)__cfg[\"TestId\"];" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestWriteConfigObjectInitCode_Works() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb);
			Assert.AreEqual("__cfg[\"TestId\"] = this.TestId;" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestInstantiate_Throws() {
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			mocks.ReplayAll();
			Globals.AssertThrows(() => new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").Instantiate(tpl, ctl), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}
	}
}
