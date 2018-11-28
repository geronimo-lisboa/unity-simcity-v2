using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line2dIntersectionService
{
    //De https://github.com/setchi/Unity-LineSegmentsIntersection/blob/master/Assets/LineSegmentIntersection/Scripts/Math2d.cs
    public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return true;
    }
}

public class LineSegment2d
{
    private static int Counter = 0;
    public static void ResetCounter()
    {
        Counter = 0;
    }
    public int Id { get; }
    public Vector2 Point1 { get; }
    public Vector2 Point2 { get; }
    public LineSegment OriginalSegment { get; }
    //Esse dado veio originalmente de dois pontos 3d. Eu guardo a magnitude deles pq vou 
    //precisar de ter a relação entre a magnitude deles no 2d e no 3d pra quando eu andar
    //em um poder andar corretamente no outro.
    public float OriginalMagnitude { get; }

    /// <summary>
    ///Descarta a informação de y e fica só com xz,
    ///X->X
    ///Y->-
    ///Z->Y
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    public LineSegment2d(Vector3 p0, Vector3 p1)
    {
        Vector2 ponto1 = new Vector2(p0.x, p0.z);
        Vector2 ponto2 = new Vector2(p1.x, p1.z);
        Point1 = ponto1;
        Point2 = ponto2;
        OriginalMagnitude = (p1 - p0).magnitude;
        Id = Counter;
        Counter++;
        OriginalSegment = new LineSegment(p0, p1);
        
    }
}

public class LineSegment
{
    
    public Vector3 Vector { get; }
    

    public Vector3 Point1 { get; }
    public Vector3 Point2 { get; }
    public LineSegment(Vector3 p1, Vector3 p2)
    {
        Point1 = p1;
        Point2 = p2;
        Vector = p2 - p1;
    }

}

