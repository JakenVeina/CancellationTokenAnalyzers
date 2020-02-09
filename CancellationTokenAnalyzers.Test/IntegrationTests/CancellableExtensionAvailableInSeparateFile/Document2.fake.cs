using System.Threading;
using System.Threading.Tasks;

namespace FakeSolution
{
    public static class MyInterfaceExtensions
    {
        public static Task MyAsyncOperation(
                this IMyInterface myInterface,
                CancellationToken cancellationToken)
            => myInterface.MyOtherAsyncOperation(cancellationToken);
    }
}
