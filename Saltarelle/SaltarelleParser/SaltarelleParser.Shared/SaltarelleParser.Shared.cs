using System;
#if SERVER
using System.Text;
using TypedMarkupParserImplDictionary = System.Collections.Generic.Dictionary<string, Saltarelle.ITypedMarkupParserImpl>;
using System.Xml;
using System.Linq;
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
			#if CLIENT
				Dictionary cfg = Dictionary.GetDictionary(pluginNodeProcessors);
				if (cfg.ContainsKey("pluginNodeProcessors")) {
					// We have an [AlternateSignature] constructor which can cause us to be called with a config object instead of real parameters
					string[]   npTypes         = (string[])cfg["pluginNodeProcessors"];
					Dictionary tmTypes         = (Dictionary)cfg["pluginTypedMarkupParsers"];
					string[]   umTypes         = (string[])cfg["pluginUntypedMarkupParsers"];
					pluginNodeProcessors       = new INodeProcessor[npTypes.Length];
					pluginTypedMarkupParsers   = new TypedMarkupParserImplDictionary();
					pluginUntypedMarkupParsers = new IUntypedMarkupParserImpl[umTypes.Length];
					for (int i = 0; i < npTypes.Length; i++) {
						Type tp = Utils.FindType(npTypes[i]);
						pluginNodeProcessors[i] = (INodeProcessor)Type.CreateInstance(tp);
					}
					foreach (DictionaryEntry tm in tmTypes) {
						Type tp = Utils.FindType((string)tm.Value);
						pluginTypedMarkupParsers[tm.Key] = (ITypedMarkupParserImpl)Type.CreateInstance(tp);
					}
					for (int i = 0; i < umTypes.Length; i++) {
						Type tp = Utils.FindType(umTypes[i]);
						pluginUntypedMarkupParsers[i] = (IUntypedMarkupParserImpl)Type.CreateInstance(tp);
					}
				}
			#endif
			docProcessor = new DocumentProcessor(pluginNodeProcessors, new TypedMarkupParser(pluginTypedMarkupParsers), new UntypedMarkupParser(pluginUntypedMarkupParsers));
			this.pluginNodeProcessors       = pluginNodeProcessors ?? new INodeProcessor[0];
			this.pluginTypedMarkupParsers   = pluginTypedMarkupParsers ?? new TypedMarkupParserImplDictionary();
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
		public void RegisterTypesForClient(IScriptManagerService svc) {
			svc.RegisterClientType(GetType());
			foreach (var p in this.pluginNodeProcessors)
				svc.RegisterClientType(p.GetType());
			foreach (var p in this.pluginTypedMarkupParsers.Values)
				svc.RegisterClientType(p.GetType());
			foreach (var p in pluginUntypedMarkupParsers)
				svc.RegisterClientType(p.GetType());
		}
		
		public object ConfigObject {
			get {
				return new { pluginNodeProcessors       = pluginNodeProcessors.Select(x => x.GetType().FullName),
				             pluginTypedMarkupParsers   = pluginTypedMarkupParsers.ToDictionary(x => x.Key, x => x.Value.GetType().FullName),
				             pluginUntypedMarkupParsers = pluginUntypedMarkupParsers.Select(x => x.GetType().FullName)
				           };
			}
		}
#endif
#if CLIENT
		[AlternateSignature] public extern SaltarelleParser(object config);
#endif
	}
}