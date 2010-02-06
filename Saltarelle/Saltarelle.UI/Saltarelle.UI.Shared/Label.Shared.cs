using System;
using Saltarelle;
#if SERVER
using System.Collections.Generic;
#endif

namespace Saltarelle.UI {
	public class Label : IControl, IClientCreateControl {
		public const string ClassName = "Label";

		private string id;
		private Position position;
		private string text;
		private string additionalClass;

		#if CLIENT
			private jQuery element;
		#endif
		
		public string Id {
			get { return id; }
			set {
				id = value;
				#if CLIENT
					if (element != null)
						element.attr("id", value);
				#endif
			}
		}
		
		public string Text {
			get {
				#if CLIENT
					if (element != null)
						return element.text();
				#endif
				return text;
			}
			set {
				#if CLIENT
					if (element != null)
						element.text(value);
				#endif
				text = value;
			}
		}
		
		public string AdditionalClass {
			get { return additionalClass; }
			set {
				#if CLIENT
					if (element != null) {
						if (!string.IsNullOrEmpty(additionalClass))
							element.removeClass(additionalClass);
						if (!string.IsNullOrEmpty(value))
							element.addClass(value);
					}
				#endif
				additionalClass = value;
			}
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				string style = PositionHelper.CreateStyle(position, -1, -1);
				return "<span class=\"" + ClassName + (!string.IsNullOrEmpty(additionalClass) ? " " + additionalClass : "") + "\" id=\"" + id + "\" style=\"" + style + "\""
				#if SERVER
					 + " __cfg=\"" + Utils.HtmlEncode(Utils.Json(ConfigObject)) + "\""
				#endif
				     + ">" + Utils.HtmlEncode(!string.IsNullOrEmpty(text) ? text : "&nbsp") + "</span>";
			}
		}

		public Position Position {
			get {
				#if CLIENT
					return element != null ? PositionHelper.GetPosition(element) : position;
				#else
					return position;
				#endif
			}
			set {
				position = value;
				#if CLIENT
					if (element != null)
						PositionHelper.ApplyPosition(element, value);
				#endif
			}
		}
		
		protected virtual void InitDefault() {
			position = PositionHelper.NotPositioned;
		}

#if SERVER
		public Label() {
			GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());
			InitDefault();
		}

		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["additionalClass"] = additionalClass;
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
		public extern Label();
		public Label(string id) {
			if (!Script.IsUndefined(id)) {
				this.id = id;
				Dictionary config = (Dictionary)Utils.EvalJson((string)JQueryProxy.jQuery("#" + id).attr("__cfg"));
				InitConfig(config);
			}
			else
				InitDefault();
		}

		protected virtual void InitConfig(Dictionary config) {
			additionalClass = (string)config["additionalClass"];
			Attach();
		}

		public jQuery Element { get { return element; } }

		public void Attach() {
			if (id == null || element != null)
				throw new Exception("Must set ID and can only attach once");
		
			element = JQueryProxy.jQuery("#" + id);
		}
#endif
	}
}