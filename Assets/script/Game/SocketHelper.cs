using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;  
using System.Linq;  
using System.Text;  

public class SocketHelper : MonoBehaviour {
	private static SocketHelper socketHelper = new SocketHelper();  
	private Socket socket;

	public static SocketHelper GetInstance() {
		return socketHelper;
	}

	public void update() {
		if (Main.isMyTurn) {
			Debug.Log("This is my turn now!");
			// TODO 3
			// 计时器开始计时
			// 计时器要做到每秒改变一次时间
			if (!Main.isGetACard) { // 还没发牌，则执行发牌
				Debug.Log("正在发牌...");
				string status = "Draw A Card\r\n"; // 请求发牌
				byte[] message = Encoding.ASCII.GetBytes(status);
				socket.Send(message, message.Length, SocketFlags.None);

				byte[] returnMsg = new byte[1024];
				int returnMsgSize = socket.Receive(returnMsg);
				String cardId = Encoding.ASCII.GetString(returnMsg, 0, returnMsgSize);
				Debug.Log(cardId);

				drawACard(int.Parse(cardId));
				Main.isGetACard = true;
				Debug.Log("isGetACard" + Main.isGetACard);
			} else if (Main.isPlayed){ // 玩家打出牌了
				string playCard = "Play Card,40\r\n"; // TODO 4 这里需要将玩家打出的这张牌，放在这个string里面
				byte[] message = Encoding.ASCII.GetBytes(playCard);
				socket.Send(message, message.Length, SocketFlags.None);

				byte[] returnMsg = new byte[1024];
				int returnMsgSize = socket.Receive(returnMsg);
				String returnMsgStr = Encoding.ASCII.GetString(returnMsg, 0, returnMsgSize);
				Debug.Log(returnMsgStr + "!!!!!");
				if (returnMsgStr == "SUCCESSFUL") {
					Main.isMyTurn = false;
					Debug.Log("SUCCESSFUL");
				}
			}
		} else {
			Debug.Log("This is someone's turn.");
			string status = "STATUS\r\n";
			byte[] message = Encoding.ASCII.GetBytes(status);
			socket.Send(message, message.Length, SocketFlags.None);

			byte[] returnMsg = new byte[1024];
			int returnMsgSize = socket.Receive(returnMsg);
			String returnMsgStr = Encoding.ASCII.GetString(returnMsg, 0, returnMsgSize);
			Debug.Log(returnMsgStr);

			if (returnMsgStr == "YOUR TURN\n") {
				Main.isMyTurn = true; 
				Debug.Log("is my turn");
			} else if (returnMsgStr == "EAST") { // 东家回合
				// 摸牌效果（东家）
				// 计时器效果开启（东家）
			} else if (returnMsgStr.Contains("EAST POST")) { // 东家出牌
				Debug.Log("EAST POST");
				// 麻将打出画面（东家）
				if (Main.isActioned) { // 玩家可以做出反应且已经做出反应
					string action = "ACTION," + Main.actionCode + "," + Main.currentMaJiangid + "," + Main.myHand + "\r\n";
					// TODO 8
					// 这里需要设计一下 ，看看怎么验证和交互，draft版本是：ACTION,PENG,MAJIANGID,HAND
					// 这里将玩家动作发送给服务器
					message = Encoding.ASCII.GetBytes(action);
					socket.Send(message, message.Length, SocketFlags.None);

					// 与服务器交互成功以后 
					Main.actionCode = "";
					Main.isActioned = false;
				} else { // 判断能否执行动作
					// TODO 9
					// 这里需要一个计时器，大概给3秒钟的时间，让大家可以比较从容操作（这个计时器是不可见的）
					// 如果这个时钟已经出现，则不执行
					int spliter = returnMsgStr.IndexOf(',');
					String cardIdStr = returnMsgStr.Substring (spliter + 1, returnMsgStr.Length - spliter - 1);
					int cardId = int.Parse(cardIdStr); //这个参数是从returnMsg来的
					Main.userAction = new UserAction(cardId / 4); // 本地逻辑判断是不是可以进行：吃，碰，杠，胡
					Main.userAction.showAction(); // 将可以执行的动作的图标显示出来
				}
			} else if (returnMsgStr.Contains("EAST CHI")) { // 东家在做吃的动作
				// TODO 10
				// 显示吃的动画（东家）
				// 将吃完的牌放到其左手边（东家）
			} else if (returnMsgStr.Contains("...")) { // TODO 11 其他家做动作
			} else { // TODO 11 其他家的逻辑再说
			} 
		}
	}

	private void drawACard(int maJiangId) { 
		GameObject notInstantiateMaJiang = Resources.Load ((maJiangId / 4).ToString ()) as GameObject;  
		notInstantiateMaJiang.transform.position = new Vector3 (3f, -2f, 0f);    
		Main.myHand.getCards().Add(new Card(Instantiate (notInstantiateMaJiang), maJiangId / 4));  
	}

	void initMyPlayer(int[] hand) {
		hand = new int[]{10, 11, 1, 0, 9, 22, 9, 10, 11, 0, 0, 9, 22, 9, 10, 11}; // mock的牌
		List<Card> cards = new List<Card>();
		for (int i = 0; i < hand.Length; i++) {
			GameObject maJiang = Instantiate (Resources.Load (hand[i].ToString()) as GameObject);
			cards.Add(new Card(maJiang, hand[i]));
		}
		Main.myHand.setCards (cards);
		Main.myHand.reorder();
	}

	void initLeftPlayer() {
		List<GameObject> leftPlayerDeck = new List<GameObject> ();
		float startPositionX = -3f;
		float startPositionY = 2f;
		for (int i = 0; i < 16; i++) {
			GameObject maJiang;
			maJiang = Resources.Load ("leftPlayerMaJiang") as GameObject;
			maJiang.transform.position = new Vector3 (startPositionX, startPositionY - i * 0.2f, 0f); 
			leftPlayerDeck.Add (maJiang);
			Instantiate (maJiang);	
		}
	}

	void initRightPlayer() {
		List<GameObject> rightPlayerDeck = new List<GameObject> ();
		float startPositionX = 3f;
		float startPositionY = 2f;
		for (int i = 0; i < 16; i++) {
			GameObject maJiang;
			maJiang = Resources.Load ("rightPlayerMaJiang") as GameObject;
			maJiang.transform.position = new Vector3 (startPositionX, startPositionY - i * 0.2f, 0f); 
			rightPlayerDeck.Add (maJiang);
			Instantiate (maJiang);	
		}
	}

	void initTopPlayer() {
		List<GameObject> topPlayerDeck = new List<GameObject> ();
		float startPositionX = -1.8f;
		float startPositionY = 2.5f;
		for (int i = 0; i < 16; i++) {
			GameObject maJiang;
			maJiang = Resources.Load ("topPlayerMaJiang") as GameObject;
			maJiang.transform.position = new Vector3 (startPositionX + i * 0.29f, startPositionY, 0f); 
			topPlayerDeck.Add (maJiang);
			Instantiate (maJiang);	
		}
	}

	private SocketHelper() {
		//采用TCP方式连接  
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
		//服务器IP地址  
		IPAddress address = IPAddress.Parse("127.0.0.1");  
		//服务器端口  
		IPEndPoint endpoint = new IPEndPoint(address,12000);  
		//异步连接,连接成功调用connectCallback方法  
		IAsyncResult result = socket.BeginConnect(endpoint, new AsyncCallback(ConnectCallback), socket);  
		//这里做一个超时的监测，当连接超过5秒还没成功表示超时  
		bool success = result.AsyncWaitHandle.WaitOne(5000, true);  

		if (!success) { //超时  
			Closed ();  
			Debug.Log ("connection Time Out");  
		} else {
			ReceiveSorket(); // 开启连接
		}
	}

	public void Closed() //关闭Socket  
	{  
		if (socket != null && socket.Connected)  
		{  
			socket.Shutdown(SocketShutdown.Both);  
			socket.Close();  
		}  
		socket = null;  
	}

	private void ConnectCallback(IAsyncResult asyncConnect)  
	{  
		Debug.Log("connect success");  
	}   

	private void ReceiveSorket() { //在这个线程中接受服务器返回的数据  
		Boolean isRoomOpen = false;
		Boolean isInit = false;

		while (true) {
			if (!socket.Connected) { //与服务器断开连接跳出循环 
				Debug.Log("Failed to clientSocket server.");  
				socket.Close();  
				break;  
			} try {
				Debug.Log("Connected.");   
				if (!isRoomOpen) {
					string initStr = "OPEN ROOM\r\n";
					byte[] data = new byte[1024];
					data = Encoding.ASCII.GetBytes(initStr);
					socket.Send(data, data.Length, SocketFlags.None);

					byte[] receive = new byte[1024];
					int receiceDataSize = socket.Receive(receive);
					String receiveData = Encoding.ASCII.GetString(receive, 0, receiceDataSize); //将接收的数据转换成字符串
					Debug.Log(receiveData);  
					isRoomOpen = true;
				} else if(!isInit) {
					string initStr = "Init\r\n";
					byte[] data = Encoding.ASCII.GetBytes(initStr);
					socket.Send(data, data.Length, SocketFlags.None);

					byte[] receive = new byte[1024]; // 获取手牌
					int receiceDataSize = socket.Receive(receive);
					String receiveData = Encoding.ASCII.GetString(receive, 0, receiceDataSize);
					Debug.Log(receiveData); 
					//					Main.jin = int.Parse(receiveData) / 4;
					Main.jin = 1; // 假金，测试用

					receive = new byte[1024];
					receiceDataSize = socket.Receive(receive);
					receiveData = Encoding.ASCII.GetString(receive, 0, receiceDataSize);
					Debug.Log(receiveData); 

					string[] ids = receiveData.Split(',');  
					// 这里的话，需不需要把金提出来，放在手牌最左边？还是说给它一个框？还是说在左上角或者哪里有个提示框，说金是这个。
					initMyPlayer (Array.ConvertAll<string, int>(ids, s => int.Parse(s)));
					initLeftPlayer ();
					initRightPlayer ();
					initTopPlayer ();

					isInit = true;
					Main.isMyTurn = false;
					break;
				}
			} catch (Exception e) {
				Debug.Log("Failed to clientSocket error." + e);  
				socket.Close();  
				break;  
			}
		} 
	}
}