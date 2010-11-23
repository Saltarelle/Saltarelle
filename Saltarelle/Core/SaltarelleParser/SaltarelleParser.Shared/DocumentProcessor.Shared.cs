using System;
using Saltarelle.NodeProcessors;
#if CLIENT
using XmlNode = System.XML.XMLNode;
#else
using System.Xml;
#endif

namespace Saltarelle {
	public interface IDocumentProcessor {
		void ProcessRecursive(XmlNode node, ITemplate template, IRenderFunction currentRenderFunction);
		ITemplate Process(XmlNode node);
		TypedMarkupData ParseTypedMarkup(string markup);
		IFragment ParseUntypedMarkup(string markup);
	}

	internal class DocumentProcessor : IDocumentProcessor {
		private static INodeProcessor[] defaultNodeProcessors = { new ImplementsOrInheritsNodeProcessor(),
		                                                          new UsingDirectiveNodeProcessor(),
		                                                          new FieldNodeProcessor(),
		                                                          new ViewDirectiveNodeProcessor(),
		                                                          new FunctionDefinitionAndCallNodeProcessor(),
		                                                          new ControlInstantiationNodeProcessor(),
		                                                          new ControlFlowTagProcessor(),
		                                                          new LeafNodeProcessor(),
		                                                          new GenericElementProcessor(),
		                                                          new EmbeddedCodeNodeProcessor(),
		                                                        };

		private INodeProcessor[] processors;
		private ITypedMarkupParser typedMarkupParser;
		private IUntypedMarkupParser untypedMarkupParser;

		public DocumentProcessor(INodeProcessor[] pluginNodeProcessors, ITypedMarkupParser typedMarkupParser, IUntypedMarkupParser untypedMarkupParser) {
			processors = (INodeProcessor[])Utils.ArrayAppendRange((pluginNodeProcessors ?? new INodeProcessor[0]), defaultNodeProcessors);
			this.typedMarkupParser = typedMarkupParser ?? new TypedMarkupParser(null);
			this.untypedMarkupParser = untypedMarkupParser ?? new UntypedMarkupParser(null);
		}

		private void ActualProcess(XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			foreach (INodeProcessor p in processors) {
				if (p.TryProcess(this, node, isRoot, template, currentRenderFunction))
					return;
			}
			throw ParserUtils.TemplateErrorException("The node " + node.ToString() + " could not be handled.");
		}
		
		public void ProcessRecursive(XmlNode node, ITemplate template, IRenderFunction currentRenderFunction) {
			if (Utils.IsNull(template)) throw Utils.ArgumentException("template");
			if (Utils.IsNull(currentRenderFunction)) throw Utils.ArgumentException("currentRenderFunction");
			if (Utils.IsNull(node)) throw Utils.ArgumentNullException("node");

			ActualProcess(node, false, template, currentRenderFunction);
		}

		public ITemplate Process(XmlNode node) {
			if (Utils.IsNull(node)) throw Utils.ArgumentNullException("node");
			
			ITemplate result = new Template();
			Utils.DoForEachChild(node, delegate(XmlNode child) {
				ActualProcess(child, true, result, result.MainRenderFunction);
			});
			return result;
		}
		
		public TypedMarkupData ParseTypedMarkup(string markup) {
			return typedMarkupParser.ParseMarkup(markup);
		}

		public IFragment ParseUntypedMarkup(string markup) {
			return untypedMarkupParser.ParseMarkup(markup);
		}
	}
}
