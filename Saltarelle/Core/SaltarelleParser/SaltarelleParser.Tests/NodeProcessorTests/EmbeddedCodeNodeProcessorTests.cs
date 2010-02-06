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
	public class EmbeddedCodeNodeProcessorTests : NodeProcessorTestBase {
		[TestMethod]
		public void TestTryProcess_DoesNotParseElement() {
			mocks.ReplayAll();
			Assert.IsFalse(new EmbeddedCodeNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<element></element>"), true, template, renderFunction));
			Assert.IsFalse(fragments.Any());
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_DoesNotParseUnknownInstruction() {
			mocks.ReplayAll();
			Assert.IsFalse(new EmbeddedCodeNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?unknown instruction?>"), true, template, renderFunction));
			Assert.IsFalse(fragments.Any());
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_ExpressionWorks() {
			mocks.ReplayAll();
			Assert.IsTrue(new EmbeddedCodeNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?x expression ?>"), true, template, renderFunction));
			Assert.AreEqual(1, fragments.Count);
			Assert.AreEqual(new CodeExpressionFragment("expression"), fragments[0]);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_StatementWorks() {
			mocks.ReplayAll();
			Assert.IsTrue(new EmbeddedCodeNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?c statements ?>"), true, template, renderFunction));
			Assert.AreEqual(1, fragments.Count);
			Assert.AreEqual(new CodeFragment("statements", 0), fragments[0]);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_EmptyExpressionThrows() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new EmbeddedCodeNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?x  ?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_EmptyStatementThrows() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new EmbeddedCodeNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<?c  ?>"), true, template, renderFunction), (TemplateErrorException ex) => true);
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}
	}
}
