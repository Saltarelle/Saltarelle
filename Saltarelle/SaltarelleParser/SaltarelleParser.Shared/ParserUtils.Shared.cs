using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
		private static readonly Regex UnqualifiedNameRegex = new Regex("^[a-z_][a-z_0-9]*$", "i");
		private static readonly Regex QualifiedNameRegex = new Regex("^(?:[a-z_][a-z_0-9]*\\.)*[a-z_][a-z_0-9]*$", "i");

		public static bool IsValidUnqualifiedName(string id) {
			return !Utils.IsNull(id) && UnqualifiedNameRegex.Exec(id) != null;
		}

		public static bool IsValidQualifiedName(string id) {
			return !Utils.IsNull(id) && QualifiedNameRegex.Exec(id) != null;
		}
#endif

		public static List<IFragment> MergeFragments(IEnumerable<IFragment> fragments) {
			List<IFragment> result = new List<IFragment>();

			IFragment current = null;
			IEnumerator<IFragment> enumerator = null;
			try {
				enumerator = fragments.GetEnumerator();
				if (enumerator.MoveNext()) {
					current = enumerator.Current;
					while (enumerator.MoveNext()) {
						var item = enumerator.Current;
						var merged = current.TryMergeWithNext(item);
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
				if (enumerator is IDisposable)
					((IDisposable)enumerator).Dispose();
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
