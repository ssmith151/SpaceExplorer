using UnityEngine;
using System.Collections;

public class PlayerShot : MonoBehaviour
{

    public GameObject playerShotEXP;
    public float timeCounter;
    public float speed;

    private Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, timeCounter);
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }
        Instantiate(playerShotEXP, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(gameObject);
    }
}
