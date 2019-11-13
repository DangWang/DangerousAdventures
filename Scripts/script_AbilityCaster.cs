using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.CodeDom.Compiler;

//TODO refactor this load and the editor load, they are the same.
public class script_AbilityCaster : MonoBehaviour   
{
    static Dictionary<string, Ability.AbilityStruct> allAbilities = new Dictionary<string, Ability.AbilityStruct>();

    public static void CastAbility(string abilityName, GameObject castingTarget)
    {
        Ability.AbilityStruct myAbility;
        if(!allAbilities.ContainsKey(abilityName))
        {
            string path = "Assets/Abilities/" + abilityName + ".ability";
            StreamReader textReader = new StreamReader(path);
            allAbilities.Add(abilityName, LoadAbility(textReader));
            textReader.Close();
        }
        myAbility = allAbilities[abilityName];

        if(myAbility.castTarget == Enumerations.CastTarget.Single)
        {
            castingTarget.GetComponent<IDefender>().Defend(myAbility.damage, myAbility.damageType);
        }
    }

    private static Ability.AbilityStruct LoadAbility(StreamReader reader)
    {
        Ability.BuffDebuff LoadBuffDebuff(string s)
        {
            Ability.BuffDebuff my = new Ability.BuffDebuff();
            string[] splitString = s.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);

            my.disable = (Enumerations.DisableTypes)Int32.Parse(splitString[0]);
            my.purge = (Enumerations.Purge)Int32.Parse(splitString[1]);
            my.durationType = (Enumerations.SpellDuration)Int32.Parse(splitString[2]);
            my.throwRange = Int32.Parse(splitString[3]);
            my.duration = Int32.Parse(splitString[4]);
            my.movementModifier = Int32.Parse(splitString[5]);
            my.throwsTarget = Boolean.Parse(splitString[6]);

            return my;
        }
        Ability.AbilityStruct ability = new Ability.AbilityStruct();
        string line = "";
        line = reader.ReadLine();
        
        while (line != null)
        {          
            //Read the line and get data
            //Debug.Log(line);
            string[] splitString = line.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            if (splitString[0] == "Name")
            {
                ability.name = splitString[1];
            }else if(splitString[0] == "Description")
            {
                ability.description = splitString[1];
            }
            else if (splitString[0] == "Aura")
            {
                ability.isAura = Boolean.Parse(splitString[1]);
            }
            else if (splitString[0] == "IsActive")
            {
                ability.isActive = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "ChargesTurn")
            {
                ability.chargesTurn = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "CastRange")
            {
                ability.castRange = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "AllowedTargets")
            {
                ability.allowedTargets = splitString[1];
            }
            else if(splitString[0] == "DurationType")
            {
                ability.durationType = (Enumerations.SpellDuration)Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "DamageDispellEffect")
            {
                ability.damageDispellEffect = Boolean.Parse(splitString[1]);
            }
            else if(splitString[0] == "TransformSquare")
            {
                ability.transformSquare = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "SquareChange")
            {
                ability.squareChange = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "SquareChangeRange")
            {
                ability.squareChange = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "NumberOfBuffsDebuffs")
            {
                ability.numberOfBuffsDebuffs = Int32.Parse(splitString[1]);
                ability.buffsDebuffs = new Ability.BuffDebuff[ability.numberOfBuffsDebuffs];
            }          
            else if(splitString[0] == "BuffsDebuffs")
            {
                ability.buffsDebuffs[Int32.Parse(splitString[1])] = LoadBuffDebuff(splitString[2]);
            }
            else if(splitString[0] == "CastTarget")
            {
                ability.castTarget = (Enumerations.CastTarget)Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "CorpseContinuation")
            {
                ability.corpseContinuation = Boolean.Parse(splitString[1]);
            }
            else if(splitString[0] == "CanBounce")
            {
                ability.canBounce = Boolean.Parse(splitString[1]);
            }            
            else if(splitString[0] == "BounceReduction")
            {
                ability.bounceReduction = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "BounceNumber")
            {
                ability.bounceNumber = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "BounceRange")
            {
                ability.bounceRange = Int32.Parse(splitString[1]);
            }      
            else if(splitString[0] == "Rejuvenation")
            {
                ability.rejuvenation = Boolean.Parse(splitString[1]);
            }
            else if(splitString[0] == "RejuvenationTime")
            {
                ability.rejuvenationTime = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "SpawnWard")
            {
                ability.spawnWard = Boolean.Parse(splitString[1]);
            }
            else if(splitString[0] == "WardRange")
            {
                ability.wardRange = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "WardInvulnerability")
            {
                ability.wardInvulnerability = Boolean.Parse(splitString[1]);
            }
            else if(splitString[0] == "WardAbilities")
            {
                ability.wardAbilities = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "Reincarnation")
            {
                ability.reincarnation = Boolean.Parse(splitString[1]);
            }
            else if(splitString[0] == "ReincarnationTime")
            {
                ability.reincarnationTime = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "ReincarnationHealth")
            {
                ability.reincarnationHealth = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "DeathProtection")
            {
                ability.deathProtection = Boolean.Parse(splitString[1]);
            }
            else if(splitString[0] == "DeathProtectionDuration")
            {
                ability.deathProtectionDuration = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "DeathProtectionLifeCycle")
            {
                ability.deathProtectionLifeCycle = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "DeathProtectionIgnore")
            {
                ability.deathProtectionIgnore = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "Summoning")
            {
                ability.summoning = Boolean.Parse(splitString[1]);
            }
            else if(splitString[0] == "Summons")
            {
                ability.summons = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "NumberOfSummons")
            {
                ability.numberOfSummons = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "Duration")
            {
                ability.duration = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "Devour")
            {
                ability.devour = Boolean.Parse(splitString[1]);
            }
            else if(splitString[0] == "DevourDamage")
            {
                ability.devourDamage = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "DevourEscapeHits")
            {
                ability.devourEscapeHits = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "AbilitiesPhase")
            {
                ability.abilitiesPhase = Boolean.Parse(splitString[1]);
            }
            else if(splitString[0] == "Damage")
            {
                ability.damage = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "DamageType")
            {
                ability.damageType = (Enumerations.DamageType)Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "IntervalsPerTurn")
            {
                ability.intervalsPerTurn = Int32.Parse(splitString[1]);
            }
            else if(splitString[0] == "NumberOfTurnIntervals")
            {
                ability.numberOfTurnIntervals = Int32.Parse(splitString[1]);
            }
            line = reader.ReadLine();
        }
        return ability;
    }
}
