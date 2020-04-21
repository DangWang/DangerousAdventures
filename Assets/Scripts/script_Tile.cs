using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class script_Tile : NetworkBehaviour
{
    public bool eligibleForTrap;
    public List<GameObject> items = new List<GameObject>();

    [SyncVar] public GameObject myTrap;

    [SyncVar] public bool occupied;

    [SyncVar] public GameObject occupier;

    public bool restrictingMovement; //TODO PROBABLY WONT BE USED SLOW GETCOMPONENT

    public bool SteppedOn(GameObject who)
    {
        occupier = who; //TODO create a function steppedoff
        if (myTrap != null)
        {
            myTrap.GetComponent<script_Trap>().Trigger(who);
            return true;
        }

        if (items.Count != 0)
        {
            if (who.GetComponent<scr_Inventory>() != null)
            {
                who.GetComponent<scr_Inventory>().PickUp(items.ElementAt(0));
                items.Remove(items.ElementAt(0));
            }
        }

        return false;
    }
}