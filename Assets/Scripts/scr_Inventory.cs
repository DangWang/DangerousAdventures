using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class scr_Inventory : NetworkBehaviour
{
    private scr_CombatController combatController;
    public List<GameObject> equippedWeapons = new List<GameObject>();
    public List<GameObject> myItems = new List<GameObject>();

    private void Start()
    {
        combatController = GetComponent<scr_CombatController>();
    }

    //Makes the position of all the items in the inventory the same as the parent gameobject.
    public void UpdateAllItemPositions()
    {
        foreach (var g in myItems) g.transform.position = transform.position;
    }

    //Checks if the character is carrying an artifact (useful for adventurers).
    public bool CarryingArtifact()
    {
        foreach (var g in myItems)
            if (g.GetComponent<scr_Item>().isArtifact)
                return true;
        return false;
    }

    public void PickUp(GameObject item)
    {
        item.GetComponent<SpriteRenderer>().enabled = false;
        myItems.Add(item);
        if (item.tag == "Weapon") //Automatically equips the item item if it's a weapon.
            EquipWeapon(item);
    }

    public void DropItem(GameObject item)
    {
        item.GetComponent<SpriteRenderer>().enabled = false;
        myItems.Remove(item);
        if (equippedWeapons.Contains(item)) //Automatically unequips the item item if it's a weapon.
            UnequipWeapon(item);
    }

    //Adds the weapon modifiers to the characters stats.
    public void EquipWeapon(GameObject item)
    {
        if (item.tag == "Weapon")
        {
            equippedWeapons.Add(item);
            var weaponScript = item.GetComponent<scr_Weapon>();
            combatController.physicalAttack += weaponScript.physicalAttack;
            combatController.physicalDefense += weaponScript.physicalDefense;
            combatController.magicalAttack += weaponScript.magicalAttack;
            combatController.magicalDefense += weaponScript.magicalDefense;
            combatController.pureAttack += weaponScript.pureAttack;
            combatController.pureDefense += weaponScript.pureDefense;
        }
        else
        {
            Debug.LogWarning("Tried to equip as a weapon, an item that is not a weapon.");
        }
    }

    //Removes the weapon modifiers to the characters stats.
    public void UnequipWeapon(GameObject item)
    {
        if (equippedWeapons.Contains(item))
        {
            equippedWeapons.Remove(item);
            var weaponScript = item.GetComponent<scr_Weapon>();
            combatController.physicalAttack -= weaponScript.physicalAttack;
            combatController.physicalDefense -= weaponScript.physicalDefense;
            combatController.magicalAttack -= weaponScript.magicalAttack;
            combatController.magicalDefense -= weaponScript.magicalDefense;
            combatController.pureAttack -= weaponScript.pureAttack;
            combatController.pureDefense -= weaponScript.pureDefense;
        }
        else
        {
            Debug.LogWarning("Tried to unequip a non equipped weapon/item. Is there a matching equip weapon error?");
        }
    }
}