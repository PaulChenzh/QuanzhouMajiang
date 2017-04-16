using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
	private float lastTime;
	private float curTime;

	private static int timeSize = 30;
	private static int restTime;

	private float[] posForNumber1 = new float[]{ 
		-0.12f, -0.15f, -0.15f, -0.15f, -0.15f, -0.15f, -0.15f, -0.15f, -0.15f, -0.15f
	};
	private float[] posForNumber2 = new float[]{ 
		0.12f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f
	};

	private GameObject[] numbers = new GameObject[2];

	private static Timer timer = new Timer(); 
	public static Timer GetInstance() {
		return timer;
	}

	private static bool isInit = false;
	private static int nowPlayer;
	private static int myPos;
	private static GameObject sign;

	public void init (int player, int pos) {
		isInit = true;
		restTime = timeSize;
		lastTime = Time.time;
		nowPlayer = player;
		myPos = pos;
		showSign (nowPlayer);
	}

	private void showSign(int pos) {
		if (sign != null) Destroy (sign);
		string signDir = "";
		int now = (pos + 4 - myPos) % 4;
		if (now == 0) signDir = "botton";
		else if (now == 1) signDir = "left";
		else if (now == 2) signDir = "top";
		else if (now == 3) signDir = "right";
		sign = Resources.Load ("sign_" + signDir) as GameObject;
		sign = Instantiate (sign);	
	}

	void Start () {}

	void Update () {
		if (!isInit) return; 

		curTime = Time.time;
		if (curTime - lastTime >= 1) {
			restTime -= 1;
			lastTime = curTime;
//			Debug.Log("restTime = " + restTime);
			Destroy (numbers [0]);
			Destroy (numbers [1]);
			int number1 = restTime / 10;
			int number2 = restTime % 10;
			numbers[0] = Resources.Load ("timer_" + number1) as GameObject;
			numbers[0].transform.position = new Vector3 (posForNumber1[number1], 0f, 0f);
			numbers[0] = Instantiate (numbers[0]);	
			numbers[1] = Resources.Load ("timer_" + number2) as GameObject;
			numbers[1].transform.position = new Vector3 (posForNumber2[number2], 0f, 0f);
			numbers[1] = Instantiate (numbers[1]);	

		}
	}

	public void newTurn() {
		Debug.Log("This is a new turn...");
		lastTime = timeSize;
		nowPlayer ++;
		showSign (nowPlayer);
	}

	public void newTurn(int t) {
		Debug.Log("This is a new turn * " + t + "...");
		lastTime = timeSize;
		nowPlayer += t;
		showSign (nowPlayer);
	}
}
