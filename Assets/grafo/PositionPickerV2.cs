using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPickerV2 : MonoBehaviour {
    public RoadControllerMK2 controller;
    public Camera SceneCamera;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = SceneCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitpoint = hit.point;
                hitpoint.y += 0.2f;//Quero um tiquinho acima do plano
                //Guarda o ponto no controller.
                if (controller.LayRoadState == RoadControllerMK2.LayRoadFirstPoint)
                {
                    controller.StoreFirstRoadPoint(hitpoint);
                }
                else if (controller.LayRoadState == RoadControllerMK2.LayRoadSecondPoint)
                {
                    controller.StoreSecondPoint(hitpoint);
                }
                
            }
        }
    }
}
