using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createRoomMain : MonoBehaviour {
	public Texture buttonTexture;
	private int repeatTime = 0;
	private string info = "";

	void Start () {}
	void Update () {}

	void OnGUI()
	{
//		// 文本显示
//		GUI.Label (new Rect (50, 200, 200, 50), info);
//
//
//		// 第一个文字按钮
//		GUI.color = Color.yellow;  //按钮文字颜色  
//		GUI.backgroundColor = Color.red; //按钮背景颜色
//
//		if(GUI.Button(new Rect(50,250,200,30), "Button1"))  
//		{// 文本显示
//			GUI.Label (new Rect (50, 200, 200, 50), info);
//
//
//			// 第一个文字按钮
//			GUI.color = Color.yellow;  //按钮文字颜色  
//			GUI.backgroundColor = Color.red; //按钮背景颜色
//
//			if(GUI.Button(new Rect(50,250,200,30), "Button1"))  
//			{
//			info = "按下了Button1"; 
//		}
//
//		// 第二个图片按钮
//		GUI.color = Color.white;  //按钮文字颜色  
//		GUI.backgroundColor = Color.green; //按钮背景颜色
//
//		if(GUI.Button(new Rect(50,300,128,64), buttonTexture))  
//		{
//			info = "按下了Button2"; 
//		}
//
//		// 持续按下的按钮
//		if(GUI.RepeatButton(new Rect(50,400,200,30),"按钮按下中"))
//		{
//			info = "按钮按下中的时间："+ repeatTime;
//			repeatTime++;  
//		}  
	}
}
