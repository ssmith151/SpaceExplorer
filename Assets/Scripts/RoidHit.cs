using UnityEngine;
using System.Collections;

public class RoidHit : MonoBehaviour
{

    public int hits { get; set; }
    public GameObject roidExplosion;
    public GameObject newRoid;
    public float maxtumble;

    private Rigidbody rb;
    private GameContoller mainScore;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        hits = Mathf.RoundToInt(gameObject.transform.localScale.x);
        rb.angularVelocity = Random.insideUnitSphere / (hits / maxtumble);
        SlowStart();
        mainScore = GameObject.Find("GameController").GetComponent<GameContoller>();
    }
    void SlowStart()
    {
        rb.drag = 40f;
        StartCoroutine(normalizeDrag());
    }
    IEnumerator normalizeDrag()
    {
        for(int i = 0; i<50; i++)
        {
            yield return new WaitForSeconds(0.1f);
            rb.drag -= rb.drag/2f;
        }
        rb.drag = 0;
        yield return null;
        hits = Mathf.RoundToInt(gameObject.transform.localScale.x);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Temp") || other.CompareTag("Roid"))
        {
            return;
        }
        if (hits > 1)
        {
            hits--;
        }
        else
        {
            GameObject explode = Instantiate(roidExplosion, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
            explode.transform.localScale = gameObject.transform.localScale;
            int sizeOfA = Mathf.RoundToInt(gameObject.transform.localScale.x);
            if (sizeOfA > 3) {
                Vector3 sprV;
                Vector3 roidV = gameObject.transform.position;
                for (int i = 0; i <= 5; i++)
                {
                    sprV = Random.insideUnitSphere;
                    Vector3 roidSpawn = new Vector3(sprV.x + roidV.x, sprV.y + roidV.y, sprV.z + roidV.z);
                    GameObject smallRoid = Instantiate(newRoid, roidSpawn, transform.rotation) as GameObject;
                    smallRoid.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    smallRoid.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    smallRoid.GetComponent<RoidHit>().hits = 1;
                }
                mainScore.AddScore(3);
            }
            if (sizeOfA > 12)
            {
                Vector3 sprV;
                Vector3 roidV = gameObject.transform.position;
                for (int i = 0; i <= 4; i++)
                {
                    sprV = Random.insideUnitSphere;
                    Vector3 roidSpawn = new Vector3(sprV.x + roidV.x, sprV.y + roidV.y, sprV.z + roidV.z);
                    GameObject smallRoid = Instantiate(newRoid, roidSpawn, transform.rotation) as GameObject;
                    smallRoid.transform.localScale = new Vector3(7.0f, 7.0f, 7.0f);

                    smallRoid.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                mainScore.AddScore(12);
            }
            if (sizeOfA > 25)
            {
                Vector3 sprV;
                Vector3 roidV = gameObject.transform.position;
                for (int i = 0; i <= 3; i++)
                {
                    sprV = Random.insideUnitSphere;
                    Vector3 roidSpawn = new Vector3(sprV.x + roidV.x, sprV.y + roidV.y, sprV.z + roidV.z);
                    GameObject smallRoid = Instantiate(newRoid, roidSpawn, transform.rotation) as GameObject;
                    smallRoid.transform.localScale = new Vector3(20.0f, 20.0f, 20.0f);
                    smallRoid.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                mainScore.AddScore(25);
            }
            mainScore.AddScore(1);
            Destroy(gameObject);
        }
    }
    void HitByPlayer()
    {
        Debug.Log(hits + " Hits left");
        if (hits > 1)
        {
            hits--;
        }
        else
        {
            GameObject explode = Instantiate(roidExplosion, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
            explode.transform.localScale = gameObject.transform.localScale;
            if (gameObject.transform.localScale.x > 3)
            {
                //    for (int i = 0; i <= (gameObject.transform.localScale.x - 3); i++)
                //    {
                //        Vector3 roidSpawn = Random.insideUnitSphere * i;
                //        Instantiate(newRoid, transform.position, transform.rotation);
                //}
                Instantiate(newRoid, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}
