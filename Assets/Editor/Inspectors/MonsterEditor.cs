using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Mirror
{
    [CustomEditor(typeof(script_MonsterController))]
    public class MonsterEditor : Editor
    {
        private AnimBool hasResistances;
        private int index;
        private script_MonsterController myScript;
        private int previousAbilityCount;
        public string[] races = {"Orc", "Human", "Elf"};

        private AnimBool showAbilityFields;

        private void OnEnable()
        {
            myScript = (script_MonsterController) target;
            showAbilityFields = new AnimBool(true);
            showAbilityFields.valueChanged.AddListener(Repaint);
            hasResistances = new AnimBool(true);
            hasResistances.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("General Options:");
            myScript.gameObject.name = EditorGUILayout.TextField("Monster Name: ", myScript.gameObject.name);
            EditorGUILayout.LabelField("Monster Description: ");
            myScript.monsterDescription = EditorGUILayout.TextArea(myScript.monsterDescription);
            index = EditorGUILayout.Popup("Race:", index, races);

            myScript.myHealth = EditorGUILayout.IntField("Health:", myScript.myHealth);
            myScript.attackDice = EditorGUILayout.IntField("Attack Dice:", myScript.attackDice);
            myScript.defenseDice = EditorGUILayout.IntField("Defense Dice:", myScript.defenseDice);
            myScript.range = EditorGUILayout.IntField("Max Range:", myScript.range);
            myScript.unrestrictedMovement =
                EditorGUILayout.Toggle("Unrestricted Movement", myScript.unrestrictedMovement);
            hasResistances.target = EditorGUILayout.ToggleLeft("Has Resistance:", hasResistances.target);
            if (EditorGUILayout.BeginFadeGroup(hasResistances.faded))
            {
                myScript.physicalResistance =
                    EditorGUILayout.IntField("Physical Resistance (dice)", myScript.physicalResistance);
                myScript.magicalResistance =
                    EditorGUILayout.IntField("Magical Resistance (dice)", myScript.magicalResistance);
                myScript.pureResistance = EditorGUILayout.IntField("Pure Resistance (dice)", myScript.pureResistance);
            }

            EditorGUILayout.EndFadeGroup();
            myScript.canCarryItems = EditorGUILayout.Toggle("Able to carry items:", myScript.canCarryItems);
            myScript.monsterCost = EditorGUILayout.IntField("Monster Cost:", myScript.monsterCost);
            myScript.respawnTimer = EditorGUILayout.IntField("Respawn Timer (in turns):", myScript.respawnTimer);
            myScript.monsterTier = EditorGUILayout.IntSlider("Tier:", myScript.monsterTier, 1, 5);
            myScript.goldDropped = EditorGUILayout.IntField("Gold Dropped:", myScript.goldDropped);
            myScript.goldRandomOffset = EditorGUILayout.IntField("Gold Random Offset:", myScript.goldRandomOffset);

            previousAbilityCount = myScript.abilityCount;
            myScript.abilityCount = EditorGUILayout.IntField("Ability Count:", myScript.abilityCount);
            if (previousAbilityCount != myScript.abilityCount) myScript.abilities = new Object[myScript.abilityCount];
            showAbilityFields.target = EditorGUILayout.ToggleLeft("Show abilities:", showAbilityFields.target);
            if (EditorGUILayout.BeginFadeGroup(showAbilityFields.faded))
                for (var i = 0; i < myScript.abilityCount; i++)
                    myScript.abilities[i] =
                        EditorGUILayout.ObjectField("Ability" + i, myScript.abilities[i], typeof(Object));
            EditorGUILayout.EndFadeGroup();
        }
    }
}