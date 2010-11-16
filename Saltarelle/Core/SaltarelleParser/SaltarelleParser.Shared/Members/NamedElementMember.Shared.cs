using System;

namespace Saltarelle.Members {
	public class NamedElementMember : IMember {
		private readonly string tagName;
		private readonly string name;
	
		//A tag name, or input/subtype for inputs.
		public string TagName { get { return tagName; } }
		public string Name { get { return name; } }
		
		public NamedElementMember(string tagName, string name) {
			if (string.IsNullOrEmpty(tagName)) throw Utils.ArgumentException("name");
			if (string.IsNullOrEmpty(name)) throw Utils.ArgumentException("name");
			this.tagName = tagName;
			this.name = name;
		}

		public string[] Dependencies {
			get { return new string[0]; }
		}
		
		public void Instantiate(ITemplate tpl, IInstantiatedTemplateControl ctl) {
			ctl.AddNamedElement(name);
		}

#if SERVER
		private static string MapTagNameToClass(string tagName) {
			if (tagName.StartsWith("input", StringComparison.InvariantCultureIgnoreCase) && (tagName.Length == 5 || tagName[5] == '/')) {
				if (tagName.Length > 6) {
					switch (tagName.Substring(6)) {
						case "checkbox":
						case "radio":
							return "CheckBoxElement";
						case "password":
						case "text":
							return "TextElement";
					}
				}
				return "InputElement";
			}
			else {
				switch (tagName.ToLowerInvariant()) {
					case "a":
						return "AnchorElement";
					case "area":
						return "AreaElement";
					case "div":
						return "DivElement";
					case "form":
						return "FormElement";
					case "iframe":
						return "IFrameElement";
					case "img":
						return "ImageElement";
					case "map":
						return "MapElement";
					case "option":
						return "OptionElement";
					case "script":
						return "ScriptElement";
					case "select":
						return "SelectElement";
					case "td":
						return "TableCellElement";
					case "table":
						return "TableElement";
					case "tr":
						return "TableRowElement";
					case "thead":
					case "tbody":
					case "tfoot":
						return "TableSectionElement";
					case "textarea":
						return "TextAreaElement";
					default:
						return "DOMElement";
				}
			}
		}

		public override bool Equals(object obj) {
			return obj is NamedElementMember && (obj as NamedElementMember).TagName == this.TagName && (obj as NamedElementMember).name == name;
		}

		public override int GetHashCode() {
			return name.GetHashCode() ^ tagName.GetHashCode();
		}

		public override string ToString() {
			return "Element " + tagName + ", " + name;
		}

		public void WriteCode(ITemplate tpl, MemberCodePoint point, CodeBuilder cb) {
			switch (point) {
				case MemberCodePoint.ClientDefinition: {
					string className = MapTagNameToClass(tagName);
					cb.AppendLine("private " + className + " " + name + " { get { return (" + className + ")Document.GetElementById(id + \"_" + name + "\"); } }").AppendLine();
					break;
				}
				case MemberCodePoint.ClientIdChanging:
					cb.AppendLine("this." + name + ".ID = value + \"_" + name + "\";");
					break;
			}
		}
#endif
	}
}
