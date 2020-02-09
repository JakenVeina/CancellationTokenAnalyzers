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
        Task MyAsyncOperation(
            CancellationToken cancellationToken = default);
    }

    public static class MyInterfaceExtensions
    {
        public static Task MyExtensionOperation(
                this IMyInterface dependency,
                CancellationToken cancellationToken = default)
            => dependency.MyAsyncOperation(cancellationToken);
    }

    public class TypeName
    {
        public async Task MyMethod(
            IMyInterface dependency,
            CancellationToken cancellationToken)
        {
            await dependency.MyExtensionOperation();
            await dependency.MyExtensionOperation(cancellationToken);
        }
    }
}
