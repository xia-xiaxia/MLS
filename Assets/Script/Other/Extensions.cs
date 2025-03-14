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
