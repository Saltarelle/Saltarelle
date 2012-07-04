using System;
using System.Xml;

namespace Saltarelle.NodeProcessors {
	internal class UsingDirectiveNodeProcessor : INodeProcessor {
		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.ProcessingInstruction || node.Name != "using")
				return false;

			if (!isRoot)
				throw ParserUtils.TemplateErrorException(string.Format("The using directive can only appear outside of the template.", node.Name));
				
			string[] sideArr      = Utils.RegexExec(node.Value, "side=\"([^\"]*)\"", "");
			string[] namespaceArr = Utils.RegexExec(node.Value, "namespace=\"([^\"]*)\"", "");
	
			if (Utils.IsNull(namespaceArr))
				throw ParserUtils.TemplateErrorException("Using directive must have the namespace specified.");

			string nmspace = namespaceArr[1].Trim(), side = (sideArr != null ? sideArr[1].Trim() : "both");
			if (!ParserUtils.IsValidQualifiedName(nmspace))
				throw ParserUtils.TemplateErrorException(string.Format("The identifier '{0}' is not a valid namespace name.", nmspace));

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
					throw ParserUtils.TemplateErrorException("The side attribute of the using directive must be 'client', 'server', or 'both'.");
			}
			
			if (serverSide)
				template.AddServerUsingDirective(nmspace);
			if (clientSide)
				template.AddClientUsingDirective(nmspace);

			return true;
		}
	}
}
