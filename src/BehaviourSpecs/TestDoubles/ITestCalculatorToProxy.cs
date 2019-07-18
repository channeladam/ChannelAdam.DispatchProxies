namespace BehaviourSpecs.TestDoubles
{
    public interface ITestCalculatorToProxy
    {
        int AddFunction(int x, int y);
        int AddFunctionWithAllParams(int x, int y, ref int zAndRefResult, out int outResult);
        void AddMethodWithRefParams(int x, int y, ref int refResult);
    }
}