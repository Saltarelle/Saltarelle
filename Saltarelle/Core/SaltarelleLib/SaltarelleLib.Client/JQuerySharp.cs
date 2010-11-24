using System;
using System.DHTML;

//Automatically generated jquery to script# code by Bjarte Djuvik Næss, www.bjarte.com
namespace Saltarelle
{
    [Imported, IgnoreNamespace]
    public delegate object EvalJsonDelegate(string s);

    [Imported, IgnoreNamespace]
    public delegate bool EachCallback(int index, DOMElement value);

    [Imported]
    [IgnoreNamespace]
    public delegate Object BasicCallback(Object obj1, Object obj2);

    [Imported, IgnoreNamespace]
    public delegate void JQueryEventHandlerDelegate(JQueryEvent evt);
    [Imported, IgnoreNamespace]
    public delegate void UnwrappedJQueryEventHandlerDelegate(DOMElement _this, JQueryEvent evt);
    [Imported, IgnoreNamespace]
    public delegate bool JQueryEventCancelHandlerDelegate(JQueryEvent evt);
    [Imported, IgnoreNamespace]
    public delegate bool UnwrappedJQueryEventCancelHandlerDelegate(DOMElement _this, JQueryEvent evt);

    [Imported, IgnoreNamespace]
    public delegate void DraggableEventHandlerDelegate(JQueryEvent evt, DraggableEventObject ui);
    [Imported, IgnoreNamespace]
    public delegate void UnwrappedDraggableEventHandlerDelegate(DOMElement _this, JQueryEvent evt, DraggableEventObject ui);
    [Imported, IgnoreNamespace]
    public delegate jQuery DraggableHelperDelegate();

    [Imported, IgnoreNamespace]
    public delegate void ResizableEventHandlerDelegate(JQueryEvent evt, ResizableEventObject ui);
    [Imported, IgnoreNamespace]
    public delegate void UnwrappedResizableEventHandlerDelegate(DOMElement _this, JQueryEvent evt, ResizableEventObject ui);

	[Imported, IgnoreNamespace]
	public delegate void DroppableEventHandlerDelegate(JQueryEvent evt, DroppableEventObject ui);
	[Imported, IgnoreNamespace]
	public delegate void UnwrappedDroppableEventHandlerDelegate(DOMElement _this, JQueryEvent evt, DroppableEventObject ui);

	[Imported, IgnoreNamespace]
	public delegate bool TabsSelectEventHandlerDelegate(JQueryEvent evt, TabsEventObject ui);
	[Imported, IgnoreNamespace]
	public delegate void TabsEventHandlerDelegate(JQueryEvent evt, TabsEventObject ui);

    [Imported, IgnoreNamespace]
    public delegate void JQGridRowSelectedEventHandler(int rowid);
    [Imported, IgnoreNamespace]
    public delegate void UnwrappedJQGridRowSelectedEventHandler(DOMElement _this, int rowid);

	[Record]
	public sealed class LeftTop {
		[PreserveCase]
		public double left;
		[PreserveCase]
		public double top;
		
		public LeftTop(int left, int top) {
			this.left = left;
			this.top  = top;
		}
	}
	
	[Record]
	public sealed class WidthHeight {
		[PreserveCase]
		public double width;
		[PreserveCase]
		public double height;
		
		public WidthHeight(double width, double height) {
			this.width  = width;
			this.height = height;
		}
	}

	[Imported]
    [IgnoreNamespace]
	public sealed class DraggableEventObject {
		[PreserveCase]
		public jQuery helper;
		[PreserveCase]
		public LeftTop position;
		[PreserveCase]
		public LeftTop offset;
	}

	[Imported]
    [IgnoreNamespace]
	public sealed class ResizableEventObject {
		[PreserveCase]
		public jQuery helper;
		[PreserveCase]
		public LeftTop originalPosition;
		[PreserveCase]
		public WidthHeight originalSize;
		[PreserveCase]
		public LeftTop position;
		[PreserveCase]
		public WidthHeight size;
	}
	
	[Imported]
    [IgnoreNamespace]
	public sealed class DroppableEventObject {
		[PreserveCase]
		public jQuery draggable;
		[PreserveCase]
		public jQuery helper;
		[PreserveCase]
		public LeftTop position;
		[PreserveCase]
		public LeftTop offset;
	}
	
	[Imported]
    [IgnoreNamespace]
	public sealed class TabsEventObject {
		[PreserveCase]
		public jQuery tab;
		[PreserveCase]
		public jQuery panel;
		[PreserveCase]
		public int index;
	}
	
	[Imported]
    [IgnoreNamespace]
	public sealed class JQueryEvent {
		[PreserveCase]
		public bool altKey;
		[PreserveCase]
		public int button;
		[PreserveCase]
		public char charCode;
		[PreserveCase]
		public bool ctrlKey;
		[PreserveCase]
		public int keyCode;
		[PreserveCase]
		public Event originalEvent;

		[PreserveCase]
		public string type;
		[PreserveCase]
		public DOMElement target;
		[PreserveCase]
		public object data;
		[PreserveCase]
		public DOMElement relatedTarget;
		[PreserveCase]
		public DOMElement currentTarget;
		[PreserveCase]
		public string pageX; // is a string according to jquery.com. Perhaps not?
		[PreserveCase]
		public string pageY;
		[PreserveCase]
		public object result;
		[PreserveCase]
		public double timeStamp;

		[PreserveCase]
		public void preventDefault() {}
		[PreserveCase]
		public bool isDefaultPrevented() { return false; }
		[PreserveCase]
		public void stopPropagation() {}
		[PreserveCase]
		public bool isPropagationStopped() { return false; }
		[PreserveCase]
		public void stopImmediatePropagation() {}
		[PreserveCase]
		public bool isImmediatePropagationStopped() { return false; }
	}
	
	[Imported]
    [IgnoreNamespace]
	public sealed class JQueryBrowser {
		[PreserveCase]
		public bool mozilla;
		[PreserveCase]
		public bool msie;
		[PreserveCase]
		public bool opera;
		[PreserveCase]
		public bool safari;
		[PreserveCase]
		public string version;
	}

    [GlobalMethods]
    [IgnoreNamespace]
    [Imported]
    public static class JQueryProxy
    {
        [PreserveCase]
        public static jQuery jQuery(string exp)
        {
            return null;
        }

        [PreserveCase]
        public static jQuery jQuery(DOMElement elm)
        {
            return null;
        }
        
        [PreserveCase]
        public static jQuery jQuery(DOMDocument elm)
        {
            return null;
        }

        [PreserveCase]
        public static jQuery jQuery(DOMElement[] elms)
        {
			return null;
        }
    }

    [Imported]
    [IgnoreNamespace]
    public partial class jQuery
    {
        //methods goes here
        ///<summary>The current version of jQuery.</summary>
        [PreserveCase]
        public String jquery() { return null; }

        ///<summary>
        ///      Get the number of elements currently matched. This returns the same
        ///      number as the 'length' property of the jQuery object.
        ///    </summary>
        [PreserveCase]
        public int size() { return 0; }

        ///<summary>
        ///      Access all matched DOM elements. This serves as a backwards-compatible
        ///      way of accessing all matched elements (other than the jQuery object
        ///      itself, which is, in fact, an array of elements).
        ///
        ///      It is useful if you need to operate on the DOM elements themselves instead of using built-in jQuery functions.
        ///    </summary>
        [PreserveCase]
        public DOMElement[] get() { return null; }

        ///<summary>
        ///      Access a single matched DOM element at a specified index in the matched set.
        ///      This allows you to extract the actual DOM element and operate on it
        ///      directly without necessarily using jQuery functionality on it.
        ///    </summary>
        [PreserveCase]
        public DOMElement get(int num) { return null; }

        ///<summary>
        ///      Set the jQuery object to an array of elements, while maintaining
        ///      the stack.
        ///    </summary>
        [PreserveCase]
        public jQuery pushStack(DOMElement[] elems) { return null; }

        ///<summary>
        ///      Set the jQuery object to an array of elements. This operation is
        ///      completely destructive - be sure to use .pushStack() if you wish to maintain
        ///      the jQuery stack.
        ///    </summary>
        [PreserveCase]
        public jQuery setArray(DOMElement[] elems) { return null; }

        ///<summary>
        ///      Execute a function within the context of every matched element.
        ///      This means that every time the passed-in function is executed
        ///      (which is once for every element matched) the 'this' keyword
        ///      points to the specific DOM element.
        ///
        ///      Additionally, the function, when executed, is passed a single
        ///      argument representing the position of the element in the matched
        ///      set (integer, zero-index).
        ///    </summary>
        [PreserveCase]
        public jQuery each(EachCallback fn) { return null; }

        ///<summary>
        ///      Searches every matched element for the object and returns
        ///      the index of the element, if found, starting with zero.
        ///      Returns -1 if the object wasn't found.
        ///    </summary>
        [PreserveCase]
        public int index(DOMElement subject) { return 0; }

        ///<summary>
        ///      Access a property on the first matched element.
        ///      This method makes it easy to retrieve a property value
        ///      from the first matched element.
        ///
        ///      If the element does not have an attribute with such a
        ///      name, undefined is returned.
        ///    </summary>
        [PreserveCase]
        public Object attr(String name) { return null; }

        ///<summary>
        ///      Set a key/value object as properties to all matched elements.
        ///
        ///      This serves as the best way to set a large number of properties
        ///      on all matched elements.
        ///    </summary>
        [PreserveCase]
        public jQuery attr(Object properties) { return null; }

        ///<summary>
        ///      Set a single property to a value, on all matched elements.
        ///
        ///      Note that you can't set the name property of input elements in IE.
        ///      Use $(html) or .append(html) or .html(html) to create elements
        ///      on the fly including the name property.
        ///    </summary>
        [PreserveCase]
        public jQuery attr(String key, Object value) { return null; }

        ///<summary>
        ///      Set a single property to a computed value, on all matched elements.
        ///
        ///      Instead of supplying a string value as described
        ///      [[DOM/Attributes#attr.28_key.2C_value_.29|above]],
        ///      a function is provided that computes the value.
        ///    </summary>
        [PreserveCase]
        public jQuery attr(String key, BasicCallback value) { return null; }

        ///<summary>
        ///      Access a style property on the first matched element.
        ///      This method makes it easy to retrieve a style property value
        ///      from the first matched element.
        ///    </summary>
        [PreserveCase]
        public String css(String name) { return null; }

        ///<summary>
        ///      Set a key/value object as style properties to all matched elements.
        ///
        ///      This serves as the best way to set a large number of style properties
        ///      on all matched elements.
        ///    </summary>
        [PreserveCase]
        public jQuery css(Object properties) { return null; }

        ///<summary>
        ///      Set a single style property to a value, on all matched elements.
        ///      If a number is provided, it is automatically converted into a pixel value.
        ///    </summary>
        [PreserveCase]
        public jQuery css(String key, String value) { return null; }

        ///<summary>
        ///      Set a single style property to a value, on all matched elements.
        ///      If a number is provided, it is automatically converted into a pixel value.
        ///    </summary>
        [PreserveCase]
        public jQuery css(String key, int value) { return null; }

        ///<summary>
        ///      Get the text contents of all matched elements. The result is
        ///      a string that contains the combined text contents of all matched
        ///      elements. This method works on both HTML and XML documents.
        ///    </summary>
        [PreserveCase]
        public String text() { return null; }

        ///<summary>
        ///      Set the text contents of all matched elements.
        ///
        ///      Similar to html(), but escapes HTML (replace "&lt;" and "&gt;" with their
        ///      HTML entities).
        ///    </summary>
        [PreserveCase]
        public String text(String val) { return null; }

        ///<summary>
        ///      Wrap all matched elements with a structure of other elements.
        ///      This wrapping process is most useful for injecting additional
        ///      stucture into a document, without ruining the original semantic
        ///      qualities of a document.
        ///
        ///      This works by going through the first element
        ///      provided (which is generated, on the fly, from the provided HTML)
        ///      and finds the deepest ancestor element within its
        ///      structure - it is that element that will en-wrap everything else.
        ///
        ///      This does not work with elements that contain text. Any necessary text
        ///      must be added after the wrapping is done.
        ///    </summary>
        [PreserveCase]
        public jQuery wrap(String html) { return null; }

        ///<summary>
        ///      Wrap all matched elements with a structure of other elements.
        ///      This wrapping process is most useful for injecting additional
        ///      stucture into a document, without ruining the original semantic
        ///      qualities of a document.
        ///
        ///      This works by going through the first element
        ///      provided and finding the deepest ancestor element within its
        ///      structure - it is that element that will en-wrap everything else.
        ///
        ///      This does not work with elements that contain text. Any necessary text
        ///      must be added after the wrapping is done.
        ///    </summary>
        [PreserveCase]
        public jQuery wrap(DOMElement elem) { return null; }

        ///<summary>
        ///      Append content to the inside of every matched element.
        ///
        ///      This operation is similar to doing an appendChild to all the
        ///      specified elements, adding them into the document.
        ///    </summary>
        [PreserveCase]
        public jQuery append(Object content) { return null; }

        ///<summary>
        ///      Prepend content to the inside of every matched element.
        ///
        ///      This operation is the best way to insert elements
        ///      inside, at the beginning, of all matched elements.
        ///    </summary>
        [PreserveCase]
        public jQuery prepend(Object content) { return null; }

        ///<summary>Insert content before each of the matched elements.</summary>
        [PreserveCase]
        public jQuery before(Object content) { return null; }

        ///<summary>Insert content after each of the matched elements.</summary>
        [PreserveCase]
        public jQuery after(Object content) { return null; }

        ///<summary>
        ///      Revert the most recent 'destructive' operation, changing the set of matched elements
        ///      to its previous state (right before the destructive operation).
        ///
        ///      If there was no destructive operation before, an empty set is returned.
        ///
        ///      A 'destructive' operation is any operation that changes the set of
        ///      matched jQuery elements. These functions are: <code>add</code>,
        ///      <code>children</code>, <code>clone</code>, <code>filter</code>,
        ///      <code>find</code>, <code>not</code>, <code>next</code>,
        ///      <code>parent</code>, <code>parents</code>, <code>prev</code> and <code>siblings</code>.
        ///    </summary>
        [PreserveCase]
        public jQuery end() { return null; }

        ///<summary>
        ///      Searches for all elements that match the specified expression.
        ///      This method is a good way to find additional descendant
        ///      elements with which to process.
        ///
        ///      All searching is done using a jQuery expression. The expression can be
        ///      written using CSS 1-3 Selector syntax, or basic XPath.
        ///    </summary>
        [PreserveCase]
        public jQuery find(String expr) { return null; }

        ///<summary>
        ///      Clone matched DOM Elements and select the clones.
        ///
        ///      This is useful for moving copies of the elements to another
        ///      location in the DOM.
        ///    </summary>
        [PreserveCase]
        public jQuery clone(bool deep) { return null; }

        ///<summary>
        ///      Removes all elements from the set of matched elements that do not
        ///      match the specified expression(s). This method is used to narrow down
        ///      the results of a search.
        ///
        ///      Provide a comma-separated list of expressions to apply multiple filters at once.
        ///    </summary>
        [PreserveCase]
        public jQuery filter(String expression) { return null; }

        ///<summary>
        ///      Removes all elements from the set of matched elements that do not
        ///      pass the specified filter. This method is used to narrow down
        ///      the results of a search.
        ///    </summary>
        [PreserveCase]
        public jQuery filter(BasicCallback filter) { return null; }

        ///<summary>
        ///      Removes the specified Element from the set of matched elements. This
        ///      method is used to remove a single Element from a jQuery object.
        ///    </summary>
        [PreserveCase]
        public jQuery not(DOMElement el) { return null; }

        ///<summary>
        ///      Removes elements matching the specified expression from the set
        ///      of matched elements. This method is used to remove one or more
        ///      elements from a jQuery object.
        ///    </summary>
        [PreserveCase]
        public jQuery not(String expr) { return null; }

        ///<summary>
        ///      Removes any elements inside the array of elements from the set
        ///      of matched elements. This method is used to remove one or more
        ///      elements from a jQuery object.
        ///
        ///      Please note: the expression cannot use a reference to the
        ///      element name. See the two examples below.
        ///    </summary>
        [PreserveCase]
        public jQuery not(jQuery elems) { return null; }

        ///<summary>
        ///      Adds more elements, matched by the given expression,
        ///      to the set of matched elements.
        ///    </summary>
        [PreserveCase]
        public jQuery add(String expr) { return null; }

        ///<summary>Adds one or more Elements to the set of matched elements.</summary>
        [PreserveCase]
        public jQuery add(DOMElement elements) { return null; }

        ///<summary>Adds one or more Elements to the set of matched elements.</summary>
        [PreserveCase]
        public jQuery add(DOMElement[] elements) { return null; }
        
        [PreserveCase]
        public jQuery andSelf() { return null; }

        ///<summary>
        ///      Checks the current selection against an expression and returns true,
        ///      if at least one element of the selection fits the given expression.
        ///
        ///      Does return false, if no element fits or the expression is not valid.
        ///
        ///      filter(String) is used internally, therefore all rules that apply there
        ///      apply here, too.
        ///    </summary>
        [PreserveCase]
        public bool isInExpression(String expr) { return false; }

        ///<summary>
        ///      Get the content of the value attribute of the first matched element.
        ///
        ///      Use caution when relying on this function to check the value of
        ///      multiple-select elements and checkboxes in a form. While it will
        ///      still work as intended, it may not accurately represent the value
        ///      the server will receive because these elements may send an array
        ///      of values. For more robust handling of field values, see the
        ///      [http://www.malsup.com/jquery/form/#fields fieldValue function of the Form Plugin].
        ///    </summary>
        [PreserveCase]
        public String val() { return null; }

        ///<summary>	Set the value attribute of every matched element.</summary>
        [PreserveCase]
        public jQuery val(String val) { return null; }

        ///<summary>
        ///      Get the html contents of the first matched element.
        ///      This property is not available on XML documents.
        ///    </summary>
        [PreserveCase]
        public String html() { return null; }

        ///<summary>
        ///      Set the html contents of every matched element.
        ///      This property is not available on XML documents.
        ///    </summary>
        [PreserveCase]
        public jQuery html(String val) { return null; }

        ///<summary></summary>
        [PreserveCase]
        public jQuery domManip(Object[] args, bool table, int dir, BasicCallback fn) { return null; }

        ///<summary>
        ///      Get a set of elements containing the unique parents of the matched
        ///      set of elements.
        ///
        ///      You may use an optional expression to filter the set of parent elements that will match.
        ///    </summary>
        [PreserveCase]
        public jQuery parent(String expr) { return null; }

        ///<summary>
        ///      Get a set of elements containing the unique parents of the matched
        ///      set of elements.
        ///
        ///      You may use an optional expression to filter the set of parent elements that will match.
        ///    </summary>
        [PreserveCase]
        public jQuery parent() { return null; }

        ///<summary>
        ///      Get a set of elements containing the unique ancestors of the matched
        ///      set of elements (except for the root element).
        ///
        ///      The matched elements can be filtered with an optional expression.
        ///    </summary>
        [PreserveCase]
        public jQuery parents(String expr) { return null; }

        ///<summary>
        ///      Get a set of elements containing the unique next siblings of each of the
        ///      matched set of elements.
        ///
        ///      It only returns the very next sibling for each element, not all
        ///      next siblings.
        ///
        ///      You may provide an optional expression to filter the match.
        ///    </summary>
        [PreserveCase]
        public jQuery next(String expr) { return null; }

        ///<summary>
        ///      Get a set of elements containing the unique next siblings of each of the
        ///      matched set of elements.
        ///
        ///      It only returns the very next sibling for each element, not all
        ///      next siblings.
        ///
        ///      You may provide an optional expression to filter the match.
        ///    </summary>
        [PreserveCase]
        public jQuery next() { return null; }

        ///<summary>
        ///      Get a set of elements containing the unique previous siblings of each of the
        ///      matched set of elements.
        ///
        ///      Use an optional expression to filter the matched set.
        ///
        ///      Only the immediately previous sibling is returned, not all previous siblings.
        ///    </summary>
        [PreserveCase]
        public jQuery prev(String expr) { return null; }

        ///<summary>
        ///      Get a set of elements containing the unique previous siblings of each of the
        ///      matched set of elements.
        ///
        ///      Use an optional expression to filter the matched set.
        ///
        ///      Only the immediately previous sibling is returned, not all previous siblings.
        ///    </summary>
        [PreserveCase]
        public jQuery prev() { return null; }

        ///<summary>
        ///      Get a set of elements containing all of the unique siblings of each of the
        ///      matched set of elements.
        ///
        ///      Can be filtered with an optional expressions.
        ///    </summary>
        [PreserveCase]
        public jQuery siblings(String expr) { return null; }

        ///<summary>
        ///      Get a set of elements containing all of the unique children of each of the
        ///      matched set of elements.
        ///
        ///      This set can be filtered with an optional expression that will cause
        ///      only elements matching the selector to be collected.
        ///    </summary>
        [PreserveCase]
        public jQuery children(String expr) { return null; }

        ///<summary>
        ///      Get a set of elements containing all of the unique children of each of the
        ///      matched set of elements.
        ///
        ///      This set can be filtered with an optional expression that will cause
        ///      only elements matching the selector to be collected.
        ///    </summary>
        [PreserveCase]
        public jQuery children() { return null; }

        ///<summary>
        ///      Append all of the matched elements to another, specified, set of elements.
        ///      This operation is, essentially, the reverse of doing a regular
        ///      $(A).append(B), in that instead of appending B to A, you're appending
        ///      A to B.
        ///    </summary>
        [PreserveCase]
        public jQuery appendTo(Object content) { return null; }

        ///<summary>
        ///      Prepend all of the matched elements to another, specified, set of elements.
        ///      This operation is, essentially, the reverse of doing a regular
        ///      $(A).prepend(B), in that instead of prepending B to A, you're prepending
        ///      A to B.
        ///    </summary>
        [PreserveCase]
        public jQuery prependTo(Object content) { return null; }

        ///<summary>
        ///      Insert all of the matched elements before another, specified, set of elements.
        ///      This operation is, essentially, the reverse of doing a regular
        ///      $(A).before(B), in that instead of inserting B before A, you're inserting
        ///      A before B.
        ///    </summary>
        [PreserveCase]
        public jQuery insertBefore(Object content) { return null; }

        ///<summary>
        ///      Insert all of the matched elements after another, specified, set of elements.
        ///      This operation is, essentially, the reverse of doing a regular
        ///      $(A).after(B), in that instead of inserting B after A, you're inserting
        ///      A after B.
        ///    </summary>
        [PreserveCase]
        public jQuery insertAfter(Object content) { return null; }

        ///<summary>Remove an attribute from each of the matched elements.</summary>
        [PreserveCase]
        public jQuery removeAttr(String name) { return null; }

        ///<summary>Adds the specified class(es) to each of the set of matched elements.</summary>
        [PreserveCase]
        public jQuery addClass(String cssClass) { return null; }

        ///<summary>Removes all or the specified class(es) from the set of matched elements.</summary>
        [PreserveCase]
        public jQuery removeClass(String cssClass) { return null; }

        ///<summary>
        ///      Adds the specified class if it is not present, removes it if it is
        ///      present.
        ///    </summary>
        [PreserveCase]
        public jQuery toggleClass(String cssClass) { return null; }

        [PreserveCase]
        public jQuery toggleClass(String cssClass, bool set) { return null; }

        ///<summary>
        ///      Removes all matched elements from the DOM. This does NOT remove them from the
        ///      jQuery object, allowing you to use the matched elements further.
        ///
        ///      Can be filtered with an optional expressions.
        ///    </summary>
        [PreserveCase]
        public jQuery remove(String expr) { return null; }
        [PreserveCase]
        public jQuery remove() { return null; }

        ///<summary>Removes all child nodes from the set of matched elements.</summary>
        [PreserveCase]
        public jQuery empty() { return null; }

        ///<summary>
        ///      Reduce the set of matched elements to a single element.
        ///      The position of the element in the set of matched elements
        ///      starts at 0 and goes to length - 1.
        ///    </summary>
        [PreserveCase]
        public jQuery eq(int pos) { return null; }

        ///<summary>Filter the set of elements to those that contain the specified text.</summary>
        [PreserveCase]
        public jQuery contains(String str) { return null; }

        ///<summary>Get the current computed, pixel, width of the first matched element.</summary>
        [PreserveCase]
        public double width() { return 0; }

        ///<summary>
        ///      Set the CSS width of every matched element. If no explicit unit
        ///      was specified (like 'em' or '%') then "px" is added to the width.
        ///    </summary>
        [PreserveCase]
        public jQuery width(String val) { return null; }

        ///<summary>
        ///      Set the CSS width of every matched element. If no explicit unit
        ///      was specified (like 'em' or '%') then "px" is added to the width.
        ///    </summary>
        [PreserveCase]
        public jQuery width(int val) { return null; }

        ///<summary>Get the current computed, pixel, height of the first matched element.</summary>
        [PreserveCase]
        public double height() { return 0; }

        ///<summary>
        ///      Set the CSS height of every matched element. If no explicit unit
        ///      was specified (like 'em' or '%') then "px" is added to the width.
        ///    </summary>
        [PreserveCase]
        public jQuery height(String val) { return null; }

        ///<summary>
        ///      Set the CSS height of every matched element. If no explicit unit
        ///      was specified (like 'em' or '%') then "px" is added to the width.
        ///    </summary>
        [PreserveCase]
        public jQuery height(int val) { return null; }

        ///<summary>
        ///      Binds a handler to a particular event (like click) for each matched element.
        ///      The event handler is passed an event object that you can use to prevent
        ///      default behaviour. To stop both default action and event bubbling, your handler
        ///      has to return false.
        ///
        ///      In most cases, you can define your event handlers as anonymous functions
        ///      (see first example). In cases where that is not possible, you can pass additional
        ///      data as the second parameter (and the handler function as the third), see
        ///      second example.
        ///    </summary>
        [PreserveCase]
        public jQuery bind(String type, Object data, Delegate fn) { return null; }

        ///<summary>
        ///      Binds a handler to a particular event (like click) for each matched element.
        ///      The handler is executed only once for each element. Otherwise, the same rules
        ///      as described in bind() apply.
        ///      The event handler is passed an event object that you can use to prevent
        ///      default behaviour. To stop both default action and event bubbling, your handler
        ///      has to return false.
        ///
        ///      In most cases, you can define your event handlers as anonymous functions
        ///      (see first example). In cases where that is not possible, you can pass additional
        ///      data as the second paramter (and the handler function as the third), see
        ///      second example.
        ///    </summary>
        [PreserveCase]
        public jQuery one(String type, Object data, Delegate fn) { return null; }

        ///<summary>
        ///      The opposite of bind, removes a bound event from each of the matched
        ///      elements.
        ///
        ///      Without any arguments, all bound events are removed.
        ///
        ///      If the type is provided, all bound events of that type are removed.
        ///
        ///      If the function that was passed to bind is provided as the second argument,
        ///      only that specific event handler is removed.
        ///    </summary>
        [PreserveCase]
        public jQuery unbind(String type, Delegate fn) { return null; }

        ///<summary>
        ///      Trigger a type of event on every matched element. This will also cause
        ///      the default action of the browser with the same name (if one exists)
        ///      to be executed. For example, passing 'submit' to the trigger()
        ///      function will also cause the browser to submit the form. This
        ///      default action can be prevented by returning false from one of
        ///      the functions bound to the event.
        ///
        ///      You can also trigger custom events registered with bind.
        ///    </summary>
        [PreserveCase]
        public jQuery trigger(String type, Object[] data) { return null; }

        ///<summary>
        ///      Trigger a type of event on every matched element. This will also cause
        ///      the default action of the browser with the same name (if one exists)
        ///      to be executed. For example, passing 'submit' to the trigger()
        ///      function will also cause the browser to submit the form. This
        ///      default action can be prevented by returning false from one of
        ///      the functions bound to the event.
        ///
        ///      You can also trigger custom events registered with bind.
        ///    </summary>
        [PreserveCase]
        public jQuery trigger(String type) { return null; }

        ///<summary>
        ///      Toggle between two function calls every other click.
        ///      Whenever a matched element is clicked, the first specified function
        ///      is fired, when clicked again, the second is fired. All subsequent
        ///      clicks continue to rotate through the two functions.
        ///
        ///      Use unbind("click") to remove.
        ///    </summary>
        [PreserveCase]
        public jQuery toggle(JQueryEventHandlerDelegate even, JQueryEventHandlerDelegate odd) { return null; }

        ///<summary>
        ///      A method for simulating hovering (moving the mouse on, and off,
        ///      an object). This is a custom method which provides an 'in' to a
        ///      frequent task.
        ///
        ///      Whenever the mouse cursor is moved over a matched
        ///      element, the first specified function is fired. Whenever the mouse
        ///      moves off of the element, the second specified function fires.
        ///      Additionally, checks are in place to see if the mouse is still within
        ///      the specified element itself (for example, an image inside of a div),
        ///      and if it is, it will continue to 'hover', and not move out
        ///      (a common error in using a mouseout event handler).
        ///    </summary>
        [PreserveCase]
        public jQuery hover(JQueryEventHandlerDelegate over, JQueryEventHandlerDelegate Out) { return null; }

        ///<summary>
        ///      Bind a function to be executed whenever the DOM is ready to be
        ///      traversed and manipulated. This is probably the most important
        ///      function included in the event module, as it can greatly improve
        ///      the response times of your web applications.
        ///
        ///      In a nutshell, this is a solid replacement for using window.onload,
        ///      and attaching a function to that. By using this method, your bound function
        ///      will be called the instant the DOM is ready to be read and manipulated,
        ///      which is when what 99.99% of all JavaScript code needs to run.
        ///
        ///      There is one argument passed to the ready event handler: A reference to
        ///      the jQuery function. You can name that argument whatever you like, and
        ///      can therefore stick with the $ alias without risk of naming collisions.
        ///
        ///      Please ensure you have no code in your &lt;body&gt; onload event handler,
        ///      otherwise $(document).ready() may not fire.
        ///
        ///      You can have as many $(document).ready events on your page as you like.
        ///      The functions are then executed in the order they were added.
        ///    </summary>
        [PreserveCase]
        public jQuery ready(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the scroll event of each matched element.</summary>
        [PreserveCase]
        public jQuery scroll(JQueryEventHandlerDelegate fn) { return null; }
        [PreserveCase]
        public jQuery scroll() { return null; }

        ///<summary>Bind a function to the submit event of each matched element.</summary>
        [PreserveCase]
        public jQuery submit(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>
        ///      Trigger the submit event of each matched element. This causes all of the functions
        ///      that have been bound to that submit event to be executed, and calls the browser's
        ///      default submit action on the matching element(s). This default action can be prevented
        ///      by returning false from one of the functions bound to the submit event.
        ///
        ///      Note: This does not execute the submit method of the form element! If you need to
        ///      submit the form via code, you have to use the DOM method, eg. $("form")[0].submit();
        ///    </summary>
        [PreserveCase]
        public jQuery submit() { return null; }

        ///<summary>Bind a function to the focus event of each matched element.</summary>
        [PreserveCase]
        public jQuery focus(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>
        ///      Trigger the focus event of each matched element. This causes all of the functions
        ///      that have been bound to thet focus event to be executed.
        ///
        ///      Note: This does not execute the focus method of the underlying elements! If you need to
        ///      focus an element via code, you have to use the DOM method, eg. $("#myinput")[0].focus();
        ///    </summary>
        [PreserveCase]
        public jQuery focus() { return null; }

        ///<summary>Bind a function to the keydown event of each matched element.</summary>
        [PreserveCase]
        public jQuery keydown(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the dblclick event of each matched element.</summary>
        [PreserveCase]
        public jQuery dblclick(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the keypress event of each matched element.</summary>
        [PreserveCase]
        public jQuery keypress(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the error event of each matched element.</summary>
        [PreserveCase]
        public jQuery error(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the blur event of each matched element.</summary>
        [PreserveCase]
        public jQuery blur(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>
        ///      Trigger the blur event of each matched element. This causes all of the functions
        ///      that have been bound to that blur event to be executed, and calls the browser's
        ///      default blur action on the matching element(s). This default action can be prevented
        ///      by returning false from one of the functions bound to the blur event.
        ///
        ///      Note: This does not execute the blur method of the underlying elements! If you need to
        ///      blur an element via code, you have to use the DOM method, eg. $("#myinput")[0].blur();
        ///    </summary>
        [PreserveCase]
        public jQuery blur() { return null; }

        ///<summary>Bind a function to the load event of each matched element.</summary>
        [PreserveCase]
        public jQuery load(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the select event of each matched element.</summary>
        [PreserveCase]
        public jQuery select(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>
        ///      Trigger the select event of each matched element. This causes all of the functions
        ///      that have been bound to that select event to be executed, and calls the browser's
        ///      default select action on the matching element(s). This default action can be prevented
        ///      by returning false from one of the functions bound to the select event.
        ///    </summary>
        [PreserveCase]
        public jQuery select() { return null; }

        ///<summary>Bind a function to the mouseup event of each matched element.</summary>
        [PreserveCase]
        public jQuery mouseup(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the unload event of each matched element.</summary>
        [PreserveCase]
        public jQuery unload(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the change event of each matched element.</summary>
        [PreserveCase]
        public jQuery change(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the mouseout event of each matched element.</summary>
        [PreserveCase]
        public jQuery mouseout(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the keyup event of each matched element.</summary>
        [PreserveCase]
        public jQuery keyup(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the click event of each matched element.</summary>
        [PreserveCase]
        public jQuery click(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>
        ///      Trigger the click event of each matched element. This causes all of the functions
        ///      that have been bound to thet click event to be executed.
        ///    </summary>
        [PreserveCase]
        public jQuery click() { return null; }
        
        [PreserveCase]
        public jQuery change() { return null; }

        ///<summary>Bind a function to the resize event of each matched element.</summary>
        [PreserveCase]
        public jQuery resize(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the mousemove event of each matched element.</summary>
        [PreserveCase]
        public jQuery mousemove(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the mousedown event of each matched element.</summary>
        [PreserveCase]
        public jQuery mousedown(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>Bind a function to the mouseover event of each matched element.</summary>
        [PreserveCase]
        public jQuery mouseover(JQueryEventHandlerDelegate fn) { return null; }

        ///<summary>
        ///      Load HTML from a remote file and inject it into the DOM, only if it's
        ///      been modified by the server.
        ///    </summary>
        [PreserveCase]
        public jQuery loadIfModified(String url, Object parameters, BasicCallback callback) { return null; }

        ///<summary>
        ///      Load HTML from a remote file and inject it into the DOM.
        ///
        ///      Note: Avoid to use this to load scripts, instead use $.getScript.
        ///      IE strips script tags when there aren't any other characters in front of it.
        ///    </summary>
        [PreserveCase]
        public jQuery load(String url, Object parameters, BasicCallback callback) { return null; }

        ///<summary>
        ///      Serializes a set of input elements into a string of data.
        ///      This will serialize all given elements.
        ///
        ///      A serialization similar to the form submit of a browser is
        ///      provided by the [http://www.malsup.com/jquery/form/ Form Plugin].
        ///      It also takes multiple-selects
        ///      into account, while this method recognizes only a single option.
        ///    </summary>
        [PreserveCase]
        public String serialize() { return null; }

        ///<summary>
        ///      Evaluate all script tags inside this jQuery. If they have a src attribute,
        ///      the script is loaded, otherwise it's content is evaluated.
        ///    </summary>
        [PreserveCase]
        public jQuery evalScripts() { return null; }

        ///<summary>
        ///      Attach a function to be executed whenever an AJAX request begins
        ///      and there is none already active.
        ///    </summary>
        [PreserveCase]
        public jQuery ajaxStart(BasicCallback callback) { return null; }

        ///<summary>Attach a function to be executed whenever all AJAX requests have ended.</summary>
        [PreserveCase]
        public jQuery ajaxStop(BasicCallback callback) { return null; }

        ///<summary>
        ///      Attach a function to be executed whenever an AJAX request completes.
        ///
        ///      The XMLHttpRequest and settings used for that request are passed
        ///      as arguments to the callback.
        ///    </summary>
        [PreserveCase]
        public jQuery ajaxComplete(BasicCallback callback) { return null; }

        ///<summary>
        ///      Attach a function to be executed whenever an AJAX request completes
        ///      successfully.
        ///
        ///      The XMLHttpRequest and settings used for that request are passed
        ///      as arguments to the callback.
        ///    </summary>
        [PreserveCase]
        public jQuery ajaxSuccess(BasicCallback callback) { return null; }

        ///<summary>
        ///      Attach a function to be executed whenever an AJAX request fails.
        ///
        ///      The XMLHttpRequest and settings used for that request are passed
        ///      as arguments to the callback. A third argument, an exception object,
        ///      is passed if an exception occured while processing the request.
        ///    </summary>
        [PreserveCase]
        public jQuery ajaxError(BasicCallback callback) { return null; }

        ///<summary>
        ///      Attach a function to be executed before an AJAX request is sent.
        ///
        ///      The XMLHttpRequest and settings used for that request are passed
        ///      as arguments to the callback.
        ///    </summary>
        [PreserveCase]
        public jQuery ajaxSend(BasicCallback callback) { return null; }

        ///<summary>Displays each of the set of matched elements if they are hidden.</summary>
        [PreserveCase]
        public jQuery show() { return null; }

        ///<summary>
        ///      Show all matched elements using a graceful animation and firing an
        ///      optional callback after completion.
        ///
        ///      The height, width, and opacity of each of the matched elements
        ///      are changed dynamically according to the specified speed.
        ///    </summary>
        [PreserveCase]
        public jQuery show(String speed, BasicCallback callback) { return null; }

        ///<summary>
        ///      Show all matched elements using a graceful animation and firing an
        ///      optional callback after completion.
        ///
        ///      The height, width, and opacity of each of the matched elements
        ///      are changed dynamically according to the specified speed.
        ///    </summary>
        [PreserveCase]
        public jQuery show(int speed, BasicCallback callback) { return null; }

        ///<summary>Hides each of the set of matched elements if they are shown.</summary>
        [PreserveCase]
        public jQuery hide() { return null; }

        ///<summary>
        ///      Hide all matched elements using a graceful animation and firing an
        ///      optional callback after completion.
        ///
        ///      The height, width, and opacity of each of the matched elements
        ///      are changed dynamically according to the specified speed.
        ///    </summary>
        [PreserveCase]
        public jQuery hide(String speed, BasicCallback callback) { return null; }

        ///<summary>
        ///      Hide all matched elements using a graceful animation and firing an
        ///      optional callback after completion.
        ///
        ///      The height, width, and opacity of each of the matched elements
        ///      are changed dynamically according to the specified speed.
        ///    </summary>
        [PreserveCase]
        public jQuery hide(int speed, BasicCallback callback) { return null; }

        ///<summary>
        ///      Toggles each of the set of matched elements. If they are shown,
        ///      toggle makes them hidden. If they are hidden, toggle
        ///      makes them shown.
        ///    </summary>
        [PreserveCase]
        public jQuery toggle() { return null; }

        ///<summary>
        ///      Reveal all matched elements by adjusting their height and firing an
        ///      optional callback after completion.
        ///
        ///      Only the height is adjusted for this animation, causing all matched
        ///      elements to be revealed in a "sliding" manner.
        ///    </summary>
        [PreserveCase]
        public jQuery slideDown(String speed, BasicCallback callback) { return null; }

        ///<summary>
        ///      Reveal all matched elements by adjusting their height and firing an
        ///      optional callback after completion.
        ///
        ///      Only the height is adjusted for this animation, causing all matched
        ///      elements to be revealed in a "sliding" manner.
        ///    </summary>
        [PreserveCase]
        public jQuery slideDown(int speed, BasicCallback callback) { return null; }

        ///<summary>
        ///      Hide all matched elements by adjusting their height and firing an
        ///      optional callback after completion.
        ///
        ///      Only the height is adjusted for this animation, causing all matched
        ///      elements to be hidden in a "sliding" manner.
        ///    </summary>
        [PreserveCase]
        public jQuery slideUp(String speed, BasicCallback callback) { return null; }

        ///<summary>
        ///      Hide all matched elements by adjusting their height and firing an
        ///      optional callback after completion.
        ///
        ///      Only the height is adjusted for this animation, causing all matched
        ///      elements to be hidden in a "sliding" manner.
        ///    </summary>
        [PreserveCase]
        public jQuery slideUp(int speed, BasicCallback callback) { return null; }

        ///<summary>
        ///      Toggle the visibility of all matched elements by adjusting their height and firing an
        ///      optional callback after completion.
        ///
        ///      Only the height is adjusted for this animation, causing all matched
        ///      elements to be hidden in a "sliding" manner.
        ///    </summary>
        [PreserveCase]
        public jQuery slideToggle(String speed, BasicCallback callback) { return null; }

        ///<summary>
        ///      Toggle the visibility of all matched elements by adjusting their height and firing an
        ///      optional callback after completion.
        ///
        ///      Only the height is adjusted for this animation, causing all matched
        ///      elements to be hidden in a "sliding" manner.
        ///    </summary>
        [PreserveCase]
        public jQuery slideToggle(int speed, BasicCallback callback) { return null; }

        ///<summary>
        ///      Fade in all matched elements by adjusting their opacity and firing an
        ///      optional callback after completion.
        ///
        ///      Only the opacity is adjusted for this animation, meaning that
        ///      all of the matched elements should already have some form of height
        ///      and width associated with them.
        ///    </summary>
        [PreserveCase]
        public jQuery fadeIn(String speed, BasicCallback callback) { return null; }

        ///<summary>
        ///      Fade in all matched elements by adjusting their opacity and firing an
        ///      optional callback after completion.
        ///
        ///      Only the opacity is adjusted for this animation, meaning that
        ///      all of the matched elements should already have some form of height
        ///      and width associated with them.
        ///    </summary>
        [PreserveCase]
        public jQuery fadeIn(int speed, BasicCallback callback) { return null; }

        ///<summary>
        ///      Fade out all matched elements by adjusting their opacity and firing an
        ///      optional callback after completion.
        ///
        ///      Only the opacity is adjusted for this animation, meaning that
        ///      all of the matched elements should already have some form of height
        ///      and width associated with them.
        ///    </summary>
        [PreserveCase]
        public jQuery fadeOut(String speed, BasicCallback callback) { return null; }

        ///<summary>
        ///      Fade out all matched elements by adjusting their opacity and firing an
        ///      optional callback after completion.
        ///
        ///      Only the opacity is adjusted for this animation, meaning that
        ///      all of the matched elements should already have some form of height
        ///      and width associated with them.
        ///    </summary>
        [PreserveCase]
        public jQuery fadeOut(int speed, BasicCallback callback) { return null; }

        ///<summary>
        ///      Fade the opacity of all matched elements to a specified opacity and firing an
        ///      optional callback after completion.
        ///
        ///      Only the opacity is adjusted for this animation, meaning that
        ///      all of the matched elements should already have some form of height
        ///      and width associated with them.
        ///    </summary>
        [PreserveCase]
        public jQuery fadeTo(String speed, int opacity, BasicCallback callback) { return null; }

        ///<summary>
        ///      Fade the opacity of all matched elements to a specified opacity and firing an
        ///      optional callback after completion.
        ///
        ///      Only the opacity is adjusted for this animation, meaning that
        ///      all of the matched elements should already have some form of height
        ///      and width associated with them.
        ///    </summary>
        [PreserveCase]
        public jQuery fadeTo(int speed, int opacity, BasicCallback callback) { return null; }

        ///<summary>
        ///      A function for making your own, custom animations. The key aspect of
        ///      this function is the object of style properties that will be animated,
        ///      and to what end. Each key within the object represents a style property
        ///      that will also be animated (for example: "height", "top", or "opacity").
        ///
        ///      Note that properties should be specified using camel case
        ///      eg. marginLeft instead of margin-left.
        ///
        ///      The value associated with the key represents to what end the property
        ///      will be animated. If a number is provided as the value, then the style
        ///      property will be transitioned from its current state to that new number.
        ///      Otherwise if the string "hide", "show", or "toggle" is provided, a default
        ///      animation will be constructed for that property.
        ///    </summary>
        [PreserveCase]
        public jQuery animate(Object parameters, String speed, String easing, BasicCallback callback) { return null; }

        ///<summary>
        ///      A function for making your own, custom animations. The key aspect of
        ///      this function is the object of style properties that will be animated,
        ///      and to what end. Each key within the object represents a style property
        ///      that will also be animated (for example: "height", "top", or "opacity").
        ///
        ///      Note that properties should be specified using camel case
        ///      eg. marginLeft instead of margin-left.
        ///
        ///      The value associated with the key represents to what end the property
        ///      will be animated. If a number is provided as the value, then the style
        ///      property will be transitioned from its current state to that new number.
        ///      Otherwise if the string "hide", "show", or "toggle" is provided, a default
        ///      animation will be constructed for that property.
        ///    </summary>
        [PreserveCase]
        public jQuery animate(Object parameters, int speed, String easing, BasicCallback callback) { return null; }
        
        [PreserveCase]
        public double scrollTop() { return 0; }
        [PreserveCase]
        public jQuery scrollTop(int val) { return null; }
        
        [PreserveCase]
        public double scrollLeft() { return 0; }
        [PreserveCase]
        public jQuery scrollLeft(int val) { return null; }

        [PreserveCase]
        public double innerWidth() { return 0; }
        [PreserveCase]
        public double innerHeight() { return 0; }

        [PreserveCase]
        public double outerWidth() { return 0; }
        [PreserveCase]
        public double outerHeight() { return 0; }
        
        [PreserveCase]
        public object data(string name) { return null; }
        [PreserveCase]
        public jQuery data(string name, object d) { return null; }
        [PreserveCase]
        public jQuery removeData(string name) { return null; }
        
        [PreserveCase]
        public jQuery contents() { return null; }

        [PreserveCase]
        public static JQueryBrowser browser;
        
        [PreserveCase]
        public static XMLHttpRequest ajax(Dictionary options) { return null; }

		[PreserveCase]
		public LeftTop position() { return null; }
		
		[PreserveCase]
		public jQuery position(LeftTop returnObject) { return null; }

		[PreserveCase]
		public LeftTop offset() { return null; }

		[PreserveCase]
		public jQuery replaceWith(object elem) { return null; }

		// JQuery UI
		[PreserveCase]
		public jQuery draggable() { return null; }

		[PreserveCase]
		public jQuery draggable(Dictionary options) { return null; }

		[PreserveCase]
		public jQuery draggable(string method) { return null; }

		[PreserveCase]
		public jQuery resizable() { return null; }

		[PreserveCase]
		public jQuery resizable(Dictionary options) { return null; }
		
		[PreserveCase]
		public jQuery droppable() { return null; }
		
		[PreserveCase]
		public jQuery droppable(string method) { return null; }

		[PreserveCase]
		public jQuery droppable(Dictionary options) { return null; }
		
		[PreserveCase]
		public jQuery tabs() { return null; }

		[PreserveCase]
		public jQuery tabs(Dictionary options) { return null; }

		[PreserveCase]
		public object tabs(string method, params object[] arg) { return null; }
		
		[PreserveCase]
		public jQuery dialog() { return null; }

		[PreserveCase]
		public jQuery dialog(Dictionary options) { return null; }

		[PreserveCase]
		public object dialog(string method, params object[] arg) { return null; }

		[PreserveCase]
		public jQuery accordion() { return null; }

		[PreserveCase]
		public jQuery accordion(Dictionary options) { return null; }

		[PreserveCase]
		public object accordion(string method, params object[] arg) { return null; }

		// jquery.json
		[PreserveCase]
		public static string quoteString(string s) { return null; }
		[PreserveCase]
		public static string toJSON(object o) { return null; }
		[PreserveCase]
		public static object evalJSON(string s) { return null; }
		[PreserveCase]
		public static object secureEvalJSON(string s) { return null; }
		
		// jquery.focus
        [PreserveCase]
        public jQuery gotfocus(JQueryEventHandlerDelegate fn) { return null; }
        [PreserveCase]
        public jQuery gotfocus() { return null; }

        [PreserveCase]
        public jQuery lostfocus(JQueryEventHandlerDelegate fn) { return null; }
        [PreserveCase]
        public jQuery lostfocus() { return null; }
        
        // bgiframe
        [PreserveCase]
        public jQuery bgiframe() { return null; }
    }
}
