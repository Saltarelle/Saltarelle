using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltarelle;
using Saltarelle.Ioc;

namespace DemoWeb.Plugins {
	internal class CopyrightMember : IMember {
		public string Name {
			get { return "CopyrightYear"; }
		}
		
		public void WriteCode(ITemplate tpl, MemberCodePoint point, CodeBuilder cb) {
			switch (point) {
				case MemberCodePoint.ServerDefinition:
				case MemberCodePoint.ClientDefinition:
					cb.AppendLine("private int copyrightYear;")
					  .AppendLine("public int CopyrightYear {").Indent()
					  .AppendLine("get { return copyrightYear; }")
					  .AppendLine("set { copyrightYear = value; }").Outdent()
					  .AppendLine("}").AppendLine();
					break;
				case MemberCodePoint.ServerConstructor:
					cb.AppendLine("copyrightYear = DateTime.Now.Year;");
					break;
				case MemberCodePoint.ClientConstructor:
					cb.AppendLine("copyrightYear = (new DateTime()).GetFullYear();");
					break;
				case MemberCodePoint.TransferConstructor:
					cb.AppendLine("copyrightYear = (int)" + ParserUtils.ConfigObjectName + "[\"copyrightYear\"];");
					break;
				case MemberCodePoint.ConfigObjectInit:
					cb.AppendLine(ParserUtils.ConfigObjectName + "[\"copyrightYear\"] = this.copyrightYear;");
					break;
			}
		}
		
		public IList<string> Dependencies {
			get { return new string[0]; }
		}
		
		public void Instantiate(ITemplate tpl, IInstantiatedTemplateControl control, IContainer container) {
		}
	}
}
