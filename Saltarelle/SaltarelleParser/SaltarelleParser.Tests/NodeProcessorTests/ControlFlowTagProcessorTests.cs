using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using Saltarelle;
using Saltarelle.NodeProcessors;
using Rhino.Mocks;
using Saltarelle.Fragments;

namespace SaltarelleParser.Tests {
	[TestFixture]
	public class ControlFlowTagProcessorTests : NodeProcessorTestBase {
		[Test]
		public void TestTryProcess_DoesNotProcessUnknownElement() {
			mocks.ReplayAll();
			Assert.IsFalse(new ControlFlowTagProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<unknown/>"), false, template, renderFunction));
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_DoesNotProcessText() {
			mocks.ReplayAll();
			Assert.IsFalse(new ControlFlowTagProcessor().TryProcess(docProcessor, Globals.GetXmlNode("x"), false, template, renderFunction));
			mocks.VerifyAll();
		}
		
		[Test]
		public void TestGetStatement_ForEachWorks() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<for each=\"int x in ints\"/>");
			Assert.AreEqual("foreach (int x in ints)", ControlFlowTagProcessor.GetStatement(n));
			mocks.VerifyAll();
		}

		[Test]
		public void TestGetStatement_ForStmtWorks() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<for stmt=\"int i = 0; i &lt; l.Length; i++\"/>");
			Assert.AreEqual("for (int i = 0; i < l.Length; i++)", ControlFlowTagProcessor.GetStatement(n));
			mocks.VerifyAll();
		}

		[Test]
		public void TestGetStatement_ForThrowsIfBothStmtAndEach() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<for each=\"int x in ints\" stmt=\"int i = 0; i &lt; l.Length; i++\"/>");
			Globals.AssertThrows(() => ControlFlowTagProcessor.GetStatement(n), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[Test]
		public void TestGetStatement_ForThrowsIfNeitherStmtNorEach() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<for/>");
			Globals.AssertThrows(() => ControlFlowTagProcessor.GetStatement(n), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[Test]
		public void TestGetStatement_IfWorks() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<if test=\"x &gt; 0\"/>");
			Assert.AreEqual("if (x > 0)", ControlFlowTagProcessor.GetStatement(n));
			mocks.VerifyAll();
		}

		[Test]
		public void TestGetStatement_IfThrowsIfNoExpression() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<if/>");
			Globals.AssertThrows(() => ControlFlowTagProcessor.GetStatement(n), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[Test]
		public void TestGetStatement_WhileWorks() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<while test=\"x &gt; 0\"/>");
			Assert.AreEqual("while (x > 0)", ControlFlowTagProcessor.GetStatement(n));
			mocks.VerifyAll();
		}

		[Test]
		public void TestGetStatement_WhileThrowsIfNoExpression() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<while/>");
			Globals.AssertThrows(() => ControlFlowTagProcessor.GetStatement(n), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[Test]
		public void TestGetStatement_SwitchWorks() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<switch expr=\"x\"/>");
			Assert.AreEqual("switch (x)", ControlFlowTagProcessor.GetStatement(n));
			mocks.VerifyAll();
		}

		[Test]
		public void TestGetStatement_SwitchThrowsIfNoExpression() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<switch/>");
			Globals.AssertThrows(() => ControlFlowTagProcessor.GetStatement(n), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}
		
		[Test]
		public void TestProcessSwitchContents_ErrorIfNoContent() {
			mocks.ReplayAll();
			XmlNode node = Globals.GetXmlNode("<switch expr=\"testexpr\">  <!-- comment --> </switch>");
			Globals.AssertThrows(() => ControlFlowTagProcessor.ProcessSwitchContent(docProcessor, node, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[Test]
		public void TestProcessSwitchContents_ErrorIfBadElementContent() {
			mocks.ReplayAll();
			XmlNode node = Globals.GetXmlNode("<switch expr=\"testexpr\"><bad/><default>x</default></switch>");
			Globals.AssertThrows(() => ControlFlowTagProcessor.ProcessSwitchContent(docProcessor, node, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[Test]
		public void TestProcessSwitchContents_ErrorIfProcessingInstructionContent() {
			mocks.ReplayAll();
			XmlNode node = Globals.GetXmlNode("<switch expr=\"testexpr\"><?bad?><default>x</default></switch>");
			Globals.AssertThrows(() => ControlFlowTagProcessor.ProcessSwitchContent(docProcessor, node, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[Test]
		public void TestProcessSwitchContents_ErrorIfTwoDefaults() {
			Expect.Call(() => docProcessor.ProcessRecursive(null, null, null)).IgnoreArguments().Repeat.Any();
			mocks.ReplayAll();
			XmlNode node = Globals.GetXmlNode("<switch expr=\"testexpr\"><default>x</default><default>y</default></switch>");
			Globals.AssertThrows(() => ControlFlowTagProcessor.ProcessSwitchContent(docProcessor, node, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}
		
		[Test]
		public void TestTryProcess_SwitchWorks() {
			XmlNode node = Globals.GetXmlNode("<switch expr=\"testexpr\"> <case value=\"1\"><x/></case>  <default><y/></default> <!-- comment --> <case value=\"2\"><z1/><z2/></case></switch>");
			XmlNode x = node.SelectSingleNode("//x"), y = node.SelectSingleNode("//y"), z1 = node.SelectSingleNode("//z1"), z2 = node.SelectSingleNode("//z2");

			Expect.Call(() => docProcessor.ProcessRecursive(x,  template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("[a]"))));
			Expect.Call(() => docProcessor.ProcessRecursive(y,  template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("[b]"))));
			Expect.Call(() => docProcessor.ProcessRecursive(z1, template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("[c1]"))));
			Expect.Call(() => docProcessor.ProcessRecursive(z2, template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("[c2]"))));

			mocks.ReplayAll();
			Assert.IsTrue(new ControlFlowTagProcessor().TryProcess(docProcessor, node, false, template, renderFunction));
			
			Assert.IsTrue(new IFragment[] { new CodeFragment("switch (testexpr) {", 1),
			                                new CodeFragment("case 1: {", 1),
			                                new LiteralFragment("[a]"),
			                                new CodeFragment("break;", -1),
			                                new CodeFragment("}", 0),
			                                new CodeFragment("default: {", 1),
			                                new LiteralFragment("[b]"),
			                                new CodeFragment("break;", -1),
			                                new CodeFragment("}", 0),
			                                new CodeFragment("case 2: {", 1),
			                                new LiteralFragment("[c1]"),
			                                new LiteralFragment("[c2]"),
			                                new CodeFragment("break;", -1),
			                                new CodeFragment("}", 0),
			                                new CodeFragment(null, -1),
			                                new CodeFragment("}", 0) }.SequenceEqual(fragments));

			mocks.VerifyAll();			
		}

		[Test]
		public void TestTryProcess_IfStatementErrorIfElseContent() {
			Expect.Call(() => docProcessor.ProcessRecursive(null, null, null)).IgnoreArguments().Repeat.Any();
			mocks.ReplayAll();
			XmlNode node = Globals.GetXmlNode("<if test=\"testexpr\">x<else>x</else>x</if>");
			Globals.AssertThrows(() => new ControlFlowTagProcessor().TryProcess(docProcessor, node, false, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}


		[Test]
		public void TestTryProcess_IfStatementErrorIfElseIfDoesNotHaveTest() {
			Expect.Call(() => docProcessor.ProcessRecursive(null, null, null)).IgnoreArguments().Repeat.Any();
			mocks.ReplayAll();
			XmlNode node = Globals.GetXmlNode("<if test=\"testexpr\">x<else-if/>x</if>");
			Globals.AssertThrows(() => new ControlFlowTagProcessor().TryProcess(docProcessor, node, false, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_IfStatementErrorIfElseIfAfterElse() {
			Expect.Call(() => docProcessor.ProcessRecursive(null, null, null)).IgnoreArguments().Repeat.Any();
			mocks.ReplayAll();
			XmlNode node = Globals.GetXmlNode("<if test=\"testexpr\">x<else/>x<else-if test=\"test2\"/>x</if>");
			Globals.AssertThrows(() => new ControlFlowTagProcessor().TryProcess(docProcessor, node, false, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}

		[Test]
		public void TestTryProcess_IfWorks() {
			XmlNode node = Globals.GetXmlNode("<if test=\"test1\"><x1/><x2/><else-if test=\"test2\"/><y1/><y2/><else/><z1/><z2/></if>");
			XmlNode x1 = node.SelectSingleNode("//x1"), x2 = node.SelectSingleNode("//x2"), y1 = node.SelectSingleNode("//y1"), y2 = node.SelectSingleNode("//y2"), z1 = node.SelectSingleNode("//z1"), z2 = node.SelectSingleNode("//z2");

			Expect.Call(() => docProcessor.ProcessRecursive(x1, template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("[a1]"))));
			Expect.Call(() => docProcessor.ProcessRecursive(x2, template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("[a2]"))));
			Expect.Call(() => docProcessor.ProcessRecursive(y1, template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("[b1]"))));
			Expect.Call(() => docProcessor.ProcessRecursive(y2, template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("[b2]"))));
			Expect.Call(() => docProcessor.ProcessRecursive(z1, template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("[c1]"))));
			Expect.Call(() => docProcessor.ProcessRecursive(z2, template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("[c2]"))));

			mocks.ReplayAll();
			Assert.IsTrue(new ControlFlowTagProcessor().TryProcess(docProcessor, node, false, template, renderFunction));
			
			Assert.IsTrue(new IFragment[] { new CodeFragment("if (test1) {", 1),
			                                new LiteralFragment("[a1]"),
			                                new LiteralFragment("[a2]"),
			                                new CodeFragment(null, -1),
			                                new CodeFragment("}", 0),
			                                new CodeFragment("else if (test2) {", 1),
			                                new LiteralFragment("[b1]"),
			                                new LiteralFragment("[b2]"),
			                                new CodeFragment(null, -1),
			                                new CodeFragment("}", 0),
			                                new CodeFragment("else {", 1),
			                                new LiteralFragment("[c1]"),
			                                new LiteralFragment("[c2]"),
			                                new CodeFragment(null, -1),
			                                new CodeFragment("}", 0) }.SequenceEqual(fragments));

			mocks.VerifyAll();			
		}

		[Test]
		public void TestTryProcess_SimpleWorks() {
			XmlNode n = Globals.GetXmlNode("<while test=\"x\"><a/><b/></while>"), n1 = n.ChildNodes[0], n2 = n.ChildNodes[1];
			Expect.Call(() => docProcessor.ProcessRecursive(n1, template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("a"))));
			Expect.Call(() => docProcessor.ProcessRecursive(n2, template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("b"))));
			mocks.ReplayAll();
			Assert.IsTrue(new ControlFlowTagProcessor().TryProcess(docProcessor, n, false, template, renderFunction));
			Assert.IsTrue(new IFragment[] { new CodeFragment("while (x) {", 1), new LiteralFragment("a"), new LiteralFragment("b"), new CodeFragment(null, -1), new CodeFragment("}", 0) }.SequenceEqual(fragments));
		}

		[Test]
		public void TestTryProcess_ErrorIfRoot() {
			mocks.ReplayAll();
			Globals.AssertThrows(() => new ControlFlowTagProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<if test=\"x\"/>"), true, template, renderFunction), (TemplateErrorException ex) => true);
			mocks.VerifyAll();
		}
	}
}
