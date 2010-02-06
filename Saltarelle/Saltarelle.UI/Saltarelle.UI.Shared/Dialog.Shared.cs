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
		public const string NoPaddingClassName = "NoPaddingDialog";
		public const string NoTitlebarClassName = "NoTitlebarDialog";
	
		private string id;
		private int width;
		private int height;
		private string title;
		private DialogModalityEnum modality;
		private Position position = PositionHelper.NotPositioned;
		private string className;
		private bool noPadding;

		#if CLIENT
			private jQuery element;
			public event EventHandler Opened;
			public event CancelEventHandler Closing;
			public event EventHandler Closed;
			public event CancelEventHandler Opening;
		#endif

		public virtual string Id {
			get { return id; }
			set {
				id = value;
				#if CLIENT
					if (element != null)
						element.attr("id", value);
				#endif
			}
		}

		public Position Position {
			get { return position; }
			set { position = value; }
		}
		
		public DialogModalityEnum Modality {
			get { return modality; }
			set { modality = value; }
		}
		
		public int ModalityInt { // Intended to use in templates.
			get { return (int)modality; }
			set { modality = (DialogModalityEnum)value; }
		}

		public virtual int Width {
			get {return width; }
			set {
				width = value;
				#if CLIENT
					if (element != null) {
						element.dialog("option", "width", value);
						element.dialog("option", "minWidth", value);
						element.dialog("option", "maxWidth", value);
					}
				#endif
			}
		}
		
		public virtual int Height {
			get { return height; }
			set {
				height = value;
				#if CLIENT
					if (element != null) {
						element.dialog("option", "height", value);
						element.dialog("option", "minHeight", value);
						element.dialog("option", "maxHeight", value);
					}
				#endif
			}
		}
		
		public string Title {
			get { return title; }
			set {
				#if CLIENT
					if (element != null) {
						WidthHeight old = GetMyExtraSpace();
						title = value;
						WidthHeight nw = GetMyExtraSpace();
						Width  += Math.Round(nw.width - old.width);
						Height += Math.Round(nw.height - old.height);
						element.dialog("option", "dialogClass", EffectiveDialogClass);
						element.dialog("option", "title", title);
						return;
					}
				#endif
				title = value;
			}
		}
		
		public string ClassName {
			get { return className; }
			set {
				className = value;
				#if CLIENT
					if (element != null) {
						element.dialog("option", "dialogClass", EffectiveDialogClass);
					}
				#endif
			}
		}
		
		protected abstract string InnerHtml { get; }

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				return "<div id=\"" + id + "\""
				#if SERVER
				     + " __cfg=\"" + Utils.HtmlEncode(Utils.Json(ConfigObject)) + "\""
				#endif
				     + ">" + InnerHtml + "</div>";
			}
		}
		
		public bool NoPadding {
			get { return noPadding; }
			set {
				#if CLIENT
					if (element != null) {
						WidthHeight old = GetMyExtraSpace();
						noPadding = value;
						WidthHeight nw = GetMyExtraSpace();
						Width  += Math.Round(nw.width - old.width);
						Height += Math.Round(nw.height - old.height);
						element.dialog("option", "dialogClass", EffectiveDialogClass);
						return;
					}
				#endif
				noPadding = value;
			}
		}
		
		protected virtual void InitDefault() {
			title = "";
			width = -1;
			height = -1;
		}

#if SERVER
		protected DialogBase() {
			#if SERVER
				GlobalServices.Provider.GetService<IScriptManagerService>().RegisterType(GetType());
			#endif
			InitDefault();
		}
		
		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["modality"] = modality;
			config["title"] = title;
			config["width"] = width;
			config["height"] = height;
			config["position"] = position;
			config["className"] = className;
			config["noPadding"] = noPadding;
		}

		private object ConfigObject {
			get {
				var config = new Dictionary<string, object>();
				AddItemsToConfigObject(config);
				return config;
			}
		}
#endif
#if CLIENT
		[AlternateSignature]
		protected extern DialogBase();

		protected DialogBase(string id) {
			if (!Script.IsUndefined(id)) {
				this.id = id;
				Dictionary config = (Dictionary)Utils.EvalJson((string)JQueryProxy.jQuery("#" + id).attr("__cfg"));
				InitConfig(config);
			}
			else
				InitDefault();
		}
		
		protected virtual void InitConfig(Dictionary config) {
			title = (string)config["title"];
			width = (int)config["width"];
			height = (int)config["height"];
			modality = (DialogModalityEnum)config["modality"];
			position = (Position)config["position"];
			className = (string)config["className"];
			noPadding = (bool)config["noPadding"];
			AttachSelf();
		}
		
		public jQuery Element {
			get { return element; }
		}
		
		protected WidthHeight GetMyExtraSpace() {
			return GetExtraSpace(!string.IsNullOrEmpty(title), !noPadding);
		}
		
		private static WidthHeight GetExtraSpace(bool hasCaption, bool hasPadding) {
			if (hasCaption)
				if (hasPadding)
					return new WidthHeight(28, 35);
				else
					return new WidthHeight(0, jQuery.browser.msie && (jQuery.browser.version == "6.0" || jQuery.browser.version == "7.0") ? 21 : 24);
			else
				if (hasPadding)
					return new WidthHeight(28, 14);
				else
					return new WidthHeight(0, 0);
		}
		
		public static WidthHeight MeasureDialog(jQuery el, bool hasCaption, bool hasPadding) {
			el.css("float", "left");
			el.addClass("ui-dialog-content");
			WidthHeight extra = GetExtraSpace(hasCaption, hasPadding);
			WidthHeight result = new WidthHeight(el.width() + extra.width, el.height() + extra.height);
			el.css("float", "none");
			el.removeClass("ui-dialog-content");
			return result;
		}

		private string EffectiveDialogClass {
			get { return (className ?? "") + (string.IsNullOrEmpty(title) ? " " + NoTitlebarClassName : "") + (noPadding ? " " + NoPaddingClassName : ""); }
		}

		protected virtual void AttachSelf() {
			if (id == null || element != null)
				throw new Exception("Must set ID and can only attach once");
			element = JQueryProxy.jQuery("#" + id);
			if (width < 0 || height < 0) {
				WidthHeight wh = MeasureDialog(element, !string.IsNullOrEmpty(title), !noPadding);
				if (width < 0)
					width = Math.Round(wh.width);
				if (height < 0)
					height = Math.Round(wh.height);
			}

			element.dialog(new Dictionary("modal", modality == DialogModalityEnum.Modal,
			                         "bgiframe", true,
			                         "title", title,
			                         "autoOpen", false,
			                         "minWidth", width,
			                         "maxWidth", width,
			                         "width", width,
			                         "minHeight", height,
			                         "maxHeight", height,
			                         "height", height,
			                         "resizable", false,
			                         "dialogClass", EffectiveDialogClass,
			                         "open", new JQueryEventHandlerDelegate(delegate { Window.SetTimeout(delegate { OnOpened(EventArgs.Empty); }, 0); }),
			                         "beforeclose", new JQueryEventCancelHandlerDelegate(delegate { CancelEventArgs e = new CancelEventArgs(); OnClosing(e); return !e.Cancel; }),
			                         "close", new JQueryEventHandlerDelegate(delegate { OnClosed(EventArgs.Empty); })
			                         ));
			Utils.Parent(element, ".ui-dialog").lostfocus(frame_LostFocus);
		}

		public virtual void Attach() {
			AttachSelf();
		}

		public void Open() {
			if ((bool)element.dialog("isOpen"))
				element.dialog("close");
				
			CancelEventArgs e = new CancelEventArgs();
			OnOpening(e);
			if (e.Cancel)
				return;

			object[] positionObj = new object[2];
			if (position.anchor == AnchoringEnum.NotPositioned) {
				positionObj[0] = "center";
				positionObj[1] = "middle";
			}
			else {
				jQuery doc = JQueryProxy.jQuery(Window.Document);
				positionObj[0] = position.left - doc.scrollLeft();
				positionObj[1] = position.top - doc.scrollTop();
			}
			
			element.dialog("option", "position", positionObj);
			element.dialog("open");
		}
		
		public bool IsOpen {
			get { return (bool)element.dialog("isOpen"); }
		}
		
		public void Close() {
			element.dialog("close");
		}

		protected virtual void OnOpening(CancelEventArgs e) {
			if (Opening != null)
				Opening(this, e);
		}
		
		protected virtual void OnOpened(EventArgs e) {
			if (Opened != null)
				Opened(this, e);
		}

		protected virtual void OnClosing(CancelEventArgs e) {
			if (Closing != null)
				Closing(this, e);
		}

		protected virtual void OnClosed(EventArgs e) {
			if (Closed != null)
				Closed(this, e);
		}
		
		private void frame_LostFocus(JQueryEvent evt) {
			if (modality == DialogModalityEnum.HideOnFocusOut)
				element.dialog("close");
		}
#endif
	}
	
	public sealed class DialogFrame : DialogBase, IControlHost {
		private string innerHtml;

		public void SetInnerHtml(string html) {
			#if CLIENT
				if (Element != null)
					throw new Exception("Can't change inner HTML after render.");
			#endif
			innerHtml = html;
		}

		protected override string InnerHtml { get { return innerHtml ?? ""; } }

#if SERVER
		public DialogFrame() {
			GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());
		}
#endif

#if CLIENT
		[AlternateSignature]
		public extern DialogFrame();

		public DialogFrame(string id) : base(id) {
		}

		public jQuery GetInnerElements() {
			return Element.children();
		}
#endif
	}
	
	public abstract class ControlDialogBase : DialogBase {
		private IControl containedControl;
		
		public override string Id {
			get { return base.Id; }
			set {
				base.Id = value;
				if (containedControl != null)
					containedControl.Id = value + "_control";
			}
		}
		
		protected IControl GetContainedControlBase() { return containedControl; }

#if SERVER
		protected override void AddItemsToConfigObject(Dictionary<string, object> config) {
			base.AddItemsToConfigObject(config);
			config.Add("containedControlType", containedControl.GetType().FullName);
		}

		protected ControlDialogBase() {
			GlobalServices.Provider.GetService<IScriptManagerService>().RegisterType(GetType());
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
		protected ControlDialogBase(string id) : base(id) {
		}

		protected override void InitConfig(Dictionary config) {
			Type tp = Type.GetType((string)config["containedControlType"]);
			containedControl = (IControl)Type.CreateInstance(tp, Id + "_control");
			base.InitConfig(config);
		}

		protected override string InnerHtml { get { return ((IClientCreateControl)containedControl).Html; } }
		
		protected void SetContainedControlBase(IClientCreateControl control) {
			if (((IControl)control).Element != null)
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
			GlobalServices.Provider.GetService<IScriptManagerService>().RegisterType(GetType());
		}

		public void SetContainedControl(IControl value) {
			SetContainedControlBase(value);
		}
#endif
#if CLIENT
		[AlternateSignature]
		public extern ControlDialog();
		
		public ControlDialog(string id) : base(id) {
		}

		public void SetContainedControl(IClientCreateControl control) {
			SetContainedControlBase(control);
		}
#endif
	}
}