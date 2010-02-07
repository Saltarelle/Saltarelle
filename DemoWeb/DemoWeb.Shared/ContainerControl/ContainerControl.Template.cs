#if SERVER
using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle;

namespace DemoWeb {
	public partial class ContainerControl : IControl, IContainerControl {
		private Dictionary<string, IControl> controls = new Dictionary<string, IControl>();
		public Dictionary<string, IControl> Controls { get { return controls; } }

		private Position position = PositionHelper.NotPositioned;
		public Position Position { get { return position; } set { position = value; } }

		private string id;
		public string Id {
			get { return id; }
			set {
				this.id = value;
				foreach (KeyValuePair<string, IControl> kvp in controls)
					kvp.Value.Id = value + "_" + kvp.Key;
			}
		}

		private Dictionary<string, object> GetConfig() {
			Dictionary<string, object> __cfg = new Dictionary<string, object>();
			return __cfg;
		}

		private readonly DemoWeb.SimpleControl Nested;

		private readonly Saltarelle.UI.Grid grid;

		private readonly Saltarelle.UI.Tree tree;

		private string dialog_inner() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@" <div style=""width: 200px; height: 200px""> Dialog content </div> ");
			return sb.ToString();
		}

		private readonly Saltarelle.UI.Label _ctl1;

		private string _ctl2_inner() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@" <div style=""width: 200px; height: 200px""> Content 1 </div> <div style=""width: 200px; height: 200px""> Content 2 </div> <div style=""width: 200px; height: 200px""> Content 3 </div> ");
			return sb.ToString();
		}

		private readonly Saltarelle.UI.DialogFrame dialog;

		private string group_inner() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@" <button type=""button"" id=""");
			sb.Append(Id);
			sb.Append(@"_showDialogButton"">Open Dialog</button> ");
			sb.Append(((IControl)_ctl1).Html);
			sb.Append(@" ");
			return sb.ToString();
		}

		private readonly Saltarelle.UI.TabControl _ctl2;

		private readonly Saltarelle.UI.GroupBox group;

		private string GetHtml() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@"<div id=""");
			sb.Append(Id);
			sb.Append(@""" style=""");
			sb.Append(PositionHelper.CreateStyle(Position, -1, -1));
			sb.Append(@"""");
			sb.Append(" __cfg=\"" + Utils.HtmlEncode(Utils.Json(GetConfig())) + "\"");
			sb.Append(@"> ");
			sb.Append(((IControl)Nested).Html);
			sb.Append(@" ");
			sb.Append(((IControl)grid).Html);
			sb.Append(@" ");
			sb.Append(((IControl)tree).Html);
			sb.Append(@" ");
			((IControlHost)dialog).SetInnerHtml(dialog_inner());
			sb.Append(((IControl)dialog).Html);
			sb.Append(@" ");
			((IControlHost)group).SetInnerHtml(group_inner());
			sb.Append(((IControl)group).Html);
			sb.Append(@" ");
			((IControlHost)_ctl2).SetInnerHtml(_ctl2_inner());
			sb.Append(((IControl)_ctl2).Html);
			sb.Append(@" </div>");
			return sb.ToString();
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new InvalidOperationException("Must assign Id before rendering.");
				return GetHtml();
			}
		}

		public ContainerControl() {
			GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());
			this.controls["Nested"] = this.Nested = new DemoWeb.SimpleControl();
			this.Nested.Person = new Person(@"Erik", @"Källén");

			this.controls["grid"] = this.grid = new Saltarelle.UI.Grid();
			this.grid.ColWidths = new int[] { 150, 100 };
			this.grid.Width = 280;
			this.grid.Height = 200;
			this.grid.ColTitles = new string[] { @"Column 1", @"Column2" };

			this.controls["tree"] = this.tree = new Saltarelle.UI.Tree();
			this.tree.Width = 280;
			this.tree.Height = 200;

			this.controls["_ctl1"] = this._ctl1 = new Saltarelle.UI.Label();
			this._ctl1.Text = @"Some Text";

			this.controls["dialog"] = this.dialog = new Saltarelle.UI.DialogFrame();
			this.dialog.ModalityInt = 2;

			this.controls["_ctl2"] = this._ctl2 = new Saltarelle.UI.TabControl();
			this._ctl2.TabCaptions = new string[] { @"Tab 1", @"Tab 2", @"Tab 3" };
			this._ctl2.RightAlignTabs = true;
			this._ctl2.SelectedTab = 1;

			this.controls["group"] = this.group = new Saltarelle.UI.GroupBox();
			this.group.Title = @"Group title";

			Init();
		}
	}
}
#endif
#if CLIENT
using System;
using Saltarelle;

namespace DemoWeb {
	public partial class ContainerControl : IControl, IContainerControl {
		private Dictionary controls = new Dictionary();
		public Dictionary Controls { get { return controls; } }

		private Position position;
		public Position Position {
			get { return element != null ? PositionHelper.GetPosition(element) : position; }
			set {
				position = value;
				if (element != null)
					PositionHelper.ApplyPosition(element, value);
			}
		}

		private jQuery element;
		public jQuery Element { get { return element; } }

		private string id;
		public string Id {
			get { return id; }
			set {
				this.id = value;
				foreach (DictionaryEntry kvp in controls)
					((IControl)kvp.Value).Id = value + "_" + kvp.Key;
				showDialogButton.attr("id", value + "_showDialogButton");
			}
		}

		private readonly DemoWeb.SimpleControl Nested;

		private readonly Saltarelle.UI.Grid grid;

		private readonly Saltarelle.UI.Tree tree;

		private jQuery showDialogButton;

		private readonly Saltarelle.UI.Label _ctl1;

		private readonly Saltarelle.UI.DialogFrame dialog;

		private readonly Saltarelle.UI.TabControl _ctl2;

		private readonly Saltarelle.UI.GroupBox group;

		public ContainerControl(string id) {
			if (!Script.IsUndefined(id)) {
				this.id = id;
				this.element = JQueryProxy.jQuery("#" + id);
				Dictionary __cfg = (Dictionary)Utils.EvalJson((string)this.element.attr("__cfg"));
				this.controls["Nested"] = this.Nested = new DemoWeb.SimpleControl(id + "_Nested");
				this.controls["grid"] = this.grid = new Saltarelle.UI.Grid(id + "_grid");
				this.controls["tree"] = this.tree = new Saltarelle.UI.Tree(id + "_tree");
				this.showDialogButton = JQueryProxy.jQuery("#" + id + "_showDialogButton");
				this.controls["_ctl1"] = this._ctl1 = new Saltarelle.UI.Label(id + "__ctl1");
				this.controls["dialog"] = this.dialog = new Saltarelle.UI.DialogFrame(id + "_dialog");
				this.controls["_ctl2"] = this._ctl2 = new Saltarelle.UI.TabControl(id + "__ctl2");
				this.controls["group"] = this.group = new Saltarelle.UI.GroupBox(id + "_group");
			}
			else {
				throw new Exception("This control must be created server-side");
			}
			Init();
		}
	}
}
#endif
