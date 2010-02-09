using System;
using Saltarelle;
#if CLIENT
using System.XML;
#endif

namespace DemoWeb {
	public partial class Lesson5InnerControl : IControl {
		public Person Person { get { return person; } set { person = value; } }
	
#if SERVER
		private void Constructed() {
		}
#endif
#if CLIENT
		private void Constructed() {
		}

		private void Attached() {
			PersonDisplay.text("Hello, " + person.FirstName + " " + person.LastName);
		}
#endif
	}
}