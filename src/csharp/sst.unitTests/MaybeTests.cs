using NUnit.Framework;
using QResurgence.SST.Utilities;

namespace QResurgence.SST.UnitTests
{
    public class MaybeTests
    {
        [Test]
        public void NothingTest()
        {
            var nothing = new Nothing<object>();

            var justHandlerExecuted = false;
            var nothingHandlerExecuted = false;

            nothing
                .Just(_ => justHandlerExecuted = true)
                .Nothing(() => nothingHandlerExecuted = true);

            Assert.False(justHandlerExecuted);
            Assert.True(nothingHandlerExecuted);
        }

        [Test]
        public void JustTest()
        {
            var something = new Maybe<object>(new object());

            var justHandlerExecuted = false;
            var nothingHandlerExecuted = false;

            something
                .Just(_ => justHandlerExecuted = true)
                .Nothing(() => nothingHandlerExecuted = true);

            Assert.True(justHandlerExecuted);
            Assert.False(nothingHandlerExecuted);
        }
    }
}