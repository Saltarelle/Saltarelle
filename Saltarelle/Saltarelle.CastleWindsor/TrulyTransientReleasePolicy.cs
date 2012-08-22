using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Releasers;
using Castle.Windsor.Diagnostics;

namespace Saltarelle.CastleWindsor {
	/// <summary>
	/// Inherits from the default ReleasePolicy; do not track our own transient objects.
	/// Only tracks components that have decommission steps
	/// registered or have pooled lifestyle.
	/// </summary>
	[Serializable]
	public class TrulyTransientReleasePolicy : LifecycledComponentsReleasePolicy {
		/// <summary></summary>
		public TrulyTransientReleasePolicy(IKernel kernel) : base(kernel) {
		}

		/// <summary></summary>
		public TrulyTransientReleasePolicy(ITrackedComponentsDiagnostic trackedComponentsDiagnostic, ITrackedComponentsPerformanceCounter trackedComponentsPerformanceCounter) : base(trackedComponentsDiagnostic, trackedComponentsPerformanceCounter) {
		}

		/// <summary></summary>
		public override void Track(object instance, Burden burden) {
			ComponentModel model = burden.Model;
 
			// we skip the tracking for object marked with our custom Transient lifestyle manager
			if ((model.LifestyleType == LifestyleType.Custom) && (typeof(TrulyTransientLifestyle) == model.CustomLifestyle))
				return;
 
			base.Track(instance, burden);
		}
	}
}
