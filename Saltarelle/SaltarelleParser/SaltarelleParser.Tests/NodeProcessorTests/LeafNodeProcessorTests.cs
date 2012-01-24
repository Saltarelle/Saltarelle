using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Saltarelle;
using Saltarelle.NodeProcessors;
using Saltarelle.Fragments;

namespace SaltarelleParser.Tests {
	[TestClass]
	public class LeafNodeProcessorTests : NodeProcessorTestBase {
		public LeafNodeProcessorTests() {
		}

		[TestMethod]
		public void TestTryProcess_CanParseText() {
			mocks.ReplayAll();
			Assert.IsTrue(new LeafNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("some_text"), true, template, renderFunction));
			Assert.AreEqual(new LiteralFragment("some_text", false), fragments.Single());
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_DoesNormalizeSpaces() {
			mocks.ReplayAll();
			Assert.IsTrue(new LeafNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("  some  text     again  "), true, template, renderFunction));
			Assert.AreEqual(new LiteralFragment(" some text again ", false), fragments.Single());
			mocks.VerifyAll();

			SetupRepo();
			mocks.ReplayAll();
			Assert.IsTrue(new LeafNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("some  text     again"), true, template, renderFunction));
			Assert.AreEqual(new LiteralFragment("some text again", false), fragments.Single());
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_NewlinesAreNormalizedToSpaces() {
			mocks.ReplayAll();
			Assert.IsTrue(new LeafNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("\n\nsome\n\ntext\n\n\n\n\nagain\n\n"), true, template, renderFunction));
			Assert.AreEqual(new LiteralFragment(" some text again ", false), fragments.Single());
			mocks.VerifyAll();
		}
		
		[TestMethod]
		public void TestTryProcess_CanParseCDataAndDoesNotNormalizeSpaces() {
			mocks.ReplayAll();
			Assert.IsTrue(new LeafNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<![CDATA[  some  text     again  ]]>"), true, template, renderFunction));
			Assert.AreEqual(new LiteralFragment("  some  text     again  ", true), fragments.Single());
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_ReturnsEmptyForComment() {
			mocks.ReplayAll();
			Assert.IsTrue(new LeafNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<!-- test comment -->"), true, template, renderFunction));
			Assert.AreEqual(0, fragments.Count);
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_ReturnsNullForElementNode() {
			mocks.ReplayAll();
			Assert.IsFalse(new LeafNodeProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<x>y</x>"), true, template, renderFunction));
			mocks.VerifyAll();
		}
	}
}
