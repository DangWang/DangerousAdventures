using UnityEngine;

public class scr_CameraController : MonoBehaviour
{
    public float cameraSpeed;

    private Vector3 _newPosition;

    private float _scroll;
    public float scrollSpeed;

    private void FixedUpdate()
    {
        _newPosition = transform.position;
        _scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKey(KeyCode.W)) _newPosition.y += cameraSpeed;
        if (Input.GetKey(KeyCode.S)) _newPosition.y -= cameraSpeed;
        if (Input.GetKey(KeyCode.D)) _newPosition.x += cameraSpeed;
        if (Input.GetKey(KeyCode.A)) _newPosition.x -= cameraSpeed;
        _newPosition.z += _scroll * scrollSpeed;

        transform.position = _newPosition;
    }
}