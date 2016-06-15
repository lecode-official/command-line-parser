
#region Using Directives

using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using System.CodeDom.Compiler;

#endregion

namespace System.CommandLine.Parser.Antlr
{
    /// <summary>
    /// Represents the lexer, which turns the command line parameter input from a character stream to a stream of tokens.
    /// </summary>
    [GeneratedCode("ANTLR", "4.5")]
    [CLSCompliant(false)]
    internal partial class CommandLineLexer : Lexer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="CommandLineLexer"/> instance.
        /// </summary>
        /// <param name="input">The character stream, which is the contains the input to the lexer and is to be tokenized.</param>
        public CommandLineLexer(ICharStream input)
            : base(input)
        {
            this.Interpreter = new LexerATNSimulator(this, CommandLineLexer._ATN);
        }

        #endregion

        #region Private Static Fields

        /// <summary>
        /// Contains the names of the literals that are recognized by the command line lexer.
        /// </summary>
        private static readonly string[] _LiteralNames = { null, "'['", "','", "']'", null, null, null, null, "'true'", "'false'" };

        /// <summary>
        /// Contains the symbolic names of the lexer rules as specified in the original grammar.
        /// </summary>
        private static readonly string[] _SymbolicNames = { null, null, null, null, "UnixStyleFlaggedIdentifiers", "WindowsStyleIdentifier", "UnixStyleIdentifier", "AssignmentOperator", "True", "False", "Number", "Digit", "String", "QuotedString", "WhiteSpaces" };

        #endregion

        #region Public Static Fields

        /// <summary>
        /// Contains the name of all available modes of the lexer.
        /// </summary>
        public static string[] modeNames = { "DEFAULT_MODE" };

        /// <summary>
        /// Contains the names of all the rules of the lexer grammar.
        /// </summary>
        public static readonly string[] ruleNames = { "T__0", "T__1", "T__2", "UnixStyleFlaggedIdentifiers", "WindowsStyleIdentifier", "UnixStyleIdentifier", "AssignmentOperator", "True", "False", "Number", "Digit", "String", "QuotedString", "WhiteSpaces" };

        /// <summary>
        /// Contains the default vocabulary of the lexer.
        /// </summary>
        public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

        /// <summary>
        /// Contains the serialized automaton used for lexing the input. This is derived from the lexer grammar.
        /// </summary>
        public static readonly string _serializedATN =
            "\x3\x430\xD6D1\x8206\xAD2D\x4417\xAEF1\x8D80\xAADD\x2\x10\x83\b\x1\x4" +
            "\x2\t\x2\x4\x3\t\x3\x4\x4\t\x4\x4\x5\t\x5\x4\x6\t\x6\x4\a\t\a\x4\b\t\b" +
            "\x4\t\t\t\x4\n\t\n\x4\v\t\v\x4\f\t\f\x4\r\t\r\x4\xE\t\xE\x4\xF\t\xF\x3" +
            "\x2\x3\x2\x3\x3\x3\x3\x3\x4\x3\x4\x3\x5\x3\x5\x6\x5(\n\x5\r\x5\xE\x5)" +
            "\x3\x6\x3\x6\x3\x6\a\x6/\n\x6\f\x6\xE\x6\x32\v\x6\x3\a\x3\a\x3\a\x3\a" +
            "\x3\a\a\a\x39\n\a\f\a\xE\a<\v\a\x3\b\x3\b\x3\t\x3\t\x3\t\x3\t\x3\t\x3" +
            "\n\x3\n\x3\n\x3\n\x3\n\x3\n\x3\v\x5\vL\n\v\x3\v\x6\vO\n\v\r\v\xE\vP\x3" +
            "\v\x5\vT\n\v\x3\v\x6\vW\n\v\r\v\xE\vX\x3\v\x3\v\a\v]\n\v\f\v\xE\v`\v\v" +
            "\x3\v\x5\v\x63\n\v\x3\v\x3\v\x6\vg\n\v\r\v\xE\vh\x5\vk\n\v\x3\f\x3\f\x3" +
            "\r\x6\rp\n\r\r\r\xE\rq\x3\xE\x3\xE\a\xEv\n\xE\f\xE\xE\xEy\v\xE\x3\xE\x3" +
            "\xE\x3\xF\x6\xF~\n\xF\r\xF\xE\xF\x7F\x3\xF\x3\xF\x3w\x2\x10\x3\x3\x5\x4" +
            "\a\x5\t\x6\v\a\r\b\xF\t\x11\n\x13\v\x15\f\x17\r\x19\xE\x1B\xF\x1D\x10" +
            "\x3\x2\t\x4\x2\x43\\\x63|\a\x2//\x32;\x43\\\x61\x61\x63|\x4\x2<<??\x3" +
            "\x2\x32;\v\x2\f\f\xF\xF\"\"$$./\x31\x31<<??]_\x4\x2\f\f\xF\xF\x5\x2\v" +
            "\f\xF\xF\"\"\x91\x2\x3\x3\x2\x2\x2\x2\x5\x3\x2\x2\x2\x2\a\x3\x2\x2\x2" +
            "\x2\t\x3\x2\x2\x2\x2\v\x3\x2\x2\x2\x2\r\x3\x2\x2\x2\x2\xF\x3\x2\x2\x2" +
            "\x2\x11\x3\x2\x2\x2\x2\x13\x3\x2\x2\x2\x2\x15\x3\x2\x2\x2\x2\x17\x3\x2" +
            "\x2\x2\x2\x19\x3\x2\x2\x2\x2\x1B\x3\x2\x2\x2\x2\x1D\x3\x2\x2\x2\x3\x1F" +
            "\x3\x2\x2\x2\x5!\x3\x2\x2\x2\a#\x3\x2\x2\x2\t%\x3\x2\x2\x2\v+\x3\x2\x2" +
            "\x2\r\x33\x3\x2\x2\x2\xF=\x3\x2\x2\x2\x11?\x3\x2\x2\x2\x13\x44\x3\x2\x2" +
            "\x2\x15j\x3\x2\x2\x2\x17l\x3\x2\x2\x2\x19o\x3\x2\x2\x2\x1Bs\x3\x2\x2\x2" +
            "\x1D}\x3\x2\x2\x2\x1F \a]\x2\x2 \x4\x3\x2\x2\x2!\"\a.\x2\x2\"\x6\x3\x2" +
            "\x2\x2#$\a_\x2\x2$\b\x3\x2\x2\x2%\'\a/\x2\x2&(\t\x2\x2\x2\'&\x3\x2\x2" +
            "\x2()\x3\x2\x2\x2)\'\x3\x2\x2\x2)*\x3\x2\x2\x2*\n\x3\x2\x2\x2+,\a\x31" +
            "\x2\x2,\x30\t\x2\x2\x2-/\t\x3\x2\x2.-\x3\x2\x2\x2/\x32\x3\x2\x2\x2\x30" +
            ".\x3\x2\x2\x2\x30\x31\x3\x2\x2\x2\x31\f\x3\x2\x2\x2\x32\x30\x3\x2\x2\x2" +
            "\x33\x34\a/\x2\x2\x34\x35\a/\x2\x2\x35\x36\x3\x2\x2\x2\x36:\t\x2\x2\x2" +
            "\x37\x39\t\x3\x2\x2\x38\x37\x3\x2\x2\x2\x39<\x3\x2\x2\x2:\x38\x3\x2\x2" +
            "\x2:;\x3\x2\x2\x2;\xE\x3\x2\x2\x2<:\x3\x2\x2\x2=>\t\x4\x2\x2>\x10\x3\x2" +
            "\x2\x2?@\av\x2\x2@\x41\at\x2\x2\x41\x42\aw\x2\x2\x42\x43\ag\x2\x2\x43" +
            "\x12\x3\x2\x2\x2\x44\x45\ah\x2\x2\x45\x46\a\x63\x2\x2\x46G\an\x2\x2GH" +
            "\au\x2\x2HI\ag\x2\x2I\x14\x3\x2\x2\x2JL\a/\x2\x2KJ\x3\x2\x2\x2KL\x3\x2" +
            "\x2\x2LN\x3\x2\x2\x2MO\x5\x17\f\x2NM\x3\x2\x2\x2OP\x3\x2\x2\x2PN\x3\x2" +
            "\x2\x2PQ\x3\x2\x2\x2Qk\x3\x2\x2\x2RT\a/\x2\x2SR\x3\x2\x2\x2ST\x3\x2\x2" +
            "\x2TV\x3\x2\x2\x2UW\x5\x17\f\x2VU\x3\x2\x2\x2WX\x3\x2\x2\x2XV\x3\x2\x2" +
            "\x2XY\x3\x2\x2\x2YZ\x3\x2\x2\x2Z^\a\x30\x2\x2[]\x5\x17\f\x2\\[\x3\x2\x2" +
            "\x2]`\x3\x2\x2\x2^\\\x3\x2\x2\x2^_\x3\x2\x2\x2_k\x3\x2\x2\x2`^\x3\x2\x2" +
            "\x2\x61\x63\a/\x2\x2\x62\x61\x3\x2\x2\x2\x62\x63\x3\x2\x2\x2\x63\x64\x3" +
            "\x2\x2\x2\x64\x66\a\x30\x2\x2\x65g\x5\x17\f\x2\x66\x65\x3\x2\x2\x2gh\x3" +
            "\x2\x2\x2h\x66\x3\x2\x2\x2hi\x3\x2\x2\x2ik\x3\x2\x2\x2jK\x3\x2\x2\x2j" +
            "S\x3\x2\x2\x2j\x62\x3\x2\x2\x2k\x16\x3\x2\x2\x2lm\t\x5\x2\x2m\x18\x3\x2" +
            "\x2\x2np\n\x6\x2\x2on\x3\x2\x2\x2pq\x3\x2\x2\x2qo\x3\x2\x2\x2qr\x3\x2" +
            "\x2\x2r\x1A\x3\x2\x2\x2sw\a$\x2\x2tv\n\a\x2\x2ut\x3\x2\x2\x2vy\x3\x2\x2" +
            "\x2wx\x3\x2\x2\x2wu\x3\x2\x2\x2xz\x3\x2\x2\x2yw\x3\x2\x2\x2z{\a$\x2\x2" +
            "{\x1C\x3\x2\x2\x2|~\t\b\x2\x2}|\x3\x2\x2\x2~\x7F\x3\x2\x2\x2\x7F}\x3\x2" +
            "\x2\x2\x7F\x80\x3\x2\x2\x2\x80\x81\x3\x2\x2\x2\x81\x82\b\xF\x2\x2\x82" +
            "\x1E\x3\x2\x2\x2\x13\x2).\x30\x38:KPSX^\x62hjqw\x7F\x3\b\x2\x2";

        /// <summary>
        /// Contains the automaton that is used for lexing the input.
        /// </summary>
        public static readonly ATN _ATN = new ATNDeserializer().Deserialize(CommandLineLexer._serializedATN.ToCharArray());

        #endregion

        #region Public Constants

        /// <summary>
        /// Contains the ID of the first rule, which is an implicit one.
        /// </summary>
        public const int T__0 = 1;

        /// <summary>
        /// Contains the ID of the second rule, which is an implicit one.
        /// </summary>
        public const int T__1 = 2;

        /// <summary>
        /// Contains the ID of the third rule, which is an implicit one.
        /// </summary>
        public const int T__2 = 3;

        /// <summary>
        /// Contains the ID of the UNIX style flagged identifiers rule.
        /// </summary>
        public const int UnixStyleFlaggedIdentifiers = 4;

        /// <summary>
        /// Contains the ID of the Windows style identifier rule.
        /// </summary>
        public const int WindowsStyleIdentifier = 5;

        /// <summary>
        /// Contains the ID of the UNIX style identifier rule.
        /// </summary>
        public const int UnixStyleIdentifier = 6;

        /// <summary>
        /// Contains the ID of the assignment operator rule.
        /// </summary>
        public const int AssignmentOperator = 7;

        /// <summary>
        /// Contains the ID of the rule for the boolean value true.
        /// </summary>
        public const int True = 8;

        /// <summary>
        /// Contains the ID of the rule for the boolean value false.
        /// </summary>
        public const int False = 9;

        /// <summary>
        /// Contains the ID of the rule for numbers.
        /// </summary>
        public const int Number = 10;

        /// <summary>
        /// Contains the ID of the rule for digits.
        /// </summary>
        public const int Digit = 11;

        /// <summary>
        /// Contains the ID of the rule for strings.
        /// </summary>
        public const int String = 12;

        /// <summary>
        /// Contains the ID of the rule for quoted strings.
        /// </summary>
        public const int QuotedString = 13;

        /// <summary>
        /// Contains the ID of the rule for white spaces.
        /// </summary>
        public const int WhiteSpaces = 14;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the vocabulary of the lexer.
        /// </summary>
        [NotNull]
        public override IVocabulary Vocabulary
        {
            get
            {
                return CommandLineLexer.DefaultVocabulary;
            }
        }

        /// <summary>
        /// Gets the name of the grammar file from which the lexer was generated.
        /// </summary>
        public override string GrammarFileName
        {
            get
            {
                return "CommandLine.g4";
            }
        }

        /// <summary>
        /// Gets the names of the rules of the lexer.
        /// </summary>
        public override string[] RuleNames
        {
            get
            {
                return CommandLineLexer.ruleNames;
            }
        }

        /// <summary>
        /// Gets the names of the modes that the lexer supports.
        /// </summary>
        public override string[] ModeNames
        {
            get
            {
                return CommandLineLexer.modeNames;
            }
        }

        /// <summary>
        /// Gets the serialized automaton, which is used by the lexer.
        /// </summary>
        public override string SerializedAtn
        {
            get
            {
                return CommandLineLexer._serializedATN;
            }
        }

        #endregion
    }
}