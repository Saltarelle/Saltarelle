using System;

namespace Saltarelle {
	public delegate string UntypedMarkupDataValueRetriever();

	[Record]
	public sealed class UntypedMarkupData {
		public readonly IFragment Fragment;
		public readonly TypedMarkupDataValueRetriever ValueRetriever;

		public UntypedMarkupData(IFragment fragment, UntypedMarkupDataValueRetriever valueRetriever) {
			if (initializerString == null) throw Utils.ArgumentNullException("initializerString");
			if (valueRetriever == null) throw Utils.ArgumentNullException("valueRetriever");
			this.Fragment       = fragment;
			this.ValueRetriever = valueRetriever;
		}
		
#if SERVER
		public override bool Equals(object obj) {
			return obj is UntypedMarkupData && ((UntypedMarkupData)obj).InitializerString == InitializerString;
		}

		public override int GetHashCode() {
			return InitializerString.GetHashCode();
		}

		public override string ToString() {
			return InitializerString;
		}
#endif
	}
}