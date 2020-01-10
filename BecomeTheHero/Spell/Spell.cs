using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents any action that can be taken on a turn.
/// </summary>
[CreateAssetMenu(fileName = "NewSpell", menuName = "Spell/Flavor Spell", order = 1)]
public class Spell : ScriptableObject
{
    public const int DAMAGE_CONSTANT = 50;

    public string spellName;
    public int spellCost;
    public AnimationSequenceObject spellAnimation;
    public List<SpellEffectChance> spellEffects; // Effects that can be invoked by the spell itself
    public List<Effect> spellProperties; // Used to modify the damage roll


    /// <summary>
    /// Returns an instance of this spell using the spell data to calculate damage and effects
    /// </summary>
    public SpellCast Cast(EntityController user, EntityController target)
    {
        // Result of the cast spell
        SpellCast result = new SpellCast();

        result.spell = this;
        result.user = user;
        result.target = target;

        // Check for MP. If MP is inadequate, don't proceed.
        result.success = CheckMP(user);

        if (result.success)
        {
            // Check for additional requirements. If they aren't met, don't proceed.
            result.success = CheckCanCast(user, target);

            if (result.success)
            {
                // Apply properties before dealing damage, as properties may affect damage or accuracy.
                List<EffectInstance> properties = new List<EffectInstance>();

                // Activate all properties on this spell
                foreach (Effect e in spellProperties)
                {
                    if (e != null && !properties.Exists(f => f.effect == e) || (properties.Exists(f => f.effect == e) && e.IsStackable()))
                    {
                        EffectInstance eff = e.CreateEventInstance(user, target, result);
                        eff.CheckSuccess();
                        eff.OnActivate();
                        properties.Add(eff);
                    }
                }

                // Get the user's active propeties. This will differ from the above list.
                List<EffectInstance> userProperties = user.GetProperties();

                foreach (EffectInstance ef in userProperties)
                {
                    ef.CheckSuccess();
                    ef.OnActivate();
                    properties.Add(ef);
                }

                // Check for spell hit. If spell misses, don't proceed.
                result.success = CheckSpellHit(user, target);

                if (result.success)
                {
                    // Calculate damage
                    CalculateDamage(user, target, result);

                    // If damage is 0, check to see if any effects were applied.
                    if (result.GetDamage() == 0)
                        result.success = result.GetEffectProcSuccess();
                }

                // Deactivate all properties
                foreach (EffectInstance e in properties)
                    e.OnDeactivate();

                // Clear properties from player
                user.ClearProperties();
            }
        }
        
        // Return spell cast.
        return result;
    }


    /// <summary>
    /// Checks if the user has enough MP to cast this spell.
    /// </summary>
    public virtual bool CheckMP(EntityController user)
    {
        if(user.GetCurrentMP() >= spellCost)
        {
            user.ModifyMP(-spellCost);
            return true;
        }

        return false;
    }


    /// <summary>
    /// Skeleton function. This checks additional requirements for the spell's success.
    /// </summary>
    public virtual bool CheckCanCast(EntityController user, EntityController target)
    {
        return true;
    }


    /// <summary>
    /// Skeleton function. This checks if the spell will hit the target.
    /// </summary>
    public virtual bool CheckSpellHit(EntityController user, EntityController target)
    {
        return true;
    }


    /// <summary>
    /// Skeleton function. Calculates the damage dealt by the spell.
    /// </summary>
    public virtual void CalculateDamage(EntityController user, EntityController target, SpellCast cast)
    {
        int[] result = { 0 };
        bool[] critical = { true };
        cast.SetDamage(result);
        cast.SetCritical(critical);
    }
}


/// <summary>
/// Represents a specific instance of a cast spell.
/// </summary>
public class SpellCast
{
    // Was this cast successful?
    public bool success = false;

    /// <summary>
    /// The spell that was cast.
    /// </summary>
    public Spell spell;

    /// <summary>
    /// The user of the spell.
    /// </summary>
    public EntityController user;
    /// <summary>
    /// The target of the spell.
    /// </summary>
    public EntityController target;

    /// <summary>
    /// Damage dealt by the spell
    /// </summary>
    private int[] damage;
    private int totalDamage = 0;
    /// <summary>
    /// Index of current hit. Used for animation processing.
    /// </summary>
    private int currentHit = 0;
    /// <summary>
    /// Indicates if any hit is critical
    /// </summary>
    public bool critical { get; private set; }
    /// <summary>
    /// Indicates which hit is critical
    /// </summary>
    private bool[] crits;

    /// <summary>
    /// Spell Effect params
    /// </summary>
    private List<EffectInstance> effects = new List<EffectInstance>();
    

    public virtual int GetDamage()
    {
        return totalDamage;
    }


    public virtual int GetDamage(int index)
    {
        return damage[index];
    }


    public virtual int GetDamageOfCurrentHit()
    {
        int result = GetDamage(currentHit);

        currentHit = currentHit < damage.Length ? currentHit + 1 : 0;

        return result;
    }


    /// <summary>
    /// Returns the amount of damage dealt by this hit, stopping when the target's HP is depleted.
    /// </summary>
    public virtual int GetDamageApplied()
    {
        int result = 0;

        if (damage == null)
            return result;

        for(int i=0; i<damage.Length; i++)
        {
            result += damage[i];

            if (target.GetCurrentHP() - result <= 0)
                return result;
        }

        return result;
    }
    

    /// <summary>
    /// Sets the damage of this SpellCast.
    /// For each hit of damage, roll for effect success.
    /// </summary>
    public virtual void SetDamage(int[] d)
    {
        damage = d;

        for (int i = 0; i < damage.Length; i++)
        {
            totalDamage += damage[i];

            for(int n = 0; n < spell.spellEffects.Count; n++)
            {
                Effect e = spell.spellEffects[i].GetEffect();

                if (e != null && !effects.Exists(f => f.effect == e) || (effects.Exists(f => f.effect == e) && e.IsStackable()))
                {
                    EffectInstance eff = e.CreateEventInstance(user, target, this);
                    eff.CheckSuccess();
                    effects.Add(eff);
                }
            }
        }
    }


    public virtual int GetNumHits()
    {
        return damage.Length;
    }


    /// <summary>
    /// Sets critical hit status.
    /// </summary>
    public virtual void SetCritical(bool[] c)
    {
        crits = c;

        for(int i=0; i<crits.Length; i++)
        {
            if (crits[i])
            {
                critical = true;
                return;
            }
        }
    }


    /// <summary>
    /// Returns true if any effect is invoked by this cast.
    /// </summary>
    public virtual bool GetEffectProcSuccess()
    {
        bool result = false;

        for(int i=0; i<effects.Count; i++)
        {
            effects[i].CheckSuccess();

            if (effects[i].castSuccess)
                result = true;
        }

        return result;
    }


    public virtual List<EffectInstance> GetEffects()
    {
        return effects;
    }
}

/// <summary>
/// Master class that handles effect distribution for spells.
/// This is slightly confusing so here's an example:
/// Say we have a spell that has a 80% chance to invoke an effect.
/// Half of the time, that effect should do one thing, the other half of the time, another.
/// The SpellEffectChance chance would be 0.8, while the effect chance is 0.5 for each.
/// Keep in mind this will NOT automatically split so be careful.
/// </summary>
[System.Serializable]
public class SpellEffectChance
{
    [System.Serializable]
    public class EffectChance
    {
        [Range(0, 1)]
        public float chance = 1.0f;
        public Effect effect;
    }

    // Odds of this effect set being invoked.
    [Range(0, 1)]
    public float chance;

    public EffectChance[] effects;

    /// <summary>
    /// Returns a random effect from effects.
    /// </summary>
    public Effect GetEffect()
    {
        float rand = Random.value;
        float cur = 0;

        for(int i=0; i<effects.Length; i++)
        {
            cur += effects[i].chance;

            if (rand <= cur)
                return effects[i].effect;
        }

        return null;
    }
}
