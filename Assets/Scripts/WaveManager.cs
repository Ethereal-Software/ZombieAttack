using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public List<BoxCollider> spawnZones;
    public GameObject walkingZombie;
    public GameObject runningZombie;

    public GameObject ZombiesContainer;
    public PlayerShoot playerShoot;

    public EnemyAgentType enemyAgentType;

    public bool spawnTest = false;
    public bool clear = false;
    List<GameObject> zombies;
    public int aliveCount = 0;
    public AudioSource waveCompleteAudio;

    public Text infoText;

    public float[] spawnZoneWeighting;

    public float textFadeTime = 5f;

    bool fadeText = false;
    int currentWave = 0;

    int waveRunCt = 0;
    int waveWalkCt = 0;

    public int startAtWave = 1;

    public List<GameObject> guns;

    public void CalculateZoneWeighting() {
        float total = 0;
        spawnZoneWeighting = new float[spawnZones.Count];
        for(var s=0; s<spawnZones.Count; s++) {
            BoxCollider sz = spawnZones[s];
            Bounds b = sz.bounds;
            Vector3 min = b.min;
            Vector3 max = b.max;
            var area = Mathf.Abs((max.x - min.x) * (max.z - min.z));
            spawnZoneWeighting[s] = area;
            total += area;
        }

        for (var s = 0; s < spawnZones.Count; s++)
        {
            spawnZoneWeighting[s] = spawnZoneWeighting[s] / total;
        }
    }

    public void NextWave() {
        currentWave++;

        ClearZombies();
        //UnparentZombies();

        SetInfoText("Wave " + currentWave);

        if (currentWave < 5) {
            waveWalkCt += 5;
        }
        else if (currentWave >= 15)
        {
            waveWalkCt = Mathf.Max(waveWalkCt - 3, 5);
            waveRunCt += 5;
            ;// waveWalkCt -= 5;
        }
        else if (currentWave >= 10)
        {
            waveRunCt += 5;
            waveWalkCt = Mathf.Max(waveWalkCt - 5, 5);
            //waveWalkCt -= 5;
        }
        else if (currentWave >= 5)
        {
            waveRunCt += 3;
            waveWalkCt += 2;
        }
        //else//>15
        //{
        //    waveRunCt += 5;
        //}

        if (currentWave == 5) {
            //give shotgun
            playerShoot.guns[1] = guns[1];
            playerShoot.SetGun(1);
        } else if (currentWave == 10) {
            //give M4
            playerShoot.guns[2] = guns[2];
            playerShoot.SetGun(2);
        } else if (currentWave == 15) {
            //give LMG
            playerShoot.guns[3] = guns[3];
            playerShoot.SetGun(3);
        }

        //Debug.Log("walkers: " + waveWalkCt +", wave: " + currentWave);
        SpawnWave(waveWalkCt, waveRunCt);
    }

    public void SpawnWave(int walkingCount, int RunningCount) {
        zombies = new List<GameObject>();

        if (spawnZoneWeighting == null) CalculateZoneWeighting();
        
        for (var s = 0; s < spawnZones.Count; s++) {
            int runCt = (int)(spawnZoneWeighting[s] * RunningCount);
            int walkCt = (int)(spawnZoneWeighting[s] * walkingCount);

            for (var i = 0; i < runCt; i++) {
                var zmTp = SpawnZombie(spawnZones[s], runningZombie);
                zombies.Add(zmTp);
            }
            for (var i = 0; i < walkCt; i++)
            {
                var zmTp = SpawnZombie(spawnZones[s], walkingZombie);
                zombies.Add(zmTp);
            }
        }

        playerShoot.UpdateZombieColliders();
    }

    public GameObject SpawnZombie(Collider zone, GameObject proto) {
        //if (spawnZones.Count == 0) return null;

        //test
        Bounds b = zone.bounds;
        Vector3 min = b.min;
        Vector3 max = b.max;

        min.y = 0;
        max.y = 0;

        var randomPos = new Vector3(Random.Range(min.x, max.x), 0, Random.Range(min.z, max.z));

        var zombieTp = Instantiate(proto, ZombiesContainer.transform);
        zombieTp.transform.position = randomPos;

        var colTp = zombieTp.GetComponent<ZombieColliders>();
        var zomTp = zombieTp.GetComponent<ZombieMovement>();
        zomTp.enemyAgentType = enemyAgentType;

        colTp.onDied = () =>
        {
            CheckState();
        };

        aliveCount++;
        return zombieTp;
    }

    public void CheckState() {
        int remaining = CountRemaining();
        aliveCount--;
        Debug.Log((remaining) + " zombies remaining");

        if (remaining == 5)
        {
            SetInfoText("5 zombies remaining");
        }
        else if (remaining == 10)
        {
            SetInfoText("10 zombies remaining");
        }
        else if (remaining == 20)
        {
            SetInfoText("20 zombies remaining");
        }


        //zombies.Remove(zombieTp);
        if (remaining == 0)
        {
            waveCompleteAudio.Play();
            NextWave();
        }
    }

    public void ClearZombies() {
        if (zombies != null)
        {
            foreach (GameObject go in zombies)
            {
                DestroyImmediate(go);
            }
        }
        zombies = new List<GameObject>();

        foreach (Transform t in ZombiesContainer.transform)
        {
            DestroyImmediate(t.gameObject);
        }
        aliveCount = 0;
    }

    public void UnparentZombies()//pilling bodies
    {
        if (zombies != null)
        {
            foreach (GameObject go in zombies)
            {
                go.transform.parent = null;//may
            }
        }
        zombies = new List<GameObject>();
    }

    public int CountRemaining() {
        int aliveCt = 0;
        foreach (GameObject g in zombies) {
            var zm = g.GetComponent<ZombieColliders>();
            if (zm.isAlive) aliveCt++;
        }
        return aliveCt;
    }

    public void SetInfoText(string tx) {
        infoText.text = tx;
        infoText.color = new Color(1, 1, 1, 1);
        fadeText = true;
        
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnTest)
        {
            spawnTest = false;

            SpawnWave(10, 5);
            
        }
        if (clear)
        {
            clear = false;
            ClearZombies();
        }
    }
    public void SetWave(int waveNumber) {
        for (var i = 0; i < waveNumber; i++) {
            NextWave();
        }
    }

    public void SpawnTest() {
        ClearZombies();
        zombies = new List<GameObject>();
        CalculateZoneWeighting();

        SetWave(startAtWave);


    }
    // Start is called before the first frame update
    void Start()
    {
        SpawnTest();
    }

    // Update is called once per frame
    float nextUpdateTime = 0f;
    void Update()
    {
        if (fadeText) {
            var col = infoText.color;
            col.a -= Time.deltaTime / textFadeTime;
            infoText.color = col;
            if (col.a <= 0) fadeText = false;
        }

        if (Time.time > nextUpdateTime) {
            CheckState();
            nextUpdateTime = Time.time + 5f;
        }
    }
}
