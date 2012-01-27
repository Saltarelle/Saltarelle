using System;
using System.DHTML;

namespace Saltarelle {
	public class DefaultScriptManagerService : IScriptManagerService {
		private int nextUniqueId;
		private ArrayList includedScripts;
		private Dictionary topLevelControls;
		
		public DefaultScriptManagerService(object config) {
			Dictionary cfg = Dictionary.GetDictionary(config);
			this.nextUniqueId = (int)cfg["nextUniqueId"];
			this.topLevelControls = new Dictionary();

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
		
		public void RegisterTopLevelControl(string id, IControl control) {
			if (!string.IsNullOrEmpty(id))
				topLevelControls[id] = control;
		}
		
		public IControl GetTopLevelControl(string id) {
			return (IControl)topLevelControls[id] ?? null;	// ?? null is required to convert from undefined to null.
		}
	}
}
