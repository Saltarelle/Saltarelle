using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using Saltarelle;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SaltarelleParser.Tests {
	public class NodeProcessorTestBase {
		internal MockRepository mocks;
		internal ITemplate template;
		internal IDocumentProcessor docProcessor;
		internal IRenderFunction renderFunction;
		internal List<IFragment> fragments;
		
		internal string ConcatenatedFragments {
			get { return string.Join("", fragments.Select(f => f.ToString()).ToArray()); }
		}

		private TestContext testContextInstance;

		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		[TestInitialize]
		public virtual void SetupRepo() {
			mocks = new MockRepository();
			template = mocks.StrictMock<ITemplate>();
			docProcessor = mocks.StrictMock<IDocumentProcessor>();
			renderFunction = mocks.StrictMock<IRenderFunction>();
			fragments = new List<IFragment>();
			Expect.Call(() => renderFunction.AddFragment(null)).Do((Action<IFragment>)(f => fragments.Add(f))).IgnoreArguments().Repeat.Any();
		}
	}
}
