using System;
#if SERVER
using System.Text;
#endif

namespace Saltarelle {
	internal class IdFragment : IFragment {
		public IdFragment() {
		}

		public IFragment TryMergeWithNext(IFragment nextFragment) {
			return null;
		}

		public void Render(ITemplate tpl, IInstantiatedTemplateControl ctl, StringBuilder sb) {
			sb.Append(((IControl)ctl).Id);
		}

#if SERVER
		public override bool Equals(object obj) {
			return obj is IdFragment;
		}

		public override int GetHashCode() {
			return 0x2FE93416;
		}

		public override string ToString() {
			return "[ID]";
		}
		
		public void WriteCode(ITemplate tpl, FragmentCodePoint point, CodeBuilder cb) {
			cb.AppendLine(ParserUtils.RenderFunctionStringBuilderName + ".Append(Id);");
		}
#endif
	}
}
