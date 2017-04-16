using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closeScript : MonoBehaviour {
	void Start () { }
	void Update () { }
	void OnMouseDown() {
		GameObject[] objs = GameObject.FindGameObjectsWithTag("message");
		for (int i = 0; i < objs.Length; i++) {
			Destroy (objs [i]);
		}
	}
}
