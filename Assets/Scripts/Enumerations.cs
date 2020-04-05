using UnityEngine;
//This is my change to demonstrate git branches!
public class Enumerations : MonoBehaviour
{
    public enum AffectedEntities
    {
        All,
        Allies,
        Enemies
    }

    public enum AOETileType
    {
        Out,
        In,
        CastingPoint
    }

    public enum AttackDie
    {
        blank,
        hit,
        crit,
        pierce
    }

    public enum CharacterState
    {
        alive,
        respawning,
        dead
    }

    public enum DamageType
    {
        Physical,
        Magical,
        Pure
    }

    public enum DefenseDie
    {
        fail,
        block,
        evade,
        retaliate
    }

    public enum Direction
    {
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        Choose = -1
    }

    public enum DisableTypes
    {
        Root,
        Stasis,
        Silence,
        Stun,
        Blind,
        Taunt,
        Slow,
        Drag,
        Throw,
        None
    }

    public enum InventorySlot
    {
        general,
        misc,
        hand,
        head,
        body
    }

    public enum Purge
    {
        Buffs,
        Debuffs,
        All,
        None
    }

    public enum SpellDuration
    {
        Instant,
        OverTime
    }

    public enum TargetPoint
    {
        Self,
        Target
    }

    public static Direction ChooseDirection(GameObject canvasDirections)
    {
        if (canvasDirections.activeInHierarchy == false)
            canvasDirections.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            canvasDirections.SetActive(false);
            return 0;
        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            canvasDirections.SetActive(false);
            return (Direction) 1;
        }

        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            canvasDirections.SetActive(false);
            return (Direction) 2;
        }

        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            canvasDirections.SetActive(false);
            return (Direction) 3;
        }

        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            canvasDirections.SetActive(false);
            return (Direction) 4;
        }

        if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            canvasDirections.SetActive(false);
            return (Direction) 5;
        }

        if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            canvasDirections.SetActive(false);
            return (Direction) 6;
        }

        if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            canvasDirections.SetActive(false);
            return (Direction) 7;
        }

        return Direction.Choose;
    }
}