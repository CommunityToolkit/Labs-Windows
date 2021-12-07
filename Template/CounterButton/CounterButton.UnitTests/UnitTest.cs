using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace CounterButton.UnitTests
{
    [TestClass]
    public class CounterButtonTests
    {
        [UITestMethod]
        public void CountTest()
        {
            // Default Count value is 0
            var cb = new Library.CounterButton();
            Assert.AreEqual(0, cb.Count);

            // Increment using the function
            cb.Increment();

            // Increment using the Count property
            cb.Count++;
            Assert.AreEqual(1, cb.Count);
        }
    }
}
