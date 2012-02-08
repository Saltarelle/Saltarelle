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
	public class PropertyMemberTests {
		private MockRepository mocks;

		[SetUp]
		public void SetupRepo() {
			mocks = new MockRepository();
		}

		[Test]
		public void TestWriteServerDefinition_WorksWithGetterAndSetterSameType() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new PropertyMember("TestId", "Namespace.ServerType", "Namespace.ClientType", AccessModifier._Public, "field", "Namespace.ServerType", "Namespace.ClientType", true, true, null, false).WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual("private Namespace.ServerType field;" + Environment.NewLine
			              + "public Namespace.ServerType TestId {" + Environment.NewLine
			              + "\tget { return field; }" + Environment.NewLine
			              + "\tset { field = value; }" + Environment.NewLine
			              + "}" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteServerDefinition_WorksWithValueChangedHook() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new PropertyMember("TestId", "Namespace.ServerType", "Namespace.ClientType", AccessModifier._Public, "field", "Namespace.ServerType", "Namespace.ClientType", true, true, "TestValueChanged", false).WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual("private Namespace.ServerType field;" + Environment.NewLine
			              + "public Namespace.ServerType TestId {" + Environment.NewLine
			              + "\tget { return field; }" + Environment.NewLine
			              + "\tset { field = value; TestValueChanged(); }" + Environment.NewLine
			              + "}" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteServerDefinition_WorksWithGetterAndSetterDifferentTypes() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new PropertyMember("TestId", "Namespace1.ServerPropertyType", "Namespace1.ClientPropertyType", AccessModifier._Public, "field", "Namespace2.ServerFieldType", "Namespace2.ClientFieldType", true, true, null, false).WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual("private Namespace2.ServerFieldType field;" + Environment.NewLine
			              + "public Namespace1.ServerPropertyType TestId {" + Environment.NewLine
			              + "\tget { return (Namespace1.ServerPropertyType)field; }" + Environment.NewLine
			              + "\tset { field = (Namespace2.ServerFieldType)value; }" + Environment.NewLine
			              + "}" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteServerDefinition_WorksWithGetterOnly() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new PropertyMember("ISomeInterface.TestId", "Namespace.ServerType", "Namespace.ClientType", AccessModifier._None, "field", "Namespace.ServerType", "Namespace.ClientType", true, false, null, false).WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual("private Namespace.ServerType field;" + Environment.NewLine
			              + "Namespace.ServerType ISomeInterface.TestId {" + Environment.NewLine
			              + "\tget { return field; }" + Environment.NewLine
			              + "}" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteServerDefinition_WorksWithSetterOnly() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new PropertyMember("TestId", "Namespace.ServerType", "Namespace.ClientType", AccessModifier._ProtectedInternal, "field", "Namespace.ServerType", "Namespace.ClientType", false, true, null, false).WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual("private Namespace.ServerType field;" + Environment.NewLine
			              + "protected internal Namespace.ServerType TestId {" + Environment.NewLine
			              + "\tset { field = value; }" + Environment.NewLine
			              + "}" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteServerDefinition_NothingWrittenWhenNoServerType() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new PropertyMember("TestId", null, "Namespace.ClientType", AccessModifier._Public, "field", null, "Namespace.ClientType", true, true, null, false).WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteServerDefinition_ClientInjectWorks() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new PropertyMember("TestId", "Namespace.ServerType", "Namespace.ClientType", AccessModifier._Public, "field", "Namespace.ServerType", "Namespace.ClientType", true, true, null, true).WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual("private Namespace.ServerType field;" + Environment.NewLine
			              + "[ClientInject]" + Environment.NewLine
			              + "public Namespace.ServerType TestId {" + Environment.NewLine
			              + "\tget { return field; }" + Environment.NewLine
			              + "\tset { field = value; }" + Environment.NewLine
			              + "}" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientDefinition_WorksWithGetterAndSetterSameType() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new PropertyMember("TestId", "Namespace.ServerType", "Namespace.ClientType", AccessModifier._Public, "field", "Namespace.ServerType", "Namespace.ClientType", true, true, null, false).WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("private Namespace.ClientType field;" + Environment.NewLine
			              + "public Namespace.ClientType TestId {" + Environment.NewLine
			              + "\tget { return field; }" + Environment.NewLine
			              + "\tset { field = value; }" + Environment.NewLine
			              + "}" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientDefinition_WorksWithGetterAndSetterDifferentTypes() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new PropertyMember("TestId", "Namespace1.ServerPropertyType", "Namespace1.ClientPropertyType", AccessModifier._Public, "field", "Namespace2.ServerFieldType", "Namespace2.ClientFieldType", true, true, null, false).WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("private Namespace2.ClientFieldType field;" + Environment.NewLine
			              + "public Namespace1.ClientPropertyType TestId {" + Environment.NewLine
			              + "\tget { return (Namespace1.ClientPropertyType)field; }" + Environment.NewLine
			              + "\tset { field = (Namespace2.ClientFieldType)value; }" + Environment.NewLine
			              + "}" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientDefinition_NothingWrittenWhenNoClientType() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new PropertyMember("TestId", "Namespace.ServerType", null, AccessModifier._Public, "field", "Namespace.ServerType", null, true, true, null, false).WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientDefinition_ClientInjectHasNoEffect() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new PropertyMember("TestId", "Namespace.ServerType", "Namespace.ClientType", AccessModifier._Public, "field", "Namespace.ServerType", "Namespace.ClientType", true, true, null, true).WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("private Namespace.ClientType field;" + Environment.NewLine
			              + "public Namespace.ClientType TestId {" + Environment.NewLine
			              + "\tget { return field; }" + Environment.NewLine
			              + "\tset { field = value; }" + Environment.NewLine
			              + "}" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteCode_NothingWrittenWhenItShouldNot() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			foreach (var cp in new[] { MemberCodePoint.ServerIdChanging, MemberCodePoint.ClientIdChanging, MemberCodePoint.ServerConstructor, MemberCodePoint.ClientConstructor, MemberCodePoint.TransferConstructor, MemberCodePoint.ConfigObjectInit, MemberCodePoint.Attach, MemberCodePoint.AttachSelf }) {
				var cb = new CodeBuilder();
				new PropertyMember("TestId", "Namespace.ServerType", "Namespace.ClientType", AccessModifier._Public, "field", "Namespace.ServerType", "Namespace.ClientType", true, true, null, false).WriteCode(tpl, cp, cb);
				Assert.AreEqual("", cb.ToString());
				Assert.AreEqual(0, cb.IndentLevel);
			}
			mocks.VerifyAll();
		}
	}
}
