using System;
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
				default:
					return null;
			}
		}
	
		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.Element)
				return false;

			string statement = GetStatement(node);
			if (statement == null)
				return false;
			
			currentRenderFunction.AddFragment(new CodeFragment(statement + " {", 1));

			Utils.DoForEachChild(node, delegate(XmlNode child) {
				docProcessor.ProcessRecursive(child, template, currentRenderFunction);
			});
			
			currentRenderFunction.AddFragment(new CodeFragment(null, -1));
			currentRenderFunction.AddFragment(new CodeFragment("}", 0));
			
			return true;
		}
	}
}
