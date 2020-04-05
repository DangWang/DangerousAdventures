using Mirror;
using UnityEngine;

public class scr_Door : NetworkBehaviour
{
    public bool closed = true;
    public Sprite closedSprite;
    public bool locked;
    public Sprite lockedSprite;
    public GameObject myKey;
    public Sprite openSprite;

    [ClientRpc]
    public void RpcToggleDoor()
    {
        print("Door switcheroo!");
        if (closed)
        {
            GetComponent<SpriteRenderer>().sprite = openSprite;
            GetComponent<script_Tile>().occupied = false;
            gameObject.tag = "OpenDoor";
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = closedSprite;
            GetComponent<script_Tile>().occupied = true;
            gameObject.tag = "Door";
        }

        GetComponent<AudioSource>().Play();
        closed = !closed;
    }

    // [ClientRpc]
    // public void RpcToggleLock(scr_Inventory actorInventory)
    // {
    //     if (myKey != null)
    //     {
    //         if (actorInventory.myItems.Contains(myKey))
    //         {
    //             if (!closed) //if the door is not closed, close it first and then lock.
    //                 RpcToggleDoor();
    //             locked = !locked;
    //         }
    //         else
    //         {
    //             Debug.Log("You are not carrying my key.");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Tried to lock/unlock a door without a lock.");
    //     }
    // }
}