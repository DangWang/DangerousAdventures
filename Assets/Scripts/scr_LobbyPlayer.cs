using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class scr_LobbyPlayer : NetworkBehaviour
{
    [SyncVar] public bool isReady;

    [SyncVar] public string myRole = "";

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                CmdChangeMyText("Adventurer");
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                CmdChangeMyText("DM");
            }
            if (Input.GetKeyUp(KeyCode.R))
            {
                CmdSwitchReady();
            }
        }
    }

    [Command]
    private void CmdChangeMyText(string role)
    {
        RpcChangeText(role, name);
        myRole = role;
    }

    [Command]
    private void CmdSwitchReady()
    {
        isReady = !isReady;
    }

    [ClientRpc]
    private void RpcChangeText(string role, string target)
    {
        GameObject.Find("Canvas/Text_" + target + "Role").GetComponent<Text>().text = target + " role: " + role;
    }

    [ClientRpc]
    public void RpcChangeName(string s)
    {
        name = s;
    }
}