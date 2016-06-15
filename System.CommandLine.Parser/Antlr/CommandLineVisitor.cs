
#region Using Directives

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.CodeDom.Compiler;

#endregion

namespace System.CommandLine.Parser.Antlr
{
    /// <summary>
    /// This interface defines a complete generic visitor for a parse tree produced by <see cref="CommandLineParser"/>.
    /// </summary>
    /// <typeparam name="TResult">The return type of the visit operation.</typeparam>
    [GeneratedCode("ANTLR", "4.5")]
    [CLSCompliant(false)]
    internal interface ICommandLineVisitor<TResult> : IParseTreeVisitor<TResult>
    {
        #region Methods

        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandLineParser.commandLine"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        TResult VisitCommandLine([NotNull] CommandLineParser.CommandLineContext context);

	    /// <summary>
	    /// Visit a parse tree produced by the <c>DefaultParameterString</c> labeled alternative in <see cref="CommandLineParser.defaultParameter"/>.
	    /// </summary>
	    /// <param name="context">The parse tree.</param>
	    /// <return>The visitor result.</return>
	    TResult VisitDefaultParameterString([NotNull] CommandLineParser.DefaultParameterStringContext context);

	    /// <summary>
	    /// Visit a parse tree produced by the <c>WindowsStyleSwitch</c> labeled alternative in <see cref="CommandLineParser.parameter"/>.
	    /// </summary>
	    /// <param name="context">The parse tree.</param>
	    /// <return>The visitor result.</return>
	    TResult VisitWindowsStyleSwitch([NotNull] CommandLineParser.WindowsStyleSwitchContext context);

	    /// <summary>
	    /// Visit a parse tree produced by the <c>WindowsStyleParameter</c> labeled alternative in <see cref="CommandLineParser.parameter"/>.
	    /// </summary>
	    /// <param name="context">The parse tree.</param>
	    /// <return>The visitor result.</return>
	    TResult VisitWindowsStyleParameter([NotNull] CommandLineParser.WindowsStyleParameterContext context);

	    /// <summary>
	    /// Visit a parse tree produced by the <c>UnixStyleParameter</c> labeled alternative in <see cref="CommandLineParser.parameter"/>.
	    /// </summary>
	    /// <param name="context">The parse tree.</param>
	    /// <return>The visitor result.</return>
	    TResult VisitUnixStyleParameter([NotNull] CommandLineParser.UnixStyleParameterContext context);

	    /// <summary>
	    /// Visit a parse tree produced by the <c>UnixStyleSwitch</c> labeled alternative in <see cref="CommandLineParser.parameter"/>.
	    /// </summary>
	    /// <param name="context">The parse tree.</param>
	    /// <return>The visitor result.</return>
	    TResult VisitUnixStyleSwitch([NotNull] CommandLineParser.UnixStyleSwitchContext context);

	    /// <summary>
	    /// Visit a parse tree produced by the <c>UnixStyleAliasParameter</c> labeled alternative in <see cref="CommandLineParser.parameter"/>.
	    /// </summary>
	    /// <param name="context">The parse tree.</param>
	    /// <return>The visitor result.</return>
	    TResult VisitUnixStyleAliasParameter([NotNull] CommandLineParser.UnixStyleAliasParameterContext context);

	    /// <summary>
	    /// Visit a parse tree produced by the <c>UnixStyleFlaggedSwitch</c> labeled alternative in <see cref="CommandLineParser.parameter"/>.
	    /// </summary>
	    /// <param name="context">The parse tree.</param>
	    /// <return>The visitor result.</return>
	    TResult VisitUnixStyleFlaggedSwitch([NotNull] CommandLineParser.UnixStyleFlaggedSwitchContext context);

	    /// <summary>
	    /// Visit a parse tree produced by the <c>String</c> labeled alternative in <see cref="CommandLineParser.value"/>.
	    /// </summary>
	    /// <param name="context">The parse tree.</param>
	    /// <return>The visitor result.</return>
	    TResult VisitString([NotNull] CommandLineParser.StringContext context);

	    /// <summary>
	    /// Visit a parse tree produced by the <c>Number</c> labeled alternative in <see cref="CommandLineParser.value"/>.
	    /// </summary>
	    /// <param name="context">The parse tree.</param>
	    /// <return>The visitor result.</return>
	    TResult VisitNumber([NotNull] CommandLineParser.NumberContext context);

	    /// <summary>
	    /// Visit a parse tree produced by the <c>Array</c> labeled alternative in <see cref="CommandLineParser.value"/>.
	    /// </summary>
	    /// <param name="context">The parse tree.</param>
	    /// <return>The visitor result.</return>
	    TResult VisitArray([NotNull] CommandLineParser.ArrayContext context);

	    /// <summary>
	    /// Visit a parse tree produced by the <c>Boolean</c> labeled alternative in <see cref="CommandLineParser.value"/>.
	    /// </summary>
	    /// <param name="context">The parse tree.</param>
	    /// <return>The visitor result.</return>
	    TResult VisitBoolean([NotNull] CommandLineParser.BooleanContext context);

        #endregion
    }
}