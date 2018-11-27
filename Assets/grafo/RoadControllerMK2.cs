using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using ExtensionMethods;

public class RoadControllerMK2 : MonoBehaviour { 

    public static readonly int LayRoadFirstPoint = 0;
    public static readonly int LayRoadSecondPoint = 1;
    public static readonly int LayRoadNotLaying = 2;
    public Button btnLayRoad;
    public int LayRoadState = LayRoadNotLaying;
    private Vector3 RoadFirstPoint, RoadSecondPoint;
    private List<LineSegment> segments = new List<LineSegment>();
    private List<Vector3> intersections = new List<Vector3>();
    
	// Use this for initialization
	void Start () {
        ///Se não estiver pondo uma rua passa a poder por o 1o ponto
        btnLayRoad.onClick.AddListener(() => {
            if(LayRoadState == LayRoadNotLaying)
            {
                LayRoadState = LayRoadFirstPoint;
            }
        });
	}
    //Guarda o 1o ponto da rua se o usuário já tiver clicado em lay road
    public void StoreFirstRoadPoint(Vector3 pt)
    {
        if (LayRoadState != LayRoadFirstPoint)
            return;
        else
        {
            RoadFirstPoint = pt;
            LayRoadState = LayRoadSecondPoint;
        }   
    }
    //Guarda o 2o ponto se o 1o ponto já tiver sido guardado.
    public void StoreSecondPoint(Vector3 pt)
    {
        if (LayRoadState != LayRoadSecondPoint)
            return;
        else
        {
            RoadSecondPoint = pt;
            LayRoadState = LayRoadNotLaying;
            segments.Add(new LineSegment(RoadFirstPoint, RoadSecondPoint));
            BuildGraph();
        }
    }

    private void BuildGraph()
    {
        intersections.Clear();
        //Acha as interseções
        List<Vector3> intersectionList = FindIntersections(segments);
        intersections = intersectionList;
        //Divide os segmentos nas interseções, criando uma nova lista de segmentos
        SplitSegments(intersectionList, segments);
        
    }

    

    private void SplitSegments(List<Vector3> intersection, List<LineSegment> segments)
    {
        int count = 0;
        foreach(var s in segments)
        {
            //testa as interseções
            foreach(var i in intersection)
            {
                float paramX = (i.x - s.Point1.x) / s.Vector.x;
                float paramY = (i.y - s.Point1.y) / s.Vector.y;
                float paramZ = (i.z - s.Point1.z) / s.Vector.z;
                //todos os parâmetros tem que ser iguais e tem que ser entre 0 e 1
                if(paramX.FComp(paramY) && paramX.FComp(paramZ) && paramX <= 1 && paramX >= 0)
                {
                    count++;
                }
            }

        }
        Debug.Log(count);
    }

    private List<Vector3> FindIntersections(List<LineSegment> segs)
    {
        List<Vector3> intersectionList = new List<Vector3>();
        //Mapeia os pontos pro 2d descartando o y
        LineSegment2d.ResetCounter();
        var _2dSegments = segs.Select<LineSegment, LineSegment2d>(s => new LineSegment2d(s.Point1, s.Point2)).ToList();
        var _2dSegsQueue = new Queue<LineSegment2d>(_2dSegments);
        while(_2dSegsQueue.Count > 0)
        {
            var s1 = _2dSegsQueue.Dequeue();
            foreach(var s2 in _2dSegsQueue)
            {
                Vector2 intersectionIn2d;
                bool doesIntersect = Line2dIntersectionService.LineSegmentsIntersection(s1.Point1, s1.Point2, s2.Point1, s2.Point2, out intersectionIn2d);
                if (doesIntersect)
                {
                    //A posição paramétrica no segmento 1 do ponto de interseção. Poderia ser no segmento 2 mas tanto faz, a posição espacial vai ser a mesma.
                    float gammaP2d = (intersectionIn2d - s1.Point1).magnitude / (s1.Point2 - s1.Point1).magnitude;
                    //Relação entre as magnitudes do vetor2d e do vetor3d que corresponde a ele
                    var R = (s1.Point2 - s1.Point1).magnitude / s1.OriginalMagnitude;//(segments[i].Point2 - segments[i].Point1).magnitude;
                    //o parâmetro no vetor 3d
                    var gammaP3d = gammaP2d * R;
                    //o ponto da interseção no 3d
                    var p3d = s1.OriginalSegment.Point1 + gammaP3d * (s1.OriginalSegment.Point2 - s1.OriginalSegment.Point1);
                    intersectionList.Add(p3d);
                }
            }
        }
        return intersectionList;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach(var s in segments)
        {
            Gizmos.DrawLine(s.Point1, s.Point2);
        }
        Gizmos.color = Color.red;
        foreach(var i in intersections)
        {
            Gizmos.DrawSphere(i, 0.25f);
        }
        
    }

	// Update is called once per frame
	void Update () {
    }

    //private bool IsNotPresentInIntersectionList(Vector3 outPoint1)
    //{
    //    float EPSILON = 0.01f;
    //    foreach(var i in intersections)
    //    {
    //        bool x = (Mathf.Abs(outPoint1.x - i.x)) < EPSILON;
    //        bool y = (Mathf.Abs(outPoint1.y - i.y)) < EPSILON;
    //        bool z = (Mathf.Abs(outPoint1.z - i.z)) < EPSILON;
    //        return x && y && z;
    //    }
    //    return false;
    //}

    ///// <summary>
    ///// http://paulbourke.net/geometry/pointlineplane/calclineline.cs
    ///// Calculates the intersection line segment between 2 lines (not segments).
    ///// Returns false if no solution can be found.
    ///// </summary>
    ///// <returns></returns>
    //public static bool CalculateLineLineIntersection(Vector3 line1Point1, Vector3 line1Point2,
    //    Vector3 line2Point1, Vector3 line2Point2, out Vector3 resultSegmentPoint1, out Vector3 resultSegmentPoint2)
    //{
    //    // Algorithm is ported from the C algorithm of 
    //    // Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
    //    resultSegmentPoint1 = Vector3.zero;
    //    resultSegmentPoint2 = Vector3.zero;
    //    float EPSILON = 0.001f;

    //    Vector3 p1 = line1Point1;
    //    Vector3 p2 = line1Point2;
    //    Vector3 p3 = line2Point1;
    //    Vector3 p4 = line2Point2;
    //    Vector3 p13 = p1 - p3;
    //    Vector3 p43 = p4 - p3;
        
    //    if (p43.sqrMagnitude < EPSILON)
    //    {
    //        return false;
    //    }
    //    Vector3 p21 = p2 - p1;
    //    if (p21.sqrMagnitude < EPSILON)
    //    {
    //        return false;
    //    }

    //    double d1343 = p13.x * (double)p43.x + (double)p13.y * p43.y + (double)p13.z * p43.z;
    //    double d4321 = p43.x * (double)p21.x + (double)p43.y * p21.y + (double)p43.z * p21.z;
    //    double d1321 = p13.x * (double)p21.x + (double)p13.y * p21.y + (double)p13.z * p21.z;
    //    double d4343 = p43.x * (double)p43.x + (double)p43.y * p43.y + (double)p43.z * p43.z;
    //    double d2121 = p21.x * (double)p21.x + (double)p21.y * p21.y + (double)p21.z * p21.z;

    //    double denom = d2121 * d4343 - d4321 * d4321;
    //    if (Math.Abs(denom) < EPSILON)
    //    {
    //        return false;
    //    }
    //    double numer = d1343 * d4321 - d1321 * d4343;

    //    double mua = numer / denom;
    //    double mub = (d1343 + d4321 * (mua)) / d4343;

    //    resultSegmentPoint1.x = (float)(p1.x + mua * p21.x);
    //    resultSegmentPoint1.y = (float)(p1.y + mua * p21.y);
    //    resultSegmentPoint1.z = (float)(p1.z + mua * p21.z);
    //    resultSegmentPoint2.x = (float)(p3.x + mub * p43.x);
    //    resultSegmentPoint2.y = (float)(p3.y + mub * p43.y);
    //    resultSegmentPoint2.z = (float)(p3.z + mub * p43.z);

    //    return true;
    //}
}
