using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a <see cref="Spell"/> that deals no damage and applies a status effect.
/// </summary>
[CreateAssetMenu(fileName = "NewSpell", menuName = "Spell/Status Spell", order = 3)]
public class StatusSpell : Spell
{
    [Header("Base Damage Params")]

    public bool checkAccuracy = true;
    [Range(0, 100)]
    public float spellAccuracy = 100;
    
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

        var accuracyModifiers = user.GetAccuracyModifiers();

        foreach (float f in accuracyModifiers)
            hit *= f;

        // Flavor text here:

        return Random.value <= hit;
    }
}

