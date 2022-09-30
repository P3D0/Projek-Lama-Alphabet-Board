using UnityEngine;
using System.Collections.Generic;
using System;

public class SmoothCurve : MonoBehaviour
{
    public static Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve, float smoothness)
    {
        if (smoothness < 1f)
        {
            smoothness = 1f;
        }
        int num = arrayToCurve.Length;
        int num2 = num * Mathf.RoundToInt(smoothness) - 1;
        List<Vector3> list = new List<Vector3>(num2);
        for (int i = 0; i < num2 + 1; i++)
        {
            float num3 = Mathf.InverseLerp(0f, num2, i);
            List<Vector3> list2 = new List<Vector3>(arrayToCurve);
            for (int j = num - 1; j > 0; j--)
            {
                for (int k = 0; k < j; k++)
                {
                    list2[k] = (1f - num3) * list2[k] + num3 * list2[k + 1];
                }
            }
            list.Add(list2[0]);
        }
        return list.ToArray();
    }
}
