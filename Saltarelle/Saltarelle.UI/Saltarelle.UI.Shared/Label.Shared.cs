using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Saltarelle;
#if CLIENT
using System.Html;
using jQueryApi;
#endif

namespace Saltarelle.UI {
	public class Label : IControl, IClientCreateControl {
		public const string ClassName = "Label";

		private string id;
		private Position position;
		private string text;
		private string additionalClass;

		#if CLIENT
			private bool isAttached = false;
		#endif
		
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
		
		public string Text {
			get {
				#if CLIENT
					if (isAttached)
						return GetElement().InnerText;
				#endif
				return text;
			}
			set {
				#if CLIENT
					if (isAttached)
						GetElement().InnerText = value;
				#endif
				text = value;
			}
		}
		
		public string AdditionalClass {
			get { return additionalClass; }
			set {
				#if CLIENT
					if (isAttached) {
						var element = jQuery.FromElement(GetElement());
						if (!string.IsNullOrEmpty(additionalClass))
							element.RemoveClass(additionalClass);
						if (!string.IsNullOrEmpty(value))
							element.AddClass(value);
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
				return "<span class=\"" + ClassName + (!string.IsNullOrEmpty(additionalClass) ? " " + additionalClass : "") + "\" id=\"" + id + "\" style=\"" + style + "\">" + Utils.HtmlEncode(!string.IsNullOrEmpty(text) ? text : "&nbsp") + "</span>";
			}
		}

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
		
		protected virtual void InitDefault() {
			position = PositionHelper.NotPositioned;
		}

#if SERVER
		public Label() {
			InitDefault();
		}

		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["id"] = id;
			config["additionalClass"] = additionalClass;
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
		public Label() {}
		public Label(object config) {
			if (!Script.IsUndefined(config)) {
				InitConfig(JsDictionary.GetDictionary(config));
			}
			else
				InitDefault();
		}

		protected virtual void InitConfig(JsDictionary config) {
			id = (string)config["id"];
			additionalClass = (string)config["additionalClass"];
			Attach();
		}

		public Element GetElement() { return isAttached ? Document.GetElementById(id) : null; }

		public void Attach() {
			if (id == null || isAttached)
				throw new Exception("Must set ID and can only attach once");
			isAttached = true;
		}
#endif
	}
}
