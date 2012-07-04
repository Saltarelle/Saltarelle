using System;
using Saltarelle.Fragments;
using System.Xml;

namespace Saltarelle.NodeProcessors {
	class EmbeddedCodeNodeProcessor : INodeProcessor {
		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.ProcessingInstruction)
				return false;

			string v = node.Value.Trim();
			switch (node.Name) {
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
