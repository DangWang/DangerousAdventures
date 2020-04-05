using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror
{
    public class script_MonsterController : MonoBehaviour, ICombatant
    {
        public Object[] abilities;
        public int abilityCount;
        public int attackDice;
        public bool attackedThisTurn;
        public bool canCarryItems;
        private GameObject canvasDirections;
        public int defenseDice;
        public int goldDropped;
        public int goldRandomOffset;
        private bool hasMoved;
        private Slider healthBar;
        public int magicalResistance;
        public int monsterCost;
        public string monsterDescription;
        public int monsterTier;
        public Enumerations.Direction myDirection = Enumerations.Direction.Choose;
        public int myHealth;
        public int myMaxMovement = 8;
        public int myRemainingMovement = 8;
        private script_BoardController myScript_BoardController;
        public GameObject myTile;
        public int physicalResistance;
        public int pureResistance;
        public int range = 1; //min 1
        public int respawnTimer; //in turns
        public bool unrestrictedMovement; //TODO ADD

        public int myHitpoints { get; set; }

        public void Attack(ICombatant target, Enumerations.DamageType damageType)
        {
            var dice = new Enumerations.AttackDie[attackDice];
            if (attackedThisTurn == false)
            {
                var set = new int[6] {0, 0, 1, 1, 2, 3};
                print("Monster Attacking");
                for (var i = 0; i < attackDice; i++)
                {
                    var j = set[Utilities.RollDice(1) - 1];
                    dice[i] = (Enumerations.AttackDie) j;
                    print("Attack die " + i + ":" + dice[i]);
                }

                target.Defend(dice, damageType, this);

                attackedThisTurn = true;
            }
        }

        public void Defend(Enumerations.AttackDie[] die, Enumerations.DamageType damageType, ICombatant attacker)
        {
            print("Monster Defending");
            Enumerations.DefenseDie[] defendDice;

            if (damageType == Enumerations.DamageType.Physical)
            {
                var set = new int[6] {0, 0, 1, 1, 2, 3};
                defendDice = new Enumerations.DefenseDie[defenseDice];
                for (var i = 0; i < defenseDice; i++)
                {
                    var j = set[Utilities.RollDice(1) - 1];
                    defendDice[i] = (Enumerations.DefenseDie) j;
                    print("Defense die " + i + ":" + defendDice[i]);
                }
            }
            else if (damageType == Enumerations.DamageType.Magical)
            {
                defendDice = new Enumerations.DefenseDie[magicalResistance];
                for (var i = 0; i < magicalResistance; i++)
                    defendDice[i] = (Enumerations.DefenseDie) Utilities.RollDice(1);
            }
            else
            {
                defendDice = new Enumerations.DefenseDie[0];
            }

            for (var i = 0; i < die.Length; i++)
                if (die[i] == Enumerations.AttackDie.hit)
                {
                    print("Def hit");
                    for (var j = 0; j < defendDice.Length; j++)
                        if (defendDice[j] == Enumerations.DefenseDie.block)
                        {
                            die[i] = Enumerations.AttackDie.blank;
                            defendDice[j] = Enumerations.DefenseDie.fail;
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.evade)
                        {
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
                    print("Def crit");
                    for (var j = 0; j < defendDice.Length; j++)
                        if (defendDice[j] == Enumerations.DefenseDie.block)
                        {
                            die[i] = Enumerations.AttackDie.blank;
                            defendDice[j] = Enumerations.DefenseDie.fail;
                            break;
                        }
                        else if (defendDice[j] == Enumerations.DefenseDie.evade)
                        {
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
                    print("Def pier");
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
        }

        public void TakeDamage(int damage)
        {
            myHitpoints -= damage;
            healthBar.value = myHitpoints;
            print(gameObject.name + myHitpoints);
            //hitpointsField.text = myHitpoints.ToString();
            if (myHitpoints <= 0) Die();
        }

        private void Start()
        {
            myScript_BoardController = GameObject.Find("Board").GetComponent<script_BoardController>();
            canvasDirections = transform.Find("Canvas_Direction").gameObject;
            healthBar = transform.Find("Canvas/Healthbar").GetComponent<Slider>();
            canvasDirections.SetActive(false);
            myHitpoints = myHealth;
            healthBar.value = myHitpoints;
            healthBar.maxValue = myHitpoints;
            healthBar.gameObject.SetActive(false);
        }

        public void SelectedUpdate()
        {
            if (myDirection == Enumerations.Direction.Choose)
                myDirection = Enumerations.ChooseDirection(canvasDirections);
            if (Input.GetKeyUp(KeyCode.R) && hasMoved == false) 
                myDirection = Enumerations.Direction.Choose;
        }

        public void ToggleHealthbar()
        {
            healthBar.gameObject.SetActive(!healthBar.gameObject.activeInHierarchy);
        }

        public void Unselected()
        {
            canvasDirections.SetActive(false);
        }

        public int Move(GameObject toTile, List<GameObject> allowedMovement)
        {
            GameObject tileTemp;
            var index = 0;
            var tilesMoved = 0;
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

                tilesMoved++;
                hasMoved = true;
                myTile.GetComponent<script_Tile>().occupied = false;
                myTile.GetComponent<script_Tile>().occupier = null;
                myTile = tileTemp;
                myTile.GetComponent<script_Tile>().occupied = true;
                myTile.GetComponent<script_Tile>().occupier = gameObject;
                transform.position = myTile.transform.position;
                myRemainingMovement--;
                //tile call method stepped on if not invisible
                activatedTrap = myTile.GetComponent<script_Tile>().SteppedOn(gameObject);
                if (activatedTrap)
                    break;
            }

            return tilesMoved;
        }

        private void Die()
        {
            //respawn countdown
            //character state respawning
            //drop gold
            //dead (respawn state)
            print(name + " is dead.");
            myTile.GetComponent<script_Tile>().occupied = false;
            GameObject.Find("DM").GetComponent<script_DMController>().monsterCost -= monsterCost;
            Destroy(gameObject);
        }
    }
}