using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;


/// <summary>
/// Manages and contains all variables utilized throughout the entire game.
/// 
/// To add a new variable, create the variable in its respective folder,
/// add it to this scriptable objects' respective list via inspector,
/// then add its name as a contstant string in <see cref="VariableConstants"/>.
/// </summary>

[CreateAssetMenu(fileName = "Variable Manager", menuName = "System/VariableManager")]
public class VariableManager : ScriptableObject
{
    #region Variable Fields
    [SerializeField]
    [Header("Booleans")]
    [Tooltip("Add all bools to this list.")]
    private List<BoolVariable> bools;

    public static VariableManager Instance;
    #endregion

    #region Variable Maps - For String & Var Lookup
    private Dictionary<string, BoolVariable> boolMap;
    #endregion

    #region Get Variable Values
    /// <summary>
    /// Gets a <see cref="BoolVariable"/> from our <see cref="boolMap"/>.
    /// If no variable by string name exists, returns null.
    /// </summary>
    /// <param name="varName">The string name of the variable.</param>
    /// <returns>The value of the variable associated with the passed in string, if any.</returns>
    public bool GetBoolVariableValue(string varName)
    {
        BoolVariable value;

        //searches our dictionary for the variable, outputs it, and returns it
        if (boolMap.TryGetValue(varName.ToLower(), out value))
        {
            return value.Value;
        }

        Debug.LogError("ERROR: No variable of name " + varName + " exists. Returning false.");
        return false;
    }
    #endregion

    #region Set Variable Values
    /// <summary>
    /// Sets the value of a <see cref="BoolVariable"/> in our <see cref="boolMap"/>.
    /// </summary>
    /// <param name="varName">The string name of the variable.</param>
    /// <param name="val">The value to set.</param>
    public void SetBoolVariableValue(string varName, bool val)
    {
        BoolVariable value;

        //searches our dictionary for the variable, outputs it, and returns it
        if (boolMap.TryGetValue(varName.ToLower(), out value))
        {
            value.Value = val;
            return;
        }

        Debug.LogError("ERROR: No variable of name " + varName + " exists.");
    }
    #endregion

    #region Populate Variable Maps
    /// <summary>
    /// Maps each bool variables's name to the bool itself, after 
    /// filling <see cref="boolMap"/> via inspector.
    /// </summary>
    private void PopulateBoolMap()
    {
        boolMap = new Dictionary<string, BoolVariable>();
        foreach (BoolVariable boolVar in this.bools)
        {
            boolMap.Add(boolVar.name.ToLower(), boolVar);
        }
    }
    #endregion

    #region On Enable
    /// <summary>
    /// Populate all our maps upon startup.
    /// </summary>
    private void OnEnable()
    {
        this.PopulateBoolMap();

        if (!Instance)
        {
            Instance = this;
        }
    }
    #endregion

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void E()
    {
        Resources.Load<VariableManager>("Variable Manager");
    }
}