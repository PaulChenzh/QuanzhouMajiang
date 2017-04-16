using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreateRoom
{
	public class CreateRoomButton : MonoBehaviour {
		void Start () {}
		void Update () {}

		void OnMouseDown () {
			Debug.Log("Going to the next step...");
			Application.LoadLevel ("CreateRoom");
		}
	}

}