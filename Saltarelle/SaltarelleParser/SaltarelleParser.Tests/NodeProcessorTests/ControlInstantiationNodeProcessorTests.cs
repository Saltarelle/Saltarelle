using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using Saltarelle;
using Saltarelle.NodeProcessors;
using Rhino.Mocks;
using Saltarelle.Members;
using Rhino.Mocks.Constraints;
using Saltarelle.Fragments;
using Is = Rhino.Mocks.Constraints.Is;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class ControlInstantiationNodeProcessorTests : NodeProcessorTestBase {
		public ControlInstantiationNodeProcessorTests() {
		}

		[SetUp]
		public override void SetupRepo() {
			base.SetupRepo();
			SetupResult.For(docProcessor.ParseTypedMarkup(null)).Do((Func<string, TypedMarkupData>)(s => { if (s == "bad") throw new TemplateErrorException("bad"); return new TypedMarkupData(s); })).IgnoreArguments();
		}

		[Test]
		public void TestTryProcess_DoesNotParseOtherNode() {
			mocks.ReplayAll();
			Assert.IsFalse(new ControlInstantiationNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<element></element>"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_AttributesExceptCustomInstantiateMapCorrectly() {
			Expect.Call(template.HasMember("TestId")).Return(false);
			Expect.Call(template.HasMember("Container")).Return(true);
			Expect.Call(() => template.AddMember(new InstantiatedControlMember("TestId", "Namespace.TestType", false, new Dictionary<string, TypedMarkupData>() { { "Attr1", new TypedMarkupData("value1") }, { "Attr2", new TypedMarkupData("value2") } }, new IMember[0])));
			Expect.Call(() => renderFunction.AddDependency(null)).IgnoreArguments().Constraints(Property.Value("Name", "TestId"));
			mocks.ReplayAll();
			Assert.IsTrue(new ControlInstantiationNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<control id=\"TestId\" type=\"Namespace.TestType\" Attr1=\"value1\" Attr2=\"value2\"/>"), false, template, renderFunction));
			Assert.AreEqual("[CONTROL id=TestId inner=0]", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_ContainerMemberIsAddedIfItDoesNotExist() {
			Expect.Call(template.HasMember("TestId")).Return(false);
			Expect.Call(template.HasMember("Container")).Return(false);
			Expect.Call(() => template.AddMember(new InstantiatedControlMember("TestId", "Namespace.TestType", false, new Dictionary<string, TypedMarkupData>(), new IMember[0])));
			Expect.Call(() => template.AddMember(new PropertyMember("Container", "IContainer", "IContainer", AccessModifier._Public, "_container", "IContainer", "IContainer", true, true, null, true)));
			Expect.Call(() => renderFunction.AddDependency(null)).IgnoreArguments().Constraints(Property.Value("Name", "TestId"));
			mocks.ReplayAll();
			Assert.IsTrue(new ControlInstantiationNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<control id=\"TestId\" type=\"Namespace.TestType\"/>"), false, template, renderFunction));
			Assert.AreEqual("[CONTROL id=TestId inner=0]", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_ContainerMemberIsNotAddedTwice() {
			Expect.Call(template.HasMember("TestId")).Return(false);
			Expect.Call(template.HasMember("Container")).Return(true);
			Expect.Call(() => template.AddMember(new InstantiatedControlMember("TestId", "Namespace.TestType", false, new Dictionary<string, TypedMarkupData>(), new IMember[0])));
			Expect.Call(() => renderFunction.AddDependency(null)).IgnoreArguments().Constraints(Property.Value("Name", "TestId"));
			mocks.ReplayAll();
			Assert.IsTrue(new ControlInstantiationNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<control id=\"TestId\" type=\"Namespace.TestType\"/>"), false, template, renderFunction));
			Assert.AreEqual("[CONTROL id=TestId inner=0]", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_RootIsError() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ControlInstantiationNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<control id=\"TestId\" type=\"Namespace.TestType\" Attr1=\"value1\" Attr2=\"value2\"/>"), true, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_InvalidPropertyValueIsError() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ControlInstantiationNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<control id=\"TestId\" type=\"Namespace.TestType\" Attr1=\"value1\" Attr2=\"bad\"/>"), false, template, renderFunction), (TemplateErrorException ex) => ex.Message == "bad");
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}
		
		[Test]
		public void TestTryProcess_CustomInstantiateMapCorrectly() {
			Expect.Call(template.HasMember("Container")).Return(true);
			Expect.Call(template.HasMember("TestId")).Return(false);
			Expect.Call(() => template.AddMember(new InstantiatedControlMember("TestId", "Namespace.TestType", true, new Dictionary<string, TypedMarkupData>(), new IMember[0])));
			Expect.Call(() => renderFunction.AddDependency(null)).IgnoreArguments().Constraints(Property.Value("Name", "TestId"));
			mocks.ReplayAll();
			Assert.IsTrue(new ControlInstantiationNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<control id=\"TestId\" type=\"Namespace.TestType\" customInstantiate=\"true\"/>"), false, template, renderFunction));
			Assert.AreEqual("[CONTROL id=TestId inner=0]", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_InvalidIdCausesError() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ControlInstantiationNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<control id=\"Test.Id\" type=\"Namespace.TestType\" />"), false, template, renderFunction), (TemplateErrorException ex) => true);
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_MissingIdCausesDefault() {
			Expect.Call(template.GetUniqueId()).Return("_test_id");
			Expect.Call(template.HasMember("Container")).Return(true);
			Expect.Call(template.HasMember("_test_id")).Return(false);
			Expect.Call(() => template.AddMember(new InstantiatedControlMember("_test_id", "Namespace.TestType", false, new Dictionary<string, TypedMarkupData>(), new IMember[0])));
			Expect.Call(() => renderFunction.AddDependency(null)).IgnoreArguments().Constraints(Property.Value("Name", "_test_id"));
			mocks.ReplayAll();
			Assert.IsTrue(new ControlInstantiationNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<control type=\"Namespace.TestType\"/>"), false, template, renderFunction));
			Assert.AreEqual("[CONTROL id=_test_id inner=0]", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_MissingTypeIsError() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ControlInstantiationNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<control id=\"TestId\" />"), false, template, renderFunction), (TemplateErrorException ex) => true);
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_CustomInstantiateAndOtherPropertiesIsError() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ControlInstantiationNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<control id=\"TestId\" type=\"Namespace.TestType\" customInstantiate=\"true\" Attr1=\"value1\" Attr2=\"value2\"/>"), false, template, renderFunction), (TemplateErrorException ex) => true);
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}
		
		private RenderFunctionMember CreateRenderFunction(string name, string parameters, params IFragment[] fragments) {
			var result = new RenderFunctionMember(name, parameters);
			foreach (IFragment f in fragments)
				result.AddFragment(f);
			return result;
		}
		
		
		[Test]
		public void TestTryProcess_ChildrenWorks() {
			XmlNode node = Globals.GetXmlNode("<control id=\"TestId\" type=\"TestType\"><elem1/>Text node<elem2><elemx/></elem2>  <elem3/></control>");
			XmlNode n1 = node.SelectSingleNode("elem1");
			XmlNode n2 = n1.NextSibling;
			XmlNode n3 = node.SelectSingleNode("elem2");
			XmlNode n4 = node.SelectSingleNode("elem3");
			
			IMember f1 = null, f2 = null, f3 = null, f4 = null;
			
			Expect.Call(() => renderFunction.AddDependency(null)).IgnoreArguments().Constraints(Property.Value("Name", "TestId"));

			using (mocks.Ordered()) {
				Expect.Call(template.HasMember("TestId")).Return(false);

				Expect.Call(template.HasMember("TestId_inner1")).Return(false);
				Expect.Call(() => template.AddMember(CreateRenderFunction("TestId_inner1", "", new IFragment[0]))).Do((Action<IMember>)(m => f1 = m));
				Expect.Call(() => docProcessor.ProcessRecursive(null, null, null)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((n, tpl, f) => { Assert.AreEqual(n, n1); Assert.AreEqual(tpl, template); f.AddFragment(new LiteralFragment("[n1]")); })).IgnoreArguments();
				Expect.Call(template.HasMember("TestId_inner2")).Return(false);
				Expect.Call(() => template.AddMember(CreateRenderFunction("TestId_inner2", "", new IFragment[0]))).Do((Action<IMember>)(m => f2 = m));
				Expect.Call(() => docProcessor.ProcessRecursive(null, null, null)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((n, tpl, f) => { Assert.AreEqual(n, n2); Assert.AreEqual(tpl, template); f.AddFragment(new LiteralFragment("[n2]")); })).IgnoreArguments();
				Expect.Call(template.HasMember("TestId_inner3")).Return(false);
				Expect.Call(() => template.AddMember(CreateRenderFunction("TestId_inner3", "", new IFragment[0]))).Do((Action<IMember>)(m => f3 = m));
				Expect.Call(() => docProcessor.ProcessRecursive(null, null, null)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((n, tpl, f) => { Assert.AreEqual(n, n3); Assert.AreEqual(tpl, template); f.AddFragment(new LiteralFragment("[n3]")); })).IgnoreArguments();
				Expect.Call(template.HasMember("TestId_inner4")).Return(false);
				Expect.Call(() => template.AddMember(CreateRenderFunction("TestId_inner4", "", new IFragment[0]))).Do((Action<IMember>)(m => f4= m));
				Expect.Call(() => docProcessor.ProcessRecursive(null, null, null)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((n, tpl, f) => { Assert.AreEqual(n, n4); Assert.AreEqual(tpl, template); f.AddFragment(new LiteralFragment("[n4]")); })).IgnoreArguments();

				Expect.Call(template.HasMember("Container")).Return(true);
				Expect.Call(() => template.AddMember(null)).IgnoreArguments().Constraints(Is.Matching((InstantiatedControlMember m) =>
				                                                                                      m.Name == "TestId" &&
				                                                                                      m.TypeName == "TestType" &&
				                                                                                      m.CustomInstantiate == false &&
				                                                                                      m.AdditionalProperties.Count == 0 &&
				                                                                                      m.Dependencies.SequenceEqual(new[] { "TestId_inner1", "TestId_inner2", "TestId_inner3", "TestId_inner4" })));
			}
			
			mocks.ReplayAll();

			Assert.IsTrue(new ControlInstantiationNodeProcessor().TryProcess(docProcessor, node, false, template, renderFunction));
			Assert.AreEqual("[CONTROL id=TestId inner=4]", ConcatenatedFragments);
			Assert.AreEqual(f1, CreateRenderFunction("TestId_inner1", "", new LiteralFragment("[n1]")));
			Assert.AreEqual(f2, CreateRenderFunction("TestId_inner2", "", new LiteralFragment("[n2]")));
			Assert.AreEqual(f3, CreateRenderFunction("TestId_inner3", "", new LiteralFragment("[n3]")));
			Assert.AreEqual(f4, CreateRenderFunction("TestId_inner4", "", new LiteralFragment("[n4]")));

			mocks.VerifyAll();
		}
	}
}
