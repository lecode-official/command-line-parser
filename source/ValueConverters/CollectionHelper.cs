
#region Using Directives

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

#endregion

namespace System.CommandLine.ValueConverters
{
    /// <summary>
    /// Represents a helper, which makes it easy to create collections of many different kinds via one, simple API.
    /// </summary>
    public static class CollectionHelper
    {
        #region Private Static Fields

        /// <summary>
        /// Contains a list of all the collection types that are supported by the collection helper.
        /// </summary>
        private static List<Type> supportedTypes = new List<Type>
        {
            // System.Collections
            typeof(ArrayList),
            typeof(Queue),
            typeof(Stack),
            typeof(ICollection),
            typeof(IEnumerable),
            typeof(IList),

            // System.Collections.Generic
            typeof(HashSet<>),
            typeof(LinkedList<>),
            typeof(List<>),
            typeof(Queue<>),
            typeof(SortedSet<>),
            typeof(Stack<>),
            typeof(ICollection<>),
            typeof(IEnumerable<>),
            typeof(IList<>),
            typeof(IReadOnlyCollection<>),
            typeof(IReadOnlyList<>),
            typeof(ISet<>),

            // System.Collections.ObjectModel
            typeof(Collection<>),
            typeof(ObservableCollection<>),
            typeof(ReadOnlyCollection<>),
            typeof(ReadOnlyObservableCollection<>)
        };

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Determines if a given type is one of the supported collection types.
        /// </summary>
        /// <param name="type">The type that is to be checked for support.</param>
        /// <returns>Returns <c>true</c> if the specified type is one of the supported collection types and <c>false</c> otherwise.</returns>
        public static bool IsSupportedCollectionType(Type type)
        {
            if (type.IsArray)
            {
                if (type.GetElementType().IsArray)
                    return false;
                return type.GetArrayRank() == 1;
            }
            if (type.IsConstructedGenericType)
                type = type.GetGenericTypeDefinition();
            return CollectionHelper.supportedTypes.Contains(type);
        }

        /// <summary>
        /// Retrieves the element type of the specified collection.
        /// </summary>
        /// <param name="type">The collection type for which the element type is to be determined.</param>
        /// <returns>Returns the type of the elements of the specified collection type. If the collection type is not generic <c>typeof(object)</c> is returned.</returns>
        public static Type GetCollectionElementType(Type type)
        {
            if (!CollectionHelper.IsSupportedCollectionType(type))
                throw new InvalidOperationException("The specified type is not a supported collection type.");

            if (type.IsArray)
                return type.GetElementType();
            if (type.IsConstructedGenericType)
                return type.GetGenericArguments()[0];
            return typeof(object);
        }

        /// <summary>
        /// Merges two collection into one collection of the same type.
        /// </summary>
        /// <param name="resultType">The type of collection into which the result is to be converted.</param>
        /// <param name="firstCollection">The first collection.</param>
        /// <param name="secondCollection">The second collection.</param>
        /// <returns>Returns a collection that contains the elements of both collections, which is of the same type as the input collections.</returns>
        public static object Merge(Type resultType, object firstCollection, object secondCollection)
        {
            // Validates that the two collections are of a supported collection type
            if (!CollectionHelper.IsSupportedCollectionType(firstCollection.GetType()))
                throw new InvalidOperationException("The type of the first collection is not a supported collection type.");
            if (!CollectionHelper.IsSupportedCollectionType(firstCollection.GetType()))
                throw new InvalidOperationException("The type of the second collection is not a supported collection type.");
            if (!CollectionHelper.IsSupportedCollectionType(resultType))
                throw new InvalidOperationException("The result type is not a supported collection type.");

            // Converts the two collections to arrays first
            Array firstArray = CollectionHelper.ToArray(firstCollection);
            Array secondArray = CollectionHelper.ToArray(secondCollection);

            // Combines the two arrays into a single array
            object[] mergedArray = new object[firstArray.Length + secondArray.Length];
            Array.Copy(firstArray, mergedArray, firstArray.Length);
            Array.Copy(secondArray, 0, mergedArray, firstArray.Length, secondArray.Length);

            // Converts the merged array back to the collection type and returns it
            return CollectionHelper.To(resultType, mergedArray);
        }

        /// <summary>
        /// Converts the specified collection to a collection of another containing the same elements.
        /// </summary>
        /// <param name="resultType">The type of collection into which the result is to be converted.</param>
        /// <param name="inputCollection">The collection, that is to be converted to another collection type.</param>
        /// <returns>Returns the converted collection.</returns>
        public static object To(Type resultType, object inputCollection)
        {
            // Checks if both collection types are supported
            if (!CollectionHelper.IsSupportedCollectionType(inputCollection.GetType()))
                throw new InvalidOperationException("The type of the input collection is not a supported collection type.");
            if (!CollectionHelper.IsSupportedCollectionType(resultType))
                throw new InvalidOperationException("The result type is not a supported collection type.");

            // First the input collection is converted to an array and then the array is converted to the result collection type
            Array array = CollectionHelper.ToArray(inputCollection);
            return CollectionHelper.FromArray(resultType, array);
        }

        /// <summary>
        /// Converts the specified collection into an array with the same elements.
        /// </summary>
        /// <param name="inputCollection">The collection that is to be converted into an array.</param>
        /// <returns>Returns an array that contains the same elements of the specified input collection.</returns>
        public static Array ToArray(object inputCollection)
        {
            // Checks if the type of the input collection is supported
            Type type = inputCollection.GetType();
            if (!CollectionHelper.IsSupportedCollectionType(type))
                throw new InvalidOperationException("The type of the input collection is not a supported collection type.");

            // Checks if the collection already is an array, then nothing needs to be done
            if (type.IsArray)
                return inputCollection as Array;

            // Creates a new array
            int arraySize = CollectionHelper.GetCount(inputCollection);
            object[] array = new object[arraySize];

            // Checks if the input collection implements ICollection, in that case there is a way to copy its contents to an array
            ICollection collection = inputCollection as ICollection;
            if (collection != null)
            {
                collection.CopyTo(array, 0);
                return array;
            }

            // Checks if the input collection implements the IEnumerable interface, in that case the enumerator can be used to copy its contents to an array
            IEnumerable enumerable = inputCollection as IEnumerable;
            if (enumerable != null)
            {
                IEnumerator enumerator = enumerable.GetEnumerator();
                int currentIndex = 0;
                while (enumerator.MoveNext())
                {
                    array[currentIndex] = enumerator.Current;
                    currentIndex += 1;
                }
                return array;
            }

            // Since some weird error must have occurred (all supported collection types either implement ICollection or IEnumerator one or the other way), an exception is thrown
            throw new InvalidOperationException("The input collection type neither implements ICollection nor IEnumerable.");
        }

        /// <summary>
        /// Converts the specified array into another collection type with the same elements.
        /// </summary>
        /// <param name="resultType">The type of collection into which the result is to be converted.</param>
        /// <param name="inputArray">The array that is to be converted into another collection type.</param>
        /// <returns>Returns a collection that contains the same elements of the specified input array.</returns>
        public static object FromArray(Type resultType, Array inputArray)
        {
            // Checks if the type of the input collection is supported
            if (!CollectionHelper.IsSupportedCollectionType(resultType))
                throw new InvalidOperationException("The result type is not a supported collection type.");

            // Determines which type the elements will have in the result collection
            Type elementType;
            if (resultType.IsGenericType)
                elementType = resultType.GenericTypeArguments[0];
            else if (resultType.IsArray)
                elementType = resultType.GetElementType();
            else
                elementType = typeof(object);
            if (resultType.IsConstructedGenericType)
                resultType = resultType.GetGenericTypeDefinition();

            // Checks if the result collection is an array and performs the necessary conversions
            if (resultType.IsArray)
            {
                Array resultArray = Array.CreateInstance(elementType, inputArray.Length);
                Array.Copy(inputArray, resultArray, inputArray.Length);
                return resultArray;
            }

            // Checks if the result collection is ICollection, IEnumerable, or IList, in that case nothing needs to be done, because Array already implements all three of them
            if (resultType == typeof(ICollection) || resultType == typeof(IEnumerable) || resultType == typeof(IList))
                return inputArray;

            // Checks if the result collection is a non-generic collection type, all non-generic collection types implement a constructor that takes an ICollection as a parameter,
            // so reflection can be used to instantiated instances of them
            if (resultType == typeof(ArrayList) || resultType == typeof(Queue) || resultType == typeof(Stack))
            {
                ConstructorInfo collectionConstructor = resultType.GetConstructor(new Type[] { typeof(ICollection) });
                object newCollection = collectionConstructor.Invoke(new object[] { inputArray });
                return newCollection;
            }

            // Since all of the following types are generic types, the input array is first cast into a generic IEnumerable
            IEnumerable enumerable = inputArray as IEnumerable;
            MethodInfo castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod( new System.Type[]{ elementType } );
            object genericEnumerable = castMethod.Invoke(null, new object[] { enumerable });

            // Many of the generic collection types have a constructor that takes a generic IEnumerable as a parameter, so reflection can be used to instantiated instances of them
            if (resultType == typeof(HashSet<>) ||
                resultType == typeof(LinkedList<>) ||
                resultType == typeof(List<>) ||
                resultType == typeof(Queue<>) ||
                resultType == typeof(SortedSet<>) ||
                resultType == typeof(Stack<>) ||
                resultType == typeof(ObservableCollection<>) ||
                resultType == typeof(IEnumerable<>) ||
                resultType == typeof(IList<>) ||
                resultType == typeof(ISet<>))
            {
                if (resultType == typeof(IEnumerable<>) || resultType == typeof(IList<>))
                    resultType = typeof(List<>);
                if (resultType == typeof(ISet<>))
                    resultType = typeof(SortedSet<>);

                Type collectionType = resultType.MakeGenericType(new Type[] { elementType });
                ConstructorInfo collectionConstructor = collectionType.GetConstructor(new Type[] { genericEnumerable.GetType() });
                object newCollection = collectionConstructor.Invoke(new object[] { genericEnumerable });
                return newCollection;
            }

            // The ReadOnlyObservableCollection<> is a special case, as it only one constructor, which takes a ObservableCollection<> as parameter, so reflection can be used to instantiated instances of it
            if (resultType == typeof(ReadOnlyObservableCollection<>))
            {
                Type observableCollectionType = typeof(ObservableCollection<>).MakeGenericType(new Type[] { elementType });
                ConstructorInfo observableCollectionConstructor = observableCollectionType.GetConstructor(new Type[] { genericEnumerable.GetType() });
                object observableCollection = observableCollectionConstructor.Invoke(new object[] { genericEnumerable });
                Type collectionType = typeof(ReadOnlyObservableCollection<>).MakeGenericType(new Type[] { elementType });
                ConstructorInfo collectionConstructor = collectionType.GetConstructor(new Type[] { observableCollectionType });
                object newCollection = collectionConstructor.Invoke(new object[] { observableCollection });
                return newCollection;
            }

            // The Collection<> and ReadOnlyCollection<> are special cases, as they have constructors, which take generic IList values as parameters, so reflection can be used to instantiated instances of them
            if (resultType == typeof(Collection<>) ||
                resultType == typeof(ReadOnlyCollection<>) ||
                resultType == typeof(ICollection<>) ||
                resultType == typeof(IReadOnlyCollection<>) ||
                resultType == typeof(IReadOnlyList<>))
            {
                if (resultType == typeof(ICollection<>))
                    resultType = typeof(Collection<>);
                if (resultType == typeof(IReadOnlyCollection<>) || resultType == typeof(IReadOnlyList<>))
                    resultType = typeof(ReadOnlyCollection<>);

                Type listType = typeof(List<>).MakeGenericType(new Type[] { elementType });
                ConstructorInfo listConstructor = listType.GetConstructor(new Type[] { genericEnumerable.GetType() });
                object newList = listConstructor.Invoke(new object[] { genericEnumerable });
                Type collectionType = resultType.MakeGenericType(new Type[] { elementType });
                ConstructorInfo collectionConstructor = collectionType.GetConstructor(new Type[] { listType });
                object newCollection = collectionConstructor.Invoke(new object[] { newList });
                return newCollection;
            }

            throw new InvalidOperationException("The array could not be converted.");
        }

        /// <summary>
        /// Gets the number of elements in the specified collection.
        /// </summary>
        /// <param name="inputCollection">The collection for which the number of elements is to be determined.</param>
        /// <returns>Returns the number of elements that are in the specified collection.</returns>
        public static int GetCount(object inputCollection)
        {
            // Determines if the specified collection is of a supported type
            if (!CollectionHelper.IsSupportedCollectionType(inputCollection.GetType()))
                throw new InvalidOperationException("The type of the input collection is not a supported collection type.");

            // Checks if the collection implements ICollection, in that case, the length of the collection can directly be accessed
            ICollection collection = inputCollection as ICollection;
            if (collection != null)
                return collection.Count;

            // Since the collection does not implement ICollection, the other way to retrieve length is via IEnumerable and IEnumerator
            IEnumerable enumerable = inputCollection as IEnumerable;
            if (enumerable != null)
            {
                int count = 0;
                IEnumerator enumerator = enumerable.GetEnumerator();
                while (enumerator.MoveNext())
                    count += 1;
                return count;
            }

            // Since some weird error must have occurred (all supported collection types either implement ICollection or IEnumerator one or the other way), an exception is thrown
            throw new InvalidOperationException("The input collection type neither implements ICollection nor IEnumerable.");
        }

        #endregion
    }
}
