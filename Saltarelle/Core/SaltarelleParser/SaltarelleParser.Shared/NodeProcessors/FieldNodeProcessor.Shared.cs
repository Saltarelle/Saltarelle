using System;
using Saltarelle.Members;
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
	internal class FieldNodeProcessor : INodeProcessor {
		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.Element || Utils.NodeName(node) != "field")
				return false;
	
			XmlAttribute typeAttr       = (XmlAttribute)node.Attributes.GetNamedItem("type"),
			             serverTypeAttr = (XmlAttribute)node.Attributes.GetNamedItem("serverType"),
			             clientTypeAttr = (XmlAttribute)node.Attributes.GetNamedItem("clientType"),
			             nameAttr       = (XmlAttribute)node.Attributes.GetNamedItem("name");

			string serverType, clientType;
			if (typeAttr != null) {
				if (string.IsNullOrEmpty(typeAttr.Value.Trim()))
					throw ParserUtils.TemplateErrorException("No type was specified for the field");
				if (serverTypeAttr != null || clientTypeAttr != null)
					throw ParserUtils.TemplateErrorException("field elements cannot have both server/client type and type specified.");
				serverType = clientType = typeAttr.Value;
			}
			else if (serverTypeAttr != null && clientTypeAttr != null) {
				if (string.IsNullOrEmpty(clientTypeAttr.Value.Trim()))
					throw ParserUtils.TemplateErrorException("No server type was specified for the field");
				if (string.IsNullOrEmpty(serverTypeAttr.Value.Trim()))
					throw ParserUtils.TemplateErrorException("No client type was specified for the field");
				serverType = serverTypeAttr.Value;
				clientType = clientTypeAttr.Value;
			}
			else
				throw ParserUtils.TemplateErrorException("field elements must have the type specified (either 'type' or 'serverType' and 'clientType').");

			if (nameAttr == null || string.IsNullOrEmpty(nameAttr.Value.Trim()))
				throw ParserUtils.TemplateErrorException("field elements must have a name specified.");

			if (Utils.GetNumChildNodes(node) > 0)
				throw ParserUtils.TemplateErrorException("field elements must be empty.");

			if (template.HasMember(nameAttr.Value))
				throw ParserUtils.TemplateErrorException("Duplicate member " + nameAttr.Value);

			template.AddMember(new FieldMember(nameAttr.Value, serverType, clientType));

			return true;
		}
	}
}
