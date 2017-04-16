using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuEvent : MonoBehaviour {

	void Start () {}
	void Update () {}

	void OnMouseDown() { 
		Debug.Log("Start HuEvent...");
		// TODO 12
		// 隐藏胡的图标
		// 读秒隐藏
		// 出现胡的效果
		// 计算盘数
		// 显示胜利结果
		Main.actionCode = "HU";
		Main.isActioned = true;
		Debug.Log("End HuEvent.");
	}
}