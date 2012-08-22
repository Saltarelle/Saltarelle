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
		private string FindNamespace(ITaskItem item) {
			string s = item.GetMetadata("CustomToolNamespace");
			if (!string.IsNullOrEmpty(s))
				return s;	// Easy - the namespace was set explicitly
			s = item.GetMetadata("Link");	// If the Link metadata is set, it defines the file as the project sees it.
			string dir = Path.GetDirectoryName(!string.IsNullOrEmpty(s) ? s : item.ItemSpec);

			string rootNamespaceWithDot = RootNamespace;
			if (!string.IsNullOrEmpty(rootNamespaceWithDot) && !rootNamespaceWithDot.EndsWith(".") && !string.IsNullOrEmpty(dir))
				rootNamespaceWithDot += ".";
			return rootNamespaceWithDot + dir;
		}
		
		public override bool Execute() {
			var inputsGroupedByConfigFile = new Dictionary<string, List<WorkItem>>();
		
			OutputFiles = new ITaskItem[InputFiles.Length];
			for (int i = 0; i < InputFiles.Length; i++) {
				string infileLocal = InputFiles[i].ItemSpec;
				OutputFiles[i] = new TaskItem(Path.ChangeExtension(infileLocal, ExecutablesCommon.GeneratedFileExtension));
				string infile     = Path.GetFullPath(infileLocal);
				string outfile    = Path.ChangeExtension(infile, ExecutablesCommon.GeneratedFileExtension);
				string configFile = ExecutablesCommon.FindConfigFilePath(infile) ?? "";
				List<WorkItem> l;
				if (!inputsGroupedByConfigFile.TryGetValue(configFile, out l))
					inputsGroupedByConfigFile[configFile] = l = new List<WorkItem>();
				l.Add(new WorkItem() { InFile = infile, OutFile = outfile, Namespace = FindNamespace(InputFiles[i]) });
			}

			bool success = true;
			foreach (var kvp in inputsGroupedByConfigFile) {
                ExecutablesCommon.ProcessWorkItemsInSeparateAppDomain(kvp.Key, kvp.Value, (item, ex) => {
                    if (ex is TemplateErrorException) {
						Log.LogError(null, null, null, item.InFile, 0, 0, 0, 0, ex.Message);
						success = false;
					}
					else {
						Log.LogErrorFromException(ex, true, true, item.InFile);
						success = false;
					}
                    return true;
                });
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
