using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {
	void Start () {}
	void Update () {}

	void OnMouseDown () {
		Debug.Log("Going to the next step...");
		Application.LoadLevel ("Loading");
	}
}
