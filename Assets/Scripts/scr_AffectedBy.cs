using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class scr_AffectedBy : NetworkBehaviour
{
    private Ability.BuffDebuff bd;
    public bool blindImmunity;
    private scr_CombatController combatController;
    public bool drawImmunity;
    public List<Ability.BuffDebuff> effects = new List<Ability.BuffDebuff>(); //the effects that are active
    [SyncVar]
    public bool isStunned, isRooted, isInStasis, isSilenced, isBlinded, isTaunted, isSlowed;
    public bool rootImmunity;
    public bool silenceImmunity;
    public bool slowImmunity;
    public bool stasisImmunity;
    public bool stunImmunity;
    private GameObject stunMarker, rootMarker, stasisMarker, silenceMarker, blindMarker, tauntMarker, slowMarker;
    public bool tauntImmunity;
    public bool throwImmunity;

    private void Start()
    {
        stunMarker = transform.Find("Canvas_Local/Marker_Stun").gameObject;
        rootMarker = transform.Find("Canvas_Local/Marker_Root").gameObject;
        stasisMarker = transform.Find("Canvas_Local/Marker_Stasis").gameObject;
        silenceMarker = transform.Find("Canvas_Local/Marker_Silence").gameObject;
        blindMarker = transform.Find("Canvas_Local/Marker_Blind").gameObject;
        tauntMarker = transform.Find("Canvas_Local/Marker_Taunt").gameObject;
        slowMarker = transform.Find("Canvas_Local/Marker_Slow").gameObject;
        combatController = GetComponent<scr_CombatController>();
        stunMarker.SetActive(false);
        rootMarker.SetActive(false);
        stasisMarker.SetActive(false);
        silenceMarker.SetActive(false);
        blindMarker.SetActive(false);
        tauntMarker.SetActive(false);
        slowMarker.SetActive(false);
    }

    [Command]
    public void CmdOnEndTurn()
    {
        OnEndTurn();
    }
    public void OnEndTurn()
    {
        UpdateDurations();
        UpdateActive();
        RpcUpdateMarkers();
    }

    public void AddEffect(Ability.BuffDebuff bd)
    {
        effects.Add(bd);
        combatController = GetComponent<scr_CombatController>();
        combatController.physicalAttack += bd.physicalAttackMod;
        combatController.physicalDefense += bd.physicalDefenseMod;
        combatController.magicalAttack += bd.magicalAttackMod;
        combatController.magicalDefense += bd.magicalDefenseMod;
        combatController.pureAttack += bd.pureAttackMod;
        combatController.pureDefense += bd.pureDefenseMod;
        UpdateActive();
        RpcUpdateMarkers();
    }

    //Removes the effects that have ended.
    public void UpdateDurations()
    {
        for (var i = 0; i < effects.Count; i++)
            //Reduce the duration. If it's still >= 0 insert the changed effect (effects are immutable).
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
    }

    //Updates the currently active effects.
    public void UpdateActive()
    {
        isStunned = false;
        isRooted = false;
        isInStasis = false;
        isSilenced = false;
        isBlinded = false;
        isTaunted = false;
        isSlowed = false;
        for (var i = 0; i < effects.Count; i++)
        {
            if (effects[i].disable == Enumerations.DisableTypes.Stun)
            {
                print(gameObject.name + "is stunned.");
                isStunned = true;
            }

            if (effects[i].disable == Enumerations.DisableTypes.Root)
            {
                print(gameObject.name + "is rooted.");
                isRooted = true;
            }

            if (effects[i].disable == Enumerations.DisableTypes.Stasis)
            {
                print(gameObject.name + "is in stasis.");
                isInStasis = true;
            }

            if (effects[i].disable == Enumerations.DisableTypes.Silence)
            {
                print(gameObject.name + "is silenced.");
                isSilenced = true;
            }

            if (effects[i].disable == Enumerations.DisableTypes.Blind)
            {
                print(gameObject.name + "is blinded.");
                isBlinded = true;
            }

            if (effects[i].disable == Enumerations.DisableTypes.Taunt)
            {
                print(gameObject.name + "is taunted.");
                isTaunted = true;
            }

            if (effects[i].disable == Enumerations.DisableTypes.Slow)
            {
                print(gameObject.name + "is slowed.");
                isSlowed = true;
            }
        }
    }

    //Updates the HUD.
    [ClientRpc]
    public void RpcUpdateMarkers()
    {
        stunMarker.SetActive(isStunned);
        rootMarker.SetActive(isRooted);
        stasisMarker.SetActive(isInStasis);
        silenceMarker.SetActive(isSilenced);
        blindMarker.SetActive(isBlinded);
        tauntMarker.SetActive(isTaunted);
        slowMarker.SetActive(isSlowed);
    }
}