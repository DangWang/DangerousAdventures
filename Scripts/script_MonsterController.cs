using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_MonsterController : MonoBehaviour
{
    script_BoardController myScript_BoardController;
    public Enumerations.Direction myDirection = Enumerations.Direction.Choose;
    public GameObject myTile;
    public int myMaxMovement = 8;
    public int myRemainingMovement = 8;
    public string monsterDescription;
    public int myHealth;
    public bool unrestrictedMovement; //TODO ADD
    public int attackDice;
    public int defenseDice;
    public int physicalResistance;
    public int magicalResistance;
    public int pureResistance;
    public int goldDropped;
    public int goldRandomOffset;
    public int monsterCost;
    public int respawnTimer; //in turns
    public int monsterTier;
    public int range; //min 1
    public bool canCarryItems;
    public int abilityCount;
    public UnityEngine.Object[] abilities;
    GameObject canvasDirections;

    void Start()
    {
        myScript_BoardController = GameObject.Find("Board").GetComponent<script_BoardController>();
        canvasDirections = this.transform.Find("Canvas_Direction").gameObject;
        canvasDirections.SetActive(false);
    }

    public void SelectedUpdate()
    {
        if (myDirection == Enumerations.Direction.Choose)
        {
            myDirection = Enumerations.ChooseDirection(canvasDirections);
        }
    }
    //Mimic the movement system of the player so you get hit by traps.
    public void Move(GameObject toTile)
    {
        this.transform.position = toTile.transform.position;
        myTile = toTile;
    }
    //TODO Damage Types and armor (defense dice)
    public void Damaged(int damage, Enumerations.DamageType type)
    {
        myHealth -= damage;
        if (myHealth < 0)
            Die();
    }
    //TODO if player close fight
    //TODO trigger traps (refactor, players and monsters same components)


    private void Die()
    {
        //respawn countdown
        //character state respawning
        //drop gold
        //dead (respawn state)
    }
}
