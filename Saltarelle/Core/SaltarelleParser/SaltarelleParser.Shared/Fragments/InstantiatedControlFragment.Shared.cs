using System;
#if SERVER
using System.Text;
#endif

namespace Saltarelle.Fragments {
	internal class InstantiatedControlFragment : IFragment {
		public readonly string Id;
		public readonly bool CustomInstantiate;
		public readonly bool HasInnerHtml;
		
		public InstantiatedControlFragment(string id, bool customInstantiate, bool hasInnerHtml) {
			if (!ParserUtils.IsValidUnqualifiedName(id)) throw Utils.ArgumentException("id");
			this.Id = id;
			this.CustomInstantiate = customInstantiate;
			this.HasInnerHtml = hasInnerHtml;
		}

		public IFragment TryMergeWithNext(IFragment nextFragment) {
			return null;
		}

		public void Render(ITemplate tpl, IInstantiatedTemplateControl ctl, StringBuilder sb) {
			#if CLIENT
			IClientCreateControl me = (IClientCreateControl)ctl.Controls[Id];
			#else
			IControl me = ctl.Controls[Id];
			#endif
			if (HasInnerHtml) {
				string innerHtml = ((IRenderFunction)tpl.GetMember(Id + "_inner")).Render(tpl, ctl);
				((IControlHost)me).SetInnerHtml(innerHtml);
			}
			sb.Append(me.Html);
		}

#if SERVER
		public override bool Equals(object obj) {
			var other = obj as InstantiatedControlFragment;
			return !Utils.IsNull(other) && other.Id == Id && other.HasInnerHtml == HasInnerHtml;
		}
		
		public override int GetHashCode() {
			return Id.GetHashCode() ^ (HasInnerHtml ? 1 : 0);
		}

		public override string ToString() {
			return "[CONTROL id=" + Id + " inner=" + (HasInnerHtml ? "yes" : "no") + "]";
		}

		public void WriteCode(ITemplate tpl, FragmentCodePoint point, CodeBuilder cb) {
			if (CustomInstantiate)
				cb.AppendLine("if (Utils.IsNull(" + Id + ")) throw new InvalidOperationException(\"The control instance " + Id + " must be assigned before the control can be rendered.\");");

			if (HasInnerHtml)
				cb.AppendLine("((IControlHost)" + Id + ").SetInnerHtml(" + Id + "_inner());");
			cb.AppendLine(ParserUtils.RenderFunctionStringBuilderName + ".Append(((IControl)" + Id + ").Html);");
		}
#endif
	}
}
