using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadContoller : MonoBehaviour
{
    private List<Vector3> points = new List<Vector3>();

    private List<Vector3> testePointPositions = new List<Vector3>();

    public void AddPoint(Vector3 v)
    {
        points.Add(v);
    }

    void Start()
    {

    }

    private void BuildGeometryFromPoints()
    {
        testePointPositions.Clear();
        //Para cada segmento, na ordem em que foram informados, faça
        for (var i = 0; i < points.Count - 1; i++)
        {
            //I need to calculate the direction of the segment and the left and right
            //vectors of the segment
            var directionVector = (points[i + 1] - points[i]).normalized;
            var sideVector = Vector3.Cross(Vector3.up, directionVector);
            var antiSideVector = sideVector * -1.0f;
            //Now I calculate the points
            var pt0 = points[i] + sideVector * 0.5f;
            var pt1 = points[i] + antiSideVector * 0.5f;
            var pt2 = points[i + 1] + sideVector * 0.5f;
            var pt3 = points[i + 1] + antiSideVector * 0.5f;
            testePointPositions.Add(pt0);
            testePointPositions.Add(pt1);
            testePointPositions.Add(pt2);
            testePointPositions.Add(pt3);

        }
    }

    void Update()
    {
        BuildGeometryFromPoints();
    }
    /// <summary>
    /// Draws some gizmos so that I can know where am I marking the points.
    /// </summary>
    private void OnDrawGizmos()
    {
        foreach (var p in points)
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
            Gizmos.DrawSphere(p, 0.2f);
        }
        foreach (var p in testePointPositions)
        {
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
            Gizmos.DrawWireCube(p, new Vector3(0.2f, 0.2f, 0.2f));
        }
    }
}
