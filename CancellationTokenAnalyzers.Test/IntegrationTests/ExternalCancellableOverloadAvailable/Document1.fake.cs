using System;
using System.Threading;
using System.Threading.Tasks;

namespace FakeSolution
{
    public static class Program
    {
        public static void Main() { }
    }

    public class TypeName
    {
        public async Task MyMethod(
            CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }
}
