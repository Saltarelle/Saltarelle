using System;
#if SERVER
using ControlDictionary = System.Collections.Generic.Dictionary<string, Saltarelle.IControl>;
#endif
#if CLIENT
using System.DHTML;
using ControlDictionary = System.Dictionary;
#endif

namespace Saltarelle
{
	public interface IControl {
		Position Position { get; set; }
		string Id { get; set; }

#if CLIENT
		DOMElement GetElement();
#endif

#if SERVER
		string Html { get; }
		object ConfigObject { get; }
#endif
	}
	
	public interface IClientCreateControl
	#if SERVER
		: IControl
	#endif
	{
		#if CLIENT
			string Html { get; }
			void Attach();
		#endif
	}
	
	public interface IControlHost
	#if SERVER
		: IControl
	#endif
	{
		void SetInnerFragments(string[] innerFragments);
		#if CLIENT
			DOMElement[] GetInnerElements();
		#endif
	}
}
