using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Rope : MonoBehaviour {
 
	// Use this for initialization
	void Start () {
        gameObject.GetComponent<CharacterJoint>().connectedBody = gameObject.transform.parent.GetComponent<Rigidbody>();
	}
}