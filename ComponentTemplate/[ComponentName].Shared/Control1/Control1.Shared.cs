using System;
using Saltarelle;
#if CLIENT
using System.XML;
#endif

namespace [ComponentName] {
	public partial class Control1 : IControl, IContainerControl {
#if SERVER
		private void Init() {
		}
#endif
#if CLIENT
		private void Init() {
		}
#endif
	}
}