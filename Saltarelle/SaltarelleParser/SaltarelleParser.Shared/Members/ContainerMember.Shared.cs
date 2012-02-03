using System;
using Saltarelle.Ioc;

namespace Saltarelle.Members {
	public class ContainerMember : IMember {
        public static string MemberName = "__Container";

        public string Name { get { return MemberName; } }

		public string[] Dependencies {
			get { return new string[0]; }
		}

		public void Instantiate(ITemplate tpl, IInstantiatedTemplateControl ctl, IContainer container) {
			throw ParserUtils.TemplateErrorException("Dynamically instantiated templates cannot reference the container fields.");
		}

#if SERVER
		public override bool Equals(object obj) {
			return obj is ContainerMember;
		}

		public override int GetHashCode() {
			return 43423405;
		}

		public override string ToString() {
			return "Container";
		}
		
		public void WriteCode(ITemplate tpl, MemberCodePoint point, CodeBuilder cb) {
			switch (point) {
				case MemberCodePoint.ServerDefinition:
				case MemberCodePoint.ClientDefinition:
                    cb.AppendLine("private IContainer _" + Name + ";");
                    if (point == MemberCodePoint.ServerDefinition)
                        cb.AppendLine("[Saltarelle.Ioc.ClientInject]");

                    cb.AppendLine("public IContainer " + Name + " {").Indent()
                      .AppendLine("get { return _" + Name + "; }")
                      .AppendLine("set { _" + Name + " = value; }").Outdent()
                      .AppendLine("}")
                      .AppendLine();
                    break;
			}
		}
#endif
	}
}
