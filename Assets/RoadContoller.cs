using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadContoller : MonoBehaviour
{
    private List<Vector3> points = new List<Vector3>();
    private List<Vector3> geometry = new List<Vector3>();
    private List<int> indexes = new List<int>();
    private List<Vector3> testePointPositions = new List<Vector3>();

    private Mesh mesh;

    public void AddPoint(Vector3 v)
    {
        points.Add(v);
    }

    void Start()
    {

    }

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    private void BuildGeometryFromPoints()
    {
        testePointPositions.Clear();
        geometry.Clear();
        indexes.Clear();
        //Para cada segmento, na ordem em que foram informados, faça
        for (var i = 0; i < points.Count - 1; i++)
        {
            ///1) Calculate the segment's direction and what is left and right.
            var directionVector = (points[i + 1] - points[i]).normalized;
            var sideVector = Vector3.Cross(Vector3.up, directionVector);
            var antiSideVector = sideVector * -1.0f;
            ///2) Calculate where, in world coordinates, are the left and right sides
            var g0 = points[i] + sideVector * 0.5f;
            var g1 = points[i] + antiSideVector * 0.5f;
            var g2 = points[i + 1] + sideVector * 0.5f;
            var g3 = points[i + 1] + antiSideVector * 0.5f;
            ///2.1) Saves them in the list that is read by the gizmo renderer.
            testePointPositions.Add(g0);
            testePointPositions.Add(g1);
            testePointPositions.Add(g2);
            testePointPositions.Add(g3);
            ///3) Calculates where points will be.
            var p0 = g0 + Vector3.up * 0.5f;
            var p1 = g1 + Vector3.up * 0.5f;
            var p2 = g2 + Vector3.up * 0.5f;
            var p3 = g3 + Vector3.up * 0.5f;
            var index = geometry.Count;
            geometry.Add(p0);
            geometry.Add(p1);
            geometry.Add(p2);
            geometry.Add(p3);
            //set the indexes 
            //0,1,2
            //2 1 3
            indexes.Add(index + 0);
            indexes.Add(index + 1);
            indexes.Add(index + 2);
            indexes.Add(index + 2);
            indexes.Add(index + 1);
            indexes.Add(index + 3);

            //var p0 = g0 + Vector3.up * 0.5f;
            //var p1 = g1 + Vector3.up * 0.5f;
            //var p4 = g2 + Vector3.up * 0.5f;
            //var p6 = g3 + Vector3.up * 0.5f;
            /////4) Assembles the mesh. (0, 1, 2, 3)
            //int index = geometry.Count;

            //geometry.Add(p0);//0
            //geometry.Add(p1);//1
            //geometry.Add(p4);//2
            //geometry.Add(p6);//3

            //indexes.Add(index + 0);
            //indexes.Add(index + 1);
            //indexes.Add(index + 2);
            //indexes.Add(index + 3);
        }
        ///Put the mesh data in the filters.
        mesh.Clear();
        mesh.SetVertices(geometry);
        mesh.SetIndices(indexes.ToArray(), MeshTopology.Triangles, 0);
    }

    private List<Vector3> testeGeo = new List<Vector3>();
    private List<int> testeIndex = new List<int>();
    void Update()
    {
        //TODO: if the old list of control points is equal to the current list, do nothing.
        BuildGeometryFromPoints();
        ///Demo de como montar uma mesh proceduralmente
        //testeGeo.Clear();
        //testeIndex.Clear();
        //testeGeo.Add(new Vector3(0, 0, 0));
        //testeGeo.Add(new Vector3(0.5f, 1.0f, 0));
        //testeGeo.Add(new Vector3(1.0f, 0, 0));
        //testeIndex.Add(0);
        //testeIndex.Add(1);
        //testeIndex.Add(2);
        //mesh.SetVertices(testeGeo);
        //mesh.SetIndices(testeIndex.ToArray(), MeshTopology.Triangles, 0);
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
