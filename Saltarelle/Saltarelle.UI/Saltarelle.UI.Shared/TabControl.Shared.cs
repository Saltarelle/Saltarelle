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
			private bool isAttached = false;
			private jQuery tabs;
			
			public event TabControlSelectedTabChangingEventHandler SelectedTabChanging;
			public event EventHandler SelectedTabChanged;
		#endif
	
		public string Id {
			get { return id; }
			set {
				id = value;
				#if CLIENT
					if (isAttached)
						tabs.attr("id", value);
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

		public string[] TabCaptions {
			get { return tabCaptions; }
			set {
				#if CLIENT
					if (isAttached) {
						// Rendered means no changing number of tabs.
						int oldNum = tabCaptions.Length;
						tabCaptions = new string[oldNum];
						for (int i = 0; i < oldNum; i++)
							tabCaptions[i] = (i < value.Length ? value[i] : null) ?? "";

						tabs.children(":eq(0)").children().each(delegate(int i, DOMElement d) {
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
					if (isAttached && value != rightAlignTabs) {
						selectedTabIfNotRendered = SelectedTab;
						tabs.tabs("destroy");
						rightAlignTabs = value;
						tabs.children(":eq(0)").html(TabsInnerHtml);
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
					if (isAttached) {
						int s = (int)tabs.tabs("option", "selected");
						return rightAlignTabs ? (tabCaptions.Length - 1 - s) : s;
					}
				#endif
				return selectedTabIfNotRendered;
			}
			set {
				#if CLIENT
					if (isAttached)
						tabs.tabs("select", rightAlignTabs ? (tabCaptions.Length - 1 - value) : value);
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
				sb.Append("<div id=\"" + id + "\" style=\"" + style + "\"><ul>");
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
			config["id"]             = id;
			config["tabCaptions"]    = tabCaptions;
			config["selectedTab"]    = selectedTabIfNotRendered;
			config["rightAlignTabs"] = rightAlignTabs;
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
		public extern TabControl();
		public TabControl(object config) {
			if (!Script.IsUndefined(config)) {
				InitConfig(Dictionary.GetDictionary(config));
			}
			else
				InitDefault();
		}

		protected virtual void InitConfig(Dictionary config) {
			id = (string)config["id"];
			tabCaptions = (string[])config["tabCaptions"];
			position = (Position)config["position"];
			selectedTabIfNotRendered = (int)config["selectedTab"];
			rightAlignTabs = (bool)config["rightAlignTabs"];
			Attach();
		}

		public void Attach() {
			if (Utils.IsNull(id) || isAttached)
				throw new Exception("Must set ID and can only attach once");
			isAttached = true;
			
			tabs = JQueryProxy.jQuery(GetElement());
			tabs.children(":gt(0)").wrap("<div style=\"position: relative\"/>");

			Tabify();
		}
		
		protected void Tabify() {
			tabs.children(":gt(0)").each(new EachCallback(delegate(int idx, DOMElement elem) {
				elem.ID = "_" + id + "-" + Utils.ToStringInvariantInt(idx + 1);
				jQuery q = JQueryProxy.jQuery(elem);
				if (jQuery.browser.msie && (jQuery.browser.version == "6.0" || jQuery.browser.version == "7.0")) {
					// Required to work with IE6/7, but messes up the looks in the designer in IE8.
					q.width(Math.Round(q.width()));
					q.height(Math.Round(q.height()));
				}
				return true;
			}));

			tabs.tabs(new Dictionary("selected", selectedTabIfNotRendered,
			                         "select", new TabsSelectEventHandlerDelegate(el_select),
			                         "show", new TabsEventHandlerDelegate(el_show)
			));
			if (rightAlignTabs) {
				// JQuery UI has a bug which causes the selected option to not work correctly when we reversed the tabs if we use the "selected" option in this case.
				tabs.tabs("select", tabCaptions.Length - 1 - selectedTabIfNotRendered);
			}
		}

		public DOMElement GetElement() { return isAttached ? Document.GetElementById(id) : null; }
		
		public DOMElement[] GetInnerElements() {
			ArrayList result = new ArrayList();
			jQuery children = tabs.children(":gt(0)").children();
			for (int i = 0; i < children.size(); i++)
				result.Add(children.get(i));
			return (DOMElement[])result;
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
			if (isAttached) {
				tabs.tabs("destroy");

				tabCaptions = (string[])(position > 0 ? tabCaptions.Extract(0, position) : new string[0]).Concat(title).Concat(tabCaptions.Extract(position));	// Script# bug/misfeature makes Extract() return the whole array if 0 is specified for count
				tabs.children().eq(0).html(TabsInnerHtml);

				jQuery q = JQueryProxy.jQuery("<div style=\"position: relative\">" + html + "</div>");
				tabs.children().eq(position).after(q);	// use this index since we have the tab caption list before everything else.

				Tabify();
			}
			else {
				if (position != tabCaptions.Length)
					throw Utils.ArgumentException("When adding a tab before render, the new tab must be the last one.");
				tabCaptions = (string[])Utils.ArrayAppend(tabCaptions, title);
				innerHtml += html;
			}
		}

		public void RemoveTab(int position) {
			if (!isAttached)
				throw new Exception("Cannot remove tab before render.");
			tabs.tabs("destroy");
			tabCaptions = (string[])(position > 0 ? tabCaptions.Extract(0, position) : new string[0]).Concat(tabCaptions.Extract(position + 1));	// Script# bug/misfeature makes Extract() return the whole array if 0 is specified for count
			tabs.children().eq(0).html(TabsInnerHtml);
			tabs.children().eq(position + 1).remove();
			Tabify();
		}
#endif
	}
}