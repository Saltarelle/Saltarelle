using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Saltarelle;
using Saltarelle.NodeProcessors;
using Rhino.Mocks;
using Saltarelle.Members;

namespace SaltarelleParser.Tests {
	[TestClass]
	public class FieldNodeProcessorTests : NodeProcessorTestBase {
		public FieldNodeProcessorTests() {
		}

		[TestMethod]
		public void TestTryProcess_DoesNotParseOtherNode() {
			mocks.ReplayAll();
			Assert.IsFalse(new FieldNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<element></element>"), true, template, renderFunction));
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_WorksWithTypeSpecified() {
			Expect.Call(template.HasMember("testName")).Return(false);
			Expect.Call(() => template.AddMember(new FieldMember("testName", "TestType", "TestType")));
			mocks.ReplayAll();
			Assert.IsTrue(new FieldNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<field name=\"testName\" type=\"TestType\"/>"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_WorksWithClientAndServerTypesSpecified() {
			Expect.Call(template.HasMember("testName")).Return(false);
			Expect.Call(() => template.AddMember(new FieldMember("testName", "ServerTestType", "ClientTestType")));
			mocks.ReplayAll();
			var actual = new FieldNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<field name=\"testName\" clientType=\"ClientTestType\" serverType=\"ServerTestType\"/>"), true, template, renderFunction);
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfClientTypeMissing() {
			Globals.AssertThrows(() => new FieldNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<field name=\"testName\" serverType=\"ServerTestType\"/>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfServerTypeMissing() {
			Globals.AssertThrows(() => new FieldNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<field name=\"testName\" clientType=\"ClientTestType\"/>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfClientTypeBlank() {
			Globals.AssertThrows(() => new FieldNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<field name=\"testName\" clientType=\" \" serverType=\"ServerTestType\"/>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfServerTypeBlank() {
			Globals.AssertThrows(() => new FieldNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<field name=\"testName\" clientType=\"TestClientType\" serverType=\" \"/>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfBothTypeAndServerType() {
			Globals.AssertThrows(() => new FieldNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<field name=\"testName\" type=\"X\" serverType=\"Y\"/>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfBothTypeAndClientType() {
			Globals.AssertThrows(() => new FieldNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<field name=\"testName\" type=\"X\" clientType=\"Y\"/>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfNameMissing() {
			Globals.AssertThrows(() => new FieldNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<field type=\"TestType\"/>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfNameBlank() {
			Globals.AssertThrows(() => new FieldNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<field name=\" \" type=\"TestType\"/>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfChildElement() {
			Globals.AssertThrows(() => new FieldNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<field name=\"testName\" type=\"TestType\">x</field>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestTryProcess_ErrorIfDuplicateMember() {
			Globals.AssertThrows(() => new FieldNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<field name=\"testName\" type=\"TestType\">x</field>"), true, template, renderFunction), (TemplateErrorException ex) => true);
		}
	}
}
