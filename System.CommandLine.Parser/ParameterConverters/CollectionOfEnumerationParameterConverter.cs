
#region Using Directives

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.CommandLine.Parser.Parameters;
using System.Linq;
using System.Reflection;

#endregion

namespace System.CommandLine.Parser.ParameterConverters
{
    /// <summary>
    /// Represents a parameter converter, which is able to convert array parameters into any type of enumeration collection.
    /// </summary>
    public class CollectionOfEnumerationParameterConverter : IParameterConverter
    {
        #region Private Static Fields

        /// <summary>
        /// Contains a list of all the collection types that are supported by the <see cref="CollectionOfEnumerationParameterConverter"/> parameter converter.
        /// </summary>
        private static IEnumerable<Type> supportedCollectionTypes = new List<Type>
        {
            typeof(IList<>),
            typeof(IEnumerable<>),
            typeof(ICollection<>),
            typeof(ISet<>),
            typeof(IReadOnlyCollection<>),
            typeof(IReadOnlyList<>),
            typeof(List<>),
            typeof(Collection<>),
            typeof(LinkedList<>),
            typeof(Queue<>),
            typeof(SortedSet<>),
            typeof(HashSet<>),
            typeof(Stack<>)
        };
        
        /// <summary>
        /// Contains a conversion map, which maps the supported types to the concrete type that are instantiated as a result.
        /// </summary>
        private static IDictionary<Type, Type> collectionTypeConversionMap = new Dictionary<Type, Type>
        {
            [typeof(IList<>)] = typeof(List<>),
            [typeof(IEnumerable<>)] = typeof(List<>),
            [typeof(ICollection<>)] = typeof(Collection<>),
            [typeof(ISet<>)] = typeof(HashSet<>),
            [typeof(IReadOnlyCollection<>)] = typeof(ReadOnlyCollection<>),
            [typeof(IReadOnlyList<>)] = typeof(ReadOnlyCollection<>),
            [typeof(List<>)] = typeof(List<>),
            [typeof(Collection<>)] = typeof(Collection<>),
            [typeof(LinkedList<>)] = typeof(LinkedList<>),
            [typeof(Queue<>)] = typeof(Queue<>),
            [typeof(SortedSet<>)] = typeof(SortedSet<>),
            [typeof(HashSet<>)] = typeof(HashSet<>),
            [typeof(Stack<>)] = typeof(Stack<>)
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
            // Checks if the property type is of one of the supported collection types, if not, then false is returned
            if (!propertyType.IsArray && !CollectionOfEnumerationParameterConverter.supportedCollectionTypes.Any(supportedCollectionType => supportedCollectionType == propertyType.GetGenericTypeDefinition()))
                return false;

            // Checks if the collection or array type is of an enumeration type, if not, then false is returned
            if (!propertyType.IsArray && !propertyType.GetGenericArguments().First().IsEnum)
                return false;
            if (propertyType.IsArray && !propertyType.GetElementType().IsEnum)
                return false;

            // Checks if the parameter is of type array, if not then the single item must be of type string and the value must be a defined enumeration value
            if (parameter.Kind == ParameterKind.Array)
            {
                foreach (Parameter item in (parameter as ArrayParameter).Value)
                {
                    if (item.Kind != ParameterKind.String && !propertyType.IsEnumDefined((item as StringParameter).Value))
                        return false;
                }
            }
            else
            {
                if (parameter.Kind != ParameterKind.String && !propertyType.IsEnumDefined((parameter as StringParameter).Value))
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
            // Gets the value of the parameter
            List<string> parameterValue = new List<string>();
            if (parameter.Kind == ParameterKind.String)
            {
                parameterValue.Add((parameter as StringParameter).Value);
            }
            else if (parameter.Kind == ParameterKind.Array)
            {
                foreach (Parameter item in (parameter as ArrayParameter).Value)
                {
                    if (item.Kind == ParameterKind.String)
                        parameterValue.Add((item as StringParameter).Value);
                    else
                        throw new InvalidOperationException("The parameter could not be converted, because the command line parameter is an array that contains values that can not be converted into a number.");
                }
            }
            else
            {
                throw new InvalidOperationException("The parameter could not be converted, because the command line parameter is neither an array of values that can be converted into numbers nor a value that can be converted into a number.");
            }

            // Determines the element type of the result collection
            if (!propertyType.IsArray && !propertyType.GetGenericArguments().First().IsEnum)
                throw new InvalidOperationException("The parameter could not be converted, because the type of array or list is not supported.");
            if (propertyType.IsArray && !propertyType.GetElementType().IsEnum)
                throw new InvalidOperationException("The parameter could not be converted, because the type of array or list is not supported.");
            Type propertyContentType = propertyType.IsArray ? propertyType.GetElementType() : propertyType.GetGenericArguments().First();

            // Creates the internal array for the specified type
            Type arrayType = propertyContentType.MakeArrayType();
            IList array = arrayType.GetConstructors().First().Invoke(new object[] { parameterValue.Count }) as IList;

            // Fills in the values into the internal array
            for (int i = 0; i < parameterValue.Count; i++)
                array[i] = Enum.Parse(propertyContentType, parameterValue[i]);

            // Checks if the property is also an array, in that case the internal array can already be returned
            if (propertyType.IsArray)
                return array;

            // Since the property type is not an array, its collection type is determined
            if (!CollectionOfEnumerationParameterConverter.collectionTypeConversionMap.ContainsKey(propertyType.GetGenericTypeDefinition()))
                throw new InvalidOperationException("The parameter could not be converted, because the type of array or list is not supported.");
            Type propertyResultType = CollectionOfEnumerationParameterConverter.collectionTypeConversionMap[propertyType.GetGenericTypeDefinition()].MakeGenericType(propertyContentType);

            // Instantiates a new result collection from the result type (all the collection types that are supported have a constructor that takes either an IList<> or an IEnumerable<> as a parameter)
            ConstructorInfo propertyTypeConstructorInfo = propertyResultType.GetConstructors().First(constructorInfo => constructorInfo.GetParameters().Count() == 1 && constructorInfo.GetParameters().First().ParameterType.IsAssignableFrom(arrayType));
            if (propertyTypeConstructorInfo == null)
                throw new InvalidOperationException("The parameter could not be converted, because the result collection type could not be instantiated.");
            object resultCollection = propertyTypeConstructorInfo.Invoke(new object[] { array });

            // Returns the instantiated collection
            return resultCollection;
        }

        #endregion
    }
}