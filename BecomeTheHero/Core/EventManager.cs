using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ScriptableObjectArchitecture;

/// <summary>
/// Manages and contains all events utilized throughout the entire game.
/// 
/// To add a new Event, create the Event in its respective events folder,
/// add it to this scriptable objects' respective event list via inspector,
/// then add its name as a contstant string in <see cref="EventConstants"/>.
/// 
/// To raise any Event, simply call <see cref="RaiseGameEvent(string)"/>(example).
/// </summary>
[CreateAssetMenu(fileName = "Event Manager", menuName = "System/EventManager")]
public class EventManager : ScriptableObject
{
    #region Event Fields
    [SerializeField]
    [Header("Game Events")]
    [Tooltip("Add all Game Events to this list.")]
    private List<GameEvent> gameEvents;

    [SerializeField]
    [Header("Bool Events")]
    [Tooltip("Add all Bool Events to this list.")]
    private List<BoolGameEvent> boolEvents;

    [SerializeField]
    [Header("Int Events")]
    [Tooltip("Add all Int Events to this list.")]
    private List<IntGameEvent> intEvents;

    [SerializeField]
    [Header("String Events")]
    [Tooltip("Add all String Events to this list.")]
    private List<StringGameEvent> stringEvents;

    [SerializeField]
    [Header("Game Object Events")]
    [Tooltip("Add all Game Object Events to this list.")]
    private List<GameObjectGameEvent> gameObjectEvents;

    [SerializeField]
    [Header("Entity Controller Events")]
    [Tooltip("Add all Entity Controller Events to this list.")]
    private List<EntityControllerGameEvent> entityControllerEvents;

    [SerializeField]
    [Header("Sequence Events")]
    [Tooltip("Add all Sequence Events to this list.")]
    private List<SequenceGameEvent> sequenceEvents;

    //Singleton PURELY to alleviate EventManager references in other scripts.
    public static EventManager Instance;
    #endregion

    #region Event Maps - For String & Event Lookup
    private Dictionary<string, GameEvent> gameEventMap;
    private Dictionary<string, BoolGameEvent> boolEventMap;
    private Dictionary<string, IntGameEvent> intEventMap;
    private Dictionary<string, StringGameEvent> stringEventMap;
    private Dictionary<string, GameObjectGameEvent> gameObjectEventMap;
    private Dictionary<string, EntityControllerGameEvent> entityControllerEventMap;
    private Dictionary<string, SequenceGameEvent> sequenceEventMap;
    #endregion

    #region Raise Events
    /// <summary>
    /// Raises the passed in <see cref="GameEvent"/>, after confirming
    /// the name is valid.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    public void RaiseGameEvent(string eventName)
    {
        //Raise the game event if not null, else print an error.
        GameEvent gameEvent = this.GetGameEvent(eventName);
        if (!gameEvent)
        {
            Debug.LogError("ERROR: Invalid Game Event name: " + eventName + ". Double check Event Constants/your spelling.");
        }
        else
        {
            this.GetGameEvent(eventName).Raise();
        }
    }

    /// <summary>
    /// Raises the passed in <see cref="BoolGameEvent"/>, after confirming
    /// the name is valid.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="value">The bool value the event is passing in with.</param>
    public void RaiseBoolEvent(string eventName, bool value)
    {
        //Raise the bool event if not null, else print an error.
        BoolGameEvent boolEvent = this.GetBoolEvent(eventName);
        if (!boolEvent)
        {
            Debug.LogError("ERROR: Invalid Bool Event name: " + eventName + ". Double check Event Constants/your spelling.");
        }
        else
        {
            this.GetBoolEvent(eventName).Raise(value);
        }
    }

    /// <summary>
    /// Raises the passed in <see cref="IntGameEvent"/>, after confirming
    /// the name is valid.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="value">The int value the event is passing in with.</param>
    public void RaiseIntEvent(string eventName, int value)
    {
        //Raise the Int event if not null, else print an error.
        IntGameEvent intEvent = this.GetIntEvent(eventName);
        if (!intEvent)
        {
            Debug.LogError("ERROR: Invalid Int Event name: " + eventName + ". Double check Event Constants/your spelling.");
        }
        else
        {
            this.GetIntEvent(eventName).Raise(value);
        }
    }

    /// <summary>
    /// Raises the passed in <see cref="StringGameEvent"/>, after confirming
    /// the name is valid.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="value">The string value the event is passing in with.</param>
    public void RaiseStringEvent(string eventName, string value)
    {
        //Raise the Int event if not null, else print an error.
        StringGameEvent stringEvent = this.GetStringEvent(eventName);
        if (!stringEvent)
        {
            Debug.LogError("ERROR: Invalid String Event name: " + eventName + ". Double check Event Constants/your spelling.");
        }
        else
        {
            this.GetStringEvent(eventName).Raise(value);
        }
    }

    /// <summary>
    /// Raises the passed in <see cref="GameObjectGameEvent"/>, after confirming
    /// the name is valid.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="value">The string value the event is passing in with.</param>
    public void RaiseGameObjectEvent(string eventName, GameObject value)
    {
        //Raise the Int event if not null, else print an error.
        GameObjectGameEvent gameObjectEvent = this.GetGameObjectEvent(eventName);
        if (!gameObjectEvent)
        {
            Debug.LogError("ERROR: Invalid String Event name: " + eventName + ". Double check Event Constants/your spelling.");
        }
        else
        {
            this.GetGameObjectEvent(eventName).Raise(value);
        }
    }

    /// <summary>
    /// Raises the passed in <see cref="EntityControllerGameEvent"/>, after confirming
    /// the name is valid.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="value">The string value the event is passing in with.</param>
    public void RaiseEntityControllerEvent(string eventName, EntityController value)
    {
        //Raise the EC event if not null, else print an error.
        EntityControllerGameEvent entityControllerEvent = this.GetEntityControllerEvent(eventName);
        if (!entityControllerEvent)
        {
            Debug.LogError("ERROR: Invalid Entity Controller Event name: " + eventName + ". Double check Event Constants/your spelling.");
        }
        else
        {
            this.GetEntityControllerEvent(eventName).Raise(value);
        }
    }


    /// <summary>
    /// Raises the passed in <see cref="SequenceGameEvent"/>, after confirming
    /// the name is valid.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    public void RaiseSequenceGameEvent(string eventName, Hero.Core.Sequence value)
    {
        //Raise the game event if not null, else print an error.
        SequenceGameEvent sequenceGameEvent = this.GetSequenceGameEvent(eventName);
        if (!sequenceGameEvent)
        {
            Debug.LogError("ERROR: Invalid Sequence Game Event name: " + eventName + ". Double check Event Constants/your spelling.");
        }
        else
        {
            this.GetSequenceGameEvent(eventName).Raise(value);
        }
    }
    #endregion

    #region Event Accessors
    /// <summary>
    /// Gets a <see cref="GameEvent"/> from our <see cref="gameEventMap"/>.
    /// If no event by string name exists, returns null.
    /// </summary>
    /// <param name="eventName">The string name of the event.</param>
    /// <returns>The game event associated with the passed in string, if any.</returns>
    public GameEvent GetGameEvent(string eventName)
    {
        GameEvent gameEvent;

        //searches our dictionary for the event, outputs it, and returns it
        if (gameEventMap.TryGetValue(eventName.ToLower(), out gameEvent))
        {
            return gameEvent;
        }
        return null;
    }

    /// <summary>
    /// Gets a <see cref="BoolGameEvent"/> from our <see cref="boolEventMap"/>.
    /// If no event by string name exists, returns null.
    /// </summary>
    /// <param name="eventName">The string name of the event.</param>
    /// <returns>The bool event associated with the passed in string, if any.</returns>
    public BoolGameEvent GetBoolEvent(string eventName)
    {
        BoolGameEvent boolEvent;

        //searches our dictionary for the event, outputs it, and returns it
        if (boolEventMap.TryGetValue(eventName.ToLower(), out boolEvent))
        {
            return boolEvent;
        }
        return null;
    }

    /// <summary>
    /// Gets a <see cref="IntGameEvent"/> from our <see cref="intEventMap"/>.
    /// If no event by string name exists, returns null.
    /// </summary>
    /// <param name="eventName">The string name of the event.</param>
    /// <returns>The int event associated with the passed in string, if any.</returns>
    public IntGameEvent GetIntEvent(string eventName)
    {
        IntGameEvent intEvent;

        //searches our dictionary for the event, outputs it, and returns it
        if (intEventMap.TryGetValue(eventName.ToLower(), out intEvent))
        {
            return intEvent;
        }
        return null;
    }

    /// <summary>
    /// Gets a <see cref="StringGameEvent"/> from our <see cref="stringEventMap"/>.
    /// If no event by string name exists, returns null.
    /// </summary>
    /// <param name="eventName">The string name of the event.</param>
    /// <returns>The string event associated with the passed in string, if any.</returns>
    public StringGameEvent GetStringEvent(string eventName)
    {
        StringGameEvent stringEvent;

        //searches our dictionary for the event, outputs it, and returns it
        if (stringEventMap.TryGetValue(eventName.ToLower(), out stringEvent))
        {
            return stringEvent;
        }
        return null;
    }

    /// <summary>
    /// Gets a <see cref="GameObjectGameEvent"/> from our <see cref="gameObjectEventMap"/>.
    /// If no event by string name exists, returns null.
    /// </summary>
    /// <param name="eventName">The string name of the event.</param>
    /// <returns>The game object event associated with the passed in string, if any.</returns>
    public GameObjectGameEvent GetGameObjectEvent(string eventName)
    {
        GameObjectGameEvent gameObjectEvent;

        //searches our dictionary for the event, outputs it, and returns it
        if (gameObjectEventMap.TryGetValue(eventName.ToLower(), out gameObjectEvent))
        {
            return gameObjectEvent;
        }
        return null;
    }

    /// <summary>
    /// Gets a <see cref="EntityControllerGameEvent"/> from our <see cref="entityControllerEventMap"/>.
    /// If no event by string name exists, returns null.
    /// </summary>
    /// <param name="eventName">The string name of the event.</param>
    /// <returns>The entity controller event associated with the passed in string, if any.</returns>
    public EntityControllerGameEvent GetEntityControllerEvent(string eventName)
    {
        EntityControllerGameEvent entityControllerEvent;

        //searches our dictionary for the event, outputs it, and returns it
        if (entityControllerEventMap.TryGetValue(eventName.ToLower(), out entityControllerEvent))
        {
            return entityControllerEvent;
        }
        return null;
    }


    /// <summary>
    /// Gets a <see cref="SequenceGameEvent"/> from our <see cref="sequenceEventMap"/>.
    /// If no event by string name exists, returns null.
    /// </summary>
    /// <param name="eventName">The string name of the event.</param>
    /// <returns>The sequence game event associated with the passed in string, if any.</returns>
    public SequenceGameEvent GetSequenceGameEvent(string eventName)
    {
        SequenceGameEvent sequenceEvent;

        //searches our dictionary for the event, outputs it, and returns it
        if (sequenceEventMap.TryGetValue(eventName.ToLower(), out sequenceEvent))
        {
            return sequenceEvent;
        }
        return null;
    }
    #endregion

    #region Populate Event Map + Overloads
    /// <summary>
    /// Maps each game event's name to the game event itself, after 
    /// filling <see cref="gameEvents"/> via inspector.
    /// </summary>
    private void PopulateGameEventMap()
    {
        gameEventMap = new Dictionary<string, GameEvent>();
        foreach (GameEvent gameEvent in this.gameEvents)
        {
            if (!gameEvent)
            {
                Debug.LogError("ERROR: No Game Event specified! Check the EventManager object to see if it's populated correctly.");
                break;
            }
            gameEventMap.Add(gameEvent.name.ToLower(), gameEvent);
        }
    }

    /// <summary>
    /// Maps each bool event's name to the bool event itself, after 
    /// filling <see cref="boolEvents"/> via inspector.
    /// </summary>
    private void PopulateBoolEventMap()
    {
        boolEventMap = new Dictionary<string, BoolGameEvent>();
        foreach (BoolGameEvent boolEvent in this.boolEvents)
        {
            boolEventMap.Add(boolEvent.name.ToLower(), boolEvent);
        }
    }

    /// <summary>
    /// Maps each int event's name to the int event itself, after 
    /// filling <see cref="intEvents"/> via inspector.
    /// </summary>
    private void PopulateIntEventMap()
    {
        intEventMap = new Dictionary<string, IntGameEvent>();
        foreach (IntGameEvent intEvent in this.intEvents)
        {
            intEventMap.Add(intEvent.name.ToLower(), intEvent);
        }
    }

    /// <summary>
    /// Maps each string event's name to the int event itself, after 
    /// filling <see cref="stringEvents"/> via inspector.
    /// </summary>
    private void PopulateStringEventMap()
    {
        stringEventMap = new Dictionary<string, StringGameEvent>();
        foreach (StringGameEvent stringEvent in this.stringEvents)
        {
            stringEventMap.Add(stringEvent.name.ToLower(), stringEvent);
        }
    }

    /// <summary>
    /// Maps each game object event's name to the int event itself, after 
    /// filling <see cref="gameObjectEvents"/> via inspector.
    /// </summary>
    private void PopulateGameObjectEventMap()
    {
        gameObjectEventMap = new Dictionary<string, GameObjectGameEvent>();
        foreach (GameObjectGameEvent gameObjectEvent in this.gameObjectEvents)
        {
            gameObjectEventMap.Add(gameObjectEvent.name.ToLower(), gameObjectEvent);
        }
    }

    /// <summary>
    /// Maps each game object event's name to the int event itself, after 
    /// filling <see cref="entityControllerEvents"/> via inspector.
    /// </summary>
    private void PopulateEntityControllerEventMap()
    {
        entityControllerEventMap = new Dictionary<string, EntityControllerGameEvent>();
        foreach (EntityControllerGameEvent entityControllerEvent in this.entityControllerEvents)
        {
            entityControllerEventMap.Add(entityControllerEvent.name.ToLower(), entityControllerEvent);
        }
    }


    /// <summary>
    /// Maps each sequence event's name to the game event itself, after 
    /// filling <see cref="sequenceEvents"/> via inspector.
    /// </summary>
    private void PopulateSequenceEventMap()
    {
        sequenceEventMap = new Dictionary<string, SequenceGameEvent>();
        foreach (SequenceGameEvent sequenceEvent in this.sequenceEvents)
        {
            sequenceEventMap.Add(sequenceEvent.name.ToLower(), sequenceEvent);
        }
    }
    #endregion

    #region On Enable
    /// <summary>
    /// Populate all our event maps upon startup.
    /// </summary>
    private void OnEnable()
    {
        this.PopulateGameEventMap();
        this.PopulateBoolEventMap();
        this.PopulateIntEventMap();
        this.PopulateStringEventMap();
        this.PopulateGameObjectEventMap();
        this.PopulateEntityControllerEventMap();
        this.PopulateSequenceEventMap();

        if (!Instance)
        {
            Instance = this;
        }
    }
    #endregion

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void E()
    {
        Resources.Load<EventManager>("Event Manager");
    }
}