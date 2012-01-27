using System;
#if SERVER
using FragmentList       = System.Collections.Generic.List<Saltarelle.IFragment>;
using FragmentEnumerator = System.Collections.Generic.IEnumerator<Saltarelle.IFragment>;
using System.Text.RegularExpressions;
#else
using FragmentList       = System.ArrayList;
using FragmentEnumerator = System.Collections.IEnumerator;
#endif

namespace Saltarelle {
	public class ParserUtils {
		public const string RenderFunctionStringBuilderName = "sb";
		public const string ConfigObjectName = "__cfg";

#if SERVER
		private static readonly Regex UnqualifiedNameRegex = new Regex("^[a-z_][a-z_0-9]*$", RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
		private static readonly Regex QualifiedNameRegex = new Regex("^(?:[a-z_][a-z_0-9]*\\.)*[a-z_][a-z_0-9]*$", RegexOptions.ECMAScript | RegexOptions.IgnoreCase);

		public static bool IsValidUnqualifiedName(string id) {
			return !Utils.IsNull(id) && UnqualifiedNameRegex.IsMatch(id);
		}

		public static bool IsValidQualifiedName(string id) {
			return !Utils.IsNull(id) && QualifiedNameRegex.IsMatch(id);
		}
#else
		private static readonly RegularExpression UnqualifiedNameRegex = new RegularExpression("^[a-z_][a-z_0-9]*$", "i");
		private static readonly RegularExpression QualifiedNameRegex = new RegularExpression("^(?:[a-z_][a-z_0-9]*\\.)*[a-z_][a-z_0-9]*$", "i");

		public static bool IsValidUnqualifiedName(string id) {
			return !Utils.IsNull(id) && UnqualifiedNameRegex.Exec(id) != null;
		}

		public static bool IsValidQualifiedName(string id) {
			return !Utils.IsNull(id) && QualifiedNameRegex.Exec(id) != null;
		}
#endif

		public static FragmentList MergeFragments(FragmentList fragments) {
			FragmentList result = new FragmentList();

			IFragment current = null;
			FragmentEnumerator enumerator = null;
			try {
				enumerator = fragments.GetEnumerator();
				if (enumerator.MoveNext()) {
					current = (IFragment)enumerator.Current;
					while (enumerator.MoveNext()) {
						IFragment item = (IFragment)enumerator.Current;
						IFragment merged = current.TryMergeWithNext(item);
						if (Utils.IsNull(merged)) {
							result.Add(current);
							current = item;
						}
						else {
							current = merged;
						}
					}
					result.Add(current);
				}
			}
			finally {
				#if SERVER
					if (!Utils.IsNull(enumerator))
						enumerator.Dispose();
				#endif
			}
			return result;
		}

		public static string MakeTypedMarkupErrorMessage(string registeredPrefix, bool isArray, string value) {
			return MakeTypedMarkupErrorMessage2(registeredPrefix + (isArray ? "[]" : "") + ":" + value);
		}

		internal static string MakeTypedMarkupErrorMessage2(string value) {
			return "Invalid typed markup \"" + value + "\".";
		}
		
		public static Exception TemplateErrorException(string message) {
			#if SERVER
				return new TemplateErrorException(message);
			#else
				return new Exception("Error in template: " + message);
			#endif
		}
	}
}
