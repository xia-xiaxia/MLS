using UnityEngine;

public  static class VectorExtensions
{
    public static Vector2 ToVector2(this Vector4 vec4)
    {
        return new Vector2(vec4.x, vec4.y);
    }
 
    
}
