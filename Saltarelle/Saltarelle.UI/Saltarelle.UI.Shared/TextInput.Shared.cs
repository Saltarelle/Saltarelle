using System;
using Saltarelle;
#if SERVER
using System.Collections.Generic;
#endif
#if CLIENT
using InvalidOperationException = System.Exception;
#endif

namespace Saltarelle.UI {
	public class TextInput : IControl, IClientCreateControl {
		private string id;
		private Position position;
		private string value;
		private string cssClass;
		private int width = -1;
		
		#if CLIENT
			private jQuery element;
			public event EventHandler ValueChanged;
		#endif
	
		public Position Position {
			get {
				#if CLIENT
					return !Utils.IsNull(element) ? PositionHelper.GetPosition(element) : position;
				#else
					return position;
				#endif
			}
			set {
				position = value;
				#if CLIENT
					if (!Utils.IsNull(element))
						PositionHelper.ApplyPosition(element, value);
				#endif
			}
		}

		public string Id {
			get { return id; }
			set {
				id = value;
				#if CLIENT
					if (!Utils.IsNull(element))
						element.attr("id", value);
				#endif
			}
		}
		
		public int Width {
			get {
				#if CLIENT
					return !Utils.IsNull(element) ? (int)Math.Round(element.width()) : width;
				#else
					return width;
				#endif
			}
			set {
				width = value;
				#if CLIENT
					if (!Utils.IsNull(element))
						element.width(value > 0 ? Utils.ToStringInvariantInt(value) : "");
				#endif
			}
		}
		
		public string CssClass {
			get { return cssClass; }
			set {
				#if CLIENT
					if (!Utils.IsNull(element)) {
						if (!string.IsNullOrEmpty(cssClass))
							element.removeClass(cssClass);
						if (!string.IsNullOrEmpty(value))
							element.addClass(value);
					}
				#endif
				cssClass = value;
			}
		}
		
		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new InvalidOperationException("Must assign Id before rendering.");
				return "<input " + Utils.IdAndStyle(id, position, width, -1, null) + " type=\"text\" class=\"" + (cssClass ?? "") + "\" value=\"" + Utils.HtmlEncode(value ?? "") + "\""
				#if SERVER
				     + "__cfg=\"" + Utils.HtmlEncode(Utils.Json(ConfigObject)) + "\""
				#endif
				     + "/>";
			}
		}
		
		public string Value {
			get {
				#if CLIENT
					return !Utils.IsNull(element) ? element.val() : value;
				#else
					return value;
				#endif
			}
			set {
				this.value = value ?? "";
				#if CLIENT
					if (!Utils.IsNull(element))
						element.val(value);
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
			GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());
			InitDefault();
		}

		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["cssClass"] = cssClass;
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
		public extern TextInput();
		
		public TextInput(string id) {
			if (!string.IsNullOrEmpty(id)) {
				this.id = id;
				Dictionary config = (Dictionary)Utils.EvalJson((string)JQueryProxy.jQuery("#" + id).attr("__cfg"));
				Attach();
			}
			else
				InitDefault();
		}

		protected virtual void InitConfig(Dictionary config) {
			cssClass = (string)config["cssClass"];
			Attach();
		}

		public jQuery Element {
			get { return element; }
		}

		public void Attach() {
			element = JQueryProxy.jQuery("#" + id);
			element.change(element_Change);
		}
		
		private void element_Change(JQueryEvent e) {
			OnValueChanged(EventArgs.Empty);
		}
		
		protected virtual void OnValueChanged(EventArgs e) {
			if (!Utils.IsNull(ValueChanged))
				ValueChanged(this, e);
		}
#endif
	}
}