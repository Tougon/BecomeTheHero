using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Manager", menuName = "System/EnemyManager")]
public class EnemyManager : ScriptableObject
{
    public static EnemyManager Instance;

    [SerializeField]
    private List<EnemyOdds> enemyOdds;

    private List<Entity> enemies;


    /// <summary>
    /// Returns a random enemy. Can give a duplicate enemy.
    /// </summary>
    public Entity GetRandomEnemy()
    {
        int index = Random.Range(0, enemies.Count);

        return enemies[index];
    }


    #region On Enable
    /// <summary>
    /// Populate the enemy list
    /// </summary>
    private void OnEnable()
    {
        enemies = new List<Entity>();

        foreach(EnemyOdds eo in enemyOdds)
        {
            for(int i=0; i<eo.relativeOdds; i++)
            {
                enemies.Add(eo.enemy);
            }
        }

        if (!Instance)
        {
            Instance = this;
        }
    }
    #endregion

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void E()
    {
        Resources.Load<EventManager>("Enemy Manager");
    }
}

[System.Serializable]
public class EnemyOdds
{
    public Entity enemy;
    [Range(1, 100)]
    public int relativeOdds = 1;
}
