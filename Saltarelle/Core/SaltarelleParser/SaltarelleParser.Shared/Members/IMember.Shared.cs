using System;

namespace Saltarelle {
#if SERVER
	public enum MemberCodePoint {
		ServerDefinition    = 0,
		ClientDefinition    = 1,
		ServerIdChanged     = 2,
		ClientIdChanged     = 3,
		ServerConstructor   = 4,
		ClientConstructor   = 5,
		TransferConstructor = 6,
		ConfigObjectInit    = 7,
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
			/// <param name="point">which type of code to write.</param>
			/// <param name="cb">The CodeBuilder instance to write to.</param>
			void WriteCode(ITemplate tpl, MemberCodePoint point, CodeBuilder cb);
		#endif
	}
}
