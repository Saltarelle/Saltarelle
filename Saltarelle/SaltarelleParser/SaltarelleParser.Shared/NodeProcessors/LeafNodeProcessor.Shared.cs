using System;
using System.Text.RegularExpressions;
using Saltarelle.Fragments;
using System.Xml;
using System.Text;

namespace Saltarelle.NodeProcessors {
	internal class LeafNodeProcessor : INodeProcessor {
		private static string NormalizeSpaces(string s) {
			#if CLIENT
				return s.ReplaceRegex(new Regex("\\s+", "g"), " ");
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
				case XmlNodeType.CDATA:
					currentRenderFunction.AddFragment(new LiteralFragment(node.Value, true));
					return true;

				case XmlNodeType.Text:
				#if !CLIENT
					case XmlNodeType.Whitespace:
					case XmlNodeType.SignificantWhitespace:
				#endif
					currentRenderFunction.AddFragment(new LiteralFragment(NormalizeSpaces(node.Value)));
					return true;

				case XmlNodeType.Comment:
					return true;

				default:
					return false;
			}
		}
	}
}
