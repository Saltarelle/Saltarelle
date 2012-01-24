using System;
using Saltarelle.Members;
using Saltarelle.Fragments;
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
	internal class FunctionDefinitionAndCallNodeProcessor : INodeProcessor {
		internal static void ProcessDefFragment(IDocumentProcessor docProcessor, XmlNode node, ITemplate template) {
			XmlAttribute nameAttr   = (XmlAttribute)node.Attributes.GetNamedItem("name");
			XmlAttribute paramsAttr = (XmlAttribute)node.Attributes.GetNamedItem("params");
			
			if (Utils.IsNull(nameAttr))
				throw ParserUtils.TemplateErrorException("The <def-fragment> element must have the name attribute specified.");
			string name = Utils.NodeValue(nameAttr);
			if (!ParserUtils.IsValidUnqualifiedName(name))
				throw ParserUtils.TemplateErrorException("The name " + name + " is not a valid unqualified identifier.");
			if (template.HasMember(name))
				throw ParserUtils.TemplateErrorException("Duplicate definition of member " + name + ".");

			RenderFunctionMember m = new RenderFunctionMember(Utils.NodeValue(nameAttr), !Utils.IsNull(paramsAttr) ? Utils.NodeValue(paramsAttr) : "");

			Utils.DoForEachChild(node, delegate(XmlNode n) {
				docProcessor.ProcessRecursive(n, template, m);
			});

			if (template.HasMember(name))
				throw ParserUtils.TemplateErrorException("Duplicate definition of member " + name + "."); // Just in case it has already been added during the recursive call.
			template.AddMember(m);
		}

		internal static IFragment ProcessCallFragment(IDocumentProcessor docProcessor, XmlNode node) {
			XmlAttribute nameAttr   = (XmlAttribute)node.Attributes.GetNamedItem("name");
			XmlAttribute paramsAttr = (XmlAttribute)node.Attributes.GetNamedItem("params");
			
			if (Utils.IsNull(nameAttr))
				throw ParserUtils.TemplateErrorException("The <call-fragment> element must have the name attribute specified.");
			string name = Utils.NodeValue(nameAttr);
			if (!ParserUtils.IsValidUnqualifiedName(name))
				throw ParserUtils.TemplateErrorException("The name " + name + " is not a valid unqualified identifier.");
			if (Utils.GetNumChildNodes(node) != 0)
				throw ParserUtils.TemplateErrorException("The <call-fragment> element cannot have children.");

			return new CodeExpressionFragment(name + "(" + (!Utils.IsNull(paramsAttr) ? Utils.NodeValue(paramsAttr) : "") + ")");
		}

		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			string name = Utils.NodeName(node);
			if (node.NodeType == XmlNodeType.Element && name == "def-fragment") {
				if (isRoot)
					throw ParserUtils.TemplateErrorException("Fragment definitions must be inside the template.");
				ProcessDefFragment(docProcessor, node, template);
				return true;
			}
			else if (node.NodeType == XmlNodeType.Element && name == "call-fragment") {
				if (isRoot)
					throw ParserUtils.TemplateErrorException("Fragment instantiations must be inside the template.");
				currentRenderFunction.AddFragment(ProcessCallFragment(docProcessor, node));
				return true;
			}
			else
				return false;
		}
	}
}
