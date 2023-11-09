using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumExtensions
{    public static T GetRandomValue<T>()
    {
        System.Array values = System.Enum.GetValues(typeof(T));
        return (T)values.GetValue(Random.Range(0, values.Length));
    }
}
