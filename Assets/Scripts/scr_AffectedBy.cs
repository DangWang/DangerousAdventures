using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class scr_AffectedBy : NetworkBehaviour
{
    private Ability.BuffDebuff _bd;
    public bool blindImmunity;
    private scr_CombatController _combatController;
    public bool drawImmunity;
    public List<Ability.BuffDebuff> effects = new List<Ability.BuffDebuff>(); //the effects that are active
    [SyncVar] public bool isStunned, isRooted, isInStasis, isSilenced, isBlinded, isTaunted, isSlowed;
    public bool rootImmunity;
    public bool silenceImmunity;
    public bool slowImmunity;
    public bool stasisImmunity;
    public bool stunImmunity;
    private GameObject _stunMarker, _rootMarker, _stasisMarker, _silenceMarker, _blindMarker, _tauntMarker, _slowMarker;
    public bool tauntImmunity;
    public bool throwImmunity;

    private void Start()
    {
        _stunMarker = transform.Find("Canvas_Local/Marker_Stun").gameObject;
        _rootMarker = transform.Find("Canvas_Local/Marker_Root").gameObject;
        _stasisMarker = transform.Find("Canvas_Local/Marker_Stasis").gameObject;
        _silenceMarker = transform.Find("Canvas_Local/Marker_Silence").gameObject;
        _blindMarker = transform.Find("Canvas_Local/Marker_Blind").gameObject;
        _tauntMarker = transform.Find("Canvas_Local/Marker_Taunt").gameObject;
        _slowMarker = transform.Find("Canvas_Local/Marker_Slow").gameObject;
        _combatController = GetComponent<scr_CombatController>();
        _stunMarker.SetActive(false);
        _rootMarker.SetActive(false);
        _stasisMarker.SetActive(false);
        _silenceMarker.SetActive(false);
        _blindMarker.SetActive(false);
        _tauntMarker.SetActive(false);
        _slowMarker.SetActive(false);
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
        _combatController = GetComponent<scr_CombatController>();
        _combatController.physicalAttack += bd.physicalAttackMod;
        _combatController.physicalDefense += bd.physicalDefenseMod;
        _combatController.magicalAttack += bd.magicalAttackMod;
        _combatController.magicalDefense += bd.magicalDefenseMod;
        _combatController.pureAttack += bd.pureAttackMod;
        _combatController.pureDefense += bd.pureDefenseMod;
        UpdateActive();
        RpcUpdateMarkers();
    }

    //Removes the effects that have ended.
    public void UpdateDurations()
    {
        for (var i = 0; i < effects.Count; i++)
        {
            //Reduce the duration. If it's still >= 0 insert the changed effect (effects are immutable).
            if (effects[i].duration >= 0)
            {
                _bd = effects[i];
                _bd.duration--;
                effects.RemoveAt(i);
                if (_bd.duration >= 0)
                {
                    effects.Insert(i, _bd);
                }
                else
                {
                    i--;
                }

                if (effects.Count == 0)
                {
                    break;
                }
            }
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
        _stunMarker.SetActive(isStunned);
        _rootMarker.SetActive(isRooted);
        _stasisMarker.SetActive(isInStasis);
        _silenceMarker.SetActive(isSilenced);
        _blindMarker.SetActive(isBlinded);
        _tauntMarker.SetActive(isTaunted);
        _slowMarker.SetActive(isSlowed);
    }
}