using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class N_GameManager : NetworkBehaviour {

    public int roidNumber;
    public int maxRoidSize;

    [Range(0, 100)]
    public int[] rockChance;
    public GameObject[] roidFieldRocks;
    bool isSpawningLevel;

    public Vector3 spawnValues;

    
    private GameObject[] tempPrefabs;
    private SpawnManager SM;

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (isServer && SceneManager.GetActiveScene().buildIndex == 1)
        {
            StartCoroutine(ServerSetupScene());
        }
    }
    [ServerCallback]
    IEnumerator ServerSetupScene()
    {
        SM = GameObject.FindObjectOfType<SpawnManager>();
        while (SM == null) {
            SM = GameObject.FindObjectOfType<SpawnManager>();
            yield return new WaitForFixedUpdate();
        }
        while (!SM.registered)
            yield return new WaitForEndOfFrame();
        CreateRoidField(roidNumber, maxRoidSize);
        yield return null;
    }
    // #######################################################################
    // this the proceedural generation ;  it requires two integer
    // input for the number of roid and the max size of them.
    void CreateRoidField(int roidTotal, int maxSize)
    {
        // initializes the array for storage of objects
        tempPrefabs = new GameObject[roidNumber];
        // ensure that spawn rates are out of 100%
        rockChance = CheckRockChance(rockChance);
        // bool to keep player safe during spawning
        isSpawningLevel = true;
        // create one random number
        System.Random randomPer = new System.Random(Time.time.ToString().GetHashCode());
        GameObject tempTransformHolder = new GameObject();
        // loop for the number of asteroids to be made
        for (int i = 0; i < roidNumber; i++)
        {
            GameObject roid = null;
            int chanceForThisLoop = randomPer.Next(1, 100);
            for (int j = 0; j < rockChance.Length; j++)
            {
                if (chanceForThisLoop < rockChance[j])
                {
                    roid = roidFieldRocks[j];
                    break;
                }
            }
            if (roid != null) {
                StartCoroutine(CreateTempObject());
                // randomize the max size
                //float roidSize = Random.Range(0.5f, maxRoidSize);
                // randomize the location of the new spawnpoint, set from spawnvalues
                //Vector3 spawnLocation = new Vector3(Random.Range(-spawnValues.x, spawnValues.x),
                    //            Random.Range(-spawnValues.y, spawnValues.y),
                    //            Random.Range(-spawnValues.z, spawnValues.z));
                // put the temporary shape into the world
                //GameObject tempRoid = Instantiate(roid, spawnLocation, Quaternion.identity) as GameObject;
                // implement the new size
                //tempRoid.GetComponent<N_SpawningCheck>().sendSize = roidSize;
                // give it a numbered name
                //tempRoid.name = "tempRoid" + i.ToString();
                // test roid position done on the temp roid game object
                //tempPrefabs[i] = tempRoid;
                //NetworkServer.Spawn(tempRoid);
                
            }
        }
    }
    int[] CheckRockChance(int[] rockChanceIn)
    {
        int newTotal = 0;
        foreach (int i in rockChanceIn)
        {
            newTotal += i;
        }
        if (newTotal <= 100)
        {
            for (int i = 0; i < rockChance.Length; i++)
            {
                if (i != 0)
                    rockChanceIn[i] += rockChance[i - 1];
            }
            return rockChanceIn;
        }
        else
        {
            int[] newChanceOut = new int[rockChanceIn.Length];
            for (int i = 0; i < rockChanceIn.Length; i++)
            {
                newChanceOut[i] = Mathf.RoundToInt(rockChanceIn[i] / newTotal);
                if (i != 0)
                    newChanceOut[i] += newChanceOut[i - 1];
            }
            return newChanceOut;
        }
    }
    IEnumerator CreateTempObject()
    {
        GameObject tempTransformHolder = new GameObject();
        float roidSize = Random.Range(0.5f, maxRoidSize);
        // randomize the location of the new spawnpoint, set from spawnvalues
        Vector3 spawnLocation = new Vector3(Random.Range(-spawnValues.x, spawnValues.x),
                    Random.Range(-spawnValues.y, spawnValues.y),
                    Random.Range(-spawnValues.z, spawnValues.z));
        tempTransformHolder.transform.position = spawnLocation;
        tempTransformHolder.transform.rotation = Random.rotation;
        tempTransformHolder.transform.localScale = new Vector3(roidSize, roidSize, roidSize);
        SpawnCheck SC = tempTransformHolder.AddComponent<SpawnCheck>();
        SC.AddSpawnComponents(SM, tempTransformHolder.transform);
        yield return null;
    }
    public void FinalizeTempObjects(GameObject[] listToCreate)
    {
        if (!isClient)
            return;
        foreach(GameObject temproid in listToCreate)
        {
            temproid.SetActive(true);
            temproid.GetComponent<N_RoidHit>().RpcMatchServerSizes();
        }
        // gives the player their vulnerablity back
        //isSpawningLevel = false;
    }
}
