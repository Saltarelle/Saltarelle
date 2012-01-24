using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Saltarelle;
using Saltarelle.NodeProcessors;
using Rhino.Mocks;

namespace SaltarelleParser.Tests {
	[TestClass]
	public class ParserUtilsTests {
		private MockRepository mocks;

		private TestContext testContextInstance;

		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		public ParserUtilsTests() {
		}

		[TestInitialize]
		public void SetupRepo() {
			mocks = new MockRepository();
		}

		[TestMethod]
		public void TestMergeFragments_WorksWithEmptySequence() {
			var actual = ParserUtils.MergeFragments(new List<IFragment>());
			Assert.AreEqual(0, actual.Count);
		}

		[TestMethod]
		public void TestMergeFragments_WorksWithOneFragment() {
			var f1 = mocks.StrictMock<IFragment>();
			mocks.ReplayAll();

			var actual = ParserUtils.MergeFragments(new List<IFragment>() { f1 });

			Assert.IsTrue(actual.SequenceEqual(new[] { f1 }));
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestMergeFragments_WorksWithThreeNonMergableFragments() {
			var f1 = mocks.StrictMock<IFragment>();
			var f2 = mocks.StrictMock<IFragment>();
			var f3 = mocks.StrictMock<IFragment>();
			using (mocks.Ordered()) {
				Expect.Call(f1.TryMergeWithNext(f2)).Return(null);
				Expect.Call(f2.TryMergeWithNext(f3)).Return(null);
			}
			mocks.ReplayAll();

			var actual = ParserUtils.MergeFragments(new List<IFragment>() { f1, f2, f3 });

			Assert.IsTrue(actual.SequenceEqual(new[] { f1, f2, f3 }));
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestMergeFragments_WorksWithThreeMergableFragments() {
			var f1 = mocks.StrictMock<IFragment>();
			var f2 = mocks.StrictMock<IFragment>();
			var f3 = mocks.StrictMock<IFragment>();
			var f1_2 = mocks.StrictMock<IFragment>();
			var f1_2_3 = mocks.StrictMock<IFragment>();
			using (mocks.Ordered()) {
				Expect.Call(f1.TryMergeWithNext(f2)).Return(f1_2);
				Expect.Call(f1_2.TryMergeWithNext(f3)).Return(f1_2_3);
			}
			mocks.ReplayAll();

			var actual = ParserUtils.MergeFragments(new List<IFragment>() { f1, f2, f3 });

			Assert.IsTrue(actual.SequenceEqual(new[] { f1_2_3 }));
			mocks.VerifyAll();
		}

		[TestMethod]
		public void TestMergeFragments_WorksWithForFragmentsMergableTwoByTwo() {
			var f1 = mocks.StrictMock<IFragment>();
			var f2 = mocks.StrictMock<IFragment>();
			var f3 = mocks.StrictMock<IFragment>();
			var f4 = mocks.StrictMock<IFragment>();
			var f1_2 = mocks.StrictMock<IFragment>();
			var f3_4 = mocks.StrictMock<IFragment>();
			using (mocks.Ordered()) {
				Expect.Call(f1.TryMergeWithNext(f2)).Return(f1_2);
				Expect.Call(f1_2.TryMergeWithNext(f3)).Return(null);
				Expect.Call(f3.TryMergeWithNext(f4)).Return(f3_4);
			}
			mocks.ReplayAll();

			var actual = ParserUtils.MergeFragments(new List<IFragment>() { f1, f2, f3, f4 });

			Assert.IsTrue(actual.SequenceEqual(new[] { f1_2, f3_4 }));
			mocks.VerifyAll();
		}
	}
}
