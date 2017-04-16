using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
	SocketManager socketManager = null;

	public static Boolean isMyTurn = false;
	public static Boolean isPlayed = false;
	public static Boolean isGetACard = false;
	public static Boolean isActioned = false;
	public static String actionCode = "";

	public static int jin;
	public static Boolean isChing = false;
	public static Boolean isPenging = false;
	public static Boolean isGanging = false;
	public static Boolean isHuing = false;
	public static int currentMaJiangid;
	public static Hand myHand = new Hand();
	public static List<Card> chiRelateds;
	public static Card chiSmall;
	public static Card chiBig;
	public static UserAction userAction;
	public static List<GameObject> actions = new List<GameObject> ();

	void Start () {
		float[] posX = new float[]{ -6f, -6f, -3.5f, 5.5f};
		float[] posY = new float[]{ -1.5f, 1.5f, 4f, 1.5f};
		GameObject[] objs = new GameObject[4];

		for (int i = 0; i < 4; i++) {
			objs [i] = Resources.Load ("myPhoto") as GameObject;
			objs [i].transform.position = new Vector3 (posX [i], posY [i], 0f); 
			objs [i] = Instantiate (objs [i]);
		}

		socketManager = SocketManager.GetInstance();
		socketManager.initGame ();
	}

	void Update () {
		socketManager.update();
	}
}
