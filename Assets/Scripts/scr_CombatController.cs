using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class scr_CombatController : NetworkBehaviour
{
    [SyncVar]
    public bool attackedThisTurn;
    [SyncVar]
    public int attackRange;
    private Slider healthBar;
    [SyncVar(hook = "OnChangeHealth")]
    public int hp;
    [SyncVar]
    public int magicalAttack;
    [SyncVar]
    public int magicalDefense;
    public scr_Player parentScript;
    [SyncVar]
    public int physicalAttack;
    [SyncVar]
    public int physicalDefense;
    [SyncVar]
    public int pureAttack;
    [SyncVar]
    public int pureDefense;

    private void Start()
    {
        //parentScript = GetComponent<scr_Player>();
        healthBar = transform.Find("Canvas/Healthbar").GetComponent<Slider>();
        healthBar.value = hp;
        healthBar.maxValue = hp;
        healthBar.gameObject.SetActive(false);
    }

    public void OnStartTurn()
    {
        attackedThisTurn = false;
    }

    public void OnSelectedUpdate()
    {
        if (Input.GetMouseButtonUp(1))
        {
            parentScript.altSelected = script_SelectObject.ReturnAlternateClick();
            if (parentScript.altSelected.tag == "Monster"
                && script_BoardController.GetTileDistance(gameObject, parentScript.selected) <= attackRange
                && attackedThisTurn == false)
                CmdAttack(parentScript.altSelected.name);
        }
    }
    
    [Command]
    private void CmdAttack(string targetS)
    {
        GameObject target = GameObject.Find(targetS);
        Attack(target.gameObject.GetComponent<scr_CombatController>());
    }

    //one attack for each damage type.
    public void Attack(scr_CombatController target)
    {
        print(name + "is attacking.");
        var set = new int[6] {0, 0, 1, 1, 2, 3};
        Enumerations.AttackDie[] dice;
        //Physical Attack
        if (physicalAttack > 0)
        {
            dice = new Enumerations.AttackDie[physicalAttack];
            for (var i = 0; i < physicalAttack; i++)
            {
                var j = set[Utilities.RollDice(1) - 1];
                dice[i] = (Enumerations.AttackDie) j;
                print("Attack die " + i + ":" + dice[i]);
            }

            target.Defend(dice, Enumerations.DamageType.Physical, this);
        }

        //Magical Attack
        if (magicalAttack > 0)
        {
            dice = new Enumerations.AttackDie[magicalAttack];
            for (var i = 0; i < physicalAttack; i++)
            {
                var j = set[Utilities.RollDice(1) - 1];
                dice[i] = (Enumerations.AttackDie) j;
                print("Attack die " + i + ":" + dice[i]);
            }

            target.Defend(dice, Enumerations.DamageType.Magical, this);
        }

        //Pure Attack
        if (pureAttack > 0)
        {
            dice = new Enumerations.AttackDie[pureAttack];
            for (var i = 0; i < physicalAttack; i++)
            {
                var j = set[Utilities.RollDice(1) - 1];
                dice[i] = (Enumerations.AttackDie) j;
                print("Attack die " + i + ":" + dice[i]);
            }

            target.Defend(dice, Enumerations.DamageType.Pure, this);
        }

        attackedThisTurn = true;
    }

    //Throws defense dice for the corresponding damageType. Pure is unblockable.
    public void Defend(Enumerations.AttackDie[] attackerDice, Enumerations.DamageType damageType,
        scr_CombatController attacker)
    {
        print(name + "is defending.");
        Enumerations.DefenseDie[] defenderDice;

        if (damageType == Enumerations.DamageType.Physical)
        {
            var set = new int[6] {0, 0, 1, 1, 2, 3};
            defenderDice = new Enumerations.DefenseDie[physicalDefense];
            for (var i = 0; i < physicalDefense; i++)
            {
                var j = set[Utilities.RollDice(1) - 1];
                defenderDice[i] = (Enumerations.DefenseDie) j;
                print("Defense die " + i + ":" + defenderDice[i]);
            }
        }
        else if (damageType == Enumerations.DamageType.Magical)
        {
            defenderDice = new Enumerations.DefenseDie[magicalDefense];
            for (var i = 0; i < magicalDefense; i++) defenderDice[i] = (Enumerations.DefenseDie) Utilities.RollDice(1);
        }
        else
        {
            defenderDice = new Enumerations.DefenseDie[0];
        }

        for (var i = 0; i < attackerDice.Length; i++)
            if (attackerDice[i] == Enumerations.AttackDie.hit)
            {
                if (defenderDice.Length > i && defenderDice[i] == Enumerations.DefenseDie.block)
                {
                }
                else if (defenderDice.Length > i && defenderDice[i] == Enumerations.DefenseDie.evade)
                {
                    //play miss sound
                }
                else if (defenderDice.Length > i && defenderDice[i] == Enumerations.DefenseDie.retaliate)
                {
                    if (attacker != null)
                        attacker.TakeDamage(1);
                }
                else
                {
                    TakeDamage(1);
                }
            }
            else if (attackerDice[i] == Enumerations.AttackDie.crit)
            {
                if (defenderDice.Length > i && defenderDice[i] == Enumerations.DefenseDie.block)
                {
                }
                else if (defenderDice.Length > i && defenderDice[i] == Enumerations.DefenseDie.evade)
                {
                    //play miss sound
                }
                else if (defenderDice.Length > i && defenderDice[i] == Enumerations.DefenseDie.retaliate)
                {
                    if (attacker != null)
                        attacker.TakeDamage(2);
                }
                else
                {
                    TakeDamage(2);
                }
            }
            else if (attackerDice[i] == Enumerations.AttackDie.pierce)
            {
                if (defenderDice.Length > i && defenderDice[i] == Enumerations.DefenseDie.block)
                {
                    TakeDamage(1);
                }
                else if (defenderDice.Length > i && defenderDice[i] == Enumerations.DefenseDie.evade)
                {
                    //play miss sound
                }
                else if (defenderDice.Length > i && defenderDice[i] == Enumerations.DefenseDie.retaliate)
                {
                    if (attacker != null)
                        attacker.TakeDamage(1);
                }
                else
                {
                    TakeDamage(1);
                }
            }
    }

    //Checks for hp <= 0 are made on the "parent" script. This only deals damage.
    public void TakeDamage(int damage)
    {
        hp -= damage;
        healthBar.value = hp;
        if (damage == 1)
        {
            //play hit sound
        }
        else if (damage == 2)
        {
            //play crit sound
        }

        if (hp <= 0) parentScript.RpcMonsterDied(gameObject.name);
    }

    public void ToggleHealthbar()
    {
        healthBar.gameObject.SetActive(!healthBar.gameObject.activeInHierarchy);
    }
    
    public void OnChangeHealth(int oldhp, int newhp)
    {
        healthBar.value = newhp;
    }
}