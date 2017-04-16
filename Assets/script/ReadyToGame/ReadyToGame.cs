using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReadyToGame
{
	public class ReadyToGame : MonoBehaviour {
		float[] posX = new float[]{ -6f, -6f, -3.5f, 5.5f};
		float[] posY = new float[]{ -1.5f, 1.5f, 4f, 1.5f};
		float[] okPosX = new float[]{ -5.2f, -5.2f, -3.5f, 4.7f};
		float[] okPosY = new float[]{ -1.5f, 1.5f, 2.8f, 1.5f};
		GameObject[] objs = new GameObject[4];
		GameObject[] readys = new GameObject[4];
		int[] posStatus;
		int positionForMe;
		SocketManager sm;

		void Start () { 
			GameObject myPhoto = Resources.Load ("myPhoto") as GameObject;
			myPhoto.transform.position = new Vector3 (-6f, -1.5f, 0f); 
			Instantiate (myPhoto);
			sm = SocketManager.GetInstance ();
			positionForMe = sm.getPositionNumber (); // 从0开始，0表示房主
			posStatus = new int[4];
			posStatus[positionForMe] = 1;
			if (positionForMe == 0) {
				GameObject dismissButton = Resources.Load ("dismissButton") as GameObject;
				dismissButton.transform.position = new Vector3 (4f, -4f, 0f); 
				dismissButton = Instantiate (dismissButton);
			}
		}

		void Update () {
			string newPlayerStatus = sm.getPlayerStatus ();
			if ("ROOM IS DISMISSED!".Equals(newPlayerStatus.ToUpper())) {
				// 提示已经解散，然后点击确定返回Menu
			} else {
				for (int i = 1; i < 4; i ++) {
					int nowStatus = newPlayerStatus [(i + positionForMe) % 4] - '0';
					int oldStatus = posStatus [(i + positionForMe) % 4];
					if (nowStatus >= 1 && oldStatus == 0) {
						objs [i] = Resources.Load ("myPhoto") as GameObject;
						objs [i].transform.position = new Vector3 (posX [i], posY [i], 0f); 
						objs [i] = Instantiate (objs [i]);
						if (nowStatus == 2) {
							readys [i] = Resources.Load ("IAmReady") as GameObject;
							readys [i].transform.position = new Vector3 (okPosX [i], okPosY [i], 0f); 
							readys [i] = Instantiate (readys [i]);
						}
					} else if (nowStatus == 0 && oldStatus >= 1) {
						Destroy (objs [i]);
						if (nowStatus == 2) {
							Destroy (readys [i]);
						}
					}
				}
				if (newPlayerStatus.Equals ("2222\n")) {
					Application.LoadLevel ("Game");
				}
			}
		}
	}
}