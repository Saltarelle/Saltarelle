using System;
#if SERVER
using System.Text;
using TypedMarkupParserImplDictionary = System.Collections.Generic.Dictionary<string, Saltarelle.ITypedMarkupParserImpl>;
using System.Xml;
#else
using TypedMarkupParserImplDictionary = System.Dictionary;
using XmlNode = System.XML.XMLNode;
#endif

namespace Saltarelle {
	public class SaltarelleParser {
		private readonly INodeProcessor[] pluginNodeProcessors;
		private readonly TypedMarkupParserImplDictionary pluginTypedMarkupParsers;
		private readonly IUntypedMarkupParserImpl[] pluginUntypedMarkupParsers;
		private readonly DocumentProcessor docProcessor;

		/// <summary>
		/// On the server, an easier way to obtain a parser is through the SaltarelleParserFactory class.
		/// </summary>
		public SaltarelleParser(INodeProcessor[] pluginNodeProcessors, TypedMarkupParserImplDictionary pluginTypedMarkupParsers, IUntypedMarkupParserImpl[] pluginUntypedMarkupParsers) {
			docProcessor = new DocumentProcessor(pluginNodeProcessors, new TypedMarkupParser(pluginTypedMarkupParsers), new UntypedMarkupParser(pluginUntypedMarkupParsers));
			this.pluginNodeProcessors     = pluginNodeProcessors ?? new INodeProcessor[0];
			this.pluginTypedMarkupParsers = pluginTypedMarkupParsers ?? new TypedMarkupParserImplDictionary();
			this.pluginUntypedMarkupParsers = pluginUntypedMarkupParsers ?? new IUntypedMarkupParserImpl[0];
		}

		public ITemplate ParseTemplate(XmlNode node) {
			return docProcessor.Process(node);
		}
		
		public IFragment ParseUntypedMarkup(string markup) {
			return docProcessor.ParseUntypedMarkup(markup);
		}
		
		public TypedMarkupData ParseTypedMarkup(string markup) {
			return docProcessor.ParseTypedMarkup(markup);
		}

		public INodeProcessor[] PluginNodeProcessors { get { return pluginNodeProcessors; } }
		public TypedMarkupParserImplDictionary PluginTypedMarkupParsers { get { return pluginTypedMarkupParsers; } }
		public IUntypedMarkupParserImpl[] PluginUntypedMarkupParsers { get { return pluginUntypedMarkupParsers; } }

#if SERVER
		public string GetClientInstantiateCode() {
			StringBuilder sb = new StringBuilder();
			sb.Append("new " + typeof(SaltarelleParser).FullName + "([");
			bool first = true;
			var sm = GlobalServices.GetService<IScriptManagerService>();
			foreach (var np in pluginNodeProcessors) {
				Type tp = np.GetType();
				if (!first)
					sb.Append(", ");
				sm.RegisterType(tp);
				sb.Append("new " + tp.FullName + "()");
				first = false;
			}
			sb.Append("], { ");
			first = true;
			foreach (var kvp in pluginTypedMarkupParsers) {
				Type tp = kvp.Value.GetType();
				if (!first)
					sb.Append(", ");
				sm.RegisterType(tp);
				sb.Append(Utils.ScriptStr(kvp.Key) + " : new " + tp.FullName + "()");
				first = false;
			}
			sb.Append(" }, [");
			first = true;
			foreach (IUntypedMarkupParserImpl up in pluginUntypedMarkupParsers) {
				Type tp = up.GetType();
				if (!first)
					sb.Append(", ");
				sm.RegisterType(tp);
				sb.Append("new " + tp.FullName + "()");
				first = false;
			}
			sb.Append("])");
			return sb.ToString();
		}
#endif
	}
}