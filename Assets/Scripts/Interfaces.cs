using UnityEngine;

public interface ISelector
{
    GameObject selected { get; set; }

    GameObject previousSelected { get; set; }
}

public interface IPlayer
{
    bool beginMyTurn { get; set; }

    bool isMyTurn { get; set; }

    void EndTurn();
}

public interface ICombatant
{
    int myHitpoints { get; set; }

    void Attack(ICombatant target, Enumerations.DamageType damageType);
    void Defend(Enumerations.AttackDie[] dice, Enumerations.DamageType damageType, ICombatant attacker);
    void TakeDamage(int damage);
}