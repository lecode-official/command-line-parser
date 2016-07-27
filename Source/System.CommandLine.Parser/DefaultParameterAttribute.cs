
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents an attribute, which can be used to specify that the property or constructor parameter is to be matched with one or multiple default command line parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DefaultParameterAttribute : Attribute { }
}