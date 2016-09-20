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
    private bool beingDestroyed;

    void Awake()
    {
        beingDestroyed = false;
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
        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForSeconds(0.1f);
            rb.drag -= rb.drag / 2f;
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
        else if (!beingDestroyed)
        {
            beingDestroyed = true;
            GameObject explode = Instantiate(roidExplosion, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
            explode.transform.localScale = gameObject.transform.localScale;
            StartCoroutine(fillVolumeOfRoid());
            mainScore.AddScore(1);
            Destroy(gameObject);
        }
    }
    IEnumerator fillVolumeOfRoid()
    {
        // this coroutine is an attempt to fill the volume of the space contained by the asteroid
        // there is a helper class 'SphereVolume' to calculate the volume from radius and visa versa
        int sizeOfA = Mathf.RoundToInt(gameObject.transform.localScale.x);
        if (sizeOfA > 3)
        {
            Vector3 sprV;
            Vector3 roidV = gameObject.transform.position;
            int volOfA = SphereVolume.fromRadius(sizeOfA);
            int sizeOfNewA = 0;
            int volOfNewA = 0;
            mainScore.AddScore(sizeOfA);
            for (int i = 0; i < sizeOfA; i++)
            {
                if (sizeOfA > 1)
                    sizeOfNewA = Mathf.RoundToInt(Random.Range(1, sizeOfA - 1));
                else
                    yield return null;
                volOfNewA = SphereVolume.fromRadius(sizeOfNewA);
                sprV = Random.insideUnitSphere;
                int radiusFromCenter = sizeOfA - sizeOfNewA;
                Vector3 roidSpawn = new Vector3(sprV.x * radiusFromCenter + roidV.x,
                                                sprV.y * radiusFromCenter + roidV.y,
                                                sprV.z * radiusFromCenter + roidV.z);
                GameObject smallRoid = Instantiate(newRoid, roidSpawn, transform.rotation) as GameObject;
                smallRoid.transform.localScale = new Vector3(sizeOfNewA, sizeOfNewA, sizeOfNewA);
                smallRoid.GetComponent<Rigidbody>().velocity = Vector3.zero;
                smallRoid.GetComponent<RoidHit>().hits = sizeOfNewA;
                volOfA = volOfA - volOfNewA;
                sizeOfA = SphereVolume.radiusFromVolume(volOfA);
            }
        }
        yield return null;
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
                Instantiate(newRoid, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}
//public static class SphereVolume
//{
//    static int[] preCalcVolumes = {0,2,19,64,151,295,509,808,1206,1718,2356,3136,4072,5177,6465,7952,9651,11576,13741,16161,18850,21821,
//        25089,28668,32572,36816,41412,46377,51723,57465,63617,70193,77208,84675,92608,101022,109931,119348,129289,
//        139767,150796,162391,174566,187334,200710,214708,229342,244627,260576,277204};

//    public static int fromRadius(int indexIn)
//    {
//        int newVolume = preCalcVolumes[indexIn];
//        return newVolume;
//    }
//    public static int radiusFromVolume(int volumeIn)
//    {
//        return Mathf.RoundToInt(Mathf.Pow((volumeIn / 2.3562f), 0.333f));
//    }
//}
