using System;
using Saltarelle;

namespace DemoWeb {
	public partial class Lesson7Control : IControl {
#if SERVER
		private void Constructed() {
			GlobalServices.GetService<IScriptManagerService>().RegisterType(typeof(SaltarelleParser));
			GlobalServices.LoadService<ILesson7Service>();
		}
#endif

#if CLIENT
		private void Constructed() {
		}

		private void Attached() {
			InsertDynamicControlButton.click(InsertDynamicControlButton_Click);
			AjaxButton.click(AjaxButton_Click);
		}
		
		private void InsertDynamicControlButton_Click(JQueryEvent evt) {
			SaltarelleParser p = new SaltarelleParser(null, null, null);
			ITemplate tpl;
			try {
				tpl = p.ParseTemplate(Utils.ParseXml(DynamicMarkupInput.val()));
			}
			catch (Exception ex) {
				Script.Alert(ex.Message);
				return;
			}
			IClientCreateControl ctl = tpl.Instantiate();
			((IControl)ctl).Id = ((IScriptManagerService)GlobalServices.GetService(typeof(IScriptManagerService))).GetUniqueId();
			Utils.RenderControl(ctl, DynamicControlContainer);
		}
		
		private void AjaxButton_Click(JQueryEvent evt) {
			string numRowsStr = NumRowsInput.val().Trim();
			if (new RegularExpression("^\\d\\d?$").Exec(numRowsStr) == null) {
				Script.Alert("You must enter a number of rows (1 - 99).");
				return;
			}
			
			((ILesson7Service)GlobalServices.GetService(typeof(ILesson7Service))).AsyncCreateGrid(Utils.ParseInt(numRowsStr),
				delegate(ControlDocumentFragment frag) {
					DocumentFragmentHelper.Inject(frag, ((IScriptManagerService)GlobalServices.Provider.GetService(typeof(IScriptManagerService))).GetUniqueId(), AjaxControlContainer);
				},
				delegate {
					Script.Alert("The call failed.");
				}
			);
		}
#endif
	}
}