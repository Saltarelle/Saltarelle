using System;
using Saltarelle.Ioc;

namespace Saltarelle.UI {
	public class DefaultSaltarelleUIService : ISaltarelleUIService
	#if SERVER
		, IService
	#endif
	{
		private string blankImageUrl;
		public string BlankImageUrl { get { return blankImageUrl; } }

#if SERVER
        public DefaultSaltarelleUIService(IRouteService routes) {
			blankImageUrl = routes.GetAssemblyResourceUrl(typeof(Saltarelle.UI.Resources).Assembly, Saltarelle.UI.Resources.BlankImage);
        }

		public void Setup() {
		}
		
		public object ConfigObject {
			get { return new { blankImageUrl }; }
		}
#endif

#if CLIENT
		public DefaultSaltarelleUIService(object config) {
			Dictionary cfg = Dictionary.GetDictionary(config);
			this.blankImageUrl = (string)cfg["blankImageUrl"];
		}
#endif
	}
}
