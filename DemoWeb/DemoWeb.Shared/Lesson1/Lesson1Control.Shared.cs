using System;
using Saltarelle;
using Saltarelle.UI;

namespace DemoWeb {
	public partial class Lesson1Control : IControl {
		public string Message {
			get { return TheText.Value; }
			set { TheText.Value = value; }
		}
	
#if SERVER
		private void Constructed() {
		}
#endif

#if CLIENT
		private void Constructed() {
			TheText.ValueChanged += TheText_ValueChanged;
		}
		
		private void Attached() {
			AddMessageButton.click(AddMessageButton_Click);
			TheText_ValueChanged(TheText, EventArgs.Empty);	// Since the current message text is not set by the server, we need to set it during initialization.
		}

		private void TheText_ValueChanged(object sender, System.EventArgs e) {
			CurrentMessageDiv.text(TheText.Value);
		}
		
		private void AddMessageButton_Click(JQueryEvent evt) {
			string msg = TheText.Value.Trim();
			if (string.IsNullOrEmpty(msg)) {
				Script.Alert("The value is empty");
				TheText.Element.focus();
				return;
			}
			Label label = new Label();
			label.Text = TheText.Value;
			label.Id = ((IScriptManagerService)GlobalServices.GetService(typeof(IScriptManagerService))).GetUniqueId();
			Utils.RenderControl(label, MessageLogDiv);
		}
#endif
	}
}
