#nullable enable

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
            int value1,
            int? value2 = default,
            CancellationToken cancellationToken = default,
            string? value3 = default);
    }

    public class TypeName
    {
        public async Task MyMethod(
            IMyInterface dependency,
            int value1,
            int value2,
            string value3,
            CancellationToken cancellationToken)
        {
            await dependency.MyAsyncOperation(value1, cancellationToken: cancellationToken);
            await dependency.MyAsyncOperation(value1, cancellationToken: cancellationToken);
        }
    }
}
