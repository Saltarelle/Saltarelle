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
		private const string MainClass        = "ui-tabs ui-widget ui-widget-content ui-corner-all";
		private const string TabBarClass      = "ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all";
		private const string ActiveTabClass   = "ui-corner-top ui-tabs-selected ui-state-active";
		private const string InactiveTabClass = "ui-state-default ui-corner-top";
		private const string TabPageClass     = "ui-tabs-panel ui-widget-content ui-corner-bottom";
	
		private string id;
		private Position position;
		private string[] tabCaptions;
		private int selectedTab;
		private string[] innerFragments;
		private bool rightAlignTabs;
		
		#if CLIENT
			private bool isAttached = false;
			
			public event TabControlSelectedTabChangingEventHandler SelectedTabChanging;
			public event EventHandler SelectedTabChanged;
			
			private JQueryEventHandlerDelegate clickHandler;
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
						
						DOMElementCollection children = GetElement().Children[0].Children[0].Children;
						for (int i = 0; i < children.Length; i++)
							children[i].Children[0].InnerHTML = (!string.IsNullOrEmpty(tabCaptions[i]) ? Utils.HtmlEncode(tabCaptions[i]) : "&nbsp");
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
						DOMElement elem = GetElement();
						JQueryProxy.jQuery(elem.Children[0].Children[0]).css("float", (value ? "right" : "left"));
					}
				#endif
				rightAlignTabs = value;
			}
		}
		
		public void SetInnerFragments(string[] fragments) {
			innerFragments = fragments ?? new string[0];
		}
		
		public int SelectedTab {
			get {
				return selectedTab;
			}
			set {
				#if CLIENT
					if (value == selectedTab)
						return;
					TabControlSelectedTabChangingEventArgs e = new TabControlSelectedTabChangingEventArgs();
					e.OldIndex = selectedTab;
					e.NewIndex = value;
					e.Cancel = false;
					OnSelectedTabChanging(e);
					if (e.Cancel)
						return;

					if (isAttached) {
						ChangeSelectionUI(selectedTab, value);
					}
					selectedTab = value;

					OnSelectedTabChanged(EventArgs.Empty);
				#else
					selectedTab = value;
				#endif
			}
		}
		
		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				string style = PositionHelper.CreateStyle(position, -1, -1);

				StringBuilder sb = new StringBuilder();
				sb.Append("<div id=\"" + id + "\" style=\"" + style + "\" class=\"" + MainClass + "\"><div class=\"" + TabBarClass + "\"><ul style=\"float: " + (rightAlignTabs ? "right" : "left") + "\">");
				for (int i = 0; i < tabCaptions.Length; i++) {
					sb.Append(GetButtonHtml(tabCaptions[i], i == selectedTab));
				}
				sb.Append("</ul></div>");
				for (int i = 0; i < innerFragments.Length; i++) {
					sb.Append(GetTabPageHtml(innerFragments[i]));
				}
				sb.Append("</div>");
				return sb.ToString();
			}
		}

		protected virtual void InitDefault() {
			position       = PositionHelper.NotPositioned;
			selectedTab    = 0;
			tabCaptions    = new string[0];
			innerFragments = new string[0];
		}
		
		private string GetTabPageHtml(string content) {
			return "<div style=\"position: relative; *width: 1px; zoom: 1\" class=\"" + TabPageClass + "\">" + content + "</div>";	// zoom:1 is required for IE6, otherwise content might be invisible.
		}
		
		private string GetButtonHtml(string text, bool active) {
			return "<li class=\"" + (active ? ActiveTabClass : InactiveTabClass) + "\"><a href=\"#\">" + (!string.IsNullOrEmpty(text) ? Utils.HtmlEncode(text) : "&nbsp;") + "</a></li>";
		}

		public void AddTab(string title, string html, int position) {
			if (position < 0 || position > tabCaptions.Length)
				throw Utils.ArgumentException("position");

			#if CLIENT
			if (isAttached) {
				string[] newCaptions = new string[tabCaptions.Length + 1];
				for (int i = 0; i < position; i++) {
					newCaptions[i] = tabCaptions[i];
				}
				newCaptions[position] = title;
				for (int i = position; i < tabCaptions.Length; i++) {
					newCaptions[i + 1] = tabCaptions[i];
				}
				tabCaptions = newCaptions;

				DOMElement elem = GetElement();
				jQuery newPage = JQueryProxy.jQuery(GetTabPageHtml(html));
				JQueryProxy.jQuery(elem.Children[position]).after(newPage);	// use this index since we have the tab caption list before everything else.
				newPage.css("display", "none");
				
				jQuery newButton = JQueryProxy.jQuery(GetButtonHtml(title, false));
				newButton.children().click(clickHandler);
				if (position == 0)
					JQueryProxy.jQuery(elem.Children[0].Children[0]).prepend(newButton);
				else
					JQueryProxy.jQuery(elem.Children[0].Children[0].Children[position - 1]).after(newButton);
			}
			else {
			#endif
				string[] newCaptions = new string[tabCaptions.Length + 1], newFragments = new string[tabCaptions.Length + 1];
				for (int i = 0; i < position; i++) {
					newCaptions[i]  = tabCaptions[i];
					newFragments[i] = innerFragments[i];
				}
				newCaptions[position]  = title;
				newFragments[position] = html;
				for (int i = position; i < tabCaptions.Length; i++) {
					newCaptions[i + 1]  = tabCaptions[i];
					newFragments[i + 1] = innerFragments[i];
				}
				tabCaptions    = newCaptions;
				innerFragments = newFragments;
			#if CLIENT
			}
			#endif

			if (selectedTab >= position)
				selectedTab++;
		}

		public void RemoveTab(int position) {
			if (position < 0 || position >= tabCaptions.Length)
				throw Utils.ArgumentException("position");
			if (tabCaptions.Length < 2)
				throw Utils.ArgumentException("Cannot remove last tab");

			#if CLIENT
			if (isAttached) {
				string[] newCaptions = new string[tabCaptions.Length - 1];
				for (int i = 0; i < position; i++) {
					newCaptions[i] = tabCaptions[i];
				}
				for (int i = position + 1; i < tabCaptions.Length; i++) {
					newCaptions[i - 1] = tabCaptions[i];
				}
				tabCaptions = newCaptions;
				
				DOMElement elem = GetElement();
				elem.RemoveChild(elem.Children[position + 1]);
				elem.Children[0].Children[0].RemoveChild(elem.Children[0].Children[0].Children[position]);

				if (selectedTab > position)
					selectedTab--;
				else if (selectedTab == position) {
					int newSel = Math.Min(selectedTab, tabCaptions.Length);
					ChangeSelectionUI(-1, newSel);
					selectedTab = newSel;
				}
			}
			else {
			#endif
				string[] newCaptions = new string[tabCaptions.Length - 1], newFragments = new string[tabCaptions.Length - 1];
				for (int i = 0; i < position; i++) {
					newCaptions[i]  = tabCaptions[i];
					newFragments[i] = innerFragments[i];
				}
				for (int i = position + 1; i < tabCaptions.Length; i++) {
					newCaptions[i - 1]  = tabCaptions[i];
					newFragments[i - 1] = innerFragments[i];
				}
				tabCaptions    = newCaptions;
				innerFragments = newFragments;

				if (selectedTab > position)
					selectedTab--;
				else if (selectedTab == position)
					selectedTab = Math.Min(selectedTab, tabCaptions.Length);
			#if CLIENT
			}
			#endif
		}

#if SERVER
		public TabControl() {
			InitDefault();
		}

		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["id"]             = id;
			config["tabCaptions"]    = tabCaptions;
			config["selectedTab"]    = selectedTab;
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
			selectedTab = (int)config["selectedTab"];
			rightAlignTabs = (bool)config["rightAlignTabs"];
			Attach();
		}
		
		private void ChangeSelectionUI(int oldSelection, int newSelection) {
			DOMElementCollection children = GetElement().Children;
			if (oldSelection != -1) {
				children[oldSelection + 1].Style.Display = "none";
				children[0].Children[0].Children[oldSelection].ClassName = InactiveTabClass;
			}
			children[newSelection + 1].Style.Display = "";
			children[0].Children[0].Children[newSelection].ClassName = ActiveTabClass;
		}

		private void Link_Click(DOMElement el, JQueryEvent evt) {
			evt.preventDefault();
			DOMElement li = el.ParentNode;
			DOMElementCollection lis = li.ParentNode.Children;
			for (int i = 0; i < lis.Length; i++) {
				if (li == lis[i]) {
					SelectedTab = i;
					return;
				}
			}
		}

		public void Attach() {
			if (Utils.IsNull(id) || isAttached)
				throw new Exception("Must set ID and can only attach once");
			isAttached = true;

			UIUtils.FixStrangeIE7SelectIssue(GetElement().Children[SelectedTab + 1]);
			
			clickHandler = (JQueryEventHandlerDelegate)Utils.Wrap(new UnwrappedJQueryEventHandlerDelegate(Link_Click));
			
			DOMElement elem = GetElement();
			DOMElementCollection children = elem.Children;
			for (int i = 0; i < tabCaptions.Length; i++) {
				if (i != selectedTab)
					children[i + 1].Style.Display = "none";
			}
			
			ArrayList links = new ArrayList();
			DOMElementCollection lis = children[0].Children[0].Children;
			for (int i = 0; i < lis.Length; i++)
				links.Add(lis[i].Children[0]);

			JQueryProxy.jQuery((DOMElement[])links).click(clickHandler);
			
			if (jQuery.browser.msie && Utils.ParseDouble(jQuery.browser.version) < 8) {
				elem.Style.Width = children[1].ClientWidth + "px";
			}
		}
		
		public DOMElement GetElement() { return isAttached ? Document.GetElementById(id) : null; }

		public DOMElement[] GetInnerElements() {
			ArrayList result = new ArrayList();
			jQuery children = JQueryProxy.jQuery(GetElement()).children(":gt(0)").children();
			for (int i = 0; i < children.size(); i++)
				result.Add(children.get(i));
			return (DOMElement[])result;
		}
		
		protected virtual void OnSelectedTabChanging(TabControlSelectedTabChangingEventArgs e) {
			if (!Utils.IsNull(SelectedTabChanging))
				SelectedTabChanging(this, e);
		}

		protected virtual void OnSelectedTabChanged(EventArgs e) {
			UIUtils.FixStrangeIE7SelectIssue(GetElement().Children[SelectedTab + 1]);
			if (!Utils.IsNull(SelectedTabChanged))
				SelectedTabChanged(this, e);
		}
#endif
	}
}