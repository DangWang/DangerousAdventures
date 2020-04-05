using System.Collections.Generic;
using UnityEngine;

public class script_MovementPainter : MonoBehaviour
{
    private static readonly GameObject marker = Resources.Load("marker") as GameObject;

    public static void AddAllowedMovementMarker(List<GameObject> allowedMovement)
    {
        foreach (var g in allowedMovement)
        {
            var mark = Instantiate(marker, g.transform.position, Quaternion.identity);
            mark.name = "Movement_Marker";
            mark.transform.parent = g.transform;
        }
    }

    public static void RemoveAllowedMovementMarker(List<GameObject> allowedMovement)
    {
        if (allowedMovement.Count > 0)
            foreach (var g in allowedMovement)
            {
                var mark = g.transform.Find("Movement_Marker").gameObject;
                if (mark != null)
                    Destroy(mark);
            }
    }
}