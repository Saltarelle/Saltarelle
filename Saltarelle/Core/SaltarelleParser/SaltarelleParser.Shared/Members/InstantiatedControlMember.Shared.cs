using System;
#if SERVER
using System.Text;
using AdditionalPropertiesDictionary = System.Collections.Generic.Dictionary<string, Saltarelle.TypedMarkupData>;
using AdditionalPropertiesEntry      = System.Collections.Generic.KeyValuePair<string, Saltarelle.TypedMarkupData>;
#else
using AdditionalPropertiesDictionary = System.Dictionary;
using AdditionalPropertiesEntry      = System.DictionaryEntry;
#endif

namespace Saltarelle.Members {
	internal class InstantiatedControlMember : IMember {
		private readonly string name;
		private readonly string typeName;
		private readonly bool customInstantiate;
		private readonly AdditionalPropertiesDictionary additionalProperties;
		private readonly bool hasInnerHtml;
		private readonly string[] dependencies;
		
		public string Name { get { return name; } }
		
		internal string TypeName { get { return typeName; } }
		internal bool CustomInstantiate { get { return customInstantiate; } }
		internal AdditionalPropertiesDictionary AdditionalProperties { get { return additionalProperties; } }
		internal bool HasInnerHtml { get { return hasInnerHtml; } }
		
		public InstantiatedControlMember(string name, string typeName, bool customInstantiate, AdditionalPropertiesDictionary additionalProperties, bool hasInnerHtml, IMember[] dependencies) {
			if (!ParserUtils.IsValidUnqualifiedName(name)) throw Utils.ArgumentException("id");
			if (string.IsNullOrEmpty(typeName)) throw Utils.ArgumentException("type");
			if (additionalProperties == null) throw Utils.ArgumentNullException("additionalProperties");
			if (dependencies == null) throw Utils.ArgumentNullException("dependencies");
			this.name = name;
			this.typeName = typeName;
			this.customInstantiate = customInstantiate;
			this.additionalProperties = additionalProperties;
			this.hasInnerHtml = hasInnerHtml;
			this.dependencies = new string[dependencies.Length];
			for (int i = 0; i < dependencies.Length; i++)
				this.dependencies[i] = dependencies[i].Name;
		}

		public void Instantiate(ITemplate tpl, IInstantiatedTemplateControl ctl) {
			if (CustomInstantiate)
				throw ParserUtils.TemplateErrorException("Dynamically instantiated templates cannot have customInstantiate controls.");
			IControl newCtl;
			#if SERVER
				newCtl = (IControl)Activator.CreateInstance(Utils.FindType(TypeName));
			#else
				Type tp = Utils.FindType(typeName);
				newCtl = (IControl)Type.CreateInstance(tp);
			#endif
			foreach (AdditionalPropertiesEntry prop in additionalProperties)
				Utils.SetPropertyValue(newCtl, prop.Key, ((TypedMarkupData)prop.Value).ValueRetriever());
			ctl.AddControl(name, newCtl);
		}

		public string[] Dependencies {
			get { return dependencies; }
		}

#if SERVER
		public override bool Equals(object obj) {
			var other = obj as InstantiatedControlMember;
			if (other == null || other.name != name || other.typeName != typeName || other.customInstantiate != customInstantiate || other.hasInnerHtml != hasInnerHtml || additionalProperties.Count != other.additionalProperties.Count || other.dependencies.Length != dependencies.Length)
				return false;
			foreach (var kvp in additionalProperties) {
				if (!other.additionalProperties.ContainsKey(kvp.Key) || other.additionalProperties[kvp.Key].InitializerString != kvp.Value.InitializerString)
					return false;
			}
			for (int i = 0; i < dependencies.Length; i++) {
				if (other.dependencies[i] != dependencies[i])
					return false;
			}
			return true;
		}
		
		public override int GetHashCode() {
			return name.GetHashCode() ^ typeName.GetHashCode() ^ (customInstantiate ? 0x8000 : 0) ^ (hasInnerHtml ? 1 : 0);
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
			sb.Append(" numInnerFragments=" + hasInnerHtml);
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
			if (customInstantiate) {
				cb.AppendFormat("private {0} _{1};", typeName, name).AppendLine()
				  .AppendFormat("private {0} {1} {{", typeName, name).AppendLine().Indent()
				  .AppendFormat("get {{ return _{0}; }}", name).AppendLine()
				  .AppendLine("set {").Indent()
				  .AppendFormat("controls[\"{0}\"] = _{0} = value;", name).AppendLine()
				  .AppendLine("if (!string.IsNullOrEmpty(id))").Indent()
				  .AppendFormat("((IControl)_{0}).Id = id + \"_{0}\";", name).AppendLine().Outdent()
				  .Outdent().AppendLine("}")
				  .Outdent().AppendLine("}");
			}
			else
				cb.AppendFormat("private readonly {0} {1};", typeName, name).AppendLine();
			cb.AppendLine();
		}
		
		private void WriteNonTransferConstructorCode(CodeBuilder cb) {
			if (!customInstantiate) {
				cb.AppendLine("this.controls[\"" + name + "\"] = this." + name + " = new " + typeName + "();");
				foreach (var kvp in additionalProperties)	
					cb.AppendLine("this." + name + "." + kvp.Key + " = " + kvp.Value.InitializerString + ";");
				cb.AppendLine();
			}
		}
		
		private void WriteTransferConstructorCode(CodeBuilder cb) {
			if (customInstantiate) {
				cb.AppendLine("Type __" + name + "Type" + " = Type.GetType((string)" + ParserUtils.ConfigObjectName + "[\"" + name + "\"]);")
				  .AppendLine("this.controls[\"" + name + "\"] = this." + name + " = Type.CreateInstance(__" + name + "Type, id + \"_" + name + "\");");
			}
			else {
				cb.AppendLine("this.controls[\"" + name + "\"] = this." + name + " = new " + typeName + "(id + \"_" + name + "\");");
			}
		}

		private void WriteConfigObjectInitCode(CodeBuilder cb) {
			if (customInstantiate) {
				cb.AppendLine(ParserUtils.ConfigObjectName + "[\"" + name + "\"] = this." + name + ".GetType().FullName;");
			}
		}
		
		private void WriteAttachCode(CodeBuilder cb) {
			if (customInstantiate)
				cb.AppendLine("if (this." + name + " == null) throw new Exception(\"Must instantiate the control 'CtlName' before attach.\");");
			cb.AppendLine("this." + name + ".Attach();");
		}
#endif
	}
}
