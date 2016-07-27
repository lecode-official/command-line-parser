
#region Using Directives

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.CommandLine.Parser.Parameters;
using System.Globalization;
using System.Linq;
using System.Text;

#endregion

namespace System.CommandLine.Parser.ParameterConverters
{
    /// <summary>
    /// Represents a parameter converter, which is able to convert array parameters into any type of string collection.
    /// </summary>
    public class CollectionOfStringParameterConverter : IParameterConverter
    {
        #region Private Static Fields

        /// <summary>
        /// Contains a map of all supported collection types, that maps the type to a conversion method, which converts the original values into the specified collection type.
        /// </summary>
        private static IDictionary<Type, Func<IEnumerable<string>, object>> conversionMap = new Dictionary<Type, Func<IEnumerable<string>, object>>
        {
            [typeof(string[])] = array => array.ToArray(),
            [typeof(IList<string>)] = array => array.ToList(),
            [typeof(IEnumerable<string>)] = array => array.ToList(),
            [typeof(ICollection<string>)] = array => new Collection<string>(array.ToList()),
            [typeof(ISet<string>)] = array => new HashSet<string>(array),
            [typeof(IReadOnlyCollection<string>)] = array => new ReadOnlyCollection<string>(array.ToList()),
            [typeof(IReadOnlyList<string>)] = array => new ReadOnlyCollection<string>(array.ToList()),
            [typeof(List<string>)] = array => array.ToList(),
            [typeof(Collection<string>)] = array => new Collection<string>(array.ToList()),
            [typeof(LinkedList<string>)] = array => new LinkedList<string>(array),
            [typeof(Queue<string>)] = array => new Queue<string>(array),
            [typeof(SortedSet<string>)] = array => new SortedSet<string>(array),
            [typeof(HashSet<string>)] = array => new HashSet<string>(array),
            [typeof(Stack<string>)] = array => new Stack<string>(array),

            [typeof(StringBuilder[])] = array => array.Select(item => new StringBuilder(item)).ToArray(),
            [typeof(IList<StringBuilder>)] = array => array.Select(item => new StringBuilder(item)).ToList(),
            [typeof(IEnumerable<StringBuilder>)] = array => array.Select(item => new StringBuilder(item)).ToList(),
            [typeof(ICollection<StringBuilder>)] = array => new Collection<StringBuilder>(array.Select(item => new StringBuilder(item)).ToList()),
            [typeof(ISet<StringBuilder>)] = array => new HashSet<StringBuilder>(array.Select(item => new StringBuilder(item))),
            [typeof(IReadOnlyCollection<StringBuilder>)] = array => new ReadOnlyCollection<StringBuilder>(array.Select(item => new StringBuilder(item)).ToList()),
            [typeof(IReadOnlyList<StringBuilder>)] = array => new ReadOnlyCollection<StringBuilder>(array.Select(item => new StringBuilder(item)).ToList()),
            [typeof(List<StringBuilder>)] = array => array.Select(item => new StringBuilder(item)).ToList(),
            [typeof(Collection<StringBuilder>)] = array => new Collection<StringBuilder>(array.Select(item => new StringBuilder(item)).ToList()),
            [typeof(LinkedList<StringBuilder>)] = array => new LinkedList<StringBuilder>(array.Select(item => new StringBuilder(item))),
            [typeof(Queue<StringBuilder>)] = array => new Queue<StringBuilder>(array.Select(item => new StringBuilder(item))),
            [typeof(SortedSet<StringBuilder>)] = array => new SortedSet<StringBuilder>(array.Select(item => new StringBuilder(item))),
            [typeof(HashSet<StringBuilder>)] = array => new HashSet<StringBuilder>(array.Select(item => new StringBuilder(item))),
            [typeof(Stack<StringBuilder>)] = array => new Stack<StringBuilder>(array.Select(item => new StringBuilder(item)))
        };

        #endregion

        #region IParameterConverter Implementation

        /// <summary>
        /// Dertermines whether the specified parameter can be converted into the specified type.
        /// </summary>
        /// <param name="propertyType">The type of the property into which the parameter is to be converted.</param>
        /// <param name="parameter">The parameters that is to be converted into the specified type.</param>
        /// <returns>Returns <c>true</c> if the specified parameter can be converted into the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvert(Type propertyType, Parameter parameter)
        {
            // Checks if the property is of one of the supported types
            if (!CollectionOfStringParameterConverter.conversionMap.ContainsKey(propertyType))
                return false;

            // Checks if the parameter is of type array, if not then the single item must be of type string, boolean, or number, otherwise all items must be of type string, boolean, or number
            if (parameter.Kind == ParameterKind.Array)
            {
                foreach (Parameter item in (parameter as ArrayParameter).Value)
                {
                    if (item.Kind != ParameterKind.String && item.Kind != ParameterKind.Boolean && item.Kind != ParameterKind.Number)
                        return false;
                }
            }
            else
            {
                if (parameter.Kind != ParameterKind.String && parameter.Kind != ParameterKind.Boolean && parameter.Kind != ParameterKind.Number)
                    return false;
            }

            // Since all requirements for the property type and the parameter were met, the parameter can be converted
            return true;
        }

        /// <summary>
        /// Converts the specified parameter into the specified type.
        /// </summary>
        /// <param name="propertyType">The type of the property into which the parameter is to be converted.</param>
        /// <param name="parameter">The parameter that is to be converted into the specified type.</param>
        /// <exception cref="InvalidOperationException">If the parameter could not be converted, an <see cref="InvalidOperationException"/> exception is thrown.</exception>
        /// <returns>Returns the converted parameter value.</returns>
        public object Convert(Type propertyType, Parameter parameter)
        {
            // Gets the converter for the specified property type, if it does not exist, then an exception is thrown
            if (!CollectionOfStringParameterConverter.conversionMap.ContainsKey(propertyType))
                throw new InvalidOperationException("The parameter could not be converted, because the type of array or list is not supported.");
            Func<IEnumerable<string>, object> converter = CollectionOfStringParameterConverter.conversionMap[propertyType];

            // Gets the value of the parameter
            List<string> parameterValue = new List<string>();
            if (parameter.Kind == ParameterKind.String)
            {
                parameterValue.Add((parameter as StringParameter).Value);
            }
            else if (parameter.Kind == ParameterKind.Boolean)
            {
                parameterValue.Add((parameter as BooleanParameter).Value.ToString());
            }
            else if (parameter.Kind == ParameterKind.Number)
            {
                parameterValue.Add((parameter as NumberParameter).Value.ToString(CultureInfo.InvariantCulture));
            }
            else if (parameter.Kind == ParameterKind.Array)
            {
                foreach (Parameter item in (parameter as ArrayParameter).Value)
                {
                    if (item.Kind == ParameterKind.String)
                        parameterValue.Add((item as StringParameter).Value);
                    else if (item.Kind == ParameterKind.Boolean)
                        parameterValue.Add((item as BooleanParameter).Value.ToString());
                    else if (item.Kind == ParameterKind.Number)
                        parameterValue.Add((item as NumberParameter).Value.ToString(CultureInfo.InvariantCulture));
                    else
                        throw new InvalidOperationException("The parameter could not be converted, because the command line parameter is an array that contains values that can not be converted into a string.");
                }
            }
            else
            {
                throw new InvalidOperationException("The parameter could not be converted, because the command line parameter is neither an array of values that can be converted into string nor a value that can be converted into a string.");
            }

            // Converts the parameter value into its destination value and returns it
            return converter(parameterValue);
        }

        #endregion
    }
}