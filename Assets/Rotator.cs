using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

    Quaternion spinAdd;
	// Use this for initialization
	void Start () {
        spinAdd = Quaternion.Euler(new Vector3(0.01f, 0, 0));
        InvokeRepeating("Spin", 0, 0.1f);
	}
	
	void Spin()
    {
        gameObject.transform.eulerAngles += new Vector3(0, 0, 0.1f);
    }
}
