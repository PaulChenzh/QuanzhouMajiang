using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChiEvent : MonoBehaviour {

	public Boolean isInOriginalPositions = true;

	void Start () {}
	void Update () {}

	void OnMouseDown() {
		Debug.Log("Start ChiEvent...");
		Main.chiRelateds = getUnrelatedMaJiang(Main.myHand.getCards(), Main.currentMaJiangid);
		if (Main.isChing) {
			foreach (Card card in Main.chiRelateds) {
				MaJiangEvent maJiangEvent = card.getMaJiang().GetComponentInParent<MaJiangEvent>();
				if( maJiangEvent != null ) {
//					maJiangEvent.; // 开启事件
				}
			}
			Main.isChing = false;
			// TODO 1
			// 去掉吃的亮边
			if (Main.chiSmall != null && Main.chiBig != null) { // 已经有选中的牌，进行吃操作
				for (int i = Main.actions.Count - 1; i >= 0; i ++) { // 将Action图标隐藏
					Main.actions[i].SetActive (false);
				}
				Main.actions = new List<GameObject> (); // 重置所有Action
				Main.chiSmall.getMaJiang().SetActive (false); // 使这张牌在画布消失
				Main.chiBig.getMaJiang().SetActive (false); // 使这张牌在画布消失
				List<Card> cards = Main.myHand.getCards();
				for (int i = cards.Count - 1; i >= 0; i ++) { // 将选中的牌从手牌移出
					if (cards[i].Equals(Main.chiBig)) {
						cards.RemoveAt(i);
					}
					if (cards[i].Equals(Main.chiSmall)) {
						cards.RemoveAt(i);
					}
				}
				// TODO 2 执行吃的动画效果
				// 中间显示吃+吃完的顺
				// 将该吃放在手牌左边
				Main.myHand.reorder (); // 重排手牌

				Main.chiSmall = null;
				Main.chiBig = null;
				Main.actionCode = "CHI";
				Main.isActioned = true;
			}
		} else {
			foreach (Card card in Main.chiRelateds) {
				MaJiangEvent maJiangEvent = card.getMaJiang().GetComponentInParent<MaJiangEvent>();
				if( maJiangEvent != null ) {
					maJiangEvent.StopAllCoroutines(); // 将与该次“吃”动作无关的麻将，禁止他们的事件
				}
			}
			// TODO 3
			// 给“吃”加个亮边
			Main.isChing = true;
		}

		Debug.Log("End ChiEvent.");
	}

	// 获得与“吃”无关的牌
	private List<Card> getUnrelatedMaJiang(List<Card> maJiangs, int currentMaJiang) {
		List<Card> tempList = new List<Card>();

		int cardId = currentMaJiang / 4;
		int cardGroup = cardId / 9;
		int cardNumber = cardId % 9;
		Boolean[] isExist = new Boolean[9];

		for (int i = maJiangs.Count - 1; i >= 0; i --) {
			int otherCardId = maJiangs[i].getMaJiangId() / 4;
			int otherGroup = otherCardId / 9;
			if (cardGroup == otherGroup) {
				isExist[cardNumber] = true;
				tempList.Add(maJiangs[i]);
			}
		}

		for (int i = tempList.Count - 1; i >= 0; i --) {
			int number = (tempList[i].getMaJiangId() / 4) % 9;
			if (number < cardNumber) {
				if (number == cardNumber - 2 && number >= 0 && isExist[cardNumber - 1]) {
					tempList.RemoveAt(i); // 将该卡牌移除
				}
				if (number == cardNumber - 1 && number >= 0 && cardNumber + 1 <= 8 && isExist[cardNumber + 1]) {
					tempList.RemoveAt(i);
				}
				if (number == cardNumber - 1 && number >= 0 && cardNumber - 2 >= 0 && isExist[cardNumber - 2]) {
					tempList.RemoveAt(i);
				}
			} else if (cardNumber > number) {
				if (number == cardNumber + 2 && number <= 8 && isExist[cardNumber + 1]) {
					tempList.RemoveAt(i);
				}
				if (number == cardNumber + 1 && number <= 8 && cardNumber + 2 <= 8 && isExist[cardNumber + 2]) {
					tempList.RemoveAt(i);
				}
				if (number == cardNumber + 1 && number <= 8 && cardNumber - 1 >= 0 && isExist[cardNumber - 1]) {
					tempList.RemoveAt(i);
				}
			}
		}

		return tempList;
	}
}