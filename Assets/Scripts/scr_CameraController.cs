using UnityEngine;

public class scr_CameraController : MonoBehaviour
{
    public float cameraSpeed;

    private Vector3 newPosition;

    private float scroll;
    public float scrollSpeed;

    private void FixedUpdate()
    {
        newPosition = transform.position;
        scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKey(KeyCode.W)) newPosition.y += cameraSpeed;
        if (Input.GetKey(KeyCode.S)) newPosition.y -= cameraSpeed;
        if (Input.GetKey(KeyCode.D)) newPosition.x += cameraSpeed;
        if (Input.GetKey(KeyCode.A)) newPosition.x -= cameraSpeed;
        newPosition.z += scroll * scrollSpeed;

        transform.position = newPosition;
    }
}