using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using jQueryApi;

namespace Saltarelle
{
	[Imported]
	[IgnoreNamespace]
	public static class jQueryExtensions
	{
		/// <summary>
		/// Invokes the bgiframe() method on a jQuery object.
		/// </summary>
		[ScriptName("bgiframe")]
		[InstanceMethodOnFirstArgument]
		public static jQueryObject BGIFrame(this jQueryObject q) {
			return null;
		}

        /// <summary>
        /// Triggers the gotfocus event on each of the matched set of elements.
        /// </summary>
        /// <returns>The current jQueryObject</returns>
        [ScriptName("gotfocus")]
		[InstanceMethodOnFirstArgument]
        public static jQueryObject GotFocus(this jQueryObject q) {
            return null;
        }

        /// <summary>
        /// Attaches a handler to the gotfocus event on each of the matched set of elements.
        /// </summary>
        /// <param name="q">The current jQueryObject</param>
        /// <param name="eventHandler">The event handler to be invoked.</param>
        /// <returns>The current jQueryObject</returns>
        [ScriptName("gotfocus")]
		[InstanceMethodOnFirstArgument]
        public static jQueryObject GotFocus(this jQueryObject q, jQueryEventHandler eventHandler) {
            return null;
        }

        /// <summary>
        /// Attaches a handler to the gotfocus event on each of the matched set of elements.
        /// </summary>
        /// <param name="q">The current jQueryObject</param>
        /// <param name="eventData">Data to be passed in into the event handler.</param>
        /// <param name="eventHandler">The event handler to be invoked.</param>
        /// <returns>The current jQueryObject</returns>
        [ScriptName("gotfocus")]
		[InstanceMethodOnFirstArgument]
        public static jQueryObject GotFocus(this jQueryObject q, JsDictionary eventData, jQueryEventHandler eventHandler) {
            return null;
        }

        /// <summary>
        /// Triggers the lostfocus event on each of the matched set of elements.
        /// </summary>
        /// <returns>The current jQueryObject</returns>
        [ScriptName("lostfocus")]
		[InstanceMethodOnFirstArgument]
        public static jQueryObject LostFocus(this jQueryObject q) {
            return null;
        }

        /// <summary>
        /// Attaches a handler to the lostfocus event on each of the matched set of elements.
        /// </summary>
        /// <param name="q">The current jQueryObject</param>
        /// <param name="eventHandler">The event handler to be invoked.</param>
        /// <returns>The current jQueryObject</returns>
        [ScriptName("lostfocus")]
		[InstanceMethodOnFirstArgument]
        public static jQueryObject LostFocus(this jQueryObject q, jQueryEventHandler eventHandler) {
            return null;
        }

        /// <summary>
        /// Attaches a handler to the lostfocus event on each of the matched set of elements.
        /// </summary>
        /// <param name="q">The current jQueryObject</param>
        /// <param name="eventData">Data to be passed in into the event handler.</param>
        /// <param name="eventHandler">The event handler to be invoked.</param>
        /// <returns>The current jQueryObject</returns>
        [ScriptName("lostfocus")]
		[InstanceMethodOnFirstArgument]
        public static jQueryObject LostFocus(this jQueryObject q, JsDictionary eventData, jQueryEventHandler eventHandler) {
            return null;
        }
	}
}
