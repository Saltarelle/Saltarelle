using System;
#if SERVER
using System.Collections.Generic;
#endif
#if CLIENT
using System.DHTML;
#endif

namespace Saltarelle.UI {
	public class GroupBox : IControl, IClientCreateControl, IControlHost {
		public const string ClassName = "GroupBox";
		public const string NoLegendChildClassName = "GroupBoxNoLegendChild";
		private string id;
		private Position position;
		private string title;
		private string innerHtml;

		#if CLIENT
			private bool isAttached = false;
		#endif

		public string Id {
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
		
		public string Title {
			get { return title; }
			set {
				#if CLIENT
					if (isAttached) {
						jQuery element = JQueryProxy.jQuery(GetElement());
						if (string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(value)) {
							element.prepend(JQueryProxy.jQuery("<legend>" + Utils.HtmlEncode(value) + "</legend>")); // add legend
							element.children().removeClass(NoLegendChildClassName);
						}
						else if (!string.IsNullOrEmpty(title) && string.IsNullOrEmpty(value)) {
							element.children().eq(0).remove(); // remove legend
							element.children(":not(legend)").removeClass(NoLegendChildClassName);
						}
						else if (!string.IsNullOrEmpty(value))
							element.children().eq(0).text(value);
					}
				#endif
			
				title = value;
			}
		}

		public void SetInnerFragments(string[] fragments) {
			innerHtml = Utils.JoinStrings("", fragments);
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				string style = PositionHelper.CreateStyle(position, -1, -1);

				return "<fieldset id=\"" + id + "\" class=\"" + ClassName + "\" style=\"" + style + "\"" + ">" + (!string.IsNullOrEmpty(title) ? "<legend>" + Utils.HtmlEncode(title) + "</legend>" : "") + (innerHtml ?? "") + "</fieldset>";
			}
		}

		protected virtual void InitDefault() {
			position = PositionHelper.NotPositioned;
			title = "";
		}

#if SERVER
		public GroupBox() {
			GlobalServices.Provider.GetService<IScriptManagerService>().RegisterClientType(GetType());
			InitDefault();
		}

		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["id"] = id;
			config["title"] = title;
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
		public extern GroupBox();
		public GroupBox(object config) {
			if (!Script.IsUndefined(config)) {
				InitConfig(Dictionary.GetDictionary(config));
			}
			else
				InitDefault();
		}

		protected virtual void InitConfig(Dictionary config) {
			id    = (string)config["id"];
			title = (string)config["title"];
			Attach();
		}

		public DOMElement GetElement() {
			return isAttached ? Document.GetElementById(id) : null;
		}

		public void Attach() {
			if (Utils.IsNull(id) || isAttached)
				throw new Exception("Must set ID and can only attach once");
			isAttached = true;

			jQuery element = JQueryProxy.jQuery(GetElement());
			if (string.IsNullOrEmpty(title))
				element.children(":not(legend)").addClass(NoLegendChildClassName);
		}
		
		public DOMElement[] GetInnerElements() {
			ArrayList result = new ArrayList();
			DOMElementCollection children = GetElement().ChildNodes;
			for (int i = 0; i < children.Length; i++) {
				if (children[i].NodeType == DOMElementType.Element && children[i].TagName != "LEGEND")
					result.Add(children[i]);
			}
			return (DOMElement[])result;
		}
#endif
	}
}