namespace Saltarelle.Mvc {
	public interface IView : IControl {
		object Model { get; set; }
	}
	
	public interface IView<T> : IView {
		new T Model { get; set; }
	}
}
