using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    
    public Enemy[] enemyScriptables;
    public List<GameObject> enemyPrefabs;
    public bool autoSpawn = false;
    public float spawnCooldown = 30f;
    
    [SerializeField]
    private float timer;
    
    [SerializeField] 
    private int enemyCount;

    public bool randomEnemy;
    public int randomOrder = 0;

    public Floor.BuildingType buildingType;

    private AgentController agentController;

    
    void Start()
    {
        timer = spawnCooldown;
        
    }

  
    void Update()
    {
        enemyCount = transform.childCount;

        if (Input.GetKeyDown(KeyCode.Q)) 
        {
           SpawnEnemy(random: randomEnemy);
        }

        if (autoSpawn)
        {
            if (enemyCount <= 0)
            {
                timer -= Time.deltaTime;
            }

            if (enemyCount == 0 && timer < 0)
            {
                SpawnEnemy();

                timer = spawnCooldown;
            }
        }
    }

    public void Init(Floor.BuildingType buildingType)
    {
        this.buildingType = buildingType;
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            AgentController agentController = enemyPrefabs[i].GetComponent<AgentController>();
            if (!agentController.buildingTypes.Contains(buildingType))
            {
                enemyPrefabs.RemoveAt(i);
            }
        }
        autoSpawn = true;
    }

    public void SpawnEnemy(Enemy scriptableObject = null, bool random = true, int count = 1)
    {
        if (random)
        {
            randomOrder = Random.Range(0, enemyScriptables.Length);
        }

        int randomPrefab = Random.Range(0,enemyPrefabs.Count);

        for (int i = 0; i < count; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[randomPrefab];
            agentController = enemyPrefab.GetComponent<AgentController>();
            if (scriptableObject)
            {
                agentController.enemyScriptableObject = scriptableObject;

            }
            else
            {
                agentController.enemyScriptableObject = enemyScriptables[randomOrder];

            }

            Instantiate(enemyPrefab,transform.position, Quaternion.Euler(0, -90, 0),
                    transform);
                Debug.Log("Enemy " + enemyPrefab.name + " Spawned!");
            
        }
        
    }
}
