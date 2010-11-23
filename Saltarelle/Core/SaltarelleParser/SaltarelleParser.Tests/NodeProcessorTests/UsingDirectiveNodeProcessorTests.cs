using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Saltarelle;
using Saltarelle.NodeProcessors;
using Rhino.Mocks;

namespace SaltarelleParser.Tests {
	[TestClass]
	public class UsingDirectiveNodeProcessorTests : NodeProcessorTestBase {
		public UsingDirectiveNodeProcessorTests() {
		}

		[TestMethod]
		public void TestTryProcess_DoesNotParseElement() {
			mocks.ReplayAll();
			Assert.IsFalse(new UsingDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<element></element>"), true, template, renderFunction));
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfNoNamespace() {
			Globals.AssertThrows(() => new UsingDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?using side=\"both\"?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfBadNamespace() {
			Globals.AssertThrows(() => new UsingDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?using namespace=\"a b\" side=\"both\"?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfBadSide() {
			Globals.AssertThrows(() => new UsingDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?using namespace=\"TestType\" side=\"x\"?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfNotRoot() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new UsingDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?using namespace=\"TestType\" side=\"both\"?>"), false, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_ClientOnlyWork() {
			Expect.Call(() => template.AddClientUsingDirective("Namespace.Client"));
			mocks.ReplayAll();
			Assert.IsTrue(new UsingDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?using namespace=\"Namespace.Client\" side=\"client\"?>"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_InterfaceWorksServerSide() {
			Expect.Call(() => template.AddServerUsingDirective("Namespace.Server"));
			mocks.ReplayAll();
			Assert.IsTrue(new UsingDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?using namespace=\"Namespace.Server\" side=\"server\"?>"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_InterfaceWorksBothSides() {
			Expect.Call(() => template.AddServerUsingDirective("Namespace.Both"));
			Expect.Call(() => template.AddClientUsingDirective("Namespace.Both"));
			mocks.ReplayAll();
			Assert.IsTrue(new UsingDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?using namespace=\"Namespace.Both\" side=\"both\"?>"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_InterfaceWorksNoSideAttr() {
			Expect.Call(() => template.AddServerUsingDirective("Namespace.Both"));
			Expect.Call(() => template.AddClientUsingDirective("Namespace.Both"));
			mocks.ReplayAll();
			Assert.IsTrue(new UsingDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?using namespace=\"Namespace.Both\"?>"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}
	}
}
