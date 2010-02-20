using System;
#if SERVER
using System.Text;
#endif

namespace Saltarelle.Fragments {
	internal class ConfigObjectFragment : IFragment {
		public ConfigObjectFragment() {
		}

		public IFragment TryMergeWithNext(IFragment nextFragment) {
			return null;
		}

		public void Render(ITemplate tpl, IInstantiatedTemplateControl ctl, StringBuilder sb) {
			#if SERVER
				StringBuilder inner = new StringBuilder();
				inner.Append("{controlTypes:{");
				bool first = true;
				foreach (var kvp in ctl.Controls) {
					if (!first)
						inner.Append(",");
					inner.Append("\"" + kvp.Key + "\":" + Utils.ScriptStr(kvp.Value.GetType().FullName));
					first = false;
				}
				inner.Append("},namedElements:[");
				first = true;
				foreach (var name in ctl.NamedElementNames) {
					if (!first)
						inner.Append(",");
					inner.Append(Utils.ScriptStr(name));
					first = false;
				}
				inner.Append("]}");
				sb.Append(" __cfg=\"" + Utils.HtmlEncode(inner.ToString()) + "\"");
			#endif
		}

#if SERVER
		public override bool Equals(object obj) {
			return obj is ConfigObjectFragment;
		}

		public override int GetHashCode() {
			return 0x552F8D95;
		}

		public override string ToString() {
			return "[CFG]";
		}

		public void WriteCode(ITemplate tpl, FragmentCodePoint point, CodeBuilder cb) {
			if (point == FragmentCodePoint.ServerRender)
				cb.AppendLine(ParserUtils.RenderFunctionStringBuilderName + ".Append(\" __cfg=\\\"\" + Utils.HtmlEncode(Utils.Json(GetConfig())) + \"\\\"\");");
		}
#endif
	}
}
