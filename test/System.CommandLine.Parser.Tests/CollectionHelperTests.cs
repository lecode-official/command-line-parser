
#region Using Directives

using System.Collections;
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

            ArrayList arrayList = new ArrayList { "foo", "bar" };
            Assert.Equal(2, CollectionHelper.GetCount(arrayList));

            Queue queue = new Queue();
            queue.Enqueue(1.0);
            queue.Enqueue(2.0);
            queue.Enqueue(3.0);
            Assert.Equal(3, CollectionHelper.GetCount(queue));
        }

        #endregion
    }
}