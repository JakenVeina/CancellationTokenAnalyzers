using System.Threading;
using System.Threading.Tasks;

namespace FakeSolution
{
    public static class Program
    {
        public static void Main() { }
    }

    public interface IMyInterface
    {
        Task MyAsyncOperation();

        Task MyOtherAsyncOperation(
            CancellationToken cancellationToken);
    }

    public static class MyInterfaceExtensions
    {
        public static Task MyAsyncOperation(
                this IMyInterface myInterface,
                CancellationToken cancellationToken)
            => myInterface.MyOtherAsyncOperation(cancellationToken);
    }

    public class TypeName
    {
        public async Task MyMethod(
                IMyInterface dependency)
            => await dependency.MyAsyncOperation();
    }
}
