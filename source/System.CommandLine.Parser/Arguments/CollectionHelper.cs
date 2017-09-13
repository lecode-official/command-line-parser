
#region Using Directives

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace System.CommandLine.Arguments
{
    /// <summary>
    /// Represents a helper, which makes it easy to create collections of many different kinds via one, simple API.
    /// </summary>
    public class CollectionHelper<T>
    {
        #region Private Static Fields

        /// <summary>
        /// Contains a list of all the collection types that are supported by the collection helper.
        /// </summary>
        private static List<Type> supportedTypes = new List<Type>
        {
            // System
            typeof(Array),

            // System.Collections
            typeof(ArrayList),
            typeof(Hashtable),
            typeof(Queue),
            typeof(SortedList),
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
            typeof(ReadOnlyObservableCollection<>),
        };

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Determines if a given type is one of the supported collection types.
        /// </summary>
        /// <param name="type">The type that is to be checked for support.</param>
        /// <returns>Returns <c>true</c> if the specified type is one of the supported collection types and <c>false</c> otherwise.</returns>
        public static bool IsSupportedCollectionType()
        {
            Type type = typeof(T);
            if (type.IsGenericType && type.IsConstructedGenericType)
                type = type.GetGenericTypeDefinition();
            return CollectionHelper<T>.supportedTypes.Contains(type);
        }

        public static T Merge(T firstCollection, T secondCollection)
        {
            return default(T);
        }

        public static TResult To<TResult>(T collection)
        {
            return default(TResult);
        }

        public static T From<TInput>(TInput collection)
        {
            return default(T);
        }

        #endregion
    }
}