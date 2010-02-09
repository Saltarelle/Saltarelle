using System;
using Saltarelle;
using Saltarelle.UI;

namespace DemoWeb {
	public partial class Lesson5Control : IControl {
#if SERVER
		private void Constructed() {
		}
#endif
#if CLIENT
		private void Constructed() {
		}
		
		private void Attached() {
		}
#endif
	}
}