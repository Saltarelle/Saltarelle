using System;
using Saltarelle.Members;
using Saltarelle.Fragments;
using System.Xml;
#if SERVER
using System.Linq;
#endif

namespace Saltarelle.NodeProcessors {
	internal class GenericElementProcessorContext {
		public string Id;
		public string Type;
	}

	internal class GenericElementProcessor : INodeProcessor {
		private string[] noContentTags = new string[] { "br", "img", "hr", "link", "input", "meta", "col", "frame", "base", "area" };

		internal static void AddSingleAttributeFragments(IDocumentProcessor docProcessor, string attrName, string attrValue, bool isRoot, ITemplate template, IRenderFunction fragments, GenericElementProcessorContext context) {
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
				context.Id = attrValue;

				fragments.AddFragment(new IdFragment());
				fragments.AddFragment(new LiteralFragment("_" + attrValue));
			}
			else if (attrName.ToLowerCase() == "for" || attrName.ToLowerCase() == "name") {
				fragments.AddFragment(new IdFragment());
				fragments.AddFragment(new LiteralFragment("_" + attrValue));
			}
			else {
				IFragment f = docProcessor.ParseUntypedMarkup(attrValue);
				fragments.AddFragment(f);
				if (attrName.ToLowerCase() == "type") {
					if (f is LiteralFragment)
						context.Type = ((LiteralFragment)f).Text;
				}
				else if (isRoot && attrName.ToLowerCase() == "style") {
					if (!(f is LiteralFragment) || !((LiteralFragment)f).Text.Trim().EndsWith(";"))
						fragments.AddFragment(new LiteralFragment(";"));
					fragments.AddFragment(new PositionFragment());
				}
			}

			fragments.AddFragment(new LiteralFragment("\""));
		}

		internal static void AddAttributeFragments(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction fragments, GenericElementProcessorContext context) {
			bool hasStyle = false;
			Utils.DoForEachAttribute(node, delegate(XmlAttribute attr) {
				AddSingleAttributeFragments(docProcessor, attr.Name, attr.Value, isRoot, template, fragments, context);
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

			GenericElementProcessorContext context = new GenericElementProcessorContext();

			currentRenderFunction.AddFragment(new LiteralFragment("<" + node.Name));
			AddAttributeFragments(docProcessor, node, isRoot, template, currentRenderFunction, context);

			if (context.Id != null) {
				string tagName = node.Name;
				if (tagName.ToLowerCase() == "input" && context.Type != null)
					tagName += "/" + context.Type;
				template.AddMember(new NamedElementMember(tagName, context.Id));
			}

			if (noContentTags.Contains(node.Name)) {
				if (Utils.GetNumChildNodes(node) > 0)
					throw ParserUtils.TemplateErrorException("The tag " + node.Name + " can not have children.");
				currentRenderFunction.AddFragment(new LiteralFragment("/>"));
			}
			else {
				currentRenderFunction.AddFragment(new LiteralFragment(">"));
				Utils.DoForEachChild(node, delegate(XmlNode child) {
					docProcessor.ProcessRecursive(child, template, currentRenderFunction);
				});
				currentRenderFunction.AddFragment(new LiteralFragment("</" + node.Name + ">"));
			}

			return true;
		}
	}
}
