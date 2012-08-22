using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle.Ioc;

namespace Saltarelle.Members {
	internal class RenderFunctionMember : IRenderFunction, IMember {
		private readonly string name;
		private readonly string parameters;
		private List<IFragment> fragments;
		private List<string> dependencies;
		
		public string Name { get { return name; } }
		public string Parameters { get { return parameters; } }
		public IList<IFragment> Fragments { get { return fragments; } }
		
		public RenderFunctionMember(string name, string parameters) {
			if (!ParserUtils.IsValidUnqualifiedName(name)) throw Utils.ArgumentException("name");
			if (parameters == null) throw Utils.ArgumentNullException("parameters");
			this.name         = name;
			this.parameters   = parameters;
			this.fragments    = new List<IFragment>();
			this.dependencies = new List<string>();
		}

		public bool IsEmpty {
			get { return fragments.Count == 0; }
		}
		
		public void AddDependency(IMember dependency) {
			dependencies.Add(dependency.Name);
		}
		
		public void AddFragment(IFragment fragment) {
			fragments.Add(fragment);
		}
		
		public IList<string> Dependencies {
			get {
				return dependencies;
			}
		}

		public void Instantiate(ITemplate tpl, IInstantiatedTemplateControl ctl, IContainer container) {
		}
		
		public string Render(ITemplate tpl, IInstantiatedTemplateControl ctl) {
			var sb = new StringBuilder();
			foreach (IFragment f in ParserUtils.MergeFragments(fragments))
				f.Render(tpl, ctl, sb);
			return sb.ToString();
		}

#if SERVER
		public override bool Equals(object obj) {
			RenderFunctionMember other = obj as RenderFunctionMember;
			if (other == null || other.name != name || other.parameters != parameters || other.fragments.Count != fragments.Count || other.dependencies.Count != dependencies.Count)
				return false;
			for (int i = 0; i < fragments.Count; i++) {
				if (!other.fragments[i].Equals(fragments[i]))
					return false;
			}
			for (int i = 0; i < dependencies.Count; i++) {
				if (other.dependencies[i] != dependencies[i])
					return false;
			}
			return true;
		}

		public override int GetHashCode() {
			return name.GetHashCode() ^ parameters.GetHashCode();
		}

		public void WriteCode(ITemplate tpl, MemberCodePoint point, CodeBuilder cb) {
			switch (point) {
				case MemberCodePoint.ServerDefinition:
					WriteDefinition(tpl, true, cb);
					break;
				case MemberCodePoint.ClientDefinition:
					if (tpl.EnableClientCreate)
						WriteDefinition(tpl, false, cb);
					break;
			}
		}

		private void WriteDefinition(ITemplate tpl, bool isServer, CodeBuilder cb) {
			cb.AppendLine("private string " + name + "(" + parameters + ") {").Indent()
			  .AppendLine("StringBuilder " + ParserUtils.RenderFunctionStringBuilderName + " = new StringBuilder();");

			foreach (IFragment f in ParserUtils.MergeFragments(fragments))
				f.WriteCode(tpl, isServer ? FragmentCodePoint.ServerRender : FragmentCodePoint.ClientRender, cb);
			
			cb.AppendLine("return " + ParserUtils.RenderFunctionStringBuilderName + ".ToString();")
			  .Outdent().AppendLine("}").AppendLine();
		}
#endif
	}
}
