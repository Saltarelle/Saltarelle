// Taken from http://www.codeproject.com/KB/recipes/command_line.aspx
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace Saltarelle {
	internal class Arguments {
		private static readonly Regex splitter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase|RegexOptions.Compiled);
		private static readonly Regex remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase|RegexOptions.Compiled);

		private Dictionary<string, string> parameters = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
		private ReadOnlyCollection<string> fileList;

		// Constructor
		public Arguments(string[] args) {
			string currentParam = null;
			var fileList = new List<string>();

			// Valid parameters forms:
			// {-,/,--}param{ ,=,:}((",')value(",'))
			// Examples: 
			// -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
			foreach(string currentArg in args) {
				// Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
				var parts = splitter.Split(currentArg, 3);

				switch (parts.Length) {
					case 1:	// Found a value (for the last parameter found (space separator))
						parts[0] = remover.Replace(parts[0], "$1");
						if (!Utils.IsNull(currentParam)) {
							if (!parameters.ContainsKey(currentParam))
								parameters.Add(currentParam, parts[0]);
							currentParam = null;
						}
						else
							fileList.Add(parts[0]);

						break;

					case 2:	// Found just a parameter
						if (!Utils.IsNull(currentParam) && !parameters.ContainsKey(currentParam))
							parameters.Add(currentParam, "");

						currentParam = parts[1];
						break;

					case 3:	// Parameter with enclosed value
						if(!Utils.IsNull(currentParam) && !parameters.ContainsKey(currentParam)) 
							parameters.Add(currentParam, "");

						currentParam = parts[1];

						// Remove possible enclosing characters (",')
						if (!parameters.ContainsKey(currentParam)) {
							parts[2] = remover.Replace(parts[2], "$1");
							parameters.Add(currentParam, parts[2]);
						}

						currentParam = null;
						break;
				}
			}

			// In case a parameter is still waiting
			if (!Utils.IsNull(currentParam) && !parameters.ContainsKey(currentParam)) 
				parameters.Add(currentParam, "");
				
			this.fileList = fileList.AsReadOnly();
		}

		public string this[string param] {
			get {
				string s;
				parameters.TryGetValue(param, out s);
				return s;
			}
		}
		
		public IList<string> FileList {
			get { return fileList; }
		}
	}
}
