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
	class ImplementsOrInheritsNodeProcessor : INodeProcessor {
		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.Element || (Utils.NodeName(node) != "inherits" && Utils.NodeName(node) != "implements"))
				return false;
	
			XmlAttribute typeAttr = (XmlAttribute)node.Attributes.GetNamedItem("type"),
			             sideAttr = (XmlAttribute)node.Attributes.GetNamedItem("side");
			if (typeAttr == null || string.IsNullOrEmpty(typeAttr.Value))
				throw ParserUtils.TemplateErrorException(Utils.NodeName(node) + " elements must have the type specified.");
			if (sideAttr == null)
				throw ParserUtils.TemplateErrorException(Utils.NodeName(node) + " elements must have the side specified.");

			bool serverSide, clientSide;
			switch (sideAttr.Value) {
				case "client":
					serverSide = false;
					clientSide = true;
					break;
				case "server":
					serverSide = true;
					clientSide = false;
					break;
				case "both":
					serverSide = true;
					clientSide = true;
					break;
				default:
					throw ParserUtils.TemplateErrorException("The side attribute of the " + Utils.NodeName(node) + " element must be 'client', 'server', or 'both'.");
			}
			if (Utils.GetNumChildNodes(node) > 0)
				throw ParserUtils.TemplateErrorException(Utils.NodeName(node) + " elements must be empty.");

			if (Utils.NodeName(node) == "implements") {
				if (serverSide) {
					if (template.ImplementsServerInterface(typeAttr.Value))
						throw ParserUtils.TemplateErrorException("The interface " + typeAttr.Value + " is implemented more than once on the server side.");
					template.AddServerInterface(typeAttr.Value);
				}
				if (clientSide) {
					if (template.ImplementsClientInterface(typeAttr.Value))
						throw ParserUtils.TemplateErrorException("The interface " + typeAttr.Value + " is implemented more than once on the client side.");
					template.AddClientInterface(typeAttr.Value);
				}
			}
			else {
				if (serverSide) {
					if (template.ServerInherits != null)
						throw ParserUtils.TemplateErrorException("Cannot inherit from more than one class on the server side.");
					template.ServerInherits = typeAttr.Value;
				}
				if (clientSide) {
					if (template.ClientInherits != null)
						throw ParserUtils.TemplateErrorException("Cannot inherit from more than one class on the client side.");
					template.ClientInherits = typeAttr.Value;
				}
			}

			return true;
		}
	}
}
