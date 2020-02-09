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

        Task MyAsyncOperation(
            string value);

        Task MyAsyncOperation(
            CancellationToken cancellationToken);

        Task MyAsyncOperation(
            string value,
            CancellationToken cancellationToken);
    }

    public class TypeName
    {
        public async Task MyMethod(
            IMyInterface dependency,
            CancellationToken cancellationToken)
        {
            await dependency.MyAsyncOperation(cancellationToken);
            await dependency.MyAsyncOperation(cancellationToken);
        }

        public async Task MyMethod(
            IMyInterface dependency,
            string value,
            CancellationToken cancellationToken)
        {
            await dependency.MyAsyncOperation(value, cancellationToken);
            await dependency.MyAsyncOperation(value, cancellationToken);
        }
    }
}
