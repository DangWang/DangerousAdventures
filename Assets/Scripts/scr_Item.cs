using UnityEngine;

public abstract class scr_Item : MonoBehaviour
{
    public int afterTradeValue;
    public int attackRange;
    public bool consumable; //TODO charges?
    public bool inInventory;
    public int initialValue;
    public bool isArtifact;
    public int magicalAttack;
    public int magicalDefense;
    public int physicalAttack;
    public int physicalDefense;
    public int pureAttack;
    public int pureDefense;
    public Enumerations.InventorySlot requiredSlot;
}
/*
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
*/