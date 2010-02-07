using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Saltarelle;

namespace DemoWeb.Plugins {
	[UntypedMarkupParserImpl]
	public class CopyrightUntypedMarkupParser : IUntypedMarkupParserImpl {
		public IFragment TryParse(string markup) {
			if (markup == "CopyrightNotice")
				return new CopyrightFragment("Erik Källén");
			return null;
		}
	}
}
