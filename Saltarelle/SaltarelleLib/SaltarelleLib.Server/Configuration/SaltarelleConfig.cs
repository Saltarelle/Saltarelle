using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace Saltarelle.Configuration {
	public class SaltarelleConfig {
		public List<PluginElement> Plugins { get; private set; }
		public RoutesElement Routes { get; private set; }
		public ScriptElementCollection Scripts { get; private set; }

        private string MakeFullPath(string path, string rootPath) {
            return Path.IsPathRooted(path) ? path : Path.Combine(rootPath, path);;
        }

        private T HandleTypeLoadException<T>(Func<T> f) {
            try {
                return f();
            }
            catch (ReflectionTypeLoadException ex) {
                if (ex.LoaderExceptions.Length > 0)
                    throw ex.LoaderExceptions[0];
                else
                    throw;
            }
        }

		public SaltarelleConfig() {
			Plugins = new List<PluginElement>();
			Routes  = new RoutesElement();
			Scripts = new ScriptElementCollection() { Debug = true };
		}

        public IEnumerable<Assembly> LoadPluginsInLoadFromContext(string rootPath) {
            var result = new List<Assembly>();
            if (!Utils.IsNull(Plugins)) {
				foreach (PluginElement p in Plugins) {
                    var asm = HandleTypeLoadException(() => Assembly.LoadFrom(MakeFullPath(p.Assembly, rootPath)));
                    result.Add(asm);
                }
            }
            return result;
        }

        public IList<Assembly> LoadPluginsInNoContext(string rootPath) {
            var result = new List<Assembly>();
            if (!Utils.IsNull(Plugins)) {
				foreach (PluginElement p in Plugins) {
                    byte[] content = File.ReadAllBytes(MakeFullPath(p.Assembly, rootPath));
                    var asm = HandleTypeLoadException(() => Assembly.Load(content));
                    result.Add(asm);
                }
            }
            return result;
        }

        public static SaltarelleConfig LoadXml(XmlNode node) {
            if (node.Name != "saltarelle")
                node = node.SelectSingleNode("//saltarelle");
            var ser = new XmlSerializer(typeof(Saltarelle.Configuration.Internal.Xml.saltarelle));
            Saltarelle.Configuration.Internal.Xml.saltarelle obj;
            try {
                obj = (Saltarelle.Configuration.Internal.Xml.saltarelle)ser.Deserialize(new XmlNodeReader(node));
            }
            catch (InvalidOperationException ex) {
                throw ex.InnerException;
            }
            
            var result = new SaltarelleConfig();
			if (obj.plugins != null)
				result.Plugins.AddRange(obj.plugins.Select(MapPlugin));
            if (obj.routes != null) {
                result.Routes.AssemblyScripts   = obj.routes.assemblyScripts;
				result.Routes.AssemblyCss       = obj.routes.assemblyCss;
				result.Routes.AssemblyResources = obj.routes.assemblyResources;
				result.Routes.Delegate          = obj.routes.@delegate;
            }
			if (obj.scripts != null) {
				if (obj.scripts.debugSpecified)
					result.Scripts.Debug = obj.scripts.debug;
				if (obj.scripts.add != null)
					result.Scripts.AddRange(obj.scripts.add.Select(MapScript));
			}

            return result;
        }

		public static SaltarelleConfig LoadFile(string fileName) {
			if (Utils.IsNull(fileName)) throw new ArgumentException("fileName");

            var doc = new XmlDocument();
            doc.Load(fileName);
            return LoadXml(doc.DocumentElement);
		}
		
		public static SaltarelleConfig GetFromWebConfig() {
            var sect = (SaltarelleConfigSection)WebConfigurationManager.GetSection("saltarelle");
			if (sect == null)
				return new SaltarelleConfig();

            var doc = new XmlDocument();
            doc.LoadXml(sect.RawXml);
            return LoadXml(doc.DocumentElement);
		}

        private static PluginElement MapPlugin(Saltarelle.Configuration.Internal.Xml.saltarellePlugin src) {
            return new PluginElement(src.assembly);
        }

        private static ScriptElement MapScript(Saltarelle.Configuration.Internal.Xml.saltarelleScriptsAdd src) {
            var position = (src.positionSpecified ? (ScriptPosition)Enum.Parse(typeof(ScriptPosition), src.position.ToString(), true) : ScriptPosition.BeforeCoreScripts);
            return new ScriptElement(src.url, position, src.assembly, src.resource);
        }
	}
}
