using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    
    public Enemy[] enemyType;
    public GameObject[] enemyPrefabs;
    public float spawnTime = 2f;
    public float spawnRate = 1f;
    public float spawnCooldown = 30f;
    private float timer;
    
    [SerializeField] 
    private int enemyCount;

    public bool randomEnemy;
    public int order = 0;

    private AgentController agentController;

    
    void Start()
    {
        timer = spawnCooldown;
        
    }

  
    void Update()
    {
        enemyCount = gameObject.transform.childCount;

        if (Input.GetKeyDown(KeyCode.Q)) 
        {
           SpawnEnemy(random: randomEnemy);
        }

        if (enemyCount <= 0)
        {
            timer -= Time.deltaTime;
        }

        if (enemyCount <=0 && spawnCooldown <= 0)
        {
            SpawnEnemy();
            
            timer = spawnCooldown;
        }
        
    }

    public void SpawnEnemy(Enemy type = null, bool random = true, int count = 1)
    {
        if (random)
        {
            order = Random.Range(0, enemyType.Length);
        }

        int selectedPrefab = Random.Range(0,enemyPrefabs.Length);

        for (int i = 0; i < count; i++)
        {
            GameObject instantiatedEnemy = Instantiate(enemyPrefabs[selectedPrefab],
                transform.position, Quaternion.Euler(0,-90,0),transform);
        
            agentController = instantiatedEnemy.GetComponent<AgentController>();
            agentController.enemyScriptableObject = enemyType[order]; 
        }
        
    }
}
