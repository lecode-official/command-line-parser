
#region Using Directives

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.CommandLine.Parser.Parameters;
using System.Linq;

#endregion

namespace System.CommandLine.Parser.ParameterConverters
{
    /// <summary>
    /// Represents a parameter converter, which is able to convert array parameters into any type of boolean collection.
    /// </summary>
    public class ArrayOfBooleanParameterConverter : IParameterConverter
    {
        #region Private Static Fields

        /// <summary>
        /// Contains a map of all supported collection types, that maps the type to a conversion method, which converts the original values into the specified collection type.
        /// </summary>
        private static IDictionary<Type, Func<IEnumerable<bool>, object>> conversionMap = new Dictionary<Type, Func<IEnumerable<bool>, object>>
        {
            [typeof(bool[])] = array => array.ToArray(),
            [typeof(IList<bool>)] = array => array.ToList(),
            [typeof(IEnumerable<bool>)] = array => array.ToList(),
            [typeof(ICollection<bool>)] = array => new Collection<bool>(array.ToList()),
            [typeof(ISet<bool>)] = array => new SortedSet<bool>(array),
            [typeof(IReadOnlyCollection<bool>)] = array => new ReadOnlyCollection<bool>(array.ToList()),
            [typeof(IReadOnlyList<bool>)] = array => new ReadOnlyCollection<bool>(array.ToList()),
            [typeof(List<bool>)] = array => array.ToList(),
            [typeof(Collection<bool>)] = array => new Collection<bool>(array.ToList()),
            [typeof(LinkedList<bool>)] = array => new LinkedList<bool>(array),
            [typeof(Queue<bool>)] = array => new Queue<bool>(array),
            [typeof(SortedSet<bool>)] = array => new SortedSet<bool>(array),
            [typeof(Stack<bool>)] = array => new Stack<bool>(array)
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
            if (!ArrayOfBooleanParameterConverter.conversionMap.ContainsKey(propertyType))
                return false;

            // Checks if the parameter is of type array, if not then the single item must be of type boolean or number, otherwise all items must be of type boolean or number
            if (parameter.Kind == ParameterKind.Array)
            {
                foreach (Parameter item in (parameter as ArrayParameter).Value)
                {
                    if (item.Kind != ParameterKind.Boolean && item.Kind != ParameterKind.Number)
                        return false;
                }
            }
            else
            {
                if (parameter.Kind != ParameterKind.Boolean && parameter.Kind != ParameterKind.Number)
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
            if (!ArrayOfBooleanParameterConverter.conversionMap.ContainsKey(propertyType))
                throw new InvalidOperationException("The parameter could not be converted.");
            Func<IEnumerable<bool>, object> converter = ArrayOfBooleanParameterConverter.conversionMap[propertyType];

            // Gets the value of the parameter
            List<bool> parameterValue = new List<bool>();
            if (parameter.Kind == ParameterKind.Boolean)
            {
                parameterValue.Add((parameter as BooleanParameter).Value);
            }
            else if (parameter.Kind == ParameterKind.Number)
            {
                parameterValue.Add((parameter as NumberParameter).Value != 0);
            }
            else if (parameter.Kind == ParameterKind.Array)
            {
                foreach (Parameter item in (parameter as ArrayParameter).Value)
                {
                    if (item.Kind == ParameterKind.Boolean)
                        parameterValue.Add((item as BooleanParameter).Value);
                    else if (item.Kind == ParameterKind.Number)
                        parameterValue.Add((item as NumberParameter).Value != 0);
                    else
                        throw new InvalidOperationException("The parameter could not be converted.");
                }
            }
            else
            {
                throw new InvalidOperationException("The parameter could not be converted.");
            }

            // Converts the parameter value into its destination value and returns it
            return converter(parameterValue);
        }

        #endregion
    }
}