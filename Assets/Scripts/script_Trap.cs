using Mirror;
using UnityEngine;

public class script_Trap : NetworkBehaviour
{
    public Object[] myEffects;
    public int trapCost = 1;

    public void Trigger(GameObject activator)
    {
        print("Trap activated!");
        foreach (var ability in myEffects)
        {
            print("Ability " + ability.name + " called.");
            script_AbilityCaster.CastAbility(ability.name, activator, null, true);
        }

        NetworkServer.Destroy(gameObject);
    }
}