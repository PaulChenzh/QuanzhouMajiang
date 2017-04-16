using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class Loading : MonoBehaviour {  

	private float fps = 10.0f;  
	private float time;  
	//一组动画的贴图，在编辑器中赋值。  
	public Texture2D[] animations;  
	private int nowFram;  
	//异步对象  
	AsyncOperation async;  

	//读取场景的进度，它的取值范围在0 - 1 之间。  
	int progress = 0;  

	void Start()  
	{  
		//在这里开启一个异步任务，  
		//进入loadScene方法。  
		StartCoroutine(loadScene());  
	}  

	//注意这里返回值一定是 IEnumerator  
	IEnumerator loadScene()  
	{  
		//yield return new WaitForEndOfFrame();//<strong>加上这么一句就可以先显示加载画面然后再进行加载</strong>  
		async = Application.LoadLevelAsync("Menu"); 
		//读取完毕后返回， 系统会自动进入C场景  
		yield return async;  

	}  

	void OnGUI()  
	{  
		//因为在异步读取场景，  
		//所以这里我们可以刷新UI  
		DrawAnimation(animations);  

	}  

	void Update()  
	{  

		//在这里计算读取的进度，  
		//progress 的取值范围在0.1 - 1之间， 但是它不会等于1  
		//也就是说progress可能是0.9的时候就直接进入新场景了  
		//所以在写进度条的时候需要注意一下。  
		//为了计算百分比 所以直接乘以100即可  
		progress =  (int)(async.progress *100);  

		//有了读取进度的数值，大家可以自行制作进度条啦。  
		Debug.Log("xuanyusong" +progress);  

	}  
	//这是一个简单绘制2D动画的方法，没什么好说的。  
	void   DrawAnimation(Texture2D[] tex)  
	{  

		time += Time.deltaTime;  

		if(time >= 1.0 / fps){  

			nowFram++;  

			time = 0;  

			if(nowFram >= tex.Length - 1)  
			{  
				nowFram = 0;  
			}  
		}  
//		GUI.DrawTexture(new Rect( 100,100,40,60) ,tex[nowFram] );  

		//在这里显示读取的进度。  
		GUI.Label(new Rect( 100,180,300,60), "lOADING!!!!!" + progress);  

	}  

//
//	//异步对象  
//	AsyncOperation asyncOperation;  
//	void Start () {  
//		DontDestroyOnLoad(gameObject);  
//	}  
//
//	void OnGUI(){  
//		//开始加载场景按钮  
//		if(GUILayout.Button("Start Load Scene")){  
//			StartCoroutine("loadScene","Game");  
//		}  
//
//		//判断异步对象并且异步对象没有加载完毕，显示进度  
//		if(asyncOperation!=null&&!asyncOperation.isDone){  
//			GUILayout.Label("progress:"+(float)asyncOperation.progress*100+"%");  
//		}  
//	}  
//
//	IEnumerator loadScene(string sceneName){  
//		yield return asyncOperation = Application.LoadLevelAsync(sceneName);  
//		print("load Complete!");  
//	} 

}  