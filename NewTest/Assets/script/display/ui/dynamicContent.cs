using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//动态容器基类.非翻页型
public class dynamicContent : MonoBase
{

	public WindowBase fatherWindow;
	const int MOVE_PERLINE = 1;//每次移动几行？
	int moveDistance = 0;//每次移动的距离)
//	int maxCount = 0;//需要翻页的最大元素总数,
	public int lineCount = 4;//一页有几行
	public int cacheLineCountRatio = 5; // 额外缓冲的行数
	public List<GameObject> nodeList;
	public int NodeWidth = 190;//左右间距
	public int NodeHeight = 240;//上下间距
	UICenterOnChild center;
	public int perlineCount = 3;//每行几列
	public bool  debugMode = false;
	bool isLoaded = false;
	bool triggerTest = false;//跳转的时候忽略触发
	int triggerLen;
	int itemCount;//物品总数
	
	//这里决定了翻页的时候哪些点移动
	int startIndex = 1;//item的第一个的序号
	int endIndex;//item的第最后的序号
	
	//这里决定了什么时候翻页
	Vector4 orgClip;
	float startY = 0;//开始坐标
	float nextY;//触发下次向下翻页的坐标
	float preY;	//触发下次向上翻页的坐标	
//	float nextDistance;	//触发下次翻页的距离

	bool canFilp;//是否可以翻页
	Vector3 mpos;
	Vector4 mClip;
	bool itemHasChange;//是否有物品更新
	float lastPosY;
	bool _onTop=false;
	bool _onBottom=false;
	void Awake ()
	{
		mClip = gameObject.GetComponent<UIPanel> ().baseClipRegion;
		gameObject.GetComponent<UIPanel> ().cullWhileDragging = true;
		gameObject.GetComponent<UIScrollView> ().iOSDragEmulation = false;
		//得到容器默认开始的位置(NGUI容器默认可能会有偏移)
		startY = transform.localPosition.y;		

	}
	
	public void cleanAll ()
	{
		canFilp = false;
		foreach (Transform each in transform) { 
			Destroy (each.gameObject); 
		} 
		nodeList.Clear();

//
//		gameObject.GetComponent<UIPanel> ().clipOffset = Vector2.zero;
//		transform.localPosition = Vector3.zero;
	}

	public void reLoad (int length)
	{
		reLoad (length, 0);
	}

	public void reLoad (int length, int jumpIndex)
	{
		isLoaded = false;
		
		//cleanAll ();
		StartCoroutine (delayLoad (length, jumpIndex));

	}

	IEnumerator delayLoad (int length, int jumpIndex)
	{
		yield return 1;
		Initialize (length, jumpIndex); 
	}

	void Initialize (int length, int index)
	{
		if (isLoaded == true)
			return;	

		cacheLineCountRatio = 5;

		//满一页+5排
		int maxCount = lineCount * perlineCount + cacheLineCountRatio * perlineCount;	
		itemCount = length;	
		if (nodeList == null)
			nodeList = new List<GameObject> ();
		int count = 0;
		//物品数目过大，就只显示2页先
		if (itemCount > maxCount) {
			count = maxCount;
			canFilp = true;
		} else {
			canFilp = false;
			count = itemCount;
		}


		if (transform.childCount > 0) {

			//多删少补
			int dis = transform.childCount - count;
			if (dis > 0) {
				//多了
				for (int i=0; i<dis; i++) {
					nodeList.Remove (transform.GetChild (0).gameObject);
					DestroyImmediate (transform.GetChild (0).gameObject);
				}
			} else if (dis < 0) {

				for (int i=0; i<Mathf.Abs (dis); i++) {
					nodeList.Add (null);
				}

			}

			//多添的部分要初始化
			for (int i = 0; i<count; i++) {
				initButton (i);
			}

		} else {
			//全加空
			nodeList.Clear();
			for (int i = 0; i<count; i++) {
				nodeList.Add (null);
				initButton (i);
			}

		}

		moveDistance = nodeList.Count / perlineCount;
		jumpToPage (index);
	
	}

	public virtual void jumpToPage (int index)
	{
		gameObject.GetComponent<UIScrollView>().DisableSpring();
		//设置翻页触发点
		setTriggerPos (index);
		//调整所有按钮的位置
		offsetAllObj ();
		//更新按钮数据
		updateVisibleItem ();

		//调整clip
		if (index/perlineCount == 0) {
			transform.localPosition = new Vector3 (transform.localPosition.x, startY, transform.localPosition.z);
			gameObject.GetComponent<UIPanel> ().clipOffset = new Vector2 (gameObject.GetComponent<UIPanel> ().clipOffset.x, -startY);
		} else if (index/perlineCount == (itemCount-1)/perlineCount) {
			//跳到最后一页的第一个的位置
			//float offset = startY + NodeHeight * (index - lineCount+1);
			float offset =startY+(index/perlineCount)*NodeHeight-mClip.w*0.5f;
			transform.localPosition = new Vector3 (transform.localPosition.x, offset, transform.localPosition.z);
			gameObject.GetComponent<UIPanel> ().clipOffset = new Vector2 (gameObject.GetComponent<UIPanel> ().clipOffset.x, -offset);
		} else{
			float offset =startY+index/perlineCount*NodeHeight-(mClip.w-NodeHeight)*0.5f;
			transform.localPosition = new Vector3 (transform.localPosition.x, offset, transform.localPosition.z);
			gameObject.GetComponent<UIPanel> ().clipOffset = new Vector2 (gameObject.GetComponent<UIPanel> ().clipOffset.x, -offset);

		}
		//	centerOn (index/perlineCount);
		
		/*
		if (GuideManager.Instance.isEqualStep (7003000) || GuideManager.Instance.isEqualStep (8004000) || GuideManager.Instance.isEqualStep (8007000)
			|| GuideManager.Instance.isEqualStep (9004000) || GuideManager.Instance.isEqualStep (12006000) || GuideManager.Instance.isEqualStep (13003000)
			|| GuideManager.Instance.isEqualStep (20003000) || GuideManager.Instance.isEqualStep (103003000) || GuideManager.Instance.isEqualStep (126003000)
			|| GuideManager.Instance.isEqualStep (120003000) || GuideManager.Instance.isEqualStep (129001000) || GuideManager.Instance.isEqualStep (124006000)
			|| GuideManager.Instance.isEqualStep (102004000) || GuideManager.Instance.isEqualStep (106005000) || GuideManager.Instance.isEqualStep (106009000)
		    || GuideManager.Instance.isEqualStep (126004000) || GuideManager.Instance.isEqualStep (133003000) || GuideManager.Instance.isEqualStep (121003000)) {
			GuideManager.Instance.guideEvent ();
		}
		 */

		isLoaded = true;
		//跳转的话忽略触发
		triggerTest = true;
	}

	public virtual void jumpToPos (float y) {
		if (itemCount <= lineCount) {
			y = 0;
		}
		else {
			float maxY = (itemCount - 1) * NodeHeight - ((mClip.w) - NodeHeight);
			if (maxY < y) {
				y = maxY;
			}
		}
		
		gameObject.GetComponent<UIScrollView> ().DisableSpring ();
		setTriggerPos (getPageIndex(y));
		offsetAllObj ();
		updateVisibleItem ();

		transform.localPosition = new Vector3 (transform.localPosition.x, y, transform.localPosition.z);
		gameObject.GetComponent<UIPanel> ().clipOffset = new Vector2 (gameObject.GetComponent<UIPanel> ().clipOffset.x, -y);

		isLoaded = true;
		triggerTest = true;
	}

	/// <summary>
	/// 
	/// </summary>
	public virtual int getPageIndex () {
		return getPageIndex(transform.localPosition.y);
	}

	public virtual int getPageIndex (float y) {
		return (int)((y + (mClip.w - NodeHeight) * 0.5f) / (perlineCount * NodeHeight) * perlineCount);
	}
	

	//排列跳转目标处的物体
	void offsetAllObj ()
	{
		int count=0;
		int height=0;
		int line=startIndex/perlineCount;

	//第一个的x便宜,如果最后一个不是一排里的最后一个,那么startindex指向的那个就可能不是从排头开始
		if(endIndex==itemCount-1 && itemCount%perlineCount!=0 && canFilp)
			count=itemCount%perlineCount;

		for (int i=0; i<nodeList.Count; i++) {



			if(count<=perlineCount-1){
				nodeList [i].transform.localPosition = new Vector3 (count*NodeWidth, -(height+line) * NodeHeight, nodeList [i].transform.localPosition.z);
			}else{
				count=0;
				height+=1;
				nodeList [i].transform.localPosition = new Vector3 (count*NodeWidth, -(height+line) * NodeHeight, nodeList [i].transform.localPosition.z);
			}
		
		
			nodeList [i].name = StringKit.intToFixString (startIndex + i + 1);
			count+=1;

		}
	}

	//计算上下翻页的触发点
	void setTriggerPos (int lookAtIndex)
	{

		if (!canFilp) {
		
			startIndex = 0;
			endIndex = itemCount - 1;
			preY = -int.MaxValue;
			nextY = int.MaxValue;
			return;
		}
		//总行数 /  2 得到触发点AB长度
		triggerLen = Mathf.Clamp (Mathf.CeilToInt (nodeList.Count / perlineCount) / 2, 1, int.MaxValue);
		
		int e = (int)(triggerLen * 0.5f);
		int s = triggerLen - e;

		int line=lookAtIndex/perlineCount;


		if (line - s - 1 <= 0) {
			//从0开始
			startIndex = 0;
			preY = -int.MaxValue;
			nextY = NodeHeight * (triggerLen - 1) * MOVE_PERLINE;
			endIndex = startIndex + nodeList.Count - 1;
		} else if (line + e + 1 >= itemCount/perlineCount - 1) {
			//到头的情况
			startIndex = itemCount - nodeList.Count;
			preY = NodeHeight * (Mathf.Ceil(itemCount/(float)perlineCount)  - triggerLen - 1) * MOVE_PERLINE;
			nextY = int.MaxValue;
			endIndex =itemCount-1;
		} else {
			//中间情况
		//	startIndex = lookAtIndex - (int)(nodeList.Count * 0.5f);
			//开始的行数*每行几个 得到那行开头的第一个
			startIndex=(line- (int)(nodeList.Count/perlineCount/2))*perlineCount;
			endIndex = startIndex + nodeList.Count - 1;
			preY =startY+ NodeHeight * (line -2) * MOVE_PERLINE-mClip.z*0.5f;
			nextY =startY+ NodeHeight * line * MOVE_PERLINE-mClip.z*0.5f;	
		}

		if (debugMode) {
			print ("jumpToIndex: " + lookAtIndex);
			print ("jumpToline: " + line);
			print ("startIndex:  " + startIndex);
			print ("endIndex:  " + endIndex);
			print ("trigetAB:  " + triggerLen);
			print ("preY:  " + preY);
			print ("nextY:  " + nextY);
		}
	}

	public virtual void OnDisable ()
	{
		//bug修正,快速开关导致层没切回来,启动的时候检测一下
		if (isLoaded == true)
			gameObject.layer = UiManager.UILAYER;	
	}

	public virtual void initButton (int  i)
	{

	}

	public virtual void  onStop ()
	{

	}

	public virtual void OnUpdate ()
	{


	}


	int _state;
	public  void _OnBorder(int  state){

//		if (debugMode)
//			print ("OnTop");

		if(_state==state)
			return;

		_state=state;
		OnBorder(state);

	}
	public virtual void OnBorder(int  state){



	}

	void Update ()
	{

		OnUpdate ();
		//没必要翻页return
		if (canFilp == false || triggerTest == false)
			return;
		
		if (lastPosY == transform.localPosition.y && itemHasChange == true) {
			onStop ();
			itemHasChange = false;
		}
		lastPosY = transform.localPosition.y;

		if(lastPosY<=0 && !_onTop){
			_onTop=true;
			_OnBorder(-1);
		}else if(lastPosY>=((itemCount-lineCount)/perlineCount-1)*NodeHeight && !_onBottom){
			_onBottom=true;
			_OnBorder(1);

		}else if(lastPosY>0 && _onTop){
			_OnBorder(0);
			_onTop=false;
		}else if(lastPosY<((itemCount-lineCount)/perlineCount-1)*NodeHeight &&  _onBottom){
			_OnBorder(0);
			_onBottom=false;
		}



		if (transform.localPosition.y < preY) {
			//上翻页触发
			if (startIndex <= 0) {
				nextY = NodeHeight * (triggerLen - 1) * MOVE_PERLINE;
				preY = -int.MaxValue;

				if (debugMode)
					print ("Begin nextY:  " + nextY);
				return;
			}	

			if (preY - NodeHeight * MOVE_PERLINE < startY){
				return;
			}
			nextY = preY;
			preY = preY - NodeHeight * MOVE_PERLINE;

			dragFinish (-1);
		}	
		
		if (transform.localPosition.y > nextY) {
			//下翻页触发
			if (endIndex/perlineCount >= (itemCount-1)/perlineCount) {
				preY = NodeHeight * (Mathf.Ceil(itemCount/(float)perlineCount) - triggerLen - 1) * MOVE_PERLINE;
				nextY = int.MaxValue;

				if (debugMode)
					print ("END preY:  " + preY);
				return;
			}
			
			
			preY = nextY;
			nextY = nextY + NodeHeight * MOVE_PERLINE;
			dragFinish (1);
		}
		
	}

	public virtual void  updateItem (GameObject item, int index)
	{
		//	item.GetComponent<ButtonBase> ().textLabel.text = index + "";
	}
	
	public void updateVisibleItem ()
	{
		for (int i=0; i< transform.childCount; i++) {
			updateItem (transform.GetChild(i).gameObject,StringKit.toInt( transform.GetChild(i).name)-1);
		}
	}

	public void dragFinish (int side)
	{	
		if (side == -1) {
			//往上翻页
			int moveNum=MOVE_PERLINE*perlineCount;
			//最后一页先移动不满一页的
			if(endIndex>=itemCount-1 && perlineCount>1){
				int less=itemCount%perlineCount;
				if(less!=0){
					moveNum=less;
				}
			}
			for (int i=0; i<moveNum; i++) {
				GameObject item = null;
				string str = StringKit.intToFixString (endIndex + 1 - i);
				Transform tran = transform.FindChild (str);
				if (tran == null)
					continue;
				item = tran.gameObject;
				item.transform.localPosition = new Vector3 (item.transform.localPosition.x, item.transform.localPosition.y + NodeHeight * moveDistance, item.transform.localPosition.z);
				item.name = StringKit.intToFixString (startIndex - i);
				updateItem (item, startIndex -1 - i);

			}
			endIndex -= moveNum;
			startIndex -= moveNum;
			itemHasChange = true;
		}
		if (side == 1) { 
			//往下翻页

			int moveNum=0;
			for (int i=0; i<MOVE_PERLINE*perlineCount; i++) {
				GameObject item = null;
				if ((endIndex + 1 + i) >= itemCount) {
					break;
				}
				moveNum+=1;
				string str = StringKit.intToFixString (startIndex + 1 + i);
				Transform tran = transform.FindChild (str);
				if (tran == null)
					continue;
				item = tran.gameObject;
				item.transform.localPosition = new Vector3 (item.transform.localPosition.x, item.transform.localPosition.y - NodeHeight * moveDistance, item.transform.localPosition.z);
				item.name = StringKit.intToFixString (endIndex + 2 + i);
				updateItem (item, endIndex +1+ i);

			}
			endIndex += moveNum;
			startIndex += moveNum;
			itemHasChange = true;

		} 
		if (debugMode) {
			print ("dragFinish - startIndex:  " + startIndex);
			print ("dragFinish - endIndex:  " + endIndex);
			print ("preY" + preY);
			print ("nextY:  " + nextY);
		}
	}
	
}
