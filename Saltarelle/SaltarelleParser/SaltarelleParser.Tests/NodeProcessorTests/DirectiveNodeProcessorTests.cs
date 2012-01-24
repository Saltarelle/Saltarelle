using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using Saltarelle;
using Saltarelle.NodeProcessors;
using Rhino.Mocks;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class DirectiveNodeProcessorTests : NodeProcessorTestBase {
		[Test]
		public void TestTryProcess_DoesNotParseElement() {
			mocks.ReplayAll();
			Assert.IsFalse(new EmbeddedCodeNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<element></element>"), true, template, renderFunction));
			Assert.IsFalse(fragments.Any());
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_DoesNotParseUnknownInstruction() {
			mocks.ReplayAll();
			Assert.IsFalse(new DirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?unknown instruction?>"), true, template, renderFunction));
			Assert.IsFalse(fragments.Any());
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_Works() {
			Expect.Call(() => template.EnableClientCreate = true);
			mocks.ReplayAll();
			Assert.IsTrue(new DirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?enableClientCreate ?>"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_ErrorIfNotRoot() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new DirectiveNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?enableClientCreate ?>"), false, template, renderFunction), (TemplateErrorException ex) => true);
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}
	}
}
