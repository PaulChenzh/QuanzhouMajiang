using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GangEvent : MonoBehaviour {

	void Start () {}
	void Update () {}

	void OnMouseDown() {
		Debug.Log("Start GangEvent...");
		int count = 0;
		List<Card> cards = Main.myHand.getCards ();
		for (int i = cards.Count - 1; i >= 0; i --) {
			if (cards [i].getMaJiangId() == Main.currentMaJiangid) {
				cards[i].getMaJiang(); // TODO 15 将这张牌从这个游戏中移除
				cards.RemoveAt (i); // 从手拍中移除
				count++;
				if (count == 3) break;
			}
		}
		// TODO 16
		// 隐藏杠的图标
		// 读秒隐藏
		// 出现杠的效果
		// 等待杠的效果结束，将所杠放置在左手边，用used的牌的图片
		Main.actionCode = "GANG";
		Main.isActioned = true;
		Debug.Log("End GangEvent.");
	}
}