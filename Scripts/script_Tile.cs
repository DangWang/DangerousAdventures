using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_Tile : MonoBehaviour
{
    public GameObject myTrap;
    public bool occupied;
    public bool eligibleForTrap;
    public bool restrictingMovement; //TODO PROBABLY WONT BE USED SLOW GETCOMPONENT

    public bool SteppedOn(GameObject who)
    {
        if(myTrap != null)
        {
            myTrap.GetComponent<script_Trap>().Trigger(who);
            return true;
        }
        return false;
    }
}
