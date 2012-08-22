using System;
using Saltarelle;
using Saltarelle.Ioc;
using Saltarelle.UI;

namespace DemoWeb {
	public partial class Lesson5InnerControl : IControl {
		public Person Person { get { return person; } set { person = value; } }

		#if SERVER
		[ClientInject]
		#endif
		public ISaltarelleUIService UIService { get; set; }
	
#if SERVER
		partial void Constructed() {
		}
#endif
#if CLIENT
		partial void Constructed() {
		}

		partial void Attached() {
			PersonDisplay.InnerText = "Hello, " + person.FirstName + " " + person.LastName;
		}
#endif
	}
}