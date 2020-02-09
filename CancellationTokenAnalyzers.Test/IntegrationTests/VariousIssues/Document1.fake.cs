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
        Task MyAsyncOperation1(
            CancellationToken cancellationToken = default);

        Task MyAsyncOperation2();

        Task MyAsyncOperation2(
            CancellationToken cancellationToken);

        Task MyAsyncOperation3();
    }

    public static class MyInterfaceExtensions
    {
        public static Task MyAsyncOperation3(
                this IMyInterface myInterface,
                CancellationToken cancellationToken)
            => myInterface.MyAsyncOperation2(cancellationToken);
    }

    public class TypeName
    {
        public async Task MyMethod1(
            IMyInterface dependency,
            CancellationToken cancellationToken)
        {
            await dependency.MyAsyncOperation1();
            await dependency.MyAsyncOperation1(cancellationToken);
        }

        public async Task MyMethod2(
            IMyInterface dependency,
            CancellationToken cancellationToken)
        {
            await dependency.MyAsyncOperation2();
            await dependency.MyAsyncOperation2(cancellationToken);
        }

        public async Task MyMethod3(
            IMyInterface dependency,
            CancellationToken cancellationToken)
        {
            await dependency.MyAsyncOperation3();
            await dependency.MyAsyncOperation3(cancellationToken);
        }
    }
}
