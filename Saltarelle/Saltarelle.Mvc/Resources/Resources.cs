using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltarelle.Mvc;
using Saltarelle;

// CSS
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.all.css", Resources.JQueryUICss)]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.accordion.css", "css/ui.accordion.css")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.autocomplete.css", "css/ui.autocomplete.css")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.base.css", "css/ui.base.css")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.button.css", "css/ui.button.css")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.core.css", "css/ui.core.css")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.datepicker.css", "css/ui.datepicker.css")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.dialog.css", "css/ui.dialog.css")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.progressbar.css", "css/ui.progressbar.css")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.resizable.css", "css/ui.resizable.css")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.selectable.css", "css/ui.selectable.css")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.slider.css", "css/ui.slider.css")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.tabs.css", "css/ui.tabs.css")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.jquery.ui.theme.css", "css/ui.theme.css")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-bg_flat_0_aaaaaa_40x100.png", "css/images/ui-bg_flat_0_aaaaaa_40x100.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-bg_flat_75_ffffff_40x100.png", "css/images/ui-bg_flat_75_ffffff_40x100.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-bg_glass_55_fbf9ee_1x400.png", "css/images/ui-bg_glass_55_fbf9ee_1x400.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-bg_glass_65_ffffff_1x400.png", "css/images/ui-bg_glass_65_ffffff_1x400.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-bg_glass_75_dadada_1x400.png", "css/images/ui-bg_glass_75_dadada_1x400.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-bg_glass_75_e6e6e6_1x400.png", "css/images/ui-bg_glass_75_e6e6e6_1x400.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-bg_glass_75_ffffff_1x400.png", "css/images/ui-bg_glass_75_ffffff_1x400.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-bg_glass_95_fef1ec_1x400.png", "css/images/ui-bg_glass_95_fef1ec_1x400.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-bg_highlight-soft_75_cccccc_1x100.png", "css/images/ui-bg_highlight-soft_75_cccccc_1x100.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-bg_inset-soft_95_fef1ec_1x100.png", "css/images/ui-bg_inset-soft_95_fef1ec_1x100.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-icons_222222_256x240.png", "css/images/ui-icons_222222_256x240.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-icons_2e83ff_256x240.png", "css/images/ui-icons_2e83ff_256x240.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-icons_454545_256x240.png", "css/images/ui-icons_454545_256x240.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-icons_888888_256x240.png", "css/images/ui-icons_888888_256x240.png")]
[assembly: WebResource("Saltarelle.Mvc.Resources.Css.images.ui-icons_cd0a0a_256x240.png", "css/images/ui-icons_cd0a0a_256x240.png")]

// Scripts
[assembly: WebResource("Saltarelle.Mvc.Resources.Scripts.jquery-1.7.2.js", Resources.JQueryScriptDebug)]
[assembly: WebResource("Saltarelle.Mvc.Resources.Scripts.jquery-1.7.2.min.js", Resources.JQueryScriptRelease)]
[assembly: WebResource("Saltarelle.Mvc.Resources.Scripts.jquery-ui-1.8.21.js", Resources.JQueryUIScriptDebug)]
[assembly: WebResource("Saltarelle.Mvc.Resources.Scripts.jquery-ui-1.8.21.min.js", Resources.JQueryUIScriptRelease)]
[assembly: WebResource("Saltarelle.Mvc.Resources.Scripts.jquery.bgiframe.js", Resources.JQueryBgiframeDebug)]
[assembly: WebResource("Saltarelle.Mvc.Resources.Scripts.jquery.bgiframe.min.js", Resources.JQueryBgiframeRelease)]
[assembly: WebResource("Saltarelle.Mvc.Resources.Scripts.jquery.focus.js", Resources.JQueryFocusScript)]
[assembly: WebResource("Saltarelle.Mvc.Resources.Scripts.json2.js", Resources.Json2Script)]
[assembly: WebResource("Saltarelle.Mvc.Resources.Scripts.sscompat.debug.js", Resources.SSCompatScriptDebug)]
[assembly: WebResource("Saltarelle.Mvc.Resources.Scripts.sscompat.js", Resources.SSCompatScriptRelease)]
[assembly: WebResource("Saltarelle.Mvc.Resources.Scripts.mscorlib.debug.js", Resources.MSCorlibScriptDebug)]
[assembly: WebResource("Saltarelle.Mvc.Resources.Scripts.mscorlib.js", Resources.MSCorlibScriptRelease)]

namespace Saltarelle.Mvc {
	public static class Resources {
		public const string JQueryUICss = "css/ui.all.css";
		
		public const string JQueryScriptRelease   = "scripts/jquery-1.7.2.min.js";
		public const string JQueryScriptDebug     = "scripts/jquery-1.7.2.js";
		public const string JQueryUIScriptRelease = "scripts/jquery-ui-1.8.2.min.js";
		public const string JQueryUIScriptDebug   = "scripts/jquery-ui-1.8.2.js";
		public const string JQueryBgiframeRelease = "scripts/jquery.bgiframe.min.js";
		public const string JQueryBgiframeDebug   = "scripts/jquery.bgiframe.js";
		public const string JQueryFocusScript     = "scripts/jquery.focus.js";
		public const string Json2Script           = "scripts/json2.js";
		public const string SSCompatScriptDebug   = "scripts/sscompat.debug.js";
		public const string SSCompatScriptRelease = "scripts/sscompat.js";
		public const string MSCorlibScriptDebug   = "scripts/mscorlib.debug.js";
		public const string MSCorlibScriptRelease = "scripts/mscorlib.js";
		
		public static readonly IList<string> CoreScriptsDebug   = new List<string>(new[] { SSCompatScriptDebug,   MSCorlibScriptDebug,   JQueryScriptDebug,   JQueryUIScriptDebug,   JQueryFocusScript, JQueryBgiframeDebug   }).AsReadOnly();
		public static readonly IList<string> CoreScriptsRelease = new List<string>(new[] { SSCompatScriptRelease, MSCorlibScriptRelease, JQueryScriptRelease, JQueryUIScriptRelease, JQueryFocusScript, JQueryBgiframeRelease }).AsReadOnly();
	}
}
