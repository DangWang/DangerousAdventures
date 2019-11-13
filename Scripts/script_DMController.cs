using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class script_DMController : MonoBehaviour, ISelector, IPlayer
{
    public List<GameObject> monsters = new List<GameObject>();
    List<GameObject> allowedMovement = new List<GameObject>();
    public GameObject[] monsterTypes;
    public int numberOfMonsters = 0;
    public int maxMonsters;
    public int movementDice;
    public int availableMovement;
    script_SelectObject myScript_SelectObject;
    public GameObject selected
    {
        get;
        set;
    }
    public GameObject altSelected
    {
        get;
        set;
    }
    public bool beginMyTurn
    {
        get;
        set;
    }
    public bool isMyTurn
    {
        get;
        set;
    }
    script_MonsterController myScript_MonsterController;
    script_MovementParser myScript_MovementParser;
    GameObject myTile;
    Canvas myCanvas;
    Text availableMovementField;
    GameObject trapMenu, activeMenu;

    public GameObject[] trapTypes;
    public List<GameObject> traps = new List<GameObject>();
    public GameObject selectedTrapType;

    void Start()
    {
        myScript_SelectObject = this.GetComponentInChildren<script_SelectObject>();
        myCanvas = this.transform.Find("Canvas_DM").GetComponent<Canvas>();
        trapMenu = this.transform.Find("Canvas_DM/TrapMenu").gameObject;
        activeMenu = this.transform.Find("Canvas_DM/ActiveHUD").gameObject;
        availableMovementField = this.transform.Find("Canvas_DM/AvailableMovement/Text").GetComponent<Text>();

        myCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if(beginMyTurn)
        {
            myCanvas.gameObject.SetActive(true);
            //TODO for every monster reset movement

            availableMovement = Utilities.RollDice(movementDice);
            availableMovementField.text = "Available:" + availableMovement.ToString();
            beginMyTurn = false;
        }
        else if(isMyTurn)
        {
            if (script_GameManager.preparationPhase == true)
            {
                if (Input.GetKeyUp(KeyCode.I))
                {
                    ToggleTrapMenu();
                }
                if (Input.GetKeyUp(KeyCode.N))
                {
                    EndPreparation();
                }
                if (Input.GetMouseButtonUp(1))
                {
                    altSelected = script_SelectObject.ReturnAlternateClick();
                    if (altSelected != null && CheckTrapSquareEligibility(altSelected)) //TODO Selected.tag in allowed squares for trap
                    {
                        script_Tile tileScript = altSelected.GetComponent<script_Tile>();
                        if (tileScript.myTrap != null)
                        {
                            //TODO Remove from my list
                            Destroy(tileScript.myTrap);
                            tileScript.myTrap = null;
                        }else
                            traps.Add(PlaceTrap(selectedTrapType, altSelected, tileScript));
                    }
                }

                return;
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.N))
                {
                    EndTurn();
                }

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    if (numberOfMonsters < maxMonsters)
                    {
                        //Check if the tile is occupied
                        myTile = selected;
                        if (selected.tag == "MonsterSpawn")
                        {
                            monsters.Add(Instantiate(monsterTypes[0], myTile.transform.position, Quaternion.identity));
                            monsters[numberOfMonsters].GetComponent<script_MonsterController>().myTile = myTile;
                            numberOfMonsters++;
                        }
                    }
                }

                if (selected.tag == "Monster")
                {
                    myScript_MonsterController = selected.GetComponent<script_MonsterController>();
                    myScript_MovementParser = selected.GetComponent<script_MovementParser>();
                    myScript_MonsterController.SelectedUpdate();
                    if (Input.GetMouseButtonUp(1))
                    {
                        script_MovementPainter.RemoveAllowedMovementMarker(allowedMovement);
                        allowedMovement = myScript_MovementParser.GetAllowedMovement(selected, ref myScript_MonsterController.myDirection, myScript_MonsterController.myRemainingMovement);
                        script_MovementPainter.AddAllowedMovementMarker(allowedMovement);
                        if (allowedMovement.Count != 0)
                        {
                            altSelected = script_SelectObject.ReturnAlternateClick();

                            if (allowedMovement.Contains(altSelected) == true)
                            {
                                if (script_BoardController.GetTileDistance(selected, altSelected) <= availableMovement)
                                {
                                    availableMovement -= script_BoardController.GetTileDistance(selected, altSelected);
                                    myScript_MonsterController.myRemainingMovement -= script_BoardController.GetTileDistance(selected, altSelected); ;
                                    availableMovementField.text = "Available:" + availableMovement.ToString();
                                    myScript_MonsterController.Move(altSelected);
                                }
                            }
                        }
                    }
                }
            }            
        }
    }
    public void OnSelectMonster()
    {
        script_MovementPainter.RemoveAllowedMovementMarker(allowedMovement);
        allowedMovement = myScript_MovementParser.GetAllowedMovement(selected, ref myScript_MonsterController.myDirection, myScript_MonsterController.myRemainingMovement);
        script_MovementPainter.AddAllowedMovementMarker(allowedMovement);
    }
    public GameObject ReturnPlayerObject()
    {
        return this.gameObject;
    }
    public void EndTurn()
    {
        myCanvas.gameObject.SetActive(false);
        script_GameManager.NextTurn();
    }  
    public void EndPreparation()
    {
        script_GameManager.EndPreparationPhase();
    }

    GameObject PlaceTrap(GameObject trap, GameObject tile, script_Tile tileScript)
    {
        GameObject newTrap = Instantiate(trap, tile.transform.position, Quaternion.identity);
        tileScript.myTrap = newTrap;
        return newTrap;
    }
    //TODO ALSO CHECK IF IT BLOCKS SOMETHING DYNAMICALLY
    bool CheckTrapSquareEligibility(GameObject tile)
    {
        /*
        List<GameObject> neighbors = script_BoardController.GetTileNeighbors(tile);
        //if(tile.tag != "Free") //TODO add more than free and blocked tile types
        foreach(GameObject g in neighbors)
        {
            //if(g.tag == "Free")//TODO and there is no trap on it
        }*/
        
        return tile.GetComponent<script_Tile>().eligibleForTrap;
    }

    public void SelectTrap(int i)//TODO Enumeration instead of integer
    {
        selectedTrapType = trapTypes[i];
    }

    public void ToggleTrapMenu()
    {
        if (trapMenu.activeInHierarchy == false)
            trapMenu.SetActive(true);
        else
        {
            trapMenu.SetActive(false);
        }
    }
    
    public void ToggleActiveMenu()
    {
        if (activeMenu.activeInHierarchy == false)
            activeMenu.SetActive(true);
        else
        {
            activeMenu.SetActive(false);
        }
    }
}
