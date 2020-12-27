using System.Text;

namespace BehaviourSpecs.TestDoubles
{
    public class Spy
    {
        public StringBuilder Log { get; } = new StringBuilder();
        public bool IsDisposeCalled { get; set; } = false;
    }
}