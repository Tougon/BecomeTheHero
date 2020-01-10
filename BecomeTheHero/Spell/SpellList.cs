using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpellList", menuName = "Spell/Spell List", order = 9)]
public class SpellList : ScriptableObject
{
    public List<Spell> spells = new List<Spell>();
}
