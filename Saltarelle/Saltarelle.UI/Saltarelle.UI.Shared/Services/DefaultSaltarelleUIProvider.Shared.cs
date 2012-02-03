using System;

namespace Saltarelle.UI {
	public class DefaultSaltarelleUIProvider : ISaltarelleUIService
	{
		private string blankImageUrl;
		public string BlankImageUrl { get { return blankImageUrl; } }

#if SERVER
        public DefaultSaltarelleUIProvider(IRouteService routes) {
			blankImageUrl = routes.GetAssemblyResourceUrl(typeof(Saltarelle.UI.Resources).Assembly, Saltarelle.UI.Resources.BlankImage);
        }

		public void Setup() {
		}
		
		public object ConfigObject {
			get { return new { blankImageUrl }; }
		}
#endif

#if CLIENT
		public DefaultSaltarelleUIProvider(object config) {
			Dictionary cfg = Dictionary.GetDictionary(config);
			this.blankImageUrl = (string)cfg["blankImageUrl"];
		}
#endif
	}
}
