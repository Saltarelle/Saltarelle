using System;

namespace Saltarelle.Ioc {
    public static class ControlFactory {
#if SERVER
        private static Func<IContainer> _containerFactory;

        public static IControl CreateControl(Type controlType) {
            return (IControl)_containerFactory().CreateObject(controlType);
        }

        public static IControl CreateControlByTypeName(string typeName) {
            return (IControl)_containerFactory().CreateObjectByTypeName(typeName);
        }

        public static T CreateControl<T>() where T : IControl {
            return (T)_containerFactory().CreateObject<T>();
        }

        public static void SetContainerFactory(Func<IContainer> containerFactory) {
            _containerFactory = containerFactory;
        }
#endif

#if CLIENT
        private static IContainer _container;

        public static IControl CreateControl(Type controlType) {
            return (IControl)_container.CreateObject(controlType);
        }

        public static IControl CreateControlByTypeName(string typeName) {
            return (IControl)_container.CreateObjectByTypeName(typeName);
        }

        public static IControl CreateControlWithConfig(Type controlType, object config) {
            return (IControl)_container.CreateObjectWithConstructorArg(controlType, config);
        }

        public static IControl CreateControlByTypeNameWithConfig(string typeName, object config) {
            return (IControl)_container.CreateObjectByTypeNameWithConstructorArg(typeName, config);
        }

        public static void SetContainer(IContainer container) {
            _container = container;
        }
#endif
    }
}