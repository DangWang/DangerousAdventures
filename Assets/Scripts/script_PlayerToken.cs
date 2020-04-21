using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror
{
    public class script_PlayerToken : MonoBehaviour, ISelector, IPlayer, ICombatant
    {
        private List<GameObject> _allowedMovement = new List<GameObject>();
        public int attackDice;
        private bool _attackedThisTurn;
        public int availableMovement;
        private Text _availableMovementField;

        private Canvas _canvasLocal, _canvasWorld;
        public int defenceDice;
        private bool _hasMoved;
        private Slider _healthBar;
        private Text _hitpointsField;
        private GameObject _inventoryHUD, _markerStun;
        public int lives = 3;
        public int magicalAttackDice;
        public int magicalDefenceDice;
        public int movementDice;
        public Enumerations.Direction myDirection = Enumerations.Direction.Choose;
        private scr_AffectedBy _myScript_AffectedBy;
        private script_BoardController _myScript_BoardController;
        private script_MovementParser _myScript_MovementParser;
        private GameObject _neighbor, _myTile, _directions;
        public bool placed;
        public int pureAttackDice;
        public int range;

        public GameObject altSelected { get; set; }

        public int myHitpoints { get; set; }

        //TODO this believes that all attacks are of the same type
        public void Attack(ICombatant target, Enumerations.DamageType damageType)
        {
            var dice = new Enumerations.AttackDie[attackDice];
            var set = new int[6] { 0, 0, 1, 1, 2, 3 };
            print("Attacking");
            for (var i = 0; i < attackDice; i++)
            {
                var j = set[Utilities.RollDice(1) - 1];
                dice[i] = (Enumerations.AttackDie)j;
                print("Attack die " + i + ":" + dice[i]);
            }

            target.Defend(dice, damageType, this);

            _attackedThisTurn = true;
        }

        //TODO Need to add magical defense dice!!
        public void Defend(Enumerations.AttackDie[] die, Enumerations.DamageType damageType, ICombatant attacker)
        {
            print("Defending");
            Enumerations.DefenseDie[] defendDice;

            if (damageType == Enumerations.DamageType.Physical)
            {
                var set = new int[6] { 0, 0, 1, 1, 2, 3 };
                defendDice = new Enumerations.DefenseDie[defenceDice];
                for (var i = 0; i < defenceDice; i++)
                {
                    var j = set[Utilities.RollDice(1) - 1];
                    defendDice[i] = (Enumerations.DefenseDie)j;
                    print("Defense die " + i + ":" + defendDice[i]);
                }
            }
            else if (damageType == Enumerations.DamageType.Magical)
            {
                defendDice = new Enumerations.DefenseDie[magicalDefenceDice];
                for (var i = 0; i < magicalDefenceDice; i++)
                    defendDice[i] = (Enumerations.DefenseDie)Utilities.RollDice(1);
            }
            else
            {
                defendDice = new Enumerations.DefenseDie[0];
            }

            for (var i = 0; i < die.Length; i++)
                if (die[i] == Enumerations.AttackDie.Hit)
                {
                    for (var j = 0; j < defendDice.Length; j++)
                        if (defendDice[j] == Enumerations.DefenseDie.Block)
                        {
                            die[i] = Enumerations.AttackDie.Blank;
                            defendDice[j] = Enumerations.DefenseDie.Fail;
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.Evade)
                        {
                            //play miss sound
                            die[i] = Enumerations.AttackDie.Blank;
                            defendDice[j] = Enumerations.DefenseDie.Fail;
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.Retaliate)
                        {
                            die[i] = Enumerations.AttackDie.Blank;
                            defendDice[j] = Enumerations.DefenseDie.Fail;
                            if (attacker != null)
                                attacker.TakeDamage(1);
                            break;
                            //Attacker take damage.
                        }
                        else
                        {
                            TakeDamage(1);
                        }

                    if (die[i] == Enumerations.AttackDie.Hit) TakeDamage(1);
                }
                else if (die[i] == Enumerations.AttackDie.Critical)
                {
                    for (var j = 0; j < defendDice.Length; j++)
                        if (defendDice[j] == Enumerations.DefenseDie.Block)
                        {
                            die[i] = Enumerations.AttackDie.Blank;
                            defendDice[j] = Enumerations.DefenseDie.Fail;
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.Evade)
                        {
                            //play miss sound
                            die[i] = Enumerations.AttackDie.Blank;
                            defendDice[j] = Enumerations.DefenseDie.Fail;
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.Retaliate)
                        {
                            die[i] = Enumerations.AttackDie.Blank;
                            defendDice[j] = Enumerations.DefenseDie.Fail;
                            if (attacker != null)
                                attacker.TakeDamage(2);
                            break;
                            //Attacker take damage.
                        }
                        else
                        {
                            TakeDamage(2);
                        }

                    if (die[i] == Enumerations.AttackDie.Critical) TakeDamage(2);
                }
                else if (die[i] == Enumerations.AttackDie.Pierce)
                {
                    for (var j = 0; j < defendDice.Length; j++)
                        if (defendDice[j] == Enumerations.DefenseDie.Block)
                        {
                            die[i] = Enumerations.AttackDie.Hit;
                            defendDice[j] = Enumerations.DefenseDie.Fail;
                            TakeDamage(1);
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.Evade)
                        {
                            //play miss sound
                            die[i] = Enumerations.AttackDie.Blank;
                            defendDice[j] = Enumerations.DefenseDie.Fail;
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.Retaliate)
                        {
                            die[i] = Enumerations.AttackDie.Blank;
                            defendDice[j] = Enumerations.DefenseDie.Fail;
                            if (attacker != null)
                                attacker.TakeDamage(1);
                            break;
                            //Attacker take damage.
                        }
                        else
                        {
                            TakeDamage(1);
                        }

                    if (die[i] == Enumerations.AttackDie.Hit) TakeDamage(1);
                }

            _hitpointsField.text = myHitpoints.ToString();
        }

        public void TakeDamage(int damage)
        {
            myHitpoints -= damage;
            _healthBar.value = myHitpoints;
            _hitpointsField.text = myHitpoints.ToString();
            print(myHitpoints);
            if (damage == 1)
            {
                //play hit sound
            }
            else if (damage == 2)
            {
                //play crit sound
            }

            if (myHitpoints <= 0) Die();
        }

        public bool beginMyTurn { get; set; }

        public bool isMyTurn { get; set; }

        public void EndTurn()
        {
            _myScript_AffectedBy.OnEndTurn();
            _canvasLocal.gameObject.SetActive(false);
            _canvasWorld.gameObject.SetActive(false);
            //script_GameManager.CmdNextTurn();
            if (_allowedMovement.Count > 0)
                script_MovementPainter.RemoveAllowedMovementMarker(_allowedMovement);
            //script_MovementPainter.RemoveAllowedMovementMarker(allowedMovement);
        }

        public GameObject previousSelected { get; set; }

        public GameObject selected { get; set; }

        private void Start()
        {
            _myScript_BoardController = GameObject.Find("Board").GetComponent<script_BoardController>();
            _myScript_MovementParser = GetComponent<script_MovementParser>();
            _myScript_AffectedBy = GetComponent<scr_AffectedBy>();
            _canvasLocal = transform.Find("Canvas_Local").GetComponent<Canvas>();
            _canvasWorld = transform.Find("Canvas_World").GetComponent<Canvas>();
            _directions = transform.Find("Canvas_Local/Directions").gameObject;
            _availableMovementField = transform.Find("Canvas_World/AdventurerHUD/Movement/Text").GetComponent<Text>();
            _inventoryHUD = transform.Find("Canvas_World/InventoryHUD").gameObject;
            _hitpointsField = transform.Find("Canvas_World/AdventurerHUD/Health/Text").GetComponent<Text>();
            _healthBar = transform.Find("Canvas_Local/Healthbar").GetComponent<Slider>();
            _markerStun = transform.Find("Canvas_Local/Marker_Stun").gameObject;
            myHitpoints = 10;
            _hitpointsField.text = myHitpoints.ToString();
            _healthBar.value = myHitpoints;
            _healthBar.maxValue = myHitpoints;
            _healthBar.gameObject.SetActive(false);
            _markerStun.SetActive(false);

            _inventoryHUD.SetActive(false);
            _canvasLocal.gameObject.SetActive(false);
            _canvasWorld.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Tab)) _healthBar.gameObject.SetActive(!_healthBar.gameObject.activeInHierarchy);
            if (Input.GetKeyUp(KeyCode.N) && beginMyTurn == false && isMyTurn) EndTurn();
            if (beginMyTurn)
            {
                _canvasLocal.gameObject.SetActive(true);
                _canvasWorld.gameObject.SetActive(true);
                myDirection = Enumerations.Direction.Choose;
                availableMovement = Utilities.RollDice(movementDice);
                _availableMovementField.text = availableMovement.ToString();
                beginMyTurn = false;
                _attackedThisTurn = false;

                script_MovementPainter.RemoveAllowedMovementMarker(_allowedMovement);
                _allowedMovement =
                    _myScript_MovementParser.GetAllowedMovement(_myTile, ref myDirection, availableMovement);
                script_MovementPainter.AddAllowedMovementMarker(_allowedMovement);
            }
            else if (isMyTurn)
            {
                if (placed && _myScript_AffectedBy.isStunned == false)
                {
                    if (myDirection == Enumerations.Direction.Choose)
                    {
                        myDirection = Enumerations.ChooseDirection(_directions);
                        //script_MovementPainter.RemoveAllowedMovementMarker(allowedMovement);
                        _allowedMovement =
                            _myScript_MovementParser.GetAllowedMovement(_myTile, ref myDirection, availableMovement);
                        script_MovementPainter.AddAllowedMovementMarker(_allowedMovement);
                    }

                    if (Input.GetKeyUp(KeyCode.R) && _hasMoved == false) myDirection = Enumerations.Direction.Choose;

                    if (Input.GetMouseButtonUp(1))
                    {
                        altSelected = script_SelectObject.ReturnAlternateClick();

                        if (_allowedMovement.Contains(altSelected))
                            if (script_BoardController.GetTileDistance(_myTile, altSelected) <= availableMovement)
                                Move(altSelected, _allowedMovement);
                        script_MovementPainter.RemoveAllowedMovementMarker(_allowedMovement);
                        _allowedMovement =
                            _myScript_MovementParser.GetAllowedMovement(_myTile, ref myDirection, availableMovement);
                        script_MovementPainter.AddAllowedMovementMarker(_allowedMovement);
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        if (selected.tag == "Monster" &&
                            script_BoardController.GetTileDistance(_myTile, selected) <= range &&
                            _attackedThisTurn == false)
                            Attack(selected.GetComponent<ICombatant>(), Enumerations.DamageType.Physical);
                        if ((selected.tag == "Door" || selected.tag == "OpenDoor") &&
                            script_BoardController.GetTileDistance(_myTile, selected) <= 1)
                        {
                        }

                        //                            selected.GetComponent<scr_Door>().ToggleDoor();
                    }
                }

                if (Input.GetKeyUp(KeyCode.Space))
                    if (placed == false)
                    {
                        if (selected.tag == "PlayerSpawn")
                        {
                            transform.position = selected.transform.position;
                            _myTile = selected;
                            placed = true;
                        }
                        else
                        {
                            Debug.Log("Select an eligible spawn tile");
                        }
                    }
            }
        }

        //TODO don't allow the player to move to occupied tiles!
        public void Move(GameObject toTile, List<GameObject> allowedMovement)
        {
            GameObject tileTemp;
            var index = 0;
            bool activatedTrap;
            while (_myTile != toTile)
            {
                //            print(index);
                tileTemp = allowedMovement.ElementAt(index++);
                if (tileTemp.GetComponent<script_Tile>().occupied)
                {
                    myDirection = Enumerations.Direction.Choose;
                    break;
                }

                _hasMoved = true;
                _myTile.GetComponent<script_Tile>().occupied = false;
                _myTile.GetComponent<script_Tile>().occupier = null;
                _myTile = tileTemp;
                _myTile.GetComponent<script_Tile>().occupied = true;
                _myTile.GetComponent<script_Tile>().occupier = gameObject;
                transform.position = _myTile.transform.position;
                availableMovement--;
                _availableMovementField.text = availableMovement.ToString();
                //tile call method stepped on if not invisible
                activatedTrap = _myTile.GetComponent<script_Tile>().SteppedOn(gameObject);
                if (activatedTrap)
                    break;
            }
        }

        public GameObject ReturnPlayerObject()
        {
            return gameObject;
        }

        public void ToggleInventory()
        {
            if (_inventoryHUD.activeInHierarchy == false)
                _inventoryHUD.SetActive(true);
            else
                _inventoryHUD.SetActive(false);
        }

        private void Die()
        {
            if (placed)
            {
                _myTile.GetComponent<script_Tile>().occupied = false;
                print("Player died!");
                if (lives <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    lives--;
                    placed = false;
                }
            }
        }
    }
}