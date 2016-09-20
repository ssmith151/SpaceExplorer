using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class N_SpawningCheck : NetworkBehaviour {

    /// <summary>
    /// This script is put onto a prefab with the following components :
    /// 1. a ridgidbody
    /// 2. a collider with a trigger roughly the same size as the object, as it will be the placeholder
    ///     for the incoming roid.
    /// 3. reset the scale and transform of the GameObject and put into prefabs
    /// </summary>
    [SyncVar(hook = "OnSendSize")]
    public float sendSize=1;
    
    //public GameObject newRoid;
    SpawnManager spawnManager;

    void Start()
    {
        spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        if(spawnManager == null)
        {
            Debug.Log("Manager Not found");
        }
    }
    [ServerCallback]
    public void OnSendSize(float sendSize)
    {
        spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        //Debug.Log("OnsendSizeRecieved");
        gameObject.transform.localScale = new Vector3(sendSize, sendSize, sendSize);
        Vector3 unini = gameObject.transform.position;
        GameObject createRock = spawnManager.GetFromPool(unini);
        createRock.name = gameObject.name.Remove(0, 4);
        //createRock.GetComponent<N_RoidHit>().MatchServerSizes(sendSize);
        //ClientScene.RegisterPrefab(createRock);
        NetworkServer.Spawn(createRock,spawnManager.assetId);
    }
    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
        {
            return;
        }
        // this part of the script deletes this object if the object occupies the same space as another roid
        // this method is acctually called BEFORE the start function (that happens on the first frame)
        Debug.Log(other.name + "   " + gameObject.name);
        if (other != null)
        {
            NetworkServer.Destroy(gameObject);
        }

    }
}
public class SpawnCheck : NetworkBehaviour
{
    //public GameObject InputRoid;
    public CapsuleCollider CC;
    [SyncVar]
    public Transform finalForm;

    SpawnManager spawnManager;

    public SpawnCheck(SpawnManager smIn, Transform transformIn)
    {
        gameObject.AddComponent<NetworkIdentity>();
        CC = gameObject.AddComponent<CapsuleCollider>();
        gameObject.transform.localScale = transformIn.localScale;
        gameObject.transform.rotation = transformIn.rotation;
        spawnManager = smIn;
        finalForm = transformIn;
        //InputRoid = newRoidIn;

        StartCoroutine(SpawnFinalFromPool());
    }
    public void AddSpawnComponents(SpawnManager smIn, Transform transformIn)
    {
        if (!isServer)
            return;
        gameObject.tag = "Temp";
        //gameObject.AddComponent<NetworkIdentity>();
        CC = gameObject.AddComponent<CapsuleCollider>();
        CC.isTrigger = true;
        gameObject.transform.localScale = transformIn.localScale;
        gameObject.transform.rotation = transformIn.rotation;
        spawnManager = smIn;
        finalForm = transformIn;
        //InputRoid = newRoidIn;

        StartCoroutine(SpawnFinalFromPool());
    }
    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
        {
            return;
        }
        // this part of the script deletes this object if the object occupies the same space as another roid
        // this method is acctually called BEFORE the start function (that happens on the first frame)
        Debug.Log(other.name + "   " + gameObject.name);
        if (other != null)
        {
            // there needs to be a check here to stop SpawnFinalFromPool() if it is in progress.
            NetworkServer.Destroy(gameObject);
        }

    }
    IEnumerator SpawnFinalFromPool()
    {
        yield return new WaitForEndOfFrame();
        if (spawnManager.poolEmpty())
        {
            Debug.Log("no pool available");
            yield return null;
        }
        GameObject finalRoid = spawnManager.GetFromPool(finalForm.position);
        if (finalRoid != null)
        {
            N_RoidHit nRoid = finalRoid.GetComponent<N_RoidHit>();
            nRoid.transforming = true;
            finalRoid.GetComponent<N_RoidHit>().ApplyTransform(finalForm);
            yield return new WaitWhile(() => nRoid.transforming);
            //finalRoid.transform.rotation = finalForm.rotation;
            //finalRoid.transform.localScale = finalForm.localScale;
            //ClientScene.RegisterPrefab(finalRoid);
            finalRoid.SetActive(true);
            NetworkServer.Spawn(finalRoid);
            //nRoid.RpcMatchServerSizes();
        } else
        {
            Debug.Log("no gameobjects in pool");
            NetworkServer.Destroy(gameObject);
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
