#nullable disable

using System;
using BehaviourSpecs.TestDoubles;
using ChannelAdam.DispatchProxies;
using ChannelAdam.TestFramework.NUnit.Abstractions;
using TechTalk.SpecFlow;

namespace BehaviourSpecs
{
    [Binding]
    [Scope(Feature = "Disposable Object Dispatch Proxy")]
    public class DisposableObjectDispatchProxyUnitSteps : MoqTestFixture
    {
        #region Private Fields

        private readonly ScenarioContext _scenarioContext;
        private int _actualOutValue;
        private int _actualReturnedValue;
        private int _actualZAndRefResult;
        private int? _expectedOutValue;
        private int? _expectedReturnedValue;
        private int? _expectedZAndRefResult;
        private DisposableTestCalculatorToProxy _disposableTestCalculatorToProxy;
        private ITestCalculatorToProxy _calculatorProxy;
        private int _x;
        private int _y;

        #endregion Private Fields

        public DisposableObjectDispatchProxyUnitSteps(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        #region Public Methods

        [BeforeScenario]
        public void BeforeScenario()
        {
            Logger.Log("---------------------------------------------------------------------------");
            Logger.Log(_scenarioContext.ScenarioInfo.Title);
            Logger.Log("---------------------------------------------------------------------------");

            _disposableTestCalculatorToProxy = new DisposableTestCalculatorToProxy();
            _calculatorProxy = DispatchProxyFactory.CreateDisposableObjectDispatchProxy<ITestCalculatorToProxy>(_disposableTestCalculatorToProxy, new TestObjectInvokeHandler());

            _x = 1;
            _y = 2;
            _actualZAndRefResult = 5;
            Logger.Log($"INPUTS: x={_x} y={_y} z={_actualZAndRefResult}");
        }

        #endregion Public Methods

        #region Given

        [Given("a proxied function with a return value")]
        public void GivenAProxiedFunctionWithAReturnValue()
        {
            _expectedReturnedValue = _x + _y;
            _expectedOutValue = null;
            _expectedZAndRefResult = null;
            Logger.Log($"EXPECTED: return value={_expectedReturnedValue}");
        }

        [Given("a proxied function with all parameters")]
        public void GivenAProxiedFunctionWithAllParameters()
        {
            _expectedReturnedValue = _expectedOutValue = _x + _y;
            _expectedZAndRefResult = _x + _y + _actualZAndRefResult;
            Logger.Log($"EXPECTED: return value={_expectedReturnedValue}, out value={_expectedOutValue}, ref result={_expectedZAndRefResult}");
        }

        [Given("a proxied method with all parameters")]
        public void GivenAProxiedMethodWithAllParameters()
        {
            _expectedReturnedValue = null;
            _expectedOutValue = _x + _y;
            _expectedZAndRefResult = _x + _y + _actualZAndRefResult;
            Logger.Log($"EXPECTED: out value={_expectedOutValue}, ref result={_expectedZAndRefResult}");
        }

        #endregion Given

        #region When

        [When("the proxied function with a return value is called")]
        public void WhenTheProxiedFunctionWithAReturnValueIsCalled()
        {
            _actualReturnedValue = _calculatorProxy.AddFunction(_x, _y);
            Logger.Log($"ACTUAL: return value={_actualReturnedValue}");
        }

        [When("the proxied function with all parameters is called")]
        public void WhenTheProxiedFunctionWithAllParametersIsCalled()
        {
            _actualReturnedValue = _calculatorProxy.AddFunctionWithAllParams(_x, _y, ref _actualZAndRefResult, out _actualOutValue);
            Logger.Log($"ACTUAL: return value={_actualReturnedValue}, ref result={_actualZAndRefResult}, out value={_actualOutValue}");
        }

        [When("the proxied method with all parameters is called")]
        public void WhenTheProxiedMethodWithRefParametersIsCalled()
        {
            _calculatorProxy.AddMethodWithAllParams(_x, _y, ref _actualZAndRefResult, out _actualOutValue);
            Logger.Log($"ACTUAL: ref result={_actualZAndRefResult}, out value={_actualOutValue}");
        }

        [When("the dispatch proxy is disposed")]
        public void WhenTheDispatchProxyIsDisposed()
        {
            IDisposable disposableProxy = _calculatorProxy as IDisposable;
            LogAssert.IsNotNull("Proxy is IDisposable", disposableProxy);
            LogAssert.IsFalse("Proxy is not yet disposed", ((ChannelAdam.DispatchProxies.Abstractions.Internal.CoreDisposableObjectDispatchProxy)_calculatorProxy).IsDisposed);
            LogAssert.IsFalse("Proxied object is not yet disposed", _disposableTestCalculatorToProxy.IsDisposed);

            disposableProxy.Dispose();
        }

        #endregion When

        #region Then

        [Then("the value returned from the proxied function with a return value has the correct return value")]
        [Then("the return, out and ref parameters from the proxied function have the correct values")]
        [Then("the out and ref parameters from the proxied method have the correct values")]
        public void ThenAssertResultingValuesFromTheProxiedFunctionOrMethod()
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

        [Then("the proxied object is disposed")]
        public void ThenTheProxiedObjectIsDisposed()
        {
            LogAssert.IsTrue("Proxy is disposed", ((ChannelAdam.DispatchProxies.Abstractions.Internal.CoreDisposableObjectDispatchProxy)_calculatorProxy).IsDisposed);
            LogAssert.IsTrue("Proxied object is disposed", _disposableTestCalculatorToProxy.IsDisposed);
        }

        #endregion Then
    }
}