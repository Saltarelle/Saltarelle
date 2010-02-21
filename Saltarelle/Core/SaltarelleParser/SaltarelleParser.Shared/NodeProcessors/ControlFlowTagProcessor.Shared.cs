using System;
using Saltarelle.Fragments;
#if CLIENT
using XmlNode      = System.XML.XMLNode;
using XmlNodeType  = System.XML.XMLNodeType;
using XmlAttribute = System.XML.XMLAttribute;
#else
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
#endif


namespace Saltarelle.NodeProcessors {
	class ControlFlowTagProcessor : INodeProcessor {
		internal static string GetStatement(XmlNode node) {
			switch (Utils.NodeName(node)) {
				case "for": {
					XmlAttribute eachAttr = (XmlAttribute)node.Attributes.GetNamedItem("each"), stmtAttr = (XmlAttribute)node.Attributes.GetNamedItem("stmt");
					if ((eachAttr != null) == (stmtAttr != null))
						throw ParserUtils.TemplateErrorException("In the <for> element, exactly one of the each and stmt attributes must be defined.");
					return eachAttr != null ? ("foreach (" + Utils.NodeValue(eachAttr) + ")") : ("for (" + Utils.NodeValue(stmtAttr) + ")");
				}
				case "if": {
					XmlAttribute testAttr = (XmlAttribute)node.Attributes.GetNamedItem("test");
					if (testAttr == null)
						throw ParserUtils.TemplateErrorException("The <if> element must have the test attribute specified.");
					return "if (" + Utils.NodeValue(testAttr) + ")";
				}
				case "while": {
					XmlAttribute testAttr = (XmlAttribute)node.Attributes.GetNamedItem("test");
					if (testAttr == null)
						throw ParserUtils.TemplateErrorException("The <while> element must have the test attribute specified.");
					return "while (" + Utils.NodeValue(testAttr) + ")";
				}
				case "switch": {
					XmlAttribute exprAttr = (XmlAttribute)node.Attributes.GetNamedItem("expr");
					if (exprAttr == null)
						throw ParserUtils.TemplateErrorException("The <switch> element must have the expr attribute specified.");
					return "switch (" + Utils.NodeValue(exprAttr) + ")";
				}
				default:
					return null;
			}
		}
		
		internal static void ProcessSwitchContent(IDocumentProcessor docProcessor, XmlNode switchNode, ITemplate template, IRenderFunction currentRenderFunction) {
			bool hasDefault = false;
			bool hasAny = false;
			Utils.DoForEachChild(switchNode, delegate(XmlNode child) {
				#if CLIENT
					if (((child.NodeType == XmlNodeType.Text || child.NodeType == XmlNodeType.CharacterData) && child.NodeValue.Trim() == "") || child.NodeType == XmlNodeType.Comment)
						return;
				#else
					if (child.NodeType == XmlNodeType.Whitespace || child.NodeType == XmlNodeType.SignificantWhitespace || child.NodeType == XmlNodeType.Comment)
						return;
				#endif

				if (child.NodeType == XmlNodeType.Element && Utils.NodeName(child) == "case") {
					XmlAttribute valueAttr = (XmlAttribute)child.Attributes.GetNamedItem("value");
					if (valueAttr == null)
						throw ParserUtils.TemplateErrorException("The <case> element must have the value attribute specified.");
					currentRenderFunction.AddFragment(new CodeFragment("case " + Utils.NodeValue(valueAttr) + ": {", 1));
				}
				else if (child.NodeType == XmlNodeType.Element && Utils.NodeName(child) == "default") {
					if (hasDefault)
						throw ParserUtils.TemplateErrorException("There can only be one <default> element inside <switch>");
					hasDefault = true;
					currentRenderFunction.AddFragment(new CodeFragment("default: {", 1));
				}
				else
					throw ParserUtils.TemplateErrorException("The <switch> element can only have <case> and <default> elements as children.");

				hasAny = true;
				Utils.DoForEachChild(child, delegate(XmlNode grandchild) {
					docProcessor.ProcessRecursive(grandchild, template, currentRenderFunction);
				});

				currentRenderFunction.AddFragment(new CodeFragment("break;", -1));
				currentRenderFunction.AddFragment(new CodeFragment("}", 0));
			});
			if (!hasAny)
				throw ParserUtils.TemplateErrorException("The <switch> element must contain at least one <case> or <default>");
		}

		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.Element)
				return false;

			if (Utils.NodeName(node) == "case" || Utils.NodeName(node) == "default")
				throw ParserUtils.TemplateErrorException("<case> and <default> can only occur inside <switch>");

			string statement = GetStatement(node);
			if (statement == null)
				return false;

			if (isRoot)
				throw ParserUtils.TemplateErrorException("Control flow nodes cannot be root elements.");

			currentRenderFunction.AddFragment(new CodeFragment(statement + " {", 1));

			if (Utils.NodeName(node) == "switch") {
				ProcessSwitchContent(docProcessor, node, template, currentRenderFunction);
			}
			else {
				Utils.DoForEachChild(node, delegate(XmlNode child) {
					docProcessor.ProcessRecursive(child, template, currentRenderFunction);
				});
			}
			
			currentRenderFunction.AddFragment(new CodeFragment(null, -1));
			currentRenderFunction.AddFragment(new CodeFragment("}", 0));
			
			return true;
		}
	}
}
