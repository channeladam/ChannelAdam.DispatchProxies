using BehaviourSpecs.TestDoubles;
using ChannelAdam.TestFramework.MSTestV2.Abstractions;
using TechTalk.SpecFlow;
using ChannelAdam.DispatchProxies.Abstractions;

namespace BehaviourSpecs
{
    [Binding]
    [Scope(Feature = "Dispatch Proxy")]
    public class DispatchProxyUnitSteps : MoqTestFixture
    {
        #region Private Fields

        private int _actualOutValue;
        private int _actualReturnedValue;
        private int _actualZAndRefResult;
        private int? _expectedOutValue;
        private int? _expectedReturnedValue;
        private int? _expectedZAndRefResult;
        private ITestCalculatorToProxy _calculatorProxy;
        private int _x;
        private int _y;

        #endregion Private Fields

        #region Public Methods

        [BeforeScenario]
        public void BeforeScenario()
        {
            _calculatorProxy = TestObjectDispatchProxy.Create<ITestCalculatorToProxy>(new TestCalculatorToProxy());

            _x = 1;
            _y = 2;
            _actualZAndRefResult = 5;
            Logger.Log($"INPUTS: x={_x} y={_y} z={_actualZAndRefResult}");
        }

        #endregion Public Methods

        #region Given

        [Given(@"a proxied function with a return value")]
        public void GivenAProxiedFunctionWithAReturnValue()
        {
            _expectedReturnedValue = _x + _y;
            _expectedOutValue = null;
            _expectedZAndRefResult = null;
            Logger.Log($"EXPECTED: return value={_expectedReturnedValue}");
        }

        [Given(@"a proxied function with in, out and ref parameters")]
        public void GivenAProxiedFunctionWithInOutAndRefParameters()
        {
            _expectedReturnedValue = _expectedOutValue = _x + _y;
            _expectedZAndRefResult = _x + _y + _actualZAndRefResult;
            Logger.Log($"EXPECTED: return value={_expectedReturnedValue}, out value={_expectedOutValue}, ref result={_expectedZAndRefResult}");
        }

        [Given(@"a proxied method with ref parameters")]
        public void GivenAProxiedMethodWithRefParameters()
        {
            _expectedReturnedValue = null;
            _expectedOutValue = null;
            _expectedZAndRefResult = _x + _y + _actualZAndRefResult;
            Logger.Log($"EXPECTED: ref result={_expectedZAndRefResult}");
        }

        #endregion Given

        #region When

        [When(@"the proxied function with a return value is called")]
        public void WhenTheProxiedFunctionWithAReturnValueIsCalled()
        {
            _actualReturnedValue = _calculatorProxy.AddFunction(_x, _y);
            Logger.Log($"ACTUAL: return value={_actualReturnedValue}");
        }

        [When(@"the proxied function with in, out and ref parameters is called")]
        public void WhenTheProxiedFunctionWithInOutAndRefParametersIsCalled()
        {
            _actualReturnedValue = _calculatorProxy.AddFunctionWithAllParams(_x, _y, ref _actualZAndRefResult, out _actualOutValue);
            Logger.Log($"ACTUAL: return value={_actualReturnedValue}, out value={_actualOutValue}, ref result={_actualZAndRefResult}");
        }

        [When(@"the proxied method with ref parameters is called")]
        public void WhenTheProxiedMethodWithRefParametersIsCalled()
        {
            _calculatorProxy.AddMethodWithRefParams(_x, _y, ref _actualZAndRefResult);
            Logger.Log($"ACTUAL: ref result={_actualZAndRefResult}");
        }

        #endregion When

        #region Then

        [Then(@"the value returned from the proxied function with a return value has the correct return value")]
        [Then(@"the ref parameters from the proxied method have the correct values")]
        [Then(@"the return value, out and ref parameters from the proxied function have the correct values")]
        public void ThenAssertResultingValuesFromTheProxiedFunction()
        {
            if (_expectedReturnedValue.HasValue)
            {
                LogAssert.AreEqual("return value", _expectedReturnedValue, _actualReturnedValue);
            }

            if (_expectedOutValue.HasValue)
            {
                LogAssert.AreEqual("out value", _expectedOutValue, _actualOutValue);
            }

            if (_expectedZAndRefResult.HasValue)
            {
                LogAssert.AreEqual("ref result", _expectedZAndRefResult, _actualZAndRefResult);
            }
        }

        #endregion Then
    }
}