using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror
{
    public class script_DMController : MonoBehaviour, ISelector, IPlayer
    {
        private List<GameObject> _allowedMovement = new List<GameObject>();
        public int availableMovement;
        private Text _availableMovementField, _trapPoints, _monsterPoints;
        public int monsterCap = 10;
        public int monsterCost;
        public List<GameObject> monsters = new List<GameObject>();
        public int movementDice;
        private Canvas _myCanvas;
        private script_MonsterController _myScript_MonsterController;
        private script_MovementParser _myScript_MovementParser;
        private script_SelectObject _myScript_SelectObject;
        private GameObject _myTile;
        public int numberOfMonsters;
        public GameObject selectedTrapType, selectedMonsterType;
        public int trapCap = 5;
        public int trapCost;
        private GameObject _trapMenu, _activeMenu, _monsterMenu;
        public List<GameObject> traps = new List<GameObject>();

        public GameObject[] trapTypes, monsterTypes;

        public GameObject altSelected { get; set; }

        public bool beginMyTurn { get; set; }

        public bool isMyTurn { get; set; }

        public void EndTurn()
        {
            foreach (var monster in monsters)
                if (monster == null)
                    monsters.Remove(null);
                else
                    monster.GetComponent<script_MonsterController>().Unselected();
            if (_allowedMovement.Count > 0)
                try
                {
                    script_MovementPainter.RemoveAllowedMovementMarker(_allowedMovement);
                }
                catch
                {
                }

            _myCanvas.gameObject.SetActive(false);
            //script_GameManager.CmdNextTurn();
            //script_MovementPainter.RemoveAllowedMovementMarker(allowedMovement);
        }

        public GameObject previousSelected { get; set; }

        public GameObject selected { get; set; }

        private void Start()
        {
            _myScript_SelectObject = GetComponentInChildren<script_SelectObject>();
            _myCanvas = transform.Find("Canvas_DM").GetComponent<Canvas>();
            _trapMenu = transform.Find("Canvas_DM/TrapMenu").gameObject;
            _monsterMenu = transform.Find("Canvas_DM/MonsterMenu").gameObject;
            _activeMenu = transform.Find("Canvas_DM/ActiveHUD").gameObject;
            _availableMovementField = transform.Find("Canvas_DM/AvailableMovement/Text").GetComponent<Text>();
            _trapPoints = transform.Find("Canvas_DM/DMHUD/TrapPoints/Text").GetComponent<Text>();
            _monsterPoints = transform.Find("Canvas_DM/DMHUD/MonsterPoints/Text").GetComponent<Text>();
            _monsterPoints.text = (monsterCap - monsterCost).ToString();
            _trapPoints.text = (trapCap - trapCost).ToString();
            _myCanvas.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Tab))
                foreach (var monster in monsters)
                    monster.GetComponent<script_MonsterController>().ToggleHealthbar();
            if (beginMyTurn)
            {
                print("Begiiiin!!");
                _monsterPoints.text = (monsterCap - monsterCost).ToString();
                _trapPoints.text = (trapCap - trapCost).ToString();
                _myCanvas.gameObject.SetActive(true);
                //TODO Remove dead monsters
                //TODO for every monster reset movement
                foreach (var monster in monsters)
                {
                    if (monster == null)
                    {
                        monsters.Remove(null);
                        continue;
                    }

                    monster.GetComponent<script_MonsterController>().myRemainingMovement = 8;
                    monster.GetComponent<script_MonsterController>().myDirection = Enumerations.Direction.Choose;
                    monster.GetComponent<script_MonsterController>().attackedThisTurn = false;
                }

                availableMovement = Utilities.RollDice(movementDice);
                _availableMovementField.text = "Available:" + availableMovement;
                beginMyTurn = false;
            }
            else if (isMyTurn)
            {
                if (script_GameManager.preparationPhase)
                {
                    if (Input.GetKeyUp(KeyCode.I)) ToggleTrapMenu();
                    if (Input.GetKeyUp(KeyCode.N)) EndPreparation();
                    if (Input.GetMouseButtonUp(1))
                    {
                        altSelected = script_SelectObject.ReturnAlternateClick();
                        if (altSelected != null && CheckTrapSquareEligibility(altSelected)
                        ) //TODO Selected.tag in allowed squares for trap
                        {
                            var tileScript = altSelected.GetComponent<script_Tile>();
                            if (tileScript.myTrap != null)
                            {
                                //TODO Remove from my list
                                trapCost -= tileScript.myTrap.GetComponent<script_Trap>().trapCost;
                                Destroy(tileScript.myTrap);
                                tileScript.myTrap = null;
                                _trapPoints.text = (trapCap - trapCost).ToString();
                            }
                            else
                            {
                                if (trapCap >= selectedTrapType.GetComponent<script_Trap>().trapCost + trapCost)
                                {
                                    traps.Add(PlaceTrap(selectedTrapType, altSelected, tileScript));
                                    trapCost += selectedTrapType.GetComponent<script_Trap>().trapCost;
                                    _trapPoints.text = (trapCap - trapCost).ToString();
                                }
                            }
                        }
                    }

                    if (Input.GetMouseButtonUp(2))
                    {
                        altSelected = script_SelectObject.ReturnAlternateClick();
                        if (altSelected != null && CheckTrapSquareEligibility(altSelected)
                        ) //TODO Selected.tag in allowed squares for trap
                        {
                            var tileScript = altSelected.GetComponent<script_Tile>();
                            if (tileScript.myTrap != null)
                            {
                                //TODO Remove from my list
                                trapCost -= tileScript.myTrap.GetComponent<script_Trap>().trapCost;
                                Destroy(tileScript.myTrap);
                                tileScript.myTrap = null;
                                _trapPoints.text = (trapCap - trapCost).ToString();
                            }
                            else
                            {
                                if (trapCap >= selectedTrapType.GetComponent<script_Trap>().trapCost + trapCost)
                                {
                                    traps.Add(PlaceTrap(selectedTrapType, altSelected, tileScript));
                                    trapCost += selectedTrapType.GetComponent<script_Trap>().trapCost;
                                    _trapPoints.text = (trapCap - trapCost).ToString();
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Input.GetKeyUp(KeyCode.N))
                    {
                        EndTurn();
                        return;
                    }

                    if (Input.GetKeyUp(KeyCode.I)) ToggleMonsterMenu();
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        //Check if the tile is occupied
                        _myTile = selected;
                        if (selected.tag == "MonsterSpawn" && _myTile.GetComponent<script_Tile>().occupied == false)
                            if (monsterCap >= monsterCost + selectedMonsterType.GetComponent<script_MonsterController>()
                                    .monsterCost)
                            {
                                monsterCost += selectedMonsterType.GetComponent<script_MonsterController>().monsterCost;
                                monsters.Add(Instantiate(selectedMonsterType, _myTile.transform.position,
                                    Quaternion.identity));
                                monsters[numberOfMonsters].GetComponent<script_MonsterController>().myTile = _myTile;
                                numberOfMonsters++;
                                _monsterPoints.text = (monsterCap - monsterCost).ToString();
                                _myTile.GetComponent<script_Tile>().occupied = true;
                                _myTile.GetComponent<script_Tile>().occupier = monsters[numberOfMonsters - 1];
                            }
                    }

                    if (previousSelected != null && previousSelected.tag == "Monster" && selected != previousSelected)
                    {
                        try
                        {
                            script_MovementPainter.RemoveAllowedMovementMarker(_allowedMovement);
                        }
                        catch
                        {
                        }

                        previousSelected.GetComponent<script_MonsterController>().Unselected();
                    }

                    if (selected != null && selected.tag == "Monster")
                    {
                        _myScript_MonsterController = selected.GetComponent<script_MonsterController>();
                        _myScript_MovementParser = selected.GetComponent<script_MovementParser>();
                        _myScript_MonsterController.SelectedUpdate();
                        if (_myScript_MonsterController.myDirection != Enumerations.Direction.Choose)
                            try
                            {
                                script_MovementPainter.RemoveAllowedMovementMarker(_allowedMovement);
                            }
                            catch
                            {
                            }

                        if (_myScript_MonsterController.myRemainingMovement < availableMovement)
                            _allowedMovement = _myScript_MovementParser.GetAllowedMovement(selected,
                                ref _myScript_MonsterController.myDirection,
                                _myScript_MonsterController.myRemainingMovement);
                        else
                            _allowedMovement = _myScript_MovementParser.GetAllowedMovement(selected,
                                ref _myScript_MonsterController.myDirection, availableMovement);
                        //                   print(myScript_MonsterController.myRemainingMovement);
                        script_MovementPainter.AddAllowedMovementMarker(_allowedMovement);
                        if (Input.GetMouseButtonUp(1))
                            if (_allowedMovement.Count != 0)
                            {
                                altSelected = script_SelectObject.ReturnAlternateClick();

                                if (_allowedMovement.Contains(altSelected))
                                    if (script_BoardController.GetTileDistance(selected, altSelected) <=
                                        availableMovement)
                                    {
                                        availableMovement -=
                                            _myScript_MonsterController.Move(altSelected, _allowedMovement);
                                        _availableMovementField.text = "Available:" + availableMovement;
                                    }
                            }
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        if (selected != null && selected.tag == "Player" && previousSelected.tag != null &&
                            previousSelected.tag == "Monster" &&
                            script_BoardController.GetTileDistance(previousSelected, selected) <=
                            previousSelected.GetComponent<script_MonsterController>().range)
                        {
                            print("Called");
                            previousSelected.GetComponent<script_MonsterController>()
                                .Attack(selected.GetComponent<ICombatant>(), Enumerations.DamageType.Physical);
                        }

                        foreach (var monster in monsters)
                        {
                            _myScript_MonsterController = monster.GetComponent<script_MonsterController>();
                            if ((selected.tag == "Door" || selected.tag == "OpenDoor") &&
                                script_BoardController.GetTileDistance(_myScript_MonsterController.myTile, selected) <=
                                1)
                                //                                selected.GetComponent<scr_Door>().ToggleDoor();
                                break;
                        }
                    }

                    //TODO this should get moved to the MonsterController
                    if (Input.GetKeyUp(KeyCode.C))
                        if (selected != null && selected.tag == "Player" && previousSelected.tag != null &&
                            previousSelected.tag == "Monster")
                            script_AbilityCaster.CastAbility("heal", selected, gameObject);
                }
            }
        }

        public void OnSelectMonster()
        {
            script_MovementPainter.RemoveAllowedMovementMarker(_allowedMovement);
            _allowedMovement = _myScript_MovementParser.GetAllowedMovement(selected,
                ref _myScript_MonsterController.myDirection, _myScript_MonsterController.myRemainingMovement);
            script_MovementPainter.AddAllowedMovementMarker(_allowedMovement);
        }

        public GameObject ReturnPlayerObject()
        {
            return gameObject;
        }

        public void EndPreparation()
        {
            //script_GameManager.EndPreparationPhase();
        }

        private GameObject PlaceTrap(GameObject trap, GameObject tile, script_Tile tileScript)
        {
            var newTrap = Instantiate(trap, tile.transform.position, Quaternion.identity);
            tileScript.myTrap = newTrap;
            return newTrap;
        }

        //TODO ALSO CHECK IF IT BLOCKS SOMETHING DYNAMICALLY
        private bool CheckTrapSquareEligibility(GameObject tile)
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

        public void SelectTrap(int i) //TODO Enumeration instead of integer
        {
            selectedTrapType = trapTypes[i];
            transform.Find("Canvas_DM/TrapMenu").GetComponent<script_UITrap>().ChangeTrap(trapTypes[i]);
        }

        public void SelectMonster(int i) //TODO Enumeration instead of integer
        {
            selectedMonsterType = monsterTypes[i];
        }


        public void ToggleTrapMenu()
        {
            if (_trapMenu.activeInHierarchy == false)
                _trapMenu.SetActive(true);
            else
                _trapMenu.SetActive(false);
        }

        public void ToggleMonsterMenu()
        {
            if (_monsterMenu.activeInHierarchy == false)
                _monsterMenu.SetActive(true);
            else
                _monsterMenu.SetActive(false);
        }

        public void ToggleActiveMenu()
        {
            if (_activeMenu.activeInHierarchy == false)
                _activeMenu.SetActive(true);
            else
                _activeMenu.SetActive(false);
        }
    }
}