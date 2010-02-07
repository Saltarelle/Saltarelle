// ===================================================================
// Author: Matt Kruse <matt@mattkruse.com>
// WWW: http://www.mattkruse.com/
//
// NOTICE: You may use this code for any purpose, commercial or
// private, without any further permission from the author. You may
// remove this notice from your final code if you wish, however it is
// appreciated by the author if at least my web site address is kept.
//
// You may *NOT* re-distribute this code in any way except through its
// use. That means, you can include it in your product, or your web
// site, or any other form where the code is actually being used. You
// may not put the plain javascript up on your site for download or
// include it in your javascript libraries for download. 
// If you wish to share this code with others, please just point them
// to the URL instead.
// Please DO NOT link directly to my .js files from your site. Copy
// the files to your server and use them there. Thank you.
// ===================================================================

// ------------------------------------------------------------------
// These functions use the same 'format' strings as the 
// java.text.SimpleDateFormat class, with minor exceptions.
// The format string consists of the following abbreviations:
// 
// Field        | Full Form          | Short Form
// -------------+--------------------+-----------------------
// Year         | yyyy (4 digits)    | yy (2 digits), y (2 or 4 digits)
// Day of Month | dd (2 digits)      | d (1 or 2 digits)
// Hour (1-12)  | hh (2 digits)      | h (1 or 2 digits)
// Hour (0-23)  | HH (2 digits)      | H (1 or 2 digits)
// Minute       | mm (2 digits)      | m (1 or 2 digits)
// Second       | ss (2 digits)      | s (1 or 2 digits)
// A/P          | t                  |
// AM/PM        | tt                 |

(function() {
	var AM = "AM", PM = "PM";

	_isInteger = function (val) {
		var digits="1234567890";
		for (var i=0; i < val.length; i++) {
			if (digits.indexOf(val.charAt(i))==-1) {
				return false;
			}
		}
		return true;
	};

	_getInt = function(str,i,minlength,maxlength) {
		for (var x=maxlength; x>=minlength; x--) {
			var token=str.substring(i,i+x);
			if (token.length < minlength) {
				return null;
			}
			if (_isInteger(token)) {
				return token;
			}
		}
		return null;
	};
		
	// ------------------------------------------------------------------
	// getDateFromFormat( date_string , format_string )
	//
	// This function takes a date string and a format string. It matches
	// If the date string matches the format string, it returns the 
	// getTime() of the date. If it does not match, it returns 0.
	// ------------------------------------------------------------------
	getDateFromFormat = function(val,format) {
		val = val + "";
		format = format + "";
		var i_val = 0;
		var i_format = 0;
		var c = "";
		var token = "";
		var token2 = "";
		var year = 0, month = 1, date = 1, hh = 0, mm = 0, ss = 0, ampm = "";
		
		while (i_format < format.length) {
			// Get next token from format string
			c = format.charAt(i_format);
			token = "";
			while ((format.charAt(i_format) == c) && (i_format < format.length)) {
				token += format.charAt(i_format++);
			}
			// Extract contents of value based on format token
			if (token=="yyyy" || token=="yy" || token=="y") {
				if (token == "yyyy")
					year = _getInt(val, i_val, 4, 4);
				if (token == "yy")
					year = _getInt(val, i_val, 2, 2);
				if (token == "y")
					year = _getInt(val, i_val, 2, 4);

				if (year == null)
					return null;

				i_val += year.length;
				if (year.length == 2) {
					if (year > 70) {
						year = 1900 + (year-0);
					}
					else {
						year = 2000 + (year-0);
					}
				}
			}
			else if (token == "MM" || token == "M") {
				month = _getInt(val, i_val, token.length, 2);
				if (month == null || (month < 1) || (month > 12))
					return null;
				i_val += month.length;
			}
			else if (token=="dd"||token=="d") {
				date = _getInt(val, i_val, token.length, 2);
				if (date == null || (date < 1) || (date > 31))
					return null;
				i_val += date.length;
			}
			else if (token=="hh"||token=="h") {
				hh = _getInt(val, i_val, token.length, 2);
				if (hh == null || (hh < 1) || (hh > 12))
					return null;
				i_val += hh.length;
			}
			else if (token=="HH"||token=="H") {
				hh = _getInt(val, i_val, token.length, 2);
				if (hh == null || (hh < 0) || (hh > 23))
					return null;
				i_val += hh.length;
			}
			else if (token == "mm" || token == "m") {
				mm = _getInt(val, i_val, token.length, 2);
				if (mm == null || (mm < 0) || (mm > 59))
					return null;
				i_val += mm.length;
			}
			else if (token == "ss" || token == "s") {
				ss = _getInt(val, i_val, token.length, 2);
				if (ss == null || (ss < 0) || (ss > 59))
					return null;
				i_val += ss.length;
			}
			else if (token == "t") {
				if (val.substring(i_val, i_val + 1).toLowerCase() == AM.charAt(0).toLowerCase())
					ampm = AM;
				else if (val.substring(i_val, i_val + 1).toLowerCase() == PM.charAt(0).toLowerCase())
					ampm = PM;
				else
					return null;
				i_val += 1;
			}
			else if (token == "tt") {
				if (val.substring(i_val, i_val + 2).toLowerCase() == AM.toLowerCase())
					ampm = AM;
				else if (val.substring(i_val,i_val+2).toLowerCase() == PM.toLowerCase())
					ampm = PM;
				else
					return null;
				i_val += 2;
			}
			else {
				if (val.substring(i_val, i_val + token.length) != token)
					return null;
				else
					i_val += token.length;
			}
		}
		// If there are any trailing characters left in the value, it doesn't match
		if (i_val != val.length)
			return null;

		// Is date valid for month?
		if (month == 2) {
			// Check for leap year
			if (((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0)) { // leap year
				if (date > 29)
					return null;
			}
			else if (date > 28)
				return null;
		}
		if ((month == 4) || (month == 6) || (month == 9) || (month == 11)) {
			if (date > 30) {
				return null;
			}
		}
		// Correct hours value
		if (hh < 12 && ampm == PM) {
			hh = hh - 0 + 12;
		}
		else if (hh > 11 && ampm == AM) {
			hh -= 12;
		}
		return new Date(Date.UTC(year, month - 1, date, hh, mm, ss));
	}

	Date.parseExact = getDateFromFormat;
})();