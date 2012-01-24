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
using Saltarelle.Fragments;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class GenericElementProcessorTests : NodeProcessorTestBase {
		public GenericElementProcessorTests() {
		}

		[Test]
		public void TestAddSingleAttributeFragments_SimpleNameValuePairWorks() {
			Expect.Call(docProcessor.ParseUntypedMarkup("test\"Value\"")).Return(new LiteralFragment("test&quot;Value&quot;"));
			mocks.ReplayAll();
			var context = new GenericElementProcessorContext();
			GenericElementProcessor.AddSingleAttributeFragments(docProcessor, "testName", "test\"Value\"", false, template, renderFunction, context);
			Assert.AreEqual(" testName=\"test&quot;Value&quot;\"", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestAddSingleAttributeFragments_StyleInRootAppendsPosition() {
			Expect.Call(docProcessor.ParseUntypedMarkup("width: 0px; ")).Return(new LiteralFragment("width: 0px; "));
			mocks.ReplayAll();
			var context = new GenericElementProcessorContext();
			GenericElementProcessor.AddSingleAttributeFragments(docProcessor, "style", "width: 0px; ", true, template, renderFunction, context);
			Assert.AreEqual(" style=\"width: 0px; [Position]\"", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestAddSingleAttributeFragments_StyleInRootAppendsSemicolonIfOneIsMissing() {
			Expect.Call(docProcessor.ParseUntypedMarkup("width: 0px")).Return(new LiteralFragment("width: 0px"));
			mocks.ReplayAll();
			var context = new GenericElementProcessorContext();
			GenericElementProcessor.AddSingleAttributeFragments(docProcessor, "style", "width: 0px", true, template, renderFunction, context);
			Assert.AreEqual(" style=\"width: 0px;[Position]\"", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestAddSingleAttributeFragments_StyleInRootAppendsSemicolonIfNonLiteralFragmentIsReturned() {
			Expect.Call(docProcessor.ParseUntypedMarkup("code")).Return(new CodeExpressionFragment("code"));
			mocks.ReplayAll();
			var context = new GenericElementProcessorContext();
			GenericElementProcessor.AddSingleAttributeFragments(docProcessor, "style", "code", true, template, renderFunction, context);
			Assert.AreEqual(" style=\"[EXPR:code];[Position]\"", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestAddSingleAttributeFragments_StyleInNonRootIsNotModified() {
			Expect.Call(docProcessor.ParseUntypedMarkup("width: 0px")).Return(new LiteralFragment("width: 0px"));
			mocks.ReplayAll();
			var context = new GenericElementProcessorContext();
			GenericElementProcessor.AddSingleAttributeFragments(docProcessor, "style", "width: 0px", false, template, renderFunction, context);
			Assert.AreEqual(" style=\"width: 0px\"", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestAddSingleAttributeFragments_IdForNonRootElementWorks() {
			Expect.Call(template.HasMember("testid")).Return(false);
			mocks.ReplayAll();
			var context = new GenericElementProcessorContext();
			GenericElementProcessor.AddSingleAttributeFragments(docProcessor, "id", "testid", false, template, renderFunction, context);
			Assert.AreEqual(" id=\"[ID]_testid\"", ConcatenatedFragments);
			Assert.AreEqual("testid", context.Id);
			mocks.VerifyAll();
		}

		[Test]
		public void TestAddSingleAttributeFragments_IdForRootElementThrowsException() {
			var context = new GenericElementProcessorContext();
			Globals.AssertThrows(() => GenericElementProcessor.AddSingleAttributeFragments(docProcessor, "id", "testid", true, template, renderFunction, context), (TemplateErrorException ex) => true);
		}

		[Test]
		public void TestAddSingleAttributeFragments_ForAndNameAttributesWorks() {
			foreach (var attrName in new[] { "for", "name" }) {
				SetupRepo();
				mocks.ReplayAll();
				var context = new GenericElementProcessorContext();
				GenericElementProcessor.AddSingleAttributeFragments(docProcessor, attrName, "TestValue", false, template, renderFunction, context);
				Assert.AreEqual(" " + attrName + "=\"[ID]_TestValue\"", ConcatenatedFragments);
				mocks.VerifyAll();
			}
		}

		[Test]
		public void TestAddSingleAttributeFragments_TypeAttributeWorks() {
			SetupRepo();
			Expect.Call(docProcessor.ParseUntypedMarkup("TestValue")).Return(new LiteralFragment("TestValue"));
			mocks.ReplayAll();
			var context = new GenericElementProcessorContext();
			GenericElementProcessor.AddSingleAttributeFragments(docProcessor, "type", "TestValue", false, template, renderFunction, context);
			Assert.AreEqual(" type=\"TestValue\"", ConcatenatedFragments);
			Assert.AreEqual("TestValue", context.Type);
			mocks.VerifyAll();
		}

		[Test]
		public void TestAddSingleAttributeFragments_ActualNameWorks() {
			Expect.Call(docProcessor.ParseUntypedMarkup("SomeName")).Return(new LiteralFragment("SomeName"));
			mocks.ReplayAll();
			var context = new GenericElementProcessorContext();
			GenericElementProcessor.AddSingleAttributeFragments(docProcessor, "actualName", "SomeName", false, template, renderFunction, context);
			Assert.AreEqual(" name=\"SomeName\"", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestAddSingleAttributeFragments_ErrorIfDuplicateMember() {
			Expect.Call(template.HasMember("ExistingMember")).Return(true);
			mocks.ReplayAll();
			var context = new GenericElementProcessorContext();
			Globals.AssertThrows(() => GenericElementProcessor.AddSingleAttributeFragments(docProcessor, "id", "ExistingMember", false, template, renderFunction, context), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[Test]
		public void TestAddAttributeFragments_SimpleNonRootWorks() {
			Expect.Call(docProcessor.ParseUntypedMarkup("val1")).Return(new LiteralFragment("val1"));
			Expect.Call(docProcessor.ParseUntypedMarkup("val2")).Return(new LiteralFragment("val2"));
			mocks.ReplayAll();
			var context = new GenericElementProcessorContext();
			GenericElementProcessor.AddAttributeFragments(docProcessor, Globals.GetXmlNode("<x attr1=\"val1\" attr2=\"val2\"/>"), false, template, renderFunction, context);
			Assert.AreEqual(" attr1=\"val1\" attr2=\"val2\"", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestAddAttributeFragments_RootAddsPositionAndStyle() {
			Expect.Call(docProcessor.ParseUntypedMarkup("val1")).Return(new LiteralFragment("val1"));
			Expect.Call(docProcessor.ParseUntypedMarkup("val2")).Return(new LiteralFragment("val2"));
			mocks.ReplayAll();
			var context = new GenericElementProcessorContext();
			GenericElementProcessor.AddAttributeFragments(docProcessor, Globals.GetXmlNode("<x attr1=\"val1\" attr2=\"val2\"/>"), true, template, renderFunction, context);
			Assert.AreEqual(" attr1=\"val1\" attr2=\"val2\" id=\"[ID]\" style=\"[Position]\"", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestAddAttributeFragments_StyleIsNotAddedIfExists() {
			Expect.Call(docProcessor.ParseUntypedMarkup("a: b")).Return(new LiteralFragment("a: b"));
			mocks.ReplayAll();
			var context = new GenericElementProcessorContext();
			GenericElementProcessor.AddAttributeFragments(docProcessor, Globals.GetXmlNode("<x style=\"a: b\"/>"), true, template, renderFunction, context);
			Assert.AreEqual(" style=\"a: b;[Position]\" id=\"[ID]\"", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_DoesNotParseTextNode() {
			mocks.ReplayAll();
			Assert.IsFalse(new GenericElementProcessor().TryProcess(docProcessor, Globals.GetXmlNode("a"), false, template, renderFunction));
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_AttributesWorkForNonRoot() {
			var node = Globals.GetXmlNode("<div id=\"a\"><div id=\"b\">x</div><div id=\"c\">y</div></div>");
		
			Expect.Call(template.HasMember("a")).Return(false);
			Expect.Call(() => template.AddMember(new NamedElementMember("div", "a")));
			Expect.Call(() => docProcessor.ProcessRecursive(node.ChildNodes[0], template, renderFunction)).Do(new Action<XmlNode, ITemplate, IRenderFunction>((n, t, f) => f.AddFragment(new LiteralFragment("[B]"))));
			Expect.Call(() => docProcessor.ProcessRecursive(node.ChildNodes[1], template, renderFunction)).Do(new Action<XmlNode, ITemplate, IRenderFunction>((n, t, f) => f.AddFragment(new LiteralFragment("[C]"))));
			mocks.ReplayAll();
			var actual = new GenericElementProcessor().TryProcess(docProcessor, node, false, template, renderFunction);
			Assert.AreEqual("<div id=\"[ID]_a\">[B][C]</div>", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_InputNoTypeWorks() {
			var node = Globals.GetXmlNode("<input id=\"a\"/>");
		
			Expect.Call(template.HasMember("a")).Return(false);
			Expect.Call(() => template.AddMember(new NamedElementMember("input", "a")));
			mocks.ReplayAll();
			var actual = new GenericElementProcessor().TryProcess(docProcessor, node, false, template, renderFunction);
			Assert.AreEqual("<input id=\"[ID]_a\"/>", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_InputWithTypeWorks() {
			var node = Globals.GetXmlNode("<input id=\"a\" type=\"text\"/>");
		
			Expect.Call(template.HasMember("a")).Return(false);
			Expect.Call(() => template.AddMember(new NamedElementMember("input/text", "a")));
			Expect.Call(docProcessor.ParseUntypedMarkup("text")).Return(new LiteralFragment("text"));
			mocks.ReplayAll();
			var actual = new GenericElementProcessor().TryProcess(docProcessor, node, false, template, renderFunction);
			Assert.AreEqual("<input id=\"[ID]_a\" type=\"text\"/>", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_InputWithNonLiteralTypeWorks() {
			var node = Globals.GetXmlNode("<input id=\"a\" type=\"{= Something}\"/>");
		
			Expect.Call(template.HasMember("a")).Return(false);
			Expect.Call(() => template.AddMember(new NamedElementMember("input", "a")));
			Expect.Call(docProcessor.ParseUntypedMarkup("{= Something}")).Return(new CodeFragment("Something", 0));
			mocks.ReplayAll();
			var actual = new GenericElementProcessor().TryProcess(docProcessor, node, false, template, renderFunction);
			Assert.AreEqual("<input id=\"[ID]_a\" type=\"[CODE:Something, indent=0]\"/>", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_EmptyWithPotentialChildrenWorks() {
			Expect.Call(template.HasMember("a")).Return(false);
			Expect.Call(() => template.AddMember(new NamedElementMember("div", "a")));
			mocks.ReplayAll();
			var actual = new GenericElementProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<div id=\"a\"></div>"), false, template, renderFunction);
			Assert.AreEqual("<div id=\"[ID]_a\"></div>", ConcatenatedFragments);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_SomeElementsSelfClose() {
			foreach (var tag in new[] { "br", "img", "hr", "link", "input", "meta", "col", "frame", "base", "area" }) {	
				SetupRepo();
				Expect.Call(template.HasMember("a")).Return(false);
				Expect.Call(() => template.AddMember(new NamedElementMember(tag, "a")));
				mocks.ReplayAll();
				Assert.IsTrue(new GenericElementProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<" + tag + " id=\"a\"/>"), false, template, renderFunction));
				Assert.AreEqual("<" + tag + " id=\"[ID]_a\"/>", ConcatenatedFragments);
				mocks.VerifyAll();
			}
		}

		[Test]
		public void TestTryProcess_RootWorks() {
			mocks.ReplayAll();
			var actual = new GenericElementProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<div></div>"), true, template, renderFunction);
			Assert.AreEqual("<div id=\"[ID]\" style=\"[Position]\"></div>", ConcatenatedFragments);
			mocks.VerifyAll();
		}

	}
}
