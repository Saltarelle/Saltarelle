using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Configuration;
using SysConfig = System.Configuration.Configuration;
using System.Reflection;

using Saltarelle;
using Saltarelle.Configuration;

namespace Saltarelle {
	public static class ExecutablesCommon {
		public const string GeneratedFileExtension = ".Template.cs";
		public const string ConfigFileName = "saltarelle.config";
		
		public static string FindConfigFilePath(string inputFilePath) {
			string currentDir = Path.GetDirectoryName(inputFilePath);
			while (!Utils.IsNull(currentDir)) {
				string s = Path.Combine(currentDir, ConfigFileName);
				if (File.Exists(s))
					return s;
				currentDir = Path.GetDirectoryName(currentDir);
			}
			return null;
		}

		public static XmlDocument CreateXmlDocument() {
			return new XmlDocument() { PreserveWhitespace = true };
		}
	
		public static string GetTemplateCodeFileContents(SaltarelleParser parser, XmlDocument doc, string className, string nmspace) {
			ITemplate template = parser.ParseTemplate(doc);
			template.ClassName = className;
			template.Nmspace   = nmspace;

			CodeBuilder cb = new CodeBuilder();
			cb.AppendLine("#pragma warning disable 1591")
			  .AppendLine("#if SERVER");
			template.WriteServerCode(cb);
			cb.AppendLine("#endif")
			  .AppendLine("#if CLIENT");
			template.WriteClientCode(cb);
			cb.AppendLine("#endif");
			
			return cb.ToString();
		}
	}
}