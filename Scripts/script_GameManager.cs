using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class script_GameManager : MonoBehaviour
{
    public static int numberOfPlayers = 2;
    public static IPlayer[] myPlayers = new IPlayer[numberOfPlayers];
    public static bool preparationPhase;
    static int currentPlayer = numberOfPlayers - 1;
    static Text currentPlayerText;


    void Start()
    {
        myPlayers[0] = GameObject.Find("DM").GetComponent<IPlayer>();
        myPlayers[1] = GameObject.Find("Player").GetComponent<IPlayer>();
        currentPlayerText = transform.Find("Canvas/CurrentPlayer/Text").GetComponent<Text>();
        preparationPhase = true;
        StartPlayerTurn(ref myPlayers[0]);//start dm turn TODO MAKE THIS SERIOUS
    }
    public static void NextTurn()
    {
        EndPlayerTurn(ref myPlayers[currentPlayer]);
        currentPlayer++;
        if(currentPlayer == numberOfPlayers)
        {
            currentPlayer = 0;
        }
        StartPlayerTurn(ref myPlayers[currentPlayer]);
    }
    public static void EndPreparationPhase()
    {
        preparationPhase = false;
        NextTurn();
    }
    static void EndPlayerTurn(ref IPlayer player)
    {
        player.isMyTurn = false;
    }

    static void StartPlayerTurn(ref IPlayer player)
    {
        player.beginMyTurn = true; //can the player exploit this?
        player.isMyTurn = true;
        currentPlayerText.text = myPlayers[currentPlayer].ToString();
    }
}