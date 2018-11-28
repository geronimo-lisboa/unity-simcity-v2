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
    private List<LineSegment> splited = new List<LineSegment>();
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
        splited = SplitSegmentsV2(segments);
        //List<Vector3> intersectionList = FindIntersections(segments);
        //intersections = intersectionList;
        //Divide os segmentos nas interseções, criando uma nova lista de segmentos
        //SplitSegments(intersectionList, segments);
        
    }


    private List<LineSegment> SplitSegmentsV2(List<LineSegment> segs)
    {
        LineSegment2d.ResetCounter();
        var _2dSegments = segs.Select<LineSegment, LineSegment2d>(s => new LineSegment2d(s.Point1, s.Point2)).ToList();
        var SplitedSegments = new List<LineSegment>();
        for (int i = 0; i < _2dSegments.Count; i++)
        {
            bool isIsolated = true;
            var s1 = _2dSegments[i];
            for (int j=0; j< _2dSegments.Count; j++)
            {
                
                var s2 = _2dSegments[j];
                if (s1 == s2)
                    continue;
                Vector2 intersectionIn2d;
                bool doesIntersect = Line2dIntersectionService.LineSegmentsIntersection(s1.Point1, s1.Point2, s2.Point1, s2.Point2, out intersectionIn2d);
                if (doesIntersect)
                {
                    //Passagem do ponto de interseção do 2d pro 3d.
                    //A posição paramétrica no segmento 1 do ponto de interseção. Poderia ser no segmento 2 mas tanto faz, a posição espacial vai ser a mesma.
                    float gammaP2d = (intersectionIn2d - s1.Point1).magnitude / (s1.Point2 - s1.Point1).magnitude;
                    //Relação entre as magnitudes do vetor2d e do vetor3d que corresponde a ele
                    var R = (s1.Point2 - s1.Point1).magnitude / s1.OriginalMagnitude;//(segments[i].Point2 - segments[i].Point1).magnitude;
                    //o parâmetro no vetor 3d
                    var gammaP3d = gammaP2d * R;
                    //o ponto da interseção no 3d
                    var intersectionPoint = s1.OriginalSegment.Point1 + gammaP3d * (s1.OriginalSegment.Point2 - s1.OriginalSegment.Point1);
                    //Os dois segmentos interceptantes devem ser divididos. Isso vai fazer com que 2 virem 4. O ponto de divisão é o ponto de interseção no 3d.
                    //1)Segmento 1
                    LineSegment s1A = new LineSegment(segs[i].Point1, intersectionPoint);
                    LineSegment s1B = new LineSegment(intersectionPoint, segs[i].Point2);
                    //LineSegment s2A = new LineSegment(s2.Point1, intersectionPoint); //Pq vou passar 2 vezes aqui.
                    //LineSegment s2B = new LineSegment(intersectionPoint, s2.Point2);
                    SplitedSegments.Add(s1A);
                    SplitedSegments.Add(s1B);
                    //SplitedSegments.Add(s2A);
                    //SplitedSegments.Add(s2B);
                    isIsolated = false;

                }
            }
            if (isIsolated)
                SplitedSegments.Add(s1.OriginalSegment);
        }
        Debug.Log($"Qtd = {SplitedSegments.Count}");
        return SplitedSegments;
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
        
        foreach(var s in splited)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(s.Point1, s.Point2);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(s.Point1, 0.125f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(s.Point2, 0.25f);
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

}
