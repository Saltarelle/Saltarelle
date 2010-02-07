using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltarelle;

namespace DemoWeb.Plugins {
	internal class CopyrightFragment : IFragment {
		private readonly string text;

		public CopyrightFragment(string text) {
			this.text = text;
		}

		public IFragment TryMergeWithNext(IFragment nextFragment) {
			return null;
		}

		public void WriteCode(ITemplate tpl, FragmentCodePoint point, CodeBuilder cb) {
			cb.AppendLine(ParserUtils.RenderFunctionStringBuilderName + ".Append(\"Copyright &copy; \" + Utils.ToStringInvariantInt(this.copyrightYear) + @\" " + text.Replace("\"", "\"\"") + "\");");
		}
		
		public void Render(ITemplate tpl, IInstantiatedTemplateControl ctl, StringBuilder sb) {
			int year = DateTime.Now.Year;
			sb.Append("Copyright &copy; " + Utils.ToStringInvariantInt(year) + " " + text);
		}
	}
}
