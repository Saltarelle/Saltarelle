using System;
using System.Xml;
using Saltarelle.Fragments;

namespace Saltarelle.NodeProcessors {
	class ControlFlowTagProcessor : INodeProcessor {
		internal static string GetStatement(XmlNode node) {
			switch (node.Name) {
				case "for": {
					XmlAttribute eachAttr = (XmlAttribute)node.Attributes.GetNamedItem("each"), stmtAttr = (XmlAttribute)node.Attributes.GetNamedItem("stmt");
					if ((eachAttr != null) == (stmtAttr != null))
						throw ParserUtils.TemplateErrorException("In the <for> element, exactly one of the each and stmt attributes must be defined.");
					return eachAttr != null ? ("foreach (" + eachAttr.Value + ")") : ("for (" + stmtAttr.Value + ")");
				}
				case "if": {
					XmlAttribute testAttr = (XmlAttribute)node.Attributes.GetNamedItem("test");
					if (testAttr == null)
						throw ParserUtils.TemplateErrorException("The <if> element must have the test attribute specified.");
					return "if (" + testAttr.Value + ")";
				}
				case "while": {
					XmlAttribute testAttr = (XmlAttribute)node.Attributes.GetNamedItem("test");
					if (testAttr == null)
						throw ParserUtils.TemplateErrorException("The <while> element must have the test attribute specified.");
					return "while (" + testAttr.Value + ")";
				}
				case "switch": {
					XmlAttribute exprAttr = (XmlAttribute)node.Attributes.GetNamedItem("expr");
					if (exprAttr == null)
						throw ParserUtils.TemplateErrorException("The <switch> element must have the expr attribute specified.");
					return "switch (" + exprAttr.Value + ")";
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
					if (((child.NodeType == XmlNodeType.Text || child.NodeType == XmlNodeType.CDATA) && child.Value.Trim() == "") || child.NodeType == XmlNodeType.Comment)
						return;
				#else
					if (child.NodeType == XmlNodeType.Whitespace || child.NodeType == XmlNodeType.SignificantWhitespace || child.NodeType == XmlNodeType.Comment)
						return;
				#endif

				if (child.NodeType == XmlNodeType.Element && child.Name == "case") {
					XmlAttribute valueAttr = (XmlAttribute)child.Attributes.GetNamedItem("value");
					if (valueAttr == null)
						throw ParserUtils.TemplateErrorException("The <case> element must have the value attribute specified.");
					currentRenderFunction.AddFragment(new CodeFragment("case " + valueAttr.Value + ": {", 1));
				}
				else if (child.NodeType == XmlNodeType.Element && child.Name == "default") {
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

			if (node.Name == "case" || node.Name == "default")
				throw ParserUtils.TemplateErrorException("<case> and <default> can only occur inside <switch>");
			if (node.Name == "else-if" || node.Name == "else")
				throw ParserUtils.TemplateErrorException("<else-if> and <else> can only occur inside <if>");

			string statement = GetStatement(node);
			if (statement == null)
				return false;

			if (isRoot)
				throw ParserUtils.TemplateErrorException("Control flow nodes cannot be root elements.");

			currentRenderFunction.AddFragment(new CodeFragment(statement + " {", 1));

			if (node.Name == "switch") {
				ProcessSwitchContent(docProcessor, node, template, currentRenderFunction);
			}
			else {
				bool hasElse = false;
				Utils.DoForEachChild(node, delegate(XmlNode child) {
					if (node.Name == "if" && (child.NodeType == XmlNodeType.Element && (child.Name == "else-if" || child.Name == "else"))) {
						if (hasElse)
							throw ParserUtils.TemplateErrorException("There cannot be other <else-if> or <else> elements after <else>.");
						if (Utils.GetNumChildNodes(child) > 0)
							throw ParserUtils.TemplateErrorException("<" + child.Name + "> elements should not have children.");
						string possibleTest;
						if (child.Name == "else-if") {
							XmlAttribute testAttr = (XmlAttribute)child.Attributes.GetNamedItem("test");
							if (testAttr == null)
								throw ParserUtils.TemplateErrorException("The <else-if> elements must have the test attribute specified.");
							possibleTest = "if (" + testAttr.Value + ") ";
						}
						else {
							hasElse = true;
							possibleTest = "";
						}
						currentRenderFunction.AddFragment(new CodeFragment(null, -1));
						currentRenderFunction.AddFragment(new CodeFragment("}", 0));
						currentRenderFunction.AddFragment(new CodeFragment("else " + possibleTest + "{", 1));
					}
					else
						docProcessor.ProcessRecursive(child, template, currentRenderFunction);
				});
			}
			
			currentRenderFunction.AddFragment(new CodeFragment(null, -1));
			currentRenderFunction.AddFragment(new CodeFragment("}", 0));
			
			return true;
		}
	}
}
