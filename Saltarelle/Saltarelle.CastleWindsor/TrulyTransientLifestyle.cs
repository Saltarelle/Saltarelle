using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Lifestyle;

namespace Saltarelle.CastleWindsor {
	/// <summary>
	/// a custom Lifestyle, it will inerit from the standard class so if the TrulyTransientReleasePolicy policy
	/// isn't used these objects are handled as standard transient objects 
	/// </summary>
	public class TrulyTransientLifestyle : TransientLifestyleManager {
	}
}
