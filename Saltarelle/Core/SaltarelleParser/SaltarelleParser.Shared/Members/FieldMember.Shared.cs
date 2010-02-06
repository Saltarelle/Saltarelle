using System;

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
			if (string.IsNullOrEmpty(serverType)) throw Utils.ArgumentException("serverType");
			if (string.IsNullOrEmpty(clientType)) throw Utils.ArgumentException("clientType");
		
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
			return other != null && other.name == name && other.serverType == serverType && other.clientType == clientType;
		}

		public override int GetHashCode() {
			return name.GetHashCode() ^ serverType.GetHashCode() ^ clientType.GetHashCode();
		}

		public override string ToString() {
			return "Field " + (serverType == clientType ? serverType : ("(" + serverType + "," + clientType + ")")) + " " + name;
		}
		
		public void WriteCode(ITemplate tpl, MemberCodePoint point, CodeBuilder cb) {
			switch (point) {
				case MemberCodePoint.ServerDefinition:
					cb.AppendLine("private " + ServerType + " " + Name + ";").AppendLine();
					break;
				case MemberCodePoint.ClientDefinition:
					cb.AppendLine("private " + ClientType + " " + Name + ";").AppendLine();
					break;
				case MemberCodePoint.TransferConstructor:
					cb.AppendLine("this." + name + " = (" + clientType + ")" + ParserUtils.ConfigObjectName + "[\"" + name + "\"];");
					break;
				case MemberCodePoint.ConfigObjectInit:
					cb.AppendLine(ParserUtils.ConfigObjectName + "[\"" + name + "\"] = this." + name + ";");
					break;
			}
		}
#endif
	}
}
