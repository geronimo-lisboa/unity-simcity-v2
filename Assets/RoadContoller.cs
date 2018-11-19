using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadContoller : MonoBehaviour
{
    private List<Vector3> controlPoints = new List<Vector3>();
    private bool isDataModified = false;
    private List<Vector3> geometry = new List<Vector3>();
    private List<int> indexes = new List<int>();
    private List<Vector3> testePointPositions = new List<Vector3>();
    private Mesh mesh;

    public float distanceFromGround = 0.2f;
    public float width = 0.5f;

    public void AddPoint(Vector3 v)
    {
        controlPoints.Add(v);
        isDataModified = true;
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
        Debug.Log("Rebuilding track");
        testePointPositions.Clear();
        geometry.Clear();
        indexes.Clear();
        //Para cada segmento, na ordem em que foram informados, faça
        for (var i = 0; i < controlPoints.Count - 1; i++)
        {
            ///1) Calculate the segment's direction and what is left and right.
            var directionVector = (controlPoints[i + 1] - controlPoints[i]).normalized;
            var sideVector = Vector3.Cross(Vector3.up, directionVector);
            var antiSideVector = sideVector * -1.0f;
            ///2) Calculate where, in world coordinates, are the left and right sides
            var g0 = controlPoints[i] + sideVector * width;
            var g1 = controlPoints[i] + antiSideVector * width;
            var g2 = controlPoints[i + 1] + sideVector * width;
            var g3 = controlPoints[i + 1] + antiSideVector * width;
            ///2.1) Saves them in the list that is read by the gizmo renderer.
            testePointPositions.Add(g0);
            testePointPositions.Add(g1);
            testePointPositions.Add(g2);
            testePointPositions.Add(g3);
            ///3) Calculates where points will be.
            var p0 = g0 + Vector3.up * distanceFromGround;
            var p1 = g1 + Vector3.up * distanceFromGround;
            var p2 = g2 + Vector3.up * distanceFromGround;
            var p3 = g3 + Vector3.up * distanceFromGround;
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
        }
        ///Put the mesh data in the filters.
        mesh.Clear();
        mesh.SetVertices(geometry);
        mesh.SetIndices(indexes.ToArray(), MeshTopology.Triangles, 0);
        isDataModified = false;
    }

    private List<Vector3> testeGeo = new List<Vector3>();
    private List<int> testeIndex = new List<int>();
    void Update()
    {
        if (isDataModified == true)
        {
            //TODO: if the old list of control points is equal to the current list, do nothing.
            BuildGeometryFromPoints();
        }
    }
    /// <summary>
    /// Draws some gizmos so that I can know where am I marking the points.
    /// </summary>
    private void OnDrawGizmos()
    {
        foreach (var p in controlPoints)
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
