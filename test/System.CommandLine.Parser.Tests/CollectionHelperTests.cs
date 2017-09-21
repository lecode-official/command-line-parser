
#region Using Directives

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.CommandLine.ValueConverters;
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

        #endregion
    }
}