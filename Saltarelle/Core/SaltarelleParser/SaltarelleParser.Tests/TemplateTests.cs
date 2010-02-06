using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saltarelle;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace SaltarelleParser.Tests {
	[TestClass]
	public class TemplateTests {
		MockRepository mocks = new MockRepository();
	
		private TestContext testContextInstance;

		public TestContext TestContext {
			get {
				return testContextInstance;
			}
			set {
				testContextInstance = value;
			}
		}

		[TestInitialize]
		public void SetupRepo() {
			mocks = new MockRepository();
		}

		[TestMethod]
		public void TestWriteServerConstructor_Works() {
			CodeBuilder cb = new CodeBuilder();

			var tpl = new Template();
			tpl.ClassName = "TestClass";

			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();
			
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ServerConstructor, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[a]")));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ServerConstructor, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[b]")));

			mocks.ReplayAll();
			
			string expected =  "public TestClass() {" + Environment.NewLine
			                +  "	GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());" + Environment.NewLine
			                +  "	[a]" + Environment.NewLine
			                +  "	[b]" + Environment.NewLine
			                +  "	Init();" + Environment.NewLine
			                +  "}" + Environment.NewLine;

			Template.WriteServerConstructor(cb, tpl, new List<IMember>() { m1, m2 });
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}

		private void TestWriteClientConstructor_Works(bool enableClientCreate) {
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
			
			string expected =  "public TestClass(string id) {" + Environment.NewLine
			                +  "	if (!Script.IsUndefined(id)) {" + Environment.NewLine
			                +  "		this.id = id;" + Environment.NewLine
			                +  "		this.element = JQueryProxy.jQuery(\"#\" + id);" + Environment.NewLine
			                +  "		Dictionary __cfg = (Dictionary)Utils.EvalJson((string)this.element.attr(\"__cfg\"));" + Environment.NewLine
			                +  "		m1 = f2();" + Environment.NewLine
			                +  "		m2 = g2();" + Environment.NewLine
			                +  "	}" + Environment.NewLine
			                +  "	else {" + Environment.NewLine
			                + (enableClientCreate
			                ?  "		this.position = PositionHelper.NotPositioned;" + Environment.NewLine
			                +  "		m1 = f1();" + Environment.NewLine
			                +  "		m2 = g1();"
			                :  "		throw new Exception(\"This control must be created server-side\");")
			                + Environment.NewLine
			                +  "	}" + Environment.NewLine
			                +  "	Init();" + Environment.NewLine
			                +  "}" + Environment.NewLine;

			Template.WriteClientConstructor(cb, tpl, new List<IMember>() { m1, m2 });
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestWriteClientConstructor_WorksEnableClientCreate() {
			TestWriteClientConstructor_Works(true);
		}

		[TestMethod]
		public void TestWriteClientConstructor_WorksDisableClientCreate() {
			TestWriteClientConstructor_Works(false);
		}

		[TestMethod]
		public void TestWriteServerIdProperty_Works() {
			CodeBuilder cb = new CodeBuilder();
			var tpl = new Template();
			tpl.ClassName = "TestClass";

			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();

			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ServerIdChanged, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[a]")));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ServerIdChanged, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[b]")));

			mocks.ReplayAll();
			
			string expected =  "private string id;" + Environment.NewLine
			                +  "public string Id {" + Environment.NewLine
			                +  "	get { return id; }" + Environment.NewLine
			                +  "	set {" + Environment.NewLine
			                +  "		this.id = value;" + Environment.NewLine
			                +  "		foreach (KeyValuePair<string, IControl> kvp in controls)" + Environment.NewLine
			                +  "			kvp.Value.Id = value + \"_\" + kvp.Key;" + Environment.NewLine
			                +  "		[a]" + Environment.NewLine
			                +  "		[b]" + Environment.NewLine
			                +  "	}" + Environment.NewLine
			                +  "}" + Environment.NewLine;

			Template.WriteServerIdProperty(cb, tpl, new List<IMember>() { m1, m2 });
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestWriteClientIdProperty_Works() {
			CodeBuilder cb = new CodeBuilder();
			var tpl = new Template();
			tpl.ClassName = "TestClass";

			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();
		
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ClientIdChanged, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[a]")));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ClientIdChanged, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[b]")));

			mocks.ReplayAll();
			
			string expected =  "private string id;" + Environment.NewLine
			                +  "public string Id {" + Environment.NewLine
			                +  "	get { return id; }" + Environment.NewLine
			                +  "	set {" + Environment.NewLine
			                +  "		this.id = value;" + Environment.NewLine
			                +  "		foreach (DictionaryEntry kvp in controls)" + Environment.NewLine
			                +  "			((IControl)kvp.Value).Id = value + \"_\" + kvp.Key;" + Environment.NewLine
			                +  "		[a]" + Environment.NewLine
			                +  "		[b]" + Environment.NewLine
			                +  "	}" + Environment.NewLine
			                +  "}" + Environment.NewLine;

			Template.WriteClientIdProperty(cb, tpl, new List<IMember>() { m1, m2 });
			Assert.AreEqual(expected, cb.ToString());
			Assert.AreEqual(0, cb.IndentLevel);
			
			mocks.VerifyAll();
		}
		
		private void TestWriteServerCode_Works(bool withNamespace) {
			CodeBuilder cb = new CodeBuilder();
			var tpl = new Template();
			tpl.ClassName = "TestClass";
			tpl.Nmspace   = withNamespace ? "TestNamespace" : null;

			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();
			Expect.Call(m1.Name).Return("m1").Repeat.Any();
			Expect.Call(m2.Name).Return("m2").Repeat.Any();
			Expect.Call(m1.Dependencies).Return(new string[] { });
			Expect.Call(m2.Dependencies).Return(new string[] { "m1" });

			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ServerIdChanged, cb));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ServerIdChanged, cb));
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
			                 +     Environment.NewLine
			                 +     (withNamespace ? "namespace TestNamespace {" + Environment.NewLine : "")
			                 + p + "public partial class TestClass : IControl, IContainerControl {" + Environment.NewLine
			                 + p + "	private Dictionary<string, IControl> controls = new Dictionary<string, IControl>();" + Environment.NewLine
			                 + p + "	public Dictionary<string, IControl> Controls { get { return controls; } }" + Environment.NewLine
			                 +     Environment.NewLine
			                 + p + "	private Position position = PositionHelper.NotPositioned;" + Environment.NewLine
			                 + p + "	public Position Position { get { return position; } set { position = value; } }" + Environment.NewLine
			                 +     Environment.NewLine
			                 + p + "	private string id;" + Environment.NewLine
			                 + p + "	public string Id {" + Environment.NewLine
			                 + p + "		get { return id; }" + Environment.NewLine
			                 + p + "		set {" + Environment.NewLine
			                 + p + "			this.id = value;" + Environment.NewLine
			                 + p + "			foreach (KeyValuePair<string, IControl> kvp in controls)" + Environment.NewLine
			                 + p + "				kvp.Value.Id = value + \"_\" + kvp.Key;" + Environment.NewLine
			                 + p + "		}" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 +     Environment.NewLine
			                 + p + "	private Dictionary<string, object> GetConfig() {" + Environment.NewLine
			                 + p + "		Dictionary<string, object> __cfg = new Dictionary<string, object>();" + Environment.NewLine
			                 + p + "		return __cfg;" + Environment.NewLine
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
			                 + p + "	public TestClass() {" + Environment.NewLine
			                 + p + "		GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());" + Environment.NewLine
			                 + p + "		Init();" + Environment.NewLine
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

		[TestMethod]
		public void TestWriteServerCode_WorksWithNamespace() {
			TestWriteServerCode_Works(true);
		}

		[TestMethod]
		public void TestWriteServerCode_WorksWithoutNamespace() {
			TestWriteServerCode_Works(false);
		}

		private void TestWriteClientCode_Works(bool withNamespace, bool enableClientCreate) {
			CodeBuilder cb = new CodeBuilder();
			var tpl = new Template();
			tpl.ClassName = "TestClass";
			tpl.Nmspace   = withNamespace ? "TestNamespace" : null;
			tpl.EnableClientCreate = enableClientCreate;

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
			}
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.TransferConstructor, cb));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.TransferConstructor, cb));
			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ClientIdChanged, cb));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ClientIdChanged, cb));

			mocks.ReplayAll();
			
			
			string p = (withNamespace ? "\t" : "");

			string expected  =     "using System;" + Environment.NewLine
			                 +     "using Saltarelle;" + Environment.NewLine
			                 +     Environment.NewLine
			                 +     (withNamespace ? "namespace TestNamespace {" + Environment.NewLine : "")
			                 + p + "public partial class TestClass : IControl, IContainerControl {" + Environment.NewLine
			                 + p + "	private Dictionary controls = new Dictionary();" + Environment.NewLine
			                 + p + "	public Dictionary Controls { get { return controls; } }" + Environment.NewLine
			                 +     Environment.NewLine
			                 + p + "	private Position position;" + Environment.NewLine
			                 + p + "	public Position Position {" + Environment.NewLine
			                 + p + "		get { return element != null ? PositionHelper.GetPosition(element) : position; }" + Environment.NewLine
			                 + p + "		set {" + Environment.NewLine
			                 + p + "			position = value;" + Environment.NewLine
			                 + p + "			if (element != null)" + Environment.NewLine
			                 + p + "				PositionHelper.ApplyPosition(element, value);" + Environment.NewLine
			                 + p + "		}" + Environment.NewLine
			                 + p + "	}" + Environment.NewLine
			                 +     Environment.NewLine
			                 + p + "	private jQuery element;" + Environment.NewLine
			                 + p + "	public jQuery Element { get { return element; } }" + Environment.NewLine
			                 +     Environment.NewLine
			                 + p + "	private string id;" + Environment.NewLine
			                 + p + "	public string Id {" + Environment.NewLine
			                 + p + "		get { return id; }" + Environment.NewLine
			                 + p + "		set {" + Environment.NewLine
			                 + p + "			this.id = value;" + Environment.NewLine
			                 + p + "			foreach (DictionaryEntry kvp in controls)" + Environment.NewLine
			                 + p + "				((IControl)kvp.Value).Id = value + \"_\" + kvp.Key;" + Environment.NewLine
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
			                 + (enableClientCreate
			                 ? p + "	[AlternateSignature]" + Environment.NewLine
			                 + p + "	public extern TestClass();" + Environment.NewLine
			                 : "")
			                 + p + "	public TestClass(string id) {" + Environment.NewLine
			                 + p + "		if (!Script.IsUndefined(id)) {" + Environment.NewLine
			                 + p + "			this.id = id;" + Environment.NewLine
			                 + p + "			this.element = JQueryProxy.jQuery(\"#\" + id);" + Environment.NewLine
			                 + p + "			Dictionary __cfg = (Dictionary)Utils.EvalJson((string)this.element.attr(\"__cfg\"));" + Environment.NewLine
			                 + p + "		}" + Environment.NewLine
			                 + p + "		else {" + Environment.NewLine
			                 + p +
			                 (enableClientCreate
			                     ? "			this.position = PositionHelper.NotPositioned;"
			                     : "			throw new Exception(\"This control must be created server-side\");"
			                 )
			                 + Environment.NewLine
			                 + p + "		}" + Environment.NewLine
			                 + p + "		Init();" + Environment.NewLine
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

		[TestMethod]
		public void TestWriteClientCode_WorksWithNamespace() {
			TestWriteClientCode_Works(true, true);
		}

		[TestMethod]
		public void TestWriteClientCode_WorksWithoutNamespace() {
			TestWriteClientCode_Works(false, true);
		}

		[TestMethod]
		public void TestWriteClientCode_WorksWithoutClientCreate() {
			TestWriteClientCode_Works(true, false);
		}
		
		[TestMethod]
		public void TestWriteGetConfig() {
			CodeBuilder cb = new CodeBuilder();

			var tpl = new Template();
			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();

			Expect.Call(() => m1.WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[a]")));
			Expect.Call(() => m2.WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb)).Do((Action<ITemplate, MemberCodePoint, CodeBuilder>)((_, __, x) => x.AppendLine("[b]")));

			mocks.ReplayAll();

			string expected =  "private Dictionary<string, object> GetConfig() {" + Environment.NewLine
			                +  "	Dictionary<string, object> __cfg = new Dictionary<string, object>();" + Environment.NewLine
			                +  "	[a]" + Environment.NewLine
			                +  "	[b]" + Environment.NewLine
			                +  "	return __cfg;" + Environment.NewLine
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
		
		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
		public void TestTopologicalSort_ThrowsIfUnknownDependency() {
			var members = new Dictionary<string, IMember>();
			AddMember(members, "m1", "x");
			AddMember(members, "m2", "m3");
			mocks.ReplayAll();

			Globals.AssertThrows(() => Template.TopologicalSort(members), (ArgumentException ex) => true);

			mocks.VerifyAll();
		}
		
		[TestMethod]
		public void TestInstantiate() {
			var tpl = new Template();

			IInstantiatedTemplateControl ctl = null;
			var m1 = mocks.StrictMock<IMember>();
			var m2 = mocks.StrictMock<IMember>();
			Expect.Call(m1.Name).Return("m1").Repeat.Any();
			Expect.Call(m2.Name).Return("m2").Repeat.Any();
			Expect.Call(m1.Dependencies).Return(new string[] { });
			Expect.Call(m2.Dependencies).Return(new string[] { "m1" });
			Expect.Call(() => m1.Instantiate(null, null)).IgnoreArguments().Constraints(Is.Same(tpl), Is.NotNull()).Do((Action<ITemplate, IInstantiatedTemplateControl>)((_, c) => { ctl = c; } ));
			Expect.Call(() => m2.Instantiate(null, null)).IgnoreArguments().Constraints(Is.Same(tpl), Is.Matching((IInstantiatedTemplateControl x) => object.ReferenceEquals(x, ctl)));
			tpl.MainRenderFunction.AddFragment(new LiteralFragment("X"));
			
			Globals.RunWithMockedScriptManager(mocks, scr => {
				Expect.Call(() => scr.RegisterType(typeof(InstantiatedTemplateControl)));
				mocks.ReplayAll();

				tpl.AddMember(m1);
				tpl.AddMember(m2);
			
				var actual = tpl.Instantiate();
				Assert.AreSame(ctl, actual);
				actual.Id = "SomeId";
				Assert.AreEqual("X", actual.Html);
			});

			mocks.VerifyAll();
		}
	}
}
