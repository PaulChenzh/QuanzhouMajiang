using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoard : MonoBehaviour {

	private float[] posX = new float[]{-2.1f, -1.5f, -0.85f, -0.2f, 0.45f, 1.1f};
	private static GameObject[] numbers = new GameObject[6];
	private static int count = 0;
	private static int roomNumber = 0;

	void Start () { }
	void Update () { }
	void OnMouseDown() {
		string name = this.name;
		if (name.Length <= "keyboardNumber".Length) {
			if (name.Contains ("Delete")) {
				if (count > 0) {
					count --;
					Destroy(numbers[count]);
					numbers [count] = null;
					roomNumber /= 10;
				}
			} else {
				for (int j = 0; j < numbers.Length; j++) {
					Destroy(numbers[j]);
				}
				numbers = new GameObject[6];
				count = 0;
				roomNumber = 0;
			}
		} else if (count < 6) {
			if (name.Contains ("0")) {
				numbers [count] = Resources.Load ("kn0") as GameObject;
				roomNumber = roomNumber * 10 + 0;
			} else if (name.Contains ("1")) {
				numbers [count] = Resources.Load ("kn1") as GameObject;
				roomNumber = roomNumber * 10 + 1;
			} else if (name.Contains ("2")) {
				numbers [count] = Resources.Load ("kn2") as GameObject;
				roomNumber = roomNumber * 10 + 2;
			} else if (name.Contains ("3")) {
				numbers [count] = Resources.Load ("kn3") as GameObject;
				roomNumber = roomNumber * 10 + 3;
			} else if (name.Contains ("4")) {
				numbers [count] = Resources.Load ("kn4") as GameObject;
				roomNumber = roomNumber * 10 + 4;
			} else if (name.Contains ("5")) {
				numbers [count] = Resources.Load ("kn5") as GameObject;
				roomNumber = roomNumber * 10 + 5;
			} else if (name.Contains ("6")) {
				numbers [count] = Resources.Load ("kn6") as GameObject;
				roomNumber = roomNumber * 10 + 6;
			} else if (name.Contains ("7")) {
				numbers [count] = Resources.Load ("kn7") as GameObject;
				roomNumber = roomNumber * 10 + 7;
			} else if (name.Contains ("8")) {
				numbers [count] = Resources.Load ("kn8") as GameObject;
				roomNumber = roomNumber * 10 + 8;
			} else if (name.Contains ("9")) {
				numbers [count] = Resources.Load ("kn9") as GameObject;
				roomNumber = roomNumber * 10 + 9;
			}
			numbers [count].transform.position = new Vector3 (posX[count], 2f, 0f); 
			numbers[count] = Instantiate (numbers [count]);
			count++;
			if (count == 6) {
				SocketManager sm = SocketManager.GetInstance ();
				if (sm.joinRoom (roomNumber) == 1) {
					Application.LoadLevel ("ReadyToGame");
				} else {
					Instantiate (Resources.Load ("closeFlag") as GameObject);
					Instantiate (Resources.Load ("message_roomIsNotExist") as GameObject);
				}
			}
		}
	}
}
