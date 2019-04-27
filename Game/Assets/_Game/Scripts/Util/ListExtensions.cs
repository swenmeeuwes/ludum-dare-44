using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListExtensions {
    public static void Push<T>(this List<T> list, T item)
    {
        list.Add(item);
    }

    public static T Pop<T>(this List<T> list)
    {
        if (list.Count == 0)
        {
            Debug.LogError("Cannot pop an empty collection");
            return default(T);
        }

        var lastEntry = list.Last(); // list[list.Count - 1] and removeAt might be faster?
        list.Remove(lastEntry);

        return lastEntry;
    }
}
