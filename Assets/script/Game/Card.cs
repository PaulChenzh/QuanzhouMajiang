using System;
using UnityEngine;

public class Card {
	private GameObject maJiang;
	private int maJiangId;

	public static int WAN = 0;
	public static int TIAO = 1;
	public static int TONG = 2;
	public static int FENG = 3;

	public Card(GameObject maJiang, int maJiangId) {
		this.maJiang = maJiang;
		this.maJiangId = maJiangId;
	}

	public GameObject getMaJiang() {
		return maJiang;
	}

	public int getMaJiangId() {
		return maJiangId;
	}
}