using System;
#if CLIENT
using XmlNode = System.XML.XMLNode;
#else
using System.Xml;
#endif

namespace Saltarelle {
	public interface INodeProcessor {
		bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction);
	}
}
