using System;
#if CLIENT
using System.Html;
#endif

namespace Saltarelle
{
	public interface IControl {
		Position Position { get; set; }
		string Id { get; set; }

#if CLIENT
		Element GetElement();
#endif

#if SERVER
		string Html { get; }
		object ConfigObject { get; }
#endif
	}
	
	public interface IClientCreateControl : IControl
	{
		#if CLIENT
			string Html { get; }
			void Attach();
		#endif
	}
	
	public interface IControlHost : IControl
	{
		void SetInnerFragments(string[] innerFragments);
		#if CLIENT
			Element[] GetInnerElements();
		#endif
	}
}
