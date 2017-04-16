using System;
using System.Collections.Generic;
using UnityEngine;

// TODO 6 有一个问题需要商榷，要统一每张牌传过来的id,应该在0~35之间
public class UserAction : MonoBehaviour { 
	private Boolean[] can = new Boolean[4];
	private Boolean used = false;
	private int[] jin = new int[4];

	private int startPosition; 

	public UserAction(int cardId) {
		Main.actions = new List<GameObject> ();
		if (isChiable(cardId)) { Main.actions.Add (Resources.Load ("chi") as GameObject); }
		if (isPengable(cardId)) { Main.actions.Add (Resources.Load ("peng") as GameObject); } 
		if (isGangable(cardId)) { Main.actions.Add (Resources.Load ("gang") as GameObject); }
		if (isHuable(cardId)) { Main.actions.Add (Resources.Load ("hu") as GameObject); }
	}

	public void showAction() {
		float logoSize = 0.88f; // TODO - POSTPONE - 这个地方需要获取动作图片的长和宽,需要去UI联调 
		float startPositionX = 2f - logoSize / 2 * Main.actions.Count; // 暂定一个action的图标宽度是88＊88
		float startPositionY = -1f;
		for (int i = 0; i < Main.actions.Count; i++) {
			Main.actions[i].transform.position = new Vector3 (startPositionX - logoSize * i, startPositionY, 0f);
			Main.actions[i] = Instantiate (Main.actions[i]);
		}
	}

	private Boolean isPengable(int cardId) {
		if (getCardNumber(cardId) >= 2) return true;
		return false;
	}

	private Boolean isGangable(int cardId) {
		if (getCardNumber(cardId) == 3) return true;
		return false;
	}

	private int getCardNumber(int cardId) {
		int count = 0;
		List<Card> cards = Main.myHand.getCards();
		for (int i = 0; i < cards.Count; i ++) {
			if (cards[i].getMaJiangId() == cardId) count ++;
		}
		return count;
	}

	private Boolean isChiable(int cardId) {
		List<Card> tempList = new List<Card> ();
		int cardGroup = cardId / 9;
		if (cardGroup == Card.FENG) return false;
		int cardNumber = cardId % 9;
		Boolean[] isExist = new Boolean[9];
		List<Card> cards = Main.myHand.getCards ();
		for (int i = cards.Count - 1; i >= 0; i--) {
			int otherCardId = cards [i].getMaJiangId ();
			int otherGroup = otherCardId / 9;
			if (cardGroup == otherGroup) {
				isExist [otherCardId % 9] = true;
				tempList.Add (cards [i]);
			}
		}

		for (int i = tempList.Count - 1; i >= 0; i--) {
			int number = (tempList [i].getMaJiangId ()) % 9;
			if (number < cardNumber) {
				if (number == cardNumber - 2 && number >= 0 && isExist [cardNumber - 1])
					return true;
				if (number == cardNumber - 1 && number >= 0 && cardNumber + 1 <= 8 && isExist [cardNumber + 1])
					return true;
				if (number == cardNumber - 1 && number >= 0 && cardNumber - 2 >= 0 && isExist [cardNumber - 2])
					return true;
			} else if (cardNumber > number) {
				if (number == cardNumber + 2 && number <= 8 && isExist [cardNumber + 1])
					return true;
				if (number == cardNumber + 1 && number <= 8 && cardNumber + 2 <= 8 && isExist [cardNumber + 2])
					return true;
				if (number == cardNumber + 1 && number <= 8 && cardNumber - 1 >= 0 && isExist [cardNumber - 1])
					return true;
			}
		}
		return false;
	}

	private Boolean isHuable(int cardId) {
		List<Card> cards = Main.myHand.getCards();
		int[][] paixing = new int[4][];
		for (int i = 0; i < 4; i++) {
			paixing [i] = new int[11];
		}
		for (int i = 0; i < cards.Count; i++) {
			Card card = cards [i];
			int id = card.getMaJiangId(); 

			paixing[id / 9][id % 9] ++;
		}
		paixing [cardId / 9] [cardId % 9]++;
		Debug.Log("!!!" + Main.jin );
		int jinCount = paixing [Main.jin / 9] [Main.jin % 9];
		paixing [Main.jin / 9] [Main.jin % 9] = 0;

		return allocateJin(jinCount, paixing);
	}

	private Boolean allocateJin(int number, int[][] paixing) {
		for (int i1 = 0; i1 <= number; i1 ++) {
			for (int i2 = 0; i2 <= number - i1; i2 ++) {
				for (int i3 = 0; i3 <= number - i1 - i2; i3 ++) {
					jin[0] = i1; jin[1] = i2; jin[2] = i3; jin[3] = number - i1 - i2 - i3;
					if (allocateCouple(paixing)) return true;
				}
			}
		}
		return false;
	}

	private Boolean allocateCouple(int[][] paixing) {
		for (int i = 0; i < 4; i ++) {
			for (int j = 0; j < 4; j ++) 
				if (j == i) {
					can[j] = true;
				} else can[j] = false;
			used = false;
			if (getAns(paixing)) return true;
		}
		return false;
	}

	private Boolean getAns(int[][] paixing) {
		Boolean ans = true;
		for (int j = 0; j < 4; j ++) {
			ans = ans && Process(paixing[j], j, 0, false, 0);
		}
		return ans && used;
	}

	private Boolean Process(int[] paixing, int id, int p, Boolean isCoupled, int needJin) {
		if (needJin > jin[id]) return false;
		if (p == 9) {
			if (needJin != jin[id]) return false;
			if (paixing[p] == 0 && paixing[p + 1] == 0) {
				if (isCoupled) used = true;
				return true;
			}
			return false;
		}
		if (paixing [p] < 0) {
			needJin -= paixing [p];
			if (needJin > jin[id]) {
				return false;
			} else {
				return Process (paixing, id, p, isCoupled, needJin);
			}
		}
		if (paixing [p] == 0) {
			Boolean ans = Process (paixing, id, p + 1, isCoupled, needJin);
			if (ans) return ans;
			paixing [p] -= 1; // 尝试顺子
			paixing [p + 1] -= 1;
			paixing [p + 2] -= 1;
			ans = Process (paixing, id, p + 1, isCoupled, needJin + 1);
			paixing [p] += 1; // 回溯
			paixing [p + 1] += 1;
			paixing [p + 2] += 1;
			if (ans) return true;
			if (!isCoupled && can[id]) {
				paixing [p] -= 2; // 尝试对子
				ans = Process (paixing, id, p + 1, true, needJin + 2);
				paixing [p] += 2; // 回溯
			}
			if (ans) return true;
			paixing [p] -= 3; // 尝试三个
			ans = Process (paixing, id, p + 1, isCoupled, needJin + 3);
			paixing [p] += 3; // 回溯
			return ans;
		}
		if (paixing [p] == 1) {
			paixing [p] -= 1; // 尝试顺子
			paixing [p + 1] -= 1;
			paixing [p + 2] -= 1;
			Boolean ans = Process (paixing, id, p + 1, isCoupled, needJin);
			paixing [p] += 1; // 回溯
			paixing [p + 1] += 1;
			paixing [p + 2] += 1;
			if (ans) return true;
			if (!isCoupled && can[id]) {
				paixing [p] -= 2; // 尝试对子
				ans = Process (paixing, id, p + 1, true, needJin + 1);
				paixing [p] += 2; // 回溯
			}
			if (ans) return true;
			paixing [p] -= 3; // 尝试三个
			ans = Process (paixing, id, p + 1, isCoupled, needJin + 2);
			paixing [p] += 3; // 回溯
			if (ans) return true;
			paixing [p] -= 1; // 尝试顺子
			paixing [p + 1] -= 1;
			paixing [p + 2] -= 1;
			ans = Process (paixing, id, p, isCoupled, needJin);
			paixing [p] += 1; // 回溯
			paixing [p + 1] += 1;
			paixing [p + 2] += 1;
			return ans;
		} else if (paixing [p] == 2) {
			paixing [p] -= 2; // 尝试顺子
			paixing [p + 1] -= 2;
			paixing [p + 2] -= 2;
			Boolean ans = Process (paixing, id, p + 1, isCoupled, needJin);
			paixing [p] += 2; // 回溯
			paixing [p + 1] += 2;
			paixing [p + 2] += 2;
			if (ans) return true;
			if (!isCoupled && can[id]) {
				paixing [p] -= 2; // 尝试对子
				ans = Process (paixing, id, p + 1, true, needJin);
				paixing [p] += 2; // 回溯
			}
			if (ans) return true;
			paixing [p] -= 3; // 尝试三个
			ans = Process (paixing, id, p + 1, isCoupled, needJin + 1);
			paixing [p] += 3; // 回溯
			if (ans) return true;
			paixing [p] -= 1; // 尝试顺子
			paixing [p + 1] -= 1;
			paixing [p + 2] -= 1;
			ans = Process (paixing, id, p, isCoupled, needJin);
			paixing [p] += 1; // 回溯
			paixing [p + 1] += 1;
			paixing [p + 2] += 1;
			return ans;
		} else if (paixing [p] == 3) {
			paixing [p] -= 3; // 尝试三个
			Boolean ans = Process (paixing, id, p + 1, isCoupled, needJin);
			paixing [p] += 3; // 回溯
			if (ans) return true;
			paixing [p] -= 1; // 尝试顺子
			paixing [p + 1] -= 1;
			paixing [p + 2] -= 1;
			ans = Process (paixing, id, p, isCoupled, needJin);
			paixing [p] += 1; // 回溯
			paixing [p + 1] += 1;
			paixing [p + 2] += 1;
			return ans;
		} else if (paixing [p] == 4) {
			paixing [p] -= 1; // 尝试顺子
			paixing [p + 1] -= 1;
			paixing [p + 2] -= 1;
			Boolean ans = Process (paixing, id, p, isCoupled, needJin);
			paixing [p] += 1; // 回溯
			paixing [p + 1] += 1;
			paixing [p + 2] += 1;
			return ans;
		}
		return false;
	}
}