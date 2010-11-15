using System;
using System.DHTML;

namespace Saltarelle.UI {
	public delegate char TransformCharDelegate(char ch);

	public static partial class UIUtils {
		public static void InputKeypressHandler(JQueryEvent evt, InputElement el, TransformCharDelegate transformChar) {
			if (evt.altKey || evt.ctrlKey)
				return; // don't ever change Alt+key or Ctrl+key

			if (jQuery.browser.msie) {
				if (evt.keyCode == 13)
					return; // Enter seems to be the only non-printable key we catch in IE.
				char newc = transformChar((char)evt.keyCode);
				if (newc == 0) {
					evt.preventDefault();
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
					evt.preventDefault();
				}
				else if (newc != evt.charCode) {
					int startPos = (int)Type.GetField(el, "selectionStart"),
					    endPos   = (int)Type.GetField(el, "selectionEnd");
					string oldVal = el.Value;
					el.Value = oldVal.Substr(0, startPos) + String.FromCharCode(newc) + oldVal.Substr(endPos);
					Type.InvokeMethod(el, "setSelectionRange", startPos + 1, startPos + 1);
					evt.preventDefault();
				}
			}
		}

		public static void AttachKeyPressHandler(DOMElement el, JQueryEventHandlerDelegate handler) {
			// delete, end, enter, escape, home, insert, page up, page down, tab, F1-F12
			// http://www.quirksmode.org/js/keys.html.
			jQuery jq = JQueryProxy.jQuery(el);
            jq.keydown(handler);
            if (jQuery.browser.msie && (jQuery.browser.version == "6.0" || jQuery.browser.version == "7.0")) {
				jq.keypress(delegate(JQueryEvent e) {
					if (new int[] { 46, 35, 13, 27, 36, 45, 33, 34, 9 }.Contains(e.keyCode) || (e.keyCode >= 112 && e.keyCode <= 123))
						handler(e);
				});
			}
		}
	}
}
