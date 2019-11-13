using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_CameraController : MonoBehaviour
{
    public float cameraSpeed;
    public float scrollSpeed;

    float scroll;

    Vector3 newPosition;

    void FixedUpdate()
    {
        newPosition = this.transform.position;
        scroll  = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKey(KeyCode.W))
        {
            newPosition.y += cameraSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            newPosition.y -= cameraSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPosition.x += cameraSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            newPosition.x -= cameraSpeed;
        }
        newPosition.z += scroll * scrollSpeed;

        this.transform.position = newPosition;
    }
}
