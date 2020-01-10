using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents any kind of creature in the game.
/// </summary>
[CreateAssetMenu(fileName = "NewEntity", menuName = "Entity/Entity", order = 0)]
public class Entity : ScriptableObject
{
    public EntityParams vals;
    // Weakness, Resistance, and Immunity go here.

    public List<Spell> moveList;
}

[System.Serializable]
public class EntityParams
{
    public string entityName;
    public string article;

    public bool useArticle;

    public int entityHP;
    public int entityMP;
    public int entityAtk;
    public int entityDef;
    public int entitySpeed;
}
