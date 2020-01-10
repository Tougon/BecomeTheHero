using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a <see cref="Spell"/> that can deal damage.
/// </summary>
[CreateAssetMenu(fileName = "NewSpell", menuName = "Spell/Offensive Spell", order = 2)]
public class OffensiveSpell : Spell
{
    [Header("Base Damage Params")]

    [Range(0, 250)]
    public float spellPower = 50.0f;

    public bool checkAccuracy = true;
    [Range(0, 100)]
    public float spellAccuracy = 100;


    [Header("Multi-Hit Params")]
    [Range(0, 10)]
    public int minNumberOfHits = 1;

    [Range(0, 10)]
    public int maxNumberOfHits = 1;
    // If checked, hit count will vary between the min and max number of hits.
    public bool varyHitCount = false;

    [Header("Critical Hit Params")]
    public bool canCritical = true;

    [Range(1, 24)]
    public int criticalHitChance = 16;


    /// <summary>
    /// Override for spell hit that factors accuracy
    /// </summary>
    public override bool CheckSpellHit(EntityController user, EntityController target)
    {
        if (!checkAccuracy)
            return true;

        float accuracy = user.GetAccuracy();
        float evasion = target.GetEvasion();

        float hit = spellAccuracy * (accuracy / evasion);

        // Get user's accuracy modifiers to decrease hit chance.
        var accuracyModifiers = user.GetAccuracyModifiers();

        foreach (float f in accuracyModifiers)
            hit *= f;

        // Flavor text here:
        return (Random.value * 100) <= hit;
    }


    /// <summary>
    /// Override for damage calculation that factors accuracy
    /// </summary>
    public override void CalculateDamage(EntityController user, EntityController target, SpellCast cast)
    {
        // Use the maximum number of htis
        int numHits = maxNumberOfHits;

        // If hit count is to be varied, randomize the number of hits here.
        if (varyHitCount)
        {
            if (minNumberOfHits > maxNumberOfHits)
                minNumberOfHits = maxNumberOfHits;

            // We may want to weight this eventually
            numHits = Random.Range(minNumberOfHits, maxNumberOfHits);
        }

        int[] result = new int[numHits];
        bool[] crits = new bool[numHits];

        // Run damage calculation for each hit
        for(int i=0; i<result.Length; i++)
        {
            float critChance = (float)criticalHitChance;

            // Indicate if this hit is critical
            bool critical = canCritical && (Random.value < (1.0f / critChance));
            crits[i] = critical;

            // Get attack and defense modifications
            float atkMod = user.GetAttackModifier();
            float defMod = target.GetDefenseModifier();

            // Negate negative attack and positive defense mods if the hit is critical
            atkMod = critical && atkMod < 1.0f ? 1.0f : atkMod;
            defMod = critical && defMod > 1.0f ? 1.0f : defMod;

            // Calculate damage
            float damage = ((((2 * DAMAGE_CONSTANT) / 5 + 2) * spellPower *
                (((float)user.GetAttack() * atkMod) / ((float)target.GetDefense() * defMod))) / 50.0f) + 1.0f;

            // Other modifier applied at the end. Includes critical hit and move specific modifiers
            var offenseModifiers = user.GetOffenseModifiers();
            var defenseModifiers = target.GetDefenseModifiers();

            // Modify the damage based on the active offensive and defensive modifiers
            foreach (float f in offenseModifiers)
                damage *= f;

            foreach (float d in defenseModifiers)
                damage *= d;

            damage *= Random.Range(0.85f, 1.0f);
            damage = critical ? damage * 1.5f : damage;

            result[i] = (int)damage;
        }

        // Set the damage and critical
        cast.SetDamage(result);
        cast.SetCritical(crits);
    }
}
