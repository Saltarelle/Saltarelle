using System;
using System.Collections.Generic;
using System.Html;
using Saltarelle.Ioc;
using jQueryApi;

namespace Saltarelle {
	public class DefaultScriptManagerService : IScriptManagerService {
		private int            nextUniqueId;
		private List<string>   includedScripts;
		private List<IControl> topLevelControls;

		public DefaultScriptManagerService(int nextUniqueId) {
			this.nextUniqueId     = nextUniqueId;
			this.topLevelControls = new List<IControl>();

			includedScripts = new List<string>();
			jQuery.Select("script").Each(delegate(int _, Element el) {
				string s = ((ScriptElement)el).Src;
				if (!Utils.IsNull(s)) {
					int ix = s.IndexOf("://");
					includedScripts.Add(ix != -1 ? s.Substr(s.IndexOf("/", ix + 3)) : s); // IE6 seems to return script paths relative, others return it as absolute
				}
				return true;
			});
		}
	
		public void EnsureScriptIncluded(string relativeUrl) {
			if (!includedScripts.Contains(relativeUrl)) {
				includedScripts.Add(relativeUrl);
				jQuery.Ajax(new jQueryAjaxOptions { Url = relativeUrl, Async = false, Cache = true, DataType = "script" });
			}
		}
		
		public string GetUniqueId() {
			return "id" + Utils.ToStringInvariantInt(nextUniqueId++);
		}
		
		public void RegisterTopLevelControl(IControl control) {
			topLevelControls.Add(control);
		}
		
		public IControl GetTopLevelControl(string id) {
            for (int i = 0; i < topLevelControls.Count; i++) {
                var c = topLevelControls[i];
                if (c.Id == id)
                    return c;
            }
			return null;
		}
	}
}
