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
	public class ImplementsOrInheritsNodeProcessorTests : NodeProcessorTestBase {
		public ImplementsOrInheritsNodeProcessorTests() {
		}

		[TestMethod]
		public void TestTryProcess_DoesNotParseElement() {
			mocks.ReplayAll();
			Assert.IsFalse(new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<element></element>"), true, template, renderFunction));
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfNoType() {
			Globals.AssertThrows(() => new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?implements side=\"both\"?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfNoSide() {
			Globals.AssertThrows(() => new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?implements type=\"TestType\"?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfBadSide() {
			Globals.AssertThrows(() => new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?implements type=\"TestType\" side=\"x\"?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfNotRoot() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?implements type=\"TestType\" side=\"both\"?>"), false, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_InterfaceWorksClientSide() {
			Expect.Call(template.ImplementsClientInterface("TestType")).Return(false);
			Expect.Call(() => template.AddClientInterface("TestType"));
			mocks.ReplayAll();
			Assert.IsTrue(new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?implements type=\"TestType\" side=\"client\"?>"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_InterfaceWorksServerSide() {
			Expect.Call(template.ImplementsServerInterface("TestType")).Return(false);
			Expect.Call(() => template.AddServerInterface("TestType"));
			mocks.ReplayAll();
			Assert.IsTrue(new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?implements type=\"TestType\" side=\"server\"?>"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_InterfaceWorksBothSides() {
			Expect.Call(template.ImplementsClientInterface("TestType")).Return(false);
			Expect.Call(template.ImplementsServerInterface("TestType")).Return(false);
			Expect.Call(() => template.AddClientInterface("TestType"));
			Expect.Call(() => template.AddServerInterface("TestType"));
			mocks.ReplayAll();
			Assert.IsTrue(new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?implements type=\"TestType\" side=\"both\"?>"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_InheritsWorksClientSide() {
			Expect.Call(template.ClientInherits).Return(null);
			Expect.Call(() => template.ClientInherits = "TestType");
			mocks.ReplayAll();
			Assert.IsTrue(new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?inherits type=\"TestType\" side=\"client\"?>"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_InheritsWorksServerSide() {
			Expect.Call(template.ServerInherits).Return(null);
			Expect.Call(() => template.ServerInherits = "TestType");
			mocks.ReplayAll();
			Assert.IsTrue(new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?inherits type=\"TestType\" side=\"server\"?>"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_InheritsWorksBothSides() {
			Expect.Call(template.ClientInherits).Return(null);
			Expect.Call(template.ServerInherits).Return(null);
			Expect.Call(() => template.ClientInherits = "TestType");
			Expect.Call(() => template.ServerInherits = "TestType");
			mocks.ReplayAll();
			Assert.IsTrue(new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?inherits type=\"TestType\" side=\"both\"?>"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}
		
		[TestMethod]
		public void TestTryProcess_ErrorIfInheritingFromMoreThanOneBaseClassClientSide() {
			Expect.Call(template.ClientInherits).Return("SomeType");
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?inherits type=\"TestType\" side=\"client\"?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfInheritingFromMoreThanOneBaseClassServerSide() {
			Expect.Call(template.ServerInherits).Return("SomeType");
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?inherits type=\"TestType\" side=\"server\"?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfImplementingInterfaceTwiceServerSide() {
			Expect.Call(template.ImplementsServerInterface("TestType")).Return(true);
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?implements type=\"TestType\" side=\"server\"?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfImplementingInterfaceTwiceClientSide() {
			Expect.Call(template.ImplementsClientInterface("TestType")).Return(true);
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ImplementsOrInheritsNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?implements type=\"TestType\" side=\"client\"?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}
	}
}
