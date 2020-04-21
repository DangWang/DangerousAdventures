using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class scr_Movement : NetworkBehaviour
{
    private static GameObject s_marker;
    public List<GameObject> allowedMovement = new List<GameObject>();
    [SyncVar] public int availableMovement;
    private GameObject _canvasDirections;
    [SyncVar] public bool hasMoved;
    [SyncVar] public bool independentDicePool;
    [SyncVar] public int movementDice;
    [SyncVar] public Enumerations.Direction myDirection = Enumerations.Direction.Choose;
    public GameObject myTile;
    public scr_Player parentScript;

    public string[] tagsOfMovementTiles;
    public bool unrestrictedMovement;

    private void Start()
    {
        s_marker = Resources.Load("marker") as GameObject;
        _canvasDirections = transform.Find("Canvas_Direction").gameObject;
        _canvasDirections.SetActive(false);
        //parentScript = GetComponent<scr_Player>();
    }

    public void OnStartTurn()
    {
        CmdOnStartTurn(myTile.name);
        myTile = GameObject.Find(myTile.name);
    }

    [Command]
    public void CmdOnStartTurn(string tileName)
    {
        if (independentDicePool)
        {
            availableMovement = Utilities.RollDice(movementDice);
        }
        else
        {
            availableMovement = 7;
        }
        hasMoved = false;
        myDirection = Enumerations.Direction.Choose;
        myTile = GameObject.Find(tileName);
    }


    public void OnSelectedUpdate()
    {
        if (!independentDicePool && parentScript.sharedRemainingMovement < availableMovement)
        {
            availableMovement = parentScript.sharedRemainingMovement;
        }
        if (myDirection == Enumerations.Direction.Choose)
        {
            myDirection = Enumerations.ChooseDirection(_canvasDirections);
            if (myDirection != Enumerations.Direction.Choose)
            {
                allowedMovement = GetAllowedMovement(myTile, ref myDirection, availableMovement);
                if (allowedMovement.Count == 0)
                {
                    print("No remaining movement");
                }

                AddAllowedMovementMarker(allowedMovement);
            }

            CmdUpdateDirection(myDirection);
        }

        if (Input.GetKeyUp(KeyCode.R) && hasMoved == false)
        {
            myDirection = Enumerations.Direction.Choose;
        }
        if (Input.GetMouseButtonUp(1))
        {
            parentScript.altSelected = script_SelectObject.ReturnAlternateClick();

            if (allowedMovement.Contains(parentScript.altSelected) && parentScript.altSelected.GetComponent<script_Tile>().occupied == false)
            {
                if (script_BoardController.GetTileDistance(myTile, parentScript.altSelected) <= availableMovement)
                {
                    CmdMove(parentScript.altSelected.name);
                }
            }
            //Move(parentScript.altSelected, allowedMovement);
            RemoveAllowedMovementMarker(allowedMovement);
            // allowedMovement = GetAllowedMovement(myTile, ref myDirection, availableMovement);
            // AddAllowedMovementMarker(allowedMovement);
        }
    }

    [Command]
    public void CmdUpdateDirection(Enumerations.Direction dir)
    {
        myDirection = dir;
    }

    public void Unselected()
    {
        _canvasDirections.SetActive(false);
        try
        {
            RemoveAllowedMovementMarker(allowedMovement);
        }
        catch
        {
            //nothing
        }

        allowedMovement.Clear();
    }


    [Command]
    private void CmdMove(string toTileName)
    {
        var toTile = GameObject.Find(toTileName);
        allowedMovement = GetAllowedMovement(myTile, ref myDirection, availableMovement);
        Move(toTile, allowedMovement);
    }


    public int Move(GameObject toTile, List<GameObject> allowedMovement)
    {
        GameObject tileTemp;
        var index = 0;
        var tilesMoved = 0;
        bool activatedTrap;
        while (myTile != toTile)
        {
            tileTemp = allowedMovement.ElementAt(index++);
            if (tileTemp.GetComponent<script_Tile>().occupied)
            {
                myDirection = Enumerations.Direction.Choose;
                break;
            }

            tilesMoved++;
            hasMoved = true;
            myTile.GetComponent<script_Tile>().occupied = false;
            myTile.GetComponent<script_Tile>().occupier = null;
            myTile = tileTemp;
            myTile.GetComponent<script_Tile>().occupied = true;
            myTile.GetComponent<script_Tile>().occupier = gameObject;
            transform.position = myTile.transform.position;
            availableMovement--;
            //availableMovementField.text = availableMovement.ToString();
            //tile call method stepped on if not invisible
            activatedTrap = myTile.GetComponent<script_Tile>().SteppedOn(gameObject);
            if (activatedTrap)
            {
                break;
            }
        }

        RpcUpdateMyTile(myTile.name);
        return tilesMoved;
    }

    [ClientRpc]
    public void RpcUpdateMyTile(string tileName)
    {
        myTile = GameObject.Find(tileName);
        allowedMovement = GetAllowedMovement(myTile, ref myDirection, availableMovement);
        AddAllowedMovementMarker(allowedMovement);
    }

    public List<GameObject> GetAllowedMovement(GameObject myTile, ref Enumerations.Direction myDirection, int availableMovement)
    {
        var tiles = new List<GameObject>();
        GameObject neighbor, tempTile = myTile;
        var i = 0;

        while (i++ < availableMovement)
        {
            neighbor = script_BoardController.GetTileNeighbor(tempTile, myDirection);
            if (neighbor == null)
            {
                return tiles;
            }
            if (myDirection != Enumerations.Direction.Choose)
            {
                var check = false;
                foreach (var tag in tagsOfMovementTiles)
                {
                    if (neighbor.tag == tag)
                    {
                        check = true;
                        tempTile = neighbor;
                        tiles.Add(neighbor);
                    }
                }

                if (check == false)
                {
                    if (tiles.Count == 0)
                    {
                        myDirection = Enumerations.Direction.Choose;
                    }
                    return tiles;
                }
            }
            else
            {
                return tiles;
            }
        }

        if (i >= availableMovement)
        {
            return tiles;
        }
        Debug.LogError("problem");
        return null;
    }

    public void AddAllowedMovementMarker(List<GameObject> allowedMovement)
    {
        if (hasAuthority)
        {
            foreach (var g in allowedMovement)
            {
                var mark = Instantiate(s_marker, g.transform.position, Quaternion.identity);
                mark.name = "Movement_Marker";
                mark.transform.parent = g.transform;
            }
        }
    }

    public void RemoveAllowedMovementMarker(List<GameObject> allowedMovement)
    {
        if (hasAuthority)
        {
            if (allowedMovement.Count > 0)
            {
                foreach (var g in allowedMovement)
                {
                    var mark = g.transform.Find("Movement_Marker").gameObject;
                    if (mark != null)
                    {
                        Destroy(mark);
                    }
                }
            }
        }
    }
}