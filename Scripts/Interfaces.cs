using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelector
{
    GameObject selected
    {
        get;
        set;
    }
}

public interface IPlayer
{
    bool beginMyTurn
    {
        get;
        set;
    }
    bool isMyTurn
    {
        get;
        set;
    }
    GameObject ReturnPlayerObject();
    void EndTurn();
}

public interface IAttacker
{
    void Attack();
}

public interface IDefender
{
    void Defend(int damage, Enumerations.DamageType damageType);
}
