using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Saltarelle.CastleWindsor;
using Saltarelle.Configuration;
using Saltarelle.Ioc;
using Saltarelle.CastleWindsor.ExtensionMethods;

namespace Saltarelle {
	[Serializable]
	public class WorkItem {
		public string InFile    { get; set; }
		public string OutFile   { get; set; }
		public string Namespace { get; set; }
	}

	public static class ExecutablesCommon {
        class Executor : MarshalByRefObject {
            private ISaltarelleParser parser;

            public void CreateParser(string configFileName) {
                IEnumerable<Assembly> plugins;
                if (!string.IsNullOrEmpty(configFileName)) {
                    var config = SaltarelleConfig.LoadFile(configFileName);
                    plugins = config.LoadPluginsInNoContext(Path.GetDirectoryName(configFileName));
                }
                else
                    plugins = new Assembly[0];

				var windsorContainer = new WindsorContainer();
				ContainerFactory.PrepareWindsorContainer(windsorContainer);
				foreach (var p in plugins)
					windsorContainer.RegisterPluginsFromAssembly(p);
				var container = ContainerFactory.CreateContainer(windsorContainer);
                parser = SaltarelleParserFactory.CreateParserWithPlugins(plugins, container);
            }

            public void ProcessWorkItem(WorkItem workItem) {
				XmlDocument doc = CreateXmlDocument();
				doc.Load(workItem.InFile);
				string outText = GetTemplateCodeFileContents(parser, doc, Path.GetFileNameWithoutExtension(workItem.InFile), workItem.Namespace);
				byte[] outBytes = Encoding.UTF8.GetBytes(outText);
				MaybeWriteFile(workItem.OutFile, outBytes);
            }

            public string ProcessContent(string content, string className, string nmspace) {
				XmlDocument doc = CreateXmlDocument();
				doc.LoadXml(content);
				return GetTemplateCodeFileContents(parser, doc, className, nmspace);
            }

		    private XmlDocument CreateXmlDocument() {
			    return new XmlDocument() { PreserveWhitespace = true };
		    }

		    private void MaybeWriteFile(string path, byte[] content) {
			    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate)) {
				    bool areEqual = false;
				    if (fs.Length == content.Length) {
				        byte[] oldContent = new byte[fs.Length];
					    fs.Read(oldContent, 0, oldContent.Length);
				        areEqual = !content.Where((b, i) => b != oldContent[i]).Any();
				    }
				    if (!areEqual) {
					    fs.Seek(0, SeekOrigin.Begin);
					    fs.Write(content, 0, content.Length);
					    fs.SetLength(content.Length);
				    }
			    }
		    }

		    private string GetTemplateCodeFileContents(ISaltarelleParser parser, XmlDocument doc, string className, string nmspace) {
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

		public const string GeneratedFileExtension = ".Template.cs";
		public const string ConfigFileName = "saltarelle.config";
		
		public static string FindConfigFilePath(string inputFilePath) {
			string currentDir = Path.GetDirectoryName(inputFilePath);
			while (currentDir != null) {
				string s = Path.Combine(currentDir, ConfigFileName);
				if (File.Exists(s))
					return s;
				currentDir = Path.GetDirectoryName(currentDir);
			}
			return null;
		}

        private static void ExecuteInSeparateAppDomain(Action<Executor> action) {
            var setup = new AppDomainSetup() { ApplicationBase = HttpUtility.UrlDecode(Path.GetDirectoryName(new Uri(typeof(ExecutablesCommon).Assembly.CodeBase).AbsolutePath)) };
            var newDomain = AppDomain.CreateDomain("Executor", null, setup);
            newDomain.AssemblyResolve += (sender, eventArgs) => ResolveAssembly(eventArgs.Name);

            var executor = (Executor)newDomain.CreateInstanceAndUnwrap(typeof(Executor).Assembly.FullName, typeof(Executor).FullName);
            action(executor);

            AppDomain.Unload(newDomain);
        }

        public static void ProcessWorkItemsInSeparateAppDomain(string configFileName, IEnumerable<WorkItem> workItems, Func<WorkItem, Exception, bool> errorCallback) {
            ExecuteInSeparateAppDomain(executor => {
                executor.CreateParser(configFileName);

                foreach (var wi in workItems) {
                    try {
                        executor.ProcessWorkItem(wi);
                    }
                    catch (Exception ex) {
                        if (errorCallback == null || !errorCallback(wi, ex))
                            throw;
                    }
                }
            });
        }

        public static string ProcessContentInSeparateAppDomain(string configFileName, string content, string className, string nmspace) {
            var result = "";
            ExecuteInSeparateAppDomain(executor => {
                executor.CreateParser(configFileName);
                result = executor.ProcessContent(content, className, nmspace);
            });
            return result;
        }

        private static Assembly ResolveAssembly(string name) {
            var parsedName = new AssemblyName(name);
            if (parsedName.Name == "SaltarelleLib" || parsedName.Name == "SaltarelleParser" || parsedName.Name == "Newtonsoft.Json" || parsedName.Name == "Castle.Core" || parsedName.Name == "Castle.Windsor" || parsedName.Name == "Saltarelle.CastleWindsor")
                return Assembly.GetExecutingAssembly(); // These assemblies are ILMerged.
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == parsedName.Name);    // Allow other plugins to be referenced, even though they have all been loaded without context.
        }
	}
}
