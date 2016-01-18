using UnityEngine;
using System.Collections;

public class RoidHit : MonoBehaviour
{

    public int hits;
    public GameObject roidExplosion;
    public GameObject newRoid;
    public float maxtumble;

    private Rigidbody rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        hits = Mathf.RoundToInt(gameObject.transform.localScale.x);
        rb.angularVelocity = Random.insideUnitSphere / (hits / maxtumble);
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
            if (gameObject.transform.localScale.x > 3) {
                for (int i = 0; i <= 5; i++)
                {
                    Vector3 roidSpawn = Random.insideUnitSphere * i;
                    GameObject smallRoid = Instantiate(newRoid, roidSpawn, transform.rotation) as GameObject;
                    smallRoid.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
            }
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
