using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Saltarelle;
#if SERVER
using System.Collections.Generic;
#endif
#if CLIENT
using jQueryApi;
using InvalidOperationException = System.Exception;
using System.Html;
#endif

namespace Saltarelle.UI {
	public class TextInput : IControl, IClientCreateControl {
		private string id;
		private Position position;
		private string value;
		private string cssClass;
		private int width = -1;
		
		#if CLIENT
			private bool isAttached = false;
			public event EventHandler ValueChanged;
		#endif
	
		public Position Position {
			get {
				#if CLIENT
					return isAttached ? PositionHelper.GetPosition(GetElement()) : position;
				#else
					return position;
				#endif
			}
			set {
				position = value;
				#if CLIENT
					if (isAttached)
						PositionHelper.ApplyPosition(GetElement(), value);
				#endif
			}
		}

		public string Id {
			get { return id; }
			set {
				#if CLIENT
					if (isAttached)
						GetElement().ID = value;
				#endif
				id = value;
			}
		}
		
		public int Width {
			get {
				#if CLIENT
					return isAttached ? (int)Math.Round(jQuery.FromElement(GetElement()).GetWidth()) : width;
				#else
					return width;
				#endif
			}
			set {
				width = value;
				#if CLIENT
					if (isAttached)
						jQuery.FromElement(GetElement()).Width(value > 0 ? Utils.ToStringInvariantInt(value) : "");
				#endif
			}
		}
		
		public string CssClass {
			get { return cssClass; }
			set {
				#if CLIENT
					if (isAttached) {
						var element = jQuery.FromElement(GetElement());
						if (!string.IsNullOrEmpty(cssClass))
							element.RemoveClass(cssClass);
						if (!string.IsNullOrEmpty(value))
							element.AddClass(value);
					}
				#endif
				cssClass = value;
			}
		}
		
		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new InvalidOperationException("Must assign Id before rendering.");
				return "<input " + Utils.IdAndStyle(id, position, width, -1, null) + " type=\"text\" class=\"" + (cssClass ?? "") + "\" value=\"" + Utils.HtmlEncode(value ?? "") + "\"/>";
			}
		}
		
		public string Value {
			get {
				#if CLIENT
					return isAttached ? ((InputElement)GetElement()).Value : value;
				#else
					return value;
				#endif
			}
			set {
				this.value = value ?? "";
				#if CLIENT
					if (isAttached)
						((InputElement)GetElement()).Value = this.value;
					OnValueChanged(EventArgs.Empty);
				#endif
			}
		}

		protected virtual void InitDefault() {
			position = PositionHelper.NotPositioned;
			value    = "";
		}

#if SERVER
		public TextInput() {
			InitDefault();
		}

		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["id"]       = id;
			config["cssClass"] = cssClass;
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
		[AlternateSignature]
		public TextInput() {}
		
		public TextInput(object config) {
			if (!Script.IsUndefined(config)) {
				InitConfig(JsDictionary.GetDictionary(config));
			}
			else
				InitDefault();
		}

		protected virtual void InitConfig(JsDictionary config) {
			id = (string)config["id"];
			cssClass = (string)config["cssClass"];
			Attach();
		}

		public Element GetElement() { return isAttached ? Document.GetElementById(id) : null; }

		public void Attach() {
			isAttached = true;
			jQuery.FromElement(GetElement()).Change(element_Change);
		}
		
		private void element_Change(jQueryEvent e) {
			OnValueChanged(EventArgs.Empty);
		}
		
		protected virtual void OnValueChanged(EventArgs e) {
			if (ValueChanged != null)
				ValueChanged(this, e);
		}
#endif
	}
}