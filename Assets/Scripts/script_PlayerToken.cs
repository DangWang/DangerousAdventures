using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror
{
    public class script_PlayerToken : MonoBehaviour, ISelector, IPlayer, ICombatant
    {
        private List<GameObject> allowedMovement = new List<GameObject>();
        public int attackDice;
        private bool attackedThisTurn;
        public int availableMovement;
        private Text availableMovementField;

        private Canvas canvasLocal, canvasWorld;
        public int defenceDice;
        private bool hasMoved;
        private Slider healthBar;
        private Text hitpointsField;
        private GameObject inventoryHUD, markerStun;
        public int lives = 3;
        public int magicalAttackDice;
        public int magicalDefenceDice;
        public int movementDice;
        public Enumerations.Direction myDirection = Enumerations.Direction.Choose;
        private scr_AffectedBy myScript_AffectedBy;
        private script_BoardController myScript_BoardController;
        private script_MovementParser myScript_MovementParser;
        private GameObject neighbor, myTile, directions;
        public bool placed;
        public int pureAttackDice;
        public int range;

        public GameObject altSelected { get; set; }

        public int myHitpoints { get; set; }

        //TODO this believes that all attacks are of the same type
        public void Attack(ICombatant target, Enumerations.DamageType damageType)
        {
            var dice = new Enumerations.AttackDie[attackDice];
            var set = new int[6] {0, 0, 1, 1, 2, 3};
            print("Attacking");
            for (var i = 0; i < attackDice; i++)
            {
                var j = set[Utilities.RollDice(1) - 1];
                dice[i] = (Enumerations.AttackDie) j;
                print("Attack die " + i + ":" + dice[i]);
            }

            target.Defend(dice, damageType, this);

            attackedThisTurn = true;
        }

        //TODO Need to add magical defense dice!!
        public void Defend(Enumerations.AttackDie[] die, Enumerations.DamageType damageType, ICombatant attacker)
        {
            print("Defending");
            Enumerations.DefenseDie[] defendDice;

            if (damageType == Enumerations.DamageType.Physical)
            {
                var set = new int[6] {0, 0, 1, 1, 2, 3};
                defendDice = new Enumerations.DefenseDie[defenceDice];
                for (var i = 0; i < defenceDice; i++)
                {
                    var j = set[Utilities.RollDice(1) - 1];
                    defendDice[i] = (Enumerations.DefenseDie) j;
                    print("Defense die " + i + ":" + defendDice[i]);
                }
            }
            else if (damageType == Enumerations.DamageType.Magical)
            {
                defendDice = new Enumerations.DefenseDie[magicalDefenceDice];
                for (var i = 0; i < magicalDefenceDice; i++)
                    defendDice[i] = (Enumerations.DefenseDie) Utilities.RollDice(1);
            }
            else
            {
                defendDice = new Enumerations.DefenseDie[0];
            }

            for (var i = 0; i < die.Length; i++)
                if (die[i] == Enumerations.AttackDie.hit)
                {
                    for (var j = 0; j < defendDice.Length; j++)
                        if (defendDice[j] == Enumerations.DefenseDie.block)
                        {
                            die[i] = Enumerations.AttackDie.blank;
                            defendDice[j] = Enumerations.DefenseDie.fail;
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.evade)
                        {
                            //play miss sound
                            die[i] = Enumerations.AttackDie.blank;
                            defendDice[j] = Enumerations.DefenseDie.fail;
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.retaliate)
                        {
                            die[i] = Enumerations.AttackDie.blank;
                            defendDice[j] = Enumerations.DefenseDie.fail;
                            if (attacker != null)
                                attacker.TakeDamage(1);
                            break;
                            //Attacker take damage.
                        }
                        else
                        {
                            TakeDamage(1);
                        }

                    if (die[i] == Enumerations.AttackDie.hit) TakeDamage(1);
                }
                else if (die[i] == Enumerations.AttackDie.crit)
                {
                    for (var j = 0; j < defendDice.Length; j++)
                        if (defendDice[j] == Enumerations.DefenseDie.block)
                        {
                            die[i] = Enumerations.AttackDie.blank;
                            defendDice[j] = Enumerations.DefenseDie.fail;
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.evade)
                        {
                            //play miss sound
                            die[i] = Enumerations.AttackDie.blank;
                            defendDice[j] = Enumerations.DefenseDie.fail;
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.retaliate)
                        {
                            die[i] = Enumerations.AttackDie.blank;
                            defendDice[j] = Enumerations.DefenseDie.fail;
                            if (attacker != null)
                                attacker.TakeDamage(2);
                            break;
                            //Attacker take damage.
                        }
                        else
                        {
                            TakeDamage(2);
                        }

                    if (die[i] == Enumerations.AttackDie.crit) TakeDamage(2);
                }
                else if (die[i] == Enumerations.AttackDie.pierce)
                {
                    for (var j = 0; j < defendDice.Length; j++)
                        if (defendDice[j] == Enumerations.DefenseDie.block)
                        {
                            die[i] = Enumerations.AttackDie.hit;
                            defendDice[j] = Enumerations.DefenseDie.fail;
                            TakeDamage(1);
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.evade)
                        {
                            //play miss sound
                            die[i] = Enumerations.AttackDie.blank;
                            defendDice[j] = Enumerations.DefenseDie.fail;
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.retaliate)
                        {
                            die[i] = Enumerations.AttackDie.blank;
                            defendDice[j] = Enumerations.DefenseDie.fail;
                            if (attacker != null)
                                attacker.TakeDamage(1);
                            break;
                            //Attacker take damage.
                        }
                        else
                        {
                            TakeDamage(1);
                        }

                    if (die[i] == Enumerations.AttackDie.hit) TakeDamage(1);
                }

            hitpointsField.text = myHitpoints.ToString();
        }

        public void TakeDamage(int damage)
        {
            myHitpoints -= damage;
            healthBar.value = myHitpoints;
            hitpointsField.text = myHitpoints.ToString();
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
            myScript_AffectedBy.OnEndTurn();
            canvasLocal.gameObject.SetActive(false);
            canvasWorld.gameObject.SetActive(false);
            //script_GameManager.CmdNextTurn();
            if (allowedMovement.Count > 0)
                script_MovementPainter.RemoveAllowedMovementMarker(allowedMovement);
            //script_MovementPainter.RemoveAllowedMovementMarker(allowedMovement);
        }

        public GameObject previousSelected { get; set; }

        public GameObject selected { get; set; }

        private void Start()
        {
            myScript_BoardController = GameObject.Find("Board").GetComponent<script_BoardController>();
            myScript_MovementParser = GetComponent<script_MovementParser>();
            myScript_AffectedBy = GetComponent<scr_AffectedBy>();
            canvasLocal = transform.Find("Canvas_Local").GetComponent<Canvas>();
            canvasWorld = transform.Find("Canvas_World").GetComponent<Canvas>();
            directions = transform.Find("Canvas_Local/Directions").gameObject;
            availableMovementField = transform.Find("Canvas_World/AdventurerHUD/Movement/Text").GetComponent<Text>();
            inventoryHUD = transform.Find("Canvas_World/InventoryHUD").gameObject;
            hitpointsField = transform.Find("Canvas_World/AdventurerHUD/Health/Text").GetComponent<Text>();
            healthBar = transform.Find("Canvas_Local/Healthbar").GetComponent<Slider>();
            markerStun = transform.Find("Canvas_Local/Marker_Stun").gameObject;
            myHitpoints = 10;
            hitpointsField.text = myHitpoints.ToString();
            healthBar.value = myHitpoints;
            healthBar.maxValue = myHitpoints;
            healthBar.gameObject.SetActive(false);
            markerStun.SetActive(false);

            inventoryHUD.SetActive(false);
            canvasLocal.gameObject.SetActive(false);
            canvasWorld.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Tab)) healthBar.gameObject.SetActive(!healthBar.gameObject.activeInHierarchy);
            if (Input.GetKeyUp(KeyCode.N) && beginMyTurn == false && isMyTurn) EndTurn();
            if (beginMyTurn)
            {
                canvasLocal.gameObject.SetActive(true);
                canvasWorld.gameObject.SetActive(true);
                myDirection = Enumerations.Direction.Choose;
                availableMovement = Utilities.RollDice(movementDice);
                availableMovementField.text = availableMovement.ToString();
                beginMyTurn = false;
                attackedThisTurn = false;

                script_MovementPainter.RemoveAllowedMovementMarker(allowedMovement);
                allowedMovement =
                    myScript_MovementParser.GetAllowedMovement(myTile, ref myDirection, availableMovement);
                script_MovementPainter.AddAllowedMovementMarker(allowedMovement);
            }
            else if (isMyTurn)
            {
                if (placed && myScript_AffectedBy.isStunned == false)
                {
                    if (myDirection == Enumerations.Direction.Choose)
                    {
                        myDirection = Enumerations.ChooseDirection(directions);
                        //script_MovementPainter.RemoveAllowedMovementMarker(allowedMovement);
                        allowedMovement =
                            myScript_MovementParser.GetAllowedMovement(myTile, ref myDirection, availableMovement);
                        script_MovementPainter.AddAllowedMovementMarker(allowedMovement);
                    }

                    if (Input.GetKeyUp(KeyCode.R) && hasMoved == false) myDirection = Enumerations.Direction.Choose;

                    if (Input.GetMouseButtonUp(1))
                    {
                        altSelected = script_SelectObject.ReturnAlternateClick();

                        if (allowedMovement.Contains(altSelected))
                            if (script_BoardController.GetTileDistance(myTile, altSelected) <= availableMovement)
                                Move(altSelected, allowedMovement);
                        script_MovementPainter.RemoveAllowedMovementMarker(allowedMovement);
                        allowedMovement =
                            myScript_MovementParser.GetAllowedMovement(myTile, ref myDirection, availableMovement);
                        script_MovementPainter.AddAllowedMovementMarker(allowedMovement);
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        if (selected.tag == "Monster" &&
                            script_BoardController.GetTileDistance(myTile, selected) <= range &&
                            attackedThisTurn == false)
                            Attack(selected.GetComponent<ICombatant>(), Enumerations.DamageType.Physical);
                        if ((selected.tag == "Door" || selected.tag == "OpenDoor") &&
                            script_BoardController.GetTileDistance(myTile, selected) <= 1){}
//                            selected.GetComponent<scr_Door>().ToggleDoor();
                    }
                }

                if (Input.GetKeyUp(KeyCode.Space))
                    if (placed == false)
                    {
                        if (selected.tag == "PlayerSpawn")
                        {
                            transform.position = selected.transform.position;
                            myTile = selected;
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
            while (myTile != toTile)
            {
//            print(index);
                tileTemp = allowedMovement.ElementAt(index++);
                if (tileTemp.GetComponent<script_Tile>().occupied)
                {
                    myDirection = Enumerations.Direction.Choose;
                    break;
                }

                hasMoved = true;
                myTile.GetComponent<script_Tile>().occupied = false;
                myTile.GetComponent<script_Tile>().occupier = null;
                myTile = tileTemp;
                myTile.GetComponent<script_Tile>().occupied = true;
                myTile.GetComponent<script_Tile>().occupier = gameObject;
                transform.position = myTile.transform.position;
                availableMovement--;
                availableMovementField.text = availableMovement.ToString();
                //tile call method stepped on if not invisible
                activatedTrap = myTile.GetComponent<script_Tile>().SteppedOn(gameObject);
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
            if (inventoryHUD.activeInHierarchy == false)
                inventoryHUD.SetActive(true);
            else
                inventoryHUD.SetActive(false);
        }

        private void Die()
        {
            if (placed)
            {
                myTile.GetComponent<script_Tile>().occupied = false;
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