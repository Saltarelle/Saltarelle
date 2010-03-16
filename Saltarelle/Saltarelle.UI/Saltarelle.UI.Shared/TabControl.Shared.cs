using System;
#if SERVER
using System.Text;
using System.Collections.Generic;
#endif
#if CLIENT
using System.DHTML;
#endif

namespace Saltarelle.UI {
	#if CLIENT
		public class TabControlSelectedTabChangingEventArgs : EventArgs {
			public bool Cancel;
			public int OldIndex;
			public int NewIndex;
		}
	
		public delegate void TabControlSelectedTabChangingEventHandler(object sender, TabControlSelectedTabChangingEventArgs e);
	#endif

	public class TabControl : IControl, IClientCreateControl, IControlHost {
		private string id;
		private Position position;
		private string[] tabCaptions;
		private int selectedTabIfNotRendered;
		private string innerHtml;
		private bool rightAlignTabs;
		
		#if CLIENT
			private jQuery element;
			
			public event TabControlSelectedTabChangingEventHandler SelectedTabChanging;
			public event EventHandler SelectedTabChanged;
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

		public string[] TabCaptions {
			get { return tabCaptions; }
			set {
				#if CLIENT
					if (!Utils.IsNull(element)) {
						// Rendered means no changing number of tabs.
						int oldNum = tabCaptions.Length;
						tabCaptions = new string[oldNum];
						for (int i = 0; i < oldNum; i++)
							tabCaptions[i] = (i < value.Length ? value[i] : null) ?? "";

						element.children(":eq(0)").children().each(delegate(int i, DOMElement d) {
							string s = tabCaptions[rightAlignTabs ? tabCaptions.Length - i - 1 : i];
							JQueryProxy.jQuery(d).children().html(!string.IsNullOrEmpty(s) ? Utils.HtmlEncode(s) : "&nbsp;");
							return true;
						});
						return;
					}
				#endif
				tabCaptions = value;
			}
		}
		
		public bool RightAlignTabs {
			get { return rightAlignTabs; }
			set {
				#if CLIENT
					if (!Utils.IsNull(element) && value != rightAlignTabs) {
						selectedTabIfNotRendered = SelectedTab;
						element.tabs("destroy");
						rightAlignTabs = value;
						element.children(":eq(0)").html(TabsInnerHtml);
						Tabify();
						return;
					}
				#endif
				rightAlignTabs = value;
			}
		}
		
		public void SetInnerHtml(string html) {
			innerHtml = html;
		}
		
		public int SelectedTab {
			get {
				#if CLIENT
					if (!Utils.IsNull(element)) {
						int s = (int)element.tabs("option", "selected");
						return rightAlignTabs ? (tabCaptions.Length - 1 - s) : s;
					}
				#endif
				return selectedTabIfNotRendered;
			}
			set {
				#if CLIENT
					if (!Utils.IsNull(element))
						element.tabs("select", rightAlignTabs ? (tabCaptions.Length - 1 - value) : value);
				#endif
				selectedTabIfNotRendered = value;
			}
		}
		
		public string TabsInnerHtml {
			get {
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < tabCaptions.Length; i++) {
					int idx = rightAlignTabs ? tabCaptions.Length - i - 1 : i;
					sb.Append("<li" + (rightAlignTabs ? " style=\"float: right\"" : "") + "><a href=\"#_" + id + "-" + Utils.ToStringInvariantInt(idx + 1) + "\">" + (!string.IsNullOrEmpty(tabCaptions[idx]) ? Utils.HtmlEncode(tabCaptions[idx]) : "&nbsp;") + "</a></li>");
				}
				return sb.ToString();
			}
		}
		
		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				string style = PositionHelper.CreateStyle(position, -1, -1);

				StringBuilder sb = new StringBuilder();
				sb.Append("<div id=\"" + id + "\" style=\"" + style + "\"");
				#if SERVER
				     sb.Append(" __cfg=\"" + Utils.HtmlEncode(Utils.Json(ConfigObject)) + "\"");
				#endif
				sb.Append("><ul>");
				sb.Append(TabsInnerHtml);
				sb.Append("</ul>");
				sb.Append(innerHtml ?? "");
				sb.Append("</div>");
				return sb.ToString();
			}
		}

		protected virtual void InitDefault() {
			position    = PositionHelper.NotPositioned;
			tabCaptions = new string[] {};
		}

#if SERVER
		public TabControl() {
			GlobalServices.Provider.GetService<IScriptManagerService>().RegisterType(GetType());
			InitDefault();
		}

		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["tabCaptions"] = tabCaptions;
			config["selectedTab"] = selectedTabIfNotRendered;
			config["rightAlignTabs"] = rightAlignTabs;
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
		public extern TabControl();
		public TabControl(string id) {
			if (!Script.IsUndefined(id)) {
				this.id = id;
				Dictionary config = (Dictionary)Utils.EvalJson((string)JQueryProxy.jQuery("#" + id).attr("__cfg"));
				InitConfig(config);
			}
			else
				InitDefault();
		}

		protected virtual void InitConfig(Dictionary config) {
			tabCaptions = (string[])config["tabCaptions"];
			position = (Position)config["position"];
			selectedTabIfNotRendered = (int)config["selectedTab"];
			rightAlignTabs = (bool)config["rightAlignTabs"];
			Attach();
		}

		public void Attach() {
			if (Utils.IsNull(id) || !Utils.IsNull(element))
				throw new Exception("Must set ID and can only attach once");
		
			element = JQueryProxy.jQuery("#" + id);
			element.children(":gt(0)").wrap("<div style=\"position: relative\"/>");

			Tabify();
		}
		
		protected void Tabify() {
			element.children(":gt(0)").each(new EachCallback(delegate(int idx, DOMElement elem) {
				elem.ID = "_" + id + "-" + Utils.ToStringInvariantInt(idx + 1);
				jQuery q = JQueryProxy.jQuery(elem);
				if (jQuery.browser.msie && (jQuery.browser.version == "6.0" || jQuery.browser.version == "7.0")) {
					// Required to work with IE6/7, but messes up the looks in the designer in IE8.
					q.width(Math.Round(q.width()));
					q.height(Math.Round(q.height()));
				}
				return true;
			}));

			element.tabs(new Dictionary("selected", selectedTabIfNotRendered,
			                            "select", new TabsSelectEventHandlerDelegate(el_select),
			                            "show", new TabsEventHandlerDelegate(el_show)
			));
			if (rightAlignTabs) {
				// JQuery UI has a bug which causes the selected option to not work correctly when we reversed the tabs if we use the "selected" option in this case.
				element.tabs("select", tabCaptions.Length - 1 - selectedTabIfNotRendered);
			}
		}

		public jQuery Element {
			get { return element; }
		}
		
		public jQuery GetInnerElements() {
			return element.children(":gt(0)").children();
		}
		
		private void el_show(JQueryEvent evt, TabsEventObject ui) {
			OnSelectedTabChanged(EventArgs.Empty);
		}
		
		private bool el_select(JQueryEvent evt, TabsEventObject ui) {
			TabControlSelectedTabChangingEventArgs e = new TabControlSelectedTabChangingEventArgs();
			e.Cancel = false;
			e.OldIndex = SelectedTab;
			e.NewIndex = (rightAlignTabs ? (tabCaptions.Length - 1 - ui.index) : ui.index);
			OnSelectedTabChanging(e);
			return !e.Cancel;
		}
		
		protected virtual void OnSelectedTabChanging(TabControlSelectedTabChangingEventArgs e) {
			if (!Utils.IsNull(SelectedTabChanging))
				SelectedTabChanging(this, e);
		}

		protected virtual void OnSelectedTabChanged(EventArgs e) {
			if (!Utils.IsNull(SelectedTabChanged))
				SelectedTabChanged(this, e);
		}
		
		public void AddTab(string title, string html, int position) {
			element.tabs("destroy");

			tabCaptions = (string[])(position > 0 ? tabCaptions.Extract(0, position) : new string[0]).Concat(title).Concat(tabCaptions.Extract(position));	// Script# bug/misfeature makes Extract() return the whole array if 0 is specified for count
			element.children().eq(0).html(TabsInnerHtml);

			jQuery q = JQueryProxy.jQuery("<div style=\"position: relative\">" + html + "</div>");
			element.children().eq(position).after(q);	// use this index since we have the tab caption list before everything else.

			Tabify();
		}

		public void RemoveTab(int position) {
			element.tabs("destroy");
			tabCaptions = (string[])(position > 0 ? tabCaptions.Extract(0, position) : new string[0]).Concat(tabCaptions.Extract(position + 1));	// Script# bug/misfeature makes Extract() return the whole array if 0 is specified for count
			element.children().eq(0).html(TabsInnerHtml);
			element.children().eq(position + 1).remove();
			Tabify();
		}
#endif
	}
}