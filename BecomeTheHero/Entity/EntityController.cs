using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Houses an <see cref="Entity"/> and allows for gameplay operations to be performed on it.
/// </summary>
public class EntityController : MonoBehaviour, IComparable<EntityController>
{
    protected TurnManager turnManger;

    [SerializeField]
    protected Entity current;
    public EntityController target { get; set; }
    public EntityParams param { get; private set; }

    public Spell action { get; protected set; }
    public SpellCast actionResult { get; set; }

    private Animator anim;
    private SpriteRenderer sprite;

    public bool dead { get; protected set; }
    
    public int damageTaken { get; private set; }
    public int maxHP { get; private set; }
    public int maxMP { get; private set; }

    // Stat modification
    private int atkStage = 0;
    private int defStage = 0;
    private int spdStage = 0;
    private int evasionStage = 0;
    private int accuracyStage = 0;

    private Dictionary<string, float> offenseModifiers = new Dictionary<string, float>();
    private Dictionary<string, float> defenseModifiers = new Dictionary<string, float>();
    private Dictionary<string, float> accuracyModifiers = new Dictionary<string, float>();
    private List<EffectInstance> effects = new List<EffectInstance>();
    private List<EffectInstance> properties = new List<EffectInstance>(); 


    // Start is called before the first frame update
    protected void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        InitEntityController();
    }


    /// <summary>
    /// Initializes an entity controller
    /// </summary>
    public void InitEntityController()
    {
        if (current != null)
        {
            // Set the params to the entity's params
            param = new EntityParams();
            param.entityHP = ((current.vals.entityHP * 2 * Spell.DAMAGE_CONSTANT) / 100) + Spell.DAMAGE_CONSTANT + 10;
            param.entityMP = current.vals.entityMP;
            param.entityAtk = ((current.vals.entityAtk * 2 * Spell.DAMAGE_CONSTANT) / 100) + 5;
            param.entityDef = ((current.vals.entityDef * 2 * Spell.DAMAGE_CONSTANT) / 100) + 5;
            param.entityName = current.vals.entityName;
            param.entitySpeed = ((current.vals.entitySpeed * 2 * Spell.DAMAGE_CONSTANT) / 100) + 5;

            maxHP = param.entityHP;
            maxMP = param.entityMP;

            // Reset stats and effects
            atkStage = 0;
            defStage = 0;
            spdStage = 0;
            evasionStage = 0;
            accuracyStage = 0;

            effects = new List<EffectInstance>();
            properties = new List<EffectInstance>();
            offenseModifiers = new Dictionary<string, float>();
            defenseModifiers = new Dictionary<string, float>();
            accuracyModifiers = new Dictionary<string, float>();
        }
    }


    #region Animation Control

    /// <summary>
    /// Sets animation to the given trigger
    /// </summary>
    public void SetAnimation(string val)
    {
        anim.SetTrigger(val);
    }


    /// <summary>
    /// Sets animation to the given trigger with the given bool
    /// </summary>
    public void SetAnimationState(string val, bool b)
    {
        anim.SetBool(val, b);
    }


    /// <summary>
    /// Starts a color tween
    /// </summary>
    public void SetColorTween(Color c, float duration)
    {
        sprite.DOColor(c, duration);
    }


    /// <summary>
    /// Returns sprite renderer component
    /// </summary>
    public SpriteRenderer GetSpriteRenderer()
    {
        return sprite;
    }


    /// <summary>
    /// Changes the speed of the animator
    /// </summary>
    public void FrameSpeedModify(float t)
    {
        anim.speed = t;
    }
    #endregion


    /// <summary>
    /// Sets an entity to the given entity
    /// </summary>
    public void SetEntity(Entity e)
    {
        current = e;
        InitEntityController();
        dead = false;
    }


    /// <summary>
    /// Apply damage to this entity
    /// </summary>
    public void ApplyDamage(int val)
    {
        // If dead, do not apply damage
        if (dead) return;

        param.entityHP -= val;

        if (param.entityHP <= 0)
        {
            OnDeath();
            dead = true;
        }
    }


    // Damage taken is reset at the beginning of every turn
    public void ResetDamageTaken() { damageTaken = 0; }
    public void IncreaseDamageTaken(int val) { damageTaken += val; }


    /// <summary>
    /// Modify MP by the given amount
    /// </summary>
    public void ModifyMP(int amt)
    {
        param.entityMP += amt;

        param.entityMP = Mathf.Clamp(param.entityMP, 0, maxMP);
    }


    /// <summary>
    /// Skeleton function for death handling
    /// </summary>
    protected virtual void OnDeath()
    {
        // Play some kind of animation idk
    }


    #region Getters

    public Entity GetEntity()
    {
        return current;
    }


    public int GetCurrentHP()
    {
        return param.entityHP;
    }


    public int GetCurrentMP()
    {
        return param.entityMP;
    }


    // Accuracy / Evasion calcs go here
    public float GetAccuracy()
    {
        return 1;
    }


    public float GetEvasion()
    {
        return 1;
    }


    /// <summary>
    /// Converts a stat stage to a multiplier
    /// </summary>
    /// <returns>The multipier corresponding to a given stat</returns>
    private float GetStatModifier(int amt)
    {
        return Mathf.Max(2.0f, 2 + (float)amt) / Mathf.Max(2.0f, 2 - (float)amt);
    }
    

    public int GetAttack() { return param.entityAtk; }
    public float GetAttackModifier() { return GetStatModifier(atkStage); }


    public int GetDefense() { return param.entityAtk; }
    public float GetDefenseModifier() { return GetStatModifier(defStage); }


    public int GetSpeed() { return param.entitySpeed; }
    public float GetSpeedModifier() { return GetStatModifier(spdStage); }


    public TurnManager GetTurnManager()
    {
        return turnManger;
    }

    #endregion


    public void SetTurnManager(TurnManager tm)
    {
        turnManger = tm;
    }


    #region Action

    public virtual void SelectAction()
    {
        action = current.moveList[UnityEngine.Random.Range(0, current.moveList.Count)];
    }


    public virtual void SelectAction(int index)
    {
        index = Mathf.Clamp(index, 0, current.moveList.Count);
        action = current.moveList[index];
    }


    public void ResetAction()
    {
        action = null;
        actionResult = null;
    }

    #endregion

    /// <summary>
    /// Heavy WIP, but this handles effect stuff
    /// </summary>
    #region Effects


    /// <summary>
    /// Execute effects when the turn begins
    /// </summary>
    public void ExecuteTurnStartEffects()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            EffectInstance eff = effects[i];
            eff.numTurnsActive++;
            eff.OnTurnStart();

            // Prevent skipping over any effects if an effect is removed
            if (!effects.Contains(eff)) i--;
        }
    }


    /// <summary>
    /// Execute effects when the move is selected
    /// </summary>
    public void ExecuteMoveSelectedEffects()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            EffectInstance eff = effects[i];
            eff.OnMoveSelected();

            // Prevent skipping over any effects if an effect is removed
            if (!effects.Contains(eff)) i--;
        }
    }


    /// <summary>
    /// Check if each effect should remain active
    /// </summary>
    public void ExecuteRemainActiveCheck()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            EffectInstance eff = effects[i];
            eff.CheckRemainActive();

            if (!eff.castSuccess)
            {
                RemoveEffect(eff);
                i--;
            }
        }
    }


    /// <summary>
    /// Execute effects when the turn ends
    /// </summary>
    public void ExecuteTurnEndEffects()
    {
        for(int i=0; i<effects.Count; i++)
        {
            EffectInstance eff = effects[i];
            eff.OnTurnEnd();

            // Prevent skipping over any effects if an effect is removed
            if (!effects.Contains(eff)) i--;
        }
    }


    /// <summary>
    /// Applies an effect
    /// </summary>
    public void ApplyEffect(EffectInstance eff)
    {
        if (eff.effect.IsStackable())
        {
            // Handle stacking (reset duration/add buffs)
        }
        else if(!effects.Exists(f => f.effect.effectName == eff.effect.effectName))
            effects.Add(eff);
    }


    /// <summary>
    /// Removes an effect
    /// </summary>
    public void RemoveEffect(EffectInstance eff)
    {
        if (effects.Contains(eff))
        {
            eff.OnDeactivate();
            effects.Remove(eff);
        }
    }


    /// <summary>
    /// Removes an effect based on its name
    /// </summary>
    public void RemoveEffect(string s)
    {
        EffectInstance eff = effects.Find(f => f.effect.effectName == s);

        if (eff != null)
        {
            eff.OnDeactivate();
            effects.Remove(eff);
        }
    }

    #endregion


    #region Stat Modification

    /// <summary>
    /// Adds an offensive modifier with a given key
    /// </summary>
    public void AddOffenseModifier(float amt, string key)
    {
        offenseModifiers.Add(key, amt);
    }

    /// <summary>
    /// Removes an offensive modifier with a given key
    /// </summary>
    public void RemoveOffenseModifier(string key)
    {
        offenseModifiers.Remove(key);
    }

    /// <summary>
    /// Adds an defensive modifier with a given key
    /// </summary>
    public void AddDefenseModifier(float amt, string key)
    {
        defenseModifiers.Add(key, amt);
    }

    /// <summary>
    /// Removes an defensive modifier with a given key
    /// </summary>
    public void RemoveDefenseModifier(string key)
    {
        defenseModifiers.Remove(key);
    }

    /// <summary>
    /// Adds an accuracy modifier with a given key
    /// </summary>
    public void AddAccuracyModifier(float amt, string key)
    {
        accuracyModifiers.Add(key, amt);
    }

    /// <summary>
    /// Removes an accuracy modifier with a given key
    /// </summary>
    public void RemoveAccuracyModifier(string key)
    {
        accuracyModifiers.Remove(key);
    }


    public Dictionary<string, float>.ValueCollection GetOffenseModifiers()
    {
        return offenseModifiers.Values;
    }

    public Dictionary<string, float>.ValueCollection GetDefenseModifiers()
    {
        return defenseModifiers.Values;
    }

    public Dictionary<string, float>.ValueCollection GetAccuracyModifiers()
    {
        return accuracyModifiers.Values;
    }

    #endregion


    #region Properties

    public void AddProperty(EffectInstance eff)
    {
        properties.Add(eff);
    }


    public List<EffectInstance> GetProperties()
    {
        return properties;
    }


    public void ClearProperties()
    {
        foreach (EffectInstance eff in properties)
            eff.OnDeactivate();

        properties.Clear();
    }

    #endregion


    #region Compare

    /// <summary>
    /// Compares the speed of two entities
    /// </summary>
    /// <param name="obj">EntityController to compare to</param>
    /// <returns>1 if this object is slower, -1 if this it is faster, random otherwise.</returns>
    public int CompareTo(EntityController other)
    {
        int speedA = GetSpeed();
        int speedB = other.GetSpeed();

        if (speedA > speedB)
            return -1;
        else if (speedB > speedA)
            return 1;

        float rand = UnityEngine.Random.value;

        if (rand > 0.5f)
            return -1;
        else
            return 1;
    }

    #endregion
}
