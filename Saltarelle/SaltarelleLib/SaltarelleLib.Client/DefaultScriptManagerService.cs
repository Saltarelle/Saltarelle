using System;
using System.Html;
using Saltarelle.Ioc;

namespace Saltarelle {
	public class DefaultScriptManagerService : IScriptManagerService {
		private int       nextUniqueId;
		private ArrayList includedScripts;
		private ArrayList topLevelControls;

		public DefaultScriptManagerService(int nextUniqueId) {
			this.nextUniqueId     = nextUniqueId;
			this.topLevelControls = new ArrayList();

			includedScripts = new ArrayList();
			JQueryProxy.jQuery("script").each(delegate(int _, DOMElement el) {
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
				jQuery.ajax(new Dictionary("url", relativeUrl, "async", false, "cache", true, "dataType", "script"));
			}
		}
		
		public string GetUniqueId() {
			return "id" + Utils.ToStringInvariantInt(nextUniqueId++);
		}
		
		public void RegisterTopLevelControl(IControl control) {
			topLevelControls.Add(control);
		}
		
		public IControl GetTopLevelControl(string id) {
            for (int i = 0; i < topLevelControls.Length; i++) {
                IControl c = (IControl)topLevelControls[i];
                if (c.Id == id)
                    return c;
            }
			return null;
		}
	}
}
