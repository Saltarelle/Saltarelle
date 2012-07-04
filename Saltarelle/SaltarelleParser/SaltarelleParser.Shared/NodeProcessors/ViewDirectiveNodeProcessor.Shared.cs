using System;
using Saltarelle;
using Saltarelle.Members;
using System.Xml;

namespace Saltarelle.NodeProcessors {
	class ViewDirectiveNodeProcessor : INodeProcessor {
		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.ProcessingInstruction || node.Name != "view")
				return false;
			if (!isRoot)
				throw ParserUtils.TemplateErrorException(string.Format("The view directive can only appear outside of the template.", node.Name));

			string[] serverTypeArr = Utils.RegexExec(node.Value, "modelType=\"([^\"]*)\"", "");
			string[] clientTypeArr = Utils.RegexExec(node.Value, "clientModelType=\"([^\"]*)\"", "");
			if (Utils.IsNull(serverTypeArr) && !Utils.IsNull(clientTypeArr))
				throw ParserUtils.TemplateErrorException("You cannot specify a client type for the model if you don't specify a server type");

			if (template.HasMember("Model") || template.HasMember("model") || template.HasMember("Saltarelle.Mvc.IView.Model"))
				throw ParserUtils.TemplateErrorException("The template already defines at least one of the members essential to use the view directive. Have you specified <?view?> more than once?");

			string serverType = (!Utils.IsNull(serverTypeArr) ? serverTypeArr[1] : "object"), clientType = (!Utils.IsNull(clientTypeArr) ? clientTypeArr[1] : null);
			string viewInterface = "Saltarelle.Mvc.IView<" + serverType + ">";

			if (template.ImplementsServerInterface(viewInterface))
				throw ParserUtils.TemplateErrorException("The template already implements the interface " + viewInterface + ".");

			template.AddServerInterface(viewInterface);
			template.AddMember(new FieldMember("model", serverType, clientType));
			template.AddMember(new PropertyMember("Model", serverType, null, AccessModifier._Public, "model", serverType, null, true, true, "ModelChanged", false));
			template.AddMember(new PropertyMember("Saltarelle.Mvc.IView.Model", "object", null, AccessModifier._None, "model", serverType, null, true, true, "ModelChanged", false));

			return true;
		}
	}
}