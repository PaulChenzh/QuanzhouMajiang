using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;  
using System.Linq;  
using System.Text;  

public class SocketManager : MonoBehaviour 
{
	private static SocketManager socketManager = new SocketManager();  
	//采用TCP方式连接
	private static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
	//服务器IP地址  
	private static IPAddress address = IPAddress.Parse("127.0.0.1");  
	//服务器端口  
	private static IPEndPoint endpoint = new IPEndPoint(address,12000);  
	//异步连接,连接成功调用connectCallback方法  
	private static IAsyncResult result = socket.BeginConnect (endpoint, new AsyncCallback (ConnectCallback), socket); 

	public static int roomId;
	private static int positionForMe;
	private static int nowPlayerInLocal;
	private static List<GameObject> leftPlayerDeck = new List<GameObject> ();
	private static List<GameObject> rightPlayerDeck = new List<GameObject> ();
	private static List<GameObject> topPlayerDeck = new List<GameObject> ();
	private static int leftCardNumber = 16;
	private static int rightCardNumber = 16;
	private static int topCardNumber = 16;

	public SocketManager () {}

	public static SocketManager GetInstance() {
		return socketManager;
	}

	private void initMyPlayer(int[] hand) {
//		hand = new int[]{10, 11, 1, 0, 9, 22, 9, 10, 11, 0, 0, 9, 22, 9, 10, 11}; // mock的牌
//				hand = new int[]{65,23,78,56,95,71,80,79,118,41,45,70,46,42,53,3}; // mock的牌
		List<Card> cards = new List<Card>();
		for (int i = 0; i < hand.Length; i++) {
			GameObject maJiang = Instantiate (Resources.Load ((hand[i] / 4).ToString()) as GameObject);
			cards.Add(new Card(maJiang, hand[i]));
		}
		Main.myHand.setCards (cards);
		Main.myHand.reorder();
	}

	private void showLeftPlayer() {
		float startPositionX = -4.5f;
		float startPositionY = 2.5f - (16 - leftCardNumber) * 0.1f;
		for (int i = 0; i < leftCardNumber; i++) {
			GameObject maJiang;
			maJiang = Resources.Load ("leftPlayerMaJiang") as GameObject;
			maJiang.transform.position = new Vector3 (startPositionX, startPositionY - i * 0.2f, 0f); 
			leftPlayerDeck.Add (maJiang);
			maJiang = Instantiate (maJiang);	
		}
	}

	private void showRightPlayer() {
		float startPositionX = 4.5f;
		float startPositionY = 2.5f - (16 - rightCardNumber) * 0.1f;
		for (int i = 0; i < rightCardNumber; i++) {
			GameObject maJiang;
			maJiang = Resources.Load ("rightPlayerMaJiang") as GameObject;
			maJiang.transform.position = new Vector3 (startPositionX, startPositionY - i * 0.2f, 0f); 
			rightPlayerDeck.Add (maJiang);
			maJiang = Instantiate (maJiang);	
		}
	}

	private void showTopPlayer() {
		float startPositionX = -2.5f + (16 - topCardNumber) * 0.145f;
		float startPositionY = 3.75f;
		for (int i = 0; i < topCardNumber; i++) {
			GameObject maJiang;
			maJiang = Resources.Load ("topPlayerMaJiang") as GameObject;
			maJiang.transform.position = new Vector3 (startPositionX + i * 0.29f, startPositionY, 0f); 
			topPlayerDeck.Add (maJiang);
			maJiang = Instantiate (maJiang);	
		}
	}

	public void initGame() {
		Boolean isInit = false;

		while (true) {
			if (!socket.Connected) { //与服务器断开连接跳出循环 
				Debug.Log("Failed to clientSocket server.");  
				socket.Close();  
				break;  
			} try {
				Debug.Log("Initing the game...");   
				if(!isInit) {
					string initStr = "Init\r\n";
					byte[] data = Encoding.ASCII.GetBytes(initStr);
					socket.Send(data, data.Length, SocketFlags.None);

					byte[] receive = new byte[1024]; // 获得金
					int receiceDataSize = socket.Receive(receive);
					String receiveData = Encoding.ASCII.GetString(receive, 0, receiceDataSize);
					Debug.Log("Jin is " + receiveData + "."); 
					Main.jin = 1; // 假金，测试用

					receive = new byte[1024]; // 获取手牌
					receiceDataSize = socket.Receive(receive);
					receiveData = Encoding.ASCII.GetString(receive, 0, receiceDataSize);
					Debug.Log("My hands are : " + receiveData + "."); 

					string[] ids = receiveData.Split(',');  
					// 这里的话，需不需要把金提出来，放在手牌最左边？还是说给它一个框？还是说在左上角或者哪里有个提示框，说金是这个。
					initMyPlayer (Array.ConvertAll<string, int>(ids, s => int.Parse(s)));
					showLeftPlayer ();
					showRightPlayer ();
					showTopPlayer ();


					initStr = "FIRST PLAYER\r\n";
					data = Encoding.ASCII.GetBytes(initStr);
					socket.Send(data, data.Length, SocketFlags.None);

					receive = new byte[1024];
					receiceDataSize = socket.Receive(receive);
					receiveData = Encoding.ASCII.GetString(receive, 0, receiceDataSize);
					Debug.Log("The first player's position is " + receiveData + "."); 
					nowPlayerInLocal= int.Parse(receiveData);
					Timer.GetInstance().init(nowPlayerInLocal, positionForMe);

					isInit = true;
					break;
				}
			} catch (Exception e) {
				Debug.Log("Failed to clientSocket error." + e);  
				socket.Close();  
				break;  
			}
		} 
	}

	public int joinRoom(int roomNumber) {
		Boolean isRoomExist = false;
		while (!isRoomExist) {
			if (!socket.Connected) { //与服务器断开连接跳出循环 
				socket.BeginConnect (endpoint, new AsyncCallback (ConnectCallback), socket);  
				//这里做一个超时的监测，当连接超过5秒还没成功表示超时  
				bool success = result.AsyncWaitHandle.WaitOne(5000, true);
				if (!success) { //超时  
					Closed ();  
					Debug.Log ("connection Time Out");  
				}
			} 
			try {
				Debug.Log("Player " + positionForMe + " is joining...");   
				if (!isRoomExist) {
					string initStr = "JOIN ROOM\r\n";
					byte[] data = new byte[1024];
					data = Encoding.ASCII.GetBytes(initStr);
					socket.Send(data, data.Length, SocketFlags.None);

					byte[] receive = new byte[1024];
					int receiceDataSize = socket.Receive(receive);
					String receiveData = Encoding.ASCII.GetString(receive, 0, receiceDataSize); //将接收的数据转换成字符串
					Debug.Log(receiveData);

					data = new byte[1024];
					data = Encoding.ASCII.GetBytes(roomNumber.ToString() + "\r\n");
					socket.Send(data, data.Length, SocketFlags.None);

					receive = new byte[1024];
					receiceDataSize = socket.Receive(receive);
					receiveData = Encoding.ASCII.GetString(receive, 0, receiceDataSize); //将接收的数据转换成字符串
					Debug.Log(receiveData);
					if (receiveData.Equals("The room is full!\n")) {
						return 2;
					} else {
						roomId = roomNumber;
						return 1;
					}
				} 
			} catch (Exception e) {
				Debug.Log("Failed to clientSocket error." + e);  
				socket.Close();  
				break;  
			}
		} 
		return 0;
	}

	public Boolean openRoom() {
		Boolean isRoomOpen = false;
		while (!isRoomOpen) {
			Debug.Log ("####"); 
			if (!socket.Connected) { //与服务器断开连接跳出循环 
				Debug.Log ("????");  
				socket.BeginConnect (endpoint, new AsyncCallback (ConnectCallback), socket);  
				Debug.Log ("!!!!");  
				//这里做一个超时的监测，当连接超过5秒还没成功表示超时  
				bool success = result.AsyncWaitHandle.WaitOne(5000, true);
				if (!success) { //超时  
					Closed ();  
					Debug.Log ("connection Time Out");  
				}
			} 
			try {
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
					roomId = int.Parse(receiveData);
					isRoomOpen = true;
					return true;
				} 
			} catch (Exception e) {
				Debug.Log("Failed to clientSocket error." + e);  
				socket.Close();  
				break;  
			}
		} 
		return false;
	}

	public String getPlayerStatus() {
		if (!socket.Connected) { //与服务器断开连接跳出循环 
			socket.BeginConnect (endpoint, new AsyncCallback (ConnectCallback), socket);  
			//这里做一个超时的监测，当连接超过5秒还没成功表示超时  
			bool success = result.AsyncWaitHandle.WaitOne (5000, true);
			if (!success) { //超时  
				Closed ();  
				Debug.Log ("connection Time Out");  
			}
		}
		try {
			Debug.Log ("Is getting players' status..."); 
			string initStr = "PLAYER STATUS\r\n";
			byte[] data = new byte[1024];
			data = Encoding.ASCII.GetBytes (initStr);
			socket.Send (data, data.Length, SocketFlags.None);

			byte[] receive = new byte[1024];
			int receiceDataSize = socket.Receive (receive);
			String receiveData = Encoding.ASCII.GetString (receive, 0, receiceDataSize); //将接收的数据转换成字符串
			Debug.Log ("Players' status is " + receiveData + "."); 
			return receiveData;
		} catch (Exception e) {
			Debug.Log ("Failed to clientSocket error." + e);  
			socket.Close ();  
		}
		return "";
	}

	public int getPositionNumber() {
		if (!socket.Connected) { //与服务器断开连接跳出循环 
			socket.BeginConnect (endpoint, new AsyncCallback (ConnectCallback), socket);  
			//这里做一个超时的监测，当连接超过5秒还没成功表示超时  
			bool success = result.AsyncWaitHandle.WaitOne(5000, true);
			if (!success) { //超时  
				Closed ();  
				Debug.Log ("connection Time Out");  
			}
		}
		try {
			Debug.Log("Player is getting his position..."); 
			string initStr = "POSITION NUMBER\r\n";
			byte[] data = new byte[1024];
			data = Encoding.ASCII.GetBytes(initStr);
			socket.Send(data, data.Length, SocketFlags.None);

			byte[] receive = new byte[1024];
			int receiceDataSize = socket.Receive(receive);
			String receiveData = Encoding.ASCII.GetString(receive, 0, receiceDataSize); //将接收的数据转换成字符串
			Debug.Log("My position is " + receiveData + "."); 
			positionForMe = int.Parse(receiveData);
			return positionForMe;
		} catch (Exception e) {
			Debug.Log("Failed to clientSocket error." + e);  
			socket.Close();  
		}
		return -1;
	}

	public void handIsUp() {
		while (true) {
			if (!socket.Connected) { //与服务器断开连接跳出循环 
				socket.BeginConnect (endpoint, new AsyncCallback (ConnectCallback), socket);  
				//这里做一个超时的监测，当连接超过5秒还没成功表示超时  
				bool success = result.AsyncWaitHandle.WaitOne(5000, true);
				if (!success) { //超时  
					Closed ();  
					Debug.Log ("connection Time Out");  
				}
			} 
			try {
				Debug.Log("Is handing up...");   
				string initStr = "HAND IS UP\r\n";
				byte[] data = new byte[1024];
				data = Encoding.ASCII.GetBytes(initStr);
				socket.Send(data, data.Length, SocketFlags.None);

				byte[] receive = new byte[1024];
				int receiceDataSize = socket.Receive(receive);
				String receiveData = Encoding.ASCII.GetString(receive, 0, receiceDataSize); //将接收的数据转换成字符串
				Debug.Log(receiveData);
				break;
			} catch (Exception e) {
				Debug.Log("Failed to clientSocket error." + e);  
				socket.Close();  
				break;  
			}
		} 
	}

	private void Closed() //关闭Socket  
	{  
		if (socket != null && socket.Connected)  
		{  
			socket.Shutdown(SocketShutdown.Both);  
			socket.Close();  
		}  
		socket = null;  
	}

	private static void ConnectCallback(IAsyncResult asyncConnect)  
	{  
		Debug.Log("connect success");  
	}

	private void drawACard(int maJiangId) {
		GameObject notInstantiateMaJiang = Resources.Load ((maJiangId / 4).ToString ()) as GameObject;  
		notInstantiateMaJiang.transform.position = new Vector3 (3.7f, -3f, 0f);    
		Main.myHand.getCards().Add(new Card(Instantiate (notInstantiateMaJiang), maJiangId));  
	}
		
	private int getNowPlayer() {
		while (true) {
			if (!socket.Connected) { //与服务器断开连接跳出循环 
				Debug.Log("Failed to clientSocket server.");  
				socket.Close();  
				break;  
			} 
			try {
				Debug.Log ("Getting the player who is playing..."); 
				string initStr = "NOW PLAYER\r\n";
				byte[] data = new byte[1024];
				data = Encoding.ASCII.GetBytes (initStr);
				socket.Send (data, data.Length, SocketFlags.None);

				byte[] receive = new byte[1024];
				int receiceDataSize = socket.Receive (receive);
				String receiveData = Encoding.ASCII.GetString (receive, 0, receiceDataSize); //将接收的数据转换成字符串
				Debug.Log ("The player " + receiveData + " is playing.");
				return int.Parse (receiveData);
			} catch (Exception e) {
				Debug.Log ("Failed to clientSocket error." + e);  
				socket.Close ();  
			}
		}
		return -1;
	}

	public void update() {
		int nowPlayerInServer = getNowPlayer ();
		if (nowPlayerInServer == positionForMe) {
			if (nowPlayerInServer != nowPlayerInLocal) { // 如果本地与服务器不一致，则调整本地
				Timer.GetInstance ().newTurn ((nowPlayerInServer - nowPlayerInLocal) % 4);
			}
			Main.isMyTurn = true;
			Debug.Log("This is my turn now!");
			nowPlayerInLocal = positionForMe;
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
				string playCard = "Play Card\r\n"; 
				byte[] message = Encoding.ASCII.GetBytes(playCard);
				socket.Send(message, message.Length, SocketFlags.None);

				byte[] returnMsg = new byte[1024];
				int returnMsgSize = socket.Receive(returnMsg);
				String returnMsgStr = Encoding.ASCII.GetString(returnMsg, 0, returnMsgSize);
				Debug.Log(returnMsgStr);

				playCard = "" + Main.currentMaJiangid + "\r\n";
				message = Encoding.ASCII.GetBytes(playCard);
				socket.Send(message, message.Length, SocketFlags.None);

				returnMsg = new byte[1024];
				returnMsgSize = socket.Receive(returnMsg);
				returnMsgStr = Encoding.ASCII.GetString(returnMsg, 0, returnMsgSize);
				Debug.Log(returnMsgStr);

				if (returnMsgStr == "SUCCESSFUL") {
					nowPlayerInLocal = (positionForMe + 1) % 4;
					Main.isMyTurn = false;
					Debug.Log("SUCCESSFUL");
				}
			}
		} else {
//			nowPlayerInLocal = nowPlayerInServer;
			Debug.Log("This is someone's turn.");
			string status = "STATUS\r\n";
			byte[] message = Encoding.ASCII.GetBytes(status);
			socket.Send(message, message.Length, SocketFlags.None);

			byte[] returnMsg = new byte[1024];
			int returnMsgSize = socket.Receive(returnMsg);
			String returnMsgStr = Encoding.ASCII.GetString(returnMsg, 0, returnMsgSize);
			Debug.Log(returnMsgStr);

			status = "" + nowPlayerInLocal + "\r\n"; // 告诉服务器当前玩家是哪个
			message = Encoding.ASCII.GetBytes(status);
			socket.Send(message, message.Length, SocketFlags.None);

			returnMsg = new byte[1024]; // 本地与服务器的当前玩家是否一致，－1为一致，0-3为服务器当前玩家
			returnMsgSize = socket.Receive(returnMsg);
			returnMsgStr = Encoding.ASCII.GetString(returnMsg, 0, returnMsgSize);
			if (int.Parse (returnMsgStr) == -1) { //本地与服务器一致，则该玩家没有打牌。
				Debug.Log ("The player " + nowPlayerInServer + " is thinking...");
			} else { // 若不一致，则更新本地
				returnMsg = new byte[1024]; // 获得当前玩家打出的牌
				returnMsgSize = socket.Receive (returnMsg);
				returnMsgStr = Encoding.ASCII.GetString (returnMsg, 0, returnMsgSize);

				int thePlayedCard = int.Parse (returnMsgStr);
				if (thePlayedCard == -1) {

					return;
				} else { // 当前玩家打出了牌
					// 打出的牌的动画
					// 将打出的牌放牌堆
					int k = (nowPlayerInServer + 4 - positionForMe) % 4;
					if (k == 1) {
						foreach (GameObject obj in leftPlayerDeck) {
							Destroy (obj);
						}
						leftPlayerDeck = new List<GameObject> ();
						leftCardNumber--;
						showLeftPlayer ();
					} else if (k == 3) {
						foreach (GameObject obj in rightPlayerDeck) {
							Destroy (obj);
						}
						rightPlayerDeck = new List<GameObject> ();
						rightCardNumber--;
						showRightPlayer ();
					} else if (k == 2) {
						foreach (GameObject obj in topPlayerDeck) {
							Destroy (obj);
						}
						topPlayerDeck = new List<GameObject> ();
						topCardNumber--;
						showTopPlayer ();
					}
					nowPlayerInLocal = (nowPlayerInServer + 1) % 4;
				}
			}

//			if (Main.isActioned) { // 玩家可以做出反应且已经做出反应
//				string action = "ACTION," + Main.actionCode + "," + Main.currentMaJiangid + "," + Main.myHand + "\r\n";
//				// TODO 8
//				// 这里需要设计一下 ，看看怎么验证和交互，draft版本是：ACTION,PENG,MAJIANGID,HAND
//				// 这里将玩家动作发送给服务器
//				message = Encoding.ASCII.GetBytes(action);
//				socket.Send(message, message.Length, SocketFlags.None);
//
//				// 与服务器交互成功以后 
//				Main.actionCode = "";
//				Main.isActioned = false;
//			} else { // 判断能否执行动作
//				// TODO 9
//				// 这里需要一个计时器，大概给3秒钟的时间，让大家可以比较从容操作（这个计时器是不可见的）
//				// 如果这个时钟已经出现，则不执行
//				int spliter = returnMsgStr.IndexOf(',');
//				String cardIdStr = returnMsgStr.Substring (spliter + 1, returnMsgStr.Length - spliter - 1);
//				int cardId = int.Parse(cardIdStr); //这个参数是从returnMsg来的
//				Main.userAction = new UserAction(cardId / 4); // 本地逻辑判断是不是可以进行：吃，碰，杠，胡
//				Main.userAction.showAction(); // 将可以执行的动作的图标显示出来
//			}

//			if (returnMsgStr == "YOUR TURN\n") {
//				Main.isMyTurn = true; 
//				Debug.Log("is my turn");
//			} else if (returnMsgStr == "EAST") { // 东家回合
//				// 摸牌效果（东家）
//				// 计时器效果开启（东家）
//			} else if (returnMsgStr.Contains("EAST POST")) { // 东家出牌
//				Debug.Log("EAST POST");
//				// 麻将打出画面（东家）
//				if (Main.isActioned) { // 玩家可以做出反应且已经做出反应
//					string action = "ACTION," + Main.actionCode + "," + Main.currentMaJiangid + "," + Main.myHand + "\r\n";
//					// TODO 8
//					// 这里需要设计一下 ，看看怎么验证和交互，draft版本是：ACTION,PENG,MAJIANGID,HAND
//					// 这里将玩家动作发送给服务器
//					message = Encoding.ASCII.GetBytes(action);
//					socket.Send(message, message.Length, SocketFlags.None);
//
//					// 与服务器交互成功以后 
//					Main.actionCode = "";
//					Main.isActioned = false;
//				} else { // 判断能否执行动作
//					// TODO 9
//					// 这里需要一个计时器，大概给3秒钟的时间，让大家可以比较从容操作（这个计时器是不可见的）
//					// 如果这个时钟已经出现，则不执行
//					int spliter = returnMsgStr.IndexOf(',');
//					String cardIdStr = returnMsgStr.Substring (spliter + 1, returnMsgStr.Length - spliter - 1);
//					int cardId = int.Parse(cardIdStr); //这个参数是从returnMsg来的
//					Main.userAction = new UserAction(cardId / 4); // 本地逻辑判断是不是可以进行：吃，碰，杠，胡
//					Main.userAction.showAction(); // 将可以执行的动作的图标显示出来
//				}
//			} else if (returnMsgStr.Contains("EAST CHI")) { // 东家在做吃的动作
//				// TODO 10
//				// 显示吃的动画（东家）
//				// 将吃完的牌放到其左手边（东家）
//			} else if (returnMsgStr.Contains("...")) { // TODO 11 其他家做动作
//			} else { // TODO 11 其他家的逻辑再说
//			} 
		}
	}
}

