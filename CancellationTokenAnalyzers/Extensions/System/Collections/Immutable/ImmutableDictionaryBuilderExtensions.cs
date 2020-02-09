namespace System.Collections.Immutable
{
    public static class ImmutableDictionaryBuilderExtensions
    {
        public static ImmutableDictionary<TKey, TValue>.Builder With<TKey, TValue>(
            this ImmutableDictionary<TKey, TValue>.Builder builder,
            TKey key,
            TValue value)
        {
            builder.Add(key, value);

            return builder;
        }
    }
}
