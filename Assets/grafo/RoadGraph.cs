using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSegment
{
    public Vector3 Point1 { get; }
    public Vector3 Point2 { get; }
    public LineSegment(Vector3 p1, Vector3 p2)
    {
        Point1 = p1;
        Point2 = p2;
    }

    public override bool Equals(object obj)
    {
        if (obj is LineSegment)
        {
            return ((LineSegment)obj).Point1.Equals(this.Point1) && ((LineSegment)obj).Point2.Equals(this.Point2);
        }
        else return false;
    }

    public override int GetHashCode()
    {
        return Point1.GetHashCode() * Point2.GetHashCode() * 91;
    }
}

