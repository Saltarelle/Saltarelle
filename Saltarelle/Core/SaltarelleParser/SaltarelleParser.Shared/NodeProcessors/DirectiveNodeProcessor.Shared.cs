using System;
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
	class DirectiveNodeProcessor : INodeProcessor {
		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.ProcessingInstruction)
				return false;

			switch (Utils.NodeName(node)) {
				case "enableClientCreate":
					if (!isRoot)
						throw ParserUtils.TemplateErrorException("The enableClientCreate directive can only appear outside of the template.");
					template.EnableClientCreate = true;
					return true;
				default:
					return false;
			}
		}
	}
}
