using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saltarelle;
using Rhino.Mocks;
using Saltarelle.Fragments;
using Saltarelle.Ioc;
using Is = Rhino.Mocks.Constraints.Is;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class TemplateTests {
		MockRepository mocks = new MockRepository();

		[SetUp]
		public void SetupRepo() {
			mocks = new MockRepository();
		}

		[Test]
		public void TestWriteServerConstructor_Works() {
			CodeBuilder cb = new CodeBuilder();

			var tpl = new Template();
			tpl.ClassName = "TestClass";

			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();
			
			mocks.ReplayAll();
			
			string expected =  "[Obsolete(@\"" + Template.DoNotCallConstructorMessage.Replace("\"", "\"\"") + "\")]" + Environment.NewLine
                            +  "public TestClass() {" + Environment.NewLine
			                +  "}" + Environment.NewLine;

			Template.WriteServerConstructor(cb, tpl);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteServerDependenciesAvailable_Works() {
			CodeBuilder cb = new CodeBuilder();

			var tpl = new Template();
			tpl.ClassName = "TestClass";

			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();
			
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ServerConstructor, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[a]")));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ServerConstructor, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[b]")));

			mocks.ReplayAll();
			
			string expected =  "public void DependenciesAvailable() {" + Environment.NewLine
			                +  "	[a]" + Environment.NewLine
			                +  "	[b]" + Environment.NewLine
			                +  "	Constructed();" + Environment.NewLine
			                +  "}" + Environment.NewLine;

			Template.WriteServerDependenciesAvailable(cb, tpl, new List<IMember>() { m1, m2 });
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientConstructor_Works() {
			CodeBuilder cb = new CodeBuilder();

			var tpl = new Template();
			tpl.ClassName = "TestClass";

			mocks.ReplayAll();
			
			string expected =  "[Obsolete(@\"" + Template.DoNotCallConstructorMessage.Replace("\"", "\"\"") + "\")]" + Environment.NewLine
			                +  "public TestClass(object config) {" + Environment.NewLine
			                +  "	__cfg = (!Script.IsUndefined(config) ? Dictionary.GetDictionary(config) : null);" + Environment.NewLine
			                +  "}" + Environment.NewLine;

			Template.WriteClientConstructor(cb, tpl);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}

		private void TestWriteClientDependenciesAvailable_Works(bool enableClientCreate) {
			CodeBuilder cb = new CodeBuilder();

			var tpl = new Template();
			tpl.ClassName = "TestClass";
			tpl.EnableClientCreate = enableClientCreate;

			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();

			if (enableClientCreate) {
				Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ClientConstructor, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("m1 = f1();")));
				Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ClientConstructor, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("m2 = g1();")));
			}
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.TransferConstructor, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("m1 = f2();")));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.TransferConstructor, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("m2 = g2();")));

			mocks.ReplayAll();
			
			string expected =  "public void DependenciesAvailable() {" + Environment.NewLine
			                +  "	if (!Utils.IsNull(__cfg)) {" + Environment.NewLine
			                +  "		this.id = (string)__cfg[\"id\"];" + Environment.NewLine
			                +  "		m1 = f2();" + Environment.NewLine
			                +  "		m2 = g2();" + Environment.NewLine
			                +  "		Constructed();" + Environment.NewLine
			                +  "		AttachSelf();" + Environment.NewLine
			                +  "	}" + Environment.NewLine
			                +  "	else {" + Environment.NewLine
			                + (enableClientCreate
			                ?  "		this.position = PositionHelper.NotPositioned;" + Environment.NewLine
			                +  "		m1 = f1();" + Environment.NewLine
			                +  "		m2 = g1();" + Environment.NewLine
			                +  "		Constructed();" + Environment.NewLine
			                :  "		throw new Exception(\"This control must be created server-side\");" + Environment.NewLine
			                )
			                +  "	}" + Environment.NewLine
			                +  "}" + Environment.NewLine;

			Template.WriteClientDependenciesAvailable(cb, tpl, new List<IMember>() { m1, m2 });
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientDependenciesAvailable_WorksEnableClientCreate() {
			TestWriteClientDependenciesAvailable_Works(true);
		}

		[Test]
		public void TestWriteClientDependenciesAvailable_WorksDisableClientCreate() {
			TestWriteClientDependenciesAvailable_Works(false);
		}
		
		[Test]
		public void TestWriteAttach_Works() {
			CodeBuilder cb = new CodeBuilder();

			var tpl = new Template();

			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();
			
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.Attach, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[a]")));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.Attach, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[b]")));

			mocks.ReplayAll();

			string expected =  "public void Attach() {" + Environment.NewLine
			                +  "	if (Script.IsNullOrEmpty(id) || isAttached) throw new Exception(\"Must set id before attach and can only attach once.\");" + Environment.NewLine
			                +  "	[a]" + Environment.NewLine
			                +  "	[b]" + Environment.NewLine
			                +  "	AttachSelf();" + Environment.NewLine
			                +  "}" + Environment.NewLine;

			Template.WriteAttach(cb, tpl, new List<IMember>() { m1, m2 });
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteAttachSelf_Works() {
			CodeBuilder cb = new CodeBuilder();

			var tpl = new Template();

			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();
			
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.AttachSelf, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[a]")));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.AttachSelf, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[b]")));

			mocks.ReplayAll();

			string expected =  "private void AttachSelf() {" + Environment.NewLine
			                +  "	[a]" + Environment.NewLine
			                +  "	[b]" + Environment.NewLine
			                +  "	this.isAttached = true;" + Environment.NewLine
			                +  "	Attached();" + Environment.NewLine
			                +  "}" + Environment.NewLine;

			Template.WriteAttachSelf(cb, tpl, new List<IMember>() { m1, m2 });
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteServerIdProperty_Works() {
			CodeBuilder cb = new CodeBuilder();
			var tpl = new Template();
			tpl.ClassName = "TestClass";

			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();

			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ServerIdChanging, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[a]")));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ServerIdChanging, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[b]")));

			mocks.ReplayAll();
			
			string expected =  "private string id;" + Environment.NewLine
			                +  "public string Id {" + Environment.NewLine
			                +  "	get { return id; }" + Environment.NewLine
			                +  "	set {" + Environment.NewLine
			                +  "		foreach (KeyValuePair<string, IControl> kvp in controls)" + Environment.NewLine
			                +  "			kvp.Value.Id = value + \"_\" + kvp.Key;" + Environment.NewLine
			                +  "		[a]" + Environment.NewLine
			                +  "		[b]" + Environment.NewLine
			                +  "		this.id = value;" + Environment.NewLine
			                +  "	}" + Environment.NewLine
			                +  "}" + Environment.NewLine;

			Template.WriteServerIdProperty(cb, tpl, new List<IMember>() { m1, m2 });
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientIdProperty_Works() {
			CodeBuilder cb = new CodeBuilder();
			var tpl = new Template();
			tpl.ClassName = "TestClass";

			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();
		
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ClientIdChanging, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[a]")));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ClientIdChanging, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[b]")));

			mocks.ReplayAll();
			
			string expected =  "private string id;" + Environment.NewLine
			                +  "public string Id {" + Environment.NewLine
			                +  "	get { return id; }" + Environment.NewLine
			                +  "	set {" + Environment.NewLine
			                +  "		foreach (DictionaryEntry kvp in controls)" + Environment.NewLine
			                +  "			((IControl)kvp.Value).Id = value + \"_\" + kvp.Key;" + Environment.NewLine
			                +  "		[a]" + Environment.NewLine
			                +  "		[b]" + Environment.NewLine
			                +  "		if (isAttached)" + Environment.NewLine
			                +  "			GetElement().ID = value;" + Environment.NewLine
			                +  "		this.id = value;" + Environment.NewLine
			                +  "	}" + Environment.NewLine
			                +  "}" + Environment.NewLine;

			Template.WriteClientIdProperty(cb, tpl, new List<IMember>() { m1, m2 });
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}
		
		private void TestWriteServerCode_Works(bool withNamespace, bool enableClientCreate) {
			CodeBuilder cb = new CodeBuilder();
			var tpl = new Template();
			tpl.ClassName = "TestClass";
			tpl.Nmspace   = withNamespace ? "TestNamespace" : null;
			tpl.EnableClientCreate = enableClientCreate;
			tpl.AddClientUsingDirective("AddedNamespace.Client");
			tpl.AddServerUsingDirective("AddedNamespace.Server");

			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();
			Expect.Call(m1.Name).Return("m1").Repeat.Any();
			Expect.Call(m2.Name).Return("m2").Repeat.Any();
			Expect.Call(m1.Dependencies).Return(new string[] { });
			Expect.Call(m2.Dependencies).Return(new string[] { "m1" });

			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ServerIdChanging, cb));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ServerIdChanging, cb));
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb));
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ServerDefinition, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[a]")));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ServerDefinition, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[b]")));
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ServerConstructor, cb));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ServerConstructor, cb));

			mocks.ReplayAll();
			
			string p = (withNamespace ? "\t" : "");

			string expected  =     "using System;" + Environment.NewLine
			                 +     "using System.Collections.Generic;" + Environment.NewLine
			                 +     "using System.Text;" + Environment.NewLine
			                 +     "using Saltarelle;" + Environment.NewLine
			                 +     "using Saltarelle.Ioc;" + Environment.NewLine
			                 +     "using AddedNamespace.Server;" + Environment.NewLine
			                 +     Environment.NewLine
			                 +     (withNamespace ? "namespace TestNamespace {" + Environment.NewLine : "")
			                 + p + "public partial class TestClass : IControl, INotifyCreated" + (enableClientCreate ? ", IClientCreateControl" : "") + " {" + Environment.NewLine
			                 + p + "	private Dictionary<string, IControl> controls = new Dictionary<string, IControl>();" + Environment.NewLine
			                 +     Environment.NewLine
			                 + p + "	private Position position = PositionHelper.NotPositioned;" + Environment.NewLine
			                 + p + "	public Position Position { get { return position; } set { position = value; } }" + Environment.NewLine
			                 +     Environment.NewLine
			                 + p + "	private string id;" + Environment.NewLine
			                 + p + "	public string Id {" + Environment.NewLine
			                 + p + "		get { return id; }" + Environment.NewLine
			                 + p + "		set {" + Environment.NewLine
			                 + p + "			foreach (KeyValuePair<string, IControl> kvp in controls)" + Environment.NewLine
			                 + p + "				kvp.Value.Id = value + \"_\" + kvp.Key;" + Environment.NewLine
			                 + p + "			this.id = value;" + Environment.NewLine
			                 + p + "		}" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 +     Environment.NewLine
			                 + p + "	public object ConfigObject {" + Environment.NewLine
			                 + p + "		get {" + Environment.NewLine
			                 + p + "			Dictionary<string, object> __cfg = new Dictionary<string, object>();" + Environment.NewLine
			                 + p + "			__cfg[\"id\"] = id;" + Environment.NewLine
			                 + p + "			return __cfg;" + Environment.NewLine
			                 + p + "		}" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 +     Environment.NewLine
			                 + p + "	private string GetHtml() {" + Environment.NewLine
			                 + p + "		StringBuilder sb = new StringBuilder();" + Environment.NewLine
			                 + p + "		return sb.ToString();" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 + Environment.NewLine
			                 + p + "	[a]" + Environment.NewLine
			                 + p + "	[b]" + Environment.NewLine
			                 + p + "	public string Html {" + Environment.NewLine
			                 + p + "		get {" + Environment.NewLine
			                 + p + "			if (string.IsNullOrEmpty(id))" + Environment.NewLine
			                 + p + "				throw new InvalidOperationException(\"Must assign Id before rendering.\");" + Environment.NewLine
			                 + p + "			return GetHtml();" + Environment.NewLine
			                 + p + "		}" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 + Environment.NewLine
                             + p + "	[Obsolete(@\"" + Template.DoNotCallConstructorMessage.Replace("\"", "\"\"") + "\")]" + Environment.NewLine
			                 + p + "	public TestClass() {" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 + Environment.NewLine
			                 + p + "	public void DependenciesAvailable() {" + Environment.NewLine
			                 + p + "		Constructed();" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 + p + "}" + Environment.NewLine
			                 + (withNamespace ? "}" + Environment.NewLine : "");

			tpl.AddMember(m1);
			tpl.AddMember(m2);
			tpl.WriteServerCode(cb);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteServerCode_WorksWithNamespace() {
			TestWriteServerCode_Works(true, false);
		}

		[Test]
		public void TestWriteServerCode_WorksWithoutNamespace() {
			TestWriteServerCode_Works(false, false);
		}

		[Test]
		public void TestWriteServerCode_WorksWithClientCreate() {
			TestWriteServerCode_Works(true, true);
		}

		private void TestWriteClientCode_Works(bool withNamespace, bool enableClientCreate) {
			CodeBuilder cb = new CodeBuilder();
			var tpl = new Template();
			tpl.ClassName = "TestClass";
			tpl.Nmspace   = withNamespace ? "TestNamespace" : null;
			tpl.EnableClientCreate = enableClientCreate;
			tpl.AddClientUsingDirective("AddedNamespace.Client");
			tpl.AddServerUsingDirective("AddedNamespace.Server");

			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();
			Expect.Call(m1.Name).Return("m1").Repeat.Any();
			Expect.Call(m2.Name).Return("m2").Repeat.Any();
			Expect.Call(m1.Dependencies).Return(new string[] { });
			Expect.Call(m2.Dependencies).Return(new string[] { "m1" });

			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ClientDefinition, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[a]")));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ClientDefinition, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[b]")));
			if (enableClientCreate) {
				Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ClientConstructor, cb));
				Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ClientConstructor, cb));
				Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.Attach, cb));
				Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.Attach, cb));
			}
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.TransferConstructor, cb));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.TransferConstructor, cb));
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ClientIdChanging, cb));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ClientIdChanging, cb));
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.AttachSelf, cb));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.AttachSelf, cb));

			mocks.ReplayAll();
			
			
			string p = (withNamespace ? "\t" : "");

			string expected  =     "using System;" + Environment.NewLine
			                 +     "using System.DHTML;" + Environment.NewLine
			                 +     "using Saltarelle;" + Environment.NewLine
			                 +     "using Saltarelle.Ioc;" + Environment.NewLine
			                 +     "using AddedNamespace.Client;" + Environment.NewLine
			                 +     Environment.NewLine
			                 +     (withNamespace ? "namespace TestNamespace {" + Environment.NewLine : "")
			                 + p + "public partial class TestClass : IControl, INotifyCreated" + (enableClientCreate ? ", IClientCreateControl" : "") + " {" + Environment.NewLine
			                 + p + "	private Dictionary controls = new Dictionary();" + Environment.NewLine
			                 + p + "	private Dictionary __cfg;" + Environment.NewLine
			                 +     Environment.NewLine
			                 + p + "	private Position position;" + Environment.NewLine
			                 + p + "	public Position Position {" + Environment.NewLine
			                 + p + "		get { return isAttached ? PositionHelper.GetPosition(GetElement()) : position; }" + Environment.NewLine
			                 + p + "		set {" + Environment.NewLine
			                 + p + "			position = value;" + Environment.NewLine
			                 + p + "			if (isAttached)" + Environment.NewLine
			                 + p + "				PositionHelper.ApplyPosition(GetElement(), value);" + Environment.NewLine
			                 + p + "		}" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 +     Environment.NewLine
			                 + p + "	private bool isAttached = false;" + Environment.NewLine
			                 + p + "	public Element GetElement() { return isAttached ? Document.GetElementById(id) : null; }" + Environment.NewLine
			                 +     Environment.NewLine
			                 + p + "	private string id;" + Environment.NewLine
			                 + p + "	public string Id {" + Environment.NewLine
			                 + p + "		get { return id; }" + Environment.NewLine
			                 + p + "		set {" + Environment.NewLine
			                 + p + "			foreach (DictionaryEntry kvp in controls)" + Environment.NewLine
			                 + p + "				((IControl)kvp.Value).Id = value + \"_\" + kvp.Key;" + Environment.NewLine
			                 + p + "			if (isAttached)" + Environment.NewLine
			                 + p + "				GetElement().ID = value;" + Environment.NewLine
			                 + p + "			this.id = value;" + Environment.NewLine
			                 + p + "		}" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 + Environment.NewLine
			                 + (enableClientCreate
			                 ? p + "	private string GetHtml() {" + Environment.NewLine
			                 + p + "		StringBuilder sb = new StringBuilder();" + Environment.NewLine
			                 + p + "		return sb.ToString();" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 + Environment.NewLine
			                 : "")
			                 + p + "	[a]" + Environment.NewLine
			                 + p + "	[b]" + Environment.NewLine
			                 + p + "	private void AttachSelf() {" + Environment.NewLine
			                 + p + "		this.isAttached = true;" + Environment.NewLine
			                 + p + "		Attached();" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 + Environment.NewLine
			                 + (enableClientCreate
			                 ? p + "	public void Attach() {" + Environment.NewLine
			                 + p + "		if (Script.IsNullOrEmpty(id) || isAttached) throw new Exception(\"Must set id before attach and can only attach once.\");" + Environment.NewLine
			                 + p + "		AttachSelf();" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 + Environment.NewLine
			                 + p + "	public string Html {" + Environment.NewLine
			                 + p + "		get {" + Environment.NewLine
			                 + p + "			if (string.IsNullOrEmpty(id))" + Environment.NewLine
			                 + p + "				throw new InvalidOperationException(\"Must assign Id before rendering.\");" + Environment.NewLine
			                 + p + "			return GetHtml();" + Environment.NewLine
			                 + p + "		}" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 + Environment.NewLine
			                 + p + "	[AlternateSignature]" + Environment.NewLine
			                 + p + "	public TestClass() {}" + Environment.NewLine
			                 : "")
			                 + p + "	[Obsolete(@\"" + Template.DoNotCallConstructorMessage.Replace("\"", "\"\"") + "\")]" + Environment.NewLine
			                 + p + "	public TestClass(object config) {" + Environment.NewLine
			                 + p + "		__cfg = (!Script.IsUndefined(config) ? Dictionary.GetDictionary(config) : null);" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 + Environment.NewLine
			                 + p + "	public void DependenciesAvailable() {" + Environment.NewLine
			                 + p + "		if (!Utils.IsNull(__cfg)) {" + Environment.NewLine
			                 + p + "			this.id = (string)__cfg[\"id\"];" + Environment.NewLine
			                 + p + "			Constructed();" + Environment.NewLine
			                 + p + "			AttachSelf();" + Environment.NewLine
			                 + p + "		}" + Environment.NewLine
			                 + p + "		else {" + Environment.NewLine
			                 + (enableClientCreate
			                 ? p + "			this.position = PositionHelper.NotPositioned;" + Environment.NewLine
			                 + p + "			Constructed();" + Environment.NewLine
			                 : p + "			throw new Exception(\"This control must be created server-side\");" + Environment.NewLine
			                 )
			                 + p + "		}" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 + p + "}" + Environment.NewLine
			                 + (withNamespace ? "}" + Environment.NewLine : "");

			tpl.AddMember(m1);
			tpl.AddMember(m2);
			tpl.WriteClientCode(cb);
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}

		[Test]
		public void TestWriteClientCode_WorksWithNamespace() {
			TestWriteClientCode_Works(true, true);
		}

		[Test]
		public void TestWriteClientCode_WorksWithoutNamespace() {
			TestWriteClientCode_Works(false, true);
		}

		[Test]
		public void TestWriteClientCode_WorksWithoutClientCreate() {
			TestWriteClientCode_Works(true, false);
		}
		
		[Test]
		public void TestWriteGetConfig() {
			CodeBuilder cb = new CodeBuilder();

			var tpl = new Template();
			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();

			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[a]")));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[b]")));

			mocks.ReplayAll();

			string expected =  "public object ConfigObject {" + Environment.NewLine
			                +  "	get {" + Environment.NewLine
			                +  "		Dictionary<string, object> __cfg = new Dictionary<string, object>();" + Environment.NewLine
			                +  "		__cfg[\"id\"] = id;" + Environment.NewLine
			                +  "		[a]" + Environment.NewLine
			                +  "		[b]" + Environment.NewLine
			                +  "		return __cfg;" + Environment.NewLine
			                +  "	}" + Environment.NewLine
			                +  "}" + Environment.NewLine;

			Template.WriteGetConfig(cb, tpl, new List<IMember>() { m1, m2 });

			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);

			mocks.VerifyAll();
		}
		
		private void AddMember(IDictionary<string, IMember> members, string name, params string[] dependencies) {
			var m = mocks.StrictMock<IMember>();
			Expect.Call(m.Name).Return(name).Repeat.Any();
			Expect.Call(m.Dependencies).Return(dependencies);
			members.Add(name, m);
		}
		
		[Test]
		public void TestTopologicalSort_Works() {
			var members = new Dictionary<string, IMember>();
			AddMember(members, "m4", "m2", "m3");
			AddMember(members, "m2", "m1");
			AddMember(members, "m10", "m5");
			AddMember(members, "m8");
			AddMember(members, "m3", "m1");
			AddMember(members, "m1");
			AddMember(members, "m11", "m9", "m10");
			AddMember(members, "m7");
			AddMember(members, "m6", "m8");
			AddMember(members, "m5");
			AddMember(members, "m9", "m7", "m6");
			mocks.ReplayAll();
			
			var result = Template.TopologicalSort(members).Select(m => m.Name).ToList();
			for (int i = 1; i <= 11; i++)
				Assert.IsTrue(result.Contains("m" + Utils.ToStringInvariantInt(i)));
			Assert.IsTrue(result.IndexOf("m2")  > result.IndexOf("m1"));
			Assert.IsTrue(result.IndexOf("m3")  > result.IndexOf("m1"));
			Assert.IsTrue(result.IndexOf("m4")  > result.IndexOf("m3"));
			Assert.IsTrue(result.IndexOf("m4")  > result.IndexOf("m2"));
			Assert.IsTrue(result.IndexOf("m10") > result.IndexOf("m5"));
			Assert.IsTrue(result.IndexOf("m11") > result.IndexOf("m10"));
			Assert.IsTrue(result.IndexOf("m11") > result.IndexOf("m9"));
			Assert.IsTrue(result.IndexOf("m6")  > result.IndexOf("m8"));
			Assert.IsTrue(result.IndexOf("m9")  > result.IndexOf("m7"));
			Assert.IsTrue(result.IndexOf("m9")  > result.IndexOf("m6"));

			mocks.VerifyAll();
		}

		[Test]
		public void TestTopologicalSort_DetectsCycles() {
			var members = new Dictionary<string, IMember>();
			AddMember(members, "m1", "m2");
			AddMember(members, "m2", "m3");
			AddMember(members, "m3", "m1");
			AddMember(members, "m4");
			AddMember(members, "m5");
			mocks.ReplayAll();

			Globals.AssertThrows(() => Template.TopologicalSort(members), (ArgumentException ex) => true);

			mocks.VerifyAll();
		}

		[Test]
		public void TestTopologicalSort_ThrowsIfUnknownDependency() {
			var members = new Dictionary<string, IMember>();
			AddMember(members, "m1", "x");
			AddMember(members, "m2", "m3");
			mocks.ReplayAll();

			Globals.AssertThrows(() => Template.TopologicalSort(members), (ArgumentException ex) => true);

			mocks.VerifyAll();
		}
		
		[Test]
		public void TestInstantiate() {
			var tpl = new Template();

			IInstantiatedTemplateControl ctl = null;
			var m1  = mocks.StrictMock<IMember>();
			var m2  = mocks.StrictMock<IMember>();
			var container = mocks.StrictMock<IContainer>();
			Expect.Call(m1.Name).Return("m1").Repeat.Any();
			Expect.Call(m2.Name).Return("m2").Repeat.Any();
			Expect.Call(m1.Dependencies).Return(new string[] { });
			Expect.Call(m2.Dependencies).Return(new string[] { "m1" });
			Expect.Call(() => m1.Instantiate(null, null, container)).IgnoreArguments().Constraints(Is.Same(tpl), Is.NotNull(), Is.Same(container)).Do((Action<ITemplate, IInstantiatedTemplateControl, IContainer>)((_, c, __) => { ctl = c; } ));
			Expect.Call(() => m2.Instantiate(null, null, container)).IgnoreArguments().Constraints(Is.Same(tpl), Is.Matching((IInstantiatedTemplateControl x) => object.ReferenceEquals(x, ctl)), Is.Same(container));
			tpl.MainRenderFunction.AddFragment(new LiteralFragment("X"));

			mocks.ReplayAll();

			tpl.AddMember(m1);
			tpl.AddMember(m2);
			
			var actual = tpl.Instantiate(container);
			Assert.AreSame(ctl, actual);
			actual.Id = "SomeId";
			Assert.AreEqual("X", actual.Html);

			mocks.VerifyAll();
		}
	}
}
