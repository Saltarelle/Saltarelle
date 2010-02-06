using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;
using System.Xml;

namespace Saltarelle {
	public class Salgen : Task {
		private class FilePair {
			public string In  { get; set; }
			public string Out { get; set; }
		}
	
		public override bool Execute() {
			if (Namespace == "")
				Namespace = null;
			if (Namespace != null && !ParserUtils.IsValidQualifiedName(Namespace)) {
				Log.LogError("Invalid namespace for Salgen task");
				return false;
			}

			var inputsGroupedByConfigFile = new Dictionary<string, List<FilePair>>();
		
			OutputFiles = new string[InputFiles.Length];
			for (int i = 0; i < InputFiles.Length; i++) {
				OutputFiles[i] = Path.ChangeExtension(InputFiles[i], ExecutablesCommon.GeneratedFileExtension);
				string infile     = Path.GetFullPath(InputFiles[i]);
				string outfile    = Path.GetFullPath(OutputFiles[i]);
				string configFile = ExecutablesCommon.FindConfigFilePath(infile) ?? "";
				List<FilePair> l;
				if (!inputsGroupedByConfigFile.TryGetValue(configFile, out l))
					inputsGroupedByConfigFile[configFile] = l = new List<FilePair>();
				l.Add(new FilePair() { In = infile, Out = outfile });
			}

			bool success = true;
			foreach (var kvp in inputsGroupedByConfigFile) {
				SaltarelleParser parser = kvp.Key != "" ? SaltarelleParserFactory.CreateParserFromConfigFile(kvp.Key) : SaltarelleParserFactory.CreateDefaultParser();
				foreach (var f in kvp.Value) {
					try {
						XmlDocument doc = ExecutablesCommon.CreateXmlDocument();
						doc.Load(f.In);
						string outText = ExecutablesCommon.GetTemplateCodeFileContents(parser, doc, Path.GetFileNameWithoutExtension(f.In), Namespace);
						using (var w = new StreamWriter(f.Out))
							w.Write(outText);
					}
					catch (TemplateErrorException ex) {
						Log.LogError(ex.Message);
						success = false;
					}
				}
			}
			return success;
		}
		
		[Required]
		public string[] InputFiles { get; set; }
		
		public string Namespace { get; set; }

		[Output]
		public string[] OutputFiles { get; set; }
	}
}
