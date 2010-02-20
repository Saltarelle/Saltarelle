﻿using System;
using Saltarelle.Members;
#if CLIENT
using MemberList             = System.ArrayList;
using HashMapDictionary      = System.Dictionary;
using StringList             = System.ArrayList;
using MemberDictionary       = System.Dictionary;
using MemberEntry            = System.DictionaryEntry;
using TopSortWorkingSet      = System.Dictionary;
using TopSortWorkingSetEntry = System.DictionaryEntry;
#else
using MemberList             = System.Collections.Generic.List<Saltarelle.IMember>;
using HashMapDictionary      = System.Collections.Generic.Dictionary<string, object>;
using StringList             = System.Collections.Generic.List<string>;
using MemberDictionary       = System.Collections.Generic.Dictionary<string, Saltarelle.IMember>;
using MemberEntry            = System.Collections.Generic.KeyValuePair<string, Saltarelle.IMember>;
using TopSortWorkingSet      = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>;
using TopSortWorkingSetEntry = System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.List<string>>;
using System.Linq;
using System.Text;
#endif
namespace Saltarelle {
	public interface ITemplate {
		IRenderFunction MainRenderFunction { get; }

		string ServerInherits { get; set; }
		string ClientInherits { get; set; }
		
		bool EnableClientCreate { get; set; }
		
		string Nmspace {get; set; }
		string ClassName { get; set; }
		
		bool HasMember(string name);
		IMember GetMember(string name);
		void AddMember(IMember member);

		bool ImplementsServerInterface(string interfaceName);
		void AddServerInterface(string interfaceName);
		bool ImplementsClientInterface(string interfaceName);		
		void AddClientInterface(string interfaceName);
		
		string GetUniqueId();
		
		InstantiatedTemplateControl Instantiate();

		#if SERVER
			string ServerInheritanceList { get; }
			string ClientInheritanceList { get; }
			
			void WriteServerCode(CodeBuilder cb);
			void WriteClientCode(CodeBuilder cb);
		#endif
	}

	public class Template : ITemplate {
		public const string MainRenderFunctionName = "GetHtml";
	
		private readonly MemberDictionary members = new MemberDictionary();

		private readonly HashMapDictionary serverInterfaces = new HashMapDictionary();
		private readonly HashMapDictionary clientInterfaces = new HashMapDictionary();
		private string serverInherits;
		private string clientInherits;
		private string className;
		private string nmspace;
		private bool enableClientCreate;
		
		private int nextUniqueId = 1;
		
		public Template() {
			members[MainRenderFunctionName] = new RenderFunctionMember(MainRenderFunctionName, "");
		}
		
		public bool EnableClientCreate { get { return enableClientCreate; } set { enableClientCreate = value; } }
		
		public string ClassName {
			get { return className; }
			set {
				if (!ParserUtils.IsValidUnqualifiedName(value)) throw Utils.ArgumentException("value");
				className = value;
			}
		}
		
		public string Nmspace {
			get { return nmspace; }
			set {
				if (!string.IsNullOrEmpty(value) && !ParserUtils.IsValidQualifiedName(value)) throw Utils.ArgumentException("value");
				nmspace = value;
			}
		}
		
		public IRenderFunction MainRenderFunction {
			get { return (RenderFunctionMember)members[MainRenderFunctionName]; }
		}

		public string ServerInherits {
			get { return serverInherits; }
			set {
				if (string.IsNullOrEmpty(value)) throw Utils.ArgumentException("value");
				serverInherits = value;
			}
		}

		public string ClientInherits {
			get { return clientInherits; }
			set {
				if (string.IsNullOrEmpty(value)) throw Utils.ArgumentException("value");
				clientInherits = value;
			}
		}
		
		public bool HasMember(string name) {
			return members.ContainsKey(name);
		}

		public IMember GetMember(string name) {
			return members.ContainsKey(name) ? (IMember)members[name] : null;
		}
		
		public void AddMember(IMember member) {
			if (members.ContainsKey(member.Name)) throw Utils.ArgumentException("name");
			members[member.Name] = member;
		}

		public bool ImplementsServerInterface(string interfaceName) {
			return serverInterfaces.ContainsKey(interfaceName);
		}
		
		public void AddServerInterface(string interfaceName) {
			if (string.IsNullOrEmpty(interfaceName) || serverInterfaces.ContainsKey(interfaceName)) throw Utils.ArgumentException("interfaceName");
			serverInterfaces[interfaceName] = null;
		}
		
		public bool ImplementsClientInterface(string interfaceName) {
			return clientInterfaces.ContainsKey(interfaceName);
		}
		
		public void AddClientInterface(string interfaceName) {
			if (string.IsNullOrEmpty(interfaceName) || clientInterfaces.ContainsKey(interfaceName)) throw Utils.ArgumentException("interfaceName");
			clientInterfaces[interfaceName] = null;
		}
		
		public string GetUniqueId() {
			return "_ctl" + Utils.ToStringInvariantInt(nextUniqueId++);
		}

		internal static MemberList TopologicalSort(MemberDictionary members) {
			TopSortWorkingSet workingSet = new TopSortWorkingSet();
			MemberList result = new MemberList();
			foreach (MemberEntry m in members) {
				StringList l = new StringList();
				l.AddRange(((IMember)m.Value).Dependencies);
				workingSet[m.Key] = l;
			}

			while (workingSet.Count > 0) {
				StringList currentSet = new StringList();
				foreach (TopSortWorkingSetEntry kvp in workingSet) {
					if (Utils.ArrayLength((StringList)kvp.Value) == 0)
						currentSet.Add(kvp.Key);
				}
				foreach (string name in currentSet) {
					result.Add(members[name]);
					workingSet.Remove(name);
				}
				foreach (TopSortWorkingSetEntry kvp in workingSet) {
					#if SERVER
						string[] l = kvp.Value.Where(delegate(string s) { return !currentSet.Contains(s); }).ToArray();
					#else
						string[] l = (string[])((StringList)kvp.Value).Filter(delegate(object s) { return !currentSet.Contains((string)s); } );
					#endif
					((StringList)kvp.Value).Clear();
					((StringList)kvp.Value).AddRange(l);
				}
				if (Utils.ArrayLength(currentSet) == 0)
					throw Utils.ArgumentException("There are either cycles in the member dependency graph, or a non-existent dependency.");
			}

			return result;
		}

		public InstantiatedTemplateControl Instantiate() {
			MemberList orderedMembers = TopologicalSort(members);
			InstantiatedTemplateControl ctl = new InstantiatedTemplateControl(delegate(IInstantiatedTemplateControl x) { return MainRenderFunction.Render(this, x); });
			foreach (IMember m in orderedMembers)
				m.Instantiate(this, ctl);
			return ctl;
		}

#if SERVER
		internal static void WriteServerConstructor(CodeBuilder cb, ITemplate tpl, MemberList orderedMembers) {
			cb.AppendLine("public " + tpl.ClassName + "() {").Indent()
			  .AppendLine("GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());");
			foreach (var m in orderedMembers)
				m.WriteCode(tpl, MemberCodePoint.ServerConstructor, cb);
			cb.AppendLine("Constructed();").Outdent()
			  .AppendLine("}");
		}
		
		internal static void WriteClientConstructor(CodeBuilder cb, ITemplate tpl, MemberList orderedMembers) {
			cb.AppendLine("public " + tpl.ClassName + "(string id) {").Indent()
			  .AppendLine("if (!Script.IsUndefined(id)) {").Indent()
			  .AppendLine("this.id = id;")
			  .AppendLine("Dictionary " + ParserUtils.ConfigObjectName + " = (Dictionary)Utils.EvalJson((string)JQueryProxy.jQuery(\"#\" + id).attr(\"" + ParserUtils.ConfigObjectName + "\"));");

			foreach (var m in orderedMembers)
				m.WriteCode(tpl, MemberCodePoint.TransferConstructor, cb);

			cb.AppendLine("Constructed();")
			  .AppendLine("AttachSelf();")
			  .Outdent().AppendLine("}")
			  .AppendLine("else {").Indent();

			if (tpl.EnableClientCreate) {
				cb.AppendLine("this.position = PositionHelper.NotPositioned;");
				foreach (var m in orderedMembers)
					m.WriteCode(tpl, MemberCodePoint.ClientConstructor, cb);
				cb.AppendLine("Constructed();");
			}
			else {
				cb.AppendLine("throw new Exception(\"This control must be created server-side\");");
			}

			cb.Outdent().AppendLine("}")
			  .Outdent().AppendLine("}");
		}
		
		internal static void WriteGetConfig(CodeBuilder cb, ITemplate tpl, MemberList orderedMembers) {
			cb.AppendLine("private Dictionary<string, object> GetConfig() {").Indent()
			  .AppendLine("Dictionary<string, object> " + ParserUtils.ConfigObjectName + " = new Dictionary<string, object>();");
			
			foreach (IMember m in orderedMembers)
				m.WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb);

			cb.AppendLine("return " + ParserUtils.ConfigObjectName + ";")
			  .Outdent().AppendLine("}");
		}
		
		private static void WriteIdProperty(CodeBuilder cb, bool server, ITemplate tpl, MemberList orderedMembers) {
			cb.AppendLine("private string id;")
			  .AppendLine("public string Id {").Indent()
			  .AppendLine("get { return id; }")
			  .AppendLine("set {").Indent()
			  .AppendLine("this.id = value;");
			  
			cb.AppendLine("foreach (" + (server ? "KeyValuePair<string, IControl>" : "DictionaryEntry") + " kvp in controls)").Indent()
			  .AppendLine((server ? "kvp.Value" : "((IControl)kvp.Value)") + ".Id = value + \"_\" + kvp.Key;").Outdent();

			foreach (var m in orderedMembers)
				m.WriteCode(tpl, server ? MemberCodePoint.ServerIdChanged : MemberCodePoint.ClientIdChanged, cb);

			cb.Outdent().AppendLine("}")
			  .Outdent().AppendLine("}");
		}
		
		internal static void WriteServerIdProperty(CodeBuilder cb, ITemplate tpl, MemberList orderedMembers) {
			WriteIdProperty(cb, true, tpl, orderedMembers);
		}

		internal static void WriteClientIdProperty(CodeBuilder cb, ITemplate tpl, MemberList orderedMembers) {
			WriteIdProperty(cb, false, tpl, orderedMembers);
		}

		internal static void WriteAttach(CodeBuilder cb, ITemplate tpl, MemberList orderedMembers) {
			cb.AppendLine("public void Attach() {").Indent()
			  .AppendLine("if (Script.IsNullOrEmpty(id) || element != null) throw new Exception(\"Must set id before attach and can only attach once.\");");
			foreach (var m in orderedMembers)
				m.WriteCode(tpl, MemberCodePoint.Attach, cb);
			cb.AppendLine("AttachSelf();").Outdent()
			  .AppendLine("}");
		}

		internal static void WriteAttachSelf(CodeBuilder cb, ITemplate tpl, MemberList orderedMembers) {
			cb.AppendLine("private void AttachSelf() {").Indent()
			  .AppendLine("this.element = JQueryProxy.jQuery(\"#\" + id);");
			foreach (var m in orderedMembers)
				m.WriteCode(tpl, MemberCodePoint.AttachSelf, cb);
			cb.AppendLine("Attached();").Outdent()
			  .AppendLine("}");
		}

		internal static string GetInheritanceList(string inherits, bool enableClientCreate, StringList interfaces) {
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrEmpty(inherits))
				sb.Append(inherits);
			sb.Append((Utils.IsStringBuilderEmpty(sb) ? "" : ", ") + "IControl" + (enableClientCreate ? ", IClientCreateControl" : ""));
			if (interfaces != null) {
				for (int i = 0; i < interfaces.Count; i++) {
					sb.Append(", " + interfaces[i]);
				}
			}
			return sb.ToString();
		}
		
		public void WriteServerCode(CodeBuilder cb) {
			MemberList orderedMembers = TopologicalSort(members);
		
			cb.AppendLine("using System;")
			  .AppendLine("using System.Collections.Generic;")
			  .AppendLine("using System.Text;")
			  .AppendLine("using Saltarelle;").AppendLine();
			
			if (!string.IsNullOrEmpty(nmspace))
				cb.AppendFormat("namespace {0} {{", nmspace).Indent().AppendLine();
				
			cb.AppendFormat("public partial class " + className)
			  .Append(" : ")
			  .Append(ServerInheritanceList)
			  .Append(" {").AppendLine().Indent()
			  .AppendLine("private Dictionary<string, IControl> controls = new Dictionary<string, IControl>();").AppendLine()
			  .AppendLine("private Position position = PositionHelper.NotPositioned;")
			  .AppendLine("public Position Position { get { return position; } set { position = value; } }").AppendLine();

			WriteIdProperty(cb, true, this, orderedMembers);
			cb.AppendLine();
			WriteGetConfig(cb, this, orderedMembers);
			cb.AppendLine();
			
			foreach (var m in orderedMembers)
				m.WriteCode(this, MemberCodePoint.ServerDefinition, cb);

			cb.AppendLine("public string Html {").Indent()
			  .AppendLine("get {").Indent()
			  .AppendLine("if (string.IsNullOrEmpty(id))").Indent()
			  .AppendLine("throw new InvalidOperationException(\"Must assign Id before rendering.\");").Outdent()
			  .AppendLine("return " + MainRenderFunctionName + "();").Outdent()
			  .AppendLine("}").Outdent()
			  .AppendLine("}").AppendLine();

			WriteServerConstructor(cb, this, orderedMembers);
			
			cb.Outdent().AppendLine("}");
			if (!string.IsNullOrEmpty(nmspace))
				cb.Outdent().AppendLine("}");
		}

		public void WriteClientCode(CodeBuilder cb) {
			MemberList orderedMembers = TopologicalSort(members);

			cb.AppendLine("using System;")
			  .AppendLine("using Saltarelle;").AppendLine();
			
			if (!string.IsNullOrEmpty(nmspace))
				cb.AppendFormat("namespace {0} {{", nmspace).Indent().AppendLine();
				
			cb.AppendFormat("public partial class " + className)
			  .Append(" : ")
			  .Append(ClientInheritanceList)
			  .Append(" {").AppendLine().Indent()
			  .AppendLine("private Dictionary controls = new Dictionary();").AppendLine()
			  .AppendLine("private Position position;")
			  .AppendLine("public Position Position {").Indent()
			  .AppendLine("get { return element != null ? PositionHelper.GetPosition(element) : position; }")
			  .AppendLine("set {").Indent()
			  .AppendLine("position = value;")
			  .AppendLine("if (element != null)").Indent()
			  .AppendLine("PositionHelper.ApplyPosition(element, value);").Outdent()
			  .Outdent().AppendLine("}")
			  .Outdent().AppendLine("}").AppendLine()
			  .AppendLine("private jQuery element;")
			  .AppendLine("public jQuery Element { get { return element; } }").AppendLine();

			WriteIdProperty(cb, false, this, orderedMembers);
			cb.AppendLine();
			
			foreach (var m in orderedMembers)
				m.WriteCode(this, MemberCodePoint.ClientDefinition, cb);

			WriteAttachSelf(cb, this, orderedMembers);
			cb.AppendLine();

			if (enableClientCreate) {
				WriteAttach(cb, this, orderedMembers);

				cb.AppendLine()
				  .AppendLine("public string Html {").Indent()
				  .AppendLine("get {").Indent()
				  .AppendLine("if (string.IsNullOrEmpty(id))").Indent()
				  .AppendLine("throw new InvalidOperationException(\"Must assign Id before rendering.\");").Outdent()
				  .AppendLine("return " + MainRenderFunctionName + "();").Outdent()
				  .AppendLine("}").Outdent()
				  .AppendLine("}").AppendLine()
				  .AppendLine("[AlternateSignature]")
				  .AppendLine("public extern " + className + "();");
			}
			WriteClientConstructor(cb, this, orderedMembers);
			
			cb.Outdent().AppendLine("}");
			if (!string.IsNullOrEmpty(nmspace))
				cb.Outdent().AppendLine("}");
		}
		
		public string ServerInheritanceList {
			get { return GetInheritanceList(serverInherits, enableClientCreate, serverInterfaces.Keys.ToList()); }
		}

		public string ClientInheritanceList {
			get { return GetInheritanceList(clientInherits, enableClientCreate, clientInterfaces.Keys.ToList()); }
		}
#endif
	}
}
