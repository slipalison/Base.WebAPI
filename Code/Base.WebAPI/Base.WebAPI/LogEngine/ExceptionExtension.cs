using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.WebAPI.LogEngine
{
    internal static class ExceptionExtension
    {
        public static IEnumerable<TSource> FromHierarchy<TSource>(this TSource source, Func<TSource, TSource> nextItem) where TSource : class
             => FromHierarchy(source, nextItem, s => s != null);
        public static string GetaAllMessages(this Exception exception)
        => string.Join(Environment.NewLine, exception.FromHierarchy(ex => ex.InnerException).Select(ex => ex.Message));
        private static IEnumerable<TSource> FromHierarchy<TSource>(this TSource source, Func<TSource, TSource> nextItem, Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue?.Invoke(current) ?? default(bool); current = nextItem(current))
                yield return current;
        }

    }

}
