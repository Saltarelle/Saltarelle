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

			var sm = GlobalServices.Provider.GetService<IScriptManagerService>();
			sm.AddStartupScript("if (!" + typeof(GlobalServices).FullName + ".hasService(" + typeof(ISaltarelleUIService).FullName + ")) " + typeof(GlobalServices).FullName + ".setService(" + typeof(ISaltarelleUIService).FullName + ", new " + typeof(DefaultSaltarelleUIProvider).FullName + "(" + Utils.ScriptStr(blankImageUrl) + "));");
		}
#endif

#if CLIENT
		public DefaultSaltarelleUIProvider(string blankImageUrl) {
			this.blankImageUrl = blankImageUrl;
		}
#endif
	}
}