
#region Using Directives

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.CommandLine.ValueConverters;
using System.Linq;
using Xunit;

#endregion

namespace System.CommandLine.Parser.Tests
{
    /// <summary>
    /// Represents a set of unit tests for the <see cref="CollectionHelper"/>.
    /// </summary>
    public class CollectionHelperTests
    {
        #region Unit Tests

        /// <summary>
        /// Tests the ability of the <see cref="CollectionHelper"/> to detect supported collection types.
        /// </summary>
        [Fact]
        public void TestIsSupportedCollectionType()
        {
            Assert.True(CollectionHelper.IsSupportedCollectionType<int[]>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<float[]>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<double[]>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<string[]>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<bool[]>());

            Assert.True(CollectionHelper.IsSupportedCollectionType<ArrayList>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<Queue>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<Stack>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<ICollection>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<IEnumerable>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<IList>());

            Assert.True(CollectionHelper.IsSupportedCollectionType<HashSet<int>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<LinkedList<float>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<List<double>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<Queue<string>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<SortedSet<bool>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<Stack<int>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<ICollection<float>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<IEnumerable<double>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<IList<string>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<IReadOnlyCollection<bool>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<IReadOnlyList<int>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<ISet<float>>());


            Assert.True(CollectionHelper.IsSupportedCollectionType<Collection<double>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<ObservableCollection<string>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<ReadOnlyCollection<bool>>());
            Assert.True(CollectionHelper.IsSupportedCollectionType<ReadOnlyObservableCollection<int>>());

            Assert.False(CollectionHelper.IsSupportedCollectionType<int>());
            Assert.False(CollectionHelper.IsSupportedCollectionType<float>());
            Assert.False(CollectionHelper.IsSupportedCollectionType<double>());
            Assert.False(CollectionHelper.IsSupportedCollectionType<string>());
            Assert.False(CollectionHelper.IsSupportedCollectionType<bool>());
            Assert.False(CollectionHelper.IsSupportedCollectionType<object>());
            Assert.False(CollectionHelper.IsSupportedCollectionType<Type>());
            Assert.False(CollectionHelper.IsSupportedCollectionType<int[,]>());
            Assert.False(CollectionHelper.IsSupportedCollectionType<float[][]>());
        }

        /// <summary>
        /// Tests the ability of the <see cref="CollectionHelper"/> to create any kind of collection from an array.
        /// </summary>
        [Fact]
        public void TestFromArray()
        {
            int[] integerArray = new int[] { 1, 2, 3, 4, 5 };
            double[] doubleArray = new double[] { 3.14159, 2.71828 };
            string[] stringArray = new string[] { "foo", "bar", "foobar" };

            ArrayList arrayList = CollectionHelper.FromArray<ArrayList>(integerArray);
            Assert.Equal(5, arrayList.Count);
            Assert.Equal(1, arrayList[0]);
            Assert.Equal(2, arrayList[1]);
            Assert.Equal(3, arrayList[2]);
            Assert.Equal(4, arrayList[3]);
            Assert.Equal(5, arrayList[4]);

            Queue queue = CollectionHelper.FromArray<Queue>(doubleArray);
            Assert.Equal(2, queue.Count);
            Assert.Equal(3.14159, queue.Dequeue());
            Assert.Equal(2.71828, queue.Dequeue());

            Stack stack = CollectionHelper.FromArray<Stack>(stringArray);
            Assert.Equal(3, stack.Count);
            Assert.Equal("foobar", stack.Pop());
            Assert.Equal("bar", stack.Pop());
            Assert.Equal("foo", stack.Pop());

            HashSet<int> genericHashSet = CollectionHelper.FromArray<HashSet<int>>(integerArray);
            Assert.Equal(5, genericHashSet.Count);
            Assert.Equal(1, genericHashSet.ElementAt(0));
            Assert.Equal(2, genericHashSet.ElementAt(1));
            Assert.Equal(3, genericHashSet.ElementAt(2));
            Assert.Equal(4, genericHashSet.ElementAt(3));
            Assert.Equal(5, genericHashSet.ElementAt(4));

            LinkedList<double> genericLinkedList = CollectionHelper.FromArray<LinkedList<double>>(doubleArray);
            Assert.Equal(2, genericLinkedList.Count);
            Assert.Equal(3.14159, genericLinkedList.ElementAt(0));
            Assert.Equal(2.71828, genericLinkedList.ElementAt(1));

            List<string> genericList = CollectionHelper.FromArray<List<string>>(stringArray);
            Assert.Equal(3, genericList.Count);
            Assert.Equal("foo", genericList[0]);
            Assert.Equal("bar", genericList[1]);
            Assert.Equal("foobar", genericList[2]);

            Queue<int> genericQueue = CollectionHelper.FromArray<Queue<int>>(integerArray);
            Assert.Equal(5, genericQueue.Count);
            Assert.Equal(1, genericQueue.ElementAt(0));
            Assert.Equal(2, genericQueue.ElementAt(1));
            Assert.Equal(3, genericQueue.ElementAt(2));
            Assert.Equal(4, genericQueue.ElementAt(3));
            Assert.Equal(5, genericQueue.ElementAt(4));

            SortedSet<double> genericSortedSet = CollectionHelper.FromArray<SortedSet<double>>(doubleArray);
            Assert.Equal(2, genericSortedSet.Count);
            Assert.Equal(2.71828, genericSortedSet.ElementAt(0));
            Assert.Equal(3.14159, genericSortedSet.ElementAt(1));

            Stack<string> genericStack = CollectionHelper.FromArray<Stack<string>>(stringArray);
            Assert.Equal(3, genericStack.Count);
            Assert.Equal("foobar", genericStack.Pop());
            Assert.Equal("bar", genericStack.Pop());
            Assert.Equal("foo", genericStack.Pop());

            Collection<int> genericCollection = CollectionHelper.FromArray<Collection<int>>(integerArray);
            Assert.Equal(5, genericCollection.Count);
            Assert.Equal(1, genericCollection.ElementAt(0));
            Assert.Equal(2, genericCollection.ElementAt(1));
            Assert.Equal(3, genericCollection.ElementAt(2));
            Assert.Equal(4, genericCollection.ElementAt(3));
            Assert.Equal(5, genericCollection.ElementAt(4));

            ObservableCollection<double> genericObservableCollection = CollectionHelper.FromArray<ObservableCollection<double>>(doubleArray);
            Assert.Equal(2, genericObservableCollection.Count);
            Assert.Equal(3.14159, genericObservableCollection.ElementAt(0));
            Assert.Equal(2.71828, genericObservableCollection.ElementAt(1));

            ReadOnlyCollection<string> genericReadOnlyCollection = CollectionHelper.FromArray<ReadOnlyCollection<string>>(stringArray);
            Assert.Equal(3, genericReadOnlyCollection.Count);
            Assert.Equal("foo", genericReadOnlyCollection.ElementAt(0));
            Assert.Equal("bar", genericReadOnlyCollection.ElementAt(1));
            Assert.Equal("foobar", genericReadOnlyCollection.ElementAt(2));

            ReadOnlyObservableCollection<int> genericReadOnlyObservableCollection = CollectionHelper.FromArray<ReadOnlyObservableCollection<int>>(integerArray);
            Assert.Equal(5, genericReadOnlyObservableCollection.Count);
            Assert.Equal(1, genericReadOnlyObservableCollection.ElementAt(0));
            Assert.Equal(2, genericReadOnlyObservableCollection.ElementAt(1));
            Assert.Equal(3, genericReadOnlyObservableCollection.ElementAt(2));
            Assert.Equal(4, genericReadOnlyObservableCollection.ElementAt(3));
            Assert.Equal(5, genericReadOnlyObservableCollection.ElementAt(4));
        }

        /// <summary>
        /// Tests the ability of the <see cref="CollectionHelper"/> to determine the length of any collection.
        /// </summary>
        [Fact]
        public void TestCollectionLength()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(5, CollectionHelper.GetCount(array));

            ArrayList arrayList = new ArrayList { "foo", "bar", "foobar" };
            Assert.Equal(3, CollectionHelper.GetCount(arrayList));

            Queue queue = new Queue();
            queue.Enqueue(1.0);
            queue.Enqueue(2.0);
            queue.Enqueue(3.0);
            Assert.Equal(3, CollectionHelper.GetCount(queue));

            Stack stack = new Stack();
            stack.Push(true);
            Assert.Equal(1, CollectionHelper.GetCount(stack));

            HashSet<int> genericHashSet = new HashSet<int> { 1, 2, 3, 4, 5, 6 };
            Assert.Equal(6, CollectionHelper.GetCount(genericHashSet));

            LinkedList<string> genericLinkedList = new LinkedList<string>();
            genericLinkedList.AddFirst("foo");
            genericLinkedList.AddFirst("bar");
            Assert.Equal(2, CollectionHelper.GetCount(genericLinkedList));

            List<double> genericList = new List<double> { 1.2, 2.3, 3.4, 4.5, 5.6, 6.7 };
            Assert.Equal(6, CollectionHelper.GetCount(genericList));

            Queue<bool> genericQueue = new Queue<bool>();
            genericQueue.Enqueue(false);
            Assert.Equal(1, CollectionHelper.GetCount(genericQueue));

            SortedSet<int> genericSortedSet = new SortedSet<int> { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
            Assert.Equal(10, CollectionHelper.GetCount(genericSortedSet));

            Stack<double> genericStack = new Stack<double>();
            genericStack.Push(3.14159);
            genericStack.Push(2.71828);
            Assert.Equal(2, CollectionHelper.GetCount(genericStack));

            Collection<string> genericCollection = new Collection<string>();
            genericCollection.Add("foo");
            genericCollection.Add("bar");
            genericCollection.Add("foobar");
            Assert.Equal(3, CollectionHelper.GetCount(genericCollection));

            ObservableCollection<bool> genericObservableCollection = new ObservableCollection<bool>() { false, true };
            Assert.Equal(2, CollectionHelper.GetCount(genericObservableCollection));

            ReadOnlyCollection<float> genericReadOnlyCollection = new ReadOnlyCollection<float>(new List<float> { 1.0f, 2.0f, 3.0f });
            Assert.Equal(3, CollectionHelper.GetCount(genericReadOnlyCollection));

            ReadOnlyObservableCollection<string> genericReadOnlyObservableCollection = new ReadOnlyObservableCollection<string>(new ObservableCollection<string> { "bar" });
            Assert.Equal(1, CollectionHelper.GetCount(genericReadOnlyObservableCollection));
        }

        /// <summary>
        /// Tests the ability of the <see cref="CollectionHelper"/> to convert arrays to any collection type.
        /// </summary>
        [Fact]
        public void TestToArray()
        {
            int[] integerArray = new int[] { 1, 2, 3, 4, 5 };
            Array array = CollectionHelper.ToArray(integerArray);
            Assert.Equal(5, array.Length);
            Assert.Equal(1, array.GetValue(0));
            Assert.Equal(2, array.GetValue(1));
            Assert.Equal(3, array.GetValue(2));
            Assert.Equal(4, array.GetValue(3));
            Assert.Equal(5, array.GetValue(4));

            ArrayList arrayList = new ArrayList { "foo", "bar", "foobar" };
            array = CollectionHelper.ToArray(arrayList);
            Assert.Equal(3, array.Length);
            Assert.Equal("foo", array.GetValue(0));
            Assert.Equal("bar", array.GetValue(1));
            Assert.Equal("foobar", array.GetValue(2));

            Queue queue = new Queue();
            queue.Enqueue(1.0);
            queue.Enqueue(2.0);
            queue.Enqueue(3.0);
            array = CollectionHelper.ToArray(queue);
            Assert.Equal(3, array.Length);
            Assert.Equal(1.0, array.GetValue(0));
            Assert.Equal(2.0, array.GetValue(1));
            Assert.Equal(3.0, array.GetValue(2));

            Stack stack = new Stack();
            stack.Push(true);
            array = CollectionHelper.ToArray(stack);
            Assert.Equal(1, array.Length);
            Assert.Equal(true, array.GetValue(0));

            HashSet<int> genericHashSet = new HashSet<int> { 1, 2, 3, 4, 5, 6 };
            array = CollectionHelper.ToArray(genericHashSet);
            Assert.Equal(6, array.Length);
            Assert.Equal(1, array.GetValue(0));
            Assert.Equal(2, array.GetValue(1));
            Assert.Equal(3, array.GetValue(2));
            Assert.Equal(4, array.GetValue(3));
            Assert.Equal(5, array.GetValue(4));
            Assert.Equal(6, array.GetValue(5));

            LinkedList<string> genericLinkedList = new LinkedList<string>();
            genericLinkedList.AddFirst("foo");
            genericLinkedList.AddFirst("bar");
            array = CollectionHelper.ToArray(genericLinkedList);
            Assert.Equal(2, array.Length);
            Assert.Equal("bar", array.GetValue(0));
            Assert.Equal("foo", array.GetValue(1));

            List<double> genericList = new List<double> { 1.2, 2.3, 3.4, 4.5, 5.6, 6.7 };
            array = CollectionHelper.ToArray(genericList);
            Assert.Equal(6, array.Length);
            Assert.Equal(1.2, array.GetValue(0));
            Assert.Equal(2.3, array.GetValue(1));
            Assert.Equal(3.4, array.GetValue(2));
            Assert.Equal(4.5, array.GetValue(3));
            Assert.Equal(5.6, array.GetValue(4));
            Assert.Equal(6.7, array.GetValue(5));

            Queue<bool> genericQueue = new Queue<bool>();
            genericQueue.Enqueue(false);
            array = CollectionHelper.ToArray(genericQueue);
            Assert.Equal(1, array.Length);
            Assert.Equal(false, array.GetValue(0));

            SortedSet<int> genericSortedSet = new SortedSet<int> { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
            array = CollectionHelper.ToArray(genericSortedSet);
            Assert.Equal(10, array.Length);
            Assert.Equal(10, array.GetValue(0));
            Assert.Equal(20, array.GetValue(1));
            Assert.Equal(30, array.GetValue(2));
            Assert.Equal(40, array.GetValue(3));
            Assert.Equal(50, array.GetValue(4));
            Assert.Equal(60, array.GetValue(5));
            Assert.Equal(70, array.GetValue(6));
            Assert.Equal(80, array.GetValue(7));
            Assert.Equal(90, array.GetValue(8));
            Assert.Equal(100, array.GetValue(9));

            Stack<double> genericStack = new Stack<double>();
            genericStack.Push(3.14159);
            genericStack.Push(2.71828);
            array = CollectionHelper.ToArray(genericStack);
            Assert.Equal(2, array.Length);
            Assert.Equal(2.71828, array.GetValue(0));
            Assert.Equal(3.14159, array.GetValue(1));

            Collection<string> genericCollection = new Collection<string>();
            genericCollection.Add("foo");
            genericCollection.Add("bar");
            genericCollection.Add("foobar");
            array = CollectionHelper.ToArray(genericCollection);
            Assert.Equal(3, array.Length);
            Assert.Equal("foo", array.GetValue(0));
            Assert.Equal("bar", array.GetValue(1));
            Assert.Equal("foobar", array.GetValue(2));

            ObservableCollection<bool> genericObservableCollection = new ObservableCollection<bool>() { false, true };
            array = CollectionHelper.ToArray(genericObservableCollection);
            Assert.Equal(2, array.Length);
            Assert.Equal(false, array.GetValue(0));
            Assert.Equal(true, array.GetValue(1));

            ReadOnlyCollection<float> genericReadOnlyCollection = new ReadOnlyCollection<float>(new List<float> { 1.0f, 2.0f, 3.0f });
            array = CollectionHelper.ToArray(genericReadOnlyCollection);
            Assert.Equal(3, array.Length);
            Assert.Equal(1.0f, array.GetValue(0));
            Assert.Equal(2.0f, array.GetValue(1));
            Assert.Equal(3.0f, array.GetValue(2));

            ReadOnlyObservableCollection<string> genericReadOnlyObservableCollection = new ReadOnlyObservableCollection<string>(new ObservableCollection<string> { "bar" });
            array = CollectionHelper.ToArray(genericReadOnlyObservableCollection);
            Assert.Equal(1, array.Length);
            Assert.Equal("bar", array.GetValue(0));
        }

        #endregion
    }
}