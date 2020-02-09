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
            IMyInterface dependency)
        {
            await dependency.MyAsyncOperation(_cancellationToken);
            await dependency.MyAsyncOperation(_cancellationToken);
        }

        private readonly CancellationToken _cancellationToken
            = CancellationToken.None;
    }
}
