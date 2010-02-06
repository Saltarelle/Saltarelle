using System;
#if SERVER
using System.Text;
#endif

namespace Saltarelle {
	internal class CodeFragment : IFragment {
		private readonly string code;
		private readonly int indentEffect;
	
		public CodeFragment(string code, int indentEffect) {
			this.code = code;
			this.indentEffect = indentEffect;
		}

		public IFragment TryMergeWithNext(IFragment nextFragment) {
			return null;
		}

		public void Render(ITemplate tpl, IInstantiatedTemplateControl ctl, StringBuilder sb) {
			throw ParserUtils.TemplateErrorException("Dynamically instantiated templates cannot have embedded code.");
		}

#if SERVER
		public override bool Equals(object obj) {
			var other = (obj as CodeFragment);
			return other != null && other.code == code && other.indentEffect == indentEffect;
		}

		public override int GetHashCode() {
			return code.GetHashCode() ^ indentEffect.GetHashCode();
		}

		public override string ToString() {
			return "[CODE:" + code + ", indent=" + Utils.ToStringInvariantInt(indentEffect) + "]";
		}
		
		public void WriteCode(ITemplate tpl, FragmentCodePoint point, CodeBuilder cb) {
			if (!string.IsNullOrEmpty(code))
				cb.AppendLine(code);
			for (int i = 0; i < indentEffect; i++)
				cb.Indent();
			for (int i = 0; i < -indentEffect; i++)
				cb.Outdent();
		}
#endif
	}
}
