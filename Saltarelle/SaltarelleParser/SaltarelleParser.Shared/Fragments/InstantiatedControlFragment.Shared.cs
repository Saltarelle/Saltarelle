using System;
using System.Text;

namespace Saltarelle.Fragments {
	internal class InstantiatedControlFragment : IFragment {
		public readonly string Id;
		public readonly bool CustomInstantiate;
		public readonly int NumInnerFragments;
		
		public InstantiatedControlFragment(string id, bool customInstantiate, int numInnerFragments) {
			if (!ParserUtils.IsValidUnqualifiedName(id)) throw Utils.ArgumentException("id");
			this.Id = id;
			this.CustomInstantiate = customInstantiate;
			this.NumInnerFragments = numInnerFragments;
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
			if (NumInnerFragments > 0) {
				string[] innerFragments = new string[NumInnerFragments];
				for (int i = 0; i < NumInnerFragments; i++) {
					innerFragments[i] = ((IRenderFunction)tpl.GetMember(Id + "_inner" + Utils.ToStringInvariantInt(i + 1))).Render(tpl, ctl);
				}
				((IControlHost)me).SetInnerFragments(innerFragments);
			}
			sb.Append(me.Html);
		}

#if SERVER
		public override bool Equals(object obj) {
			var other = obj as InstantiatedControlFragment;
			return !Utils.IsNull(other) && other.Id == Id && other.NumInnerFragments == NumInnerFragments;
		}
		
		public override int GetHashCode() {
			return Id.GetHashCode() ^ NumInnerFragments;
		}

		public override string ToString() {
			return "[CONTROL id=" + Id + " inner=" + NumInnerFragments.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
		}

		public void WriteCode(ITemplate tpl, FragmentCodePoint point, CodeBuilder cb) {
			if (CustomInstantiate)
				cb.AppendLine("if (Utils.IsNull(" + Id + ")) throw new InvalidOperationException(\"The control instance " + Id + " must be assigned before the control can be rendered.\");");
			
			if (NumInnerFragments > 0) {
				cb.Append("((IControlHost)" + Id + ").SetInnerFragments(new string[] { ");
				for (int i = 0; i < NumInnerFragments; i++) {
					if (i > 0)
						cb.Append(", ");
					cb.Append(Id + "_inner" + (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture) + "()");
				}
				cb.AppendLine(" });");
			}

			cb.AppendLine(ParserUtils.RenderFunctionStringBuilderName + ".Append(((IControl)" + Id + ").Html);");
		}
#endif
	}
}
