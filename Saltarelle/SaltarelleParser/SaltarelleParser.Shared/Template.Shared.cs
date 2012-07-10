using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle.Members;
using Saltarelle.Ioc;
#if SERVER
using System.Linq;
using System.Reflection;
#endif

namespace Saltarelle {
	public interface ITemplate : IInstantiable {
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

		void AddServerUsingDirective(string nmspace);
		void AddClientUsingDirective(string nmspace);

		string GetUniqueId();
		
		#if SERVER
			string ServerInheritanceList { get; }
			string ClientInheritanceList { get; }
			
			void WriteServerCode(CodeBuilder cb);
			void WriteClientCode(CodeBuilder cb);
		#endif
	}

	public class Template : ITemplate, IInstantiable {
		public const string MainRenderFunctionName = "GetHtml";
	
		private readonly Dictionary<string, IMember> members = new Dictionary<string, Saltarelle.IMember>();
		
		private readonly Dictionary<string, object> serverUsings     = new Dictionary<string, object>();
		private readonly Dictionary<string, object> clientUsings     = new Dictionary<string, object>();
		private readonly Dictionary<string, object> serverInterfaces = new Dictionary<string, object>();
		private readonly Dictionary<string, object> clientInterfaces = new Dictionary<string, object>();
		private string serverInherits;
		private string clientInherits;
		private string className;
		private string nmspace;
		private bool enableClientCreate;
		
		private int nextUniqueId = 1;
		
		public Template() {
			members[MainRenderFunctionName] = new RenderFunctionMember(MainRenderFunctionName, "");
			#if SERVER
				serverUsings["System"] = null;
				serverUsings["System.Collections.Generic"] = null;
				serverUsings["System.Text"] = null;
				serverUsings["Saltarelle"] = null;
				serverUsings["Saltarelle.Ioc"] = null;

				clientUsings["System"] = null;
				clientUsings["System.Collections"] = null;
				clientUsings["System.Collections.Generic"] = null;
				clientUsings["System.Html"] = null;
				clientUsings["Saltarelle"] = null;
				clientUsings["Saltarelle.Ioc"] = null;
			#endif
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

		public void AddServerUsingDirective(string nmspace) {
			this.serverUsings[nmspace] = null;
		}

		public void AddClientUsingDirective(string nmspace) {
			this.clientUsings[nmspace] = null;
		}

		public string GetUniqueId() {
			return "_ctl" + Utils.ToStringInvariantInt(nextUniqueId++);
		}

		internal static List<IMember> TopologicalSort(Dictionary<string, IMember> members) {
			var workingSet = new Dictionary<string, List<string>>();
			var result = new List<IMember>();
			foreach (var m in members) {
				var l = new List<string>();
				l.AddRange(((IMember)m.Value).Dependencies);
				workingSet[m.Key] = l;
			}

			while (workingSet.Count > 0) {
				var currentSet = new List<string>();
				foreach (var kvp in workingSet) {
					if (kvp.Value.Count == 0)
						currentSet.Add(kvp.Key);
				}
				foreach (string name in currentSet) {
					result.Add(members[name]);
					workingSet.Remove(name);
				}
				foreach (var kvp in workingSet) {
					#if SERVER
						var l = kvp.Value.Where(delegate(string s) { return !currentSet.Contains(s); }).ToArray();
					#else
						var l = kvp.Value.Filter(delegate(string s) { return !currentSet.Contains(s); } );
					#endif
					kvp.Value.Clear();
					kvp.Value.AddRange(l);
				}
				if (currentSet.Count == 0)
					throw Utils.ArgumentException("There are either cycles in the member dependency graph, or a non-existent dependency.");
			}

			return result;
		}

		public IControl Instantiate(IContainer container) {
			List<IMember> orderedMembers = TopologicalSort(members);
			InstantiatedTemplateControl ctl = new InstantiatedTemplateControl(delegate(IInstantiatedTemplateControl x) { return MainRenderFunction.Render(this, x); });
			foreach (IMember m in orderedMembers) {
				try {
					m.Instantiate(this, ctl, container);
				}
#if SERVER
				catch (Exception ex) {
					if (ex is TargetInvocationException)
						ex = ex.InnerException;
					throw new Exception("Error instantiating member " + m.Name + ": " + ex.Message, ex);
				}
#else
				catch (Exception ex) {
					throw new Exception("Error instantiating member " + m.Name + ": " + ex.Message);
				}
#endif
			}
			return ctl;
		}

#if SERVER
        internal const string DoNotCallConstructorMessage = "Do not construct this type directly. Always use IContainer.Resolve*()";

		internal static void WriteServerConstructor(CodeBuilder cb, ITemplate tpl) {
			cb.AppendLine("[Obsolete(@\"" + DoNotCallConstructorMessage.Replace("\"", "\"\"") + "\")]")
			  .AppendLine("public " + tpl.ClassName + "() {")
			  .AppendLine("}");
		}

		internal static void WriteServerDependenciesAvailable(CodeBuilder cb, ITemplate tpl, IList<IMember> orderedMembers) {
			cb.AppendLine("public void DependenciesAvailable() {").Indent();
			foreach (var m in orderedMembers)
				m.WriteCode(tpl, MemberCodePoint.ServerConstructor, cb);
			cb.AppendLine("Constructed();").Outdent()
			  .AppendLine("}");
		}
		
		internal static void WriteClientConstructor(CodeBuilder cb, ITemplate tpl) {
			cb.AppendLine("[Obsolete(@\"" + DoNotCallConstructorMessage.Replace("\"", "\"\"") + "\")]")
			  .AppendLine("public " + tpl.ClassName + "(object config) {").Indent()
			  .AppendLine(ParserUtils.ConfigObjectName + " = (!Script.IsUndefined(config) ? JsDictionary.GetDictionary(config) : null);")
			  .Outdent().AppendLine("}");
		}

		internal static void WriteClientDependenciesAvailable(CodeBuilder cb, ITemplate tpl, IList<IMember> orderedMembers) {
			cb.AppendLine("public void DependenciesAvailable() {").Indent()
			  .AppendLine("if (!Utils.IsNull(" + ParserUtils.ConfigObjectName + ")) {").Indent()
			  .AppendLine("this.id = (string)" + ParserUtils.ConfigObjectName + "[\"id\"];");

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
		
		internal static void WriteGetConfig(CodeBuilder cb, ITemplate tpl, IList<IMember> orderedMembers) {
			cb.AppendLine("public object ConfigObject {").Indent()
			  .AppendLine("get {").Indent()
			  .AppendLine("Dictionary<string, object> " + ParserUtils.ConfigObjectName + " = new Dictionary<string, object>();")
			  .AppendLine(ParserUtils.ConfigObjectName + "[\"id\"] = id;");
			
			foreach (IMember m in orderedMembers)
				m.WriteCode(tpl, MemberCodePoint.ConfigObjectInit, cb);

			cb.AppendLine("return " + ParserUtils.ConfigObjectName + ";")
			  .Outdent().AppendLine("}")
			  .Outdent().AppendLine("}");
		}
		
		private static void WriteIdProperty(CodeBuilder cb, bool server, ITemplate tpl, IList<IMember> orderedMembers) {
			cb.AppendLine("private string id;")
			  .AppendLine("public string Id {").Indent()
			  .AppendLine("get { return id; }")
			  .AppendLine("set {").Indent();

			cb.AppendLine("foreach (var kvp in controls)").Indent()
			  .AppendLine("kvp.Value.Id = value + \"_\" + kvp.Key;").Outdent();

			foreach (var m in orderedMembers)
				m.WriteCode(tpl, server ? MemberCodePoint.ServerIdChanging : MemberCodePoint.ClientIdChanging, cb);
				
			if (!server) {
				cb.AppendLine("if (isAttached)").Indent()
				  .AppendLine("GetElement().ID = value;").Outdent();
			}

			cb.AppendLine("this.id = value;")
			  .Outdent().AppendLine("}")
			  .Outdent().AppendLine("}");
		}
		
		internal static void WriteServerIdProperty(CodeBuilder cb, ITemplate tpl, IList<IMember> orderedMembers) {
			WriteIdProperty(cb, true, tpl, orderedMembers);
		}

		internal static void WriteClientIdProperty(CodeBuilder cb, ITemplate tpl, IList<IMember> orderedMembers) {
			WriteIdProperty(cb, false, tpl, orderedMembers);
		}

		internal static void WriteAttach(CodeBuilder cb, ITemplate tpl, IList<IMember> orderedMembers) {
			cb.AppendLine("public void Attach() {").Indent()
			  .AppendLine("if (Script.IsNullOrEmpty(id) || isAttached) throw new Exception(\"Must set id before attach and can only attach once.\");");
			foreach (var m in orderedMembers)
				m.WriteCode(tpl, MemberCodePoint.Attach, cb);
			cb.AppendLine("AttachSelf();").Outdent()
			  .AppendLine("}");
		}

		internal static void WriteAttachSelf(CodeBuilder cb, ITemplate tpl, IList<IMember> orderedMembers) {
			cb.AppendLine("private void AttachSelf() {").Indent();
			foreach (var m in orderedMembers)
				m.WriteCode(tpl, MemberCodePoint.AttachSelf, cb);
			cb.AppendLine("this.isAttached = true;")
			  .AppendLine("Attached();").Outdent()
			  .AppendLine("}");
		}

		internal static string GetInheritanceList(string inherits, bool enableClientCreate, IList<string> interfaces) {
			var sb = new StringBuilder();
			if (!string.IsNullOrEmpty(inherits))
				sb.Append(inherits);
			sb.Append((Utils.IsStringBuilderEmpty(sb) ? "" : ", ") + "IControl, INotifyCreated" + (enableClientCreate ? ", IClientCreateControl" : ""));
			if (!Utils.IsNull(interfaces)) {
				for (int i = 0; i < interfaces.Count; i++) {
					sb.Append(", " + interfaces[i]);
				}
			}
			return sb.ToString();
		}
		
		public void WriteServerCode(CodeBuilder cb) {
			var orderedMembers = TopologicalSort(members);

			foreach (var us in serverUsings)
				cb.AppendLine("using " + us.Key + ";");
			cb.AppendLine();

			if (!string.IsNullOrEmpty(nmspace))
				cb.AppendFormat("namespace {0} {{", nmspace).Indent().AppendLine();
				
			cb.AppendFormat("public partial class " + className)
			  .Append(" : ")
			  .Append(ServerInheritanceList)
			  .Append(" {").AppendLine().Indent()
			  .AppendLine("partial void Constructed();").AppendLine()
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

			WriteServerConstructor(cb, this);
			cb.AppendLine();
			WriteServerDependenciesAvailable(cb, this, orderedMembers);
			
			cb.Outdent().AppendLine("}");
			if (!string.IsNullOrEmpty(nmspace))
				cb.Outdent().AppendLine("}");
		}

		public void WriteClientCode(CodeBuilder cb) {
			var orderedMembers = TopologicalSort(members);

			foreach (var us in clientUsings)
				cb.AppendLine("using " + us.Key + ";");
			cb.AppendLine();
			
			if (!string.IsNullOrEmpty(nmspace))
				cb.AppendFormat("namespace {0} {{", nmspace).Indent().AppendLine();
				
			cb.AppendFormat("public partial class " + className)
			  .Append(" : ")
			  .Append(ClientInheritanceList)
			  .Append(" {").AppendLine().Indent()
			  .AppendLine("partial void Constructed();")
			  .AppendLine("partial void Attached();").AppendLine()
			  .AppendLine("private Dictionary<string, IControl> controls = new Dictionary<string, IControl>();")
			  .AppendLine("private JsDictionary " + ParserUtils.ConfigObjectName + ";")
			  .AppendLine()
			  .AppendLine("private Position position;")
			  .AppendLine("public Position Position {").Indent()
			  .AppendLine("get { return isAttached ? PositionHelper.GetPosition(GetElement()) : position; }")
			  .AppendLine("set {").Indent()
			  .AppendLine("position = value;")
			  .AppendLine("if (isAttached)").Indent()
			  .AppendLine("PositionHelper.ApplyPosition(GetElement(), value);").Outdent()
			  .Outdent().AppendLine("}")
			  .Outdent().AppendLine("}").AppendLine()
			  .AppendLine("private bool isAttached = false;")
			  .AppendLine("public Element GetElement() { return isAttached ? Document.GetElementById(id) : null; }").AppendLine();

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
				  .AppendLine("public " + className + "() {}");
			}
			WriteClientConstructor(cb, this);
			cb.AppendLine();
			WriteClientDependenciesAvailable(cb, this, orderedMembers);
			
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
