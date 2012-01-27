using System;

namespace Saltarelle {
	public interface IRenderFunction
	#if SERVER
		: IMember
	#endif
	{
		void AddDependency(IMember dependency);
		void AddFragment(IFragment fragment);
		bool IsEmpty { get; }
		
		string Render(ITemplate tpl, IInstantiatedTemplateControl ctl);
	}
}
