using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class script_MovementPainter : MonoBehaviour
{
    static GameObject marker = Resources.Load("marker") as GameObject;

    public static void AddAllowedMovementMarker(List<GameObject> allowedMovement)
    {
        foreach(GameObject g in allowedMovement)
        {
            GameObject mark = Instantiate(marker, g.transform.position, Quaternion.identity);
            mark.name = "Movement_Marker";
            mark.transform.parent = g.transform;
        }
    }

    public static void RemoveAllowedMovementMarker(List<GameObject> allowedMovement)
    {
        foreach (GameObject g in allowedMovement)
        {
            GameObject mark = g.transform.Find("Movement_Marker").gameObject;
            Destroy(mark);
        }
    }
}
