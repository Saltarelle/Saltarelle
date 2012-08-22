using Saltarelle.Ioc;

namespace Saltarelle {
    public static class GlobalServices {
        private static DefaultContainer container;
        private static DefaultScriptManagerService scriptManager;

        public static void Initialize(ScriptManagerConfig config) {
            if (container == null) {
                container = new DefaultContainer();
            }
            if (scriptManager == null)
                scriptManager = new DefaultScriptManagerService(config.nextUniqueId);
			foreach (var svc in config.services)
				container.RegisterServiceConfig(svc.Key, svc.Value);
			foreach (var inj in config.injections)
				container.RegisterInjection(inj.Key, inj.Value);

            foreach (ScriptManagerConfigControlRow c in config.controls) {
                scriptManager.RegisterTopLevelControl((IControl)container.CreateObjectByTypeNameWithConstructorArg(c.type, c.config));
            }
        }

        public static IContainer Container { get { return container; } }
        public static IScriptManagerService ScriptManager { get { return scriptManager; } }
    }
}
