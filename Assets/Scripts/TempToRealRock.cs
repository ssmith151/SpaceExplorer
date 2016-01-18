using UnityEngine;
using System.Collections;

public class TempToRealRock : MonoBehaviour
{
    /// <summary>
    /// This script is put onto a prefab with the following components :
    /// 1. a ridgidbody
    /// 2. a collider with a trigger roughly the same size as the object, as it will be the placeholder
    ///     for the incoming roid.
    /// 3. reset the scale and transform of the GameObject and put into prefabs
    /// </summary>

    public GameObject newRoid;

    void Start() {
        // After the object decides to delete itself, it will replace itself with the real prefab
        // Notes on real prefab : 
        // real prefab needs a collider and ridgidbody compent to be detected for collision
        GameObject createRock = Instantiate(newRoid, transform.position, transform.rotation) as GameObject;
        createRock.transform.localScale = gameObject.transform.localScale;
        createRock.GetComponent<Rigidbody>().mass = 10.0f * gameObject.transform.localScale.x;
        createRock.name = gameObject.name.Remove(0,4);
        //gets rid of temp object
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // this part of the script deletes this object if the object occupies the same space as another roid
        // this method is acctually called BEFORE the start function (that happens on the first frame)
        Debug.Log(other.name + "   " + gameObject.name);
        if (other != null)
        {
            Destroy(gameObject);
        }
        
    }

}
