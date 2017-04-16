using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 需要重构，但是是之后的事情
public class MaJiangEvent : MonoBehaviour { 

	public Boolean isInOriginalPositions = true;

	void Start () {}
	void Update () {}

	void OnMouseDown() {
		if (Main.isChing) {
			List<Card> cards = Main.myHand.getCards (); // 先将所有卡牌位置还原
			for (int i = 0; i < cards.Count; i ++) {
				MaJiangEvent maJiangEvent = cards[i].getMaJiang().GetComponentInParent<MaJiangEvent>();
				if (!maJiangEvent.isInOriginalPositions) {
					Vector3 position = cards[i].getMaJiang().transform.position;
					cards[i].getMaJiang().transform.position = new Vector3 (position.x, position.y - 0.2f, 0f);
					maJiangEvent.isInOriginalPositions = true;
				}
			}
			/**
			* 此时该list应该是有序的，也一定会是有序的
			*/
			for (int i = 0; i < Main.chiRelateds.Count - 1; i++) {
				if (this.gameObject.Equals (Main.chiRelateds[i].getMaJiang())) {
					int dis = Main.chiRelateds [i].getMaJiangId() - Main.currentMaJiangid;
					if (dis == -2) {
						Main.chiSmall = Main.chiRelateds [i];
						for (int j = i + 1; j < Main.chiRelateds.Count; j ++) {
							if (Main.chiRelateds [j].getMaJiangId() - Main.currentMaJiangid == -1) {
								Main.chiBig = Main.chiRelateds [j];
								break;
							}
						}
					} else if (dis == -1) {
						Main.chiSmall = Main.chiRelateds [i];
						for (int j = i + 1; j < Main.chiRelateds.Count; j ++) {
							if (Main.chiRelateds [j].getMaJiangId() - Main.currentMaJiangid == 1) {
								Main.chiBig = Main.chiRelateds [j];
								break;
							}
						}
						if (Main.chiBig == null) {
							Main.chiBig = Main.chiSmall;
							for (int j = i - 1; j >= 0; j --) {
								if (Main.chiRelateds [j].getMaJiangId() - Main.currentMaJiangid == -2) {
									Main.chiSmall = Main.chiRelateds [j];
									break;
								}
							}
						}
					} else if (dis == 1) {
						Main.chiSmall = Main.chiRelateds [i];
						for (int j = i + 1; j < Main.chiRelateds.Count; j ++) {
							if (Main.chiRelateds [j].getMaJiangId() - Main.currentMaJiangid == 2) {
								Main.chiBig = Main.chiRelateds [j];
								break;
							}
						}
						if (Main.chiBig == null) {
							Main.chiBig = Main.chiSmall;
							for (int j = i - 1; j >= 0; j --) {
								if (Main.chiRelateds [j].getMaJiangId() - Main.currentMaJiangid == -1) {
									Main.chiSmall = Main.chiRelateds [j];
									break;
								}
							}
						}
					} else if (dis == 2) {
						Main.chiSmall = Main.chiRelateds [i];
						for (int j = i + 1; j < Main.chiRelateds.Count; j ++) {
							if (Main.chiRelateds [j].getMaJiangId() - Main.currentMaJiangid == 1) {
								Main.chiBig = Main.chiRelateds [j];
								break;
							}
						}
					}
				}
			}
			// 将这两张牌选中
			Vector3 pos= Main.chiSmall.getMaJiang().transform.position; 
			Main.chiSmall.getMaJiang().transform.position = new Vector3 (pos.x, pos.y + 0.2f, 0f);
			Main.chiSmall.getMaJiang().GetComponentInParent<MaJiangEvent>().isInOriginalPositions = true;
			pos = Main.chiBig.getMaJiang().transform.position;
			Main.chiBig.getMaJiang().transform.position = new Vector3 (pos.x, pos.y + 0.2f, 0f);
			Main.chiBig.getMaJiang().GetComponentInParent<MaJiangEvent>().isInOriginalPositions = true;
		} else {
			List<Card> cards = Main.myHand.getCards ();
			for (int i = cards.Count - 1; i >= 0; i--) {
				Card card = cards [i];
				GameObject aObject = card.getMaJiang ();
				if (this.gameObject.Equals (aObject)) {
					Vector3 position = this.gameObject.transform.position;
					if (this.isInOriginalPositions) {
						this.gameObject.transform.position = new Vector3 (position.x, position.y + 0.2f, 0f);
						this.isInOriginalPositions = false;
					} else {
						if (Main.isMyTurn && !Main.isPlayed) { // 双击出牌
							Main.currentMaJiangid = cards [i].getMaJiangId(); // 将现在的牌置为打出牌
							this.gameObject.SetActive (false); // 使这张牌在画布消失
							// TODO 1
							// 出现打出这张牌的效果（比如打出去的动画+放大显示在前面一秒)
							// 该牌放置在自己的打出牌堆
							cards.RemoveAt (i); // 将这张牌移出手牌
							Main.myHand.reorder (); // 重排手牌
							Timer.GetInstance().newTurn();
							Main.isPlayed = true;
							break;
						} else {
							this.gameObject.transform.position = new Vector3 (position.x, position.y - 0.2f, 0f);
							this.isInOriginalPositions = true;
						}
					} 
				} else {
					MaJiangEvent otherEvent = aObject.GetComponent<MaJiangEvent> ();
					if (!otherEvent.isInOriginalPositions) {
						Vector3 position = aObject.transform.position;
						aObject.transform.position = new Vector3 (position.x, position.y - 0.2f, 0f);
						otherEvent.isInOriginalPositions = true;
					}
				}
			}
		}
	}
}
