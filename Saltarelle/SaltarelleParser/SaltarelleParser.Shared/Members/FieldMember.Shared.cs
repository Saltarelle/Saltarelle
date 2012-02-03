using System;
using Saltarelle.Ioc;

namespace Saltarelle.Members {
	public class FieldMember : IMember {
		private readonly string name;
		private readonly string serverType;
		private readonly string clientType;
	
		public string Name { get { return name; } }
		public string ServerType { get { return serverType; } }
		public string ClientType { get { return clientType; } }
		
		public FieldMember(string name, string serverType, string clientType) {
			if (!ParserUtils.IsValidUnqualifiedName(name)) throw Utils.ArgumentException("name");
		
			this.name       = name;
			this.serverType = serverType;
			this.clientType = clientType;
		}

		public string[] Dependencies {
			get { return new string[0]; }
		}

		public void Instantiate(ITemplate tpl, IInstantiatedTemplateControl ctl) {
			throw ParserUtils.TemplateErrorException("Dynamically instantiated templates cannot have fields.");
		}

#if SERVER
		public override bool Equals(object obj) {
			var other = obj as FieldMember;
			return !Utils.IsNull(other) && other.name == name && other.serverType == serverType && other.clientType == clientType;
		}

		public override int GetHashCode() {
			return name.GetHashCode() ^ (!Utils.IsNull(serverType) ? serverType.GetHashCode() : 0) ^ (!Utils.IsNull(clientType) ? clientType.GetHashCode() : 0);
		}

		public override string ToString() {
			return "Field " + (serverType == clientType ? serverType : ("(" + serverType + "," + clientType + ")")) + " " + name;
		}
		
		public void WriteCode(ITemplate tpl, MemberCodePoint point, CodeBuilder cb) {
			switch (point) {
				case MemberCodePoint.ServerDefinition:
					if (!Utils.IsNull(serverType))
						cb.AppendLine("private " + serverType + " " + Name + ";").AppendLine();
					break;
				case MemberCodePoint.ClientDefinition:
					if (!Utils.IsNull(clientType))
						cb.AppendLine("private " + ClientType + " " + Name + ";").AppendLine();
					break;
				case MemberCodePoint.TransferConstructor:
					if (!Utils.IsNull(serverType) && !Utils.IsNull(clientType))
						cb.AppendLine("this." + name + " = (" + clientType + ")" + ParserUtils.ConfigObjectName + "[\"" + name + "\"];");
					break;
				case MemberCodePoint.ConfigObjectInit:
					if (!Utils.IsNull(serverType) && !Utils.IsNull(clientType))
						cb.AppendLine(ParserUtils.ConfigObjectName + "[\"" + name + "\"] = this." + name + ";");
					break;
			}
		}
#endif
	}
}
