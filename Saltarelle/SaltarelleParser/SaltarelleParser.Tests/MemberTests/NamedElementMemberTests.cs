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
	public class NamedElementMemberTests {
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
			new NamedElementMember("div", "ElementName").WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientDefinition_Works() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new NamedElementMember("div", "ElementName").WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("private DivElement ElementName { get { return (DivElement)Document.GetElementById(id + \"_ElementName\"); } }" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);

			cb = new CodeBuilder();
			new NamedElementMember("input", "ElementName").WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("private InputElement ElementName { get { return (InputElement)Document.GetElementById(id + \"_ElementName\"); } }" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);

			cb = new CodeBuilder();
			new NamedElementMember("input/x", "ElementName").WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("private InputElement ElementName { get { return (InputElement)Document.GetElementById(id + \"_ElementName\"); } }" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);

			cb = new CodeBuilder();
			new NamedElementMember("input/text", "ElementName").WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("private TextElement ElementName { get { return (TextElement)Document.GetElementById(id + \"_ElementName\"); } }" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);

			cb = new CodeBuilder();
			new NamedElementMember("inputx", "ElementName").WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("private Element ElementName { get { return (Element)Document.GetElementById(id + \"_ElementName\"); } }" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);

			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientIdChangedCode_Works() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new NamedElementMember("div", "TestId").WriteCode(tpl, MemberCodePoint.ClientIdChanging, cb);
			Assert.AreEqual("this.TestId.ID = value + \"_TestId\";" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteCode_NothingWrittenWhenItShouldNot() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			foreach (var cp in new[] { MemberCodePoint.AttachSelf, MemberCodePoint.ServerIdChanging, MemberCodePoint.ServerConstructor, MemberCodePoint.ClientConstructor, MemberCodePoint.TransferConstructor, MemberCodePoint.ConfigObjectInit, MemberCodePoint.Attach }) {
				var cb = new CodeBuilder();
				new NamedElementMember("div", "TestId").WriteCode(tpl, cp, cb);
				Assert.AreEqual("", cb.ToString());
				Assert.AreEqual(0, cb.IndentLevel);
			}
			mocks.VerifyAll();
		}

		[Test]
		public void TestInstantiate_AddsNamedMember() {
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			var container = mocks.StrictMock<IContainer>();
			Expect.Call(() => ctl.AddNamedElement("TestId"));
			mocks.ReplayAll();
			new NamedElementMember("div", "TestId").Instantiate(tpl, ctl, container);
			mocks.VerifyAll();
		}
	}
}
