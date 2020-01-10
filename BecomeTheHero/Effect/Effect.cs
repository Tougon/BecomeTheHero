using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents an effect of an attack.
/// </summary>
[CreateAssetMenu(fileName = "NewEffect", menuName = "Effect", order = 3)]
public class Effect : ScriptableObject
{
    [SerializeField]
    private bool stackable;
    public bool castSuccess { get; private set; }
    public string effectName;
    // Current instance of effect calculations should be ran on
    public EffectInstance current;

    public AnimationSequenceObject activationAnimation;
    public AnimationSequenceObject deactivationAnimation;
    public AnimationSequenceObject miscAnimation;

    // Used for function callbacks
    public UnityEvent CheckSuccess;
    public UnityEvent CheckRemainActive;
    public UnityEvent OnActivate;
    public UnityEvent OnMoveSelected;
    public UnityEvent OnDeactivate;
    public UnityEvent OnTurnStart;
    public UnityEvent OnTurnEnd;

    public bool IsStackable() { return stackable; }


    /// <summary>
    /// Create an instance of this effect
    /// </summary>
    public EffectInstance CreateEventInstance(EntityController u, EntityController t, SpellCast s)
    {
        EffectInstance e = new EffectInstance();
        e.user = u;
        e.target = t;
        e.spell = s;
        e.effect = this;
        return e;
    }


    #region Check Functions

    /// <summary>
    /// Check if this effect should be applied
    /// </summary>
    public void CheckForCastSuccess()
    {
        castSuccess = true;
        CheckSuccess.Invoke();
    }


    /// <summary>
    /// Check if this effect should remain active
    /// </summary>
    public void CheckForRemainActive()
    {
        castSuccess = true;
        CheckRemainActive.Invoke();
    }


    public void IsActiveForLessThanXTurns(int limit)
    {
        castSuccess = !castSuccess ? false : current.numTurnsActive < limit;
    }


    public void IsUserLastMoveSuccessful()
    {
        castSuccess = !castSuccess ? false : current.user.actionResult.success;
    }


    public void IsUserLastMoveEqualToTarget (Spell s)
    {
        castSuccess = !castSuccess ? false : s == current.user.action;
    }


    public void IsUserHealthAbovePercent(float percent)
    {
        percent = Mathf.Clamp(percent, 0.0f, 1.0f);

        float healthPercent = ((float)current.user.GetCurrentHP() / (float)current.user.maxHP);
        castSuccess = !castSuccess ? false : healthPercent > percent;
    }


    public void IsCurrentMoveEqualToTarget(Spell s)
    {
        castSuccess = !castSuccess ? false : s == current.spell.spell;
    }

    #endregion


    #region Action Functions

    /// <summary>
    /// Sends a dialogue message to the <see cref="DialogueManager"/>
    /// </summary>
    public void SendDialogue(string dialogue)
    {
        dialogue.Replace("[user]", current.user.param.entityName);
        dialogue.Replace("[target]", current.target.param.entityName);
        EventManager.Instance.RaiseStringEvent(EventConstants.ON_DIALOGUE_QUEUE, dialogue);
    }


    public void ApplyEffectToUser()
    {
        current.user.ApplyEffect(current);
    }


    public void RemoveEffectFromUser()
    {
        current.user.RemoveEffect(current);
    }


    public void RemoveEffectFromUser(string name)
    {

    }


    public void ApplyEffectToTarget()
    {
        current.target.ApplyEffect(current);
    }


    public void RemoveEffectFromTarget()
    {

    }


    public void RemoveEffectFromTarget(string name)
    {

    }


    public void ApplyPropertyToUser(Effect property)
    {
        EffectInstance eff = property.CreateEventInstance(current.user, current.target, current.spell);
        eff.numTurnsActive = current.numTurnsActive;
        current.user.AddProperty(eff);
    }


    #region MP/HP Manipulation


    public void ModifyUserMPFromDamageDealt(string s)
    {
        string[] param = s.Split(',');
        ModifyUserMPFromDamageDealt(float.Parse(param[0]), int.Parse(param[1]), int.Parse(param[2]));
    }


    public void ModifyUserMPFromDamageDealt(float modifier, int min, int max)
    {
        int damage = current.spell.GetDamageApplied();
        damage = (int)(((float)damage) * modifier);

        damage = Mathf.Clamp(damage, min, max);
        current.user.ModifyMP(damage);
    }


    public void ModifyTargetMPFromDamageDealt(string s)
    {
        string[] param = s.Split(',');
        ModifyTargetMPFromDamageDealt(float.Parse(param[0]), int.Parse(param[1]), int.Parse(param[2]));
    }


    public void ModifyTargetMPFromDamageDealt(float modifier, int min, int max)
    {
        int damage = current.spell.GetDamageApplied();
        damage = (int)(((float)damage) * modifier);

        damage = Mathf.Clamp(damage, min, max);
        current.target.ModifyMP(damage);
    }


    public void ModifyUserMPFromDamageTaken(string s)
    {
        string[] param = s.Split(',');
        ModifyUserMPFromDamageTaken(float.Parse(param[0]), int.Parse(param[1]), int.Parse(param[2]));
    }


    public void ModifyUserMPFromDamageTaken(float modifier, int min, int max)
    {
        int damage = current.user.damageTaken;
        damage = (int)(((float)damage) * modifier);

        damage = Mathf.Clamp(damage, min, max);
        current.user.ModifyMP(damage);
    }


    public void ModifyTargetMPFromDamageTaken(string s)
    {
        string[] param = s.Split(',');
        ModifyTargetMPFromDamageTaken(float.Parse(param[0]), int.Parse(param[1]), int.Parse(param[2]));
    }


    public void ModifyTargetMPFromDamageTaken(float modifier, int min, int max)
    {
        int damage = current.target.damageTaken;
        damage = (int)(((float)damage) * modifier);

        damage = Mathf.Clamp(damage, min, max);
        current.target.ModifyMP(damage);
    }

    #endregion

    #region Stat Modifiers

    public void ApplyOffenseModifierToUser(float amt)
    {

    }


    public void RemoveOffenseModifierFromUser()
    {

    }


    public void RemoveOffenseModifierFromUser(string name)
    {

    }


    public void ApplyOffenseModifierToTarget(float amt)
    {

    }


    public void RemoveOffenseModifierFromTarget()
    {

    }


    public void RemoveOffenseModifierFromTarget(string name)
    {

    }


    public void ApplyDefenseModifierToUser(float amt)
    {
        current.user.AddDefenseModifier(amt, current.effect.effectName);
    }


    public void RemoveDefenseModifierFromUser()
    {
        current.user.RemoveDefenseModifier(current.effect.effectName);
    }


    public void RemoveDefenseModifierFromUser(string name)
    {
        current.user.RemoveDefenseModifier(name);
    }


    public void ApplyDefenseModifierToTarget(float amt)
    {

    }


    public void RemoveDefenseModifierFromTarget()
    {

    }


    public void RemoveDefenseModifierFromTarget(string name)
    {

    }


    public void ApplyAccuracyModifierToUser(float amt)
    {
        current.user.AddAccuracyModifier(amt, current.effect.effectName);
    }


    public void ApplyAttackAccuracyModifierToUser(float amt)
    {
        float result = 1;

        for (int i = 0; i < current.numTurnsActive; i++)
            result *= amt;

        current.user.AddAccuracyModifier(result, current.effect.effectName);
    }


    public void RemoveAccuracyModifierFromUser()
    {
        current.user.RemoveAccuracyModifier(current.effect.effectName);
    }


    public void RemoveAccuracyModifierFromUser(string name)
    {
        current.user.RemoveAccuracyModifier(name);
    }

    #endregion

    #endregion
}


/// <summary>
/// Represents an instance of an <see cref="Effect"/>
/// </summary>
public class EffectInstance
{
    public int numTurnsActive;
    public bool castSuccess { get; private set; }

    // Effect this instance is linked to
    public Effect effect;

    public EntityController user;
    public EntityController target;

    public SpellCast spell;


    #region Checks

    public void CheckSuccess()
    {
        effect.current = this;
        effect.CheckForCastSuccess();
        castSuccess = effect.castSuccess;
    }


    public void CheckRemainActive()
    {
        if (castSuccess)
        {
            effect.current = this;
            effect.CheckForRemainActive();
            castSuccess = effect.castSuccess;
        }
    }


    public void OnActivate()
    {
        if (castSuccess)
        {
            effect.current = this;
            effect.OnActivate.Invoke();
        }
    }


    public void OnDeactivate()
    {
        if (castSuccess)
        {
            effect.current = this;
            effect.OnDeactivate.Invoke();
        }
    }


    public void OnTurnStart()
    {
        if (castSuccess)
        {
            effect.current = this;
            effect.OnTurnStart.Invoke();
        }
    }


    public void OnMoveSelected()
    {
        if (castSuccess)
        {
            effect.current = this;
            effect.OnMoveSelected.Invoke();
        }
    }


    public void OnTurnEnd()
    {
        if (castSuccess)
        {
            effect.current = this;
            effect.OnTurnEnd.Invoke();
        }
    }

    #endregion
}
