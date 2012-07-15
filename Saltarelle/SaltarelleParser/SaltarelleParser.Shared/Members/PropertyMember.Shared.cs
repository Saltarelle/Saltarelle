using System.Collections.Generic;
using Saltarelle.Ioc;
#pragma warning disable 414

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
		private bool   clientInject;
		
		public PropertyMember(string name, string serverType, string clientType, AccessModifier accessModifier, string backingFieldName, string backingFieldServerType, string backingFieldClientType, bool hasGetter, bool hasSetter, string valueChangedHookName, bool clientInject) {
			if (string.IsNullOrEmpty(name)) throw Utils.ArgumentException("name");
			if (string.IsNullOrEmpty(backingFieldName)) throw Utils.ArgumentException("backingFieldName");
			if (!hasGetter && !hasSetter) throw Utils.ArgumentException("Must have getter or setter.");
			if (string.IsNullOrEmpty(serverType) != string.IsNullOrEmpty(backingFieldServerType)) throw Utils.ArgumentException("serverType and backingFieldServerType must both be either null or non-null.");
			if (string.IsNullOrEmpty(clientType) != string.IsNullOrEmpty(backingFieldClientType)) throw Utils.ArgumentException("clientType and backingFieldClientType must both be either null or non-null.");
			if (valueChangedHookName != null && !ParserUtils.IsValidUnqualifiedName(valueChangedHookName)) throw Utils.ArgumentException("valueChangedHookName");
			
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
			this.clientInject = clientInject;
		}
	
		public string Name {
			get { return name; }
		}

		public IList<string> Dependencies {
			get { return new string[0]; }
		}

		public void Instantiate(ITemplate tpl, IInstantiatedTemplateControl control, IContainer container) {
		}
		
#if SERVER
		private void WriteDefinition(CodeBuilder cb, string type, string backingFieldType, bool isServer) {
			cb.AppendLine(string.Format("private {0} {1};", isServer ? backingFieldServerType : backingFieldClientType, backingFieldName));
			if (isServer && clientInject)
				cb.AppendLine("[ClientInject]");
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
					if (clientType != null)
						WriteDefinition(cb, clientType, backingFieldClientType, false);
					break;
				case MemberCodePoint.ServerDefinition:
					if (serverType != null)
						WriteDefinition(cb, serverType, backingFieldServerType, true);
					break;
			}
		}

		public override bool Equals(object obj) {
			var other = (obj as PropertyMember);
			return other != null && other.name == name && other.serverType == serverType && other.clientType == clientType && other.accessModifier == accessModifier && other.backingFieldName == backingFieldName && other.backingFieldServerType == backingFieldServerType && other.backingFieldClientType == backingFieldClientType &&  other.hasGetter == hasGetter && other.hasSetter == hasSetter && other.valueChangedHookName == valueChangedHookName;
		}

		public override int GetHashCode() {
			// I can't be bothered to do all members. Means a few more collitions but who cares?
			return name.GetHashCode() ^ (serverType != null ? serverType.GetHashCode() : 0x80000) ^ (clientType != null ? clientType.GetHashCode() << 1 : 0x4000) ^ backingFieldName.GetHashCode() ^ ((int)accessModifier << 16) ^ (hasGetter ? 2 : 0) ^ (hasSetter ? 1 : 0);
		}

		public override string ToString() {
			return "Property " + (serverType ?? clientType) + " " + name;
		}
#endif
	}
}
