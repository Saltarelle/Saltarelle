using System;
using System.Xml;

namespace Saltarelle.NodeProcessors {
	class DirectiveNodeProcessor : INodeProcessor {
		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.ProcessingInstruction)
				return false;

			switch (node.Name) {
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
