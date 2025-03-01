using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CollectionExtension
{
    public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> source, int count)
    {
        if (source == null)
            throw new System.ArgumentNullException(nameof(source));

        if (count < 0)
            throw new System.ArgumentOutOfRangeException(nameof(count), "Number of elements should be positive.");

        var sourceList = source.ToList();
        var actualCount = Mathf.Min(count, sourceList.Count);

        return sourceList.OrderBy(x => Random.value).Take(actualCount);
    }

    public static T GetRandom<T>(this IEnumerable<T> collection)
    {
        var randomElementNumber = Random.Range(0, collection.Count());
        return collection.ElementAt(randomElementNumber);
    }

    public static T GetRandomOrDefault<T>(this IEnumerable<T> collection)
    {
        if (!collection.Any())
        {
            return default(T);
        }

        var randomElementNumber = Random.Range(0, collection.Count());
        return collection.ElementAt(randomElementNumber);
    }
}

