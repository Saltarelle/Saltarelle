using System;
using Saltarelle.Members;
using Saltarelle.Fragments;
#if CLIENT
using XmlNode      = System.XML.XMLNode;
using XmlNodeType  = System.XML.XMLNodeType;
using XmlAttribute = System.XML.XMLAttribute;
#else
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
#endif

namespace Saltarelle.NodeProcessors {
	internal class GenericElementProcessor : INodeProcessor {
		private string[] noContentTags = new string[] { "br", "img", "hr", "link", "input", "meta", "col", "frame", "base", "area" };

		internal static void AddSingleAttributeFragments(IDocumentProcessor docProcessor, string attrName, string attrValue, bool isRoot, ITemplate template, IRenderFunction fragments) {
			string actualName;
			if (attrName == "actualName") {
				// This will translate into "name", but not with our ID prefixed. We need this because for top-level templates we want the names undisturbed to allow for a nice form submission.
				// However, for nested forms which we won't submit using the normal techniques, we need to prefix it in order to make radio buttons work reliably.
				actualName = "name";
			}
			else
				actualName = attrName;

			fragments.AddFragment(new LiteralFragment(" " + actualName + "=\""));
			if (attrName == "id") {
				if (isRoot)
					throw ParserUtils.TemplateErrorException("Can't specify an ID for the top level element.");
				if (template.HasMember(attrValue))
					throw ParserUtils.TemplateErrorException("Duplicate member " + attrValue);
				template.AddMember(new NamedElementMember(attrValue));

				fragments.AddFragment(new IdFragment());
				fragments.AddFragment(new LiteralFragment("_" + attrValue));
			}
			else if (attrName == "for" || attrName == "name") {
				fragments.AddFragment(new IdFragment());
				fragments.AddFragment(new LiteralFragment("_" + attrValue));
			}
			else {
				IFragment f = docProcessor.ParseUntypedMarkup(attrValue);
				fragments.AddFragment(f);
				if (isRoot && attrName == "style") {
					if (!(f is LiteralFragment) || !((LiteralFragment)f).Text.Trim().EndsWith(";"))
						fragments.AddFragment(new LiteralFragment(";"));
					fragments.AddFragment(new PositionFragment());
				}
			}

			fragments.AddFragment(new LiteralFragment("\""));
		}

		internal static void AddAttributeFragments(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction fragments) {
			bool hasStyle = false;
			Utils.DoForEachAttribute(node, delegate(XmlAttribute attr) {
				AddSingleAttributeFragments(docProcessor, attr.Name, attr.Value, isRoot, template, fragments);
				if (attr.Name == "style")
					hasStyle = true;
			});

			if (isRoot) {
				fragments.AddFragment(new LiteralFragment(" id=\""));
				fragments.AddFragment(new IdFragment());
				fragments.AddFragment(new LiteralFragment("\""));
				if (!hasStyle) {
					fragments.AddFragment(new LiteralFragment(" style=\""));
					fragments.AddFragment(new PositionFragment());
					fragments.AddFragment(new LiteralFragment("\""));
				}
			}
		}

		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			if (node.NodeType != XmlNodeType.Element)
				return false;

			currentRenderFunction.AddFragment(new LiteralFragment("<" + Utils.NodeName(node)));
			AddAttributeFragments(docProcessor, node, isRoot, template, currentRenderFunction);
			if (isRoot)
				currentRenderFunction.AddFragment(new ConfigObjectFragment());

			if (noContentTags.Contains(Utils.NodeName(node))) {
				if (Utils.GetNumChildNodes(node) > 0)
					throw ParserUtils.TemplateErrorException("The tag " + Utils.NodeName(node) + " can not have children.");
				currentRenderFunction.AddFragment(new LiteralFragment("/>"));
			}
			else {
				currentRenderFunction.AddFragment(new LiteralFragment(">"));
				Utils.DoForEachChild(node, delegate(XmlNode child) {
					docProcessor.ProcessRecursive(child, template, currentRenderFunction);
				});
				currentRenderFunction.AddFragment(new LiteralFragment("</" + Utils.NodeName(node) + ">"));
			}

			return true;
		}
	}
}
