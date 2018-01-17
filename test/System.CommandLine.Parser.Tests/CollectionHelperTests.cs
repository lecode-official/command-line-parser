
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
        /// Tests the ability of the <see cref="CollectionHelper"/> to convert any collection type to any other collection type.
        /// </summary>
        [Fact]
        public void TestTo()
        {
            int[] integerArray = new int[] { 1, 2, 3, 4, 5 };
            List<int> genericList = CollectionHelper.To(typeof(List<int>), integerArray) as List<int>;
            Assert.Equal(5, genericList.Count);
            Assert.Equal(1, genericList[0]);
            Assert.Equal(2, genericList[1]);
            Assert.Equal(3, genericList[2]);
            Assert.Equal(4, genericList[3]);
            Assert.Equal(5, genericList[4]);

            ArrayList arrayList = new ArrayList { "foo", "bar" };
            Collection<string> genericCollection = CollectionHelper.To(typeof(Collection<string>), arrayList) as Collection<string>;
            Assert.Equal(2, genericCollection.Count);
            Assert.Equal("foo", genericCollection[0]);
            Assert.Equal("bar", genericCollection[1]);

            ObservableCollection<bool> genericObservableCollection = new ObservableCollection<bool>(new List<bool> { true });
            Queue<bool> genericQueue = CollectionHelper.To(typeof(Queue<bool>), genericObservableCollection) as Queue<bool>;
            Assert.Equal(1, genericQueue.Count);
            Assert.Equal(true, genericQueue.Dequeue());

            Stack stack = new Stack();
            stack.Push(3.14159);
            stack.Push(2.71828);
            double[] doubleArray = CollectionHelper.To(typeof(double[]), stack) as double[];
            Assert.Equal(2, doubleArray.Length);
            Assert.Equal(2.71828, doubleArray[0]);
            Assert.Equal(3.14159, doubleArray[1]);

            LinkedList<float> genericLinkedList = new LinkedList<float>();
            genericLinkedList.AddFirst(1.0f);
            genericLinkedList.AddLast(2.0f);
            float[] floatArray = CollectionHelper.To(typeof(float[]), genericLinkedList) as float[];
            Assert.Equal(2, floatArray.Length);
            Assert.Equal(1.0f, floatArray[0]);
            Assert.Equal(2.0f, floatArray[1]);
        }

        /// <summary>
        /// Tests the ability of the <see cref="CollectionHelper"/> to merge different kinds of collections.
        /// </summary>
        public void TestMerge()
        {
            int[] firstIntegerArray = new int[] { 1, 2, 3 };
            int[] secondIntegerArray = new int[] { 4, 5 };
            int[] mergedIntegerArray = CollectionHelper.Merge(typeof(int[]), firstIntegerArray, secondIntegerArray) as int[];
            Assert.Equal(5, mergedIntegerArray.Length);
            Assert.Equal(1, mergedIntegerArray[0]);
            Assert.Equal(2, mergedIntegerArray[1]);
            Assert.Equal(3, mergedIntegerArray[2]);
            Assert.Equal(4, mergedIntegerArray[3]);
            Assert.Equal(5, mergedIntegerArray[4]);

            ArrayList firstArrayList = new ArrayList { "foo" };
            ArrayList secondArrayList = new ArrayList { "bar" };
            ArrayList mergedArrayList = CollectionHelper.Merge(typeof(ArrayList), firstArrayList, secondArrayList) as ArrayList;
            Assert.Equal(2, mergedArrayList.Count);
            Assert.Equal("foo", mergedArrayList[0]);
            Assert.Equal("bar", mergedArrayList[1]);

            List<double> firstGenericList = new List<double> { 2.71828, 6.62607 };
            List<double> secondGenericList = new List<double> { 3.14159 };
            List<double> mergedGenericList = CollectionHelper.Merge(typeof(List<double>), firstGenericList, secondGenericList) as List<double>;
            Assert.Equal(3, mergedGenericList.Count);
            Assert.Equal(2.71828, mergedGenericList.ElementAt(0));
            Assert.Equal(6.62607, mergedGenericList.ElementAt(1));
            Assert.Equal(3.14159, mergedGenericList.ElementAt(1));

            Queue firstQueue = new Queue();
            firstQueue.Enqueue(true);
            bool[] secondBooleanArray = new bool[] { false, true };
            Collection<bool> mergedGenericCollection = CollectionHelper.Merge(typeof(Collection<bool>), firstQueue, secondBooleanArray) as Collection<bool>;
            Assert.Equal(3, mergedGenericCollection.Count);
            Assert.Equal(true, mergedGenericCollection.ElementAt(0));
            Assert.Equal(false, mergedGenericCollection.ElementAt(1));
            Assert.Equal(true, mergedGenericCollection.ElementAt(1));
        }

        /// <summary>
        /// Tests the ability of the <see cref="CollectionHelper"/> to detect supported collection types.
        /// </summary>
        [Fact]
        public void TestIsSupportedCollectionType()
        {
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(int[])));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(float[])));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(double[])));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(string[])));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(bool[])));

            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(ArrayList)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(Queue)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(Stack)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(ICollection)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(IEnumerable)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(IList)));

            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(HashSet<int>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(LinkedList<float>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(List<double>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(Queue<string>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(SortedSet<bool>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(Stack<int>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(ICollection<float>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(IEnumerable<double>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(IList<string>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(IReadOnlyCollection<bool>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(IReadOnlyList<int>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(ISet<float>)));


            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(Collection<double>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(ObservableCollection<string>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(ReadOnlyCollection<bool>)));
            Assert.True(CollectionHelper.IsSupportedCollectionType(typeof(ReadOnlyObservableCollection<int>)));

            Assert.False(CollectionHelper.IsSupportedCollectionType(typeof(int)));
            Assert.False(CollectionHelper.IsSupportedCollectionType(typeof(float)));
            Assert.False(CollectionHelper.IsSupportedCollectionType(typeof(double)));
            Assert.False(CollectionHelper.IsSupportedCollectionType(typeof(string)));
            Assert.False(CollectionHelper.IsSupportedCollectionType(typeof(bool)));
            Assert.False(CollectionHelper.IsSupportedCollectionType(typeof(object)));
            Assert.False(CollectionHelper.IsSupportedCollectionType(typeof(Type)));
            Assert.False(CollectionHelper.IsSupportedCollectionType(typeof(int[,])));
            Assert.False(CollectionHelper.IsSupportedCollectionType(typeof(float[][])));
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

            ArrayList arrayList = CollectionHelper.FromArray(typeof(ArrayList), integerArray) as ArrayList;
            Assert.Equal(5, arrayList.Count);
            Assert.Equal(1, arrayList[0]);
            Assert.Equal(2, arrayList[1]);
            Assert.Equal(3, arrayList[2]);
            Assert.Equal(4, arrayList[3]);
            Assert.Equal(5, arrayList[4]);

            Queue queue = CollectionHelper.FromArray(typeof(Queue), doubleArray) as Queue;
            Assert.Equal(2, queue.Count);
            Assert.Equal(3.14159, queue.Dequeue());
            Assert.Equal(2.71828, queue.Dequeue());

            Stack stack = CollectionHelper.FromArray(typeof(Stack), stringArray) as Stack;
            Assert.Equal(3, stack.Count);
            Assert.Equal("foobar", stack.Pop());
            Assert.Equal("bar", stack.Pop());
            Assert.Equal("foo", stack.Pop());

            HashSet<int> genericHashSet = CollectionHelper.FromArray(typeof(HashSet<int>), integerArray) as HashSet<int>;
            Assert.Equal(5, genericHashSet.Count);
            Assert.Equal(1, genericHashSet.ElementAt(0));
            Assert.Equal(2, genericHashSet.ElementAt(1));
            Assert.Equal(3, genericHashSet.ElementAt(2));
            Assert.Equal(4, genericHashSet.ElementAt(3));
            Assert.Equal(5, genericHashSet.ElementAt(4));

            LinkedList<double> genericLinkedList = CollectionHelper.FromArray(typeof(LinkedList<double>), doubleArray) as LinkedList<double>;
            Assert.Equal(2, genericLinkedList.Count);
            Assert.Equal(3.14159, genericLinkedList.ElementAt(0));
            Assert.Equal(2.71828, genericLinkedList.ElementAt(1));

            List<string> genericList = CollectionHelper.FromArray(typeof(List<string>), stringArray) as List<string>;
            Assert.Equal(3, genericList.Count);
            Assert.Equal("foo", genericList[0]);
            Assert.Equal("bar", genericList[1]);
            Assert.Equal("foobar", genericList[2]);

            Queue<int> genericQueue = CollectionHelper.FromArray(typeof(Queue<int>), integerArray) as Queue<int>;
            Assert.Equal(5, genericQueue.Count);
            Assert.Equal(1, genericQueue.ElementAt(0));
            Assert.Equal(2, genericQueue.ElementAt(1));
            Assert.Equal(3, genericQueue.ElementAt(2));
            Assert.Equal(4, genericQueue.ElementAt(3));
            Assert.Equal(5, genericQueue.ElementAt(4));

            SortedSet<double> genericSortedSet = CollectionHelper.FromArray(typeof(SortedSet<double>), doubleArray) as SortedSet<double>;
            Assert.Equal(2, genericSortedSet.Count);
            Assert.Equal(2.71828, genericSortedSet.ElementAt(0));
            Assert.Equal(3.14159, genericSortedSet.ElementAt(1));

            Stack<string> genericStack = CollectionHelper.FromArray(typeof(Stack<string>), stringArray) as Stack<string>;
            Assert.Equal(3, genericStack.Count);
            Assert.Equal("foobar", genericStack.Pop());
            Assert.Equal("bar", genericStack.Pop());
            Assert.Equal("foo", genericStack.Pop());

            Collection<int> genericCollection = CollectionHelper.FromArray(typeof(Collection<int>), integerArray) as Collection<int>;
            Assert.Equal(5, genericCollection.Count);
            Assert.Equal(1, genericCollection.ElementAt(0));
            Assert.Equal(2, genericCollection.ElementAt(1));
            Assert.Equal(3, genericCollection.ElementAt(2));
            Assert.Equal(4, genericCollection.ElementAt(3));
            Assert.Equal(5, genericCollection.ElementAt(4));

            ObservableCollection<double> genericObservableCollection = CollectionHelper.FromArray(typeof(ObservableCollection<double>), doubleArray) as ObservableCollection<double>;
            Assert.Equal(2, genericObservableCollection.Count);
            Assert.Equal(3.14159, genericObservableCollection.ElementAt(0));
            Assert.Equal(2.71828, genericObservableCollection.ElementAt(1));

            ReadOnlyCollection<string> genericReadOnlyCollection = CollectionHelper.FromArray(typeof(ReadOnlyCollection<string>), stringArray) as ReadOnlyCollection<string>;
            Assert.Equal(3, genericReadOnlyCollection.Count);
            Assert.Equal("foo", genericReadOnlyCollection.ElementAt(0));
            Assert.Equal("bar", genericReadOnlyCollection.ElementAt(1));
            Assert.Equal("foobar", genericReadOnlyCollection.ElementAt(2));

            ReadOnlyObservableCollection<int> genericReadOnlyObservableCollection = CollectionHelper.FromArray(typeof(ReadOnlyObservableCollection<int>), integerArray) as ReadOnlyObservableCollection<int>;
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
