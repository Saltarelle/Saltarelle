using System;

namespace Saltarelle {
	public interface ITypedMarkupParserImpl {
		TypedMarkupData Parse(string registeredPrefix, bool isArray, string value);
	}
}
