using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class Extensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
    {
        return collection == null || !collection.Any();
    }
    
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = RNG.RandiRange(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}