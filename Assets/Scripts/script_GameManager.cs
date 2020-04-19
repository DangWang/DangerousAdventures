using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class script_GameManager : NetworkBehaviour
{
    public static int playersInGame = 0;
    public static int numberOfPlayers = 2;
    public static scr_Player[] myPlayers = new scr_Player[numberOfPlayers];
    public static bool preparationPhase;
    private static int s_currentPlayer = 1;
    private static Text s_currentPlayerText;
    private bool _gameInProgress;

    private void Start()
    {
        //myPlayers[0] = GameObject.Find("DMNew").GetComponent<scr_Player>();
        //myPlayers[1] = GameObject.Find("PlayerNew").GetComponent<scr_Player>();
        //currentPlayerText = transform.Find("Canvas/CurrentPlayer/Text").GetComponent<Text>();
        preparationPhase = false;
        _gameInProgress = false;
        //StartPlayerTurn(ref myPlayers[0]);//start dm turn TODO MAKE THIS SERIOUS
    }

    private void Update()
    {
        if (isServer)
        {
            if (Input.GetKey(KeyCode.F5)) SceneManager.LoadScene(0);
            if (playersInGame == numberOfPlayers && _gameInProgress == false)
            {
                print("Let the gaaaameeeees begiiin!");
                _gameInProgress = true;
                RpcNextTurn(myPlayers[1].name);
            }
        }
    }

    [ClientRpc]
    public void RpcNextTurn(string current)
    {
        RpcEndPlayerTurn(current);
        s_currentPlayer++;
        if (s_currentPlayer >= numberOfPlayers)
            s_currentPlayer = 0;
        print("Next player: " + myPlayers[s_currentPlayer].name);
        if (isServer)
            RpcStartPlayerTurn(myPlayers[s_currentPlayer].name);
    }

    [ClientRpc]
    private void RpcEndPlayerTurn(string player)
    {
        GameObject.Find(player).GetComponent<scr_Player>().isMyTurn = false;
    }

    [ClientRpc]
    private void RpcStartPlayerTurn(string player)
    {
        GameObject.Find(player).GetComponent<scr_Player>().beginMyTurn = true; //can the player exploit this?
        GameObject.Find(player).GetComponent<scr_Player>().isMyTurn = true;
        //currentPlayerText.text = myPlayers[currentPlayer].name;
    }
}