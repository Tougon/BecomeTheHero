using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityController
{
    [SerializeField]
    private Spell attack;

    [SerializeField]
    private Spell defend;

    [SerializeField]
    private SpellList spellList;
    private List<Spell> availableSpells = new List<Spell>(4);

    [SerializeField]
    private int amountMPGainPerTurn = 5;

    
    new void Awake()
    {
        base.Awake();

        EventManager.Instance.GetGameEvent(EventConstants.ON_TURN_BEGIN).AddListener(OnTurnBegin);

        EventManager.Instance.GetGameEvent(EventConstants.ATTACK_SELECTED).AddListener(SetActionToAttack);
        EventManager.Instance.GetGameEvent(EventConstants.DEFEND_SELECTED).AddListener(SetActionToDefend);
        EventManager.Instance.GetIntEvent(EventConstants.SPELL_SELECTED).AddListener(SetActionToSpell);
    }

    // Start is called before the first frame update
    void Start()
    {
        // initialize the player 
        EventManager.Instance.RaiseEntityControllerEvent(EventConstants.ON_PLAYER_INITIALIZE, this);
    }


    public void OnTurnBegin()
    {
        ModifyMP(amountMPGainPerTurn);
        PopulateSpellList();
    }


    public void PopulateSpellList()
    {
        availableSpells.Clear();

        for (int i = 0; i < 4; i++)
        {
            availableSpells.Add(spellList.spells[Random.Range(0, spellList.spells.Count)]);
        }

        EventManager.Instance.RaiseEntityControllerEvent(EventConstants.ON_SPELL_LIST_INITIALIZE, this);
    }


    public List<Spell> GetAvailableSpells()
    {
        return availableSpells;
    }


    /// <summary>
    /// Call death event
    /// </summary>
    protected override void OnDeath()
    {
        string dialogueSeq = param.entityName + " falls...";
        EventManager.Instance.RaiseStringEvent(EventConstants.ON_DIALOGUE_QUEUE, dialogueSeq);
    }


    #region Action Selection

    public void SetActionToSpell(int index)
    {
        index = Mathf.Clamp(index, 0, availableSpells.Count);
        action = availableSpells[index];

        // Don't do this if the spell affects the user.
        SetTarget();
    }


    public void SetActionToAttack()
    {
        action = attack;

        SetTarget();
    }


    public void SetActionToDefend()
    {
        action = defend;
        target = this;
    }

    
    public void SetTarget()
    {
        if (turnManger.GetNumEntities() > 2)
        {
            // This is where we add targeting options
        }
        else
            target = turnManger.GetOther(this);
    }

    #endregion
    
    void OnDestroy()
    {
        EventManager.Instance.GetGameEvent(EventConstants.ATTACK_SELECTED).RemoveListener(SetActionToAttack);
        EventManager.Instance.GetGameEvent(EventConstants.DEFEND_SELECTED).RemoveListener(SetActionToDefend);
        EventManager.Instance.GetIntEvent(EventConstants.SPELL_SELECTED).RemoveListener(SetActionToSpell);
    }
}
