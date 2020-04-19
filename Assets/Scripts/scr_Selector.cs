using Mirror;
using UnityEngine;

public class scr_Selector : NetworkBehaviour
{
    private Vector3 _mouseScreenPosition;
    private Vector3 _mouseWorldPosition;
    private GameObject _previousSelected;
    private Ray _ray;
    private Transform _root;
    private scr_Player _script;
    public GameObject selected;

    private void Start()
    {
        _root = transform;
        while (_root.parent != null) _root = _root.parent;
        if (_root.GetComponent<scr_Player>() == null)
            Debug.LogError("The root parent cannot select objects.");
        else
            _script = _root.GetComponent<scr_Player>();
    }

    private void Update()
    {
        _mouseScreenPosition = Input.mousePosition;
        _mouseScreenPosition.z = 1;
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(_mouseScreenPosition);
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(_ray, out hit, Mathf.Infinity)) //, layerMask))
            {
                _script.previousSelected = _script.selected;
                selected = hit.collider.gameObject;
                _script.selected = selected;
                //TODO if dm and selected = monster call OnMonsterSelected 
            }
        }
        else if (Physics.Raycast(_ray, out hit, Mathf.Infinity)) //, layerMask))
        {
            selected = hit.collider.gameObject;
            if (_previousSelected != null && selected != _previousSelected)
                _previousSelected.GetComponent<SpriteRenderer>().color = Color.white;
            if (selected.tag == "Free" || selected.tag == "Wall" || selected.tag == "PlayerSpawn" ||
                selected.tag == "MonsterSpawn" || selected.tag == "Door" || selected.tag == "OpenDoor")
            {
                selected.GetComponent<SpriteRenderer>().color = Color.magenta;
                _previousSelected = selected;
            }
        }
    }

    public static GameObject ReturnAlternateClick()
    {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) //, layerMask))
        {
            Debug.Log("ALTERNATE" + hit.collider.gameObject.name);
            return hit.collider.gameObject;
        }

        Debug.LogWarning("No object was raycasted after click");
        return null;
    }
}