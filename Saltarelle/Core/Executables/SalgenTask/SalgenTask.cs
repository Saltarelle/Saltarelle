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
		private class DataItem {
			public string InFile    { get; set; }
			public string OutFile   { get; set; }
			public string Namespace { get; set; }
		}
		
		private string FindNamespace(ITaskItem item) {
			string s = item.GetMetadata("CustomToolNamespace");
			if (!string.IsNullOrEmpty(s))
				return s;	// Easy - the namespace was set explicitly
			s = item.GetMetadata("Link");
			string dir = Path.GetDirectoryName(!string.IsNullOrEmpty(s) ? s : item.ItemSpec);

			string rootNamespaceWithDot = RootNamespace;
			if (!string.IsNullOrEmpty(rootNamespaceWithDot) && !rootNamespaceWithDot.EndsWith(".") && !string.IsNullOrEmpty(dir))
				rootNamespaceWithDot += ".";
			return rootNamespaceWithDot + dir;
		}
		
		private void MaybeWriteFile(string path, byte[] content) {
			using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate)) {
				bool areEqual = false;
				if (fs.Length == content.Length) {
					areEqual = true;
					byte[] oldContent = new byte[fs.Length];
					fs.Read(oldContent, 0, oldContent.Length);
					for (int i = 0; i < content.Length; i++) {
						if (content[i] != oldContent[i]) {
							areEqual = false;
							break;
						}
					}
				}
				if (!areEqual) {
					fs.Seek(0, SeekOrigin.Begin);
					fs.Write(content, 0, content.Length);
				}
			}
		}

		public override bool Execute() {
			var inputsGroupedByConfigFile = new Dictionary<string, List<DataItem>>();
		
			OutputFiles = new ITaskItem[InputFiles.Length];
			for (int i = 0; i < InputFiles.Length; i++) {
				string infileLocal = InputFiles[i].ItemSpec;
				OutputFiles[i] = new TaskItem(Path.ChangeExtension(infileLocal, ExecutablesCommon.GeneratedFileExtension));
				string infile     = Path.GetFullPath(infileLocal);
				string outfile    = Path.ChangeExtension(infile, ExecutablesCommon.GeneratedFileExtension);
				string configFile = ExecutablesCommon.FindConfigFilePath(infile) ?? "";
				InputFiles[i].SetMetadata("LastGenOutput", Path.GetFileName(outfile));
				List<DataItem> l;
				if (!inputsGroupedByConfigFile.TryGetValue(configFile, out l))
					inputsGroupedByConfigFile[configFile] = l = new List<DataItem>();
				l.Add(new DataItem() { InFile = infile, OutFile = outfile, Namespace = FindNamespace(InputFiles[i]) });
			}

			bool success = true;
			foreach (var kvp in inputsGroupedByConfigFile) {
				SaltarelleParser parser = kvp.Key != "" ? SaltarelleParserFactory.CreateParserFromConfigFile(kvp.Key) : SaltarelleParserFactory.CreateDefaultParser();
				foreach (var f in kvp.Value) {
					try {
						XmlDocument doc = ExecutablesCommon.CreateXmlDocument();
						doc.Load(f.InFile);
						string outText = ExecutablesCommon.GetTemplateCodeFileContents(parser, doc, Path.GetFileNameWithoutExtension(f.InFile), f.Namespace);
						byte[] outBytes = Encoding.UTF8.GetBytes(outText);
						MaybeWriteFile(f.OutFile, outBytes);
					}
					catch (TemplateErrorException ex) {
						Log.LogError(Path.GetFileName(f.InFile) + ": " + ex.Message);
						success = false;
					}
					catch (XmlException ex) {
						Log.LogError(Path.GetFileName(f.InFile) + ": " + ex.Message);
						success = false;
					}
				}
			}
			return success;
		}
		
		[Required]
		public ITaskItem[] InputFiles { get; set; }

		[Output]
		public ITaskItem[] OutputFiles { get; set; }

		[Required]
		public string RootNamespace { get; set; }
	}
}
