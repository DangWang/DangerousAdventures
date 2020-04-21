using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class scr_Monster : NetworkBehaviour
{
    public Object[] abilities;
    public bool abilityCasted;
    public int abilityCount;
    public List<Ability.AbilityStruct> activeAbilities = new List<Ability.AbilityStruct>();

    private scr_AffectedBy _affectedBy;

    //TODO add list of active abilities over time
    public bool casting;
    private scr_CombatController _combatController;
    public int goldDropped;
    public int goldRandomOffset;
    public bool hasAbilities;
    public bool hasInventory;
    public int monsterCost;
    public string monsterDescription;
    public int monsterTier;
    private scr_Movement _movementController;
    private script_BoardController _myScript_BoardController;
    public scr_Player parentScript;
    public int respawnTimer; //in turns
    public string spawnTag;

    private void Start()
    {
        _combatController = GetComponent<scr_CombatController>();
        _movementController = GetComponent<scr_Movement>();
        _affectedBy = GetComponent<scr_AffectedBy>();
        foreach (var ab in abilities)
        {
            script_AbilityCaster.PrepareAbility(ab.name);
        }
        //parentScript = GetComponent<scr_Player>();
    }

    public void OnStartTurn()
    {
        //reset movement controller
        GetComponent<scr_Movement>().OnStartTurn();
        //reset combat controller
        GetComponent<scr_CombatController>().OnStartTurn();
        abilityCasted = false;
        foreach (var ab in activeAbilities)
        {
            print(gameObject.name + " is casting an active ability");
            script_AbilityCaster.CastAbility(ab, gameObject, gameObject);
            activeAbilities.Remove(ab);
        }
    }

    public void SelectedUpdate()
    {
        if (hasAuthority)
        {
            if (!_affectedBy.isStunned)
            {
                if (Input.GetKeyUp(KeyCode.C) && !abilityCasted)
                {
                    casting = !casting;
                    if (abilities.Length == 1 && script_AbilityCaster.allAbilities[abilities[0].name].targetPoint == Enumerations.TargetPoint.Self)
                    {
                        script_AbilityCaster.CastAbility(abilities[0].name, gameObject, gameObject);
                        abilityCasted = true;
                        casting = false;
                    }
                }

                if (!casting)
                {
                    _movementController.OnSelectedUpdate(); //movement logic
                    _combatController.OnSelectedUpdate(); //combat logic

                    //open door if in range
                    if (Input.GetMouseButtonUp(1))
                    {
                        if ((parentScript.altSelected.tag == "Door" || parentScript.altSelected.tag == "OpenDoor")
                            && script_BoardController.GetTileDistance(gameObject, parentScript.selected) <= 1)
                        {
                            CmdToggleDoor(parentScript.altSelected.name);
                        }
                    }
                }
                else
                {
                    if (Input.GetMouseButtonUp(1) && abilities.Length == 1 && !abilityCasted)
                    {
                        script_AbilityCaster.CastAbility(abilities[0].name, parentScript.altSelected, gameObject);
                        abilityCasted = true;
                    }
                }
            }
        }
    }

    [Command]
    public void CmdToggleDoor(string doorName)
    {
        GameObject.Find(doorName).GetComponent<scr_Door>().RpcToggleDoor();
    }

    public void Unselected()
    {
        _movementController.Unselected();
    }


    public void Die()
    {
        //respawn countdown
        //character state respawning
        //drop gold
        //dead (respawn state)
        print(name + " is dead.");
        _movementController.myTile.GetComponent<script_Tile>().occupied = false;
        parentScript.AddMonsterPoints(monsterCost, respawnTimer);
        //Destroy(gameObject);
        if (hasAuthority)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}