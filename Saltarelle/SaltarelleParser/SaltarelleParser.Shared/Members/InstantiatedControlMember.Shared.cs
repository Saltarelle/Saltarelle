using System;
using System.Collections.Generic;
using Saltarelle.Ioc;
using System.Text;

namespace Saltarelle.Members {
	internal class InstantiatedControlMember : IMember {
		private readonly string name;
		private readonly string typeName;
		private readonly bool customInstantiate;
		private readonly IDictionary<string, TypedMarkupData> additionalProperties;
		private readonly IList<string> dependencies;
		
		public string Name { get { return name; } }
		
		internal string TypeName { get { return typeName; } }
		internal bool CustomInstantiate { get { return customInstantiate; } }
		internal IDictionary<string, TypedMarkupData> AdditionalProperties { get { return additionalProperties; } }
		
		public InstantiatedControlMember(string name, string typeName, bool customInstantiate, IDictionary<string, TypedMarkupData> additionalProperties, IList<IMember> dependencies) {
			if (!ParserUtils.IsValidUnqualifiedName(name)) throw Utils.ArgumentException("id");
			if (string.IsNullOrEmpty(typeName)) throw Utils.ArgumentException("type");
			if (Utils.IsNull(additionalProperties)) throw Utils.ArgumentNullException("additionalProperties");
			if (Utils.IsNull(dependencies)) throw Utils.ArgumentNullException("dependencies");
			this.name = name;
			this.typeName = typeName;
			this.customInstantiate = customInstantiate;
			this.additionalProperties = additionalProperties;
			this.dependencies = new List<string>();
			for (int i = 0; i < dependencies.Count; i++)
				this.dependencies.Add(dependencies[i].Name);
		}

		public void Instantiate(ITemplate tpl, IInstantiatedTemplateControl ctl, IContainer container) {
			if (CustomInstantiate)
				throw ParserUtils.TemplateErrorException("Dynamically instantiated templates cannot have customInstantiate controls.");
			var newCtl = (IControl)container.CreateObjectByTypeName(typeName);

			foreach (var prop in additionalProperties)
				Utils.SetPropertyValue(newCtl, prop.Key, ((TypedMarkupData)prop.Value).ValueRetriever());
			ctl.AddControl(name, newCtl);
		}

		public IList<string> Dependencies {
			get { return dependencies; }
		}

#if SERVER
		public override bool Equals(object obj) {
			var other = obj as InstantiatedControlMember;
			if (Utils.IsNull(other) || other.name != name || other.typeName != typeName || other.customInstantiate != customInstantiate || additionalProperties.Count != other.additionalProperties.Count || other.dependencies.Count != dependencies.Count)
				return false;
			foreach (var kvp in additionalProperties) {
				if (!other.additionalProperties.ContainsKey(kvp.Key) || other.additionalProperties[kvp.Key].InitializerString != kvp.Value.InitializerString)
					return false;
			}
			for (int i = 0; i < dependencies.Count; i++) {
				if (other.dependencies[i] != dependencies[i])
					return false;
			}
			return true;
		}
		
		public override int GetHashCode() {
			return name.GetHashCode() ^ typeName.GetHashCode() ^ (customInstantiate ? 0x8000 : 0);
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append("Control name=" + name + " type=" + typeName + (customInstantiate ? " customInstantiate" : "") + " props=(");
			bool first = true;
			foreach (var kvp in additionalProperties) {
				if (!first)
					sb.Append(", ");
				sb.Append(kvp.Key + ":" + kvp.Value);
				first = false;
			}
			return sb.ToString();
		}
		
		public void WriteCode(ITemplate tpl, MemberCodePoint point, CodeBuilder cb) {
			switch (point) {
				case MemberCodePoint.ServerDefinition:
				case MemberCodePoint.ClientDefinition:
					WriteDefinition(cb);
					break;
				case MemberCodePoint.ServerConstructor:
				case MemberCodePoint.ClientConstructor:
					WriteNonTransferConstructorCode(cb);
					break;
				case MemberCodePoint.TransferConstructor:
					WriteTransferConstructorCode(cb);
					break;
				case MemberCodePoint.Attach:
					WriteAttachCode(cb);
					break;
				case MemberCodePoint.ConfigObjectInit:
					WriteConfigObjectInitCode(cb);
					break;
			}
		}

		private void WriteDefinition(CodeBuilder cb) {
			cb.AppendFormat("private {0} {1} {{", typeName, name).AppendLine().Indent()
			  .AppendFormat("get {{ return ({0})controls[\"{1}\"]; }}", typeName, name).AppendLine();

			if (customInstantiate) {
				cb.AppendLine("set {").Indent()
				  .AppendFormat("controls[\"{0}\"] = value;", name).AppendLine()
				  .AppendLine("if (!string.IsNullOrEmpty(id))").Indent()
				  .AppendFormat("((IControl)controls[\"{0}\"]).Id = id + \"_{0}\";", name).Outdent().AppendLine()
				  .Outdent().AppendLine("}");
			}
			
			cb.Outdent().AppendLine("}").AppendLine();
		}
		
		private void WriteNonTransferConstructorCode(CodeBuilder cb) {
			if (!customInstantiate) {
				cb.AppendLine("{");
				cb.AppendLine(typeName + " c = (" + typeName + ")Container.CreateObject(typeof(" + typeName + "));");
				foreach (var kvp in additionalProperties)	
					cb.AppendLine("c." + kvp.Key + " = " + kvp.Value.InitializerString + ";");
				cb.AppendLine("this.controls[\"" + name + "\"] = c;");
				cb.AppendLine("}");
			}
		}
		
		private void WriteTransferConstructorCode(CodeBuilder cb) {
			if (customInstantiate) {
				cb.AppendLine("this.controls[\"" + name + "\"] = (" + TypeName + ")Container.CreateObjectByTypeNameWithConstructorArg((string)" + ParserUtils.ConfigObjectName + "[\"" + name + "$type\"], " + ParserUtils.ConfigObjectName + "[\"" + name + "\"]);");
			}
			else {
				cb.AppendLine("this.controls[\"" + name + "\"] = (" + TypeName + ")Container.CreateObjectWithConstructorArg(typeof(" + TypeName + "), " + ParserUtils.ConfigObjectName + "[\"" + name + "\"]);");
			}
		}

		private void WriteConfigObjectInitCode(CodeBuilder cb) {
			if (customInstantiate)
				cb.AppendLine(ParserUtils.ConfigObjectName + "[\"" + name + "$type\"] = this." + name + ".GetType().FullName;");
			cb.AppendLine(ParserUtils.ConfigObjectName + "[\"" + name + "\"] = this." + name + ".ConfigObject;");
		}
		
		private void WriteAttachCode(CodeBuilder cb) {
			if (customInstantiate)
				cb.AppendLine("if (Utils.IsNull(this." + name + ")) throw new Exception(\"Must instantiate the control 'CtlName' before attach.\");");
			cb.AppendLine("this." + name + ".Attach();");
		}
#endif
	}
}
