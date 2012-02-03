using Saltarelle.Ioc;

namespace Saltarelle {
    public static class GlobalServices {
        private static DefaultContainer container;
        private static DefaultScriptManagerService scriptManager;

        public static void Initialize(ScriptManagerConfig config) {
            if (container == null) {
                container = new DefaultContainer();
                ControlFactory.SetContainer(container);
            }
            if (scriptManager == null)
                scriptManager = new DefaultScriptManagerService(config.nextUniqueId);
            container.RegisterServiceConfig(config.services);
            container.RegisterInjections(config.injections);

            foreach (ScriptManagerConfigControlRow c in config.controls) {
                scriptManager.RegisterTopLevelControl(ControlFactory.CreateControlByTypeNameWithConfig(c.type, c.config));
            }
        }

        public static IContainer Container { get { return container; } }
        public static IScriptManagerService ScriptManager { get { return scriptManager; } }
    }
}
