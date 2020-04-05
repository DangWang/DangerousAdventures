using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class scr_Player : NetworkBehaviour
{
    private GameObject activeMenu, monsterMenu, trapMenu, inventoryHUD, artifactMenu;
    private int uid = 0; //unique id for each and every object that I spawn
    public int artifactCap = 3;
    public int artifactCount;

    public GameObject[] artifactTypes;
    
    public int monsterCap;
    public List<CreationPoint> monsterPoints = new List<CreationPoint>();
    public List<GameObject> monsters = new List<GameObject>();
    public GameObject[] monsterTypes;

    private GameObject myCanvas;
    public int numberOfMonsters;
    public GameObject selected;
    public GameObject selectedArtifactType;
    public GameObject selectedMonsterType;
    public GameObject selectedTrapType;

    public int sharedMovementDice;
    public int sharedRemainingMovement;
    public int trapCap = 5;
    public int trapCost;
    public List<GameObject> traps = new List<GameObject>();
    public GameObject[] trapTypes;

    public GameObject previousSelected { get; set; }

    public GameObject altSelected { get; set; }

    public bool beginMyTurn;

    [SyncVar]
    public bool isMyTurn;

    [SyncVar]
    public int connID;


    private void Start()
    {
        myCanvas = transform.Find("Canvas_World").gameObject;
        monsterMenu = transform.Find("Canvas_World/MonsterMenu").gameObject;
        activeMenu = transform.Find("Canvas_World/ActiveMenu").gameObject;
        trapMenu = transform.Find("Canvas_World/TrapMenu").gameObject;
        artifactMenu = transform.Find("Canvas_World/ArtifactMenu").gameObject;
        myCanvas.SetActive(false);
        AddMonsterPoints(10, 0);
    }


    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyUp(KeyCode.Tab))
                foreach (GameObject monster in GameObject.FindObjectsOfType(typeof(GameObject)))
                {
                    if(monster.tag == "Monster")
                        monster.GetComponent<scr_CombatController>().ToggleHealthbar();
                }

            if (beginMyTurn)
            {
                print("Player: " + name + " is beginning his turn.");
                myCanvas.SetActive(true);
                sharedRemainingMovement = Utilities.RollDice(sharedMovementDice);
                foreach (var monster in monsters)
                {
                    //remove dead monsters
                    if (monster == null)
                    {
                        monsters.Remove(null);
                        continue;
                    }

                    monster.GetComponent<scr_Monster>().OnStartTurn();
                }

                sharedRemainingMovement = Utilities.RollDice(sharedMovementDice);
                UpdateMP();
                beginMyTurn = false;
            }
            else if (isMyTurn)
            {
                //End my turn
                if (Input.GetKeyUp(KeyCode.N))
                {
                    EndTurn();
                    return;
                }

                if (Input.GetMouseButtonDown(1)) altSelected = script_SelectObject.ReturnAlternateClick();
                if (previousSelected != null && previousSelected.tag == "Monster" && selected != previousSelected)
                    previousSelected.GetComponent<scr_Monster>().Unselected();
                if (selected != null && selected.tag == "Monster" && monsters.Contains(selected))
                {
                    selected.GetComponent<scr_Monster>().SelectedUpdate();
                    selectedArtifactType = null;
                    selectedMonsterType = null;
                    selectedTrapType = null;
                    //Update selected monster (act like a player token).
                }

                //Place/Spawn monsters
                if (Input.GetKeyUp(KeyCode.O)) 
                    ToggleMonsterMenu();
                if (Input.GetKeyUp(KeyCode.Space) && selectedMonsterType != null)
                    if (selected.tag == selectedMonsterType.GetComponent<scr_Monster>().spawnTag
                        && selected.GetComponent<script_Tile>().occupied == false)
                        if (monsterCap >= selectedMonsterType.GetComponent<scr_Monster>().monsterCost)
                        {
                            monsterPoints.RemoveRange(0, selectedMonsterType.GetComponent<scr_Monster>().monsterCost);
                            monsterCap -= selectedMonsterType.GetComponent<scr_Monster>().monsterCost;
                            PlaceMonster(selectedMonsterType, selected);
                        }

                //Place Traps TODO change it to the same system as monsters
                if (Input.GetKeyUp(KeyCode.T)) 
                    ToggleTrapMenu();
                if (Input.GetMouseButtonUp(1) && selectedTrapType != null)
                {
                    altSelected = script_SelectObject.ReturnAlternateClick();
                    if (altSelected != null)
                    {
                        var tileScript = altSelected.GetComponent<script_Tile>();
                        //Remove existing trap
                        if (tileScript.myTrap != null)
                        {
                            traps.Remove(tileScript.myTrap);
                            trapCost -= tileScript.myTrap.GetComponent<script_Trap>().trapCost;
                            CmdDestroyTrap(altSelected.name);
                            //trapPoints.text = (trapCap - trapCost).ToString();
                        }
                        //Place trap
                        else
                        {
                            if (trapCap >= selectedTrapType.GetComponent<script_Trap>().trapCost + trapCost)
                            {
                                PlaceTrap(selectedTrapType, altSelected);
                                trapCost += selectedTrapType.GetComponent<script_Trap>().trapCost;
                                //trapPoints.text = (trapCap - trapCost).ToString();
                            }
                        }
                    }
                }

                //Place artifacts
                if (Input.GetKeyUp(KeyCode.U)) 
                    ToggleArtifactMenu();
                if (Input.GetMouseButtonUp(1) && selectedArtifactType != null)
                {
                    altSelected = script_SelectObject.ReturnAlternateClick();
                    if (altSelected != null)
                    {
                        var tileScript = altSelected.GetComponent<script_Tile>();
                        //Remove items
                        if (tileScript.items.Count != 0)
                        {
                        }
                        //Place trap
                        else
                        {
                            if (artifactCap >= 1 + artifactCount)
                            {
                                //artifac.Add(PlaceTrap(selectedTrapType, altSelected, tileScript));
                                artifactCount += 1;
                                PlaceArtifact(selectedArtifactType, altSelected, tileScript);
                                //trapPoints.text = (trapCap - trapCost).ToString();
                            }
                        }
                    }
                }

                //Inventory logic
                if (Input.GetKeyUp(KeyCode.I)) ToggleInventory();
            }
        }
    }


    public void EndTurn()
    {
        foreach (var monster in monsters)
        {
            if (monster == null) monsters.Remove(null);
            monster.GetComponent<scr_AffectedBy>().CmdOnEndTurn();
        }

        if (selected != null && selected.tag == "Monster") 
            selected.GetComponent<scr_Monster>().Unselected();
        myCanvas.SetActive(false);
        print("I SAID NEXT!");
        CmdEndTurn();
    }


    [Command]
    void CmdEndTurn()
    {
        GameObject.Find("GM").GetComponent<script_GameManager>().RpcNextTurn(name);
    }


    [Command]
    public void CmdMonsterDied(string monster)
    {
        RpcMonsterDied(monster);
    }

    [ClientRpc]
    public void RpcMonsterDied(string monsterS)
    {
        GameObject monster = GameObject.Find(monsterS);
        monster.GetComponent<scr_Monster>().Die();
        monsters.Remove(monster);
    }


    public void ToggleMonsterMenu()
    {
        if (monsterMenu.activeInHierarchy == false)
            monsterMenu.SetActive(true);
        else
            monsterMenu.SetActive(false);
    }


    public void ToggleActiveMenu()
    {
        if (activeMenu.activeInHierarchy == false)
            activeMenu.SetActive(true);
        else
            activeMenu.SetActive(false);
    }


    public void ToggleInventory()
    {
        if (inventoryHUD.activeInHierarchy == false)
        {
            inventoryHUD.SetActive(true);
            if (selected.tag == "Monster" && selected.GetComponent<scr_Monster>().hasInventory)
            {
                //change the inventory elements logic
            }
        }
        else
        {
            inventoryHUD.SetActive(false);
        }
    }


    public void ToggleTrapMenu()
    {
        if (trapMenu.activeInHierarchy == false)
            trapMenu.SetActive(true);
        else
            trapMenu.SetActive(false);
    }


    public void ToggleArtifactMenu()
    {
        if (artifactMenu.activeInHierarchy == false)
            artifactMenu.SetActive(true);
        else
            artifactMenu.SetActive(false);
    }


    public void SelectMonster(int i) //TODO Enumeration instead of integer
    {
        selectedMonsterType = monsterTypes[i];
        selectedTrapType = null;
        selectedArtifactType = null;
    }


    public void SelectTrap(int i) //TODO Enumeration instead of integer
    {
        selectedTrapType = trapTypes[i];
        selectedArtifactType = null;
        selectedMonsterType = null;
        transform.Find("Canvas_World/TrapMenu").GetComponent<script_UITrap>().ChangeTrap(trapTypes[i]);
    }


    public void SelectArtifact(int i) //TODO Enumeration instead of integer
    {
        selectedArtifactType = artifactTypes[i];
        selectedMonsterType = null;
        selectedTrapType = null;
    }

    private void PlaceMonster(GameObject monster, GameObject tile)
    {
        print("Caaaaleeeeeeeleleleleeld");
        CmdSpawnMonster(monster.name, name + uid, tile.name, name);
        uid++;
    }

    [Command]
    private void CmdSpawnMonster(string monsterName, string name, string tileName, string playerName)
    {
        GameObject prefab = null;
        var tile = GameObject.Find(tileName);
        var tileScript = tile.GetComponent<script_Tile>();

        foreach (var g in NetworkManager.singleton.spawnPrefabs)
            if (g.name == monsterName)
                prefab = g;
        if(prefab == null)
            Debug.LogError("Error. Prefab not found!");
        var newMonster = Instantiate(prefab);
        newMonster.name = monsterName + name;
        newMonster.transform.position = tile.transform.position;
        newMonster.GetComponent<scr_Movement>().myTile = tile;
        tileScript.occupied = true;
        tileScript.occupier = newMonster;
        newMonster.GetComponent<scr_NetworkedObject>().tempName = (newMonster.name);
        NetworkServer.Spawn(newMonster, GameObject.Find("NetworkManager")
            .GetComponent<scr_NetworkManager>().pInfo[connID].connection);
        RpcAddMonsterToList(newMonster.name, playerName);
    }

    [ClientRpc]
    private void RpcAddMonsterToList(string monsterName, string playerName)
    {
        var player = GameObject.Find(playerName);
        var monster = GameObject.Find(monsterName);
        if (monster == null)
        {
            foreach (var g in GameObject.FindGameObjectsWithTag("Monster"))
            {
                if (g.GetComponent<scr_NetworkedObject>().tempName == monsterName)
                {
                    monster = g;
                }
            }
        }
        player.GetComponent<scr_Player>().monsters.Add(monster);
        player.GetComponent<scr_Player>().monsters[numberOfMonsters].GetComponent<scr_Movement>().myTile = selected;
        player.GetComponent<scr_Player>().monsters[numberOfMonsters].GetComponent<scr_Movement>().parentScript = this;
        player.GetComponent<scr_Player>().monsters[numberOfMonsters].GetComponent<scr_CombatController>().parentScript =
            this;
        player.GetComponent<scr_Player>().monsters[numberOfMonsters].GetComponent<scr_Monster>().parentScript = this;
        player.GetComponent<scr_Player>().monsters[numberOfMonsters].GetComponent<scr_Movement>().OnStartTurn();
        player.GetComponent<scr_Player>().monsters[numberOfMonsters].GetComponent<scr_CombatController>().OnStartTurn();
        player.GetComponent<scr_Player>().numberOfMonsters++;
    }

    private void PlaceTrap(GameObject trap, GameObject tile)
    {
        CmdSpawnTrap(trap.name, name + uid, tile.name, name);
        uid++;
    }

    [Command]
    private void CmdSpawnTrap(string trapName, string name, string tileName, string playerName)
    {
        GameObject prefab = null;
        var tile = GameObject.Find(tileName);
        var tileScript = tile.GetComponent<script_Tile>();

        foreach (var g in NetworkManager.singleton.spawnPrefabs)
            if (g.name == trapName)
                prefab = g;
        var newTrap = Instantiate(prefab);
        newTrap.name = trapName + name;
        newTrap.transform.position = tile.transform.position;
        newTrap.GetComponent<scr_NetworkedObject>().tempName = (newTrap.name);
        NetworkServer.Spawn(newTrap);
        tile.GetComponent<script_Tile>().myTrap = newTrap;

        //Traps don't have client authority.
        // newTrap.GetComponent<NetworkIdentity>().AssignClientAuthority(GameObject.Find("NetworkManager")
        //     .GetComponent<scr_NetworkManager>().pInfo[connID].connection);
        RpcAddTrapToList(newTrap.name, playerName);
    }
    
    private void CmdDestroyTrap(string tileName)
    {
        var tile = GameObject.Find(tileName);
        var tileScript = tile.GetComponent<script_Tile>();
        
        Destroy(tileScript.myTrap);
        tileScript.myTrap = null;
    }

    [ClientRpc]
    private void RpcAddTrapToList(string trapName, string playerName)
    {
        var player = GameObject.Find(playerName);
        var trap = GameObject.Find(trapName);
        player.GetComponent<scr_Player>().traps.Add(trap);
    }

    private GameObject PlaceArtifact(GameObject artifact, GameObject tile, script_Tile tileScript)
    {
        CmdSpawnArtifact(artifact.name, name + artifactCount);
        GameObject.Find(artifact.name + name + artifactCount).transform.position = tile.transform.position;
        tileScript.items.Add(GameObject.Find(artifact.name + name + artifactCount));
        return GameObject.Find(artifact.name + name + artifactCount);
    }

    [Command]
    private void CmdSpawnArtifact(string artifactName, string name)
    {
        GameObject prefab = null;
        foreach (var g in NetworkManager.singleton.spawnPrefabs)
            if (g.name == artifactName)
                prefab = g;
        var newArtifact = Instantiate(prefab);
        newArtifact.name = artifactName + name;
        newArtifact.GetComponent<scr_NetworkedObject>().tempName = (newArtifact.name);
        NetworkServer.Spawn(newArtifact);
    }


    //Monster point related functions.
    //Used when monsters die.
    public void AddMonsterPoints(int count, int cooldown)
    {
        for (var i = 0; i < count; i++) monsterPoints.Add(new CreationPoint(cooldown));
    }


    //Updates the cooldowns of the creation(monster) ponts.
    private void UpdateMP()
    {
        print("called");
        monsterCap = 0;
        foreach (var cp in monsterPoints)
        {
            if (cp.isActive)
                monsterCap++;
            cp.UpdateCD();
        }
    }
}


//This class represents the monster and trap points for the DM.
public class CreationPoint
{
    private int cdRemaining;
    public bool isActive;

    public CreationPoint(int cooldown)
    {
        cdRemaining = cooldown;
        if (cdRemaining == 0)
            isActive = true;
        else
            isActive = false;
    }

    public void UpdateCD()
    {
        if (cdRemaining > 0) cdRemaining--;
        if (cdRemaining == 0)
            isActive = true;
        else
            isActive = false;
    }
}