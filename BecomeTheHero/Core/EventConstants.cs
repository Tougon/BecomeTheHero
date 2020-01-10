using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains string representations of each event name.
/// This class was constructed in order to collect all events
/// into the same place.
/// </summary>
public static class EventConstants
{
    #region Game Event Constants
    public const string ON_BATTLE_BEGIN = "OnBattleBegin";
    public const string ON_TURN_BEGIN = "OnTurnBegin";
    public const string ON_MOVE_SELECTED = "OnMoveSelected";
    public const string ON_TURN_END = "OnTurnEnd";
    public const string ON_PLAYER_DEFEAT = "OnPlayerDefeat";
    public const string BEGIN_SEQUENCE = "BeginSequence";
    public const string ATTACK_SELECTED = "AttackSelected";
    public const string DEFEND_SELECTED = "DefendSelected";
    #endregion

    #region Bool Event Constants
    public const string SET_UI_INPUT_STATE = "SetUIInputState";
    #endregion

    #region Int Event Constants
    public const string SPELL_SELECTED = "SpellSelected";
    #endregion

    #region String Event Constants
    public const string ON_DIALOGUE_QUEUE = "OnDialogueQueue";
    #endregion

    #region GameObject Event Constants
    #endregion

    #region Entity Controller Event Constants
    public const string ON_PLAYER_INITIALIZE = "OnPlayerInitialize";
    public const string ON_ENEMY_INITIALIZE = "OnEnemyInitialize";
    public const string ON_SPELL_LIST_INITIALIZE = "OnSpellListInitialize";
    public const string ON_ENEMY_DEFEAT = "OnEnemyDefeat";
    #endregion

    #region Sequence Event Constants
    public const string ON_SEQUENCE_QUEUE = "OnSequenceQueue";
    #endregion
}
