using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 节日许愿动态容器
/// </summary>
public class ContentFestivalWishItem : dynamicContent {

	private List<FestivalWish> festivalWishs;
	public Card chooseCard;
	public GameObject festivalWishItemUIPrefab;
	private CallBack wishUpdate;
	public WindowBase fatherwindow;
	
	public void Initialize (List<FestivalWish> wishs, CallBack callback,WindowBase wb)
	{
		this.festivalWishs = wishs;
		this.wishUpdate = callback;
		this.fatherwindow = wb;
	}
	
	public override void updateItem (GameObject item, int index)
	{ 
		FestivalWishItemUI button = item.GetComponent<FestivalWishItemUI> ();
		button.initItemUI (festivalWishs [index],fatherwindow); 
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null)
		{
			nodeList [i] = NGUITools.AddChild(gameObject, festivalWishItemUIPrefab);
		}
		nodeList [i].name = StringKit. intToFixString (i + 1);
		FestivalWishItemUI button= nodeList [i].GetComponent<FestivalWishItemUI> ();
		button.initItemUI (festivalWishs [i],fatherwindow); 
		
	}
	void OnDisable()
	{
//		cleanAll();
	}
}
