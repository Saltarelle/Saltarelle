using System;
#if CLIENT
using XmlNode = System.XML.XMLNode;
using XmlNodeType = System.XML.XMLNodeType;
using XmlAttribute = System.XML.XMLAttribute;
#else
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
#endif

namespace Saltarelle.NodeProcessors {
	internal class LeafNodeProcessor : INodeProcessor {
		private static string NormalizeSpaces(string s) {
			#if CLIENT
				return s.Replace(new RegularExpression("\\s+", "g"), " ");
			#else
				StringBuilder sb = new StringBuilder();
				bool atSpace = false;
				foreach (char ch in s) {
					if (char.IsWhiteSpace(ch)) {
						if (!atSpace) sb.Append(" ");
						atSpace = true;
					}
					else {
						sb.Append(ch);
						atSpace = false;
					}
				}
				return sb.ToString();
			#endif
		}

		public bool TryProcess(IDocumentProcessor docProcessor, XmlNode node, bool isRoot, ITemplate template, IRenderFunction currentRenderFunction) {
			switch (node.NodeType) {
				#if CLIENT
					case XmlNodeType.CharacterData:
				#else
					case XmlNodeType.CDATA:
				#endif
					currentRenderFunction.AddFragment(new LiteralFragment(Utils.NodeValue(node), true));
					return true;

				case XmlNodeType.Text:
				#if !CLIENT
					case XmlNodeType.Whitespace:
					case XmlNodeType.SignificantWhitespace:
				#endif
					currentRenderFunction.AddFragment(new LiteralFragment(NormalizeSpaces(Utils.NodeValue(node))));
					return true;

				case XmlNodeType.Comment:
					return true;

				default:
					return false;
			}
		}
	}
}
