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
using Is = Rhino.Mocks.Constraints.Is;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class InstantiatedControlMemberTests {
		private MockRepository mocks;

		[SetUp]
		public void SetupRepo() {
			mocks = new MockRepository();
		}

		[Test]
		public void TestWriteDefinition_NonCustomInstantiateWorks() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			string expected = "private Namespace.TestType TestId {" + Environment.NewLine
			                + "	get { return (Namespace.TestType)controls[\"TestId\"]; }" + Environment.NewLine
			                + "}" + Environment.NewLine + Environment.NewLine;

			var member = new InstantiatedControlMember("TestId", "Namespace.TestType", false, new Dictionary<string, TypedMarkupData>(), new IMember[0]);

			CodeBuilder cb = new CodeBuilder();
			member.WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);

			cb = new CodeBuilder();
			member.WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteDefinition_CustomInstantiateWorks() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			string expected = "private Namespace.TestType TestId {" + Environment.NewLine
			                + "	get { return (Namespace.TestType)controls[\"TestId\"]; }" + Environment.NewLine
			                + "	set {" + Environment.NewLine
			                + "		controls[\"TestId\"] = value;" + Environment.NewLine
			                + "		if (!string.IsNullOrEmpty(id))" + Environment.NewLine
			                + "			((IControl)controls[\"TestId\"]).Id = id + \"_TestId\";" + Environment.NewLine
			                + "	}" + Environment.NewLine
			                + "}" + Environment.NewLine + Environment.NewLine;

			var member = new InstantiatedControlMember("TestId", "Namespace.TestType", true, new Dictionary<string, TypedMarkupData>(), new IMember[0]);

			CodeBuilder cb = new CodeBuilder();
			member.WriteCode(tpl, MemberCodePoint.ServerDefinition, cb);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);

			cb = new CodeBuilder();
			member.WriteCode(tpl, MemberCodePoint.ClientDefinition, cb);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteIdChangedCode_NothingWritten() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			var member = new InstantiatedControlMember("TestId", "Namespace.TestType", false, new Dictionary<string, TypedMarkupData>(), new IMember[0]);

			CodeBuilder cb = new CodeBuilder();
			member.WriteCode(tpl, MemberCodePoint.ServerIdChanging, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);

			cb = new CodeBuilder();
			member.WriteCode(tpl, MemberCodePoint.ClientIdChanging, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteIdChangedCode_CustomInstantiateWorks() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			var member = new InstantiatedControlMember("TestId", "Namespace.TestType", true, new Dictionary<string, TypedMarkupData>(), new IMember[0]);

			CodeBuilder cb = new CodeBuilder();
			member.WriteCode(tpl, MemberCodePoint.ServerIdChanging, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);

			cb = new CodeBuilder();
			member.WriteCode(tpl, MemberCodePoint.ClientIdChanging, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		private void TestWriteConstructorCode_NonCustomInstantiateWorks(bool server) {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			var cb = new CodeBuilder();

			string expected = "{" + Environment.NewLine
			                + "Namespace.Type c = (Namespace.Type)ControlFactory.CreateControl(typeof(Namespace.Type));" + Environment.NewLine
			                + "c.Prop1 = value1;" + Environment.NewLine
			                + "c.Prop2 = value2;" + Environment.NewLine
			                + "this.controls[\"CtlName\"] = c;" + Environment.NewLine
			                + "}" + Environment.NewLine;

			var member = new InstantiatedControlMember("CtlName", "Namespace.Type", false, new Dictionary<string, TypedMarkupData>() { { "Prop1", new TypedMarkupData("value1") }, { "Prop2", new TypedMarkupData("value2") } }, new IMember[0]);
			if (server)
				member.WriteCode(tpl, MemberCodePoint.ServerConstructor, cb);
			else
				member.WriteCode(tpl, MemberCodePoint.ClientConstructor, cb);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteServerConstructorCode_Works() {
			TestWriteConstructorCode_NonCustomInstantiateWorks(true);
		}

		[Test]
		public void TestWriteClientConstructorCode_Works() {
			TestWriteConstructorCode_NonCustomInstantiateWorks(false);
		}

		[Test]
		public void TestWriteServerConstructorCode_NothingWrittenForCustomInstantiate() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			var cb = new CodeBuilder();
			new InstantiatedControlMember("CtlName", "Namespace.Type", true, new Dictionary<string, TypedMarkupData>(), new IMember[0]).WriteCode(tpl, MemberCodePoint.ServerConstructor, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientConstructorCode_NothingWrittenForCustomInstantiate() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			var cb = new CodeBuilder();
			new InstantiatedControlMember("CtlName", "Namespace.Type", true, new Dictionary<string, TypedMarkupData>(), new IMember[0]).WriteCode(tpl, MemberCodePoint.ClientConstructor, cb);
			Assert.AreEqual("", cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteTransferConstructorCode_NonCustomInstantiateWorks() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			var cb = new CodeBuilder();
			string expected = "this.controls[\"CtlName\"] = (Namespace.Type)ControlFactory.CreateControlWithConfig(typeof(Namespace.Type), __cfg[\"CtlName\"]);" + Environment.NewLine;
			new InstantiatedControlMember("CtlName", "Namespace.Type", false, new Dictionary<string, TypedMarkupData>() { { "Prop1", new TypedMarkupData("value1") }, { "Prop2", new TypedMarkupData("value2") } }, new IMember[0]).WriteCode(tpl, MemberCodePoint.TransferConstructor, cb);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteTransferConstructorCode_CustomInstantiateWorks() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			var cb = new CodeBuilder();
			string expected = "this.controls[\"CtlName\"] = (Namespace.Type)ControlFactory.CreateControlByTypeNameWithConfig((string)__cfg[\"CtlName$type\"], __cfg[\"CtlName\"]);" + Environment.NewLine;
			new InstantiatedControlMember("CtlName", "Namespace.Type", true, new Dictionary<string, TypedMarkupData>(), new IMember[0]).WriteCode(tpl, MemberCodePoint.TransferConstructor, cb);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteConfigObjectInitCode_CorrectForNonCustomInstantiate() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new InstantiatedControlMember("CtlName", "Namespace.Type", false, new Dictionary<string, TypedMarkupData>(), new IMember[0]).WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb);
			Assert.AreEqual("__cfg[\"CtlName\"] = this.CtlName.ConfigObject;" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteConfigObjectInitCode_ControlTypeNameWrittenForCustomInstantiate() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new InstantiatedControlMember("CtlName", "Namespace.Type", true, new Dictionary<string, TypedMarkupData>(), new IMember[0]).WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb);
			Assert.AreEqual("__cfg[\"CtlName$type\"] = this.CtlName.GetType().FullName;" + Environment.NewLine + "__cfg[\"CtlName\"] = this.CtlName.ConfigObject;" + Environment.NewLine, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}
		
		[Test]
		public void TestWriteAttachCode_WorksForCustomInstantiate() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			var cb = new CodeBuilder();
			string expected = "if (Utils.IsNull(this.CtlName)) throw new Exception(\"Must instantiate the control 'CtlName' before attach.\");" + Environment.NewLine + "this.CtlName.Attach();" + Environment.NewLine;
			new InstantiatedControlMember("CtlName", "Namespace.Type", true, new Dictionary<string, TypedMarkupData>() { { "Prop1", new TypedMarkupData("value1") }, { "Prop2", new TypedMarkupData("value2") } }, new IMember[0]).WriteCode(tpl, MemberCodePoint.Attach, cb);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}
		
		[Test]
		public void TestWriteAttachCode_WorksForNonCustomInstantiate() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			var cb = new CodeBuilder();
			string expected = "this.CtlName.Attach();" + Environment.NewLine;
			new InstantiatedControlMember("CtlName", "Namespace.Type", false, new Dictionary<string, TypedMarkupData>() { { "Prop1", new TypedMarkupData("value1") }, { "Prop2", new TypedMarkupData("value2") } }, new IMember[0]).WriteCode(tpl, MemberCodePoint.Attach, cb);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			mocks.VerifyAll();
		}
		
		[Test]
		public void TestWriteCode_NothingWrittenWhenItShouldNot() {
			var tpl = mocks.StrictMock<ITemplate>();
			mocks.ReplayAll();
			foreach (var cp in new[] { MemberCodePoint.AttachSelf }) {
				var cb = new CodeBuilder();
				new InstantiatedControlMember("CtlName", "Namespace.Type", true, new Dictionary<string, TypedMarkupData>(), new IMember[0]).WriteCode(tpl, cp, cb);
				Assert.AreEqual("", cb.ToString());
				Assert.AreEqual(0, cb.IndentLevel);
			}
			mocks.VerifyAll();
		}

		[Test]
		public void TestInstantiate_Works() {
			var propValues = new Dictionary<string, TypedMarkupData>() {
			                     { "IntProperty",    new TypedMarkupData("", () => 42) },
			                     { "StringProperty", new TypedMarkupData("", () => "Test value") },
			                     { "ArrayProperty",  new TypedMarkupData("", () => new int[] { 20, 21 }) }
			                 };

			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
            var container = mocks.StrictMock<IContainer>();

            Expect.Call(container.CreateObjectByTypeName("SaltarelleParser.Tests.TestControlClass")).Return(new TestControlClass());
			TestControlClass addedControl = null;
			Expect.Call(() => ctl.AddControl(null, null)).IgnoreArguments().Constraints(Is.Equal("CtlName"), Is.TypeOf<TestControlClass>()).Do((Action<string, IControl>)((_, c) => addedControl = (TestControlClass)c));
			mocks.ReplayAll();
			CodeBuilder cb = new CodeBuilder();
			new InstantiatedControlMember("CtlName", "SaltarelleParser.Tests.TestControlClass", false, propValues, new IMember[0]).Instantiate(tpl, ctl, container);
			mocks.VerifyAll();

			Assert.AreEqual(42, addedControl.IntProperty);
			Assert.AreEqual("Test value", addedControl.StringProperty);
			Assert.IsTrue(new int[] { 20, 21 }.SequenceEqual(addedControl.ArrayProperty));
		}

		[Test]
		public void TestInstantiate_CustomInstantiateThrows() {
			var tpl = mocks.StrictMock<ITemplate>();
			var ctl = mocks.StrictMock<IInstantiatedTemplateControl>();
			var container = mocks.StrictMock<IContainer>();
			mocks.ReplayAll();
			Globals.AssertThrows(() => new InstantiatedControlMember("CtlName", "Namespace.Type", true, new Dictionary<string, TypedMarkupData>(), new IMember[0]).Instantiate(tpl, ctl, container), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}
	}
	
	public class TestControlClass : IControl {
		public Position Position {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public string Id {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		
		public int    IntProperty    { get; set; }
		public string StringProperty { get; set; }
		public int[]  ArrayProperty  { get; set; }

		public string Html { get { throw new NotImplementedException(); } }
		public object ConfigObject { get { throw new NotImplementedException(); } }
	}
}
