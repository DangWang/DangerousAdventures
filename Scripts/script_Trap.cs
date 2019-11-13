using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_Trap : MonoBehaviour
{
    public UnityEngine.Object[] myEffects;

    public void Trigger(GameObject activator)
    {
        print("Trap activated!");
        foreach(UnityEngine.Object ability in myEffects)
        {
            print("Ability " + ability.name + " called.");
            script_AbilityCaster.CastAbility(ability.name, activator);
        }
        Destroy(this.gameObject);
    }
}
