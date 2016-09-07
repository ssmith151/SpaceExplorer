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
    public int playerCurrentAltShot;
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
    private float mouseWheel;
    private AudioSource audS;
    private bool inCollider;
    // samples for testing, no concept designed
    private string[] altFireNames  = new string[] {"Sticky Bombs", "Blaster Cannon", "Photon torpedos"};

    // Use this for initialization
    void Awake () {
        audS = shotSpawn.GetComponent<AudioSource>();
        rb = gameObject.GetComponent<Rigidbody>();
        inCollider = false;
        if (playerCurrentAltShot == -1 && altShots.Length > 0)
        {
            playerCurrentAltShot = 0;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        PlayerInputs();

        ForwardSpeedCalc();

        SidewaysSpeedCalc();

        Fire();

        AltFire();

        if (mouseWheel != 0 && altShots.Length>1)
        {
            AltFireSelect();
        }

        rb.transform.Rotate(-mouseY, mouseX, 0);

        Vector3 movement = new Vector3(currentSideWaysSpeed * Time.deltaTime, 0.0f, currentForwardSpeed * Time.deltaTime);
        rb.transform.Translate(movement);
    }
    void PlayerInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        mouseFire = Input.GetButtonDown("Fire1");
        mouseAltFire = Input.GetButtonDown("Fire2");
        mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        horizontal = Mathf.Clamp(horizontal, -1, 1);
        vertical = Mathf.Clamp(vertical, -1, 1);
        mouseX = rotationSmoother * Mathf.Clamp(mouseX, -1, 1);
        mouseY = rotationSmoother * Mathf.Clamp(mouseY, -1, 1);
    }
    void ForwardSpeedCalc()
    {
        float newForwardSpeed = 0.0f;
        newForwardSpeed += vertical;
        newForwardSpeed = currentForwardSpeed + newForwardSpeed;
        currentForwardSpeed = Mathf.Clamp(newForwardSpeed, minForwardSpeed, maxForwardSpeed);
        float speedBarFill = currentForwardSpeed / maxForwardSpeed;
        speedBar.fillAmount = speedBarFill;
    }
    void SidewaysSpeedCalc()
    {
        float newHorizontalSpeed = 0.0f;
        newHorizontalSpeed += horizontal;
        newHorizontalSpeed = currentSideWaysSpeed + newHorizontalSpeed - Mathf.Sign(currentSideWaysSpeed) * sidewaysSpeedReducer;
        currentSideWaysSpeed = Mathf.Clamp(newHorizontalSpeed, -5.0f, 5.0f);
    }
    void Fire()
    {
        if (mouseFire && !mouseAltFire)
        {
            audS.Play();
            Instantiate(shot, shotSpawn.transform.position, shotSpawn.transform.rotation);
        }
    }
    void AltFire()
    {
        if (mouseAltFire && !mouseFire)
        {
            if (playerCurrentAltShot != -1)
            {
                audS.Play();
                Instantiate(altShots[playerCurrentAltShot], shotSpawn.transform.position, shotSpawn.transform.rotation);
            }
        }
    }
    void AltFireSelect()
    {
        if (mouseWheel > 0)
        {
            if (playerCurrentAltShot + 1 < altShots.Length)
                playerCurrentAltShot += 1;
            else
                playerCurrentAltShot = 0;
        }
        else
        {
            if (playerCurrentAltShot > 0)
                playerCurrentAltShot -= 1;
            else
                playerCurrentAltShot = altShots.Length - 1;
        }
        MessageToAltShotDisplay();
    }
    void MessageToAltShotDisplay()
    {
        gc.UpdateAltShotText(altFireNames[playerCurrentAltShot]);
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
            //Debug.Log("there is a collision");
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
