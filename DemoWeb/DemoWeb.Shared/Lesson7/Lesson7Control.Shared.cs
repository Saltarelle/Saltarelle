using System;
using System.Text.RegularExpressions;
using Saltarelle;
using Saltarelle.Ioc;

#if CLIENT
using System.Html;
using jQueryApi;
#endif

namespace DemoWeb {
	public partial class Lesson7Control : IControl {
#if SERVER
		private void Constructed() {
		}
#endif

		#if SERVER
		[ClientInject]
		#endif
		public ILesson7Service Service { get; set; }

		#if SERVER
		[ClientInject]
		#endif
		public IContainer Container { get; set; }

		#if SERVER
		[ClientInject]
		#endif
		public IScriptManagerService ScriptManager { get; set; }

#if CLIENT
		private void Constructed() {
		}

		private void Attached() {
			jQuery.FromElement(InsertDynamicControlButton).Click(InsertDynamicControlButton_Click);
			jQuery.FromElement(AjaxButton).Click(AjaxButton_Click);
		}
		
		private void InsertDynamicControlButton_Click(jQueryEvent evt) {
			var p = new SaltarelleParser(null, null, null);
			ITemplate tpl;
			try {
				tpl = p.ParseTemplate(Utils.ParseXml(DynamicMarkupInput.Value));
			}
			catch (Exception ex) {
				Window.Alert(ex.Message);
				return;
			}
			IClientCreateControl ctl = (IClientCreateControl)tpl.Instantiate(Container);
			ctl.Id = ScriptManager.GetUniqueId();
			Utils.RenderControl(ctl, DynamicControlContainer);
		}
		
		private void AjaxButton_Click(jQueryEvent evt) {
			string numRowsStr = NumRowsInput.Value.Trim();
			if (new Regex("^\\d\\d?$").Exec(numRowsStr) == null) {
				Window.Alert("You must enter a number of rows (1 - 99).");
				return;
			}
			
			Service.AsyncCreateGrid(Utils.ParseInt(numRowsStr),
				frag => {
					DocumentFragmentHelper.Inject(frag, ScriptManager.GetUniqueId(), Container, AjaxControlContainer);
				},
				() => {
					Window.Alert("The call failed.");
				}
			);
		}
#endif
	}
}