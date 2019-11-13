using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO Get finished from Aze
public class Ability : MonoBehaviour
{
    public struct BuffDebuff
    {
        public Enumerations.DisableTypes disable;
        public Enumerations.Purge purge;
        public Enumerations.SpellDuration durationType;
        public int throwRange;
        public int duration;
        public int movementModifier;
        public bool throwsTarget; //TODO Put it somewhere more intuitive
    }
    public struct AbilityStruct
    {
        public string name;
        public string description;
        public bool isAura;
        public int isActive; //int because I want a dropdown menu in the Ability Editor
        public int castRange;
        public int chargesTurn;
        public string allowedTargets; //tags
        public Enumerations.SpellDuration durationType;
        public int duration;
        public bool damageDispellEffect; /* */
        public int transformSquare; /* */
        public int squareChange; /* */
        public int squareChangeRange; /* */
        public int numberOfBuffsDebuffs;
        public BuffDebuff[] buffsDebuffs;
        public Enumerations.CastTarget castTarget; //CastType
        public bool corpseContinuation;/* */
        public bool canBounce; 
        public int bounceReduction; //effect reduction
        public int bounceNumber;
        public int bounceRange;
        public bool rejuvenation;/* */
        public int rejuvenationTime;/* */
        public bool spawnWard;/* */
        public int wardRange;/* */
        public bool wardInvulnerability;/* */
        public int wardAbilities;/* */
        public bool reincarnation;/* */
        public int reincarnationTime;/* */
        public int reincarnationHealth;/* */
        public bool deathProtection;/* */
        public int deathProtectionDuration;/* */
        public int deathProtectionLifeCycle;/* */
        public int deathProtectionIgnore;/* */
        public bool summoning;/*/ */
        public int summons;/* */
        public int numberOfSummons;/* */
        public bool devour;/* */
        public int devourDamage;/* */
        public int devourEscapeHits;/* */
        public bool abilitiesPhase;/* */
        public Enumerations.DamageType damageType;
        public int damage;//TODO //because healing is negative damage modifiers should work only if it's positive
        public int intervalsPerTurn;/* */
        public int numberOfTurnIntervals;/* */
    }
}
 