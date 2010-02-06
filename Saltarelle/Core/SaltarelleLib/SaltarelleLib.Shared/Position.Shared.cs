using System;
#if SERVER
using System.Text;
#endif
#if CLIENT
using System.DHTML;
#endif

namespace Saltarelle
{
	public enum AnchoringEnum {
		NotPositioned = 0,
		TopLeft       = 1,
		Fixed         = 2
	}

	[Record]
	public sealed class Position {
		public readonly AnchoringEnum anchor;
		public readonly int left;
		public readonly int top;
		
		public Position(AnchoringEnum anchor, int left, int top) {
			this.anchor = anchor;
			this.left   = left;
			this.top    = top;
		}
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
		public static void ApplyPosition(jQuery el, Position pos) {
			switch (pos.anchor) {
				case AnchoringEnum.NotPositioned:
					el.css("position", "");
					el.css("left", "");
					el.css("top", "");
					break;
				case AnchoringEnum.Fixed:
					el.css("position", "relative");
					el.css("left", "0px");
					el.css("top", "0px");
					break;
				case AnchoringEnum.TopLeft:
					el.css("position", "absolute");
					el.css("left", Utils.ToStringInvariantInt(pos.left) + "px");
					el.css("top", Utils.ToStringInvariantInt(pos.top) + "px");
					break;
			}
		}
		
		public static Position GetPosition(jQuery el) {
			DOMElement d = el.get(0);
			switch (d.Style.Position) {
				case "absolute":
					return LeftTop(Utils.ParseInt(d.Style.Left.Replace("px", "")), Utils.ParseInt(d.Style.Top.Replace("px", "")));
				case "relative":
					return Fixed;
				default:
					return NotPositioned;
			}
		}
#endif
	}
}