using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public GameContoller gc;
    public float maxForwardSpeed;
    public float minForwardSpeed;
    public float rotationSmoother;
    public float autoCorrectSmoother;
    public float sidewaysSpeedReducer;
    public GameObject shotSpawn;
    public GameObject shot;
    public GameObject[] altShots;
    public GameObject playerExp;
    public Image speedBar;
    public Image health;
    public int hitsMax;
    public int hits;

    private Rigidbody rb;
    private float currentForwardSpeed;
    private float currentSideWaysSpeed;
    private float horizontal;
    private float vertical;
    private float mouseX;
    private float mouseY;
    private bool mouseFire;
    private bool mouseAltFire;
    private AudioSource audS;
    private bool inCollider;


    // Use this for initialization
    void Awake () {
        audS = shotSpawn.GetComponent<AudioSource>();
        rb = gameObject.GetComponent<Rigidbody>();
        inCollider = false;

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        mouseFire = Input.GetButtonDown("Fire1");
        mouseAltFire = Input.GetButtonDown("Fire2");

        horizontal = Mathf.Clamp(horizontal, -1, 1);
        vertical =  Mathf.Clamp(vertical, -1, 1);
        mouseX = rotationSmoother * Mathf.Clamp(mouseX, -1, 1);
        mouseY = rotationSmoother * Mathf.Clamp(mouseY, -1, 1);

        float newForwardSpeed = 0.0f;
        newForwardSpeed += vertical;
        newForwardSpeed = currentForwardSpeed + newForwardSpeed;
        currentForwardSpeed = Mathf.Clamp(newForwardSpeed, minForwardSpeed, maxForwardSpeed);
        float speedBarFill = currentForwardSpeed / maxForwardSpeed;
        speedBar.fillAmount = speedBarFill;

        float newHorizontalSpeed = 0.0f;
        newHorizontalSpeed += horizontal;
        newHorizontalSpeed = currentSideWaysSpeed + newHorizontalSpeed - Mathf.Sign(currentSideWaysSpeed) * sidewaysSpeedReducer;
        currentSideWaysSpeed = Mathf.Clamp(newHorizontalSpeed, -5.0f, 5.0f);


        if (mouseFire && !mouseAltFire)
        {
            audS.Play();
            Instantiate(shot, shotSpawn.transform.position, shotSpawn.transform.rotation);
        }
        if (mouseAltFire && !mouseFire)
        {
            audS.Play();
            if ( altShots[0]!= null)
            {
                Instantiate(altShots[0], shotSpawn.transform.position, shotSpawn.transform.rotation);
            }

        }

        //Quaternion newFacing = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        //newFacing = Quaternion.Euler(rb.transform.rotation.x - mouseY,
        //   rb.transform.rotation.y + mouseX, rb.transform.rotation.z);
        //rb.MoveRotation(newFacing * rb.rotation);
        rb.transform.Rotate(-mouseY, mouseX, 0);

        Vector3 movement = new Vector3(newHorizontalSpeed * Time.deltaTime, 0.0f, currentForwardSpeed * Time.deltaTime);
        rb.transform.Translate(movement);
    }

    void OnTriggerEnter (Collider other)
    {
        if (gc.isSpawningLevel)
        {
            return;
        }
        if (other.CompareTag("Player"))
            return;
        if (inCollider == false)
        {
            Debug.Log("there is a collision");
            StartCoroutine(DamagePlayer(other));
        }
    }
    IEnumerator DamagePlayer(Collider other) {
        if (hits > 0)
        {
            inCollider = true;
            //Debug.Log(other.name);
            other.gameObject.SendMessage("HitByPlayer");
            hits--;
            CheckHits();
            yield return new WaitForSeconds(0.1f);
            inCollider = false;
        }
    }
    void CheckHits() {
        float newFill = hits * 1.0f / hitsMax * 1.0f;
        Debug.Log(newFill);
        health.fillAmount = newFill;
        if (hits <= 0)
        {
            gc.GameOver();
            Instantiate(playerExp, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(gameObject);
        }
    }
}
