using UnityEngine;

public class script_Inventory : MonoBehaviour
{
    //things
}

//TODO: Seperate c# files
public abstract class Item : MonoBehaviour
{
    public int afterTradeValue;
    public int attackRangeBonus;
    public bool consumable; //TODO charges?
    public int defenseDiceBonus;
    public int initialValue;
    public int magicalAttackDiceBonus;
    public int physicalAttackDiceBonus;
    public int pureAttackDiceBonus;
    public Enumerations.InventorySlot requiredSlot;
}

public class Weapon : Item
{
    public bool twoHanded;

    public virtual void Attack()
    {
    }

    public void SpecialAttack() //ability
    {
    }
}

public class WeaponRestricted : Weapon
{
    public int ammunition;

    public override void Attack()
    {
        if (ammunition > 0) base.Attack();
    }
}

public class Armor : Item
{
}

public class MiscItem : Item
{
}