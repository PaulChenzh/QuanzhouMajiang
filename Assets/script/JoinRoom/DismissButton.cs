using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DismissButton : MonoBehaviour {
	void Start () {}
	void Update () {}
	void OnMouseDown() {
		Application.LoadLevel ("Menu");
	}
}
