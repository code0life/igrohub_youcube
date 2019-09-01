using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING, STOP };

    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    public GameObject target;
    public int nextWave = 0;

    public Transform[] spawnPoints;
    GameObject[] allEnemy;

    public float timeBetweenWaves = 5f;
    private float waveCountdown;

    private float searchCountdown = 1f;

    public SpawnState state = SpawnState.WAITING;

    void Start()
    {

        if (spawnPoints.Length == 0)
        {
            Debug.Log( "No spawn points. Set it" );
        }

        waveCountdown = timeBetweenWaves;
    }

    void Update()
    {
        if (state == SpawnState.STOP) 
        {
            return;
        }

        if (state == SpawnState.WAITING)
        {
            if (!IsAlive())
            {
                Interface.instance.DoneSpawnInfo(waves[nextWave]);
                WaveCompleted();  
            }
            else
            {

                allEnemy = GameObject.FindGameObjectsWithTag("enemy");
                Interface.instance.SpawnInfo(allEnemy.Length);
                return;
            }
        }

        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave( waves[nextWave] ));
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
    }

    void WaveCompleted()
    {
        //Debug.Log("WaveCompleted" );
        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        if (nextWave+1 > waves.Length-1)
        {
            nextWave = 0;
            Debug.Log("CompliteAllWaves");
        }
        else
        {
            nextWave++;
        }

           
    }

    bool IsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("enemy") == null)
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        Debug.Log("SpawnWave "+_wave.name);
        state = SpawnState.SPAWNING;

        for (int i = 0; i < _wave.count; i++)
        {
            Spawn(_wave.enemy);
            yield return new WaitForSeconds(1f/_wave.rate);
        }

        state = SpawnState.WAITING;
        yield break;
    }

    public void Stop()
    {

        state = SpawnState.STOP;

    }

    void Spawn(Transform _enemy)
    {

        Transform _spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Transform cube =  Instantiate(_enemy, _spawnPoint.position, _spawnPoint.rotation);
        cube.GetComponent<AI>().target = target;

        AI.EnumBehavior typeCube = (AI.EnumBehavior)Random.Range(0, 3);
        cube.GetComponent<AI>().behavior = typeCube;

        if (typeCube == AI.EnumBehavior.ESCAPE)
        {
            cube.GetComponent<CubeContent>().color = Color.red;
        }
        else if (typeCube == AI.EnumBehavior.NEUTRAL)
        {
            cube.GetComponent<CubeContent>().color = Color.green;
        }
        else if (typeCube == AI.EnumBehavior.CHASE)
        {
            cube.GetComponent<CubeContent>().color = Color.black;
            cube.GetComponent<AI>().attention_radius = 50;
        }
        cube.GetComponent<CubeContent>().rows = Random.Range(1, 3);

        Debug.Log("Spawn - " + _enemy.name);
    }

}

