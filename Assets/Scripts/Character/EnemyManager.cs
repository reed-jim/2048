using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : ScriptableObject
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Enemy[] enemyControllers;

    private void Awake()
    {
        enemyControllers = new Enemy[9];

        for (int i = 0; i < enemyControllers.Length; i++)
        {
            enemyControllers[i] = Instantiate(enemyPrefab).GetComponent<Enemy>();
            enemyControllers[i].Id = $"enemy{i}";
        }
    }

    IEnumerator SpawnEnemy()
    {
        for (int i = 0; i < enemyControllers.Length; i++)
        {
            if (enemyControllers[i].EnemyState == EnemyState.Inactive)
            {
    
            }
        }

        yield return new WaitForSeconds(5f);
    }
    
    IEnumerator SetEnemyDestination()
    {
        for (int i = 0; i < enemyControllers.Length; i++)
        {
            if (enemyControllers[i].EnemyState == EnemyState.Inactive)
            {
    
            }
        }

        yield return new WaitForSeconds(5f);
    }
}