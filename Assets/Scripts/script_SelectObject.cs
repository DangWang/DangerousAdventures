using Mirror;
using UnityEngine;

public class script_SelectObject : NetworkBehaviour
{
    private Vector3 mouseScreenPosition;
    private Vector3 mouseWorldPosition;
    private GameObject previousSelected;
    private Ray ray;
    private Transform root;
    private ISelector script;
    public GameObject selected;

    private void Start()
    {
        root = transform;
        while (root.parent != null) root = root.parent;
        if (root.GetComponent<ISelector>() == null)
            Debug.LogError("The root parent cannot select objects.");
        else
            script = root.GetComponent<ISelector>();
    }

    private void Update()
    {
        mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = 1;
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) //, layerMask))
            {
                script.previousSelected = script.selected;
                selected = hit.collider.gameObject;
                script.selected = selected;
                //TODO if dm and selected = monster call OnMonsterSelected 
            }
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity)) //, layerMask))
        {
            selected = hit.collider.gameObject;
            if (previousSelected != null && selected != previousSelected)
                previousSelected.GetComponent<SpriteRenderer>().color = Color.white;
            if (selected.tag == "Free" || selected.tag == "Wall" || selected.tag == "PlayerSpawn" ||
                selected.tag == "MonsterSpawn" || selected.tag == "Door" || selected.tag == "OpenDoor")
            {
                selected.GetComponent<SpriteRenderer>().color = Color.magenta;
                previousSelected = selected;
            }
        }
    }

    public static GameObject ReturnAlternateClick()
    {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) //, layerMask))
        {
            return hit.collider.gameObject;
        }

        Debug.LogWarning("No object was raycasted after click");
        return null;
    }
}