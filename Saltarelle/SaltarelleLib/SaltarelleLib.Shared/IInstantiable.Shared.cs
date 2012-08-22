using Saltarelle.Ioc;

namespace Saltarelle {
	public interface IInstantiable {
		IControl Instantiate(IContainer container);
	}
}