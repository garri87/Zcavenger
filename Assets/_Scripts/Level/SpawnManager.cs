using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Enemy[] enemyType;
    public GameObject[] enemyPrefabs;
    public float spawnTime = 2f;
    public float spawnRate = 1f;
    [SerializeField] 
    private int enemyCount;

    public bool randomEnemy;
    public int order = 0;

    private AgentController agentController;


    void Start()
    {
        
    }

  
    void Update()
    {
        enemyCount = gameObject.transform.childCount;

        if (Input.GetKeyDown(KeyCode.Q)) 
        {
           SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        if (randomEnemy)
        {
            order = Random.Range(0, enemyType.Length);
        }

        int selectedPrefab = Random.Range(0,enemyPrefabs.Length);
        GameObject instantiatedEnemy = Instantiate(enemyPrefabs[selectedPrefab], transform.position, Quaternion.Euler(0,-90,0));
        instantiatedEnemy.transform.parent = gameObject.transform;
        agentController = instantiatedEnemy.GetComponent<AgentController>();
        agentController.enemyScriptableObject = enemyType[order];
    }
}
