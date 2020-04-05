using System.Collections.Generic;
using UnityEngine;

public class script_AffectedBy : MonoBehaviour
{
    private Ability.BuffDebuff bd;
    public List<Ability.BuffDebuff> effects = new List<Ability.BuffDebuff>();
    public bool getsStunned;

    public void UpdateEffects()
    {
        for (var i = 0; i < effects.Count; i++)
        {
            if (effects[i].duration >= 0)
            {
                bd = effects[i];
                bd.duration--;
                effects.RemoveAt(i);
                if (bd.duration >= 0)
                    effects.Insert(i, bd);
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