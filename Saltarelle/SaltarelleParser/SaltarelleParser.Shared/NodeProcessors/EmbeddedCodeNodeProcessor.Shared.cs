using System;
using Saltarelle.Fragments;
#if CLIENT
using XmlNode = System.XML.XMLNode;
using XmlAttribute = System.XML.XMLAttribute;
using XmlNodeType = System.XML.XMLNodeType;
#else
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
#endif

namespace Saltarelle.NodeProcessors {
	class EmbeddedCodeNodeProcessor : INodeProcessor {
		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.ProcessingInstruction)
				return false;

			string v = Utils.NodeValue(node).Trim();
			switch (Utils.NodeName(node)) {
				case "x":
					if (v == "")
						throw ParserUtils.TemplateErrorException("Empty embedded expression");
					currentRenderFunction.AddFragment(new CodeExpressionFragment(v)); return true;
				case "c":
					if (v == "")
						throw ParserUtils.TemplateErrorException("Empty embedded code");
					currentRenderFunction.AddFragment(new CodeFragment(v, 0)); return true;
				default:  return false;
			}
		}
	}
}
