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
	internal class UsingDirectiveNodeProcessor : INodeProcessor {
		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.ProcessingInstruction || Utils.NodeName(node) != "using")
				return false;

			if (!isRoot)
				throw ParserUtils.TemplateErrorException(string.Format("The using directive can only appear outside of the template.", Utils.NodeName(node)));
				
			string[] sideArr      = Utils.RegexExec(Utils.NodeValue(node), "side=\"([^\"]*)\"", "");
			string[] namespaceArr = Utils.RegexExec(Utils.NodeValue(node), "namespace=\"([^\"]*)\"", "");
	
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
