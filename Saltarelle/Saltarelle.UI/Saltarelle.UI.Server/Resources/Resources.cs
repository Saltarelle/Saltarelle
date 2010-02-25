using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltarelle.Mvc;
using Saltarelle.UI;

[assembly: CssResource("Saltarelle.UI.Resources.blank.gif", Resources.BlankImage, "blank-image")]
[assembly: CssResource("Saltarelle.UI.Resources.folder_closed.gif", Resources.FolderClosedImage, "folder_closed-image")]
[assembly: CssResource("Saltarelle.UI.Resources.folder_open.gif", Resources.FolderOpenImage, "folder_open-image")]
[assembly: CssResource("Saltarelle.UI.Resources.item.gif", Resources.ItemImage, "item-image")]
[assembly: CssResource("Saltarelle.UI.Resources.minus.gif", Resources.MinusImage, "minus-image")]
[assembly: CssResource("Saltarelle.UI.Resources.plus.gif", Resources.PlusImage, "plus-image")]

namespace Saltarelle.UI {
	public static class Resources {
		public const string BlankImage        = "blank.gif";
		public const string FolderClosedImage = "folder_closed.gif";
		public const string FolderOpenImage   = "folder_open.gif";
		public const string ItemImage         = "item.gif";
		public const string MinusImage        = "minus.gif";
		public const string PlusImage         = "plus.gif";
	}
}
