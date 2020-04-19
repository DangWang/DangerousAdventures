using System;
using Mirror;

public class scr_NetworkedObject : NetworkBehaviour
{
    [SyncVar] public string tempName = "";

    private void Update()
    {
        if (gameObject.name != tempName && tempName != "")
        {
            gameObject.name = tempName;
            print(gameObject.name);
            enabled = false;
        }
    }
}