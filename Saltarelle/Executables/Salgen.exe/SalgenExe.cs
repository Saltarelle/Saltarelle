using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

namespace Saltarelle {
	class SalgenExe {
		private class Options {
			public bool   IsValid    { get; private set; }
			public string InputFile  { get; private set; }
			public string Nmspace    { get; private set; }
			public string ClassName  { get; private set; }
			public string OutputFile { get; private set; }
			public string ConfigFile { get; private set; }
			
			public Options(string[] args) {
				Arguments cmdLine = new Arguments(args);

				if (cmdLine.FileList.Count != 1)
					return;
				InputFile = Path.GetFullPath(cmdLine.FileList[0]);

				Nmspace = cmdLine["namespace"];
				if (!Utils.IsNull(Nmspace) && !ParserUtils.IsValidQualifiedName(Nmspace))
					IsValid = false;
				
				ClassName = cmdLine["class"];
				if (Utils.IsNull(ClassName)) {
					ClassName = new Regex("[^0-9a-zA-Z_]", RegexOptions.IgnoreCase).Replace(Path.GetFileNameWithoutExtension(InputFile), "_");
					if (ClassName.Length == 0 || ClassName.IndexOfAny(new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }) == 0)
						ClassName = "_" + ClassName;
				}
				else if (!ParserUtils.IsValidUnqualifiedName(ClassName))
					IsValid = false;
				
				OutputFile = cmdLine["out"];
				if (OutputFile == "")
					return;
				if (Utils.IsNull(OutputFile))
					OutputFile = Path.ChangeExtension(InputFile, ExecutablesCommon.GeneratedFileExtension);

				ConfigFile = cmdLine["config"];
				if (ConfigFile == "")
					return;
				else if (Utils.IsNull(ConfigFile))
					ConfigFile = ExecutablesCommon.FindConfigFilePath(InputFile);

				IsValid = true;
			}
		}
	
		private static void Usage() {
			Console.WriteLine("Usage:");
			Console.WriteLine("   salgen [options] input-file");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine("   -class <ClassName>");
			Console.WriteLine("      Set the name of the class (default: same as the input file name)");
			Console.WriteLine("   -namespace <Namespace.Name>");
			Console.WriteLine("      Set the namespace inside which the class is generated (default: none)");
			Console.WriteLine("   -out <output-file>");
			Console.WriteLine("      Specify the name of the output file (default: <input-file>.Template.cs)");
			Console.WriteLine("   -config <config-file>");
			Console.WriteLine("      Manually specify the location of the config file.");
			Console.WriteLine("         (default: saltarelle.config in input file directory or any parent one)");
		}
		
		private static void Main(string[] args) {
			try {
				Options opts = new Options(args);
				if (!opts.IsValid) {
					Usage();
					#if DEBUG
						if (System.Diagnostics.Debugger.IsAttached)
							Console.ReadLine();
					#endif
					return;
				}

                ExecutablesCommon.ProcessWorkItemsInSeparateAppDomain(opts.ConfigFile, new[] { new WorkItem { InFile = opts.InputFile, OutFile = opts.OutputFile, Namespace = opts.Nmspace } }, null);
			}
			catch (Exception ex) {
				Console.WriteLine(ex.Message);
				#if DEBUG
					if (System.Diagnostics.Debugger.IsAttached)
						Console.ReadLine();
				#endif
			}
		}
	}
}
