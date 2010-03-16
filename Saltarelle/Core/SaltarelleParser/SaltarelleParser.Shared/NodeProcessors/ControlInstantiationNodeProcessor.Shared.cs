using System;
using Saltarelle.Members;
using Saltarelle.Fragments;
#if CLIENT
using XmlNode = System.XML.XMLNode;
using XmlAttribute = System.XML.XMLAttribute;
using XmlNodeType = System.XML.XMLNodeType;
using TypedMarkupDictionary = System.Dictionary;
#else
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TypedMarkupDictionary = System.Collections.Generic.Dictionary<string, Saltarelle.TypedMarkupData>;
#endif

namespace Saltarelle.NodeProcessors {
	internal class ControlInstantiationNodeProcessor : INodeProcessor {
		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.Element || Utils.NodeName(node) != "control")
				return false;
			
			if (isRoot)
				throw ParserUtils.TemplateErrorException("The root element can not be a control.");

			string id = null;
			string type = null;
			bool customInstantiate = false;
			TypedMarkupDictionary additionalProperties = new TypedMarkupDictionary();

			Utils.DoForEachAttribute(node, delegate(XmlAttribute attr) {
				if (attr.Name == "id") {
					if (!ParserUtils.IsValidUnqualifiedName(attr.Value))
						throw ParserUtils.TemplateErrorException("The id '" + attr.Value + "' is not a valid identifier.");
					id = attr.Value;
				}
				else if (attr.Name == "type") {
					if (string.IsNullOrEmpty(attr.Value))
						throw ParserUtils.TemplateErrorException("The control type '" + attr.Value + "' is invalid.");
					type = attr.Value;
				}
				else if (attr.Name == "customInstantiate") {
					string v = attr.Value.ToLowerCase();
					customInstantiate = Utils.ParseBool(attr.Value);
				}
				else {
					additionalProperties[attr.Name] = docProcessor.ParseTypedMarkup(attr.Value);
				}
			});
			
			if (customInstantiate && additionalProperties.Count > 0)
				throw ParserUtils.TemplateErrorException("There can not be any property assignments when customInstantiate is true.");

			if (Utils.IsNull(type))
				throw ParserUtils.TemplateErrorException("The control '" + id + "' does not have a type specified.");
			if (Utils.IsNull(id))
				id = template.GetUniqueId();
			if (template.HasMember(id))
				throw ParserUtils.TemplateErrorException("Duplicate definition of member " + id);

			IMember[] dependencies = new IMember[0];
			bool hasInnerHtml = false;
			if (Utils.GetNumChildNodes(node) > 0) {
				IRenderFunction innerFunction = new RenderFunctionMember(id + "_inner", "");

				Utils.DoForEachChild(node, delegate(XmlNode n) {
					docProcessor.ProcessRecursive(n, template, innerFunction);
				});
				
				if (!innerFunction.IsEmpty) {
					if (template.HasMember(((IMember)innerFunction).Name))
						throw ParserUtils.TemplateErrorException("The internal name " + ((IMember)innerFunction).Name + " is already in use.");
					template.AddMember((IMember)innerFunction);

					dependencies = new IMember[] { (IMember)innerFunction };
					hasInnerHtml = true;
				}
			}
			
			IMember controlMember = new InstantiatedControlMember(id, type, customInstantiate, additionalProperties, hasInnerHtml, dependencies);

			template.AddMember(controlMember);

			currentRenderFunction.AddFragment(new InstantiatedControlFragment(id, customInstantiate, hasInnerHtml));
			currentRenderFunction.AddDependency(controlMember);

			return true;
		}
	}
}
