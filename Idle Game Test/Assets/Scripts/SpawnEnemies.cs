using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    List<GameObject> enemies;
    int spawnCount;
    GameObject enemy;

    public Transform enemyTrans;

    private void Start()
    {
        spawnCount = 30;
        GameObject groundGo = GameObject.FindGameObjectWithTag("Ground");

        for (int i = 0; i < spawnCount; i++)
        {
            enemy = Instantiate(Resources.Load("Enemy-1"), enemyTrans) as GameObject;
            enemy.transform.position = new Vector3(Random.Range(-210, -190), Random.Range(-20, 20), -5);
        }
    }

    private void Update()
    {
        enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Monster"));
        if (enemies.Count < spawnCount)
        {
            enemy = Instantiate(Resources.Load("Enemy-1"), enemyTrans) as GameObject;
            enemy.transform.position = new Vector3(Random.Range(-210, -190), Random.Range(-20, 20), -5);
        }
    }
}
