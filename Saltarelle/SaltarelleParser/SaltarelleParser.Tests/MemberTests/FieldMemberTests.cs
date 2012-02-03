using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using Saltarelle;
using Saltarelle.Ioc;
using Saltarelle.NodeProcessors;
using Rhino.Mocks;
using Saltarelle.Members;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class FieldMemberTests {
		private MockRepository mocks;

		[SetUp]
		public void SetupRepo() {
			mocks = new MockRepository();
		}

		[Test]
		public void TestWriteServerDefinition_Works() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual("private Namespace.ServerType TestId;" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteServerDefinition_NothingWrittenWhenNotServer() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", null, "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientDefinition_WorksWhenClientType() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("private Namespace.ClientType TestId;" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientDefinition_NothingWrittenWhenNotClient() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", null).WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteTransferConstructorCode_WorksWhenBothServerAndClient() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.TransferConstructor, cb);
			Assert.AreEqual("this.TestId = (Namespace.ClientType)__cfg[\"TestId\"];" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteTransferConstructorCode_NothingWrittenWhenNotClient() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", null).WriteCode(tpl, MemberCodePoint.TransferConstructor, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteTransferConstructorCode_NothingWrittenWhenNotServer() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", null, "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.TransferConstructor, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteConfigObjectInitCode_WorksWhenBothServerAndClient() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb);
			Assert.AreEqual("__cfg[\"TestId\"] = this.TestId;" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteConfigObjectInitCode_NothingWrittenWhenNotClient() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", "Namespace.ServerType", null).WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteConfigObjectInitCode_NothingWrittenWhenNotServer() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new FieldMember("TestId", null, "Namespace.ClientType").WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteCode_NothingWrittenWhenItShouldNot() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			foreach (var cp in new[] { MemberCodePoint.ServerIdChanging, MemberCodePoint.ClientIdChanging, MemberCodePoint.ServerConstructor, MemberCodePoint.ClientConstructor, MemberCodePoint.Attach, MemberCodePoint.AttachSelf }) {
				var cb = new CodeBuilder();
				new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").WriteCode(tpl, cp, cb);
				Assert.AreEqual("", cb.ToString());
				Assert.AreEqual(0, cb.IndentLevel);
			}
			mocks.VerifyAll();
		}

		[Test]
		public void TestInstantiate_Throws() {
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			mocks.ReplayAll();
			Globals.AssertThrows(() => new FieldMember("TestId", "Namespace.ServerType", "Namespace.ClientType").Instantiate(tpl, ctl), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}
	}
}
