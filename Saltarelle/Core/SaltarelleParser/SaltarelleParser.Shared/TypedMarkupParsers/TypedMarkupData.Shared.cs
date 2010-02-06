using System;
#if SERVER
using System.Text;
#endif

namespace Saltarelle {
	public delegate object TypedMarkupDataValueRetriever();

	[Record]
	public sealed class TypedMarkupData {
		public readonly string InitializerString;
		public readonly TypedMarkupDataValueRetriever ValueRetriever;

		public TypedMarkupData(string initializerString, TypedMarkupDataValueRetriever valueRetriever) {
			if (initializerString == null) throw Utils.ArgumentNullException("initializerString");
			if (valueRetriever == null) throw Utils.ArgumentNullException("valueRetriever");
			this.InitializerString = initializerString;
			this.ValueRetriever    = valueRetriever;
		}

#if SERVER
		/// Should only be used during test
		internal TypedMarkupData(string initializerString) {
			if (initializerString == null) throw new ArgumentNullException("initializerString");
			this.InitializerString = initializerString;
			this.ValueRetriever    = null;
		}

		public override bool Equals(object obj) {
			return obj is TypedMarkupData && ((TypedMarkupData)obj).InitializerString == InitializerString;
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
