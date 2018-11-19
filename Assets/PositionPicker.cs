using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Gives me where the game object was touched, in the world coordinate system.
/// </summary>
public class PositionPicker : MonoBehaviour
{
    public Camera SceneCamera;
    public RoadContoller MyRoadController;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = SceneCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitpoint = hit.point;
                MyRoadController.AddPoint(hitpoint);

            }
        }
    }
}
