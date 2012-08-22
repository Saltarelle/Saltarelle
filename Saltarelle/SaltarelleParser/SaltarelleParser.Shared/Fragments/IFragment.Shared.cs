using System;
using System.Text;

namespace Saltarelle {
#if SERVER
	public enum FragmentCodePoint {
		ServerRender = 0,
		ClientRender = 1
	}
#endif

	public interface IFragment {
		/// <summary>
		/// Try to merge this frament with the next one
		/// </summary>
		/// <param name="nextFragment">The next fragment</param>
		/// <returns>A new fragment if the merge was successful, null if the fragments couldn't be merged</returns>
		IFragment TryMergeWithNext(IFragment nextFragment);
		
		/// <summary>
		/// Render this fragment to a StringBuilder (used when instantiating a non-compiled template)
		/// </summary>
		/// <param name="tpl">The template which is being rendered.</param>
		/// <param name="ctl">The instanitiated template.</param>
		/// <param name="sb">StringBuilder instance to render to.</param>
		void Render(ITemplate tpl, IInstantiatedTemplateControl ctl, StringBuilder sb);

		#if SERVER
			/// <summary>
			/// Write the code necessary for rendering this fragment inside a rendering method.
			/// </summary>
			/// <param name="tpl">The template for which code is being generated.</param>
			/// <param name="point">Type of code to generate.</param>
			/// <param name="cb">The CodeBuilder to write the code to.</param>
			void WriteCode(ITemplate tpl, FragmentCodePoint point, CodeBuilder cb);
		#endif
	}
}
