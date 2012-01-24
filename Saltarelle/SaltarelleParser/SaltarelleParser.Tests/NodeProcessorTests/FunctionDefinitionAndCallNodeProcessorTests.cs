using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Saltarelle;
using Saltarelle.NodeProcessors;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Saltarelle.Members;
using Saltarelle.Fragments;

namespace SaltarelleParser.Tests {
	[TestClass]
	public class FunctionDefinitionAndCallNodeProcessorTests : NodeProcessorTestBase {
		[TestMethod]
		public void TestTryProcess_DoesNotProcessDirective() {
			mocks.ReplayAll();
			Assert.IsFalse(new FunctionDefinitionAndCallNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?def-fragment ?>"), false, template, renderFunction));
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_DoesNotProcessUnknownElement() {
			mocks.ReplayAll();
			Assert.IsFalse(new FunctionDefinitionAndCallNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<unknown/>"), false, template, renderFunction));
			mocks.VerifyAll();
		}
		
		private void TestTryProcess_DefFragmentWorks(bool hasParams) {
			XmlNode node = Globals.GetXmlNode("<def-fragment name=\"FragName\"" + (hasParams ? " params=\"some param\"" : "") + "><x/><y/></def-fragment>");
			RenderFunctionMember innerFunction = null;

			using (mocks.Ordered()) {
				Expect.Call(template.HasMember("FragName")).Return(false);
				Expect.Call(() => docProcessor.ProcessRecursive(null, null, null)).IgnoreArguments().Constraints(Is.Same(node.ChildNodes[0]), Is.Same(template), Is.NotSame(renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, x) => { innerFunction = (RenderFunctionMember)x; x.AddFragment(new LiteralFragment("[a]")); } ));
				Expect.Call(() => docProcessor.ProcessRecursive(null, null, null)).IgnoreArguments().Constraints(Is.Same(node.ChildNodes[1]), Is.Same(template), Is.NotNull()).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, x) => { Assert.AreSame(innerFunction, x); x.AddFragment(new LiteralFragment("[b]")); } ));
				Expect.Call(template.HasMember("FragName")).Return(false);
				Expect.Call(() => template.AddMember(null)).IgnoreArguments().Do((Action<IMember>)(m => Assert.AreSame(innerFunction, m)));
			}
			mocks.ReplayAll();
			Assert.IsTrue(new FunctionDefinitionAndCallNodeProcessor().TryProcess(docProcessor, node, false, template, innerFunction));
			
			Assert.AreEqual("FragName", innerFunction.Name);
			Assert.AreEqual(hasParams ? "some param" : "", innerFunction.Parameters);
			Assert.IsTrue(new[] { new LiteralFragment("[a]"), new LiteralFragment("[b]") }.SequenceEqual(innerFunction.Fragments));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}
		
		[TestMethod]
		public void TestTryProcess_DefFragmentWorksWithoutParams() {
			TestTryProcess_DefFragmentWorks(false);
		}

		[TestMethod]
		public void TestTryProcess_DefFragmentWorksWithParams() {
			TestTryProcess_DefFragmentWorks(true);
		}
		
		[TestMethod]
		public void TestTryProcess_DefFragmentErrorIfNoName() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new FunctionDefinitionAndCallNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<def-fragment><x/></def-fragment>"), false, template, renderFunction), (TemplateErrorException ex) => true);
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_DefFragmentErrorIfInvalidUnqualifiedName() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new FunctionDefinitionAndCallNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<def-fragment name=\"A.B\"><x/></def-fragment>"), false, template, renderFunction), (TemplateErrorException ex) => true);
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_DefFragmentErrorIfDuplicateName() {
			Expect.Call(template.HasMember("FragName")).Return(true);
			mocks.ReplayAll();
			Globals.AssertThrows(() => new FunctionDefinitionAndCallNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<def-fragment name=\"FragName\"><x/><y/></def-fragment>"), false, template, renderFunction), (TemplateErrorException ex) => true);
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_DefFragmentRootIsError() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new FunctionDefinitionAndCallNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<def-fragment name=\"FragName\"><x/><y/></def-fragment>"), true, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		private void TestTryProcess_CallFragmentWorks(bool hasParams) {
			mocks.ReplayAll();
			Assert.IsTrue(new FunctionDefinitionAndCallNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<call-fragment name=\"FragName\"" + (hasParams ? " params=\"some param\"" : "") + "/>"), false, template, renderFunction));
			Assert.AreEqual(1, fragments.Count);
			Assert.AreEqual(new CodeExpressionFragment("FragName(" + (hasParams ? "some param" : "") + ")"), fragments[0]);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_CallFragmentMissingNameIsError() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new FunctionDefinitionAndCallNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<call-fragment/>"), false, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_CallFragmentInvalidNameIsError() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new FunctionDefinitionAndCallNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<call-fragment name=\"A.B\"/>"), false, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_ChildrenIsError() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new FunctionDefinitionAndCallNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<call-fragment name=\"FragName\"><x/></call-fragment>"), false, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_CallFragmentRootIsError() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new FunctionDefinitionAndCallNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<call-fragment name=\"FragName\"/>"), true, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}
	}
}
