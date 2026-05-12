using System.Collections.Generic;
using System.Collections.Specialized;

namespace PlainCEETimer.Modules.Extensions;

public static class CollectionExtensions
{
    public static TNameValueCollection SetEx<TNameValueCollection>(this TNameValueCollection nvc, string name, string value)
        where TNameValueCollection : NameValueCollection
    {
        nvc.Set(name, value);
        return nvc;
    }

    public static TCollection AddEx<TCollection, TValue>(this TCollection collection, TValue value)
        where TCollection : ICollection<TValue>
    {
        return AddEx(collection, value, true);
    }

    public static TCollection AddEx<TCollection, TValue>(this TCollection collection, TValue value, bool condition)
        where TCollection : ICollection<TValue>
    {
        if (condition)
        {
            collection.Add(value);
        }

        return collection;
    }
}