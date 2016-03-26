
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
        /// Gets a dictionary of the command line parameters that have been parsed, where the key is the name of the parameter.
        /// </summary>
        public IDictionary<string, Parameter> Parameters { get; internal set; } = new Dictionary<string, Parameter>();

        /// <summary>
        /// Gets a collection of the default parameters.
        /// </summary>
        public IEnumerable<Parameter> DefaultParameters { get; internal set; } = new List<Parameter>();

        #endregion
    }
}