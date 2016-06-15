
#region Using Directives

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.CodeDom.Compiler;

#endregion

namespace System.CommandLine.Parser.Antlr
{
    /// <summary>
    /// This class provides an empty implementation of <see cref="ICommandLineVisitor{Result}"/>, which can be extended to create a visitor which only needs to handle a subset of the available methods.
    /// </summary>
    /// <typeparam name="TResult">The return type of the visit operation.</typeparam>
    [GeneratedCode("ANTLR", "4.5")]
    [CLSCompliant(false)]
    internal partial class CommandLineBaseVisitor<TResult> : AbstractParseTreeVisitor<TResult>, ICommandLineVisitor<TResult>
    {
        #region Public Virtual Methods

        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandLineParser.commandLine"/>. The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/> on <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public virtual TResult VisitCommandLine([NotNull] CommandLineParser.CommandLineContext context) => this.VisitChildren(context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandLineParser.DefaultParameterString"/>. The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/> on <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public virtual TResult VisitDefaultParameterString([NotNull] CommandLineParser.DefaultParameterStringContext context) => this.VisitChildren(context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandLineParser.WindowsStyleSwitch"/>. The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>on <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public virtual TResult VisitWindowsStyleSwitch([NotNull] CommandLineParser.WindowsStyleSwitchContext context) => this.VisitChildren(context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandLineParser.WindowsStyleParameter"/>. The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/> on <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public virtual TResult VisitWindowsStyleParameter([NotNull] CommandLineParser.WindowsStyleParameterContext context) => this.VisitChildren(context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandLineParser.UnixStyleParameter"/>. The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/> on <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public virtual TResult VisitUnixStyleParameter([NotNull] CommandLineParser.UnixStyleParameterContext context) => this.VisitChildren(context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandLineParser.UnixStyleSwitch"/>. The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/> on <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public virtual TResult VisitUnixStyleSwitch([NotNull] CommandLineParser.UnixStyleSwitchContext context) => this.VisitChildren(context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandLineParser.UnixStyleAliasParameter"/>. The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>on <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public virtual TResult VisitUnixStyleAliasParameter([NotNull] CommandLineParser.UnixStyleAliasParameterContext context) => this.VisitChildren(context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandLineParser.UnixStyleFlaggedSwitch"/>. The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/> on <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public virtual TResult VisitUnixStyleFlaggedSwitch([NotNull] CommandLineParser.UnixStyleFlaggedSwitchContext context) => this.VisitChildren(context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandLineParser.String"/>. The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/> on <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public virtual TResult VisitString([NotNull] CommandLineParser.StringContext context) => this.VisitChildren(context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandLineParser.Number"/>. The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/> on <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public virtual TResult VisitNumber([NotNull] CommandLineParser.NumberContext context) => this.VisitChildren(context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandLineParser.Array"/>. The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/> on <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public virtual TResult VisitArray([NotNull] CommandLineParser.ArrayContext context) => this.VisitChildren(context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandLineParser.Boolean"/>. The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/> on <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public virtual TResult VisitBoolean([NotNull] CommandLineParser.BooleanContext context) => this.VisitChildren(context);

        #endregion
    }
}