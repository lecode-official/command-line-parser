
grammar CommandLine;

commandLine: defaultParameter* parameter*;

defaultParameter: String
    | QuotedString
    ;

parameter: WindowsStyleIdentifier                       # WindowsStyleSwitch
    | WindowsStyleIdentifier AssignmentOperator value   # WindowsStyleParameter
    | UnixStyleIdentifier                               # UnixStyleSwitch
    | UnixStyleIdentifier AssignmentOperator value      # UnixStyleParameter
    | UnixStyleFlaggedIdentifiers                       # UnixStyleFlaggedSwitches
    ;

value: String                                           # String
    | QuotedString                                      # QuotedString
    | Number                                            # Number
    | '[' value (',' value)+ ']'                        # Array
    | True                                              # True
    | False                                             # False
    ;

UnixStyleFlaggedIdentifiers: '-' [a-zA-Z]+;

WindowsStyleIdentifier: '/' [a-zA-Z] ([a-zA-Z] | [0-9] | '-' | '_')+;

UnixStyleIdentifier: '--' [a-zA-Z] ([a-zA-Z] | [0-9] | '-' | '_')+;

AssignmentOperator: '='
    | ':'
    ;

True: 'true';

False: 'false';

Number: Digit+
    | Digit+ '.' Digit*
    | '.' Digit+
    ;

Digit: [0-9];

String: ~[-,:=/"\[\] \r\n]+;

QuotedString: '"' ~[-/\r\n]*? '"';

WhiteSpaces: [ \t\r\n]+ -> skip;