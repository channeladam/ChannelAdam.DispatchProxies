#nullable disable

using System;
using BehaviourSpecs.TestDoubles;
using ChannelAdam.DispatchProxies;
using ChannelAdam.TestFramework.NUnit.Abstractions;
using TechTalk.SpecFlow;

namespace BehaviourSpecs
{
    [Binding]
    [Scope(Feature = "Disposable Object Dispatch Proxy With Destructor")]
    public class DisposableObjectDispatchProxyWithDestructorUnitSteps : MoqTestFixture
    {
        #region Private Fields

        private readonly ScenarioContext _scenarioContext;
        private int _actualOutValue;
        private int _actualReturnedValue;
        private int _actualZAndRefResult;
        private int? _expectedOutValue;
        private int? _expectedReturnedValue;
        private int? _expectedZAndRefResult;
        private Spy _spy;
        private DisposableTestCalculatorToProxyWithDestructor _proxiedObject;
        private ITestCalculatorToProxy _calculatorProxy;
        private int _x;
        private int _y;

        #endregion Private Fields

        public DisposableObjectDispatchProxyWithDestructorUnitSteps(ScenarioContext context)
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

            _spy = new Spy();
            _proxiedObject = new DisposableTestCalculatorToProxyWithDestructor(_spy);
            _calculatorProxy = DispatchProxyFactory.CreateDisposableObjectDispatchProxyWithDestructor<ITestCalculatorToProxy>(_proxiedObject, new TestObjectInvokeHandler());

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

        [Given("the dispatch proxy is created so it can be finalised")]
        public void GivenTheDispatchProxyIsCreatedSoItCanBeFinalised()
        {
            _calculatorProxy = null;
            _proxiedObject = null;

            _spy = new Spy();

            // Do NOT store the proxied object and dispatch proxy as a member variable - so when this method finishes they are eligible for garbage collection.
            var proxiedObject = new DisposableTestCalculatorToProxyWithDestructor(_spy);
            DispatchProxyFactory.CreateDisposableObjectDispatchProxyWithDestructor<ITestCalculatorToProxy>(proxiedObject, new TestObjectInvokeHandler());

            LogAssert.IsFalse("Proxied object's Dispose() method has been called", _spy.IsDisposeCalled);
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

            LogAssert.IsFalse("Proxied object is not yet disposed", _proxiedObject.IsDisposed);

            disposableProxy.Dispose();
        }

        [When("the dispatch proxy is finalised")]
        public static void WhenTheDispatchProxyIsFinalised()
        {
            // The weak references were not stored as member variables and should be collectable.
            // Force garbage collection.
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking:true, compacting:true);
            GC.WaitForPendingFinalizers();
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
            if (_calculatorProxy is not null)
            {
                LogAssert.IsTrue("Proxy is disposed", ((ChannelAdam.DispatchProxies.Abstractions.Internal.CoreDisposableObjectDispatchProxy)_calculatorProxy).IsDisposed);
            }

            if (_proxiedObject is not null)
            {
                LogAssert.IsTrue("Proxied object is disposed", _proxiedObject.IsDisposed);
            }

            Logger.Log($"Log from the spy:{Environment.NewLine}{_spy.Log}");
            LogAssert.IsTrue("Proxied object's Dispose() method has been called", _spy.IsDisposeCalled);
        }

        #endregion Then
    }
}