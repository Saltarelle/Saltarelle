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
	public class ContainerPropertyMemberTests {
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
			new ContainerPropertyMember().WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual("private Saltarelle.Ioc.IContainer ___Container;" + Environment.NewLine
			              + "[Saltarelle.Ioc.ClientInject]" + Environment.NewLine
			              + "public Saltarelle.Ioc.IContainer __Container {" + Environment.NewLine
			              + "\tget { return ___Container; }" + Environment.NewLine
			              + "\tset { ___Container = value; }" + Environment.NewLine
			              + "}" + Environment.NewLine + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientDefinition_Works() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new ContainerPropertyMember().WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual("private Saltarelle.Ioc.IContainer ___Container;" + Environment.NewLine
			              + "public Saltarelle.Ioc.IContainer __Container {" + Environment.NewLine
			              + "\tget { return ___Container; }" + Environment.NewLine
			              + "\tset { ___Container = value; }" + Environment.NewLine
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
				new PropertyMember("TestId", "Namespace.ServerType", "Namespace.ClientType", AccessModifier._Public, "field", "Namespace.ServerType", "Namespace.ClientType", true, true, null).WriteCode(tpl, cp, cb);
				Assert.AreEqual("", cb.ToString());
				Assert.AreEqual(0, cb.IndentLevel);
			}
			mocks.VerifyAll();
		}

		[Test]
		public void TestInstantiate_Throws() {
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
            var c   = mocks.StrictMock<IContainer>();
			mocks.ReplayAll();
			Globals.AssertThrows(() => new PropertyMember("TestId", "Namespace.ServerType", "Namespace.ClientType", AccessModifier._Public, "field", "Namespace.ServerType", "Namespace.ClientType", true, true, null).Instantiate(tpl, ctl, c), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}
	}
}
