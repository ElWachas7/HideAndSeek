using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public static class MyRandom 
{
   public static T RouletteWheelSelection<T>(Dictionary<T, float> elements)
    {
        if (elements == null || elements.Count == 0)
        {
            Debug.Log("lista vacia");
            return default;
        }

        float total = 0f;
        foreach (var value in elements.Values)
            total += value;

        float randomValue = Random.Range(0f, total);
        float cumulative = 0f;
        foreach (var elem in elements)
        {
            cumulative += elem.Value;
            if(randomValue <= cumulative)
            {
                return elem.Key;
            }
        }
        return default;
    }
}

