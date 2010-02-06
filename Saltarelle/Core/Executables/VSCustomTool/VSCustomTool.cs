using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Saltarelle {
	[Guid("D5BAD3F4-6C57-4901-AEEC-AE18BAB760D9")]
	[ComVisible(true)]
	public class SalgenCustomTool : IVsSingleFileGenerator {
		public int DefaultExtension(out string pbstrDefaultExtension) {
			pbstrDefaultExtension = ExecutablesCommon.GeneratedFileExtension;
			return pbstrDefaultExtension.Length;
		}

		public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress) {
			string className = Path.GetFileNameWithoutExtension(wszInputFilePath);
			
			string result;
			try {
				string configPath = ExecutablesCommon.FindConfigFilePath(wszInputFilePath);
				SaltarelleParser parser = configPath != null ? SaltarelleParserFactory.CreateParserFromConfigFile(configPath) : SaltarelleParserFactory.CreateDefaultParser();
			
				XmlDocument doc = ExecutablesCommon.CreateXmlDocument();
				doc.LoadXml(bstrInputFileContents);
				
				result = ExecutablesCommon.GetTemplateCodeFileContents(parser, doc, className, wszDefaultNamespace);
			}
			catch (Exception ex) {
				result = ex.ToString();
			}

			byte[] bytes = Encoding.UTF8.GetBytes(result);
			rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(bytes.Length);
			Marshal.Copy(bytes, 0, rgbOutputFileContents[0], bytes.Length);
			pcbOutput = (uint)bytes.Length;
			return VSConstants.S_OK;
		}
	}
}
