using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentBase : MonoBase
{
	
	public	UICenterOnChild  UICenter;
	int maxItemNum = -1;
	protected int maxPage = -1;
	public bool ignoreChild = true;
	public int pageChildCount = 1;//每页里有多少个元素
	public int pageWidth;
	public int pageHeight;
	public bool loaded = false;
	public int pageCount = 0;
	public GameObject[] pageList;
	public GameObject activeGameObj;
	public	RadioScrollBar ScrollBar;
	public WindowBase fatherWindow;
	UIPanel contentPanel;
	bool	buttonLoad = false;
	Vector3 mpos;
	// saving page's position to restore
//	protected Vector3 page2_locPosition;
//	protected Vector3 page3_locPosition;

	void Awake ()
	{
		contentPanel = gameObject.GetComponent<UIPanel> ();
		contentPanel.cullWhileDragging = true;
		mpos=contentPanel.transform.localPosition;
	}

	public void setMaxNum (int num)
	{
		maxItemNum = num;
		if (num != -1) {
			if (num != 0) {
				float b = (float)num;
				float a = (float)pageChildCount;
				if (a == 0)
					a = 1;
				maxPage = (int)Mathf.Ceil (b / a);
			} else {
				
				maxPage = 0;
			}
		} else {
			
			maxPage = -1;	
		}
		
		
		
		if (loaded == true && ScrollBar != null) {
		
			if (maxPage != ScrollBar.pageCount)
				ScrollBar.Init(maxPage);
			if (activeGameObj != null && maxPage != 0)
				ScrollBar.check ((int)StringKit.toInt (activeGameObj.name));
		
		}
	}
	
	public void hideAllButton ()
	{
		 
		contentPanel.alpha = 0;
		
	}

	public void showAllButton ()
	{
		
		contentPanel.alpha = 1;
	}
	
	public  virtual void initAllButton (GameObject each)
	{
		
		
		
	}
	
	public  void _initAllButton ()
	{
 
		if (ignoreChild == true) {
			//如果是单页只更新那3页
			foreach (Transform each in transform) {
		 
			
				initAllButton (each.gameObject);
			
			}
			
			return;
 
			
		}
		
		
		if (buttonLoad == true)
			return;
		foreach (Transform each in transform) {
			foreach (Transform each2 in each) {
			
				initAllButton (each2.gameObject);
			
			}
			
 
		}
		
		buttonLoad = true;
		
	}

	public void updateAfterIndex (GameObject button)
	{
			
		//find who's him father
	
		
		GameObject father = null;
		
		if (pageList [0] != null && button.transform.IsChildOf (pageList [0].transform))
			father = pageList [0];
		if (pageList [1] != null && button.transform.IsChildOf (pageList [1].transform))
			father = pageList [1];
		if (pageList [2] != null && button.transform.IsChildOf (pageList [2].transform))
			father = pageList [2];
		
		if (father == null)
			return;
		
		//find him index
		
		int _pageCount = (int)StringKit.toInt (father.name);
		

		
		//update
		Transform T_nextPage = transform.FindChild (formatNum (_pageCount));
		if (T_nextPage != null) {
			if ((int)StringKit.toInt (T_nextPage.name) > maxPage) {
				Destroy (T_nextPage.gameObject);
				if (T_nextPage.name != "001") {
					GetComponent<UIScrollView> ().MoveRelative (new Vector3 (pageWidth, 0, 0));
				}
			} else
			
				updateContent (T_nextPage.gameObject, _pageCount);
		}
		
		T_nextPage = transform.FindChild (formatNum (_pageCount + 1));

		if (T_nextPage != null) {		
			if (StringKit.toInt (T_nextPage.name) > maxPage) {
				Destroy (T_nextPage.gameObject);
				GetComponent<UIScrollView> ().MoveRelative (new Vector3 (1f, 0, 0));
			} else
				updateContent (T_nextPage.gameObject, _pageCount + 1);
		}
		
		T_nextPage = transform.FindChild (formatNum (_pageCount + 2));

		if (T_nextPage != null) {
			if (StringKit.toInt (T_nextPage.name) > maxPage) {
				Destroy (T_nextPage.gameObject);
				GetComponent<UIScrollView> ().MoveRelative (new Vector3 (1f, 0, 0));
			} else		
				updateContent (T_nextPage.gameObject, _pageCount + 2);
		}
	}
	
	public void updateAllIndex (int maxnum)
	{
		setMaxNum (maxnum);
		//	print ("here contentbase !!!!");
		int minPageCount = 999999;
		if (pageList [0] != null && StringKit.toInt (pageList [0].name) < minPageCount) 
			minPageCount = StringKit.toInt (pageList [0].name);
		if (pageList [1] != null && StringKit.toInt (pageList [1].name) < minPageCount)
			minPageCount = StringKit.toInt (pageList [1].name);
		if (pageList [2] != null && StringKit.toInt (pageList [2].name) < minPageCount)
			minPageCount = StringKit.toInt (pageList [2].name);
		
		Transform T_nextPage = transform.FindChild (formatNum (minPageCount));
		if (T_nextPage != null)
			updateContent (T_nextPage.gameObject, minPageCount);
		
		T_nextPage = transform.FindChild (formatNum (minPageCount + 1));
		if (T_nextPage != null)
			updateContent (T_nextPage.gameObject, minPageCount + 1);
		
		T_nextPage = transform.FindChild (formatNum (minPageCount + 2));
		if (T_nextPage != null)
			updateContent (T_nextPage.gameObject, minPageCount + 2);
			
	}


	public void jumpToPage (int pageNum)
	{
		if (pageNum > maxPage)
			return;

		//总页只有1，没必要跳
		if (maxPage == 1)
			return;
		//两页跳到第一页也很无聊
		if (maxPage == 2 && pageNum == 1)
			return;
		
		//最后一页和第一页特殊处理
		Destroy (contentPanel.gameObject.GetComponent<SpringPanel> ());
		
	 
		
			 
		//如果只有两页, 只可能跳到第二页
		if (maxPage == 2 && pageNum == 2) {
			pageList [0].name = formatNum (pageNum - 1);
			pageList [1].name = formatNum (pageNum);
			pageList [2].name = "none";
				
			pageList [0].transform.localPosition = new Vector3 (pageWidth * (pageNum - 2), pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);	
			pageList [1].transform.localPosition = new Vector3 (pageWidth * (pageNum - 1), pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);			

			updateContent (pageList [1], pageNum);
			updateContent (pageList [0], pageNum - 1);
			activeGameObj = pageList [1];
			updateActivePage ();
			
			contentPanel.transform.localPosition = new Vector3 (mpos.x+-pageWidth * (pageNum - 1), contentPanel.transform.localPosition.y, contentPanel.transform.localPosition.z);
			contentPanel.clipOffset = new Vector4 (pageWidth * (pageNum - 1), contentPanel.clipOffset.y);
			pageCount = pageNum;
			
			if (ScrollBar != null)
				ScrollBar.check ((int)StringKit.toInt (pageList [1].name));
			return;
		} 
			
			
			
		//其他多页的情况
		
		
		if (pageNum == maxPage) {
		

			pageList [0].name = formatNum (pageNum - 2);
			pageList [1].name = formatNum (pageNum - 1);
			pageList [2].name = formatNum (pageNum);

		
			pageList [2].transform.localPosition = new Vector3 (pageWidth * pageNum, pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);			
			pageList [1].transform.localPosition = new Vector3 (pageWidth * (pageNum - 1), pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);	
			pageList [0].transform.localPosition = new Vector3 (pageWidth * (pageNum - 2), pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);	
			


			updateContent (pageList [2], pageNum);
			updateContent (pageList [1], pageNum - 1);
			updateContent (pageList [0], pageNum - 2);
			activeGameObj = pageList [2];
			updateActivePage ();

			
		} else if (pageNum == 1) {
			 
			pageList [0].name = formatNum (pageNum);
			pageList [1].name = formatNum (pageNum + 1);
			pageList [2].name = formatNum (pageNum + 2);
			
			pageList [2].transform.localPosition = new Vector3 (pageWidth * (pageNum + 2), pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);			
			pageList [1].transform.localPosition = new Vector3 (pageWidth * (pageNum + 1), pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);	
			pageList [0].transform.localPosition = new Vector3 (pageWidth * pageNum, pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);	
			

			updateContent (pageList [2], pageNum + 2);
			updateContent (pageList [1], pageNum + 1);
			updateContent (pageList [0], pageNum);
			activeGameObj = pageList [0];			
			updateActivePage ();
			
		} else {
			 
			pageList [0].name = formatNum (pageNum - 1);
			pageList [1].name = formatNum (pageNum);
			pageList [2].name = formatNum (pageNum + 1);	
			
			pageList [2].transform.localPosition = new Vector3 (pageWidth * (pageNum + 1), pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);			
			pageList [1].transform.localPosition = new Vector3 (pageWidth * pageNum, pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);	
			pageList [0].transform.localPosition = new Vector3 (pageWidth * (pageNum - 1), pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);	
			

			updateContent (pageList [2], pageNum + 1);
			updateContent (pageList [1], pageNum);
			updateContent (pageList [0], pageNum - 1);
			
 
			activeGameObj = pageList [1];
			updateActivePage ();
		}
		contentPanel.transform.localPosition = new Vector3 (mpos.x+-pageWidth * pageNum, contentPanel.transform.localPosition.y, contentPanel.transform.localPosition.z);
		contentPanel.clipOffset = new Vector4 (pageWidth * pageNum, contentPanel.clipOffset.y);
		pageCount = pageNum;
		
		if (ScrollBar != null)
			ScrollBar.check ((int)StringKit.toInt (activeGameObj.name));
	}
	
	public void init (int num)
	{
		bool hidePage3, hidePage2, hidePage1;
		
		if (loaded == true)
			return;

		
		setMaxNum (num);
		pageList = new GameObject[3];
		
		//初始化3个页面

			
		if (transform.FindChild ("001") != null && transform.FindChild ("002") &&
			transform.FindChild ("003") != null) {
			pageList [0] = transform.FindChild ("001").gameObject;
			pageList [1] = transform.FindChild ("002").gameObject;
			pageList [2] = transform.FindChild ("003").gameObject;
		} else {		
			for (int i = 0; i < 3; i++) {
				pageList [i] = transform.GetChild (i).gameObject;
				pageList [i].name = "00" + (i + 1).ToString ();
			}
		}
	 

	
		hidePage2 = false;
		hidePage3 = false;
		hidePage1 = false;
		
		
		
		//如果不是无限页数
		if (num != -1) {
			if (maxPage < 3 && pageList [2].transform.localScale != Vector3.zero) {
				//	pageList [2].transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
				//page3_locPosition = pageList [2].transform.localPosition;
				pageList [2].transform.localPosition = pageList [0].transform.localPosition + new Vector3 (0, 3000, 0);
				pageList [2].name = "none";
				hidePage3 = true;
			}
			if (maxPage < 2 && pageList [1].transform.localScale != Vector3.zero) {
				//	pageList [1].transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
				//page2_locPosition = pageList [1].transform.localPosition;
				pageList [1].transform.localPosition = pageList [0].transform.localPosition + new Vector3 (0, 3000, 0);
				pageList [1].name = "none";
				hidePage2 = true;
			}
			if (maxPage < 1 && pageList [0].transform.localScale != Vector3.zero) {
				//	pageList [0].transform.localPosition = new Vector3 (0.001f, 0.001f, 0.001f);
				hidePage1 = true;
				pageList [0].name = "none";
			}
		}
		// check if need to reset
		if (hidePage3 == false) {
			pageList [2].transform.localScale = Vector3.one;
		
			//pageList[2].transform.localPosition = page3_locPosition;
		} else {
			pageList [2].transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
		}
		if (hidePage2 == false) {
			pageList [1].transform.localScale = Vector3.one;
			
			//pageList[1].transform.localPosition = page2_locPosition;
		} else {
			pageList [1].transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
		}
		if (hidePage1 == false) {
			pageList [0].transform.localScale = Vector3.one;
		
		} else {
			pageList [0].transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
		}
		
		

		
		
		
		_initAllButton ();
		
		if (pageList [1] != null) {
			if (hidePage2 == false) {
				pageList [1].transform.localPosition = new Vector3 (pageList [0].transform.localPosition.x + pageWidth, pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);
				updateContent (pageList [1], 2);
			}
		}
			
		if (pageList [2] != null) {
			if (hidePage3 == false) {
				pageList [2].transform.localPosition = new Vector3 (pageList [1].transform.localPosition.x + pageWidth, pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);
				updateContent (pageList [2], 3);
			}
		}
			
		if (ScrollBar != null) 
			ScrollBar.Init(maxPage);
			
		loaded = true;
		
		UICenter.Recenter ();
		
		if (pageCount == 1)	
			updateContent (pageList [0], 1);

		contentPanel.depth += 1;
	}

	/// <summary>
	/// Go back to first page and update first 3 pages.
	/// Compare with init(), ReloadItems don't create new materials. init only can call once.
	/// Compare withe updateAllPages(), updateAllPages updates current 3 pages.
	/// </summary>
	public void ReloadItems (int num)
	{
		bool hidePage3, hidePage2, hidePage1;
		
		// Move to first page
		this.GetComponent<UIScrollView> ().MoveRelative (new Vector3 (pageWidth * pageCount, 0, 0));
		pageCount = 1;
		
		setMaxNum (num);
		pageList = new GameObject[3];
		
		
		if (transform.FindChild ("001") != null && transform.FindChild ("002") &&
			transform.FindChild ("003") != null) {
			pageList [0] = transform.FindChild ("001").gameObject;
			pageList [1] = transform.FindChild ("002").gameObject;
			pageList [2] = transform.FindChild ("003").gameObject;
		} else {		
			for (int i = 0; i < 3; i++) {
				pageList [i] = transform.GetChild (i).gameObject;
				pageList [i].name = "00" + (i + 1).ToString ();
			}
		}
		
		hidePage2 = false;
		hidePage3 = false;
		hidePage1 = false;
		
		if (num != -1) {
			if (maxPage < 3) {
				pageList [2].transform.localPosition = pageList [0].transform.localPosition + new Vector3 (0, 3000, 0);
				hidePage3 = true;
			}

			if (maxPage < 2) {
				pageList [1].transform.localPosition = pageList [0].transform.localPosition + new Vector3 (0, 3000, 0);
				hidePage2 = true;
			}

			if (maxPage < 1) {
				hidePage1 = true;
			}
		}
		// check if need to reset
		if (hidePage3 == false) {
			pageList [2].transform.localScale = Vector3.one;
		} else {
			pageList [2].transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
		}
		
		if (hidePage2 == false) {
			pageList [1].transform.localScale = Vector3.one;

		} else {
			pageList [1].transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
			
		}
		if (hidePage1 == false) {
			pageList [0].transform.localScale = Vector3.one;
		} else {
			pageList [0].transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
		}
		
		
		if (pageList [1] != null) {
			if (hidePage2 == false) {
				pageList [1].transform.localPosition = new Vector3 (pageList [0].transform.localPosition.x + pageWidth, pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);
				updateContent (pageList [1], 2);
			}
		}
			
		if (pageList [2] != null) {
			if (hidePage3 == false) {
				pageList [2].transform.localPosition = new Vector3 (pageList [1].transform.localPosition.x + pageWidth, pageList [0].transform.localPosition.y, pageList [0].transform.localPosition.z);
				updateContent (pageList [2], 3);
			}
		}
			
		if (ScrollBar != null) 
			ScrollBar.Init(maxPage);
			
		loaded = true;
		
		
		UICenter.Recenter ();
		
	
		if (pageCount == 1)	
			updateContent (pageList [0], 1);
	}
	
	public bool moveDown ()
	{
		if (StringKit.toInt (activeGameObj.name) >= maxPage)
			return false;
		
		this.GetComponent<UIScrollView> ().MoveRelative (new Vector3 (-pageWidth, 0, 0));
		
		GameObject lastObj = null;
		int _lastIndex = StringKit.toInt (activeGameObj.name);
		_lastIndex += 1;
		
		foreach (GameObject each in pageList) {
			if (each.name == formatNum (_lastIndex)) {
				lastObj = each;
				break;
			}
			
		}
		if (lastObj != null)
			adjuestPage (lastObj);
		return true;
	}
	
	public bool moveUp ()
	{
		 
		if (StringKit.toInt (activeGameObj.name) <= 1)
			return false;
		
		this.GetComponent<UIScrollView> ().MoveRelative (new Vector3 (pageWidth, 0, 0));
		
		//get the last obj
		GameObject lastObj = null;
		int _lastIndex = StringKit.toInt (activeGameObj.name);
		_lastIndex -= 1;
		
		foreach (GameObject each in pageList) {
			if (each.name == formatNum (_lastIndex)) {
				
				lastObj = each;
				break;
			}
			
		}
		if (lastObj != null)
			adjuestPage (lastObj);
		return true;
	}

	public virtual void CreateButton (int index, GameObject page, int buttonIndex)
	{
		
	}
 
	public void adjuestPage (GameObject obj)
	{
		if (activeGameObj == obj)
			return;
		int side = 0;
		if (StringKit.toInt (obj.name) > pageCount)
			side = 1;
		if (StringKit.toInt (obj.name) < pageCount)
			side = -1;
		
		activeGameObj = obj;
		updateActivePage ();
		
		pageCount = StringKit.toInt (obj.name);
		if (ScrollBar != null)
			ScrollBar.check (pageCount);
		
		if (side == 1) {
		
			//updateContent(activeGameObj);
			if (pageCount > maxPage - 1 && maxPage != -1)
				return;
			
			if (pageCount > 2) {
			
				int movepage = pageCount - 2;
			
	 
			
				GameObject movepageObj = transform.FindChild (formatNum (movepage)).gameObject;
			
				if (movepageObj == null) {
					 
					return;
				}
			
				float x = pageWidth + activeGameObj.transform.localPosition.x;
				movepageObj.transform.localPosition = new Vector3 (x, activeGameObj.transform.localPosition.y, activeGameObj.transform.localPosition.z);
			
				movepageObj.name = formatNum (pageCount + 1);
				updateContent (movepageObj, pageCount + 1);
				// if(pageCount==3) 	updateContent(activeGameObj,pageCount);
			} 
			return;
			
		}
		
		if (side == -1) {
		
			//	updateContent(activeGameObj);
			if (pageCount > 1) {
			
				int movepage = pageCount + 2;
			
	 
				Transform e = transform.FindChild (formatNum (movepage));
				
				if (e == null)
					return;	
				
				GameObject movepageObj = e.gameObject;
			
				if (movepageObj == null) {
					 
					return;
				}
				
				float x = activeGameObj.transform.localPosition.x - pageWidth;
				movepageObj.transform.localPosition = new Vector3 (x, activeGameObj.transform.localPosition.y, activeGameObj.transform.localPosition.z);
			
				movepageObj.name = formatNum (pageCount - 1);
				updateContent (movepageObj, pageCount - 1);
				//if(pageCount==3) 	updateContent(activeGameObj,pageCount);
				// if(pageCount==2) 	updateContent(activeGameObj,pageCount);
			}  
			return;
		}
	}
	
	string  formatNum (int movepage)
	{
		string str = "";
			
		if (movepage > 0 && movepage < 10)
			str = "00" + movepage;
		if (movepage > 9 && movepage < 100)
			str = "0" + movepage;
		if (movepage > 99 && movepage < 1000)
			str = "" + movepage;	
		return str;
		
	}
 
	protected void updateContent (GameObject obj, int pageNUm)
	{
//		print ("update::::::::"+obj.name);
		if (ignoreChild == false) {

			for (int i=0; i<obj.transform.childCount; i++) {
				int index = pageChildCount * pageNUm - pageChildCount + i;
				if (index < 0)
					continue;
				if (maxItemNum != -1 && index >= maxItemNum) {
					//print (index+"________"+maxItemNum);
					//	obj.transform.GetChild(i).transform.localScale=Vector3.zero;
					continue;
				}
		
				if (index < 0)
					index = 0;
				
				CreateButton (index, obj, i);
			}
		} else {
			int index = pageNUm - 1;
			CreateButton (index, obj, 0);
		}
 
		
		
	}
	
	public void updateActivePage ()
	{
		
	
		updateActive (activeGameObj, StringKit.toInt (activeGameObj.name));
	}
	
	public virtual void updateActive (GameObject obj, int pageNUm)
	{
		updateContent (activeGameObj, pageNUm);
		
	}
}
