using System;
using Saltarelle;
using Saltarelle.Ioc;
using Saltarelle.UI;
#if CLIENT
using System.Html;
using jQueryApi;
#endif

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

		#if SERVER
		[ClientInject]
		#endif
		public IScriptManagerService ScriptManager { get; set; }

#if CLIENT
		private void Constructed() {
			TheText.ValueChanged += TheText_ValueChanged;
		}
		
		private void Attached() {
			jQuery.FromElement(AddMessageButton).Click(AddMessageButton_Click);
			TheText_ValueChanged(TheText, EventArgs.Empty);	// Since the current message text is not set by the server, we need to set it during initialization.
		}

		private void TheText_ValueChanged(object sender, System.EventArgs e) {
			CurrentMessageDiv.InnerText = TheText.Value;
		}
		
		private void AddMessageButton_Click(jQueryEvent evt) {
			string msg = TheText.Value.Trim();
			if (string.IsNullOrEmpty(msg)) {
				Window.Alert("The value is empty");
				TheText.GetElement().Focus();
				return;
			}
			Label label = (Label)Container.CreateObject(typeof(Label));
			label.Text = TheText.Value;
			label.Id = ScriptManager.GetUniqueId();
			Utils.RenderControl(label, MessageLogDiv);
		}
#endif
	}
}
