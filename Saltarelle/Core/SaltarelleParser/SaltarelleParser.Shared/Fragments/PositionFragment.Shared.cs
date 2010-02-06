using System;
#if SERVER
using System.Text;
#endif

namespace Saltarelle {
	internal class PositionFragment : IFragment {
		public PositionFragment() {
		}

		public IFragment TryMergeWithNext(IFragment nextFragment) {
			return null;
		}

		public void Render(ITemplate tpl, IInstantiatedTemplateControl ctl, StringBuilder sb) {
			sb.Append(PositionHelper.CreateStyle(((IControl)ctl).Position, -1, -1));
		}

#if SERVER
		public override bool Equals(object obj) {
			return obj is PositionFragment;
		}

		public override int GetHashCode() {
			return 0x303AC363;
		}

		public override string ToString() {
			return "[Position]";
		}

		public void WriteCode(ITemplate tpl, FragmentCodePoint point, CodeBuilder cb) {
			cb.AppendLine(ParserUtils.RenderFunctionStringBuilderName + ".Append(PositionHelper.CreateStyle(Position, -1, -1));");
		}
#endif
	}
}
