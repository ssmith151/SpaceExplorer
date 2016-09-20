using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class N_RoidHit : NetworkBehaviour {


    [SyncVar(hook = "OnHitsChange")]
    public int hits;
    public NetworkTransSync syncTrans;
    public GameObject roidExplosion;
    public GameObject newRoid;
    public float maxtumble;
    public bool transforming;

    private Rigidbody rb;
    //private GameContoller mainScore;

    public NetworkHash128 assetID;
    [SyncVar]
    private bool beingDestroyed;
    private SpawnManager spawnManager;

    void Start()
    {
        beingDestroyed = false;
        rb = gameObject.GetComponent<Rigidbody>();
        SlowStart();
        //mainScore = GameObject.Find("GameController").GetComponent<GameContoller>();
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
       // if (syncTrans==null)
       //     syncTrans = new NetworkTransSync(   gameObject.transform.position,
       //                                        gameObject.transform.rotation,
       //                                         gameObject.transform.localScale);
        transforming = false;
    }
    [ClientRpc]
    public void RpcMatchServerSizes()
    {
        if (!isClient)
        {
            return;
        }
        gameObject.SetActive(true);
        Debug.Log("Show client match, hits : " + hits);
        if (rb == null)
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }
        gameObject.transform.localScale = syncTrans.scl;
        gameObject.transform.rotation = syncTrans.rot;
        //hits = Mathf.RoundToInt(syncTrans.scl.x);
        rb.angularVelocity = Random.insideUnitSphere / (hits / maxtumble);
        rb.mass = 10.0f * hits;
    }
    public void ApplyTransform(Transform transIn)
    {
        if (syncTrans == null)
        {
            syncTrans = new NetworkTransSync(Vector3.zero, Quaternion.identity, Vector3.zero);
        }
        if (rb == null)
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }
        syncTrans.pos = transIn.position;
        syncTrans.rot = transIn.rotation;
        syncTrans.scl = transIn.localScale;
        gameObject.transform.localScale = syncTrans.pos;
        gameObject.transform.rotation = syncTrans.rot;
        if (!isServer)
        {
            hits = Mathf.RoundToInt(syncTrans.scl.x);
            Debug.Log(hits + "  Hits");
        }
        rb.angularVelocity = Random.insideUnitSphere / (hits / maxtumble);
        rb.mass = 10.0f * hits;
        transforming = false;
    }
    void SlowStart()
    {
        rb.drag = 40f;
        StartCoroutine(normalizeDrag());
    }
    IEnumerator normalizeDrag()
    {
        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForSeconds(0.1f);
            rb.drag -= rb.drag / 2f;
        }
        rb.drag = 0;
        yield return null;
        //hits = Mathf.RoundToInt(gameObject.transform.localScale.x);
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Temp") || other.gameObject.CompareTag("Roid"))
        {
            return;
        }
        if (hits >= 1)
        {
            TakeHit(1);
        }
    }
    IEnumerator fillVolumeOfRoid()
    {
        if (!isServer)
            yield return null;
        // this coroutine is an attempt to fill the volume of the space contained by the asteroid
        // there is a helper class 'SphereVolume' to calculate the volume from radius and visa versa
        int sizeOfA = Mathf.RoundToInt(gameObject.transform.localScale.x);
        if (sizeOfA > 3)
        {
            CreateNewRoidsFromSize(sizeOfA);
        }
        yield return null;
    }
    void CreateNewRoidsFromSize(int sizeOfA)
    {
        Vector3 sprV;
        Vector3 roidV = gameObject.transform.position;
        int volOfA = SphereVolume.fromRadius(sizeOfA);
        int sizeOfNewA = 0;
        int volOfNewA = 0;
        //mainScore.AddScore(sizeOfA);
        for (int i = 0; i < sizeOfA; i++)
        {
            if (spawnManager.poolEmpty())
            {
                return;
            }
            if (sizeOfA > 1)
                sizeOfNewA = Mathf.RoundToInt(Random.Range(1, sizeOfA - 1));
            else
                return;
            volOfNewA = SphereVolume.fromRadius(sizeOfNewA);
            sprV = Random.insideUnitSphere;
            int radiusFromCenter = sizeOfA - sizeOfNewA;
            Vector3 roidSpawn = new Vector3(sprV.x * radiusFromCenter + roidV.x,
                                            sprV.y * radiusFromCenter + roidV.y,
                                            sprV.z * radiusFromCenter + roidV.z);
            StartCoroutine(CreateTempObject(roidSpawn,sizeOfNewA));

            //GameObject smallRoid = Instantiate(newRoid, roidSpawn, gameObject.transform.rotation) as GameObject;
            //smallRoid.transform.localScale = new Vector3(sizeOfNewA, sizeOfNewA, sizeOfNewA);
            //smallRoid.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //volOfA = volOfA - volOfNewA;
            //sizeOfA = SphereVolume.radiusFromVolume(volOfA);
            //smallRoid.GetComponent<N_SpawningCheck>().sendSize = sizeOfNewA;
            //NetworkServer.Spawn(smallRoid);
        }
    }
    IEnumerator CreateTempObject(Vector3 spawnPositionIn, float spawnSizeIn)
    {
        GameObject tempTransformHolder = new GameObject();
        //float roidSize = Random.Range(0.5f, maxRoidSize);
        // randomize the location of the new spawnpoint, set from spawnvalues
        //Vector3 spawnLocation = new Vector3(Random.Range(-spawnValues.x, spawnValues.x),
        //            Random.Range(-spawnValues.y, spawnValues.y),
        //            Random.Range(-spawnValues.z, spawnValues.z));
        tempTransformHolder.transform.position = spawnPositionIn;
        tempTransformHolder.transform.rotation = Random.rotation;
        tempTransformHolder.transform.localScale = new Vector3(spawnSizeIn, spawnSizeIn, spawnSizeIn);
        SpawnCheck SC = tempTransformHolder.AddComponent<SpawnCheck>();
        SC.AddSpawnComponents(spawnManager, tempTransformHolder.transform);
        yield return null;
    }
    void HitByPlayer()
    {
        Debug.Log(hits + " Hits left");
        if (hits > 1)
        {
            TakeHit(1);
        }
    }
    
    void OnHitsChange(int hits)
    {
        if(isServer)
            RpcClientHitsChange(hits);
    }
    [ClientRpc]
    void RpcClientHitsChange(int hits)
    {
        Debug.Log("ChangedClientHits");
    }
    void TakeHit(int hitsIn)
    {
        Debug.Log("Registered a hit");
        if (!isServer)
        {
            return;
        }
        Debug.Log("server thinks its a hit");
        for (int i = 0; i < hitsIn; i++)
        {
            hits--;
            OnTakeHit();
        }
    }
    void OnTakeHit()
    {
        if (hits > 0)
            return;
        if (!beingDestroyed)
        {
            beingDestroyed = true;
            GameObject explode = Instantiate(roidExplosion, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
            explode.transform.localScale = gameObject.transform.localScale;
            StartCoroutine(fillVolumeOfRoid());
            //mainScore.AddScore(1);
            spawnManager.UnSpawnObject(gameObject);
            NetworkServer.UnSpawn(gameObject);
            //Destroy(gameObject,0.3f);
        }
    }
}
public static class SphereVolume
{
    static int[] preCalcVolumes = {0,2,19,64,151,295,509,808,1206,1718,2356,3136,4072,5177,6465,7952,9651,11576,13741,16161,18850,21821,
        25089,28668,32572,36816,41412,46377,51723,57465,63617,70193,77208,84675,92608,101022,109931,119348,129289,
        139767,150796,162391,174566,187334,200710,214708,229342,244627,260576,277204};

    public static int fromRadius(int indexIn)
    {
        int newVolume = preCalcVolumes[indexIn];
        return newVolume;
    }
    public static int radiusFromVolume(int volumeIn)
    {
        return Mathf.RoundToInt(Mathf.Pow((volumeIn / 2.3562f), 0.333f));
    }
}
public class NetworkTransSync
{
    [SyncVar]
    public Vector3 pos;
    [SyncVar]
    public Quaternion rot;
    [SyncVar]
    public Vector3 scl;

    public NetworkTransSync(Vector3 posIn,Quaternion rotIn,Vector3 sclIn)
    {
        pos = posIn;
        rot = rotIn;
        scl = sclIn;
    }
}
