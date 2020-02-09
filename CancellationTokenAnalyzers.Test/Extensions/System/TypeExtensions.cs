using System.Threading.Tasks;
using System.IO;

namespace System
{
    public static class TypeExtensions
    {
        public static Task<string> GetNamespaceResourceTextAsync(
            this Type type,
            string resourceName)
        {
            using var stream = type.Assembly.GetManifestResourceStream(type, resourceName)!;
            
            return new StreamReader(stream).ReadToEndAsync();
        }
    }
}
