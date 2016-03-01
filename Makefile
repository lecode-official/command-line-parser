
# The path to the ANTLR4 executable
ANTLR4            = java -Xmx500M -cp "/usr/local/lib/antlr-4.5-complete.jar:$CLASSPATH" org.antlr.v4.Tool

# The input and output files
INPUTFILES        = "CommandLine.g4"
JAVAOUTPUTFILES   = CommandLineBaseListener.java CommandLineLexer.java CommandLineListener.java CommandLineParser.java
CSHARPOUTPUTFILES = CommandLineBaseListener.cs CommandLineLexer.cs CommandLineListener.cs CommandLineParser.cs

# The target that generates the Java as well as the C# version of the lexer and parser
All: CommandLineJava CommandLineCSharp

# The target that generates the Java version of the lexer and parser
CommandLineJava: $(JAVAOUTPUTFILES)
$(JAVAOUTPUTFILES):
	$(ANTLR4) $(INPUTFILES)

# The target that generates the C# version of the lexer and parser
CommandLineCSharp: $(CSHARPOUTPUTFILES)
$(CSHARPOUTPUTFILES):
	$(ANTLR4) $(INPUTFILES) -package System.CommandLine.Parser.Antlr -DLanguage=CSharp