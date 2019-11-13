using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enumerations : MonoBehaviour
{
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
    };

    public enum CharacterState
    {
        alive,
        respawning,
        dead
    }

    public enum CastTarget
    {
        Single, AOE
    }

    public enum SpellDuration
    {
        Instant, OverTime
    }

    public enum DamageType
    {
        Physical, Magical, Pure
    }

    public enum DisableTypes
    {
        Root, Stasis, Silence, Stun, Blind, Taunt, Slow, Drag, Throw
    }
    public enum Purge
    {
        Buffs, Debuffs, All, None
    }

    public enum AOETileType
    {
        Out, In, CastingPoint
    }

    public enum InventorySlot
    {
        general, misc, hand, head, body
    }

    public static Enumerations.Direction ChooseDirection(GameObject canvasDirections)
    {
        if(canvasDirections.activeInHierarchy == false)
            canvasDirections.SetActive(true);
        if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            canvasDirections.SetActive(false);
            return (Enumerations.Direction)0;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            canvasDirections.SetActive(false);
            return (Enumerations.Direction)1;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            canvasDirections.SetActive(false);
            return (Enumerations.Direction)2;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            canvasDirections.SetActive(false);
            return (Enumerations.Direction)3;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            canvasDirections.SetActive(false);
            return (Enumerations.Direction)4;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            canvasDirections.SetActive(false);
            return (Enumerations.Direction)5;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            canvasDirections.SetActive(false);
            return (Enumerations.Direction)6;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            canvasDirections.SetActive(false);
            return (Enumerations.Direction)7;
        }
        else
        {
            //print("Invalid direction number!");
        }
        return Enumerations.Direction.Choose;
    }
}
