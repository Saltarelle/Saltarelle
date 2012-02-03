using Saltarelle.Ioc;

namespace Saltarelle.Members {
	public class PropertyMember : IMember {
		private string name;
		private string serverType;
		private string clientType;
		private AccessModifier accessModifier;
		private string backingFieldName;
		private string backingFieldServerType;
		private string backingFieldClientType;
		private bool   hasGetter;
		private bool   hasSetter;
		private string valueChangedHookName;
		
		public PropertyMember(string name, string serverType, string clientType, AccessModifier accessModifier, string backingFieldName, string backingFieldServerType, string backingFieldClientType, bool hasGetter, bool hasSetter, string valueChangedHookName) {
			if (string.IsNullOrEmpty(name)) throw Utils.ArgumentException("name");
			if (string.IsNullOrEmpty(backingFieldName)) throw Utils.ArgumentException("backingFieldName");
			if (!hasGetter && !hasSetter) throw Utils.ArgumentException("Must have getter or setter.");
			if (string.IsNullOrEmpty(serverType) != string.IsNullOrEmpty(backingFieldServerType)) throw Utils.ArgumentException("serverType and backingFieldServerType must both be either null or non-null.");
			if (string.IsNullOrEmpty(clientType) != string.IsNullOrEmpty(backingFieldClientType)) throw Utils.ArgumentException("clientType and backingFieldClientType must both be either null or non-null.");
			if (!Utils.IsNull(valueChangedHookName) && !ParserUtils.IsValidUnqualifiedName(valueChangedHookName)) throw Utils.ArgumentException("valueChangedHookName");
			
			this.name = name;
			this.serverType = serverType;
			this.clientType = clientType;
			this.accessModifier = accessModifier;
			this.backingFieldName = backingFieldName;
			this.backingFieldServerType = backingFieldServerType;
			this.backingFieldClientType = backingFieldClientType;
			this.hasGetter = hasGetter;
			this.hasSetter = hasSetter;
			this.valueChangedHookName = valueChangedHookName;
		}
	
		public string Name {
			get { return name; }
		}

		public string[] Dependencies {
			get { return new string[0]; }
		}

		public void Instantiate(ITemplate tpl, IInstantiatedTemplateControl control) {
			throw ParserUtils.TemplateErrorException("Cannot dynamically instantiate templates with properties.");
		}
		
#if SERVER
		private void WriteDefinition(CodeBuilder cb, string type, string backingFieldType) {
			cb.Append(AccessModifierHelper.WriteDeclarator(accessModifier, type, name)).AppendLine(" {").Indent();
			if (hasGetter)
				cb.AppendLine("get { return " + (type != backingFieldType ? "(" + type + ")" : "") + backingFieldName + "; }");
			if (hasSetter)
				cb.AppendLine("set { " + backingFieldName + " = " + (type != backingFieldType ? "(" + backingFieldType + ")" : "") + "value; " + (!string.IsNullOrEmpty(valueChangedHookName) ? valueChangedHookName + "(); " : "") + "}");
			cb.Outdent().AppendLine("}").AppendLine();
		}

		public void WriteCode(ITemplate tpl, MemberCodePoint point, CodeBuilder cb) {
			switch (point) {
				case MemberCodePoint.ClientDefinition:
					if (!Utils.IsNull(clientType))
						WriteDefinition(cb, clientType, backingFieldClientType);
					break;
				case MemberCodePoint.ServerDefinition:
					if (!Utils.IsNull(serverType))
						WriteDefinition(cb, serverType, backingFieldServerType);
					break;
			}
		}

		public override bool Equals(object obj) {
			var other = (obj as PropertyMember);
			return !Utils.IsNull(other) && other.name == name && other.serverType == serverType && other.clientType == clientType && other.accessModifier == accessModifier && other.backingFieldName == backingFieldName && other.backingFieldServerType == backingFieldServerType && other.backingFieldClientType == backingFieldClientType &&  other.hasGetter == hasGetter && other.hasSetter == hasSetter && other.valueChangedHookName == valueChangedHookName;
		}

		public override int GetHashCode() {
			// I can't be bothered to do all members. Means a few more collitions but who cares?
			return name.GetHashCode() ^ (!Utils.IsNull(serverType) ? serverType.GetHashCode() : 0x80000) ^ (!Utils.IsNull(clientType) ? clientType.GetHashCode() << 1 : 0x4000) ^ backingFieldName.GetHashCode() ^ ((int)accessModifier << 16) ^ (hasGetter ? 2 : 0) ^ (hasSetter ? 1 : 0);
		}

		public override string ToString() {
			return "Property " + (serverType ?? clientType) + " " + name;
		}
#endif
	}
}
