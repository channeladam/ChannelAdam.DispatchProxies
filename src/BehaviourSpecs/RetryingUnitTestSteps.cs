using System;
using ChannelAdam.DispatchProxies;
using BehaviourSpecs.TestDoubles;
using TechTalk.SpecFlow;
using Moq;
using ChannelAdam.TestFramework.MSTestV2.Abstractions;

namespace BehaviourSpecs
{
    [Binding]
    [Scope(Feature = "Retrying")]
    public class RetryingUnitTestSteps : MoqTestFixture
    {
        private const int RetryNone = 0;
        private const int RetryOnce = 1;

        private int retriesCount;
        private Mock<IFakeService> mockFakeService;
        private IFakeService retryEnabledProxyOfFakeService;

        #region Setup / Teardown

        [BeforeScenario]
        public void Setup()
        {
            this.mockFakeService = base.MyMockRepository.Create<IFakeService>();
        }

        [AfterScenario]
        public void CleanUp()
        {
            Logger.Log("About to verify mock objects");
            base.MyMockRepository.Verify();

            Logger.Log("Disposing disposible proxy");
            ((IDisposable)retryEnabledProxyOfFakeService)?.Dispose();
        }

        #endregion

        #region Given

        [Given(@"a retry enabled disposable object real proxy for an action that will always fail - with no retry policy specified")]
        public void GivenARetryEnabledDisposableObjectRealProxyForAnActionThatWillAlwaysFail_WithNoRetryPolicySpecified()
        {
            CreateMockThatAlwaysThrowsException();
            CreateRetryProxyWithNoRetryPolicy();
        }

        [Given(@"a retry enabled disposable object real proxy for an action that will always fail - with no retries configured")]
        public void GivenARetryEnabledDisposableObjectRealProxyForAnActionThatWillAlwaysFail_WithNoRetriesConfigured()
        {
            CreateMockThatAlwaysThrowsException();
            CreateRetryProxyWithRetriesCount(RetryNone);
        }

        [Given(@"a retry enabled disposable object real proxy for an action that will always fail - with one retry configured")]
        public void GivenARetryEnabledDisposableObjectRealProxyForAnActionThatWillAlwaysFail_WithOneRetryConfigured()
        {
            CreateMockThatAlwaysThrowsException();
            CreateRetryProxyWithRetriesCount(RetryOnce);
        }

        #endregion

        #region When

        [When(@"the proxy is executed for an action that fails")]
        public void WhenTheProxyIsExecutedForAnActionThatFails()
        {
            base.Try(() => this.retryEnabledProxyOfFakeService.DoIt());
        }

        #endregion

        #region Then

        [Then(@"the action was executed without retries")]
        [Then(@"the retry policy was invoked and the retries happened as expected")]
        public void ThenTheRetryPolicyWasInvokedAndTheRetriesHappenedAsExpected()
        {
            base.AssertExpectedException();

            int count = this.retriesCount + 1;
            Logger.Log($"Verifying mock was called {count} times");
            this.mockFakeService.Verify(m => m.DoIt(), Times.Exactly(count));
        }

        #endregion

        #region Private Methods

        private void CreateMockThatAlwaysThrowsException()
        {
            const string exceptionText = "This is always thrown";

            this.ExpectedException.MessageShouldContainText = exceptionText;

            this.mockFakeService.Setup(m => m.DoIt())
                .Throws(new Exception(exceptionText))
                .Verifiable();
        }

        private void CreateRetryProxyWithNoRetryPolicy()
        {
            this.retriesCount = RetryNone;

            this.retryEnabledProxyOfFakeService = RetryEnabledObjectDisposableDispatchProxy.Create<IFakeService>(this.mockFakeService.Object, null);
        }

        private void CreateRetryProxyWithRetriesCount(int retryCount)
        {
            this.retriesCount = retryCount;

            this.retryEnabledProxyOfFakeService = RetryEnabledObjectDisposableDispatchProxy.Create<IFakeService>(this.mockFakeService.Object, new SimpleRetryPolicyFunction(this.retriesCount));
        }

        #endregion
    }
}
