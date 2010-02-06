using System;

namespace Saltarelle.UI {
	public class CancelEventArgs : EventArgs {
		public bool Cancel;
	}
	public delegate void CancelEventHandler(object sender, CancelEventArgs e);
}