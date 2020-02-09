namespace System
{
    public static class StringExtensions
    {
        public static int ParseInt32(
                this string value)
            => int.Parse(value);
    }
}
