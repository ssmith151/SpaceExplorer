using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public int m_ObjectPoolSize = 40;
    public GameObject m_Prefab;
    public GameObject[] m_Pool;
    public bool registered = false;

    public NetworkHash128 assetId { get; set; }

    public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetId);
    public delegate void UnSpawnDelegate(GameObject spawned);

    void Awake()
    {
        assetId = m_Prefab.GetComponent<NetworkIdentity>().assetId;
        m_Pool = new GameObject[m_ObjectPoolSize];
        for (int i = 0; i < m_ObjectPoolSize; ++i)
        {
            m_Pool[i] = (GameObject)Instantiate(m_Prefab, Vector3.zero, Quaternion.identity);
            m_Pool[i].name = "PoolObject" + i;
            m_Pool[i].SetActive(false);
        }
        Debug.Log("Created Asset Pool");
        ClientScene.RegisterSpawnHandler(assetId, SpawnObject, UnSpawnObject);
        registered = true;
    }

    public GameObject GetFromPool(Vector3 position)
    {
        foreach (var obj in m_Pool)
        {
            if (!obj.activeInHierarchy)
            {
                Debug.Log("Activating object " + obj.name + " at " + position);
                obj.transform.position = position;
                obj.SetActive(true);
                return obj;
            }
        }
        Debug.LogError("Could not grab object from pool, nothing available");
        return null;
    }
    public bool poolEmpty()
    {
        foreach(GameObject go in m_Pool)
        {
            if (go.activeInHierarchy)
                return false;
        }
        return true;
    }
    public GameObject SpawnObject(Vector3 position, NetworkHash128 assetId)
    {
        return GetFromPool(position);
    }

    public void UnSpawnObject(GameObject spawned)
    {
        Debug.Log("Re-pooling object " + spawned.name);
        spawned.SetActive(false);
    }
}