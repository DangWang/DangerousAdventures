using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

//TODO Move to seperate file
public class AOETile
{
    public Enumerations.AOETileType myType;
}

[Serializable]
public class AbilityEditor : EditorWindow
{
    private bool abilityLoaded;
    public string[] active_passive = {"Passive", "Active"};
    private Rect AOEBox;

    public AOETile[,] AOETiles = new AOETile[10, 10];
    public Color castingPointTileColor = Color.yellow;
    private Ability.AbilityStruct currentAbility;
    private Event e;
    public Color inTileColor = Color.red;
    private Vector2 mouseClickPos = Vector2.zero;
    private int number;

    private int oldNumberOfBuffs;

    //TODO Maybe move colors to tiles? REFACTOR ALL THIS SHIT
    public Color outTileColor = Color.grey;
    public Vector2 scrollPos = Vector2.zero;
    private Object source;
    private bool test = false;

    [MenuItem("DangerousAdventures/Abilities")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AbilityEditor));
    }

    private void OnEnable()
    {
        currentAbility = new Ability.AbilityStruct();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("General Options:");
        source = EditorGUILayout.ObjectField("Ability to edit:", source, typeof(Object));
        currentAbility.name = EditorGUILayout.TextField("Ability Name: ", currentAbility.name);
        EditorGUILayout.LabelField("Ability Description: ");
        currentAbility.description = EditorGUILayout.TextArea(currentAbility.description);
        currentAbility.isAOE = EditorGUILayout.Toggle("AOE:", currentAbility.isAOE);
        currentAbility.isActive = EditorGUILayout.Popup("Use:", currentAbility.isActive, active_passive);
        currentAbility.allowedTargets = EditorGUILayout.TagField("Targets:", currentAbility.allowedTargets);
        currentAbility.durationType =
            (Enumerations.SpellDuration) EditorGUILayout.EnumPopup("Duration Type:", currentAbility.durationType);
        if (currentAbility.durationType == Enumerations.SpellDuration.OverTime)
            currentAbility.duration = EditorGUILayout.IntField("Duration:", currentAbility.duration);
        currentAbility.chargesTurn = EditorGUILayout.IntField("Uses/Turn:", currentAbility.chargesTurn);
        currentAbility.targetPoint =
            (Enumerations.TargetPoint) EditorGUILayout.EnumPopup("Target Point:", currentAbility.targetPoint);
        if (currentAbility.isAOE)
        {
            e = Event.current;
            if (e.type == EventType.MouseDown)
            {
                mouseClickPos = e.mousePosition;
                //get tile
                if (mouseClickPos.y >= 216 && mouseClickPos.y <= 416 && mouseClickPos.x >= 0 && mouseClickPos.x <= 200)
                {
                    var tileRow = (int) (mouseClickPos.y - 216) / 20;
                    var tileColumn = (int) (mouseClickPos.x / 20);
                    AOETiles[tileRow, tileColumn].myType =
                        (Enumerations.AOETileType) (((int) AOETiles[tileRow, tileColumn].myType + 1) % 3);
                }

                Debug.Log(mouseClickPos);
            }

            EditorGUILayout.LabelField("AOE Grid and stuff!");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(400), GUILayout.Height(200));
            EditorGUI.DrawRect(new Rect(0, 0, 200, 200), Color.black);
            //TODO GET AOE BOX Position 216-416 along the y axis
            number = EditorGUI.IntField(new Rect(200, 0, 100, 20), "Test:", number);
            //test = EditorGUI.Toggle(new Rect(20,20,20,20),test);
            for (var i = 0; i < 10; i++)
            for (var j = 0; j < 10; j++)
            {
                if (AOETiles[i, j] == null)
                    AOETiles[i, j] = new AOETile();

                if (AOETiles[i, j].myType == Enumerations.AOETileType.Out)
                    EditorGUI.DrawRect(new Rect(j * 20 + 1, i * 20 + 1, 18, 18), outTileColor);
                else if (AOETiles[i, j].myType == Enumerations.AOETileType.In)
                    EditorGUI.DrawRect(new Rect(j * 20 + 1, i * 20 + 1, 18, 18), inTileColor);
                else if (AOETiles[i, j].myType == Enumerations.AOETileType.CastingPoint)
                    EditorGUI.DrawRect(new Rect(j * 20 + 1, i * 20 + 1, 18, 18), castingPointTileColor);
                else
                    EditorGUI.DrawRect(new Rect(j * 20 + 1, i * 20 + 1, 18, 18), Color.magenta);
            }

            if (abilityLoaded)
            {
                foreach (var coords in currentAbility.tilesInAOE)
                    AOETiles[coords[0], coords[1]].myType = Enumerations.AOETileType.In;
                AOETiles[currentAbility.tilesInAOE.ElementAt(0)[0], currentAbility.tilesInAOE.ElementAt(0)[1]].myType =
                    Enumerations.AOETileType.CastingPoint;
                abilityLoaded = false;
            }

            //BeginWindows();
            //ConversationsScript.DrawNodeEditor(helper, e, helper.entryNodes, helper.conversations);
            //EndWindows();
            EditorGUILayout.EndScrollView();
        }

        currentAbility.canBounce = EditorGUILayout.Toggle("Can Bounce:", currentAbility.canBounce);
        if (currentAbility.canBounce)
        {
            currentAbility.bounceReduction =
                EditorGUILayout.IntField("Bounce Effect Reduction:", currentAbility.bounceReduction);
            currentAbility.bounceNumber = EditorGUILayout.IntField("Max Bounces:", currentAbility.bounceNumber);
            currentAbility.bounceRange = EditorGUILayout.IntField("Bounce Range:", currentAbility.bounceRange);
        }

        currentAbility.damage = EditorGUILayout.IntField("Damage/Healing:", currentAbility.damage);
        if (currentAbility.damage > 0)
            currentAbility.damageType =
                (Enumerations.DamageType) EditorGUILayout.EnumPopup("Damage Type:", currentAbility.damageType);
        currentAbility.numberOfBuffsDebuffs =
            EditorGUILayout.IntField("Number of buffs/debuffs:", currentAbility.numberOfBuffsDebuffs);
        if (oldNumberOfBuffs != currentAbility.numberOfBuffsDebuffs)
        {
            oldNumberOfBuffs = currentAbility.numberOfBuffsDebuffs;
            currentAbility.buffsDebuffs = new Ability.BuffDebuff[currentAbility.numberOfBuffsDebuffs];
        }

        for (var i = 0; i < currentAbility.numberOfBuffsDebuffs; i++)
        {
            EditorGUILayout.LabelField("Buff/Debuff" + (i + 1));
            currentAbility.buffsDebuffs[i].disable =
                (Enumerations.DisableTypes) EditorGUILayout.EnumPopup("Disable:",
                    currentAbility.buffsDebuffs[i].disable);
            if (currentAbility.buffsDebuffs[i].disable == Enumerations.DisableTypes.Slow)
                currentAbility.buffsDebuffs[i].movementModifier = EditorGUILayout.IntField("Movement Modifier:",
                    currentAbility.buffsDebuffs[i].movementModifier);
            if (currentAbility.buffsDebuffs[i].disable == Enumerations.DisableTypes.Throw)
                currentAbility.buffsDebuffs[i].throwRange =
                    EditorGUILayout.IntField("Range:", currentAbility.buffsDebuffs[i].throwRange);
            currentAbility.buffsDebuffs[i].purge =
                (Enumerations.Purge) EditorGUILayout.EnumPopup("Purge:", currentAbility.buffsDebuffs[i].purge);
            currentAbility.buffsDebuffs[i].durationType =
                (Enumerations.SpellDuration) EditorGUILayout.EnumPopup("Duration Type:",
                    currentAbility.buffsDebuffs[i].durationType);
            if (currentAbility.buffsDebuffs[i].durationType == Enumerations.SpellDuration.OverTime)
                currentAbility.buffsDebuffs[i].duration =
                    EditorGUILayout.IntField("Duration:", currentAbility.buffsDebuffs[i].duration);
            currentAbility.buffsDebuffs[i].physicalAttackMod = EditorGUILayout.IntField("Physical Attack Mod:",
                currentAbility.buffsDebuffs[i].physicalAttackMod);
            currentAbility.buffsDebuffs[i].physicalDefenseMod = EditorGUILayout.IntField("Physical Defense Mod:",
                currentAbility.buffsDebuffs[i].physicalDefenseMod);
            currentAbility.buffsDebuffs[i].magicalAttackMod = EditorGUILayout.IntField("Magical Attack Mod:",
                currentAbility.buffsDebuffs[i].magicalAttackMod);
            currentAbility.buffsDebuffs[i].magicalDefenseMod = EditorGUILayout.IntField("Magical Defense Mod:",
                currentAbility.buffsDebuffs[i].magicalDefenseMod);
            currentAbility.buffsDebuffs[i].pureAttackMod =
                EditorGUILayout.IntField("Pure Attack Mod:", currentAbility.buffsDebuffs[i].pureAttackMod);
            currentAbility.buffsDebuffs[i].pureDefenseMod = EditorGUILayout.IntField("Pure Defense Mod:",
                currentAbility.buffsDebuffs[i].pureDefenseMod);
        }

        if (GUILayout.Button("Load", GUILayout.Width(100)))
        {
            var path = "Assets/Abilities/" + source.name + ".ability";
            var textReader = new StreamReader(path);
            if (textReader == null) Debug.LogError("Ability file not found.");
            currentAbility = LoadAbility(textReader);
            textReader.Close();
        }

        if (GUILayout.Button("Save", GUILayout.Width(100)))
        {
            var path = "Assets/Abilities/" + source.name + ".ability";
            var textWriter = new StreamWriter(path);
            SaveAbility(textWriter, currentAbility);
            textWriter.Close();
        }
    }

    //TODO Save AOETiles
    private Ability.AbilityStruct LoadAbility(StreamReader reader)
    {
        abilityLoaded = true;

        Ability.BuffDebuff LoadBuffDebuff(string s)
        {
            var my = new Ability.BuffDebuff();
            var splitString = s.Split(new[] {"_"}, StringSplitOptions.RemoveEmptyEntries);

            my.disable = (Enumerations.DisableTypes) int.Parse(splitString[0]);
            my.purge = (Enumerations.Purge) int.Parse(splitString[1]);
            my.durationType = (Enumerations.SpellDuration) int.Parse(splitString[2]);
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
            var splitString = s.Split(new[] {"(", ",", ")", " "}, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < splitString.Length; i += 2)
            {
                Debug.Log(splitString[i]);
                Debug.Log(splitString[i + 1]);
                my.Add(new int[2] {int.Parse(splitString[i]), int.Parse(splitString[i + 1])});
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
            var splitString = line.Split(new[] {":"}, StringSplitOptions.RemoveEmptyEntries);
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
                ability.durationType = (Enumerations.SpellDuration) int.Parse(splitString[1]);
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
                oldNumberOfBuffs = ability.numberOfBuffsDebuffs;
            }
            else if (splitString[0] == "BuffsDebuffs")
            {
                ability.buffsDebuffs[int.Parse(splitString[1])] = LoadBuffDebuff(splitString[2]);
            }
            else if (splitString[0] == "CastTarget")
            {
                ability.targetPoint = (Enumerations.TargetPoint) int.Parse(splitString[1]);
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
                ability.damageType = (Enumerations.DamageType) int.Parse(splitString[1]);
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

    //TODO Send Save to Aze
    private void SaveAbility(StreamWriter writer, Ability.AbilityStruct ability)
    {
        writer.WriteLine("Name: " + ability.name);
        writer.WriteLine("Description: " + ability.description);
        writer.WriteLine("IsAOE: " + ability.isAOE);
        writer.WriteLine("IsActive: " + ability.isActive);
        writer.WriteLine("ChargesTurn: " + ability.chargesTurn);
        writer.WriteLine("CastRange: " + ability.castRange);
        writer.WriteLine("AllowedTargets: " + ability.allowedTargets);
        writer.WriteLine("DurationType: " + (int) ability.durationType);
        writer.WriteLine("Duration: " + ability.duration);
        writer.WriteLine("DamageDispellEffect: " + ability.damageDispellEffect);
        writer.WriteLine("TransformSquare: " + ability.transformSquare);
        writer.WriteLine("SquareChange: " + ability.squareChange);
        writer.WriteLine("SquareChangeRange: " + ability.squareChangeRange);
        writer.WriteLine("NumberOfBuffsDebuffs: " + ability.numberOfBuffsDebuffs);
        for (var i = 0; i < ability.numberOfBuffsDebuffs; i++)
            writer.WriteLine("BuffsDebuffs:" + i + ":"
                             + (int) ability.buffsDebuffs[i].disable
                             + "_" + (int) ability.buffsDebuffs[i].purge
                             + "_" + (int) ability.buffsDebuffs[i].durationType
                             + "_" + ability.buffsDebuffs[i].throwRange
                             + "_" + ability.buffsDebuffs[i].duration
                             + "_" + ability.buffsDebuffs[i].movementModifier
                             + "_" + ability.buffsDebuffs[i].throwsTarget
                             + "_" + ability.buffsDebuffs[i].physicalAttackMod
                             + "_" + ability.buffsDebuffs[i].physicalDefenseMod
                             + "_" + ability.buffsDebuffs[i].magicalAttackMod
                             + "_" + ability.buffsDebuffs[i].magicalDefenseMod
                             + "_" + ability.buffsDebuffs[i].pureAttackMod
                             + "_" + ability.buffsDebuffs[i].pureDefenseMod
            );
        writer.WriteLine("CastTarget: " + (int) ability.targetPoint);
        int castX = -1, castY = -1;
        var aoeString = "";
        for (var i = 0; i < 10; i++)
        for (var j = 0; j < 10; j++)
            if (ability.isAOE && AOETiles[i, j].myType == Enumerations.AOETileType.CastingPoint)
            {
                castX = i;
                castY = j;
                aoeString += "(" + i + "," + j + ")";
            }

        for (var i = 0; i < 10; i++)
        for (var j = 0; j < 10; j++)
            if (ability.isAOE && AOETiles[i, j].myType == Enumerations.AOETileType.In)
                aoeString += "(" + i + "," + j + ")";
        writer.WriteLine("TilesInAOE: " + aoeString);
        writer.WriteLine("CorpseContinuation: " + ability.corpseContinuation);
        writer.WriteLine("CanBounce: " + ability.canBounce);
        writer.WriteLine("BounceReduction: " + ability.bounceReduction);
        writer.WriteLine("BounceNumber: " + ability.bounceNumber);
        writer.WriteLine("BounceRange: " + ability.bounceRange);
        writer.WriteLine("Damage: " + ability.damage);
        writer.WriteLine("DamageType: " + (int) ability.damageType);
    }
}