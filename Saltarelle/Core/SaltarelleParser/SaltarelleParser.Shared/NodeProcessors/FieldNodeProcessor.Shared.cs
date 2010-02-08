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
			if (node.NodeType != XmlNodeType.ProcessingInstruction || Utils.NodeName(node) != "field")
				return false;
			if (!isRoot)
				throw ParserUtils.TemplateErrorException(string.Format("The {0} directive can only appear outside of the template.", Utils.NodeName(node)));

			string[] typeArr       = Utils.RegexExec(Utils.NodeValue(node), "type=\"([^\"]*)\"", "");
			string[] serverTypeArr = Utils.RegexExec(Utils.NodeValue(node), "serverType=\"([^\"]*)\"", "");
			string[] clientTypeArr = Utils.RegexExec(Utils.NodeValue(node), "clientType=\"([^\"]*)\"", "");
			string[] nameArr       = Utils.RegexExec(Utils.NodeValue(node), "name=\"([^\"]*)\"", "");

			string serverType, clientType;
			if (typeArr != null) {
				if (string.IsNullOrEmpty(typeArr[1].Trim()))
					throw ParserUtils.TemplateErrorException("No type was specified for the field");
				if (serverTypeArr != null || clientTypeArr != null)
					throw ParserUtils.TemplateErrorException("field elements cannot have both server/client type and type specified.");
				serverType = clientType = typeArr[1].Trim();
			}
			else if (serverTypeArr != null && clientTypeArr != null) {
				if (string.IsNullOrEmpty(serverTypeArr[1].Trim()))
					throw ParserUtils.TemplateErrorException("No server type was specified for the field");
				if (string.IsNullOrEmpty(clientTypeArr[1].Trim()))
					throw ParserUtils.TemplateErrorException("No client type was specified for the field");
				serverType = serverTypeArr[1].Trim();
				clientType = clientTypeArr[1].Trim();
			}
			else
				throw ParserUtils.TemplateErrorException("field elements must have the type specified (either 'type' or 'serverType' and 'clientType').");

			string name = nameArr != null ? nameArr[1].Trim() : null;
			if (string.IsNullOrEmpty(name))
				throw ParserUtils.TemplateErrorException("field elements must have a name specified.");

			if (template.HasMember(name))
				throw ParserUtils.TemplateErrorException("Duplicate member " + name);

			template.AddMember(new FieldMember(name, serverType, clientType));

			return true;
		}
	}
}
