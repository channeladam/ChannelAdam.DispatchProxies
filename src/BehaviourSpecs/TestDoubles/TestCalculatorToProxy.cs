﻿namespace BehaviourSpecs.TestDoubles
{
    public class TestCalculatorToProxy : ITestCalculatorToProxy
    {
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
