using System;

namespace Saltarelle.Members {
	public class NamedElementMember : IMember {
		private readonly string name;
	
		public string Name { get { return name; } }
		
		public NamedElementMember(string name) {
			if (string.IsNullOrEmpty(name)) throw Utils.ArgumentException("name");
			this.name = name;
		}

		public string[] Dependencies {
			get { return new string[0]; }
		}
		
		public void Instantiate(ITemplate tpl, IInstantiatedTemplateControl ctl) {
			ctl.AddNamedElement(name);
		}

#if SERVER
		public override bool Equals(object obj) {
			return obj is NamedElementMember && (obj as NamedElementMember).name == name;
		}

		public override int GetHashCode() {
			return name.GetHashCode();
		}

		public override string ToString() {
			return "Element " + name;
		}

		public void WriteCode(ITemplate tpl, MemberCodePoint point, CodeBuilder cb) {
			switch (point) {
				case MemberCodePoint.ClientDefinition:
					cb.AppendLine("private DOMElement " + name + " { get { return Document.GetElementById(id + \"_" + name + "\"); } }").AppendLine();
					break;
				case MemberCodePoint.ClientIdChanging:
					cb.AppendLine("this." + name + ".ID = value + \"_" + name + "\";");
					break;
			}
		}
#endif
	}
}
