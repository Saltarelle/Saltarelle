using System;

namespace Saltarelle.UI {
#if SERVER
	[GlobalService(typeof(ISaltarelleUIService))]
#endif
	public class DefaultSaltarelleUIProvider : ISaltarelleUIService
	#if SERVER
	, IGlobalService
	#endif
	{
		private string blankImageUrl;
		public string BlankImageUrl { get { return blankImageUrl; } }

#if SERVER
		public void Setup() {
			blankImageUrl = Saltarelle.Mvc.Routes.GetAssemblyResourceUrl(typeof(Saltarelle.UI.Resources).Assembly, Saltarelle.UI.Resources.BlankImage);
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
