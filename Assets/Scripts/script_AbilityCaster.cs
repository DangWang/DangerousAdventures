using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mirror;
using UnityEngine;


//TODO refactor this load and the editor load, they are the same.
public class script_AbilityCaster : NetworkBehaviour
{
    public static Dictionary<string, Ability.AbilityStruct> allAbilities = new Dictionary<string, Ability.AbilityStruct>();

    public static void PrepareAbility(string abilityName)
    {
        if (!allAbilities.ContainsKey(abilityName))
        {
            //TODO ability not loaded when in build
            var path = "../Summer/Assets/Abilities/" + abilityName + ".ability";
            //Debug.LogError(path);
            var textReader = new StreamReader(path);
            allAbilities.Add(abilityName, LoadAbility(textReader));
            textReader.Close();
        }
    }

    public static void CastAbility(string abilityName, GameObject castingTarget, GameObject caster, bool isTrap = false)
    {
        Ability.AbilityStruct myAbility;
        PrepareAbility(abilityName);
        myAbility = allAbilities[abilityName];

        if (myAbility.durationType == Enumerations.SpellDuration.OverTime && myAbility.duration > 1)
        {
            myAbility.duration--;
            if (caster != null)
            {
                caster.GetComponent<scr_Monster>().activeAbilities.Add(myAbility);
            }
        }

        if (!myAbility.isAOE)
        {
            if (myAbility.damage > 0)
            {
                var attackDice = new Enumerations.AttackDie[myAbility.damage];
                for (var i = 0; i < myAbility.damage; i++)
                {
                    attackDice[i] = (Enumerations.AttackDie)Utilities.RollDice(1);
                }
                if (isTrap)
                {
                    castingTarget.GetComponent<scr_CombatController>().Defend(attackDice, Enumerations.DamageType.Pure, null);
                }
                else
                {
                    castingTarget.GetComponent<scr_CombatController>().Defend(attackDice, myAbility.damageType, null);
                }
            }
            else if (myAbility.damage < 0)
            {
                //Heal
                castingTarget.GetComponent<scr_CombatController>().TakeDamage(myAbility.damage);
            }

            if (castingTarget.GetComponent<scr_AffectedBy>() != null)
            {
                foreach (var bd in myAbility.buffsDebuffs)
                {
                    castingTarget.GetComponent<scr_AffectedBy>().AddEffect(bd);
                }
            }
        }
        else
        {
            var castingPoint = castingTarget;
            var xoff = myAbility.tilesInAOE.ElementAt(0)[0];
            var yoff = myAbility.tilesInAOE.ElementAt(0)[1];
            foreach (var coords in myAbility.tilesInAOE)
            {
                castingTarget = script_BoardController.GetTileByCoords((int)castingPoint.transform.position.x + coords[0] - xoff, (int)castingPoint.transform.position.y + coords[1] - yoff);
                if (castingTarget.GetComponent<script_Tile>().occupied)
                {
                    print("Casted on occupied");
                    castingTarget = castingTarget.GetComponent<script_Tile>().occupier;
                    if (myAbility.damage > 0)
                    {
                        var attackDice = new Enumerations.AttackDie[myAbility.damage];
                        for (var i = 0; i < myAbility.damage; i++)
                        {
                            attackDice[i] = (Enumerations.AttackDie)Utilities.RollDice(1);
                        }
                        if (isTrap)
                        {
                            castingTarget.GetComponent<scr_CombatController>().Defend(attackDice, Enumerations.DamageType.Pure, null);
                        }
                        else
                        {
                            castingTarget.GetComponent<scr_CombatController>().Defend(attackDice, myAbility.damageType, null);
                        }
                    }
                    else if (myAbility.damage < 0)
                    {
                        //Heal
                        castingTarget.GetComponent<scr_CombatController>().TakeDamage(myAbility.damage);
                    }

                    if (castingTarget.GetComponent<scr_AffectedBy>() != null)
                    {
                        foreach (var bd in myAbility.buffsDebuffs)
                        {
                            castingTarget.GetComponent<scr_AffectedBy>().AddEffect(bd);
                        }
                    }
                }
                else
                {
                    print("Unoccupied tile");
                }
            }
        }
    }

    public static void CastAbility(Ability.AbilityStruct myAbility, GameObject castingTarget, GameObject caster,
        bool isTrap = false)
    {
        if (myAbility.durationType == Enumerations.SpellDuration.OverTime && myAbility.duration > 1)
        {
            myAbility.duration--;
            if (caster != null)
            {
                caster.GetComponent<scr_Monster>().activeAbilities.Add(myAbility);
            }
        }

        if (!myAbility.isAOE)
        {
            if (myAbility.damage > 0)
            {
                var attackDice = new Enumerations.AttackDie[myAbility.damage];
                for (var i = 0; i < myAbility.damage; i++)
                {
                    attackDice[i] = (Enumerations.AttackDie)Utilities.RollDice(1);
                }
                if (isTrap)
                {
                    castingTarget.GetComponent<scr_CombatController>().Defend(attackDice, Enumerations.DamageType.Pure, null);
                }
                else
                {
                    castingTarget.GetComponent<scr_CombatController>().Defend(attackDice, myAbility.damageType, null);
                }
            }
            else if (myAbility.damage < 0)
            {
                //Heal
                castingTarget.GetComponent<scr_CombatController>().TakeDamage(myAbility.damage);
            }

            if (castingTarget.GetComponent<scr_AffectedBy>() != null)
            {
                foreach (var bd in myAbility.buffsDebuffs)
                {
                    castingTarget.GetComponent<scr_AffectedBy>().AddEffect(bd);
                }
            }
        }
        else
        {
            var castingPoint = castingTarget;
            var xoff = myAbility.tilesInAOE.ElementAt(0)[0];
            var yoff = myAbility.tilesInAOE.ElementAt(0)[1];
            foreach (var coords in myAbility.tilesInAOE)
            {
                castingTarget = script_BoardController.GetTileByCoords((int)castingPoint.transform.position.x + coords[0] - xoff, (int)castingPoint.transform.position.y + coords[1] - yoff);
                if (castingTarget.GetComponent<script_Tile>().occupied)
                {
                    print("Casted on occupied");
                    castingTarget = castingTarget.GetComponent<script_Tile>().occupier;
                    if (myAbility.damage > 0)
                    {
                        var attackDice = new Enumerations.AttackDie[myAbility.damage];
                        for (var i = 0; i < myAbility.damage; i++)
                        {
                            attackDice[i] = (Enumerations.AttackDie)Utilities.RollDice(1);
                        }
                        if (isTrap)
                        {
                            castingTarget.GetComponent<scr_CombatController>().Defend(attackDice, Enumerations.DamageType.Pure, null);
                        }
                        else
                        {
                            castingTarget.GetComponent<scr_CombatController>().Defend(attackDice, myAbility.damageType, null);
                        }
                    }
                    else if (myAbility.damage < 0)
                    {
                        //Heal
                        castingTarget.GetComponent<scr_CombatController>().TakeDamage(myAbility.damage);
                    }

                    if (castingTarget.GetComponent<scr_AffectedBy>() != null)
                    {
                        foreach (var bd in myAbility.buffsDebuffs)
                        {
                            castingTarget.GetComponent<scr_AffectedBy>().AddEffect(bd);
                        }
                    }
                }
                else
                {
                    print("Unoccupied tile");
                }
            }
        }
    }

    private static Ability.AbilityStruct LoadAbility(StreamReader reader)
    {
        Ability.BuffDebuff LoadBuffDebuff(string s)
        {
            var my = new Ability.BuffDebuff();
            var splitString = s.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);

            my.disable = (Enumerations.DisableTypes)int.Parse(splitString[0]);
            my.purge = (Enumerations.Purge)int.Parse(splitString[1]);
            my.durationType = (Enumerations.SpellDuration)int.Parse(splitString[2]);
            my.throwRange = int.Parse(splitString[3]);
            my.duration = int.Parse(splitString[4]);
            my.movementModifier = int.Parse(splitString[5]);
            my.throwsTarget = bool.Parse(splitString[6]);
            my.physicalAttackMod = int.Parse(splitString[7]);
            my.physicalDefenseMod = int.Parse(splitString[8]);
            my.magicalAttackMod = int.Parse(splitString[9]);
            my.magicalDefenseMod = int.Parse(splitString[10]);
            my.pureAttackMod = int.Parse(splitString[11]);
            my.pureDefenseMod = int.Parse(splitString[12]);

            return my;
        }

        List<int[]> LoadAOETiles(string s)
        {
            var my = new List<int[]>();
            var splitString = s.Split(new[] { "(", ",", ")", " " }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < splitString.Length; i += 2)
            {
                Debug.Log(splitString[i]);
                Debug.Log(splitString[i + 1]);
                my.Add(new int[2] { int.Parse(splitString[i]), int.Parse(splitString[i + 1]) });
            }

            return my;
        }

        var ability = new Ability.AbilityStruct();
        var line = "";
        line = reader.ReadLine();

        while (line != null)
        {
            //Read the line and get data
            //Debug.Log(line);
            var splitString = line.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            if (splitString[0] == "Name")
            {
                ability.name = splitString[1];
            }
            else if (splitString[0] == "Description")
            {
                ability.description = splitString[1];
            }
            else if (splitString[0] == "IsAOE")
            {
                ability.isAOE = bool.Parse(splitString[1]);
            }
            else if (splitString[0] == "IsActive")
            {
                ability.isActive = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "ChargesTurn")
            {
                ability.chargesTurn = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "CastRange")
            {
                ability.castRange = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "AllowedTargets")
            {
                ability.allowedTargets = splitString[1];
            }
            else if (splitString[0] == "DurationType")
            {
                ability.durationType = (Enumerations.SpellDuration)int.Parse(splitString[1]);
            }
            else if (splitString[0] == "DamageDispellEffect")
            {
                ability.damageDispellEffect = bool.Parse(splitString[1]);
            }
            else if (splitString[0] == "TransformSquare")
            {
                ability.transformSquare = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "SquareChange")
            {
                ability.squareChange = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "SquareChangeRange")
            {
                ability.squareChange = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "NumberOfBuffsDebuffs")
            {
                ability.numberOfBuffsDebuffs = int.Parse(splitString[1]);
                ability.buffsDebuffs = new Ability.BuffDebuff[ability.numberOfBuffsDebuffs];
            }
            else if (splitString[0] == "BuffsDebuffs")
            {
                ability.buffsDebuffs[int.Parse(splitString[1])] = LoadBuffDebuff(splitString[2]);
            }
            else if (splitString[0] == "CastTarget")
            {
                ability.targetPoint = (Enumerations.TargetPoint)int.Parse(splitString[1]);
            }
            else if (splitString[0] == "TilesInAOE")
            {
                ability.tilesInAOE = LoadAOETiles(splitString[1]);
            }
            else if (splitString[0] == "CorpseContinuation")
            {
                ability.corpseContinuation = bool.Parse(splitString[1]);
            }
            else if (splitString[0] == "CanBounce")
            {
                ability.canBounce = bool.Parse(splitString[1]);
            }
            else if (splitString[0] == "BounceReduction")
            {
                ability.bounceReduction = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "BounceNumber")
            {
                ability.bounceNumber = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "BounceRange")
            {
                ability.bounceRange = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "Rejuvenation")
            {
                ability.rejuvenation = bool.Parse(splitString[1]);
            }
            else if (splitString[0] == "RejuvenationTime")
            {
                ability.rejuvenationTime = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "SpawnWard")
            {
                ability.spawnWard = bool.Parse(splitString[1]);
            }
            else if (splitString[0] == "WardRange")
            {
                ability.wardRange = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "WardInvulnerability")
            {
                ability.wardInvulnerability = bool.Parse(splitString[1]);
            }
            else if (splitString[0] == "WardAbilities")
            {
                ability.wardAbilities = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "Reincarnation")
            {
                ability.reincarnation = bool.Parse(splitString[1]);
            }
            else if (splitString[0] == "ReincarnationTime")
            {
                ability.reincarnationTime = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "ReincarnationHealth")
            {
                ability.reincarnationHealth = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "DeathProtection")
            {
                ability.deathProtection = bool.Parse(splitString[1]);
            }
            else if (splitString[0] == "DeathProtectionDuration")
            {
                ability.deathProtectionDuration = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "DeathProtectionLifeCycle")
            {
                ability.deathProtectionLifeCycle = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "DeathProtectionIgnore")
            {
                ability.deathProtectionIgnore = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "Summoning")
            {
                ability.summoning = bool.Parse(splitString[1]);
            }
            else if (splitString[0] == "Summons")
            {
                ability.summons = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "NumberOfSummons")
            {
                ability.numberOfSummons = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "Duration")
            {
                ability.duration = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "Devour")
            {
                ability.devour = bool.Parse(splitString[1]);
            }
            else if (splitString[0] == "DevourDamage")
            {
                ability.devourDamage = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "DevourEscapeHits")
            {
                ability.devourEscapeHits = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "AbilitiesPhase")
            {
                ability.abilitiesPhase = bool.Parse(splitString[1]);
            }
            else if (splitString[0] == "Damage")
            {
                ability.damage = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "DamageType")
            {
                ability.damageType = (Enumerations.DamageType)int.Parse(splitString[1]);
            }
            else if (splitString[0] == "IntervalsPerTurn")
            {
                ability.intervalsPerTurn = int.Parse(splitString[1]);
            }
            else if (splitString[0] == "NumberOfTurnIntervals")
            {
                ability.numberOfTurnIntervals = int.Parse(splitString[1]);
            }

            line = reader.ReadLine();
        }

        return ability;
    }
}