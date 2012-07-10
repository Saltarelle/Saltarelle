using System;
using System.Runtime.CompilerServices;
using System.Text;
#if CLIENT
using System.Html;
#endif

namespace Saltarelle
{
	public enum AnchoringEnum {
		NotPositioned = 0,
		TopLeft       = 1,
		Fixed         = 2
	}

	[Record]
#if SERVER
	[Serializable]
#endif
	public sealed class Position {
		public readonly AnchoringEnum anchor;
		public readonly int left;
		public readonly int top;
		
		public Position(AnchoringEnum anchor, int left, int top) {
			this.anchor = anchor;
			this.left   = left;
			this.top    = top;
		}

#if SERVER
		public override bool Equals(object obj) {
			Position other = obj as Position;
			return other != null && this.anchor == other.anchor && (this.anchor != AnchoringEnum.TopLeft || (this.left == other.left && this.top == other.top));
		}

		public override int GetHashCode() {
			return (int)anchor ^ (anchor == AnchoringEnum.TopLeft ? (left << 16 + top) : 0);
		}
#endif
	}
	
	public static class PositionHelper {
		public static Position NotPositioned = new Position(AnchoringEnum.NotPositioned, 0, 0);
		public static Position Fixed         = new Position(AnchoringEnum.Fixed, 0, 0);
		
		public static Position LeftTop(int left, int top) {
			return new Position(AnchoringEnum.TopLeft, left, top);
		}
		
		public static string PositionToString(Position position) {
			switch (position.anchor) {
				case AnchoringEnum.Fixed: return "fixed";
				case AnchoringEnum.NotPositioned: return "np";
				case AnchoringEnum.TopLeft: return "lt(" + Utils.ToStringInvariantInt(position.left) + "," + Utils.ToStringInvariantInt(position.top) + ")";
				default: throw new Exception("Invalid anchor");
			}
		}
		
		public static string CreateStyle(Position pos, int width, int height) {
			StringBuilder sb = new StringBuilder();
			switch (pos.anchor) {
				case AnchoringEnum.NotPositioned:
					break;
				case AnchoringEnum.Fixed:
					sb.Append("position: relative; left: 0px; top: 0px; ");
					break;
				case AnchoringEnum.TopLeft:
					sb.Append("position: absolute; left: " + Utils.ToStringInvariantInt(pos.left) + "px; top: " + Utils.ToStringInvariantInt(pos.top) + "px;");
					break;
				default:
					throw new Exception("Invalid position anchor " + pos.anchor);
			}
			if (width >= 0)
				sb.Append("width:" + Utils.ToStringInvariantInt(width) + "px;");
			if (height >= 0)
				sb.Append("height:" + Utils.ToStringInvariantInt(height) + "px;");
			return sb.ToString();
		}

#if CLIENT
		public static void ApplyPosition(Element el, Position pos) {
			switch (pos.anchor) {
				case AnchoringEnum.NotPositioned:
					el.Style.Position = "";
					el.Style.Left     = "";
					el.Style.Top      = "";
					break;
				case AnchoringEnum.Fixed:
					el.Style.Position = "relative";
					el.Style.Left     = "0px";
					el.Style.Top      = "0px";
					break;
				case AnchoringEnum.TopLeft:
					el.Style.Position = "absolute";
					el.Style.Left     = Utils.ToStringInvariantInt(pos.left) + "px";
					el.Style.Top      = Utils.ToStringInvariantInt(pos.top) + "px";
					break;
			}
		}
		
		public static Position GetPosition(Element el) {
			switch (el.Style.Position) {
				case "absolute":
					return LeftTop(Utils.ParseInt(el.Style.Left.Replace("px", "")), Utils.ParseInt(el.Style.Top.Replace("px", "")));
				case "relative":
					return Fixed;
				default:
					return NotPositioned;
			}
		}
#endif
	}
}