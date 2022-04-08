using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New_Enemy", menuName = "Enemy/New Enemy")]
public class Enemy : ScriptableObject
{
    public string enemyName;
    
    public GameObject enemyPrefab;
    public List<GameObject> itemDrop;
    
    public enum EnemyType
    {
        Humanoid,
        Walker,
        Runner,
        Crawler,
        Crippled,
        Brute,
    }
    public EnemyType enemyType;
    
    public int maxHealth;
    public int minDamage;
    public int maxDamage;
    public float minSpeed;
    public float speed;
    public float attackDistance;
    public int bleedDamageProbability;

}
