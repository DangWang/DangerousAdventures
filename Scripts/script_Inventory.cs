using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_Inventory : MonoBehaviour
{
    //things
}

//TODO: Seperate c# files
public abstract class Item : MonoBehaviour
{
    public int initialValue;
    public int afterTradeValue;
    public bool consumable; //TODO charges?
    public Enumerations.InventorySlot requiredSlot;
    public int attackRangeBonus;
    public int physicalAttackDiceBonus;
    public int magicalAttackDiceBonus;
    public int pureAttackDiceBonus;
    public int defenseDiceBonus;
}

public class Weapon : Item
{
    public bool twoHanded;
    
    virtual public void Attack()
    {
    
    }
    public void SpecialAttack()//ability
    {
        
    }
}

public class WeaponRestricted : Weapon
{
    public int ammunition;

    public override void Attack()
    {
        if (ammunition > 0)
        {
            base.Attack();
        }
    }
}

public class Armor : Item
{
}

public class MiscItem : Item
{
}
