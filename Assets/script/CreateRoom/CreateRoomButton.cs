using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoomButton : MonoBehaviour {
	void Start () {}
	void Update () {}

	void OnMouseDown () {
		Debug.Log("Going to the next step...");
		SocketManager sm = SocketManager.GetInstance ();
		if (sm.openRoom ()) {
			Application.LoadLevel ("ReadyToGame");
		}
	}
}
