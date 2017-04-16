using UnityEngine;
using System.Collections;
using System;

public class BatteryAndTime : MonoBehaviour {

	string _time = string.Empty;
	string _battery = string.Empty;

	void Start()
	{
		StartCoroutine("UpdataBattery");
	}


	void OnGUI()
	{
		GUILayout.Label(_battery, GUILayout.Width(100), GUILayout.Height(100));
	}


	IEnumerator UpdataBattery()
	{
		while (true)
		{
			//此处的battery是一个百分比数字，比如电量是93%，则这个数字是93
			_battery = GetBatteryLevel().ToString();
			print("battery::::" + _battery);
			yield return new WaitForSeconds(300f);
		}
	}


	int GetBatteryLevel()
	{
//		try
//		{
//			string CapacityString = System.IO.File.ReadAllText("/sys/class/power_supply/battery/capacity");
//			return int.Parse(CapacityString);
//		}
//		catch (Exception e)
//		{
//			Debug.Log("Failed to read battery power; " + e.Message);
//		}
		return -1;
	}
}
