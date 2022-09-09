using System.Collections;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public enum spawnEnemyType { Slime, Turtle, Dragon};
    public spawnEnemyType enemyType;

    public GameObject slimeEnemy = null;
    public GameObject turtleEnemy = null;
    public GameObject dragonEnemy = null;

    private GameObject instSlime = null;
    private GameObject instTurtle = null;
    private GameObject instDragon = null;

    //private bool spawn = false;

    public float waveInterval  = 3.0f;
    public int   spawnCount    = 3;
    private float spawnRate    = 1f;
    private float respawn = 0.0f;
    private float respawnDelay = 10.0f;

    WaitForSeconds waitSpawnIntervalFirst = null;

    private void Awake()
    {
        StartCoroutine(SlimeSpawn());
        StartCoroutine(TurtleSpawn());
        StartCoroutine(DragonSpawn());
    }
    private void Start()
    {
        waitSpawnIntervalFirst = new WaitForSeconds(spawnRate);
    }

    private void Update()
    {
        ReSpawn();
    }

    //몬스터 사망시 리스폰 함수
    private void ReSpawn()
    {
        switch (enemyType)
        {
            case spawnEnemyType.Slime:
                if (instSlime == null)
                {
                    respawn += Time.deltaTime;
                    if (respawn > respawnDelay)
                    {
                        respawn = 0.0f;
                        StartCoroutine(SlimeSpawn());
                    }
                }
                break;
            case spawnEnemyType.Turtle:
                if (instTurtle == null)
                {
                    respawn += Time.deltaTime;
                    if (respawn > respawnDelay)
                    {
                        respawn = 0.0f;
                        StartCoroutine(TurtleSpawn());
                    }
                }
                break;
            case spawnEnemyType.Dragon:
                if (instDragon == null)
                {
                    respawn += Time.deltaTime;
                    if (respawn > respawnDelay)
                    {
                        respawn = 0.0f;
                        StartCoroutine(DragonSpawn());
                    }
                }
                break;
        }
    }
    //슬라임 몬스터 스폰 함수 
    IEnumerator SlimeSpawn()
    {

        //yield return waitSpawnInterval;
        for (int j = 0; j < waveInterval; j++)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                switch (enemyType) {
                    case spawnEnemyType.Slime:
                        instSlime = Instantiate(slimeEnemy);
                        instSlime.transform.position = this.transform.position;
                        instSlime.transform.rotation = this.transform.rotation;
                        instSlime.GetComponent<Enemy>().Init(this);
                        break;
                }
                yield return waitSpawnIntervalFirst;
            }
        }
    }
    //거북이 몬스터 스폰 함수 
    IEnumerator TurtleSpawn()
    {

        //yield return waitSpawnInterval;
        for (int j = 0; j < waveInterval; j++)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                switch (enemyType)
                {
                    case spawnEnemyType.Turtle:
                        instTurtle = Instantiate(turtleEnemy);
                        instTurtle.transform.position = transform.position;
                        instTurtle.transform.rotation = transform.rotation;
                        break;
                }
                yield return waitSpawnIntervalFirst;
            }
        }
    }
    //드래곤 스폰 함수 
    IEnumerator DragonSpawn()
    {

        //yield return waitSpawnInterval;
        for (int j = 0; j < waveInterval; j++)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                switch (enemyType)
                {
                    case spawnEnemyType.Dragon:
                        instDragon = Instantiate(dragonEnemy);
                        instDragon.transform.position = transform.position;
                        instDragon.transform.rotation = transform.rotation;
                        break;
                }
                yield return waitSpawnIntervalFirst;
            }
        }
    }
}
