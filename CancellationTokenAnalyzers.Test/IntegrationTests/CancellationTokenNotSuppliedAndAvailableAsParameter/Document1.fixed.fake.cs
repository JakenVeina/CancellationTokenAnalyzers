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

    public class TypeName
    {
        public async Task MyMethod(
            IMyInterface dependency,
            CancellationToken cancellationToken)
        {
            await dependency.MyAsyncOperation(cancellationToken);
            await dependency.MyAsyncOperation(cancellationToken);
        }
    }
}
