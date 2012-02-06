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

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class ViewDirectiveNodeProcessorTests : NodeProcessorTestBase {
		public ViewDirectiveNodeProcessorTests() {
		}

		[Test]
		public void TestTryProcess_DoesNotParseElement() {
			mocks.ReplayAll();
			Assert.IsFalse(new ViewDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<element></element>"), true, template, renderFunction));
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_DoesNotUnknownDirective() {
			mocks.ReplayAll();
			Assert.IsFalse(new ViewDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?unknown ?>"), true, template, renderFunction));
			mocks.VerifyAll();
		}

		private void TestTryProcess_WorksWithType(bool hasServerType, bool hasClientType) {
			string actualServerType = (hasServerType ? "ServerType" : "object");
			Expect.Call(template.ImplementsServerInterface("Saltarelle.Mvc.IView<" + actualServerType + ">")).Return(false);
			Expect.Call(() => template.AddServerInterface("Saltarelle.Mvc.IView<" + actualServerType + ">"));
			Expect.Call(template.HasMember("model")).Return(false);
			Expect.Call(() => template.AddMember(new FieldMember("model", actualServerType, hasClientType ? "ClientType" : null)));
			Expect.Call(template.HasMember("Model")).Return(false);
			Expect.Call(() => template.AddMember(new PropertyMember("Model", actualServerType, null, AccessModifier._Public, "model", actualServerType, null, true, true, "ModelChanged", false)));
			Expect.Call(template.HasMember("Saltarelle.Mvc.IView.Model")).Return(false);
			Expect.Call(() => template.AddMember(new PropertyMember("Saltarelle.Mvc.IView.Model", "object", null, AccessModifier._None, "model", actualServerType, null, true, true, "ModelChanged", false)));
			mocks.ReplayAll();

			new ViewDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?view" + (hasServerType ? " modelType=\"ServerType\"" : "") + (hasClientType ? " clientModelType=\"ClientType\"" : "") + "?>"), true, template, renderFunction);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_WorksWithServerTypeWithoutClientType() {
			TestTryProcess_WorksWithType(true, false);
		}

		[Test]
		public void TestTryProcess_WorksWithServerTypeAndServerType() {
			TestTryProcess_WorksWithType(true, true);
		}

		[Test]
		public void TestTryProcess_WorksWithoutModelType() {
			TestTryProcess_WorksWithType(true, true);
		}

		[Test]
		public void TestTryProcess_ErrorIfClientTypeButNotServerType() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ViewDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?view clientModelType=\"ClientType\"?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_ErrorIfModelMemberAlreadyExists() {
			Expect.Call(template.HasMember("Model")).Return(true);
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ViewDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?view modelType=\"ServerType\" ?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_ErrorIfNotRoot() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ViewDirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?view modelType=\"ServerType\" ?>"), false, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}
	}
}
