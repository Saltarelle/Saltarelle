using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using SysConfig = System.Configuration.Configuration;
using Saltarelle.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Web.Configuration;
using System.Web.Hosting;

namespace Saltarelle {
	public static class SaltarelleParserFactory {
		/// <summary>Creates a parser based on the content of a config section.</summary>
		/// <param name="config">Configuration file</param>
		/// <param name="rootPath">Path to resolve referenced assembly files in.</param>
		public static SaltarelleParser CreateParserFromConfigSection(SaltarelleConfigSection config, string rootPath) {
			var nodeProcessors            = new List<INodeProcessor>();
			var typedParserImplementers   = new Dictionary<string, ITypedMarkupParserImpl>();
			var untypedParserImplementers = new List<IUntypedMarkupParserImpl>();

			if (config != null && config.Plugins != null) {
				foreach (PluginElement p in config.Plugins) {
					Assembly asm = LoadAssembly(p.Assembly, rootPath);
					nodeProcessors.AddRange(GetNodeProcessors(asm));
					AddTypedMarkupParsersToDictionary(asm, typedParserImplementers);
					untypedParserImplementers.AddRange(GetUntypedParsers(asm));
				}
			}
			
			return new SaltarelleParser(nodeProcessors.ToArray(), typedParserImplementers, untypedParserImplementers.ToArray());
		}

		/// <summary>
		/// Creates a parser with plugins specified in a config file.
		/// </summary>
		public static SaltarelleParser CreateParserFromConfigFile(string configFile) {
			if (string.IsNullOrEmpty(configFile) || !File.Exists(configFile)) throw new ArgumentException("configFile");
			return CreateParserFromConfigSection(LoadConfig(configFile), Path.GetDirectoryName(configFile));
		}
		
		/// <summary>
		/// Creates a parser with no plugins.
		/// </summary>
		public static SaltarelleParser CreateDefaultParser() {
			return new SaltarelleParser(new INodeProcessor[0], new Dictionary<string, ITypedMarkupParserImpl>(), new IUntypedMarkupParserImpl[0]);
		}
		
		/// <summary>
		/// Creates a parser with plugins loaded from web.config.
		/// </summary>
		public static SaltarelleParser CreateParserFromWebConfig() {
			return CreateParserFromConfigSection((SaltarelleConfigSection)WebConfigurationManager.GetSection("saltarelle"), HostingEnvironment.MapPath("~/bin"));
		}

		private static SaltarelleConfigSection LoadConfig(string fileName) {
			if (fileName == null) throw new ArgumentException("fileName");
			ConfigurationFileMap fileMap = new ConfigurationFileMap(fileName);
			SysConfig cfg = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
			return cfg != null ? (SaltarelleConfigSection)cfg.Sections["saltarelle"] : null;
		}
		
		private static IEnumerable<INodeProcessor> GetNodeProcessors(Assembly asm) {
			var seq = (   from tp in asm.GetTypes()
			               let attr = (NodeProcessorAttribute)Attribute.GetCustomAttribute(tp, typeof(NodeProcessorAttribute))
			             where attr != null
			           orderby attr.Priority descending
			            select tp);
			var result = new List<INodeProcessor>();
			foreach (var tp in seq) {
				if (!typeof(INodeProcessor).IsAssignableFrom(tp))
					throw new ConfigurationErrorsException("The type " + tp.FullName + ", which is decorated with a NodeProcessorAttribute, does not implement INodeProcessor.");
				if (tp.GetConstructor(Type.EmptyTypes) == null)
					throw new ConfigurationErrorsException("The type " + tp.FullName + ", which is decorated with a NodeProcessorAttribute, does not have a parameterless public constructor.");
				result.Add((INodeProcessor)Activator.CreateInstance(tp));
			}
			return result;
		}

		private static IEnumerable<IUntypedMarkupParserImpl> GetUntypedParsers(Assembly asm) {
			var seq = (   from tp in asm.GetTypes()
			               let attr = (UntypedMarkupParserImplAttribute)Attribute.GetCustomAttribute(tp, typeof(UntypedMarkupParserImplAttribute))
			             where attr != null
			           orderby attr.Priority descending
			            select tp);
			var result = new List<IUntypedMarkupParserImpl>();
			foreach (var tp in seq) {
				if (!typeof(IUntypedMarkupParserImpl).IsAssignableFrom(tp))
					throw new ConfigurationErrorsException("The type " + tp.FullName + ", which is decorated with a UntypedMarkupParserImplAttribute, does not implement IUntypedMarkupParserImpl.");
				if (tp.GetConstructor(Type.EmptyTypes) == null)
					throw new ConfigurationErrorsException("The type " + tp.FullName + ", which is decorated with a UntypedMarkupParserImplAttribute, does not have a parameterless public constructor.");
				result.Add((IUntypedMarkupParserImpl)Activator.CreateInstance(tp));
			}
			return result;
		}

		private static void AddTypedMarkupParsersToDictionary(Assembly asm, IDictionary<string, ITypedMarkupParserImpl> parsers) {
			var seq = (   from tp in asm.GetTypes()
			               let p = new { tp, attr = (TypedMarkupParserImplAttribute)Attribute.GetCustomAttribute(tp, typeof(TypedMarkupParserImplAttribute)) }
			             where p.attr != null
			             group p by p.attr.Prefix into g
			            select g);

			foreach (var g in seq) {
				if (g.Count() > 1)
					throw new ConfigurationErrorsException("The plugin assembly " + asm.GetName().Name + " defines more than one typed markup processor for the prefix " + g.Key + ".");
				var tp = g.Single().tp;
				if (!typeof(ITypedMarkupParserImpl).IsAssignableFrom(tp))
					throw new ConfigurationErrorsException("The type " + tp.FullName + ", which is decorated with a TypedMarkupParserImplAttribute, does not implement ITypedMarkupParserImpl.");
				if (tp.GetConstructor(Type.EmptyTypes) == null)
					throw new ConfigurationErrorsException("The type " + tp.FullName + ", which is decorated with a TypedMarkupParserImplAttribute, does not have a parameterless public constructor.");
				if (!parsers.ContainsKey(g.Key))
					parsers.Add(g.Key, (ITypedMarkupParserImpl)Activator.CreateInstance(tp));
			}
		}
		
		private static Assembly LoadAssembly(string identifier, string rootPath) {
			// There might be a better way of doing this, but if so, I don't know about it.
			try {
				return Assembly.Load(new AssemblyName(identifier));
			}
			catch (Exception) {
				string path = Path.IsPathRooted(identifier) ? identifier : Path.Combine(rootPath, identifier);
				return Assembly.LoadFrom(path);
			}
		}
	}
}
