
#region Using Directives

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

#endregion

namespace System.CommandLine.Arguments
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
        public static bool IsSupportedCollectionType<T>()
        {
            Type type = typeof(T);
            if (type.IsArray)
                return true;
            if (!type.IsConstructedGenericType)
                return false;
            return CollectionHelper.supportedTypes.Contains(type.GetGenericTypeDefinition());
        }

        /// <summary>
        /// Merges two collection into one collection of the same type.
        /// </summary>
        /// <param name="firstCollection">The first collection.</param>
        /// <param name="secondCollection">The second collection.</param>
        /// <returns>Returns a collection that contains the elements of both collections, which is of the same type as the input collections.</returns>
        public static T Merge<T>(T firstCollection, T secondCollection)
            where T : class
        {
            // Validates that the two collections are of a supported collection type
            if (!CollectionHelper.IsSupportedCollectionType<T>())
                throw new InvalidOperationException("The specified type is not a supported collection type.");

            // Converts the two collections to arrays first
            Array firstArray = CollectionHelper.ToArray<T>(firstCollection);
            Array secondArray = CollectionHelper.ToArray<T>(secondCollection);

            // Combines the two arrays into a single array
            object[] mergedArray = new object[firstArray.Length + secondArray.Length];
            Array.Copy(firstArray, mergedArray, firstArray.Length);
            Array.Copy(secondArray, 0, mergedArray, firstArray.Length, secondArray.Length);

            // Converts the merged array back to the collection type and returns it
            return CollectionHelper.To<object[], T>(mergedArray);
        }

        /// <summary>
        /// Converts the specified collection to a collection of another containing the same elements.
        /// </summary>
        /// <param name="inputCollection">The collection, that is to be converted to another collection type.</param>
        /// <returns>Returns the converted collection.</returns>
        public static TResult To<TInput, TResult>(TInput inputCollection)
            where TResult : class
        {
            // Checks if both collection types are supported
            if (!CollectionHelper.IsSupportedCollectionType<TInput>())
                throw new InvalidOperationException("The type of the specified collection is not a supported collection type.");
            if (!CollectionHelper.IsSupportedCollectionType<TResult>())
                throw new InvalidOperationException("The type of the result is not a supported collection type.");

            // First the input collection is converted to an array and then the array is converted to the result collection type
            Array array = CollectionHelper.ToArray(inputCollection);
            return CollectionHelper.FromArray<TResult>(array);
        }

        /// <summary>
        /// Converts the specified collection to a collection of another containing the same elements.
        /// </summary>
        /// <param name="inputCollection">The collection, that is to be converted to another collection type.</param>
        /// <returns>Returns the converted collection.</returns>
        public static TResult From<TInput, TResult>(TInput inputCollection)
            where TResult : class
        {
            // Checks if both collection types are supported
            if (!CollectionHelper.IsSupportedCollectionType<TInput>())
                throw new InvalidOperationException("The type of the specified collection is not a supported collection type.");
            if (!CollectionHelper.IsSupportedCollectionType<TResult>())
                throw new InvalidOperationException("The type of the result is not a supported collection type.");

            // First the input collection is converted to an array and then the array is converted to the result collection type
            Array array = CollectionHelper.ToArray<TInput>(inputCollection);
            return CollectionHelper.FromArray<TResult>(array);
        }

        /// <summary>
        /// Converts the specified collection into an array with the same elements.
        /// </summary>
        /// <param name="inputCollection">The collection that is to be converted into an array.</param>
        /// <returns>Returns an array that contains the same elements of the specified input collection.</returns>
        public static Array ToArray<T>(T inputCollection)
        {
            // Checks if the type of the input collection is supported
            if (!CollectionHelper.IsSupportedCollectionType<T>())
                throw new InvalidOperationException("The type of the specified collection is not a supported collection type.");

            // Checks if the collection already is an array, then nothing needs to be done
            Type type = typeof(T);
            if (type.IsArray)
                return inputCollection as Array;

            // Creates a new array
            int arraySize = CollectionHelper.GetCount<T>(inputCollection);
            object[] array = new object[arraySize];

            // Checks if the input collection implements ICollection, in that case there is a way to copy its contents to an array
            ICollection collection = inputCollection as ICollection;
            if (collection != null)
            {
                collection.CopyTo(array, arraySize);
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
            throw new InvalidOperationException("The specified collection type neither implements ICollection nor IEnumerable.");
        }

        /// <summary>
        /// Converts the specified array into another collection type with the same elements.
        /// </summary>
        /// <param name="inputArray">The array that is to be converted into another collection type.</param>
        /// <returns>Returns a collection that contains the same elements of the specified input array.</returns>
        public static T FromArray<T>(Array inputArray)
            where T : class
        {
            // Checks if the type of the input collection is supported
            if (!CollectionHelper.IsSupportedCollectionType<T>())
                throw new InvalidOperationException("The return type is not a supported collection type.");

            // Determines which type the elements will have in the result collection
            Type type = typeof(T);
            Type elementType;
            if (type.IsGenericType)
                elementType = type.GenericTypeArguments[0];
            else if (type.IsArray)
                elementType = type.GetElementType();
            else
                elementType = typeof(object);
            if (type.IsConstructedGenericType)
                type = type.GetGenericTypeDefinition();

            // Checks if the result collection is an array and performs the necessary conversions
            if (type.IsArray)
            {
                Array resultArray = Array.CreateInstance(elementType, inputArray.Length);
                Array.Copy(inputArray, resultArray, inputArray.Length);
                return resultArray as T;
            }

            // Checks if the result collection is ICollection, IEnumerable, or IList, in that case nothing needs to be done, because Array already implements all three of them
            if (type == typeof(ICollection) || type == typeof(IEnumerable) || type == typeof(IList))
                return inputArray as T;

            // Checks if the result collection is a non-generic collection type, all non-generic collection types implement a constructor that takes an ICollection as a parameter, so reflection can be used to instantiated
            // instances of them
            if (type == typeof(ArrayList) || type == typeof(Queue) || type == typeof(Stack))
            {
                ConstructorInfo collectionConstructor = type.GetConstructor(new Type[] { typeof(ICollection) });
                object newCollection = collectionConstructor.Invoke(new object[] { inputArray });
                return newCollection as T;
            }

            // Since all of the following types are generic types, the input array is first cast into a generic IEnumerable
            IEnumerable enumerable = inputArray as IEnumerable;
            MethodInfo castMethod = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod( new System.Type[]{ elementType } );
            object genericEnumerable = castMethod.Invoke(null, new object[] { enumerable });

            // Many of the generic collection types have a constructor that takes a generic IEnumerable as a parameter, so reflection can be used to instantiated instances of them
            if (type == typeof(HashSet<>) ||
                type == typeof(LinkedList<>) ||
                type == typeof(List<>) ||
                type == typeof(Queue<>) ||
                type == typeof(SortedSet<>) ||
                type == typeof(Stack<>) ||
                type == typeof(ObservableCollection<>) ||
                type == typeof(IEnumerable<>) ||
                type == typeof(IList<>) ||
                type == typeof(ISet<>))
            {
                if (type == typeof(IEnumerable<>) || type == typeof(IList<>))
                    type = typeof(List<>);
                if (type == typeof(ISet<>))
                    type = typeof(SortedSet<>);

                Type collectionType = type.MakeGenericType(new Type[] { elementType });
                ConstructorInfo collectionConstructor = collectionType.GetConstructor(new Type[] { genericEnumerable.GetType() });
                object newCollection = collectionConstructor.Invoke(new object[] { genericEnumerable });
                return newCollection as T;
            }

            // The ReadOnlyObservableCollection<> is a special case, as it only one constructor, which takes a ObservableCollection<> as parameter, so reflection can be used to instantiated instances of it
            if (type == typeof(ReadOnlyObservableCollection<>))
            {
                Type observableCollectionType = typeof(ObservableCollection<>).MakeGenericType(new Type[] { elementType });
                ConstructorInfo observableCollectionConstructor = observableCollectionType.GetConstructor(new Type[] { genericEnumerable.GetType() });
                object observableCollection = observableCollectionConstructor.Invoke(new object[] { genericEnumerable });
                Type collectionType = typeof(ReadOnlyObservableCollection<>).MakeGenericType(new Type[] { elementType });
                ConstructorInfo collectionConstructor = collectionType.GetConstructor(new Type[] { observableCollectionType });
                object newCollection = collectionConstructor.Invoke(new object[] { observableCollection });
                return newCollection as T;
            }

            // The Collection<> and ReadOnlyCollection<> are special cases, as they have constructors, which take generic IList values as parameters, so reflection can be used to instantiated instances of them
            if (type == typeof(Collection<>) ||
                type == typeof(ReadOnlyCollection<>) ||
                type == typeof(ICollection<>) ||
                type == typeof(IReadOnlyCollection<>) ||
                type == typeof(IReadOnlyList<>))
            {
                if (type == typeof(ICollection<>))
                    type = typeof(Collection<>);
                if (type == typeof(IReadOnlyCollection<>) || type == typeof(IReadOnlyList<>))
                    type = typeof(ReadOnlyCollection<>);

                Type listType = typeof(List<>).MakeGenericType(new Type[] { elementType });
                ConstructorInfo listConstructor = listType.GetConstructor(new Type[] { genericEnumerable.GetType() });
                object newList = listConstructor.Invoke(new object[] { genericEnumerable });
                Type collectionType = type.MakeGenericType(new Type[] { elementType });
                ConstructorInfo collectionConstructor = collectionType.GetConstructor(new Type[] { listType });
                object newCollection = collectionConstructor.Invoke(new object[] { newList });
                return newCollection as T;
            }

            throw new InvalidOperationException("The array could not be converted.");
        }

        /// <summary>
        /// Gets the number of elements in the specified collection.
        /// </summary>
        /// <param name="inputCollection">The collection for which the number of elements is to be determined.</param>
        /// <returns>Returns the number of elements that are in the specified collection.</returns>
        public static int GetCount<T>(T inputCollection)
        {
            // Determines if the specified collection is of a supported type
            if (!CollectionHelper.IsSupportedCollectionType<T>())
                throw new InvalidOperationException("The type of the specified collection is not a supported collection type.");

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
            throw new InvalidOperationException("The specified collection type neither implements ICollection nor IEnumerable.");
        }

        #endregion
    }
}