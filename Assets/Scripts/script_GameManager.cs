using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class script_GameManager : NetworkBehaviour
{
    public static int playersInLobby = 0;
    public static int playersInGame = 0;
    public static int maxNumberOfPlayers = 3;
    public static scr_Player[] myPlayers = new scr_Player[maxNumberOfPlayers];
    public static bool preparationPhase;
    private static int _currentPlayer = 0;
    private static Text _currentPlayerText;
    private bool _gameInProgress;

    private void Start()
    { ;
        preparationPhase = false;
        _gameInProgress = false;
    }

    private void Update()
    {
        if (isServer)
        {
            if (Input.GetKey(KeyCode.F5))
            {
                SceneManager.LoadScene(0);
            }
            if (playersInGame == playersInLobby && _gameInProgress == false)
            {
                print("Let the gaaaameeeees begiiin!");
                _gameInProgress = true;
                RpcNextTurn(myPlayers[playersInGame - 1].name);
            }
        }
    }

    [ClientRpc]
    public void RpcNextTurn(string current)
    {
        if (isServer)
        {
            RpcEndPlayerTurn(current);
        }
        _currentPlayer++;
        if (_currentPlayer >= playersInGame)
        {
            _currentPlayer = 0;
        }
        print("Next player: " + myPlayers[_currentPlayer].name);
        if (isServer)
        {
            RpcStartPlayerTurn(myPlayers[_currentPlayer].name);
        }
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