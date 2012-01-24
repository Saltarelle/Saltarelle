using System;
#if CLIENT
using FragmentList = System.ArrayList;
using StringList   = System.ArrayList;
#else
using FragmentList = System.Collections.Generic.List<Saltarelle.IFragment>;
using StringList   = System.Collections.Generic.List<string>;
using System.Text;
#endif

namespace Saltarelle.Members {
	internal class RenderFunctionMember : IRenderFunction, IMember {
		private readonly string name;
		private readonly string parameters;
		private FragmentList fragments;
		private StringList dependencies;
		
		public string Name { get { return name; } }
		public string Parameters { get { return parameters; } }
		public FragmentList Fragments { get { return fragments; } }
		
		public RenderFunctionMember(string name, string parameters) {
			if (!ParserUtils.IsValidUnqualifiedName(name)) throw Utils.ArgumentException("name");
			if (Utils.IsNull(parameters)) throw Utils.ArgumentNullException("parameters");
			this.name         = name;
			this.parameters   = parameters;
			this.fragments    = new FragmentList();
			this.dependencies = new StringList();
		}

		public bool IsEmpty {
			get { return Utils.ArrayLength(fragments) == 0; }
		}
		
		public void AddDependency(IMember dependency) {
			dependencies.Add(dependency.Name);
		}
		
		public void AddFragment(IFragment fragment) {
			fragments.Add(fragment);
		}
		
		public string[] Dependencies {
			get {
				#if SERVER
					return dependencies.ToArray();
				#else
					return (string[])dependencies;
				#endif
			}
		}

		public void Instantiate(ITemplate tpl, IInstantiatedTemplateControl ctl) {
		}
		
		public string Render(ITemplate tpl, IInstantiatedTemplateControl ctl) {
			StringBuilder sb = new StringBuilder();
			foreach (IFragment f in ParserUtils.MergeFragments(fragments))
				f.Render(tpl, ctl, sb);
			return sb.ToString();
		}

#if SERVER
		public override bool Equals(object obj) {
			RenderFunctionMember other = obj as RenderFunctionMember;
			if (Utils.IsNull(other) || other.name != name || other.parameters != parameters || other.fragments.Count != fragments.Count || other.dependencies.Count != dependencies.Count)
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
