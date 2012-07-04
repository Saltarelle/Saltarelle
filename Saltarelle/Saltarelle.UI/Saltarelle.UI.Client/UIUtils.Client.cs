using System;
using System.Html;
using jQueryApi;

namespace Saltarelle.UI {
	public delegate char TransformCharDelegate(char ch);

	public static partial class UIUtils {
		public static void InputKeypressHandler(jQueryEvent evt, InputElement el, TransformCharDelegate transformChar) {
			if (evt.AltKey || evt.CtrlKey)
				return; // don't ever change Alt+key or Ctrl+key

			if (jQuery.Browser.MSIE) {
				if (evt.keyCode == 13)
					return; // Enter seems to be the only non-printable key we catch in IE.
				char newc = transformChar((char)evt.keyCode);
				if (newc == 0) {
					evt.PreventDefault();
				}
				else if (newc != evt.keyCode) {
					Type.SetField(evt.originalEvent, "keyCode", newc);
				}
			}
			else {
				if (evt.charCode == 0)
					return; // Firefox, and likely other non-IE browsers, lets us trap non-characters, but we don't want that.

				char newc = transformChar(evt.charCode);
				if (newc == 0) {
					evt.PreventDefault();
				}
				else if (newc != evt.charCode) {
					int startPos = ((dynamic)el).selectionStart,
					    endPos   = ((dynamic)el).selectionEnd;
					string oldVal = el.Value;
					el.Value = oldVal.Substr(0, startPos) + String.FromCharCode(newc) + oldVal.Substr(endPos);
					((dynamic)el).setSelectionRange(startPos + 1, startPos + 1);
					evt.PreventDefault();
				}
			}
		}

		public static void AttachKeyPressHandler(Element el, jQueryEventHandler handler) {
			// return, escape, F1-F12
			// http://www.quirksmode.org/js/keys.html.
			var jq = jQuery.FromElement(el);
            jq.Keydown(handler);
            if (jQuery.Browser.MSIE && (jQuery.Browser.Version == "6.0" || jQuery.Browser.Version == "7.0")) {
				jq.Keypress(delegate(jQueryEvent e) {
					if (e.keyCode == 13 || e.keyCode == 27 || (e.keyCode >= 112 && e.keyCode <= 123))
						handler(e);
				});
			}
		}

		/// <summary>
		/// This method will hide then immediately restore all SELECT elements that are children of a specific element, if the browser is IE7. The reason is to fix a strange IE7 bug which causes SELECTs to sometimes be non-interactable.
		/// </summary>
		/// <param name="parent"></param>
		public static void FixStrangeIE7SelectIssue(Element parent) {
			if (jQuery.Browser.MSIE && Utils.ParseInt(jQuery.Browser.Version) == 7) {
				// Fix for the strange IE7 bug that causes SELECTs to sometimes be non-interactable
				Window.SetTimeout(delegate {
					jQuery.FromElement(parent).Find("select").Each((_, el) => {
						string oldDisplay = el.Style.Display;
						el.Style.Display = (el.Style.Display != "none" ? "none" : "");
						el.Style.Display = oldDisplay;
						return true;
					});
				}, 0);
			}
		}
	}
}
