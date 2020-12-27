using ChannelAdam.Disposing.Abstractions;

namespace BehaviourSpecs.TestDoubles
{
    /// <summary>
    /// Test class that is disposable with a destructor.
    /// </summary>
    /// <remarks>
    /// The base class DisposableWithDestructor has the destructor that is called by the garbage collector.
    /// </remarks>
    public class DisposableTestCalculatorToProxyWithDestructor : DisposableWithDestructor, ITestCalculatorToProxy
    {
        private readonly Spy _spy;

        public DisposableTestCalculatorToProxyWithDestructor(Spy spy)
        {
            _spy = spy ?? throw new System.ArgumentNullException(nameof(spy));
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _spy.IsDisposeCalled = true;
            _spy.Log.AppendLine("########## OnDisposing(bool isDisposing) #############");

            base.OnDisposing(isDisposing);
        }

        public int AddFunction(int x, int y)
        {
            return x + y;
        }

        public int AddFunctionWithAllParams(int x, int y, ref int zAndRefResult, out int outResult)
        {
            outResult = x + y;
            zAndRefResult = x + y + zAndRefResult;
            return outResult;
        }

        public void AddMethodWithAllParams(int x, int y, ref int zAndRefResult, out int outResult)
        {
            outResult = x + y;
            zAndRefResult = x + y + zAndRefResult;
        }
    }
}
