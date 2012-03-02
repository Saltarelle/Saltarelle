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
			// return, escape, F1-F12
			// http://www.quirksmode.org/js/keys.html.
			jQuery jq = JQueryProxy.jQuery(el);
            jq.keydown(handler);
            if (jQuery.browser.msie && (jQuery.browser.version == "6.0" || jQuery.browser.version == "7.0")) {
				jq.keypress(delegate(JQueryEvent e) {
					if (e.keyCode == 13 || e.keyCode == 27 || (e.keyCode >= 112 && e.keyCode <= 123))
						handler(e);
				});
			}
		}

		/// <summary>
		/// This method will hide then immediately restore all SELECT elements that are children of a specific element, if the browser is IE7. The reason is to fix a strange IE7 bug which causes SELECTs to sometimes be non-interactable.
		/// </summary>
		/// <param name="parent"></param>
		public static void FixStrangeIE7SelectIssue(DOMElement parent) {
			if (jQuery.browser.msie && Utils.ParseInt(jQuery.browser.version) == 7) {
				// Fix for the strange IE7 bug that causes SELECTs to sometimes be non-interactable
				Window.SetTimeout(delegate {
					JQueryProxy.jQuery(parent).find("select").each(delegate(int _, DOMElement el) {
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
