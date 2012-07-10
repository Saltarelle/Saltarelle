using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using Saltarelle.Ioc;
#if SERVER
using System.Linq;
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
		private IDictionary<string, ITypedMarkupParserImpl> pluginTypedMarkupParsers;
		private IUntypedMarkupParserImpl[] pluginUntypedMarkupParsers;
		private DocumentProcessor docProcessor;

        #if CLIENT
            private JsDictionary configObject;

            private void EnsureInitialized() {
                if (configObject == null)
                    return;

				var npTypes = (string[])configObject["pluginNodeProcessors"];
				var tmTypes = (JsDictionary)configObject["pluginTypedMarkupParsers"];
				var umTypes = (string[])configObject["pluginUntypedMarkupParsers"];

				pluginNodeProcessors       = new INodeProcessor[npTypes.Length];
				pluginTypedMarkupParsers   = new Dictionary<string, ITypedMarkupParserImpl>();
				pluginUntypedMarkupParsers = new IUntypedMarkupParserImpl[umTypes.Length];

				for (int i = 0; i < npTypes.Length; i++) {
					pluginNodeProcessors[i] = (INodeProcessor)Container.CreateObjectByTypeName(npTypes[i]);
				}
				foreach (var tm in tmTypes) {
					pluginTypedMarkupParsers[tm.Key] = (ITypedMarkupParserImpl)Container.CreateObjectByTypeName((string)tm.Value);
				}
				for (int i = 0; i < umTypes.Length; i++) {
					pluginUntypedMarkupParsers[i] = (IUntypedMarkupParserImpl)Container.CreateObjectByTypeName(umTypes[i]);
				}

			    docProcessor = new DocumentProcessor(pluginNodeProcessors, new TypedMarkupParser(pluginTypedMarkupParsers), new UntypedMarkupParser(pluginUntypedMarkupParsers));

                configObject = null;
            }

            public IContainer Container { get; set; }
        #endif

		/// <summary>
		/// On the server, an easier way to obtain a parser is through the SaltarelleParserFactory class.
		/// </summary>
		public SaltarelleParser(INodeProcessor[] pluginNodeProcessors, IDictionary<string, ITypedMarkupParserImpl> pluginTypedMarkupParsers, IUntypedMarkupParserImpl[] pluginUntypedMarkupParsers) {
			#if CLIENT
				JsDictionary cfg = JsDictionary.GetDictionary(pluginNodeProcessors);
				if (!Script.IsNullOrUndefined(cfg) && cfg.ContainsKey("pluginNodeProcessors")) {
				    // We have an [AlternateSignature] constructor which can cause us to be called with a config object instead of real parameters
                    configObject = cfg;
                    return;
				}
			#endif
			docProcessor = new DocumentProcessor(pluginNodeProcessors, new TypedMarkupParser(pluginTypedMarkupParsers), new UntypedMarkupParser(pluginUntypedMarkupParsers));
			this.pluginNodeProcessors       = pluginNodeProcessors ?? new INodeProcessor[0];
			this.pluginTypedMarkupParsers   = pluginTypedMarkupParsers ?? new Dictionary<string, ITypedMarkupParserImpl>();
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
            #if CLIENT
                EnsureInitialized();
            #endif
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

		public IDictionary<string, ITypedMarkupParserImpl> PluginTypedMarkupParsers {
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
		[AlternateSignature] public SaltarelleParser(object config) {}
#endif
	}
}