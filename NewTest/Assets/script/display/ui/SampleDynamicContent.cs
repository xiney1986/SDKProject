using UnityEngine;
using System.Collections;

public class SampleDynamicContent : MonoBase
{
	// >2右翻开始

	UICenterOnChild centerChild;
	public CallBack<GameObject> callbackUpdateEach;
	public CallBack onLoadFinish;
	public CallBack<GameObject> onCenterItem;
	public CallBack callbackLeftFilp;
	public CallBack callbackRigthFilp;
	public WindowBase fatherWindow;
	public PageSlider pageSlider;
	int indexNow;
	public int maxCount;
	public int itemSize;
	public int startIndex = 0;
	bool justJump;
	UIPanel mPanel;

	public GameObject getCenterObj ()
	{
		return centerChild.centeredObject;
	}


	void Awake ()
	{
		centerChild = gameObject.GetComponent<UICenterOnChild> ();
		mPanel = gameObject.GetComponent<UIPanel> ();
		if (centerChild != null)
			centerChild.onFinished = adjustPage;



	}

	void hidePage (string name)
	{
		Transform tmp = transform.FindChild (name);
		tmp.localPosition = Vector3.zero;
		tmp.gameObject.SetActive (false);
	}

	public void init ()
	{
		int i=0;
		foreach (Transform each in transform) {
			each.gameObject.SetActive (true);
			each.name=StringKit.intToFixString(i+1);
			each.transform.localPosition = new Vector3 (itemSize*i, each.transform.localPosition.y, each.transform.localPosition.z);
			i+=1;
		}
	

		if (maxCount == 2) {
			hidePage ("003");
		} else if (maxCount == 1) {
			hidePage ("003");
			hidePage ("002");
		} else if (maxCount == 0) {
			hidePage ("003");
			hidePage ("002");
			hidePage ("001");
		} 

		resetPos();

		if (startIndex == 0)
			StartCoroutine (load (transform.GetChild (0).gameObject));
		else
			jumpTo (startIndex);
		
		if(pageSlider!=null)
			pageSlider.initPageSlider(maxCount);

	}

	void OnEnable ()
	{

	}

	IEnumerator load (GameObject obj)
	{
		foreach (Transform each in transform) {
			if (callbackUpdateEach != null && each.gameObject.activeSelf)
				callbackUpdateEach (each.gameObject);
			yield return 0;
		}

		if (onLoadFinish != null)
			onLoadFinish ();

		onCenterItem (obj);

		yield break;
	}

	void rightFlip (int newIndex)
	{
		string childFix = StringKit.intToFixString (newIndex - 1);
		//size must be +-1;
		Transform tmp = transform.FindChild (childFix);
		if (tmp == null) {
			Debug.LogError ("rightFlip name=" + transform.name + ",childFix=" + childFix);
			return;
		}
		tmp.name = StringKit.intToFixString (newIndex + 2);
		tmp.transform.localPosition += new Vector3 (itemSize * 3, 0, 0);

//		print (" flip,move:" + tmp.name);
		
		if (callbackUpdateEach != null){
			callbackUpdateEach (tmp.gameObject);
		}
	}
	
	void leftFlip (int newIndex)
	{
		string childFix = StringKit.intToFixString (newIndex + 3);
		//size must be +-1;
		Transform tmp = transform.FindChild (childFix);
		if (tmp == null) {
			Debug.LogError ("leftFlip name=" + transform.name + ",childFix=" + childFix);
			return;
		}
		tmp.name = StringKit.intToFixString (newIndex);
		tmp.transform.localPosition -= new Vector3 (itemSize * 3, 0, 0);
		
//		print (" flip,move:" + tmp.name);
		
		if (callbackUpdateEach != null){
			callbackUpdateEach (tmp.gameObject);
		}
	}

	public void jumpTo (int index)
	{

		justJump = true;
		Transform obj1 = transform.GetChild (0);
		Transform obj2 = transform.GetChild (1);
		Transform obj3 = transform.GetChild (2);

//		adjustPage(index);

		if(maxCount>2){

		if (index == 0) {


			obj1.localPosition = new Vector3 (0, obj1.transform.localPosition.y, obj1.transform.localPosition.z);
			obj1.name = StringKit.intToFixString (index + 1);
			//obj1.gameObject.SetActive (maxCount < 1 ? false : true);

			obj2.localPosition = new Vector3 (itemSize, obj2.transform.localPosition.y, obj2.transform.localPosition.z);
			obj2.name = StringKit.intToFixString (index + 2);
			//obj2.gameObject.SetActive (maxCount < 2 ? false : true);

			obj3.localPosition = new Vector3 (itemSize * 2, obj3.transform.localPosition.y, obj3.transform.localPosition.z);
			obj3.name = StringKit.intToFixString (index + 3);
			//obj3.gameObject.SetActive (maxCount < 3 ? false : true);

		} else if (index == maxCount - 1 ) {

			obj3.localPosition = new Vector3 (itemSize * index, obj3.transform.localPosition.y, obj3.transform.localPosition.z);
			obj3.name = StringKit.intToFixString (index + 1);
			//obj3.gameObject.SetActive (maxCount < 3 ? false : true);
				
			obj2.localPosition = new Vector3 (itemSize * (index - 1), obj2.transform.localPosition.y, obj2.transform.localPosition.z);
			obj2.name = StringKit.intToFixString (index);
			//obj2.gameObject.SetActive (maxCount < 2 ? false : true);

			obj1.localPosition = new Vector3 (itemSize * (index - 2), obj1.transform.localPosition.y, obj1.transform.localPosition.z);
			obj1.name = StringKit.intToFixString (index - 1);
			//obj1.gameObject.SetActive (maxCount < 1 ? false : true);


		} else {
	
			obj2.localPosition = new Vector3 (itemSize * index, obj2.transform.localPosition.y, obj2.transform.localPosition.z);
			obj2.name = StringKit.intToFixString (index + 1);
			//obj2.gameObject.SetActive (maxCount < 2 ? false : true);

			obj3.localPosition = new Vector3 (itemSize * (index + 1), obj3.transform.localPosition.y, obj3.transform.localPosition.z);
			obj3.name = StringKit.intToFixString (index + 2);
			//obj3.gameObject.SetActive (maxCount < 3 ? false : true);

			obj1.localPosition = new Vector3 (itemSize * (index - 1), obj1.transform.localPosition.y, obj1.transform.localPosition.z);
			obj1.name = StringKit.intToFixString (index);
			//obj1.gameObject.SetActive (maxCount < 1 ? false : true);
		}

		}else{

			obj1.localPosition = new Vector3 (0, obj1.transform.localPosition.y, obj1.transform.localPosition.z);
			obj1.name = StringKit.intToFixString (1);
		//	obj1.gameObject.SetActive (maxCount < 1 ? false : true);		
			obj2.localPosition = new Vector3 (itemSize, obj2.transform.localPosition.y, obj2.transform.localPosition.z);
			obj2.name = StringKit.intToFixString (2);
			//obj2.gameObject.SetActive (maxCount < 2 ? false : true);	
			obj3.localPosition = new Vector3 (itemSize * 2, obj3.transform.localPosition.y, obj3.transform.localPosition.z);
			obj3.name = StringKit.intToFixString (3);
		//	obj3.gameObject.SetActive (maxCount < 3 ? false : true);
		}
		obj1.gameObject.SetActive (maxCount < 1 ? false : true);
		obj2.gameObject.SetActive (maxCount < 2 ? false : true);
		obj3.gameObject.SetActive (maxCount < 3 ? false : true);
		Transform centeritem=transform.FindChild(StringKit.intToFixString(index+1));
			centerOn (centeritem, index);
		indexNow = index;
		StartCoroutine (load (centeritem.gameObject));

		if(pageSlider!=null)
			pageSlider.setActivePage(indexNow+1);
	}

	void centerOn (Transform item, int index)
	{

		transform.localPosition = new Vector3 (-index * itemSize, transform.localPosition.y, transform.localPosition.z);
		mPanel.clipOffset = new Vector2 (index * itemSize, mPanel.clipOffset.y);
		centerChild.CenterOn (item);
	}
	void resetPos ()
	{
		transform.localPosition = new Vector3 (0, transform.localPosition.y, transform.localPosition.z);
		mPanel.clipOffset = new Vector2 (0, mPanel.clipOffset.y);
		centerChild.CenterOn (transform.FindChild("001"));
	}


	public void adjustPage (int newIndex){
		if (onCenterItem != null && newIndex != indexNow)
			onCenterItem (getCenterObj ());
		//直接跳转不触发翻页事件 
		if (justJump == true) {
			justJump = false;
			indexNow = newIndex;
			return;
		}
		if(pageSlider!=null)
			pageSlider.setActivePage(newIndex+1);
		
		if (newIndex > indexNow) {
			//右翻
			if (newIndex > 1) {
				//右翻开始
				if (newIndex > maxCount - 2) {
					if(callbackRigthFilp!=null) {
						int tempMaxCount=maxCount;
						callbackRigthFilp();
						if(tempMaxCount < maxCount) {
							rightFlip (newIndex);
						}
					}
				} else {
					rightFlip (newIndex);
				}
			}
		} else if (newIndex < indexNow) {
			
			if (newIndex < maxCount - 2) {
				if (newIndex < 1) {
					if(callbackLeftFilp!=null) {
						int tempMaxCount=maxCount;
						callbackLeftFilp();
						if(tempMaxCount > maxCount) {
							leftFlip (newIndex);
						}
					}
				} else {
					leftFlip (newIndex);
				}
			}
			
		}
		indexNow = newIndex;
	}
	public void adjustPage ()
	{
		string name = centerChild.centeredObject.name;
		int newIndex = StringKit.toInt (name) - 1;
		adjustPage(newIndex);
	}
}
