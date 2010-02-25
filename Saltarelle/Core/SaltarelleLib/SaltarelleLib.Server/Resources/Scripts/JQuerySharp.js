//encapsulate jquery in a object to allow script sharp import
JQueryProxy = function() {}

//alias the jquery object
JQueryProxy.prototype.jquery = jQuery;

JQueryProxy.prototype.getItem = function(currentScope){
    //this is just funny
    return currentScope;
}

//alias the get function in the jQuery object itself
jQuery.prototype.isInExpression = jQuery.prototype.is;


FunctionHelper = function() {}
FunctionHelper.stripScope = function(d){return d._targets[1];}
FunctionHelper.getItem = function(currentScope){
    //this is just funny
    return currentScope;
}


