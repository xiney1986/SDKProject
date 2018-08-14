using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 卡片小头像滑动容器
/// </summary>
public class ContentStarSoulEquiTop : dynamicContent {

	/* fields */
	public GameObject itemPerfab;
	public SoulEquiShow soulEquiShow;
	private List<Card> data;
	public ContentShowItem contentShowItem;

	/* methods */
	/***/
	public void init(List<Card> cards) {
		this.data=cards;
		base.reLoad(data.Count);
	}
	/** 初始化button */
	public void initTopButton(int selectIndex){
		if (nodeList.Count > 0) {
			if(selectIndex>nodeList.Count-1) selectIndex=nodeList.Count-1;
			GameObject gameObj=nodeList [selectIndex];
			for(int i=0;i<nodeList.Count;i++){
				GameObject bojj=nodeList[i];
				if(bojj!=null){
					StarSoulTopButtonItem btnn=bojj.GetComponent<StarSoulTopButtonItem>();
					btnn.linkQualityEffectPointByRotate();
					if(i==selectIndex){
						btnn.showEffectByQuality();
					}else{
						btnn.HideEffectByQuality();
					}
				}
			}
			if(nodeList.Count>3) {
				if(selectIndex<=1)
					gameObj=nodeList[2];
				if(gameObj!=null) {
					StarSoulTopButtonItem selectBtn = gameObj.GetComponent<StarSoulTopButtonItem>();
					UIScrollView scrollView = GetComponent<UIScrollView> ();
					if(selectBtn!=null)SpringPanel.Begin (scrollView.gameObject, -selectBtn.transform.localPosition+new Vector3(212f,0f,0f), 5);
				}
			} else {
				UIScrollView scrollView = GetComponent<UIScrollView> ();
				SpringPanel.Begin (scrollView.gameObject, Vector3.zero, 5);
			}
		}
	}
	/** 初始化滑动条中的条目button */
	public override void initButton(int i) {
		if(nodeList[i]==null) {
			nodeList[i]=NGUITools.AddChild(gameObject,itemPerfab);
		}
		StarSoulTopButtonItem item=nodeList[i].GetComponent<StarSoulTopButtonItem>();
		item.fatherWindow=fatherWindow;
		item.contentShowItem=contentShowItem;
	}
	/** 更新滑动条目数据 */
	public override void updateItem(GameObject item,int index) {
		if(data==null||index>=data.Count||data[index]==null)return;
		StarSoulTopButtonItem itemC=item.GetComponent<StarSoulTopButtonItem>();
		itemC.init(data[index]);
	}

	
}