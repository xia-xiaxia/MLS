using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
    }
}
public class Pair<T1, T2>
{
    public T1 first { get; set; }
    public T2 second { get; set; }

    public Pair()
    {
        first = default;
        second = default;
    }
    public Pair(T1 first, T2 second)
    {
        this.first = first;
        this.second = second;
    }

    public override string ToString()
    {
        return $"({first}, {second})";
    }
}

public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    public static List<T> GetRandomElements<T>(this List<T> list, int n)
    {
        if (n > list.Count)
            Debug.LogError("n cannot be greater than the number of elements in the list");

        List<T> result = new List<T>(list);
        int count = result.Count;
        for (int i = 0; i < n; i++)
        {
            int k = rng.Next(i, count);
            T temp = result[i];
            result[i] = result[k];
            result[k] = temp;
        }

        return result.GetRange(0, n);
    }
}

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = (T)(object)this;
    }
}

