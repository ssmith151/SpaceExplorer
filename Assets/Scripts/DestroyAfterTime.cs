using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour {

    public float timeWait;

	void Start () {
        Destroy(gameObject, timeWait);
	}
}
