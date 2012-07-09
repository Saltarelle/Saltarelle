using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Saltarelle;

namespace DemoWeb.Plugins {
	[NodeProcessor]
	public class CopyrightNodeProcessor : INodeProcessor {
		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.Element || node.Name != "copyright")
				return false;

			if (node.ChildNodes.Count != 1 || node.ChildNodes[0].NodeType != XmlNodeType.Text)
				throw new TemplateErrorException("The copyright node must have a single text child.");

			CopyrightMember m = new CopyrightMember();
			if (template.HasMember(m.Name))
				throw new TemplateErrorException("Duplicate definition of the member " + m.Name);
			template.AddMember(m);
			
			currentRenderFunction.AddFragment(new CopyrightFragment(node.ChildNodes[0].Value));
			
			return true;
		}
	}
}
