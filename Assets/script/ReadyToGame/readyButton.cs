using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class readyButton : MonoBehaviour {
	GameObject IAmReady;
	void Start () {}
	void Update () {}
	void OnMouseDown() {
		GameObject IAmReady = Instantiate (Resources.Load ("IAmReady") as GameObject);
		IAmReady.transform.position = new Vector3 (-5.2f, -1.5f, 0f); 
		IAmReady = Instantiate (IAmReady);
		this.gameObject.SetActive (false);

		SocketManager sm = SocketManager.GetInstance ();
		sm.handIsUp ();
//		Application.LoadLevel ("Game");
	}
}
