using System;
using System.Xml;

namespace Saltarelle {
	public interface INodeProcessor {
		bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction);
	}
}
