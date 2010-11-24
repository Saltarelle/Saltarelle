using System;
#if SERVER
using System.Collections.Generic;
#endif
#if CLIENT
using System.DHTML;
#endif

namespace Saltarelle.UI {
	public enum DialogModalityEnum {
		Modeless       = 0,
		Modal          = 1,
		HideOnFocusOut = 2
	}
	
	public abstract class DialogBase : IControl, IClientCreateControl {
		private const string NoPaddingClassName = "NoPaddingDialog";
		private const short  FirstDialogZIndex  = 10000;
		private const string ModalCoverId       = "DialogModalCoverDiv";
	
		private string id;
		private string title;
		private DialogModalityEnum modality;
		private Position position = PositionHelper.NotPositioned;
		private string className;
		private bool hasPadding;
		private bool removeOnClose;

		#if CLIENT
			private bool hasBgiframe = false;
			private bool isAttached;
			private bool areEventsBound = false;
			public event EventHandler Opened;
			public event CancelEventHandler Closing;
			public event EventHandler Closed;
			public event CancelEventHandler Opening;
			
			private static ArrayList currentShownDialogs = new ArrayList();
		#endif

		public virtual string Id {
			get { return id; }
			set {
				id = value;
				#if CLIENT
					if (isAttached)
						GetElement().ID = value;
				#endif
			}
		}

		public Position Position {
			get { return position; }
			set { position = value; }
		}

		public bool RemoveOnClose {
			get { return removeOnClose; }
			set { removeOnClose = value; }
		}
		
		public DialogModalityEnum Modality {
			get { return modality; }
			set { modality = value; }
		}
		
		public int ModalityInt { // Intended to use in templates.
			get { return (int)modality; }
			set { modality = (DialogModalityEnum)value; }
		}

		public string Title {
			get { return title; }
			set {
				#if CLIENT
					string oldTitle = title;
					title = (value ?? "").Trim();

					if (isAttached) {
						DOMElement elem = GetElement();
						if (string.IsNullOrEmpty(oldTitle) && !string.IsNullOrEmpty(title)) {
							// Add the titlebar.
							jQuery tb = JQueryProxy.jQuery(TitlebarHtml);
							tb.insertBefore(JQueryProxy.jQuery(elem.Children[hasBgiframe ? 1 : 0]));
							if (areEventsBound)
								tb.find("a").click(delegate(JQueryEvent evt) { Close(); evt.preventDefault(); });
						}
						else if (!string.IsNullOrEmpty(oldTitle) && string.IsNullOrEmpty(title)) {
							// Remove the titlebar.
							JQueryProxy.jQuery(elem.Children[hasBgiframe ? 1 : 0]).remove();
						}
						else if (!string.IsNullOrEmpty(title)) {
							elem.Children[hasBgiframe ? 1 : 0].Children[0].InnerText = title;
						}
					}
				#else
					title = (value ?? "").Trim();
				#endif
			}
		}

		public string ClassName {
			get { return className; }
			set {
				#if CLIENT
					string oldClassName = className;
					className = (value ?? "").Trim();
					if (isAttached) {
						GetElement().ClassName = EffectiveDialogClass;
					}
				#else
					className = (value ?? "").Trim();
				#endif
			}
		}

		protected abstract string InnerHtml { get; }
		
		private string EffectiveDialogClass {
			get {
				return "ui-dialog ui-widget ui-widget-content ui-corner-all"
				     + (!hasPadding ? " " + NoPaddingClassName : "")
				     + (!string.IsNullOrEmpty(className) ? " " + className : "");
			}
		}
		
		private string TitlebarHtml {
			get {
				if (string.IsNullOrEmpty(title))
					return null;
				return "<div style=\"MozUserSelect: none; *width: expression(this.nextSibling.clientWidth + 'px')\" class=\"ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix\" unselectable=\"on\">"
				     +     "<span style=\"MozUserSelect: none\" class=\"ui-dialog-title\" unselectable=\"on\">" + title + "</span>"
				     +     "<a style=\"MozUserSelect: none\" class=\"ui-dialog-titlebar-close ui-corner-all\" href=\"#\"><span style=\"MozUserSelect: none\" class=\"ui-icon ui-icon-closethick\" unselectable=\"on\">close</span></a>"
				     + "</div>";
			}
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				return "<div id=\"" + Utils.HtmlEncode(id) + "\" style=\"position: absolute; width: auto; height: auto; *width: 1px; *height: 1px\" class=\"" + EffectiveDialogClass + "\" tabindex=\"-1\" unselectable=\"on\">"
				     +    TitlebarHtml
				     +    "<div class=\"ui-dialog-content ui-widget-content\">"
				     +        InnerHtml
				     +    "</div>"
				     + "</div>";
			}
		}
		
		public bool HasPadding {
			get { return hasPadding; }
			set {
				hasPadding = value;
				#if CLIENT
					if (isAttached) {
						GetElement().ClassName = EffectiveDialogClass;
					}
				#endif
			}
		}
		
		protected virtual void InitDefault() {
			title         = "";
			className     = "";
			modality      = DialogModalityEnum.Modeless;
			hasPadding    = true;
			removeOnClose = false;
		}

#if SERVER
		protected DialogBase() {
			GlobalServices.Provider.GetService<IScriptManagerService>().RegisterClientType(GetType());
			InitDefault();
		}
		
		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["id"]            = id;
			config["modality"]      = modality;
			config["title"]         = title;
			config["position"]      = position;
			config["hasPadding"]    = hasPadding;
			config["className"]     = className;
			config["removeOnClose"] = removeOnClose;
		}

		public object ConfigObject {
			get {
				var config = new Dictionary<string, object>();
				AddItemsToConfigObject(config);
				return config;
			}
		}
#endif
#if CLIENT
		private static void RepositionCover(DOMElement cover) {
			cover.Style.Top    = Math.Max(Document.Body.ScrollTop,  Document.DocumentElement.ScrollTop).ToString() + "px";
			cover.Style.Left   = Math.Max(Document.Body.ScrollLeft, Document.DocumentElement.ScrollLeft).ToString() + "px";
   			cover.Style.Width  = Document.DocumentElement.ClientWidth.ToString()  + "px";
			cover.Style.Height = Document.DocumentElement.ClientHeight.ToString() + "px";
		}

		private static DOMElement GetModalCover(bool createIfMissing) {
			DOMElement elem = Document.GetElementById(ModalCoverId);
			if (elem != null || !createIfMissing)
				return elem;
			
			jQuery jq = JQueryProxy.jQuery("<div id=\"" + ModalCoverId + "\" class=\"ui-widget-overlay\" style=\"display: none\">&nbsp;</div>");
			if (jQuery.browser.msie && Utils.ParseDouble(jQuery.browser.version) < 7.0) {
				jq.bgiframe();
				// Need to position the cover in JavaScript. In all other browsers, this is done in CSS.
				jQuery wnd = JQueryProxy.jQuery((DOMElement)Script.Literal("window"));
				wnd.scroll(delegate(JQueryEvent evt) {
					RepositionCover(GetModalCover(false));
				});
				wnd.resize(delegate(JQueryEvent evt) {
					RepositionCover(GetModalCover(false));
				});
				RepositionCover(jq.get(0));
			}

			jq.appendTo(Document.Body);
			return jq.get(0);
		}

		[AlternateSignature]
		protected extern DialogBase();

		protected DialogBase(object config) {
			if (!Script.IsUndefined(config)) {
				InitConfig(Dictionary.GetDictionary(config));
			}
			else
				InitDefault();
		}
		
		protected virtual void InitConfig(Dictionary config) {
			id            = (string)config["id"];
			title         = (string)config["title"];
			modality      = (DialogModalityEnum)config["modality"];
			position      = (Position)config["position"];
			hasPadding    = (bool)config["hasPadding"];
			className     = (string)config["className"];
			removeOnClose = (bool)config["removeOnClose"];
			AttachSelf();
		}

		public DOMElement GetElement() { return isAttached ? Document.GetElementById(id) : null; }
		
		private void MoveElementToEnd(DOMElement elem) {
			elem.ParentNode.RemoveChild(elem);
			Document.Body.AppendChild(elem);
		}

		protected virtual void AttachSelf() {
			if (Utils.IsNull(id) || isAttached)
				throw new Exception("Must set ID and can only attach once");
			isAttached = true;

			// Move the dialog to the end of the body.
			DOMElement element = GetElement();
			MoveElementToEnd(element);
			element.Style.Display = "none";
		}

		public virtual void Attach() {
			AttachSelf();
		}

		public void Open() {
			if (IsOpen)
				Close();
			if (IsOpen)
				return;	// Apparantly a Closing handler prevented us from closing.

			if (!isAttached)
				throw new Exception("Cannot open dialog before attach");

			CancelEventArgs e = new CancelEventArgs();
			OnOpening(e);
			if (e.Cancel)
				return;

			DOMElement elem = GetElement();
			
			if (!areEventsBound) {
				JQueryProxy.jQuery(elem).lostfocus(Element_LostFocus);
				if (!string.IsNullOrEmpty(title)) {
					JQueryProxy.jQuery(elem.Children[0].GetElementsByTagName("a")[0]).click(delegate(JQueryEvent evt) { Close(); evt.preventDefault(); });
				}
				areEventsBound = true;
			}

			// Defer the bgiframe until opening to save load time.
			if (!hasBgiframe && jQuery.browser.msie && Utils.ParseDouble(jQuery.browser.version) < 7) {
				JQueryProxy.jQuery(elem).bgiframe();
				hasBgiframe = true;
			}
			
			short zIndex = FirstDialogZIndex;
			if (currentShownDialogs.Length > 0) {
				DialogBase tail = (DialogBase)currentShownDialogs[currentShownDialogs.Length - 1];
				zIndex = (short)(tail.GetElement().Style.ZIndex + 2);
			}

			// Move the element to the correct position, or to (0, 0) if it is to be centered later.
			elem.Style.Left    = (position.anchor == AnchoringEnum.TopLeft ? position.left : 0).ToString() + "px";
			elem.Style.Top     = (position.anchor == AnchoringEnum.TopLeft ? position.top  : 0).ToString() + "px";
			elem.Style.ZIndex  = zIndex;
			// Show the dialog.
			elem.Style.Display = "";

			if (position.anchor != AnchoringEnum.TopLeft) {
				// Center the dialog
				jQuery wnd = JQueryProxy.jQuery((DOMElement)(object)Window.Self), el = JQueryProxy.jQuery(elem);
				elem.Style.Left = Math.Round(Document.Body.ScrollLeft + (wnd.width()  - el.width() ) / 2).ToString() + "px";
				elem.Style.Top  = Math.Round(Document.Body.ScrollTop  + (wnd.height() - el.height()) / 2).ToString() + "px";
			}
			
			if (modality == DialogModalityEnum.Modal) {
				DOMElement cover = GetModalCover(true);
				cover.Style.ZIndex  = (short)(zIndex - 1);
				cover.Style.Display = "";
			}

			currentShownDialogs.Add(this);

			elem.Focus();
			OnOpened(EventArgs.Empty);
		}
		
		public bool IsOpen {
			get { return isAttached ? GetElement().Style.Display != "none" : false; }
		}
		
		public void Close() {
			if (!IsOpen)
				return;
		
			CancelEventArgs e = new CancelEventArgs();
			OnClosing(e);
			if (e.Cancel)
				return;

			// remove this dialog from the shown list
			for (int i = 0; i < currentShownDialogs.Length; i++) {
				if (currentShownDialogs[i] == this) {
					currentShownDialogs.RemoveAt(i);
					break;
				}
			}

			// find the topmost modal dialog
			int modalIndex = -1;
			for (int i = currentShownDialogs.Length - 1; i >= 0; i--) {
				if (((DialogBase)currentShownDialogs[i]).Modality == DialogModalityEnum.Modal) {
					modalIndex = i;
					break;
				}
			}

			// handle the modal cover
			DOMElement cover = GetModalCover(false);
			if (modalIndex == -1) {
				if (cover != null)
					cover.Style.Display = "none";
			}
			else {
				cover.Style.ZIndex = (short)(((DialogBase)currentShownDialogs[modalIndex]).GetElement().Style.ZIndex - 1);
			}

			GetElement().Style.Display = "none";
			if (currentShownDialogs.Length > 0)
				((DialogBase)currentShownDialogs[currentShownDialogs.Length - 1]).Focus();

			OnClosed(EventArgs.Empty);
		}

		protected virtual void OnOpening(CancelEventArgs e) {
			if (!Utils.IsNull(Opening))
				Opening(this, e);
		}
		
		protected virtual void OnOpened(EventArgs e) {
			if (!Utils.IsNull(Opened))
				Opened(this, e);
		}

		protected virtual void OnClosing(CancelEventArgs e) {
			if (!Utils.IsNull(Closing))
				Closing(this, e);
		}

		protected virtual void OnClosed(EventArgs e) {
			if (!Utils.IsNull(Closed))
				Closed(this, e);

			if (removeOnClose) {
				JQueryProxy.jQuery(GetElement()).remove();
				isAttached = false;
			}
		}
		
		private void ModalFocusOut() {
			DOMElement activeElem = Document.ActiveElement;

			bool ok = false;
			int i;
			for (i = 0; i < currentShownDialogs.Length; i++) {
				if (currentShownDialogs[i] == this)
					break;
			}
			if (i < currentShownDialogs.Length) {
				for (i = i + 1; i < currentShownDialogs.Length; i++) {	// allow focus to go to a later dialog
					if (((DialogBase)currentShownDialogs[i]).GetElement().Contains(activeElem)) {
						ok = true;
						break;
					}
				}
			}
			else
				ok = true;	// the dialog is no longer on the stack - it is being hidden

			if (!ok)
				Focus();
		}
		
		private void VolatileFocusOut() {
			DOMElement activeElem = Document.ActiveElement;

			// find out whether it's a child of ours or of a dialog later in the dialog stack
			int i = 0;
			for (i = 0; i < currentShownDialogs.Length; i++) {
				if (currentShownDialogs[i] == this)
					break;
			}
			for (; i < currentShownDialogs.Length; i++) {
				if (((DialogBase)currentShownDialogs[i]).GetElement().Contains(activeElem))
					return;
			}
			
			// Focus left us - hide.
			Close();
			if (IsOpen)
				Focus();	// Just in case a Closing handler prevented the close.
		}
		
		private void Element_LostFocus(JQueryEvent evt) {
			switch (modality) {
				case DialogModalityEnum.Modal:
					Window.SetTimeout(ModalFocusOut, 0);
					break;
				case DialogModalityEnum.HideOnFocusOut:
					Window.SetTimeout(VolatileFocusOut, 0);
					break;
			}
		}
		
		public virtual void Focus() {
			GetElement().Focus();
		}
#endif
	}

	public sealed class DialogFrame : DialogBase, IControlHost {
		private string innerHtml;

		public void SetInnerFragments(string[] fragments) {
			#if CLIENT
				if (GetElement() != null)
					throw new Exception("Can't change inner HTML after render.");
			#endif
			innerHtml = Utils.JoinStrings("", fragments ?? new string[0]);
		}

		protected override string InnerHtml { get { return innerHtml ?? ""; } }

#if SERVER
		public DialogFrame() {
			GlobalServices.GetService<IScriptManagerService>().RegisterClientType(GetType());
		}
#endif

#if CLIENT
		[AlternateSignature]
		public extern DialogFrame();

		public DialogFrame(object config) : base(config) {
		}

		public DOMElement[] GetInnerElements() {
			jQuery jq = JQueryProxy.jQuery(GetElement());
			ArrayList result = new ArrayList();
			for (int i = 0; i < jq.size(); i++)
				result.Add(jq.get(i));
			return (DOMElement[])result;
		}
#endif
	}
	
	public abstract class ControlDialogBase : DialogBase {
		private IControl containedControl;
		
		public override string Id {
			get { return base.Id; }
			set {
				base.Id = value;
				if (!Utils.IsNull(containedControl))
					containedControl.Id = value + "_control";
			}
		}
		
		protected IControl GetContainedControlBase() { return containedControl; }

#if SERVER
		protected override void AddItemsToConfigObject(Dictionary<string, object> config) {
			base.AddItemsToConfigObject(config);
			config.Add("containedControlType", containedControl.GetType().FullName);
			config.Add("containedControlData", containedControl.ConfigObject);
		}

		protected ControlDialogBase() {
			GlobalServices.Provider.GetService<IScriptManagerService>().RegisterClientType(GetType());
		}

		protected override string InnerHtml { get { return containedControl.Html; } }

		protected void SetContainedControlBase(IControl value) {
			containedControl = value;
			if (!string.IsNullOrEmpty(Id))
				containedControl.Id = Id + "_control";
		}
#endif
#if CLIENT
		[AlternateSignature]
		protected extern ControlDialogBase();
		protected ControlDialogBase(object config) : base(config) {
		}

		protected override void InitConfig(Dictionary config) {
			Type tp = Type.GetType((string)config["containedControlType"]);
			containedControl = (IControl)Type.CreateInstance(tp, config["containedControlData"]);
			base.InitConfig(config);
		}

		protected override string InnerHtml { get { return ((IClientCreateControl)containedControl).Html; } }
		
		protected void SetContainedControlBase(IClientCreateControl control) {
			if (!Utils.IsNull(((IControl)control).GetElement()))
				throw new Exception("The control must not be rendered.");
			containedControl = (IControl)control;
			if (!string.IsNullOrEmpty(Id))
				((IControl)control).Id = Id + "_control";
		}
		
		public override void Attach() {
			((IClientCreateControl)containedControl).Attach();
			AttachSelf();
		}
#endif
	}
	
	public sealed class ControlDialog : ControlDialogBase {
		public IControl GetContainedControl() {
			return GetContainedControlBase();
		}

#if SERVER
		public ControlDialog() {
			GlobalServices.Provider.GetService<IScriptManagerService>().RegisterClientType(GetType());
		}

		public void SetContainedControl(IControl value) {
			SetContainedControlBase(value);
		}
#endif
#if CLIENT
		[AlternateSignature]
		public extern ControlDialog();
		
		public ControlDialog(object config) : base(config) {
		}

		public void SetContainedControl(IClientCreateControl control) {
			SetContainedControlBase(control);
		}
#endif
	}
}
