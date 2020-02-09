using System;
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

        void MySyncOperation(
            string value);
    }

    public static class MyInterfaceExtensions
    {
        public static Task MyAsyncOperation(
            this IMyInterface myInterface,
            string value)
        {
            myInterface.MySyncOperation(value);
            return Task.CompletedTask;
        }
    }

    public class TypeName
    {
        public async Task MyMethod(
                IMyInterface dependency,
                CancellationToken cancellationToken)
        {
            await dependency.MyAsyncOperation();
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }
}
