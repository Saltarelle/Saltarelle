using System;
#if SERVER
using System.Collections.Generic;
#endif
#if CLIENT
using System.Collections;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using jQueryApi;

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
				#if CLIENT
					if (isAttached)
						GetElement().ID = value;
				#endif
				id = value;
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
						var element = jQuery.FromElement(GetElement());
						if (string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(value)) {
							element.Prepend(jQuery.FromHtml("<legend>" + Utils.HtmlEncode(value) + "</legend>")); // add legend
							element.Children().RemoveClass(NoLegendChildClassName);
						}
						else if (!string.IsNullOrEmpty(title) && string.IsNullOrEmpty(value)) {
							element.Children().Eq(0).Remove(); // remove legend
							element.Children(":not(legend)").RemoveClass(NoLegendChildClassName);
						}
						else if (!string.IsNullOrEmpty(value))
							element.Children().Eq(0).Text(value);
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
		public GroupBox() {}
		public GroupBox(object config) {
			if (!Script.IsUndefined(config)) {
				InitConfig(JsDictionary.GetDictionary(config));
			}
			else
				InitDefault();
		}

		protected virtual void InitConfig(JsDictionary config) {
			id    = (string)config["id"];
			title = (string)config["title"];
			Attach();
		}

		public Element GetElement() {
			return isAttached ? Document.GetElementById(id) : null;
		}

		public void Attach() {
			if (Utils.IsNull(id) || isAttached)
				throw new Exception("Must set ID and can only attach once");
			isAttached = true;

			var element = jQuery.FromElement(GetElement());
			if (string.IsNullOrEmpty(title))
				element.Children(":not(legend)").AddClass(NoLegendChildClassName);
		}
		
		public IList<Element> GetInnerElements() {
			var result = new List<Element>();
			ElementCollection children = GetElement().ChildNodes;
			for (int i = 0; i < children.Length; i++) {
				if (children[i].NodeType == ElementType.Element && children[i].TagName.ToUpperCase() != "LEGEND")
					result.Add(children[i]);
			}
			return (Element[])result;
		}
#endif
	}
}