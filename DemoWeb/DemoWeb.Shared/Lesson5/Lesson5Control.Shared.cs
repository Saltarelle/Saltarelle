using System;
using Saltarelle;
using Saltarelle.UI;

namespace DemoWeb {
	public partial class Lesson5Control : IControl {
#if SERVER
		partial void Constructed() {
		}
#endif
#if CLIENT
		partial void Constructed() {
		}
		
		partial void Attached() {
		}
#endif
	}
}