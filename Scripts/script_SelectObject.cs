using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_SelectObject : MonoBehaviour
{
    public GameObject selected;
    ISelector script;
    Transform root;
    Ray ray;
    Vector3 mouseScreenPosition;
    Vector3 mouseWorldPosition;

    void Start()
    {
        root = this.transform;
        while(root.parent != null)
        {
            root = root.parent;
        } 
        if(root.GetComponent<ISelector>() == null)
        {
            Debug.LogError("The root parent cannot select objects.");
        }else{
            script = root.GetComponent<ISelector>();
        }
    }
    void Update()
    {
        mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = 1;
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if(Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))//, layerMask))
            {
                selected = hit.collider.gameObject;
                script.selected = selected;
                //TODO if dm and selected = monster call OnMonsterSelected 
            }
        }
    }

    public static GameObject ReturnAlternateClick()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))//, layerMask))
        {
            return hit.collider.gameObject;
        }else{
            Debug.LogWarning("No object was raycasted after click");
            return null;
        }
    }
}
