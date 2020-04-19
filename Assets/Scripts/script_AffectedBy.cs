using System.Collections.Generic;
using UnityEngine;

public class script_AffectedBy : MonoBehaviour
{
    private Ability.BuffDebuff _bd;
    public List<Ability.BuffDebuff> effects = new List<Ability.BuffDebuff>();
    public bool getsStunned;

    public void UpdateEffects()
    {
        for (var i = 0; i < effects.Count; i++)
        {
            if (effects[i].duration >= 0)
            {
                _bd = effects[i];
                _bd.duration--;
                effects.RemoveAt(i);
                if (_bd.duration >= 0)
                    effects.Insert(i, _bd);
                else
                    i--;
                if (effects.Count == 0)
                    break;
            }

            if (effects[i].disable == Enumerations.DisableTypes.Stun) print(gameObject.name + " stunned");
        }
    }

    public bool CheckStunned()
    {
        foreach (var bd in effects)
            if (bd.disable == Enumerations.DisableTypes.Stun)
                return true;
        return false;
    }
}