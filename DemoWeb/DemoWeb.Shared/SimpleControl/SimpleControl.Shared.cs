using System;
using Saltarelle;
#if CLIENT
using System.XML;
#endif

namespace DemoWeb {
	public partial class SimpleControl : IControl, IContainerControl {
		public Person Person { get { return person; } set { person = value; } }
	
#if SERVER
		private void Init() {
		}
#endif
#if CLIENT
		private void Init() {
			AlertButton.click(AlertButton_Click);
			Textbox.ValueChanged += Textbox_ValueChanged;
		}

		void Textbox_ValueChanged(object sender, EventArgs e) {
			ValueDisplayer.text(Textbox.Value);
		}
		
		private void AlertButton_Click(JQueryEvent evt) {
			Script.Alert(Textbox.Value);
		}
#endif
	}
}