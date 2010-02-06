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
	public class ControlFlowTagProcessorTests : NodeProcessorTestBase {
		[TestMethod]
		public void TestTryProcess_DoesNotProcessUnknownElement() {
			mocks.ReplayAll();
			Assert.IsFalse(new ControlFlowTagProcessor().TryProcess(docProcessor, Globals.GetXmlNode("<unknown/>"), false, template, renderFunction));
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestTryProcess_DoesNotProcessText() {
			mocks.ReplayAll();
			Assert.IsFalse(new ControlFlowTagProcessor().TryProcess(docProcessor, Globals.GetXmlNode("x"), false, template, renderFunction));
			mocks.VerifyAll();
		}
		
		[TestMethod]
		public void TestGetStatement_ForEachWorks() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<for each=\"int x in ints\"/>");
			Assert.AreEqual("foreach (int x in ints)", ControlFlowTagProcessor.GetStatement(n));
		}

		[TestMethod]
		public void TestGetStatement_ForStmtWorks() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<for stmt=\"int i = 0; i &lt; l.Length; i++\"/>");
			Assert.AreEqual("for (int i = 0; i < l.Length; i++)", ControlFlowTagProcessor.GetStatement(n));
		}

		[TestMethod]
		public void TestGetStatement_ForThrowsIfBothStmtAndEach() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<for each=\"int x in ints\" stmt=\"int i = 0; i &lt; l.Length; i++\"/>");
			Globals.AssertThrows(() => ControlFlowTagProcessor.GetStatement(n), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestGetStatement_ForThrowsIfNeitherStmtNorEach() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<for/>");
			Globals.AssertThrows(() => ControlFlowTagProcessor.GetStatement(n), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestGetStatement_IfWorks() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<if test=\"x &gt; 0\"/>");
			Assert.AreEqual("if (x > 0)", ControlFlowTagProcessor.GetStatement(n));
		}

		[TestMethod]
		public void TestGetStatement_IfThrowsIfNoExpression() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<if/>");
			Globals.AssertThrows(() => ControlFlowTagProcessor.GetStatement(n), (TemplateErrorException ex) => true);
		}

		[TestMethod]
		public void TestGetStatement_WhileWorks() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<while test=\"x &gt; 0\"/>");
			Assert.AreEqual("while (x > 0)", ControlFlowTagProcessor.GetStatement(n));
		}

		[TestMethod]
		public void TestGetStatement_WhileThrowsIfNoExpression() {
			mocks.ReplayAll();
			XmlNode n = Globals.GetXmlNode("<while/>");
			Globals.AssertThrows(() => ControlFlowTagProcessor.GetStatement(n), (TemplateErrorException ex) => true);
		}
		
		[TestMethod]
		public void TestTryProcess_Works() {
			XmlNode n = Globals.GetXmlNode("<if test=\"x\"><a/><b/></if>"), n1 = n.ChildNodes[0], n2 = n.ChildNodes[1];
			Expect.Call(() => docProcessor.ProcessRecursive(n1, template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("a"))));
			Expect.Call(() => docProcessor.ProcessRecursive(n2, template, renderFunction)).Do((Action<XmlNode, ITemplate, IRenderFunction>)((_, __, f) => f.AddFragment(new LiteralFragment("b"))));
			mocks.ReplayAll();
			Assert.IsTrue(new ControlFlowTagProcessor().TryProcess(docProcessor, n, false, template, renderFunction));
			Assert.IsTrue(new IFragment[] { new CodeFragment("if (x) {", 1), new LiteralFragment("a"), new LiteralFragment("b"), new CodeFragment(null, -1), new CodeFragment("}", 0) }.SequenceEqual(fragments));
		}
	}
}
