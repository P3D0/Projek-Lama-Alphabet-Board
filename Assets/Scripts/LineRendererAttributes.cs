using UnityEngine;
using System;
using System.Collections.Generic;

public class LineRendererAttributes : MonoBehaviour
{
    int numberOfPoints;
    List<Vector3> points;
    public Material material;
	public Color[] colors;

    private void Start()
    {
        if (material == null)
        {
            material = new Material(Shader.Find("Sprites/Default"));
            material.color = Color.red;
        }
        GetComponent<LineRenderer>().material = material;
        points = new List<Vector3>();
    }

    public void SetRandomColor()
    {
        if (colors == null)
        {
            return;
        }
        if (material == null)
        {
            return;
        }
        material.color = colors[UnityEngine.Random.Range(0, colors.Length)];
    }

    public int NumberOfPoints
    {
        get
        {
            return numberOfPoints;
        }
        set
        {
            numberOfPoints = value;
        }
    }

    public List<Vector3> Points
    {
        get
        {
            return points;
        }
        set
        {
            points = value;
        }
    }
}
