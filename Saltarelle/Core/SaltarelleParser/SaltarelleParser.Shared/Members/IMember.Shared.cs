using System;

namespace Saltarelle {
	public enum AccessModifier {
		_Public            = 0,
		_Protected         = 1,
		_Private           = 2,
		_Internal          = 3,
		_ProtectedInternal = 4,
		_None              = 5
	}
	
	public static class AccessModifierHelper {
		public static string WriteDeclarator(AccessModifier am, string type, string name) {
			string s;
			switch (am) {
				case AccessModifier._Public:            s = "public "; break;
				case AccessModifier._Protected:         s = "protected "; break;
				case AccessModifier._Private:           s = "private "; break;
				case AccessModifier._Internal:          s = "internal "; break;
				case AccessModifier._ProtectedInternal: s = "protected internal "; break;
				case AccessModifier._None:              s = ""; break;
				default: throw Utils.ArgumentException("am");
			}
			return s + type + " " + name;
		}
	}

#if SERVER
	public enum MemberCodePoint {
		ServerDefinition    = 0,
		ClientDefinition    = 1,
		ServerIdChanging    = 2,
		ClientIdChanging    = 3,
		ServerConstructor   = 4,
		ClientConstructor   = 5,
		TransferConstructor = 6,
		ConfigObjectInit    = 7,
		Attach              = 8,
		AttachSelf          = 9
	}
#endif

	public interface IMember {
		/// <summary>
		/// The name of the member, which must be a valid C# unqualified identifier
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// The names of all members upon which this member is dependent.
		/// </summary>
		string[] Dependencies { get; }
		
		/// <summary>
		/// Instantiate this member in the specified control.
		/// </summary>
		/// <param name="tpl">The template that is being instantiated.</param>
		/// <param name="control">The control into which the template is being instantiated.</param>
		void Instantiate(ITemplate tpl, IInstantiatedTemplateControl control);

		#if SERVER
			/// <summary>
			/// Write code for the member
			/// </summary>
			/// <param name="tpl">Template for which code is being generated.</param>
			/// <param name="point">Which type of code to write.</param>
			/// <param name="cb">The CodeBuilder instance to write to.</param>
			void WriteCode(ITemplate tpl, MemberCodePoint point, CodeBuilder cb);
		#endif
	}
}
