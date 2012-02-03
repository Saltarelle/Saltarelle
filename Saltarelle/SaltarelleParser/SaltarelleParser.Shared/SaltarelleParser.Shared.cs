using System;
using Saltarelle.Ioc;
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
    public interface ISaltarelleParser {
		ITemplate ParseTemplate(XmlNode node);
		IFragment ParseUntypedMarkup(string markup);
		TypedMarkupData ParseTypedMarkup(string markup);

        #if SERVER
		    object ConfigObject { get; }
        #endif
    }

	#if SERVER
    [ClientInjectProperty(typeof(IContainer), "Container")]
    #endif
    public class SaltarelleParser : ISaltarelleParser {
		private INodeProcessor[] pluginNodeProcessors;
		private TypedMarkupParserImplDictionary pluginTypedMarkupParsers;
		private IUntypedMarkupParserImpl[] pluginUntypedMarkupParsers;
		private DocumentProcessor docProcessor;

        #if CLIENT
            private Dictionary configObject;
            private IContainer container;

            private void EnsureInitialized() {
                if (configObject == null)
                    return;

				string[]   npTypes = (string[])configObject["pluginNodeProcessors"];
				Dictionary tmTypes = (Dictionary)configObject["pluginTypedMarkupParsers"];
				string[]   umTypes = (string[])configObject["pluginUntypedMarkupParsers"];

				pluginNodeProcessors       = new INodeProcessor[npTypes.Length];
				pluginTypedMarkupParsers   = new TypedMarkupParserImplDictionary();
				pluginUntypedMarkupParsers = new IUntypedMarkupParserImpl[umTypes.Length];

				for (int i = 0; i < npTypes.Length; i++) {
					pluginNodeProcessors[i] = (INodeProcessor)Container.CreateObjectByTypeName(npTypes[i]);
				}
				foreach (DictionaryEntry tm in tmTypes) {
					pluginTypedMarkupParsers[tm.Key] = (ITypedMarkupParserImpl)Container.CreateObjectByTypeName((string)tm.Value);
				}
				for (int i = 0; i < umTypes.Length; i++) {
					pluginUntypedMarkupParsers[i] = (IUntypedMarkupParserImpl)Container.CreateObjectByTypeName(umTypes[i]);
				}

			    docProcessor = new DocumentProcessor(pluginNodeProcessors, new TypedMarkupParser(pluginTypedMarkupParsers), new UntypedMarkupParser(pluginUntypedMarkupParsers));

                configObject = null;
            }

            public IContainer Container {
                get { return container; }
                set { container = value; }
            }
        #endif

		/// <summary>
		/// On the server, an easier way to obtain a parser is through the SaltarelleParserFactory class.
		/// </summary>
		public SaltarelleParser(INodeProcessor[] pluginNodeProcessors, TypedMarkupParserImplDictionary pluginTypedMarkupParsers, IUntypedMarkupParserImpl[] pluginUntypedMarkupParsers) {
			#if CLIENT
				Dictionary cfg = Dictionary.GetDictionary(pluginNodeProcessors);
				if (cfg.ContainsKey("pluginNodeProcessors")) {
				    // We have an [AlternateSignature] constructor which can cause us to be called with a config object instead of real parameters
                    configObject = cfg;
                    return;
				}
			#endif
			docProcessor = new DocumentProcessor(pluginNodeProcessors, new TypedMarkupParser(pluginTypedMarkupParsers), new UntypedMarkupParser(pluginUntypedMarkupParsers));
			this.pluginNodeProcessors       = pluginNodeProcessors ?? new INodeProcessor[0];
			this.pluginTypedMarkupParsers   = pluginTypedMarkupParsers ?? new TypedMarkupParserImplDictionary();
			this.pluginUntypedMarkupParsers = pluginUntypedMarkupParsers ?? new IUntypedMarkupParserImpl[0];
		}

		public ITemplate ParseTemplate(XmlNode node) {
            #if CLIENT
                EnsureInitialized();
            #endif
			return docProcessor.Process(node);
		}
		
		public IFragment ParseUntypedMarkup(string markup) {
            #if CLIENT
                EnsureInitialized();
            #endif
			return docProcessor.ParseUntypedMarkup(markup);
		}
		
		public TypedMarkupData ParseTypedMarkup(string markup) {
			return docProcessor.ParseTypedMarkup(markup);
		}

		public INodeProcessor[] PluginNodeProcessors {
            get {
                #if CLIENT
                    EnsureInitialized();
                #endif
                return pluginNodeProcessors;
            }
        }

		public TypedMarkupParserImplDictionary PluginTypedMarkupParsers {
            get {
                #if CLIENT
                    EnsureInitialized();
                #endif
                return pluginTypedMarkupParsers;
            }
        }

		public IUntypedMarkupParserImpl[] PluginUntypedMarkupParsers {
            get {
                #if CLIENT
                    EnsureInitialized();
                #endif
                return pluginUntypedMarkupParsers;
            }
        }

#if SERVER
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