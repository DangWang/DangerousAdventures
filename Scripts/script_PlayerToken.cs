using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class script_PlayerToken : MonoBehaviour, ISelector, IPlayer, IAttacker, IDefender
{
    List<GameObject> allowedMovement = new List<GameObject>();
    public int hitpoints;
    public int range;
    public int attackDice;
    public int defenceDice;
    public Enumerations.Direction myDirection = Enumerations.Direction.Choose;
    public int availableMovement;
    public int movementDice;
    script_BoardController myScript_BoardController;
    script_MovementParser myScript_MovementParser;
    GameObject neighbor, myTile, directions;
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
    public bool placed = false;
    Canvas canvasLocal, canvasWorld;
    GameObject inventoryHUD;
    Text availableMovementField;
    Text hitpointsField;

    void Start()
    {
        myScript_BoardController = GameObject.Find("Board").GetComponent<script_BoardController>();
        myScript_MovementParser = this.GetComponent<script_MovementParser>();
        canvasLocal = this.transform.Find("Canvas_Local").GetComponent<Canvas>();
        canvasWorld = this.transform.Find("Canvas_World").GetComponent<Canvas>();
        directions = this.transform.Find("Canvas_Local/Directions").gameObject;
        availableMovementField = this.transform.Find("Canvas_World/AdventurerHUD/Movement/Text").GetComponent<Text>();
        inventoryHUD = this.transform.Find("Canvas_World/InventoryHUD").gameObject;
        hitpointsField = this.transform.Find("Canvas_World/AdventurerHUD/Health/Text").GetComponent<Text>();
        hitpointsField.text = hitpoints.ToString();

        inventoryHUD.SetActive(false);
        canvasLocal.gameObject.SetActive(false);
        canvasWorld.gameObject.SetActive(false);
    }

    void Update()
    {
        if(beginMyTurn)
        {
            canvasLocal.gameObject.SetActive(true);
            canvasWorld.gameObject.SetActive(true);

            availableMovement = Utilities.RollDice(movementDice);
            availableMovementField.text = availableMovement.ToString();
            beginMyTurn = false;

            script_MovementPainter.RemoveAllowedMovementMarker(allowedMovement);
            allowedMovement = myScript_MovementParser.GetAllowedMovement(myTile, ref myDirection, availableMovement);
            script_MovementPainter.AddAllowedMovementMarker(allowedMovement);
        }
        else if(isMyTurn)
        {
            if(Input.GetKeyUp(KeyCode.N))
            {
                EndTurn();
            }

            if(Input.GetKeyUp(KeyCode.Space))
            {
                if(placed == false)
                {
                    if(selected.tag == "PlayerSpawn")
                    {
                        this.transform.position = selected.transform.position;
                        myTile = selected;
                        placed = true;
                    }else{
                        Debug.Log("Select an eligible spawn tile");
                    }
                }
            }
            if (myDirection == Enumerations.Direction.Choose)
            {
                myDirection = Enumerations.ChooseDirection(directions);
                script_MovementPainter.RemoveAllowedMovementMarker(allowedMovement);
                allowedMovement = myScript_MovementParser.GetAllowedMovement(myTile, ref myDirection, availableMovement);
                script_MovementPainter.AddAllowedMovementMarker(allowedMovement);
            }

            if(Input.GetMouseButtonUp(1))
            {
                altSelected = script_SelectObject.ReturnAlternateClick();   

                if(allowedMovement.Contains(altSelected) == true)
                {
                    if(script_BoardController.GetTileDistance(myTile, altSelected) <= availableMovement)
                    {
                        Move(altSelected, allowedMovement);
                    }
                }
                script_MovementPainter.RemoveAllowedMovementMarker(allowedMovement);
                allowedMovement = myScript_MovementParser.GetAllowedMovement(myTile, ref myDirection, availableMovement);
                script_MovementPainter.AddAllowedMovementMarker(allowedMovement);
            }
        }
    }
    //TODO don't allow the player to move to occupied tiles!
    public void Move(GameObject toTile, List<GameObject> allowedMovement)
    {
        //this.transform.position = toTile.transform.position;
        //myTile = toTile;
        int index = 0;
        bool activatedTrap;
        while(myTile != toTile)
        {
            myTile = allowedMovement.ElementAt(index++);
            this.transform.position = myTile.transform.position;
            availableMovement--;
            availableMovementField.text = availableMovement.ToString();
            //tile call method stepped on if not invisible
            activatedTrap = myTile.GetComponent<script_Tile>().SteppedOn(this.gameObject);
            if(activatedTrap)
                break;
        }
    }
    public GameObject ReturnPlayerObject()
    {
        return this.gameObject;
    }
    public void ToggleInventory()
    {
        if(inventoryHUD.activeInHierarchy == false)
            inventoryHUD.SetActive(true);
        else
            inventoryHUD.SetActive(false);
    }
    public void EndTurn()
    {
        canvasLocal.gameObject.SetActive(false);
        canvasWorld.gameObject.SetActive(false);
        script_GameManager.NextTurn();
    }   

    public void Attack()
    {
        print("Attacking");
    }
    public void Defend(int damage, Enumerations.DamageType damageType)
    {
        print("Defending");
        if(damageType == Enumerations.DamageType.Physical)
            hitpoints -= (damage - Utilities.RollDice(defenceDice));
        else
            hitpoints -= damage;
        hitpointsField.text = hitpoints.ToString();
        if(hitpoints < 0)
        {
            Die();
        }
    }
    private void Die()
    {
        print("I died!");
        Destroy(this.gameObject);
    }
}
