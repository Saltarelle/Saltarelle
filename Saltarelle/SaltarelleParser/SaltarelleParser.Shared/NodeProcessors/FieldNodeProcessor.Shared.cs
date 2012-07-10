using System;
using Saltarelle.Members;
using System.Xml;

namespace Saltarelle.NodeProcessors {
	internal class FieldNodeProcessor : INodeProcessor {
		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.ProcessingInstruction || node.Name != "field")
				return false;
			if (!isRoot)
				throw ParserUtils.TemplateErrorException(string.Format("The {0} directive can only appear outside of the template.", node.Name));

			string[] typeArr       = Utils.RegexExec(node.Value, "type=\"([^\"]*)\"", "");
			string[] serverTypeArr = Utils.RegexExec(node.Value, "serverType=\"([^\"]*)\"", "");
			string[] clientTypeArr = Utils.RegexExec(node.Value, "clientType=\"([^\"]*)\"", "");
			string[] nameArr       = Utils.RegexExec(node.Value, "name=\"([^\"]*)\"", "");

			string serverType, clientType;
			if (!Utils.IsNull(typeArr)) {
				if (string.IsNullOrEmpty(typeArr[1].Trim()))
					throw ParserUtils.TemplateErrorException("No type was specified for the field");
				if (!Utils.IsNull(serverTypeArr) || !Utils.IsNull(clientTypeArr))
					throw ParserUtils.TemplateErrorException("field elements cannot have both server/client type and type specified.");
				serverType = clientType = typeArr[1].Trim();
			}
			else if (!Utils.IsNull(serverTypeArr) && !Utils.IsNull(clientTypeArr)) {
				if (string.IsNullOrEmpty(serverTypeArr[1].Trim()))
					throw ParserUtils.TemplateErrorException("No server type was specified for the field");
				if (string.IsNullOrEmpty(clientTypeArr[1].Trim()))
					throw ParserUtils.TemplateErrorException("No client type was specified for the field");
				serverType = serverTypeArr[1].Trim();
				clientType = clientTypeArr[1].Trim();
			}
			else
				throw ParserUtils.TemplateErrorException("field elements must have the type specified (either 'type' or 'serverType' and 'clientType').");

			string name = !Utils.IsNull(nameArr) ? nameArr[1].Trim() : null;
			if (string.IsNullOrEmpty(name))
				throw ParserUtils.TemplateErrorException("field elements must have a name specified.");

			if (template.HasMember(name))
				throw ParserUtils.TemplateErrorException("Duplicate member " + name);

			template.AddMember(new FieldMember(name, serverType, clientType));

			return true;
		}
	}
}
