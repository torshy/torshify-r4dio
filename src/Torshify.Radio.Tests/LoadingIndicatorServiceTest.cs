using NUnit.Framework;
using Torshify.Radio.Core.Services;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Tests
{
    [TestFixture]
    public class LoadingIndicatorServiceTest
    {
        private ILoadingIndicatorService _indicatorService;

        [SetUp]
        public void Setup()
        {
            _indicatorService = new LoadingIndicatorService();
        }

        [Test]
        public void PushOnce_IsLoading()
        {
            _indicatorService.Push();

            Assert.IsTrue(_indicatorService.IsLoading);
        }

        [Test]
        public void PushOnce_PopOnce_IsNotLoading()
        {
            _indicatorService.Push();
            _indicatorService.Pop();

            Assert.IsFalse(_indicatorService.IsLoading);
        }

        [Test]
        public void PushMultiple_IsLoading()
        {
            _indicatorService.Push();
            _indicatorService.Push();
            _indicatorService.Push();

            Assert.IsTrue(_indicatorService.IsLoading);
        }

        [Test]
        public void PushPop_Balanced_IsNotLoading()
        {
            _indicatorService.Push();
            _indicatorService.Push();
            _indicatorService.Push();

            _indicatorService.Pop();
            _indicatorService.Pop();
            _indicatorService.Pop();

            Assert.IsFalse(_indicatorService.IsLoading);
        }

        [Test]
        public void EnterLoadingBlock_RevertsState()
        {
            using(_indicatorService.EnterLoadingBlock())
            {
                Assert.IsTrue(_indicatorService.IsLoading);
            }

            Assert.IsFalse(_indicatorService.IsLoading);
        }

        [Test]
        public void NestedEnterLoadingBloack_RevertsState()
        {
            using (_indicatorService.EnterLoadingBlock())
            {
                Assert.IsTrue(_indicatorService.IsLoading);

                using (_indicatorService.EnterLoadingBlock())
                {
                    Assert.IsTrue(_indicatorService.IsLoading);

                    using (_indicatorService.EnterLoadingBlock())
                    {
                        Assert.IsTrue(_indicatorService.IsLoading);
                    }
                }
            }

            Assert.IsFalse(_indicatorService.IsLoading);
        }
    }
}