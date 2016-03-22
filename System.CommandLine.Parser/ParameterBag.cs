
#region Using Directives

using System.Collections.Generic;

#endregion

namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents a bag, which is able to contain parsed properties.
    /// </summary>
    public class ParameterBag
    {
        #region Public Properties

        /// <summary>
        /// Gets the original command line parameters from which the properties were parsed.
        /// </summary>
        public string CommandLineParameters { get; internal set; }

        /// <summary>
        /// Gets a collection of the command line parameters that have been parsed.
        /// </summary>
        public IEnumerable<CommandLineParameter> Parameters { get; internal set; } = new List<CommandLineParameter>();

        #endregion
    }
}