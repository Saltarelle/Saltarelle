using System;
#if SERVER
using System.Collections.Generic;
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
			private jQuery element;
		#endif

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
		
		public string Title {
			get { return title; }
			set {
				#if CLIENT
					if (!Utils.IsNull(element)) {
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

		public void SetInnerHtml(string values) {
			innerHtml = values;
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				string style = PositionHelper.CreateStyle(position, -1, -1);

				return "<fieldset id=\"" + id + "\" class=\"" + ClassName + "\" style=\"" + style + "\""
				#if SERVER
					 + " __cfg=\"" + Utils.HtmlEncode(Utils.Json(ConfigObject)) + "\""
				#endif
				     + ">" + (!string.IsNullOrEmpty(title) ? "<legend>" + Utils.HtmlEncode(title) + "</legend>" : "") + (innerHtml ?? "") + "</fieldset>";
			}
		}

		protected virtual void InitDefault() {
			position = PositionHelper.NotPositioned;
			title = "";
		}

#if SERVER
		public GroupBox() {
			GlobalServices.Provider.GetService<IScriptManagerService>().RegisterType(GetType());
			InitDefault();
		}

		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["title"] = title;
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
		public extern GroupBox();
		public GroupBox(string id) {
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
			Attach();
		}

		public jQuery Element {
			get { return element; }
		}

		public void Attach() {
			if (Utils.IsNull(id) || !Utils.IsNull(element))
				throw new Exception("Must set ID and can only attach once");
			element = JQueryProxy.jQuery("#" + id);
			if (string.IsNullOrEmpty(title))
				element.children(":not(legend)").addClass(NoLegendChildClassName);
		}
		
		public jQuery GetInnerElements() {
			return element.children(":not(legend)");
		}
#endif
	}
}