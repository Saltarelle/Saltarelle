using System;
using System.Text;

namespace Saltarelle.Fragments {
	internal class CodeExpressionFragment : IFragment {
		public readonly string Expression;
	
		public CodeExpressionFragment(string expression) {
			if (string.IsNullOrEmpty(expression)) throw Utils.ArgumentException("expression");
			this.Expression = expression;
		}

		public IFragment TryMergeWithNext(IFragment nextFragment) {
			return null;
		}

		public void Render(ITemplate tpl, IInstantiatedTemplateControl ctl, StringBuilder sb) {
			throw ParserUtils.TemplateErrorException("Dynamically instantiated templates cannot have embedded code.");
		}

#if SERVER
		public override bool Equals(object obj) {
			return (obj is CodeExpressionFragment) && (obj as CodeExpressionFragment).Expression == Expression;
		}

		public override int GetHashCode() {
			return Expression.GetHashCode();
		}

		public override string ToString() {
			return "[EXPR:" + Expression + "]";
		}
		
		public void WriteCode(ITemplate tpl, FragmentCodePoint point, CodeBuilder cb) {
			cb.AppendLine(ParserUtils.RenderFunctionStringBuilderName + ".Append(" + Expression + ");");
		}
#endif
	}
}
