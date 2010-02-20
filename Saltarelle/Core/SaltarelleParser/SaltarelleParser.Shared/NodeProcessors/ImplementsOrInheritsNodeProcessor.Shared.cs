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
			if (node.NodeType != XmlNodeType.ProcessingInstruction || (Utils.NodeName(node) != "inherits" && Utils.NodeName(node) != "implements"))
				return false;

			if (!isRoot)
				throw ParserUtils.TemplateErrorException(string.Format("The {0} directive can only appear outside of the template.", Utils.NodeName(node)));
				
			string[] sideArr = Utils.RegexExec(Utils.NodeValue(node), "side=\"([^\"]*)\"", "");
			string[] typeArr = Utils.RegexExec(Utils.NodeValue(node), "type=\"([^\"]*)\"", "");
	
			if (typeArr == null)
				throw ParserUtils.TemplateErrorException(Utils.NodeName(node) + " elements must have the type specified.");
			if (sideArr == null)
				throw ParserUtils.TemplateErrorException(Utils.NodeName(node) + " elements must have the side specified.");

			string side = sideArr[1].Trim(), type = typeArr[1].Trim();

			bool serverSide, clientSide;
			switch (side) {
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

			if (Utils.NodeName(node) == "implements") {
				if (serverSide) {
					if (template.ImplementsServerInterface(type))
						throw ParserUtils.TemplateErrorException("The interface " + type + " is implemented more than once on the server side.");
					template.AddServerInterface(type);
				}
				if (clientSide) {
					if (template.ImplementsClientInterface(type))
						throw ParserUtils.TemplateErrorException("The interface " + type + " is implemented more than once on the client side.");
					template.AddClientInterface(type);
				}
			}
			else {
				if (serverSide) {
					if (template.ServerInherits != null)
						throw ParserUtils.TemplateErrorException("Cannot inherit from more than one class on the server side.");
					template.ServerInherits = type;
				}
				if (clientSide) {
					if (template.ClientInherits != null)
						throw ParserUtils.TemplateErrorException("Cannot inherit from more than one class on the client side.");
					template.ClientInherits = type;
				}
			}

			return true;
		}
	}
}
