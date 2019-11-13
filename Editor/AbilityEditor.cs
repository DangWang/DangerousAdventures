using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using System.CodeDom.Compiler;

//TODO Move to seperate file
public class AOETile
{
    public Enumerations.AOETileType myType;
}

[System.Serializable]
public class AbilityEditor : EditorWindow
{
    UnityEngine.Object source;
    Ability.AbilityStruct currentAbility;
    int oldNumberOfBuffs;
    public Vector2 scrollPos = Vector2.zero;
    Vector2 mouseClickPos = Vector2.zero;
    int number = 0;
    bool test = false;
    Event e;
    Rect AOEBox;

    public AOETile[,] AOETiles = new AOETile[10,10];
    //TODO Maybe move colors to tiles? REFACTOR ALL THIS SHIT
    public Color outTileColor = Color.grey;
    public Color inTileColor = Color.red;
    public Color castingPointTileColor = Color.yellow;
    public string[] active_passive = new string[] { "Passive", "Active"};

    [MenuItem("DangerousAdventures/Abilities")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AbilityEditor));
    }

    private void OnEnable()
    {
        currentAbility = new Ability.AbilityStruct();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("General Options:");
        source = EditorGUILayout.ObjectField("Ability to edit:", source, typeof(UnityEngine.Object));
        currentAbility.name = EditorGUILayout.TextField("Ability Name: ", currentAbility.name);
        EditorGUILayout.LabelField("Ability Description: ");
        currentAbility.description = EditorGUILayout.TextArea(currentAbility.description);
        currentAbility.isAura = EditorGUILayout.Toggle("Aura:", currentAbility.isAura);
        currentAbility.isActive = EditorGUILayout.Popup("Use:", currentAbility.isActive, active_passive);
        currentAbility.allowedTargets = EditorGUILayout.TagField("Targets:", currentAbility.allowedTargets);
        currentAbility.durationType = (Enumerations.SpellDuration)EditorGUILayout.EnumPopup("Duration Type:", currentAbility.durationType);
        if(currentAbility.durationType == Enumerations.SpellDuration.OverTime)
        {
            currentAbility.duration = EditorGUILayout.IntField("Duration:", currentAbility.duration);
        }
        currentAbility.chargesTurn = EditorGUILayout.IntField("Uses/Turn:", currentAbility.chargesTurn);
        currentAbility.castTarget = (Enumerations.CastTarget)EditorGUILayout.EnumPopup("Cast Target:", currentAbility.castTarget);
        if(currentAbility.castTarget == Enumerations.CastTarget.AOE)
        {
            e = Event.current;
            if(e.type == EventType.MouseDown)
            {
                mouseClickPos = e.mousePosition;
                //get tile
                if(mouseClickPos.y >= 216 && mouseClickPos.y <= 416 && mouseClickPos.x >= 0 && mouseClickPos.x <= 200)
                {
                    int tileRow = (int)((int)(mouseClickPos.y - 216) / 20);
                    int tileColumn = (int)(mouseClickPos.x / 20);
                    AOETiles[tileRow,tileColumn].myType = (Enumerations.AOETileType)(((int)AOETiles[tileRow,tileColumn].myType + 1) % 3);
                }
                Debug.Log(mouseClickPos);
            }
            EditorGUILayout.LabelField("AOE Grid and stuff!");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(400), GUILayout.Height(200));
            EditorGUI.DrawRect(new Rect(0,0,200,200), Color.black);
            //TODO GET AOE BOX Position 216-416 along the y axis
            number = EditorGUI.IntField(new Rect(200,0,100,20), "Test:", number);
            //test = EditorGUI.Toggle(new Rect(20,20,20,20),test);
            for(int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    if(AOETiles[i,j] == null)
                        AOETiles[i,j] = new AOETile();

                    if(AOETiles[i,j].myType == Enumerations.AOETileType.Out)
                        EditorGUI.DrawRect(new Rect(j * 20 + 1,i * 20 + 1, 18, 18), outTileColor);
                    else if(AOETiles[i,j].myType == Enumerations.AOETileType.In)
                        EditorGUI.DrawRect(new Rect(j * 20 + 1,i * 20 + 1, 18, 18), inTileColor);
                    else if(AOETiles[i,j].myType == Enumerations.AOETileType.CastingPoint) 
                        EditorGUI.DrawRect(new Rect(j * 20 + 1,i * 20 + 1, 18, 18), castingPointTileColor);
                    else
                        EditorGUI.DrawRect(new Rect(j * 20 + 1,i * 20 + 1, 18, 18), Color.magenta);
                }
            }
            //BeginWindows();
            //ConversationsScript.DrawNodeEditor(helper, e, helper.entryNodes, helper.conversations);
            //EndWindows();
            EditorGUILayout.EndScrollView();
        }
        currentAbility.canBounce = EditorGUILayout.Toggle("Can Bounce:", currentAbility.canBounce);
        if(currentAbility.canBounce)
        {
            currentAbility.bounceReduction = EditorGUILayout.IntField("Bounce Effect Reduction:", currentAbility.bounceReduction);
            currentAbility.bounceNumber = EditorGUILayout.IntField("Max Bounces:", currentAbility.bounceNumber);
            currentAbility.bounceRange = EditorGUILayout.IntField("Bounce Range:", currentAbility.bounceRange);
        }
        currentAbility.damage = EditorGUILayout.IntField("Damage/Healing:", currentAbility.damage);
        if(currentAbility.damage > 0)
        {
            currentAbility.damageType = (Enumerations.DamageType)EditorGUILayout.EnumPopup("Damage Type:", currentAbility.damageType);
        }
        currentAbility.numberOfBuffsDebuffs = EditorGUILayout.IntField("Number of buffs/debuffs:", currentAbility.numberOfBuffsDebuffs);
        if(oldNumberOfBuffs != currentAbility.numberOfBuffsDebuffs)
        {
            oldNumberOfBuffs = currentAbility.numberOfBuffsDebuffs;
            currentAbility.buffsDebuffs = new Ability.BuffDebuff[currentAbility.numberOfBuffsDebuffs];
        }
        for(int i = 0; i < currentAbility.numberOfBuffsDebuffs; i++)
        {
            EditorGUILayout.LabelField("Buff/Debuff" + (i+1).ToString());
            currentAbility.buffsDebuffs[i].disable = (Enumerations.DisableTypes)EditorGUILayout.EnumPopup("Damage Type:", currentAbility.buffsDebuffs[i].disable);
            if(currentAbility.buffsDebuffs[i].disable == Enumerations.DisableTypes.Slow)
            {
                currentAbility.buffsDebuffs[i].movementModifier = EditorGUILayout.IntField("Movement Modifier:", currentAbility.buffsDebuffs[i].movementModifier);
            }
            if(currentAbility.buffsDebuffs[i].disable == Enumerations.DisableTypes.Throw)
            {
                currentAbility.buffsDebuffs[i].throwRange = EditorGUILayout.IntField("Range:", currentAbility.buffsDebuffs[i].throwRange);
            }
            currentAbility.buffsDebuffs[i].purge = (Enumerations.Purge)EditorGUILayout.EnumPopup("Purge:", currentAbility.buffsDebuffs[i].purge);
            currentAbility.buffsDebuffs[i].durationType = (Enumerations.SpellDuration)EditorGUILayout.EnumPopup("Duration Type:", currentAbility.buffsDebuffs[i].durationType);
            if(currentAbility.buffsDebuffs[i].durationType == Enumerations.SpellDuration.OverTime)
            {
                currentAbility.buffsDebuffs[i].duration = EditorGUILayout.IntField("Duration:", currentAbility.duration);
            }
        }

        if (GUILayout.Button("Load", GUILayout.Width(100)))
        {
            string path = "Assets/Abilities/" + source.name + ".ability";
            StreamReader textReader = new StreamReader(path);
            if(textReader == null)
            {
                Debug.LogError("Ability file not found.");
            }
            currentAbility = LoadAbility(textReader);
            textReader.Close();
        }
        if (GUILayout.Button("Save", GUILayout.Width(100)))
        {
            string path = "Assets/Abilities/" + source.name + ".ability";
            StreamWriter textWriter = new StreamWriter(path);
            SaveAbility(textWriter, currentAbility);
            textWriter.Close();
        }
    }

    //TODO Save AOETiles
    private Ability.AbilityStruct LoadAbility(StreamReader reader)
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
                oldNumberOfBuffs = ability.numberOfBuffsDebuffs;
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
    //TODO Send Save to Aze
    private void SaveAbility(StreamWriter writer, Ability.AbilityStruct ability)
    {
        writer.WriteLine("Name: " + ability.name.ToString());
        writer.WriteLine("Description: " + ability.description.ToString());
        writer.WriteLine("Aura: " + ability.isAura.ToString());
        writer.WriteLine("IsActive: " + ability.isActive.ToString());
        writer.WriteLine("ChargesTurn: " + ability.chargesTurn.ToString());
        writer.WriteLine("CastRange: " + ability.castRange.ToString());
        writer.WriteLine("AllowedTargets: " + ability.allowedTargets.ToString());
        writer.WriteLine("DurationType: " + (int)ability.durationType);
        writer.WriteLine("DamageDispellEffect: " + ability.damageDispellEffect.ToString());
        writer.WriteLine("TransformSquare: " + ability.transformSquare.ToString());
        writer.WriteLine("SquareChange: " + ability.squareChange.ToString());
        writer.WriteLine("SquareChangeRange: " + ability.squareChangeRange.ToString());
        writer.WriteLine("NumberOfBuffsDebuffs: " + ability.numberOfBuffsDebuffs.ToString());
        for(int i = 0; i < ability.numberOfBuffsDebuffs; i++)
        {
            writer.WriteLine("BuffsDebuffs:" + i.ToString() + ":"
                             + (int)ability.buffsDebuffs[i].disable 
                             + "_" + (int)ability.buffsDebuffs[i].purge 
                             + "_" + (int)ability.buffsDebuffs[i].durationType 
                             + "_" + ability.buffsDebuffs[i].throwRange
                             + "_" + ability.buffsDebuffs[i].duration
                             + "_" + ability.buffsDebuffs[i].movementModifier
                             + "_" + ability.buffsDebuffs[i].throwsTarget.ToString());
        }
        writer.WriteLine("CastTarget: " + (int)ability.castTarget);
        writer.WriteLine("CorpseContinuation: " + ability.corpseContinuation.ToString());
        writer.WriteLine("CanBounce: " + ability.canBounce.ToString());
        writer.WriteLine("BounceReduction: " + ability.bounceReduction.ToString());
        writer.WriteLine("BounceNumber: " + ability.bounceNumber.ToString());
        writer.WriteLine("BounceRange: " + ability.bounceRange.ToString());
        writer.WriteLine("Damage: " + ability.damage.ToString());
        writer.WriteLine("DamageType: " + (int)ability.damageType);
    }
}