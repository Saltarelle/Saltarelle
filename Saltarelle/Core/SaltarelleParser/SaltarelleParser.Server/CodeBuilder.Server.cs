using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltarelle {
	public class CodeBuilder {
		private Dictionary<int, string> indents = new Dictionary<int, string>() { { 0, "" } };
		private int indentLevel = 0;
		private StringBuilder sb;
		private bool atLineStart;
		
		public CodeBuilder() : this(new StringBuilder()) {
		}
		
		public CodeBuilder(StringBuilder sb) {
			this.sb = sb;
			atLineStart = true;
		}
		
		internal int IndentLevel { get { return indentLevel; } }

		public CodeBuilder Indent() {
			indentLevel++;
			if (!indents.ContainsKey(indentLevel))
				indents.Add(indentLevel, new string('\t', indentLevel));
			return this;
		}
		
		public CodeBuilder Outdent() {
			indentLevel--;
			return this;
		}

		public CodeBuilder Append(string value) {
			if (atLineStart)
				sb.Append(indents[indentLevel]);
			atLineStart = false;
			sb.Append(value);
			return this;
		}
		
		public CodeBuilder AppendFormat(string format, params object[] args) {
			Append(string.Format(format, args));
			return this;
		}

		public CodeBuilder AppendLine(string value) {
			Append(value);
			AppendLine();
			return this;
		}
		
		public CodeBuilder AppendLine() {
			sb.AppendLine();
			atLineStart = true;
			return this;
		}
		
		public CodeBuilder PreventIndent() {
			atLineStart = false;
			return this;
		}

		public override string ToString() {
			return sb.ToString();
		}
	}
}
