
// Represents the grammar for the command line parameters
grammar CommandLine;

// The command line parameters can have default parameters and/or parameters, default parameters are just strings, while parameters are switches
// or key-value-pairs
commandLine: defaultParameter* parameter*;

// Default parameters are strings, strings can either be string that is not quoted (but there are several limitations that apply) or a string in
// quotes, which can pretty much contain any character
defaultParameter: String
    | QuotedString
    ;

// The parameters can be either Windows style (starting with "/") or Unix style (starting with "--"), Unix style flagged switches are also
// supported (flags start with a "-" followed by one or more characters, which each represent a switch), parameters can either be key-value-pairs
// or switches (switches do not require a value, because when they are specified they implicitely have the boolean value true)
parameter: WindowsStyleIdentifier
    | WindowsStyleIdentifier AssignmentOperator value
    | UnixStyleIdentifier
    | UnixStyleIdentifier AssignmentOperator value
    | UnixStyleFlaggedIdentifiers
    ;

// Currently strings, numbers, arrays, and boolean values are supported as values for parameters
value: String
    | QuotedString
    | Number
    | '[' (value (',' value)*)? ']'
    | True
    | False
    ;

// The Unix style flagged identifier starts with a "-" followed by one or more switches, each represented by a single character
UnixStyleFlaggedIdentifiers: '-' [a-zA-Z]+;

// Windows style identifiers starts with a "/" followed by one or more valid identifier characters
WindowsStyleIdentifier: '/' [a-zA-Z] ([a-zA-Z] | [0-9] | '-' | '_')+;

// Unix style identifiers starts with "--" followed by one or more valid identifier characters
UnixStyleIdentifier: '--' [a-zA-Z] ([a-zA-Z] | [0-9] | '-' | '_')+;

// "=" and ":" are valid assignment operators (in the Windows world "=" is preferred, while in the Unix world ":" is preferred)
AssignmentOperator: '='
    | ':'
    ;

// The two boolean values true and false
True: 'true';
False: 'false';

// Numbers can come in three different styles: 123, 123.456, .456, and 123.
Number: '-'? Digit+
    | '-'? Digit+ '.' Digit*
    | '-'? '.' Digit+
    ;
Digit: [0-9];

// Unquoted strings can consist of any character except "-", ",", ":", "=", "/", '"', "[", "]", spaces, and line-breaks, because otherwise the
// grammar would be ambigous
String: ~[-,:=/"\[\] \r\n]+;

// Quoted strings can consist of any character except for line-breaks
QuotedString: '"' ~[\r\n]*? '"';

// All kinds of white spaces as well as line-breaks are thrown out and ignored
WhiteSpaces: [ \t\r\n]+ -> skip;