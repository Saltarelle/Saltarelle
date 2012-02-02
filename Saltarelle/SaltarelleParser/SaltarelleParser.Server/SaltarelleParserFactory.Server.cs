using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Saltarelle.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Web.Configuration;
using System.Web.Hosting;

namespace Saltarelle {
	public static class SaltarelleParserFactory {
		/// <summary>
		/// Creates a parser with the specified plugin assemblies.
		/// </summary>
		public static ISaltarelleParser CreateParserWithPlugins(IEnumerable<Assembly> plugins) {
			var nodeProcessors            = new List<INodeProcessor>();
			var typedParserImplementers   = new Dictionary<string, ITypedMarkupParserImpl>();
			var untypedParserImplementers = new List<IUntypedMarkupParserImpl>();

            if (plugins != null) {
                foreach (var asm in plugins) {
				    nodeProcessors.AddRange(GetNodeProcessors(asm));
				    AddTypedMarkupParsersToDictionary(asm, typedParserImplementers);
				    untypedParserImplementers.AddRange(GetUntypedParsers(asm));
			    }
            }
			
			return new SaltarelleParser(nodeProcessors.ToArray(), typedParserImplementers, untypedParserImplementers.ToArray());
		}

		private static IEnumerable<INodeProcessor> GetNodeProcessors(Assembly asm) {
			var seq = (   from tp in asm.GetTypes()
			               let attr = (NodeProcessorAttribute)Attribute.GetCustomAttribute(tp, typeof(NodeProcessorAttribute))
			             where !Utils.IsNull(attr)
			           orderby attr.Priority descending
			            select tp);
			var result = new List<INodeProcessor>();
			foreach (var tp in seq) {
				if (!typeof(INodeProcessor).IsAssignableFrom(tp))
					throw new InvalidOperationException("The type " + tp.FullName + ", which is decorated with a NodeProcessorAttribute, does not implement INodeProcessor.");
				if (tp.GetConstructor(Type.EmptyTypes) == null)
					throw new InvalidOperationException("The type " + tp.FullName + ", which is decorated with a NodeProcessorAttribute, does not have a parameterless public constructor.");
				result.Add((INodeProcessor)Activator.CreateInstance(tp));
			}
			return result;
		}

		private static IEnumerable<IUntypedMarkupParserImpl> GetUntypedParsers(Assembly asm) {
			var seq = (   from tp in asm.GetTypes()
			               let attr = (UntypedMarkupParserImplAttribute)Attribute.GetCustomAttribute(tp, typeof(UntypedMarkupParserImplAttribute))
			             where !Utils.IsNull(attr)
			           orderby attr.Priority descending
			            select tp);
			var result = new List<IUntypedMarkupParserImpl>();
			foreach (var tp in seq) {
				if (!typeof(IUntypedMarkupParserImpl).IsAssignableFrom(tp))
					throw new InvalidOperationException("The type " + tp.FullName + ", which is decorated with a UntypedMarkupParserImplAttribute, does not implement IUntypedMarkupParserImpl.");
				if (tp.GetConstructor(Type.EmptyTypes) == null)
					throw new InvalidOperationException("The type " + tp.FullName + ", which is decorated with a UntypedMarkupParserImplAttribute, does not have a parameterless public constructor.");
				result.Add((IUntypedMarkupParserImpl)Activator.CreateInstance(tp));
			}
			return result;
		}

		private static void AddTypedMarkupParsersToDictionary(Assembly asm, IDictionary<string, ITypedMarkupParserImpl> parsers) {
			var seq = (   from tp in asm.GetTypes()
			               let p = new { tp, attr = (TypedMarkupParserImplAttribute)Attribute.GetCustomAttribute(tp, typeof(TypedMarkupParserImplAttribute)) }
			             where !Utils.IsNull(p.attr)
			             group p by p.attr.Prefix into g
			            select g);

			foreach (var g in seq) {
				if (g.Count() > 1)
					throw new InvalidOperationException("The plugin assembly " + asm.GetName().Name + " defines more than one typed markup processor for the prefix " + g.Key + ".");
				var tp = g.Single().tp;
				if (!typeof(ITypedMarkupParserImpl).IsAssignableFrom(tp))
					throw new InvalidOperationException("The type " + tp.FullName + ", which is decorated with a TypedMarkupParserImplAttribute, does not implement ITypedMarkupParserImpl.");
				if (tp.GetConstructor(Type.EmptyTypes) == null)
					throw new InvalidOperationException("The type " + tp.FullName + ", which is decorated with a TypedMarkupParserImplAttribute, does not have a parameterless public constructor.");
				if (!parsers.ContainsKey(g.Key))
					parsers.Add(g.Key, (ITypedMarkupParserImpl)Activator.CreateInstance(tp));
			}
		}
	}
}
